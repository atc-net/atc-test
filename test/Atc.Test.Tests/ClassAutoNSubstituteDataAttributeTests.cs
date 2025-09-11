namespace Atc.Test.Tests;

public sealed class ClassAutoNSubstituteDataAttributeTests
{
    public class TestData : TheoryData<SampleEnum>
    {
        public TestData()
        {
            Add(SampleEnum.One);
            Add(SampleEnum.Two);
            Add(SampleEnum.Three);
        }
    }

    [Theory]
    [ClassAutoNSubstituteData(typeof(TestData))]
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
}
