using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action"/> in order to implement <see cref="IAction"/>.
/// </summary>
public readonly partial struct NullableActionWrapper : IAction
{
    internal readonly Action? callback;

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
    public NullableActionWrapper(Action? callback) => this.callback = callback;

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke() => callback?.Invoke();
}
