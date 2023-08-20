using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed TState T1, T2 TResult"/> in order to implement <see cref="IFunc{T1, T2, TResult}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly struct StatedFuncPointer<TState, T1, T2, TResult> : IFunc<T1, T2, TResult>
{
    private readonly delegate* managed<TState, T1, T2, TResult> callback;
    private readonly TState state;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedFuncPointer(delegate* managed<TState, T1, T2, TResult> callback, TState state)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke{U1, U2}(U1, U2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2
        => callback(state, arg1, arg2);

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        return callback(state, arg1, arg2);
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        return callback(state, arg1, arg2);
    }
#endif

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke{U1, U2, TAction}(U1, U2, TAction)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IFunc<T1, T2, TResult>.Invoke<U1, U2, TAction>(U1 arg1, U2 arg2, TAction callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        callback.Invoke(this.callback(state, arg1, arg2));
    }
}