namespace Atc.Test.Tests;

public sealed class MemberAutoNSubstituteDataAttributeTests
{
    public static readonly IEnumerable<object[]> MemberData =
    [
        [SampleEnum.One],
        [SampleEnum.Two],
        [SampleEnum.Three]
    ];

    [Theory]
    [MemberAutoNSubstituteData(nameof(MemberData))]
    public void MemberAutoNSubstituteData_Should_Call_For_MemberData(
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

    public static readonly IEnumerable<object[]> MemberDataMultipleValues =
    [
        [SampleEnum.One, SampleEnum.Two],
        [SampleEnum.One, SampleEnum.One]
    ];

    [Theory]
    [MemberAutoNSubstituteData(nameof(MemberDataMultipleValues))]
    public void MemberAutoNSubstituteData_Should_Call_For_Each_MemberData_Values(
        SampleEnum firstValue,
        SampleEnum secondValue,
        [Frozen] ISampleInterface interfaceType,
        SampleClass concreteType,
        SampleDependantClass dependantType)
    {
        firstValue.Should().BeOneOf(SampleEnum.One, SampleEnum.Two, SampleEnum.Three);
        secondValue.Should().BeOneOf(SampleEnum.One, SampleEnum.Two, SampleEnum.Three);
        interfaceType.Should().NotBeNull();
        interfaceType.IsSubstitute().Should().BeTrue();
        concreteType.Should().NotBeNull();
        concreteType.IsSubstitute().Should().BeFalse();
        dependantType.Should().NotBeNull();
        dependantType.Dependency.Should().Be(interfaceType);
    }
}