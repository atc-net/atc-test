namespace Atc.Test.Tests;

public sealed class ObjectExtensionsTests
{
    [Fact]
    public void InvokeProtectedMethod_Generic_Should_Invoke_And_Return_Typed_Value()
    {
        // Arrange
        var host = new ProtectedMethodHost();

        // Act
        var result = host.InvokeProtectedMethod<int>("Add", 2, 3);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void InvokeProtectedMethod_Should_Invoke_And_Return_Object()
    {
        // Arrange
        var host = new ProtectedMethodHost();

        // Act
        var result = host.InvokeProtectedMethod("Echo", "hi");

        // Assert
        result.Should().Be("hi");
    }

    [Fact]
    public void HasProperties_Should_Return_True_When_Object_Has_Properties()
    {
        // Arrange
        var sut = new SampleClass();

        // Act & Assert
        sut.HasProperties().Should().BeTrue();
    }

    [Fact]
    public void HasProperties_Should_Return_False_When_Object_Has_No_Properties()
    {
        // Arrange
        var sut = new object();

        // Act & Assert
        sut.HasProperties().Should().BeFalse();
    }
}
