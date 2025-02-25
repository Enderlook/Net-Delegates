using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed void"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly struct NullableActionPointer : IAction
{
    private readonly delegate* managed<void> callback;

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

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        delegate*<void> callback = this.callback;
        if (callback is not null)
            callback();
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Memory<Type> IDelegate.GetSignature() => Helper.VoidArray;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        delegate*<void> callback = this.callback;
        if (callback is not null)
            callback();
        return null;
    }
#endif

    /// <summary>
    /// Extract the wrapped callback.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator delegate* managed<void>(NullableActionPointer callback) => callback.callback;

    /// <summary>
    /// Wrap an callback.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <returns>Wrapper of callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableActionPointer(delegate* managed<void> callback) => new(callback);

    /// <summary>
    /// Cast a non nullable callback into a nullable one.
    /// </summary>
    /// <param name="callback">Callback to cast.</param>
    /// <returns>Casted callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableActionPointer(ActionPointer callback) => new(callback.callback);
}