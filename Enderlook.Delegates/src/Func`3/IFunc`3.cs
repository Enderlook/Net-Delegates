namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter and returns a value.
/// </summary>
/// <typeparam name="T1">Type of the parameter.</typeparam>
/// <typeparam name="T2">Type of the parameter</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public interface IFunc<in T1, in T2, out TResult> : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg1">First argument to pass as parameter.</param>
    /// <param name="arg2">Second argument to pass as parameter.</param>
    /// <typeparam name="U1">Specialized type of <typeparamref name="T1"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="U2">Specialized type of <typeparamref name="T2"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <returns>Return value of the callback.</returns>
    public abstract TResult Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="U1">Specialized type of <typeparamref name="T1"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="U2">Specialized type of <typeparamref name="T2"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="TAction">Type of callback.</typeparam>
    /// <param name="arg1">First argument to pass as parameter.</param>
    /// <param name="arg2">Second argument to pass as parameter.</param>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        virtual
#endif
        void Invoke<U1, U2, TAction>(U1 arg1, U2 arg2, TAction callback)
        where U1 : T1
        where U2 : T2
        where TAction : IAction<TResult>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => callback.Invoke(Invoke(arg1, arg2))
#endif
        ;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="U1">Specialized type of <typeparamref name="T1"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="U2">Specialized type of <typeparamref name="T2"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="TFunc">Type of callback.</typeparam>
    /// <typeparam name="TResult2">Type of callback result.</typeparam>
    /// <param name="arg1">First argument to pass as parameter.</param>
    /// <param name="arg2">Second argument to pass as parameter.</param>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <returns>Return value of <paramref name="callback"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        virtual
#endif
        TResult2 Invoke<U1, U2, TFunc, TResult2>(U1 arg1, U2 arg2, TFunc callback)
        where U1 : T1
        where U2 : T2
        where TFunc : IFunc<TResult, TResult2>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => callback.Invoke(Invoke(arg1, arg2))
#endif
        ;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        return Invoke(arg1, arg2);
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => Signature<T1, T2, TResult>.Array;

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        return Invoke(arg1, arg2);
    }
#endif
}