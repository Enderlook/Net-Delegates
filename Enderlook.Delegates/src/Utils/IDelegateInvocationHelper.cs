namespace Enderlook.Delegates;

/// <summary>
/// Represent an object used to invoke objects which implement <see cref="IDelegate"/> with dynamic parameters, while at the same time, trying to avoid unnecessy allocations or boxing/unboxing.
/// </summary>
public interface IDelegateInvocationHelper
{
    /// <summary>
    /// Get the argument at the specified index.<br/>
    /// The generic parameter <typeparamref name="T"/> is for convenience in order to avoid boxing, the method is valid to be called with any type that can be assigned, boxed or unboxed to the runtime type of the parameter (supports covariance).
    /// </summary>
    /// <typeparam name="T">Type of the argument to get. This type is used to avoid boxing.</typeparam>
    /// <param name="index">Index of the argument to get.</param>
    /// <returns>Argument at the requested index.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter type <typeparamref name="T"/> is not assignable, boxeable nor unboxeable to the runtime type of the parameter or <paramref name="index"/> is not a valid index of parameters.</exception>
    public T GetParameter<T>(int index)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;

    /// <summary>
    /// Stores the result of the call.
    /// The generic parameter <typeparamref name="T"/> is for convenience in order to avoid boxing, the method is valid to be called with any type as long as the runtime type of <paramref name="value"/> can be assigned, boxed or unboxed as a result (supports contravariance).<br/>
    /// If a delegate doesn't returns a value, it must still call this method, using an <see cref="object"/> generic argument and a <see langword="null"/> value in order to inform the delegate that it was executed successfuly.<br/>
    /// It's left undefined what happens when this method is called more than once.
    /// </summary>
    /// <typeparam name="T">Type of the result. This type is used to avoid boxing.</typeparam>
    /// <param name="value">Return value of the call.</param>
    /// <exception cref="ArgumentException">Thrown when the runtime type of <paramref name="value"/> is not assignable, boxeable nor unboxeable to a supported type of this helper.</exception>
    public void SetResult<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
}
