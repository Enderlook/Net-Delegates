using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed T void"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
public unsafe readonly partial struct NullableActionPointer<T> : IAction<T>
#if NET9_0_OR_GREATER
    where T: allows ref struct
#endif
{
    private readonly delegate* managed<T, void> callback;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableActionPointer() { }

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableActionPointer(delegate* managed<T, void> callback) => this.callback = callback;

    /// <inheritdoc cref="IAction{T}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T arg)
    {
        delegate*<T, void> callback = this.callback;
        if (callback is not null)
            callback(arg);
    }
}