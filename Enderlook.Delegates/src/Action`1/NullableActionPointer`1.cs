using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed T void"/> in order to implement <see cref="IAction{T}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
public unsafe readonly struct NullableActionPointer<T> : IAction<T>
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

    /// <inheritdoc cref="IAction{T}.Invoke{U}(U)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke<U>(U arg)
        where U : T
    {
        delegate*<T, void> callback = this.callback;
        if (callback is not null)
            callback(arg);
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg);
        delegate*<T, void> callback = this.callback;
        if (callback is not null)
            callback(arg);
        return null;
    }

    /// <summary>
    /// Extract the wrapped callback.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator delegate* managed<T, void>(NullableActionPointer<T> callback) => callback.callback;

    /// <summary>
    /// Wrap an callback.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <returns>Wrapper of callback..</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableActionPointer<T>(delegate* managed<T, void> callback) => new(callback);

    /// <summary>
    /// Cast a non nullable callback into a nullable one.
    /// </summary>
    /// <param name="callback">Callback to cast.</param>
    /// <returns>Casted callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableActionPointer<T>(ActionPointer<T> callback) => new(callback.callback);
}