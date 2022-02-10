// ReSharper disable UseNullableAnnotationInsteadOfAttribute
namespace Atc.Test;

/// <summary>
/// Extensions for <see cref="object"/> used in tests.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Invokes a protected method of a class.
    /// </summary>
    /// <param name="obj">The target object.</param>
    /// <param name="methodName">The name of the protected method.</param>
    /// <param name="args">The method arguments.</param>
    /// <returns>The return value of the call.</returns>
    [return: MaybeNull]
    public static object InvokeProtectedMethod(this object obj, string methodName, params object[] args)
        => (obj ?? throw new ArgumentNullException(nameof(obj)))
            .GetType()
            .GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args.Select(a => a.GetType()).ToArray(),
                modifiers: null)?
            .Invoke(obj, args);

    /// <summary>
    /// Invokes a protected method of a class.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <param name="obj">The target object.</param>
    /// <param name="methodName">The name of the protected method.</param>
    /// <param name="args">The method arguments.</param>
    /// <returns>The return value of the call.</returns>
    [return: MaybeNull]
    public static T InvokeProtectedMethod<T>(this object obj, string methodName, params object[] args)
        => (T?)InvokeProtectedMethod(obj, methodName, args);

    /// <summary>
    /// Checks if an object has any properties.
    /// </summary>
    /// <remarks>
    /// This check is relevant before using the .BeEquivalentTo assertion,
    /// which will throw for object with no properties.
    /// </remarks>
    /// <param name="obj">The target object.</param>
    /// <returns>True if object has properties.</returns>
    public static bool HasProperties(this object obj)
        => (obj ?? throw new ArgumentNullException(nameof(obj)))
            .GetType()
            .GetProperties()
            .Length > 0;
}