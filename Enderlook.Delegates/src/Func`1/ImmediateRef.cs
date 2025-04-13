using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

#if NET9_0_OR_GREATER
/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly ref partial struct ImmediateRef<TResult> : IFunc<TResult>
    where TResult: allows ref struct
{
    internal readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmediateRef(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{TResult}.Invoke()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke() => value;
}
#endif