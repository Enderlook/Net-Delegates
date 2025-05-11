using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Represent an implementation of <see cref="ISafeDelegateInvocationHelper"/> which doesn't have any parameter nor accepts a return value.
/// </summary>
public struct VoidHelper : ISafeDelegateInvocationHelper
{
    private bool hasResult;

    readonly int ISafeDelegateInvocationHelper.ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0;
    }

    readonly bool ISafeDelegateInvocationHelper.AcceptsReturn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool ISafeDelegateInvocationHelper.AcceptsParameterType(int index, Type type) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool ISafeDelegateInvocationHelper.AcceptsReturnType(Type type) => false;

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
        if (typeof(T) != typeof(object) && value is not null)
            Helper.ThrowArgumentException_NoReturn();
        hasResult = true;
    }

    /// <summary>
    /// Determines if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed.
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed. Otherwise, <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetResult() => hasResult;
}