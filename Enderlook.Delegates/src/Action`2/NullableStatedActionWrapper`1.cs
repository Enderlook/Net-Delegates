using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T1, T2, T3}"/> in order to implement <see cref="IAction{T1, T2}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
public readonly struct NullableStatedActionWrapper<TState, T1, T2> : IAction<T1, T2>
{
    private readonly Action<TState, T1, T2>? callback;
    private readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionWrapper() => state = default!;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionWrapper(Action<TState, T1, T2>? callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T1, T2}.Invoke{U1, U2}(U1, U2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2
        => callback?.Invoke(state, arg1, arg2);

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        callback?.Invoke(state, arg1, arg2);
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => SignatureVoid<T1, T2>.Array;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        callback?.Invoke(state, arg1, arg2);
        return null;
    }
#endif

    /// <summary>
    /// Cast a non nullable callback into a nullable one.
    /// </summary>
    /// <param name="callback">Callback to cast.</param>
    /// <returns>Casted callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableStatedActionWrapper<TState, T1, T2>(StatedActionWrapper<TState, T1, T2> callback) => new(callback.callback, callback.state);
}