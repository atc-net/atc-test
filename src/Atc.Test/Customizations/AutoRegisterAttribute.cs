namespace Atc.Test.Customizations;

/// <summary>
/// Marks a class for automatic registration on the <see cref="IFixture"/>
/// created by <see cref="FixtureFactory"/>.
/// </summary>
/// <remarks>
/// The decorated class must implement either <see cref="ICustomization"/> or
/// <see cref="ISpecimenBuilder"/> and expose a parameterless constructor.
/// Decorating a class that implements neither results in a
/// <see cref="NotSupportedException"/> when the fixture is created.
/// <para>
/// Discovery is performed by <see cref="AutoRegisterCustomization"/>, which scans the
/// assemblies loaded into the current <see cref="AppDomain"/>. Types in a test project
/// referencing Atc.Test are therefore picked up without any further configuration.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [AutoRegister]
/// public class GuidCustomization : ICustomization
/// {
///     public void Customize(IFixture fixture)
///         =&gt; fixture.Register(() =&gt; Guid.NewGuid());
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoRegisterAttribute : Attribute
{
}