namespace Enderlook.Delegates;

/// <summary>
/// Represent extra information about the invocation helper that allows to perform a safer invocation of the delegate.
/// </summary>
public interface ISafeDelegateInvocationHelper : IDelegateInvocationHelper
{
    /// <summary>
    /// Number of parameters stored in this invocation helper.<br/>
    /// See <see cref="IDelegateInvocationHelper.GetParameter{T}(int)"/>.
    /// </summary>
    public int ParametersCount { get; }

    /// <summary>
    /// Determines if the invocation helper accepts a return value.<br/>
    /// Even if the invocation helper doesn't accept a return value, <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> should still be called with a <see cref="object"/> generic argument and a <see langword="null"/> value.<br/>
    /// Delegates that doesn't return can still be called even if this returns <see langword="true"/>, as long as <see cref="AcceptsReturnType(Type)"/> returns <see langword="true"/> for <see cref="object"/> type, since a <see langword="null"/> value is passed at the end of the invocation.
    /// </summary>
    public bool AcceptsReturn { get; }

    /// <summary>
    /// Determines if it's capable of accepting the specified return type and all its derived types.<br/>
    /// This would be the type of the return value and not necessary the type of the generic argument in <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/>, since the generic argument can be a less derived type, as long as the runtime value is of a valid type.<br/>
    /// If the invocation helper doesn't accept a return value, then <see cref="AcceptsReturn"/> and this method should always return <see langword="false"/>, regardless of argument type. And despite of this, <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> should still be called with a <see cref="object"/> generic argument and a <see langword="null"/> value.
    /// </summary>
    /// <param name="type">Return type to check.</param>
    /// <returns><see langword="true"/> if the type is accepted. Otherwise, <see langword="false"/>.</returns>
    public bool AcceptsReturnType(Type type);

    /// <summary>
    /// Determines if it's capable of returning the specified parameter type and all its basal types at the specified index.<br/>
    /// In order words, determines if <see cref="TryGetParameter{T}(int, out T?)"/> would return <see langword="true"/> and if <see cref="IDelegateInvocationHelper.GetParameter{T}(int)"/> would not throw any exception when executed with the same parameters.
    /// </summary>
    /// <param name="index">Index of the argument to get.</param>
    /// <param name="type">Parameter type to check.</param>
    /// <returns><see langword="true"/> if the type is accepted. Otherwise, <see langword="false"/>.<br/>
    /// It also returns <see langword="false"/> if <paramref name="index"/> is negative, equal or greater than <see cref="ParametersCount"/>.</returns>
    public bool AcceptsParameterType(int index, Type type);

    /// <summary>
    /// Try get the argument at the specified index.<br/>
    /// The generic parameter <typeparamref name="T"/> is for convenience in order to avoid boxing, the method is valid to be called with any type that can be assigned, boxed or unboxed to the runtime type of the parameter (supports covariance).
    /// </summary>
    /// <typeparam name="T">Type of the argument to get. This type is used to avoid boxing.</typeparam>
    /// <param name="index">Index of the argument to get.</param>
    /// <param name="value">Argument at the requested index.</param>
    /// <returns><see langword="true"/> if the argument was correctly stored in <paramref name="value"/>. Otherwise <see langword="false"/>.<br/>
    /// It also returns <see langword="false"/> if <paramref name="index"/> is negative, equal or greater than <see cref="ParametersCount"/>.</returns>
    public bool TryGetParameter<T>(int index, out T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    {
        if (AcceptsParameterType(index, typeof(T)))
        {
            value = GetParameter<T>(index);
            return true;
        }
        value = default;
        return false;
    }
#else
    ;
#endif
}