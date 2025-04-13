using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.Builder;

/// <summary>
/// Represent an implementation of <see cref="ISafeDelegateInvocationHelper"/> which doesn't have any parameter and accepts a return value.
/// </summary>
#if NET9_0_OR_GREATER
public ref struct NoArgumentsInvocationHelper<TResult> : ISafeDelegateInvocationHelper
    where TResult : allows ref struct
#else
public struct NoArgumentsInvocationHelper<TResult> : ISafeDelegateInvocationHelper
#endif
{
    private TResult? result;
    private bool hasResult;

    readonly int ISafeDelegateInvocationHelper.ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0;
    }

    readonly bool ISafeDelegateInvocationHelper.AcceptsReturn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool ISafeDelegateInvocationHelper.AcceptsParameterType(int index, Type type) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool ISafeDelegateInvocationHelper.AcceptsReturnType(Type type) => typeof(TResult).IsAssignableFrom(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool ISafeDelegateInvocationHelper.TryGetParameter<T>(int index, out T? value) where T : default
    {
        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T IDelegateInvocationHelper.GetParameter<T>(int index)
    {
        Helper.ThrowArgumentException_Parameter();
        return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDelegateInvocationHelper.SetResult<T>(T? value) where T : default
    {
        if (hasResult) Helper.ThrowInvalidOperationException_AlreadyHasResult();
        result = CasterHelper<T, TResult>.TryCast(value, out bool can);
        if (!can) Helper.ThrowArgumentException_Return();
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