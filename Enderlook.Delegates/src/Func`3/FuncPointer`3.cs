using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see langword="delegate* managed T1 T2 TResult"/> in order to implement <see cref="IFunc{T1, T2, TResult}"/>.
/// </summary>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public unsafe readonly partial struct FuncPointer<T1, T2, TResult> : IFunc<T1, T2, TResult>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TResult : allows ref struct
#endif
{
    private readonly delegate* managed<T1, T2, TResult> callback;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncPointer(delegate* managed<T1, T2, TResult> callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke(T1 arg1, T2 arg2)
        => callback(arg1, arg2);
}
