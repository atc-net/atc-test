namespace Atc.Test.Tests;

public sealed class SubstituteExtensionsTests
{
    [Fact]
    public void ReceivedCallWithArgument_Should_Return_The_Single_Argument()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();

        // Act
        service.Notify("hello");

        // Assert
        service.ReceivedCallWithArgument<string>().Should().Be("hello");
    }

    [Fact]
    public void ReceivedCallsWithArguments_Should_Return_All_Arguments()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();

        // Act
        service.Notify("a");
        service.Notify("b");

        // Assert
        service.ReceivedCallsWithArguments<string>().Should().Equal("a", "b");
    }

    [Fact]
    public Task WaitForCall_Should_Complete_When_Call_Already_Received()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();
        service.Notify("x");

        // Act
        var act = () => service.WaitForCall(s => s.Notify("x"), TimeSpan.FromSeconds(5));

        // Assert
        return act.Should().NotThrowAsync();
    }

    [Fact]
    public Task WaitForCall_Should_Complete_When_Call_Made_After_Wait_Starts()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();
        var waitTask = service.WaitForCall(s => s.Notify("y"), TimeSpan.FromSeconds(5));

        // Act — the call is made after WaitForCall has registered its callback (done synchronously
        // before its first await), so this completes deterministically without a background thread.
        service.Notify("y");
        var act = () => waitTask;

        // Assert
        return act.Should().NotThrowAsync();
    }

    [Fact]
    public Task WaitForCall_Should_Throw_When_Call_Not_Received_Within_Timeout()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();

        // Act
        var act = () => service.WaitForCall(s => s.Notify("z"), TimeSpan.FromMilliseconds(100));

        // Assert
        return act.Should().ThrowAsync<ReceivedCallsException>();
    }

    [Fact]
    public async Task WaitForCall_Func_Task_Should_Complete_When_Call_Already_Received()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();
        await service.SendAsync("x");

        // Act
        var act = () => service.WaitForCall(s => s.SendAsync("x"), TimeSpan.FromSeconds(5));

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task WaitForCall_Func_ValueTask_Should_Complete_When_Call_Already_Received()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();
        await service.CalculateAsync(1);

        // Act
        var act = () => service.WaitForCall(s => s.CalculateAsync(1), TimeSpan.FromSeconds(5));

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public Task WaitForCallForAnyArgs_Should_Complete_Regardless_Of_Arguments()
    {
        // Arrange
        var service = Substitute.For<ISampleService>();
        service.Notify("actual");

        // Act
        var act = () => service.WaitForCallForAnyArgs(s => s.Notify("ignored"), TimeSpan.FromSeconds(5));

        // Assert
        return act.Should().NotThrowAsync();
    }
}
