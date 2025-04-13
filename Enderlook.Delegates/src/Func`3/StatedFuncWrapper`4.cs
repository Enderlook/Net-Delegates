using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Func{T1}"/> in order to implement <see cref="IFunc{T1, T2, TResult}"/>.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <typeparam name="T1">Type of parameter.</typeparam>
/// <typeparam name="T2">Type of parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
/// <remarks>This type must always be constructed, calling any method on <see langword="default"/> is an error.</remarks>
public readonly partial struct StatedFuncWrapper<TState, T1, T2, TResult> : IFunc<T1, T2, TResult>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TResult : allows ref struct
#endif
{
    private readonly Func<TState, T1, T2, TResult> callback;
    private readonly TState state;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <param name="state">State passed to the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StatedFuncWrapper(Func<TState, T1, T2, TResult> callback, TState state)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
        this.state = state;
    }

    /// <inheritdoc cref="IFunc{T1, T2, TResult}.Invoke(T1, T2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Invoke(T1 arg1, T2 arg2) => callback(state, arg1, arg2);
}