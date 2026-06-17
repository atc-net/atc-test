namespace Atc.Test;

/// <summary>
/// Extensions for the <see cref="EquivalencyOptions{T}"/> type.
/// </summary>
public static class EquivalencyAssertionOptionsExtensions
{
    /// <summary>
    /// Configures .BeEquivalentTo extensions to compare <see cref="DateTime"/> and
    /// <see cref="DateTimeOffset "/> values by checking if they are within the specified
    /// number of milliseconds (default = 1s).
    /// </summary>
    /// <typeparam name="T">The generic parameter for the <see cref="EquivalencyOptions{T}"/>.</typeparam>
    /// <param name="options">The <see cref="EquivalencyOptions{T}"/> to configure.</param>
    /// <param name="precision">The precision in milliseconds.</param>
    /// <returns>The configured <see cref="EquivalencyOptions{T}"/>.</returns>
    public static EquivalencyOptions<T> CompareDateTimeUsingCloseTo<T>(
        this EquivalencyOptions<T> options,
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
    /// <typeparam name="T">The generic parameter for the <see cref="EquivalencyOptions{T}"/>.</typeparam>
    /// <param name="options">The <see cref="EquivalencyOptions{T}"/> to configure.</param>
    /// <param name="precision">The precision.</param>
    /// <returns>The configured <see cref="EquivalencyOptions{T}"/>.</returns>
    public static EquivalencyOptions<T> CompareDateTimeUsingCloseTo<T>(
        this EquivalencyOptions<T> options,
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

    /// <summary>
    /// Configures .BeEquivalentTo extensions to compare <see cref="JsonElement"/> by
    /// comparing the underlying JSON string representation.
    /// </summary>
    /// <typeparam name="T">The generic parameter for the <see cref="EquivalencyOptions{T}"/>.</typeparam>
    /// <param name="options">The <see cref="EquivalencyOptions{T}"/> to configure.</param>
    /// <returns>The configured <see cref="EquivalencyOptions{T}"/>.</returns>
    public static EquivalencyOptions<T> CompareJsonElementUsingJson<T>(
        this EquivalencyOptions<T> options)
        => options.Using(new JsonElementEquivalencyStep());

    private sealed class JsonElementEquivalencyStep : IEquivalencyStep
    {
        public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IValidateChildNodeEquivalency valueChildNodes)
        {
            if (comparands.Subject is not JsonElement subject ||
                comparands.Expectation is not JsonElement expectation)
            {
                return EquivalencyResult.ContinueWithNext;
            }

            var newComparands = new Comparands(subject.GetRawText(), expectation.GetRawText(), typeof(string));
            valueChildNodes.AssertEquivalencyOf(newComparands, context);

            return EquivalencyResult.EquivalencyProven;
        }
    }
}