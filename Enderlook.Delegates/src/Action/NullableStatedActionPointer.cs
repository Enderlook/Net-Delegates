using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed void"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
public unsafe readonly partial struct NullableStatedActionPointer<TState> : IAction
{
    private readonly delegate* managed<TState, void> callback;
    private readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableStatedActionPointer()
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
    public NullableStatedActionPointer(delegate* managed<TState, void> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke()
    {
        delegate*<TState, void> callback = this.callback;
        if (callback is not null)
            callback(state);
    }
}