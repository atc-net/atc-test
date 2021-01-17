using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.TestTools.Tests
{
    public class AutoNSubStituteDataAttributeTests
    {
        public class ConcreteType
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }

        public interface IInterfaceType
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }

        [Theory, AutoNSubstituteData]
        public void Should_Create_Concrete_Type_With_AutoFixture(
            ConcreteType concreteType)
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
            IInterfaceType abstractType,
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
    }
}