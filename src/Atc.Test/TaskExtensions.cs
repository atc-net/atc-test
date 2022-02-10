namespace Atc.Test;

/// <summary>
/// Extensions for the <see cref="Task"/> type.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Gets the default Timeout used.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Extension calling <see cref="Task.WhenAll{T}(IEnumerable{Task{T}})"/>.
    /// </summary>
    /// <typeparam name="T">The type of the completed tasks.</typeparam>
    /// <param name="tasks">The tasks to wait on for completion.</param>
    /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
    public static Task<T[]> AwaitTasks<T>(this IEnumerable<Task<T>> tasks)
        => Task.WhenAll(tasks);

    /// <summary>
    /// Extension calling <see cref="Task.WhenAll(IEnumerable{Task})"/>.
    /// </summary>
    /// <param name="tasks">The tasks to wait on for completion.</param>
    /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
    public static Task AwaitTasks(this IEnumerable<Task> tasks)
        => Task.WhenAll(tasks);

    /// <summary>
    /// Adds a timeout to a Task.
    /// </summary>
    /// <typeparam name="T">The type of the completed task.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static Task<T> AddTimeout<T>(
        this Task<T> task,
        TimeSpan timeout = default)
        => AddTimeoutInternal(
            task ?? throw new ArgumentNullException(nameof(task)),
            timeout);

    /// <summary>
    /// Adds a timeout to a Task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static Task AddTimeout(
        this Task task,
        TimeSpan timeout = default)
        => AddTimeoutInternal(
            task ?? throw new ArgumentNullException(nameof(task)),
            timeout);

    private static async Task<T> AddTimeoutInternal<T>(
        this Task<T> task,
        TimeSpan timeout = default)
    {
        if (Debugger.IsAttached)
        {
            return await task.ConfigureAwait(false);
        }

        await Task.WhenAny(task, Task.Delay(
                timeout > TimeSpan.Zero
                    ? timeout
                    : DefaultTimeout))
            .ConfigureAwait(false);

        if (!task.IsCompleted)
        {
            throw new TimeoutException();
        }

        return await task.ConfigureAwait(false);
    }

    private static async Task AddTimeoutInternal(
        this Task task,
        TimeSpan timeout = default)
    {
        if (Debugger.IsAttached)
        {
            await task.ConfigureAwait(false);
        }

        await Task.WhenAny(task, Task.Delay(
                timeout > TimeSpan.Zero
                    ? timeout
                    : DefaultTimeout))
            .ConfigureAwait(false);

        if (!task.IsCompleted)
        {
            throw new TimeoutException();
        }

        await task.ConfigureAwait(false);
    }
}