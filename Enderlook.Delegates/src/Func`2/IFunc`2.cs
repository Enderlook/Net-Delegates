﻿namespace Enderlook.Delegates;

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
    public abstract TResult Invoke<U>(U arg) where U : T;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="U">Specialized type of <typeparamref name="T"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="TAction">Type of callback.</typeparam>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2__OR_GREATER
        virtual
#endif
        void Invoke<U, TAction>(U arg, TAction callback)
        where U : T
        where TAction : IAction<TResult>
#if NET5_0_OR_GREATER || NETSTANDARD2__OR_GREATER
        => callback.Invoke(Invoke(arg))
#endif
        ;

    /// <summary>
    /// Executes this callback, and pass the return value to <paramref name="callback"/>.<br/>
    /// This can be used to avoid boxing or improve devirtualization.
    /// </summary>
    /// <typeparam name="U">Specialized type of <typeparamref name="T"/>, useful to avoid boxing or improve inlining in value types.</typeparam>
    /// <typeparam name="TFunc">Type of callback.</typeparam>
    /// <typeparam name="TResult2">Type of callback result.</typeparam>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <param name="callback">Callback where return value is passed.</param>
    /// <returns>Return value of <paramref name="callback"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    public
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        virtual
#endif
        TResult2 Invoke<U, TFunc, TResult2>(U arg, TFunc callback)
        where U : T
        where TFunc : IFunc<TResult, TResult2>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
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

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => Signature<T, TResult>.Array;

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T arg);
        return Invoke(arg);
    }
#endif
}