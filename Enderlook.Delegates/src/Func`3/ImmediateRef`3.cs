using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

#if NET9_0_OR_GREATER
/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{T1, T2, TResult}"/>.
/// </summary>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly ref partial struct ImmediateRef<T1, T2, TResult> : IFunc<T1, T2, TResult>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TResult : allows ref struct
#endif
{
    private readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmediateRef(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke(T1 arg1, T2 arg2)
        => value;
}
#endif