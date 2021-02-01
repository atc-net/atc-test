using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;

namespace Atc.Test
{
    /// <summary>
    /// Extensions for test substitutes created by NSubstitute.
    /// </summary>
    [SuppressMessage(
        "AsyncUsage",
        "AsyncFixer03:Fire-and-forget async-void methods or delegates",
        Justification = "Calls on substitutes are made for validation, await is not required.")]
    public static class SubstituteExtensions
    {
        /// <summary>
        /// Returns argument of a call to the substitute.
        /// </summary>
        /// <typeparam name="T">
        /// The type for the argument.
        /// </typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="because">A formatted phrase as is supported by System.String.Format(System.String,System.Object[])
        /// explaining why the assertion is needed. If the phrase does not start with the
        /// word because, it is prepended automatically.</param>
        /// <param name="becauseArgs">Zero or more objects to format using the placeholders in because.</param>
        /// <returns>Argument of a call to the substitute.</returns>
        public static T ReceivedCallWithArgument<T>(
            this object substitute,
            string because = "",
            params object[] becauseArgs)
        {
            var arguments = substitute.ReceivedCalls()
                .SelectMany(x => x.GetArguments())
                .OfType<T>()
                .ToArray();

            arguments.Should().HaveCount(1, because, becauseArgs);

            return arguments[0];
        }

        /// <summary>
        /// Returns arguments of calls to the substitute.
        /// </summary>
        /// <typeparam name="T">
        /// The type for the argument.
        /// </typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="because">A formatted phrase as is supported by System.String.Format(System.String,System.Object[])
        /// explaining why the assertion is needed. If the phrase does not start with the
        /// word because, it is prepended automatically.</param>
        /// <param name="becauseArgs">Zero or more objects to format using the placeholders in because.</param>
        /// <returns>Arguments of a calls to the substitute.</returns>
        public static T[] ReceivedCallsWithArguments<T>(
            this object substitute,
            string because = "",
            params object[] becauseArgs)
        {
            var arguments = substitute.ReceivedCalls()
                .SelectMany(x => x.GetArguments())
                .OfType<T>()
                .ToArray();

            arguments.Should().NotBeEmpty(because, becauseArgs);

            return arguments;
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is made to the Substitute.
        /// </summary>
        /// <typeparam name="T">Type of the substitute.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCall<T>(
            this T substitute,
            Action<T> substituteCall,
            TimeSpan timeout = default)
            where T : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .When(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    substituteCall
                        ?? throw new ArgumentNullException(nameof(substituteCall)),
                    MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is made to the Substitute.
        /// </summary>
        /// <typeparam name="T">Type of the substitute.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCall<T>(
            this T substitute,
            Func<T, Task> substituteCall,
            TimeSpan timeout = default)
            where T : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .When(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    x => substituteCall(x),
                    MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is made to the Substitute.
        /// </summary>
        /// <typeparam name="TSubstitute">Type of the substitute.</typeparam>
        /// <typeparam name="TResult">The substitute call return type.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCall<TSubstitute, TResult>(
            this TSubstitute substitute,
            Func<TSubstitute, ValueTask<TResult>> substituteCall,
            TimeSpan timeout = default)
            where TSubstitute : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .When(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    async x => await substituteCall(x)
                        .ConfigureAwait(false),
                    MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is made to the Substitute.
        /// </summary>
        /// <typeparam name="T">Type of the substitute.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCallForAnyArgs<T>(
            this T substitute,
            Action<T> substituteCall,
            TimeSpan timeout = default)
            where T : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .WhenForAnyArgs(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    substituteCall
                        ?? throw new ArgumentNullException(nameof(substituteCall)),
                    MatchArgs.Any);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is mate to the Substitute.
        /// </summary>
        /// <typeparam name="T">Type of the substitute.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCallForAnyArgs<T>(
            this T substitute,
            Func<T, Task> substituteCall,
            TimeSpan timeout = default)
            where T : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .WhenForAnyArgs(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    x => substituteCall(x),
                    MatchArgs.Any);
        }

        /// <summary>
        /// Creates a <see cref="Task" /> that will complete
        /// when the specified call is mate to the Substitute.
        /// </summary>
        /// <typeparam name="TSubstitute">Type of the substitute.</typeparam>
        /// <typeparam name="TResult">The substitute call return type.</typeparam>
        /// <param name="substitute">The substitute.</param>
        /// <param name="substituteCall">The call to wait for.</param>
        /// <param name="timeout">Timeout for the wait operation.</param>
        /// <returns>A task representing the async operation.</returns>
        public static async Task WaitForCallForAnyArgs<TSubstitute, TResult>(
            this TSubstitute substitute,
            Func<TSubstitute, ValueTask<TResult>> substituteCall,
            TimeSpan timeout = default)
            where TSubstitute : class
        {
            var completion = new TaskCompletionSource<bool>();
            substitute
                .WhenForAnyArgs(substituteCall)
                .Do(c => completion.TrySetResult(true));

            await completion
                .WaitForCompletion(timeout)
                .ConfigureAwait(false);

            substitute
                .ValidateCallReceived(
                    async x => await substituteCall(x)
                        .ConfigureAwait(false),
                    MatchArgs.Any);
        }

        private static void ValidateCallReceived<T>(
            this T substitute,
            Action<T> substituteCall,
            MatchArgs matchArgs)
            where T : class
        {
            var context = SubstitutionContext.Current;
            var callRouter = context.GetCallRouterFor(substitute);

            context.ThreadContext
                .SetNextRoute(
                    callRouter,
                    x => context.RouteFactory.CheckReceivedCalls(
                        x,
                        matchArgs ?? MatchArgs.AsSpecifiedInCall,
                        Quantity.AtLeastOne()));
            substituteCall(substitute);
        }

        private static async Task WaitForCompletion(
            this TaskCompletionSource<bool> completion,
            TimeSpan timeout)
        {
            try
            {
                await completion.Task
                    .AddTimeout(timeout)
                    .ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                // Ignore
            }
        }
    }
}