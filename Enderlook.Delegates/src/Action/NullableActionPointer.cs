using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed void"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly partial struct NullableActionPointer : IAction
{
    internal readonly delegate* managed<void> callback;

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
    public NullableActionPointer(delegate* managed<void> callback) => this.callback = callback;

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke()
    {
        delegate*<void> callback = this.callback;
        if (callback is not null)
            callback();
    }
}