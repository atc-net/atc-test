#if NET6_0_OR_GREATER
namespace Atc.Test.Customizations.Generators;

/// <summary>
/// Responsible for generating <see cref="DateOnly"/> instances
/// that has not been canceled.
/// </summary>
[AutoRegister]
public class DateOnlyGenerator : ISpecimenBuilder
{
    /// <inheritdoc/>
    public object Create(object request, ISpecimenContext context)
    {
        if (!request.IsRequestFor<DateOnly>())
        {
            return new NoSpecimen();
        }

        return DateOnly.FromDateTime(context.Create<DateTime>());
    }
}
#endif