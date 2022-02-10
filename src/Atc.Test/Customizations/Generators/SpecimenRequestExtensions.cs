namespace Atc.Test.Customizations.Generators;

/// <summary>
/// Extensions for <see cref="ISpecimenBuilder"/> requests.
/// </summary>
public static class SpecimenRequestExtensions
{
    /// <summary>
    /// Checks if an <see cref="ISpecimenBuilder"/> request is
    /// for a specific <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type to check for.</typeparam>
    /// <param name="request">The request.</param>
    /// <returns>True if the request is for the specified Type of <typeparamref name="T"/>.</returns>
    public static bool IsRequestFor<T>(this object request) => request switch
    {
        ParameterInfo pi => pi.ParameterType == typeof(T),
        Type type => type == typeof(T),
        _ => false,
    };
}