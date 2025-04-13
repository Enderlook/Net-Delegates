using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.Builder;

/// <summary>
/// Represent a builded delegate invocation helper.
/// </summary>
/// <typeparam name="TContent">Type that contains the parameters.</typeparam>
/// <typeparam name="TResult">Type of return.</typeparam>
public
#if NET9_0_OR_GREATER
    ref
#endif
    struct DelegateInvocationHelper<TContent, TResult> : ISafeDelegateInvocationHelper
#if NET9_0_OR_GREATER
    where TContent : IDelegateInvocationHelperParameterBuilder, allows ref struct
    where TResult : allows ref struct
#else
    where TContent: IDelegateInvocationHelperParameterBuilder
#endif
{
    private readonly TContent content;
    private TResult? result = default;
    private bool hasResult = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal DelegateInvocationHelper(TContent content) => this.content = content;

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.ParametersCount"/>
    public readonly int ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => content.ParametersCount;
    }

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsReturn"/>
    public readonly bool AcceptsReturn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsReturnType(Type)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsReturnType(Type type) => typeof(TResult).IsAssignableFrom(type);

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsParameterType(int, Type)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsParameterType(int index, Type type) => content.AcceptsParameterType(index, type);

    /// <inheritdoc cref="IDelegateInvocationHelper.GetParameter{T}(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetParameter<T>(int index)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => content.GetParameter<T>(ParametersCount - index - 1);

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.TryGetParameter{T}(int, out T?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetParameter<T>(int index, out T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        value = content.TryGetParameter<T>(ParametersCount - index - 1, out bool can);
        return can;
    }

    /// <inheritdoc cref="IDelegateInvocationHelper.SetResult{T}(T?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        result = CasterHelper<T?, TResult>.Cast(value);
        hasResult = true;
    }

    /// <summary>
    /// Determines if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed.
    /// </summary>
    /// <param name="result">Result of the invocation, if the method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed. Otherwise, <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetResult([NotNullWhen(true)] out TResult? result)
    {
        result = this.result;
        return hasResult;
    }
}
