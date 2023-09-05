namespace Atc.Test.Customizations;

/// <summary>
/// Responsible for setting up recursion behavior
/// to ensure AutoFixture will not throw on recursion.
/// </summary>
public class RecursionCustomization : ICustomization
{
    /// <inheritdoc/>
    public void Customize(IFixture fixture)
    {
        fixture?.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture?.Behaviors
            .Add(new OmitOnRecursionBehavior());
    }
}