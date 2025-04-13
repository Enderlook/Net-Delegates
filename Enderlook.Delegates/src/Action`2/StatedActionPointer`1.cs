using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed TState T1 T2 void"/> in order to implement <see cref="IAction{T1, T2}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly partial struct StatedActionPointer<TState, T1, T2> : IAction<T1, T2>
#if NET9_0_OR_GREATER
    where T1: allows ref struct
    where T2: allows ref struct
#endif
{
    internal readonly delegate* managed<TState, T1, T2, void> callback;
    internal readonly TState state;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedActionPointer()
    {
        callback = &Dummy;
        state = default!;
    }

    private static void Dummy(TState state, T1 arg1, T2 arg2) { }

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedActionPointer(delegate* managed<TState, T1, T2, void> callback, TState state)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IAction{T1, T2}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T1 arg1, T2 arg2) => callback(state, arg1, arg2);
}