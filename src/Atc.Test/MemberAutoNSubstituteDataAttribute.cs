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

    /// <summary>
    /// Produces the final theory data rows by merging member-supplied data with AutoFixture generated specimens.
    /// </summary>
    /// <param name="testMethod">The target test method whose parameters drive specimen generation.</param>
    /// <param name="disposalTracker">xUnit disposal tracker (unused directly here but required by override).</param>
    /// <returns>Augmented data rows containing original values plus generated specimens.</returns>
    /// <remarks>
    /// Processing for each source row:
    /// 1. Create an isolated fixture.
    /// 2. Apply frozen injection logic (positional + promotion) before resolving additional parameters.
    /// 3. Generate remaining parameters using <see cref="GetSpecimen(IFixture, ParameterInfo)"/>.
    /// 4. Preserve original row metadata (label, skip, etc.).
    /// </remarks>
    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod,
        DisposalTracker disposalTracker)
    {
        // Retrieve the original member data rows (without any AutoFixture augmentation yet).
        var baseRows = await base.GetData(testMethod, disposalTracker).ConfigureAwait(false);
        var parameters = testMethod.GetParameters();
        var augmented = new List<ITheoryDataRow>(baseRows.Count);

        // Pre-compute an injector tailored to the frozen parameters of this method (with exact-type promotion enabled).
        var frozenInjector = FrozenParameterInjector.Build(parameters, enableExactTypePromotion: true);

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

    /// <summary>
    /// Resolves a specimen for a single parameter after applying any parameter-level customizations.
    /// </summary>
    /// <param name="fixture">The fixture instance configured for the current data row.</param>
    /// <param name="parameter">The reflective description of the theory method parameter.</param>
    /// <returns>The resolved object instance to be injected into the theory invocation.</returns>
    /// <remarks>
    /// Customizations are applied in a deterministic order with non-frozen customizations first, ensuring
    /// that <see cref="FrozenAttribute"/> observes the final constructed form when freezing an instance.
    /// </remarks>
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