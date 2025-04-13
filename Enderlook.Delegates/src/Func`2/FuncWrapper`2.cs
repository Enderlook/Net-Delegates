using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Func{T, TResult}"/> in order to implement <see cref="IFunc{T, TResult}"/>.
/// </summary>
/// <typeparam name="T">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly partial struct FuncWrapper<T, TResult> : IFunc<T, TResult>
#if NET9_0_OR_GREATER
    where T : allows ref struct
    where TResult : allows ref struct
#endif
{
    private readonly Func<T, TResult> callback;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncWrapper(Func<T, TResult> callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IFunc{T, TResult}.Invoke(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke(T arg) => callback(arg);
}
