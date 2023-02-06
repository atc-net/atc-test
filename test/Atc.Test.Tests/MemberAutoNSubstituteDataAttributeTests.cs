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

    public static readonly IEnumerable<object[]> TestDataMultiple = new[]
    {
        new object[] { SampleEnum.One, SampleEnum.Two },
        new object[] { SampleEnum.One, SampleEnum.One },
    };

    [Theory]
    [MemberAutoNSubstituteData(nameof(TestDataMultiple))]
    public void MemberAutoNSubstituteData_Should_Call_For_Each_Value_Mutliple(
        SampleEnum value1,
        SampleEnum value2,
        [Frozen] ISampleInterface interfaceType,
        SampleClass concreteType,
        SampleDependantClass dependantType)
    {
        value1.Should().BeOneOf(SampleEnum.One, SampleEnum.Two, SampleEnum.Three);
        value2.Should().BeOneOf(SampleEnum.One, SampleEnum.Two, SampleEnum.Three);
        interfaceType.Should().NotBeNull();
        interfaceType.IsSubstitute().Should().BeTrue();
        concreteType.Should().NotBeNull();
        concreteType.IsSubstitute().Should().BeFalse();
        dependantType.Should().NotBeNull();
        dependantType.Dependency.Should().Be(interfaceType);
    }
}