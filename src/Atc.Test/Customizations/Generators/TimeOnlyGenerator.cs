namespace Atc.Test.Customizations.Generators;

/// <summary>
/// Responsible for generating <see cref="TimeOnly"/> instances
/// derived from a generated <see cref="DateTime"/>.
/// </summary>
[AutoRegister]
public class TimeOnlyGenerator : ISpecimenBuilder
{
    /// <inheritdoc/>
    public object Create(
        object request,
        ISpecimenContext context)
    {
        if (!request.IsRequestFor<TimeOnly>())
        {
            return new NoSpecimen();
        }

        return TimeOnly.FromDateTime(context.Create<DateTime>());
    }
}
