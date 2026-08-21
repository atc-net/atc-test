namespace Atc.Test.Customizations.Generators;

/// <summary>
/// Responsible for generating deterministic <see cref="TimeProvider"/> instances
/// that report a fixed point in time.
/// </summary>
/// <remarks>
/// Without this generator a <see cref="TimeProvider"/> parameter would be substituted
/// as an abstract class, which yields a provider returning <c>default</c> timestamps.
/// The generated provider reports a stable, fixture-generated UTC time, which makes
/// assertions on time-dependent code deterministic. Combine with <c>[Frozen]</c> to
/// share the same instant across the system under test and the assertions.
/// </remarks>
[AutoRegister]
public class TimeProviderGenerator : ISpecimenBuilder
{
    /// <inheritdoc/>
    public object Create(
        object request,
        ISpecimenContext context)
    {
        if (!request.IsRequestFor<TimeProvider>())
        {
            return new NoSpecimen();
        }

        return new FixedTimeProvider(context.Create<DateTime>());
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset utcNow;

        public FixedTimeProvider(DateTime dateTime)
            => utcNow = new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));

        public override DateTimeOffset GetUtcNow()
            => utcNow;

        public override TimeZoneInfo LocalTimeZone
            => TimeZoneInfo.Utc;
    }
}
