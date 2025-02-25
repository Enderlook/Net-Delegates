using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T}"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
public readonly struct NullableStatedActionWrapper<TState> : IAction
{
    private readonly Action<TState>? callback;
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionWrapper(Action<TState> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke() => callback?.Invoke(state);

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        callback?.Invoke(state);
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Memory<Type> IDelegate.GetSignature() => Helper.VoidArray;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        callback?.Invoke(state);
        return null;
    }
#endif

    /// <summary>
    /// Cast a non nullable callback into a nullable one.
    /// </summary>
    /// <param name="callback">Callback to cast.</param>
    /// <returns>Casted callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableStatedActionWrapper<TState>(StatedActionWrapper<TState> callback) => new(callback.callback, callback.state);
}