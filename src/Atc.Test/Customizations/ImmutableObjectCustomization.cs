namespace Atc.Test.Customizations;

[AutoRegister]
public class ImmutableObjectCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableArray<>),
                typeof(List<>),
                o => ImmutableArray.ToImmutableArray(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableList<>),
                typeof(List<>),
                o => ImmutableList.ToImmutableList(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableDictionary<,>),
                typeof(Dictionary<,>),
                o => ImmutableDictionary.ToImmutableDictionary(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableHashSet<>),
                typeof(HashSet<>),
                o => ImmutableHashSet.ToImmutableHashSet(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableSortedSet<>),
                typeof(SortedSet<>),
                o => ImmutableSortedSet.ToImmutableSortedSet(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableSortedDictionary<,>),
                typeof(SortedDictionary<,>),
                o => ImmutableSortedDictionary.ToImmutableSortedDictionary(o)));
    }

    private sealed class ImmutableObjectBuilder(
        Type immutableType,
        Type underlyingType,
        Func<dynamic, object> converter)
        : ISpecimenBuilder
    {
        public object Create(
            object request,
            ISpecimenContext context)
        {
            if (GetRequestType(request) is not { IsGenericType: true } type
                || type.GetGenericTypeDefinition() != immutableType
                || type.GetGenericArguments() is not { Length: > 0 } args)
            {
                return new NoSpecimen();
            }

            var listType = underlyingType.MakeGenericType(args);
            dynamic list = context.Resolve(listType);

            return converter.Invoke(list);
        }

        private static Type? GetRequestType(object request)
            => request switch
            {
                ParameterInfo pi => pi.ParameterType,
                Type t => t,
                _ => null,
            };
    }
}