namespace Atc.Test.Customizations.Generators;

/// <summary>
/// Responsible for generating readable and deterministic absolute
/// <see cref="Uri"/> instances using the reserved <c>example.org</c> domain.
/// </summary>
[AutoRegister]
public class UriGenerator : ISpecimenBuilder
{
    /// <inheritdoc/>
    public object Create(
        object request,
        ISpecimenContext context)
    {
        if (!request.IsRequestFor<Uri>())
        {
            return new NoSpecimen();
        }

        var segment = Sanitize(context.Create<string>());

        return new Uri($"https://example.org/{segment}");
    }

    private static string Sanitize(string value)
    {
        var sb = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            if (char.IsAsciiLetterOrDigit(c) || c is '-')
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }

        return sb.Length == 0
            ? Guid.NewGuid().ToString("N")
            : sb.ToString();
    }
}
