using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps an immediate value in order to implement <see cref="IFunc{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">Type of return value.</typeparam>
public unsafe readonly partial struct Immediate<TResult> : IFunc<TResult>
{
    internal readonly TResult value;

    /// <summary>
    /// Wraps an <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Immediate(TResult value) => this.value = value;

    /// <inheritdoc cref="IFunc{TResult}.Invoke()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke() => value;
}