using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed TState T1 T2 void"/> in order to implement <see cref="IAction{T1, T2}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
public unsafe readonly partial struct NullableStatedActionPointer<TState, T1, T2> : IAction<T1, T2>
#if NET9_0_OR_GREATER
    where T1: allows ref struct
    where T2: allows ref struct
#endif
{
    private readonly delegate* managed<TState, T1, T2, void> callback;
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
    public NullableStatedActionPointer(delegate* managed<TState, T1, T2, void> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T1, T2}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T1 arg1, T2 arg2)
    {
        delegate*<TState, T1, T2, void> callback = this.callback;
        if (callback is not null)
        {
            callback(state, arg1, arg2);
        }
    }
}