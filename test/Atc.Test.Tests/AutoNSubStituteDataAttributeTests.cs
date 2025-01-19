namespace Atc.Test.Tests;

public sealed class AutoNSubstituteDataAttributeTests
{
    [Theory, AutoNSubstituteData]
    public void Should_Create_Concrete_Type_With_AutoFixture(
        SampleClass concreteType)
    {
        concreteType
            .Should().NotBeNull();
        concreteType.IsSubstitute()
            .Should().BeFalse();
        concreteType.StringProperty
            .Should()
            .NotBeNullOrWhiteSpace();
        concreteType.IntProperty
            .Should()
            .BeGreaterThan(0);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Create_Abstract_Type_With_NSubstitute(
        ISampleInterface abstractType,
        string str,
        int i)
    {
        abstractType
            .Should().NotBeNull();
        abstractType.IsSubstitute()
            .Should().BeTrue();

        abstractType.StringProperty
            .Returns(str);
        abstractType.StringProperty
            .Should().Be(str);

        abstractType.IntProperty
            .Returns(i);
        abstractType.IntProperty
            .Should().Be(i);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Create_Uncancelled_CancellationToken(
        CancellationToken cancellationToken)
    {
        cancellationToken.IsCancellationRequested
            .Should().BeFalse();
    }
}