namespace Atc.Test;

internal static class FrozenParameterInjector
{
    private static readonly MethodInfo? InjectMethod = typeof(FixtureRegistrar).GetMethod(
        nameof(FixtureRegistrar.Inject),
        BindingFlags.Public | BindingFlags.Static);

    /// <summary>
    /// Builds an injector delegate that performs positional frozen injections and (optionally) exact-type promotions.
    /// </summary>
    /// <param name="parameters">Ordered method parameters.</param>
    /// <param name="enableExactTypePromotion">When true, a later frozen parameter whose index exceeds supplied row length
    /// will re-use an earlier supplied value only if the earlier parameter type is exactly the same (no interface/base widening).</param>
    /// <returns>An injector delegate accepting (suppliedRowValues, fixture) that performs injections/promotions.</returns>
    internal static Action<object?[], IFixture> Build(
        ParameterInfo[] parameters,
        bool enableExactTypePromotion)
    {
        var frozenParameters = parameters
            .Select((p, i) => new FrozenDescriptor(i, p, p.GetCustomAttribute<FrozenAttribute>() != null))
            .Where(x => x.IsFrozen)
            .ToArray();

        if (frozenParameters.Length == 0)
        {
            return static (_, _) => { };
        }

        // Pre-compute mapping from type to earliest supplied parameter index for fast exact promotion lookup.
        var earliestIndexByType = parameters
            .Select((p, i) => (p.ParameterType, Index: i))
            .GroupBy(x => x.ParameterType)
            .ToDictionary(g => g.Key, g => g.Min(x => x.Index));

        return (suppliedData, fixture) =>
        {
            // Phase 1: direct positional injection where row already supplies the frozen parameter value.
            foreach (var frozen in frozenParameters)
            {
                if (suppliedData.Length > frozen.Index)
                {
                    InjectGeneric(fixture, frozen.Parameter.ParameterType, suppliedData[frozen.Index]);
                }
            }

            if (!enableExactTypePromotion)
            {
                return; // Class data does not promote.
            }

            // Phase 2: exact-type promotion only (no interface/base assignability) to avoid cross-interface bleed.
            foreach (var frozen in frozenParameters)
            {
                if (suppliedData.Length > frozen.Index)
                {
                    continue; // Already handled by direct positional reuse.
                }

                if (!earliestIndexByType.TryGetValue(frozen.Parameter.ParameterType, out var earliestIndex))
                {
                    continue;
                }

                if (earliestIndex >= suppliedData.Length)
                {
                    continue; // Earliest instance not actually supplied in this row.
                }

                // Reuse supplied instance at earliest index for this exact type.
                var instance = suppliedData[earliestIndex];
                InjectGeneric(fixture, frozen.Parameter.ParameterType, instance);
            }
        };
    }

    private static void InjectGeneric(
        IFixture fixture,
        Type type,
        object? instance)
        => InjectMethod?
            .MakeGenericMethod(type)
            .Invoke(null, [fixture, instance]);

    private readonly struct FrozenDescriptor(
        int index,
        ParameterInfo parameter,
        bool isFrozen)
    {
        public int Index { get; } = index;

        public ParameterInfo Parameter { get; } = parameter;

        public bool IsFrozen { get; } = isFrozen;
    }
}
