using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{T, TResult}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly struct Immediate<T, TResult> : IFunc<T, TResult>
{
    private readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Immediate(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke{U}(U)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke<U>(U arg) where U : T => value;

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke{U, TAction}(U, TAction)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IFunc<T, TResult>.Invoke<U, TAction>(U arg, [NotNull] TAction callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        callback.Invoke(value);
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg);
        return value;
    }

    /// <summary>
    /// Extract the wrapped value.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TResult(Immediate<T, TResult> callback) => callback.value;

    /// <summary>
    /// Wrap a value.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    /// <returns>Wrapper of value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Immediate<T, TResult>(TResult value) => new(value);
}
