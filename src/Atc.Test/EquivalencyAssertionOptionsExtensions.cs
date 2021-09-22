using System;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace Atc.Test
{
    /// <summary>
    /// Extensions for the <see cref="EquivalencyAssertionOptions{T}"/> type.
    /// </summary>
    public static class EquivalencyAssertionOptionsExtensions
    {
        /// <summary>
        /// Configures .BeEquivalentTo extensions to compare <see cref="DateTime"/> and
        /// <see cref="DateTimeOffset "/> values by checking if they are within the specified
        /// number of milliseconds (default = 1s).
        /// </summary>
        /// <typeparam name="T">The generic parameter for the <see cref="EquivalencyAssertionOptions{T}"/>.</typeparam>
        /// <param name="options">The <see cref="EquivalencyAssertionOptions{T}"/> to configure.</param>
        /// <param name="precision">The precision in milliseconds.</param>
        /// <returns>The configured <see cref="EquivalencyAssertionOptions{T}"/>.</returns>
        public static EquivalencyAssertionOptions<T> CompareDateTimeUsingCloseTo<T>(
            this EquivalencyAssertionOptions<T> options,
            int precision = 1000)
            => (options ?? throw new ArgumentNullException(nameof(options)))
                .Using<DateTimeOffset>(ctx => ctx.Subject
                    .Should()
                    .BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(precision)))
                .WhenTypeIs<DateTimeOffset>()
                .Using<DateTime>(ctx => ctx.Subject
                    .Should()
                    .BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(precision)))
                    .WhenTypeIs<DateTime>();

        /// <summary>
        /// Configures .BeEquivalentTo extensions to compare <see cref="DateTime"/> and
        /// <see cref="DateTimeOffset "/> values by checking if they are within the specified
        /// number of milliseconds (default = 1s).
        /// </summary>
        /// <typeparam name="T">The generic parameter for the <see cref="EquivalencyAssertionOptions{T}"/>.</typeparam>
        /// <param name="options">The <see cref="EquivalencyAssertionOptions{T}"/> to configure.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>The configured <see cref="EquivalencyAssertionOptions{T}"/>.</returns>
        public static EquivalencyAssertionOptions<T> CompareDateTimeUsingCloseTo<T>(
            this EquivalencyAssertionOptions<T> options,
            TimeSpan precision)
            => (options ?? throw new ArgumentNullException(nameof(options)))
                .Using<DateTimeOffset>(ctx => ctx.Subject
                    .Should()
                    .BeCloseTo(ctx.Expectation, precision))
                .WhenTypeIs<DateTimeOffset>()
                .Using<DateTime>(ctx => ctx.Subject
                    .Should()
                    .BeCloseTo(ctx.Expectation, precision))
                    .WhenTypeIs<DateTime>();
    }
}