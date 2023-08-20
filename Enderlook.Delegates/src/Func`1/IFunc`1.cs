namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback that returns a value.
/// </summary>
/// <typeparam name="TResult">Type of return value.</typeparam>
public interface IFunc<out TResult> : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <returns>Return value of the callback.</returns>
    public abstract TResult Invoke();

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="TAction">Type of callback.</typeparam>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        virtual
#endif
        void Invoke<TAction>(TAction callback)
        where TAction : IAction<TResult>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => callback.Invoke(Invoke())
#endif
        ;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="TFunc">Type of callback.</typeparam>
    /// <typeparam name="TResult2">Type of callback result.</typeparam>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <returns>Return value of <paramref name="callback"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        virtual
#endif
        TResult2 Invoke<TFunc, TResult2>(TFunc callback)
        where TFunc : IFunc<TResult, TResult2>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => callback.Invoke(Invoke())
#endif
        ;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        return Invoke();
    }

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        return Invoke();
    }
#endif
}
