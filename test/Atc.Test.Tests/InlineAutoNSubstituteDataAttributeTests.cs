using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Atc.Test.Tests
{
    public class InlineAutoNSubstituteDataAttributeTests
    {
        [Theory]
        [InlineAutoNSubstituteData(SampleEnum.One)]
        [InlineAutoNSubstituteData(SampleEnum.Two)]
        [InlineAutoNSubstituteData(SampleEnum.Three)]
        public void InlineAutoNSubstituteData_Should_Call_For_Each_Value(
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
}
