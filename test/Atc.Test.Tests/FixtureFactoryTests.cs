namespace Atc.Test.Tests;

public sealed class FixtureFactoryTests
{
    [Fact]
    public void Create_Should_Return_A_Fixture()
    {
        // Act
        var fixture = FixtureFactory.Create();

        // Assert
        fixture.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_Apply_AutoRegistered_SpecimenBuilders()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var marker = fixture.Create<AutoRegisterMarker>();

        // Assert
        marker.Source.Should().Be(AutoRegisterMarkerBuilder.Sentinel);
    }
}
