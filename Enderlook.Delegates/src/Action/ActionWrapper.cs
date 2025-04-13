using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly partial struct ActionWrapper : IAction
{
    internal readonly Action callback;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper() => callback = Helper.DelegateAction;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper(Action callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke() => callback();
}
