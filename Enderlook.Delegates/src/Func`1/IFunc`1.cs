using System.Diagnostics.CodeAnalysis;

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
    public TResult Invoke();

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
        void Invoke<TAction>([NotNull] TAction callback)
        where TAction : IAction<TResult>
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
