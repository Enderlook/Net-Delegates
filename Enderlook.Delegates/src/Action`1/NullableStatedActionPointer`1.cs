using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed TState T void"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T">Type of parameter.</typeparam>
public unsafe readonly partial struct NullableStatedActionPointer<TState, T> : IAction<T>
#if NET9_0_OR_GREATER
    where T: allows ref struct
#endif
{
    private readonly delegate* managed<TState, T, void> callback;
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
    public NullableStatedActionPointer(delegate* managed<TState, T, void> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T arg)
    {
        delegate*<TState, T, void> callback = this.callback;
        if (callback is not null)
            callback(state, arg);
    }
}