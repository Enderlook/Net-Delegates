using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Func{T}"/> in order to implement <see cref="IFunc{T, TResult}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly struct StatedFuncWrapper<TState, T, TResult> : IFunc<T, TResult>
{
    private readonly Func<TState, T, TResult> callback;
    private readonly TState state;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedFuncWrapper(Func<TState, T, TResult> callback, TState state)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke{U}(U)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke<U>(U arg1) where U : T => callback(state, arg1);

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke{U, TAction}(U, TAction)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IFunc<T, TResult>.Invoke<U, TAction>(U arg1, TAction callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        callback.Invoke(this.callback(state, arg1));
    }

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke{U, TFunc, TResult2}(U, TFunc)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TResult2 IFunc<T, TResult>.Invoke<U, TFunc, TResult2>(U arg, TFunc callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        return callback.Invoke(this.callback(state, arg));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg);
        return callback(state, arg);
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => Signature<T, TResult>.Array;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T arg);
        return callback(state, arg);
    }
#endif
}