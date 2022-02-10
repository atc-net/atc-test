namespace Atc.Test.Tests;

public class MemberAutoNSubstituteDataAttributeTests
{
    public static readonly IEnumerable<object[]> TestData = new[]
    {
        new object[] { SampleEnum.One },
        new object[] { SampleEnum.Two },
        new object[] { SampleEnum.Three },
    };

    [Theory]
    [MemberAutoNSubstituteData(nameof(TestData))]
    public void MemberAutoNSubstituteData_Should_Call_For_Each_Value(
        SampleEnum value,
        [Frozen] ISampleInterface interfaceType,
        SampleClass concreteType,
        SampleDependantClass dependantType)
    {
        value.Should().BeOneOf(SampleEnum.One, SampleEnum.Two, SampleEnum.Three);
        interfaceType.Should().NotBeNull();
        interfaceType.IsSubstitute().Should().BeTrue();
        concreteType.Should().NotBeNull();
        concreteType.IsSubstitute().Should().BeFalse();
        dependantType.Should().NotBeNull();
        dependantType.Dependency.Should().Be(interfaceType);
    }
}