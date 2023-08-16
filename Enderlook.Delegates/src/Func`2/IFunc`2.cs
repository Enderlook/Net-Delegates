using System.Diagnostics.CodeAnalysis;

namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter and returns a value.
/// </summary>
/// <typeparam name="T">Type of the parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public interface IFunc<in T, out TResult> : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <typeparam name="U">Specialized type of <typeparamref name="T"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <returns>Return value of the callback.</returns>
    public TResult Invoke<U>(U arg) where U : T;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <typeparam name="U">Specialized type of <typeparamref name="T"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="TAction">Type of callback.</typeparam>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2__OR_GREATER
        virtual
#endif
        void Invoke<U, TAction>(U arg, [NotNull] TAction callback)
        where U : T
        where TAction : IAction<TResult>
#if NET5_0_OR_GREATER || NETSTANDARD2__OR_GREATER
        => callback.Invoke(Invoke(arg))
#endif
        ;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg);
        return Invoke(arg);
    }

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T arg);
        return Invoke(arg);
    }
#endif
}