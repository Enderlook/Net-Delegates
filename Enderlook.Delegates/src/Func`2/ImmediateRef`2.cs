using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

#if NET9_0_OR_GREATER
/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{T, TResult}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly ref partial struct ImmediateRef<T, TResult> : IFunc<T, TResult>
#if NET9_0_OR_GREATER
    where T : allows ref struct
    where TResult : allows ref struct
#endif
{
    internal readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmediateRef(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke(T arg) => value;
}
#endif