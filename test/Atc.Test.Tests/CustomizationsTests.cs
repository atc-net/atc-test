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
    public void TimeOnlyGenerator_Should_Create_A_Populated_TimeOnly()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var time = fixture.Create<TimeOnly>();

        // Assert
        time.Should().NotBe(default);
    }

    [Fact]
    public void UriGenerator_Should_Create_An_Absolute_Uri()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var uri = fixture.Create<Uri>();

        // Assert
        uri.IsAbsoluteUri.Should().BeTrue();
        uri.Scheme.Should().Be(Uri.UriSchemeHttps);
        uri.Host.Should().Be("example.org");
    }

    [Fact]
    public void UriGenerator_Should_Create_Unique_Uris()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var first = fixture.Create<Uri>();
        var second = fixture.Create<Uri>();

        // Assert
        first.Should().NotBe(second);
    }

    [Fact]
    public void TimeProviderGenerator_Should_Create_A_Provider_With_A_Fixed_Non_Default_Time()
    {
        // Arrange
        var fixture = FixtureFactory.Create();

        // Act
        var timeProvider = fixture.Create<TimeProvider>();

        // Assert
        timeProvider.GetUtcNow().Should().NotBe(default);
        timeProvider.GetUtcNow().Should().Be(timeProvider.GetUtcNow());
    }

    [Fact]
    public void TimeProviderGenerator_Should_Support_Frozen_Reuse()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var frozen = fixture.Freeze<TimeProvider>();

        // Act
        var resolved = fixture.Create<TimeProvider>();

        // Assert
        resolved.Should().BeSameAs(frozen);
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
