namespace Atc.Test.Tests;

public sealed class EquivalencyAssertionOptionsExtensionsTests
{
    [Fact]
    public void CompareDateTimeUsingCloseTo_Should_Treat_Values_Within_Precision_As_Equivalent()
    {
        // Arrange
        var baseTime = new DateTime(2026, 6, 17, 12, 0, 0, DateTimeKind.Utc);
        var subject = new TimeHolder
        {
            Timestamp = baseTime.AddMilliseconds(500),
            Offset = new DateTimeOffset(baseTime).AddMilliseconds(500),
        };
        var expectation = new TimeHolder
        {
            Timestamp = baseTime,
            Offset = new DateTimeOffset(baseTime),
        };

        // Act
        var act = () => subject
            .Should()
            .BeEquivalentTo(expectation, opt => opt.CompareDateTimeUsingCloseTo());

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CompareJsonElementUsingJson_Should_Treat_Equal_Json_As_Equivalent()
    {
        // Arrange
        const string json = "{\"name\":\"test\",\"value\":42}";
        var subject = new JsonHolder { Element = JsonDocument.Parse(json).RootElement };
        var expectation = new JsonHolder { Element = JsonDocument.Parse(json).RootElement };

        // Act
        var act = () => subject
            .Should()
            .BeEquivalentTo(expectation, opt => opt.CompareJsonElementUsingJson());

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CompareJsonElementUsingJson_Should_Detect_Different_Json_As_Not_Equivalent()
    {
        // Arrange
        // Two JsonElement values of the same ValueKind pass *default* equivalency trivially,
        // so this failing assertion proves the custom JsonElement step actually compares the
        // underlying JSON (exercises the rewritten Handle / AssertEquivalencyOf path).
        var subject = new JsonHolder { Element = JsonDocument.Parse("{\"name\":\"test\",\"value\":42}").RootElement };
        var expectation = new JsonHolder { Element = JsonDocument.Parse("{\"name\":\"test\",\"value\":99}").RootElement };

        // Act
        var act = () => subject
            .Should()
            .BeEquivalentTo(expectation, opt => opt.CompareJsonElementUsingJson());

        // Assert
        act.Should().Throw<Exception>();
    }
}