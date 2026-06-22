namespace Atc.Test.Tests.TestSupport.Customizations;

/// <summary>
/// A test-only specimen builder discovered via <see cref="AutoRegisterAttribute"/>; used to verify
/// that <c>FixtureFactory.Create()</c> auto-registers builders. Returns <see cref="NoSpecimen"/> for
/// anything other than <see cref="AutoRegisterMarker"/>, so it does not affect other tests.
/// </summary>
[AutoRegister]
public sealed class AutoRegisterMarkerBuilder : ISpecimenBuilder
{
    public const string Sentinel = "auto-registered";

    public object Create(
        object request,
        ISpecimenContext context)
        => request.IsRequestFor<AutoRegisterMarker>()
            ? new AutoRegisterMarker { Source = Sentinel }
            : new NoSpecimen();
}