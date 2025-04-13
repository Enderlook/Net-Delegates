using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T}"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly partial struct ActionWrapper<T> : IAction<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    internal readonly Action<T> callback;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper() => callback = SignatureVoid<T>.DelegateAction;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper(Action<T> callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IAction{T}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(T arg)
        => callback(arg);
}