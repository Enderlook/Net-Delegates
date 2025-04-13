using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T1, T2}"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
public readonly partial struct NullableActionWrapper<T1, T2> : IAction<T1, T2>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
#endif
{
    private readonly Action<T1, T2>? callback;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableActionWrapper() { }

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableActionWrapper(Action<T1, T2>? callback) => this.callback = callback;

    /// <inheritdoc cref="IAction{T1, T2}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T1 arg1, T2 arg2)
        => callback?.Invoke(arg1, arg2);
}