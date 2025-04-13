using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed void"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly partial struct StatedActionPointer<TState> : IAction
{
    internal readonly delegate* managed<TState, void> callback;
    internal readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedActionPointer()
    {
        callback = SignatureVoid<TState>.PointerAction;
        state = default!;
    }
    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedActionPointer(delegate* managed<TState, void> callback, TState state)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke() => callback(state);
}