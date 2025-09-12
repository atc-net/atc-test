namespace Atc.Test.Tests;

public sealed class MemberAutoNSubstituteDataAttributeTests
{
    public static readonly IEnumerable<object[]> MemberData =
    [
        [SampleEnum.One],
        [SampleEnum.Two],
        [SampleEnum.Three],
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

    [Theory]
    [MemberAutoNSubstituteData(nameof(DataFrozenProvided))]
    public void MemberData_SuppliesValueForFrozenParameter_ReusedForDependants(
        [Frozen] ISampleInterface frozen,
        SampleDependantClass dependant)
    {
        frozen.Should().NotBeNull();
        dependant.Should().NotBeNull();
        dependant.Dependency.Should().BeSameAs(frozen);
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(DataFrozenProvided))]
    public void MemberData_SuppliesValueEarlier_ReusedByLaterFrozenParameter(
        ISampleInterface provided,
        [Frozen] ISampleInterface frozen,
        SampleDependantClass dependant)
    {
        provided.Should().NotBeNull();
        frozen.Should().NotBeNull();
        dependant.Should().NotBeNull();
        frozen.Should().BeSameAs(provided);
        dependant.Dependency.Should().BeSameAs(provided);
    }

    public static IEnumerable<object?[]> DataFrozenProvided()
    {
        yield return [Substitute.For<ISampleInterface>()];
    }

    // Negative promotion scenario
    // We provide an earlier argument typed as IFirstInterface whose concrete instance also
    // implements ISecondInterface. The later parameter is [Frozen] ISecondInterface. Desired
    // invariant: the framework should NOT promote the earlier IFirstInterface value to satisfy
    // the frozen ISecondInterface; a distinct instance should be generated/injected instead.
    // Current promotion logic (Type.IsInstanceOfType) is broad and WILL incorrectly reuse it.
    // This test therefore initially fails (red) and will pass after tightening promotion logic
    // in the upcoming refactor (shared frozen helper with exact-type matching / index mapping).
    [Theory]
    [MemberAutoNSubstituteData(nameof(DualInterfaceProvidedData))]
    public void MemberData_DifferentInterface_NotPromoted(
        IFirstInterface first,
        [Frozen] ISecondInterface second,
        SecondInterfaceDependant dependant)
    {
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        dependant.Should().NotBeNull();

        // Guard: dependant wired to the frozen instance.
        dependant.Dependency.Should().BeSameAs(second);

        // Specification: these should NOT be the same instance
        second.Should().NotBeSameAs(first);
    }

    public static IEnumerable<object?[]> DualInterfaceProvidedData()
    {
        yield return [new DualInterfaceImpl { Name = "n", Description = "d" }];
    }
}