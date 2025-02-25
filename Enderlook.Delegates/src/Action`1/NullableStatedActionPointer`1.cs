using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed TState T void"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T">Type of parameter.</typeparam>
public unsafe readonly struct NullableStatedActionPointer<TState, T> : IAction<T>
{
    private readonly delegate* managed<TState, T, void> callback;
    private readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionPointer() => state = default!;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionPointer(delegate* managed<TState, T, void> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T}.Invoke{U}(U)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke<U>(U arg)
        where U : T
    {
        delegate*<TState, T, void> callback = this.callback;
        if (callback is not null)
        {
            callback(state, arg);
        }
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg);
        delegate*<TState, T, void> callback = this.callback;
        if (callback is not null)
        {
            callback(state, arg);
        }
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => SignatureVoid<T>.Array;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T arg);
        delegate*<TState, T, void> callback = this.callback;
        if (callback is not null)
        {
            callback(state, arg);
        }
        return null;
    }
#endif

    /// <summary>
    /// Cast a non nullable callback into a nullable one.
    /// </summary>
    /// <param name="callback">Callback to cast.</param>
    /// <returns>Casted callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableStatedActionPointer<TState, T>(StatedActionPointer<TState, T> callback) => new(callback.callback, callback.state);
}