namespace Atc.Test;

/// <summary>
/// Data attribute combining <see cref="MemberDataAttributeBase"/> semantics with AutoFixture + NSubstitute specimen generation.
/// </summary>
/// <remarks>
/// 1. The referenced member (field / property / method) must return a type assignable to <c>IEnumerable&lt;object?[]&gt;</c>.<br/>
/// 2. Supplied row values are appended with generated specimens for any remaining test method parameters.<br/>
/// 3. Parameters decorated with <see cref="FrozenAttribute"/> participate in a two-phase reuse model:
///    <list type="number">
///      <item><b>Direct positional reuse</b>: If the row already supplies a value at the frozen parameter's index, that instance is injected (frozen) into the fixture.</item>
///      <item><b>Promotion</b>: If the frozen parameter appears later (no supplied value at its index yet) an earlier supplied argument whose runtime type is assignable to the frozen parameter type is promoted and injected.</item>
///    </list>
/// This mirrors the behavior of <c>ClassAutoNSubstituteDataAttribute</c> while extending it with the promotion scenario common in member data ordering.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MemberAutoNSubstituteDataAttribute : MemberDataAttributeBase
{
    public MemberAutoNSubstituteDataAttribute(string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        MemberName = memberName;
        Parameters = parameters;
    }

    /// <summary>
    /// Gets the member name passed to the constructor.
    /// </summary>
    public new string MemberName { get; }

    /// <summary>
    /// Gets the parameter values passed to the constructor (for method members).
    /// </summary>
    public object[] Parameters { get; }

    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod,
        DisposalTracker disposalTracker)
    {
        // Retrieve the original member data rows (without any AutoFixture augmentation yet).
        var baseRows = await base.GetData(testMethod, disposalTracker).ConfigureAwait(false);
        var parameters = testMethod.GetParameters();
        var augmented = new List<ITheoryDataRow>(baseRows.Count);

        // Pre-compute an injector tailored to the frozen parameters of this method.
        var frozenInjector = BuildFrozenInjector(parameters);

        foreach (var row in baseRows)
        {
            var data = row.GetData(); // The raw supplied data (could be shorter than parameter list)
            var fixture = FixtureFactory.Create(); // Fresh fixture per row for isolation

            // Apply frozen injections (positional + promotions) before generating remaining specimens.
            frozenInjector(data, fixture);

            var extendedData = data
                .Concat(parameters
                    .Skip(data.Length)
                    .Select(p => GetSpecimen(fixture, p)))
                .ToArray();

            augmented.Add(new TheoryDataRow(extendedData)
            {
                Explicit = row.Explicit,
                Label = row.Label,
                Skip = row.Skip,
                TestDisplayName = row.TestDisplayName,
                Timeout = row.Timeout,
                Traits = row.Traits ?? new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase),
            });
        }

        return augmented;
    }

    private static Action<object?[], IFixture> BuildFrozenInjector(ParameterInfo[] parameters)
    {
        // Identify parameters decorated with [Frozen]; capture index + type for later injection/promotion.
        var frozenParameters = parameters
            .Select((p, i) => (Index: i, Type: p.ParameterType, Frozen: p.GetCustomAttribute<FrozenAttribute>()))
            .Where(x => x.Frozen is not null)
            .ToArray();

        if (frozenParameters.Length == 0)
        {
            // Fast path: no frozen parameters -> no-op.
            return static (_, _) => { };
        }

        var injectMethod = typeof(FixtureRegistrar).GetMethod(
            nameof(FixtureRegistrar.Inject),
            BindingFlags.Public | BindingFlags.Static);

        return (suppliedData, fixture) =>
        {
            // Phase 1: Direct positional injections for frozen parameters already covered by supplied row data.
            foreach (var frozen in frozenParameters)
            {
                if (suppliedData.Length > frozen.Index)
                {
                    injectMethod?
                        .MakeGenericMethod(frozen.Type)
                        .Invoke(null, [fixture, suppliedData[frozen.Index]]);
                }
            }

            // Phase 2: Promotions – for frozen parameters whose index is beyond supplied data length,
            // attempt to reuse an earlier compatible supplied argument (interface / base type friendly).
            foreach (var frozen in frozenParameters)
            {
                if (suppliedData.Length <= frozen.Index)
                {
                    var promoted = suppliedData.FirstOrDefault(d => d is not null && frozen.Type.IsInstanceOfType(d));
                    if (promoted is not null)
                    {
                        injectMethod?
                            .MakeGenericMethod(frozen.Type)
                            .Invoke(null, [fixture, promoted]);
                    }
                }
            }
        };
    }

    private static object GetSpecimen(
        IFixture fixture,
        ParameterInfo parameter)
    {
        // Gather parameter-level customization sources (e.g. [Frozen], [Greedy], etc.)
        var attributes = parameter
            .GetCustomAttributes()
            .OfType<IParameterCustomizationSource>()
            .OrderBy(x => x is FrozenAttribute);

        foreach (var attribute in attributes)
        {
            // Each customization mutates the fixture before resolving the specimen.
            attribute
                .GetCustomization(parameter)
                .Customize(fixture);
        }

        // Resolve the final specimen through AutoFixture's pipeline honoring prior customizations.
        return new SpecimenContext(fixture)
            .Resolve(parameter);
    }
}