using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Func{TResult}"/> in order to implement <see cref="IFunc{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly struct FuncWrapper<TResult> : IFunc<TResult>
{
    private readonly Func<TResult> callback;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncWrapper(Func<TResult> callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IFunc{TResult}.Invoke()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke() => callback();

    /// <inheritdoc cref="IFunc{TResult}.Invoke{TAction}(TAction)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IFunc<TResult>.Invoke<TAction>(TAction callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        callback.Invoke(this.callback());
    }

    /// <inheritdoc cref="IFunc{TResult}.Invoke{TFunc, TResult2}(TFunc)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TResult2 IFunc<TResult>.Invoke<TFunc, TResult2>(TFunc callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        return callback.Invoke(this.callback());
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        return callback();
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => Signature<TResult>.Array;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        return callback();
    }
#endif

    /// <summary>
    /// Extract the wrapped callback.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Func<TResult>(FuncWrapper<TResult> callback) => callback.callback;

    /// <summary>
    /// Wrap an callback.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <returns>Wrapper of callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FuncWrapper<TResult>(Func<TResult> callback) => new(callback);
}