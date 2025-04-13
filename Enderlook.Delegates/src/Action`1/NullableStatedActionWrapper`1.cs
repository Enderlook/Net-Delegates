using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T1, T2}"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T">Type of parameter.</typeparam>
public readonly partial struct NullableStatedActionWrapper<TState, T> : IAction<T>
#if NET9_0_OR_GREATER
    where T: allows ref struct
#endif
{
    private readonly Action<TState, T>? callback;
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
    public NullableStatedActionWrapper(Action<TState, T> callback, TState state)
    {
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T arg) => callback?.Invoke(state, arg);
}