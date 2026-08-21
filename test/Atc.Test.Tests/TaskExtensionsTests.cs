namespace Atc.Test.Tests;

public sealed class TaskExtensionsTests
{
    [Fact]
    public async Task AwaitTasks_Generic_Should_Return_All_Results()
    {
        // Arrange
        var tasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3) };

        // Act
        var results = await tasks.AwaitTasks();

        // Assert
        results.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task AwaitTasks_Should_Complete_All_Tasks()
    {
        // Arrange
        var completed = 0;
        var tasks = new[]
        {
            Task.Run(() => Interlocked.Increment(ref completed)),
            Task.Run(() => Interlocked.Increment(ref completed)),
        };

        // Act
        await tasks.AwaitTasks();

        // Assert
        completed.Should().Be(2);
    }

    [Fact]
    public async Task AddTimeout_Generic_Should_Return_Value_When_Completed_In_Time()
    {
        // Arrange
        var task = Task.FromResult(42);

        // Act
        var result = await task.AddTimeout(TimeSpan.FromSeconds(5));

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public Task AddTimeout_Generic_Should_Throw_When_Timed_Out()
    {
        // Arrange — a task that never completes on its own.
        var task = new TaskCompletionSource<int>().Task;

        // Act
        var act = () => task.AddTimeout(TimeSpan.FromMilliseconds(100));

        // Assert
        return act.Should().ThrowAsync<TimeoutException>();
    }

    [Fact]
    public Task AddTimeout_Should_Throw_When_Timed_Out()
    {
        // Arrange — a task that never completes on its own.
        var task = new TaskCompletionSource().Task;

        // Act
        var act = () => task.AddTimeout(TimeSpan.FromMilliseconds(100));

        // Assert
        return act.Should().ThrowAsync<TimeoutException>();
    }

    [Fact]
    public Task AddTimeout_Should_Throw_For_Null_Task()
    {
        // Arrange
        Task task = null;

        // Act
        var act = () => task.AddTimeout();

        // Assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }
}
