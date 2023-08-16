using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{T1, T2, TResult}"/>.
/// </summary>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly struct Immediate<T1, T2, TResult> : IFunc<T1, T2, TResult>
{
    private readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Immediate(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke{U1, U2}(U1, U2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2
        => value;

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke{U1, U2, TAction}(U1, U2, TAction)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IFunc<T1, T2, TResult>.Invoke<U1, U2, TAction>(U1 arg1, U2 arg2, [NotNull] TAction callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        callback.Invoke(value);
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 _, out T2 _);
        return value;
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 _, out T2 _);
        return value;
    }
#endif

    /// <summary>
    /// Extract the wrapped value.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TResult(Immediate<T1, T2, TResult> callback) => callback.value;

    /// <summary>
    /// Wrap a value.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    /// <returns>Wrapper of value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Immediate<T1, T2, TResult>(TResult value) => new(value);
}
