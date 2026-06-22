namespace Atc.Test.Tests;

public sealed class CustomizationsTests
{
    [Fact]
    public void RecursionCustomization_Should_Not_Throw_For_Recursive_Types()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var act = () => fixture.Create<RecursiveNode>();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CancellationTokenGenerator_Should_Create_Non_Canceled_Token()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var token = fixture.Create<CancellationToken>();

        // Assert
        token.IsCancellationRequested.Should().BeFalse();
    }

    [Fact]
    public void DateOnlyGenerator_Should_Create_A_Populated_DateOnly()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var date = fixture.Create<DateOnly>();

        // Assert
        date.Should().NotBe(default);
    }

    [Fact]
    public void ImmutableObjectCustomization_Should_Create_A_Populated_ImmutableArray()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var values = fixture.Create<ImmutableArray<int>>();

        // Assert
        values.Should().NotBeEmpty();
    }
}
