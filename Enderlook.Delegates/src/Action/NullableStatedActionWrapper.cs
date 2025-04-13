using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T}"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
public readonly partial struct NullableStatedActionWrapper<TState> : IAction
{
    private readonly Action<TState>? callback;
    private readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionWrapper()
    {
        callback = default;
        state = default!;
    }

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
}