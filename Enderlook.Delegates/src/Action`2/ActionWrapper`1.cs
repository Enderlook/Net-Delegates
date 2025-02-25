using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action{T1, T2}"/> in order to implement <see cref="IAction{T1, T2}"/>.
/// </summary>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly struct ActionWrapper<T1, T2> : IAction<T1, T2>
{
    private static readonly Action<T1, T2> Shared = new((arg1, arg2) => { });

    internal readonly Action<T1, T2> callback;

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper() => callback = Shared;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper(Action<T1, T2> callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IAction{T1, T2}.Invoke{U1, U2}(U1, U2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2
        => callback(arg1, arg2);

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        callback(arg1, arg2);
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => SignatureVoid<T1, T2>.Array;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        callback(arg1, arg2);
        return null;
    }
#endif

    /// <summary>
    /// Extract the wrapped callback.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Action<T1, T2>(ActionWrapper<T1, T2> callback) => callback.callback;

    /// <summary>
    /// Wrap an callback.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <returns>Wrapper of callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ActionWrapper<T1, T2>(Action<T1, T2> callback) => new(callback);
}