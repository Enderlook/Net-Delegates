using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.Builder;

#if NET9_0_OR_GREATER
internal ref struct ReadOnlySpanAdapter<TResult>(ReadOnlySpan<object?> args) : ISafeDelegateInvocationHelper
#else
internal struct ReadOnlySpanAdapter<TResult>(object?[] args) : ISafeDelegateInvocationHelper
#endif
{
#if NET9_0_OR_GREATER
    private readonly ReadOnlySpan<object?> span = args;
#else
    private readonly object?[] span = args;
#endif
    private TResult? result = default;
    private bool hasResult = false;

    public readonly int ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => span.Length;
    }

    public readonly bool AcceptsReturn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsReturnType(Type type) => typeof(TResult).IsAssignableFrom(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsParameterType(int index, Type type)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (type.IsByRefLike)
            return false;
#endif

        ReadOnlySpan<object?> span = this.span;
        if (unchecked((uint)index >= (uint)span.Length))
            return false;

        object? v = span[index];
        return v is null ? !type.IsValueType : type.IsAssignableFrom(v.GetType());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetParameter<T>(int index)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        T? value;
        ReadOnlySpan<object?> span = this.span;
        if (unchecked((uint)index < (uint)span.Length))
        {
            value = CasterHelper<object?, T>.TryCast(span[index], out bool can);
            if (can)
            {
                goto end;
            }
        }
        Helper.ThrowArgumentException_Parameter();
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out value);
#else
        value = default;
#endif
    end:
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetParameter<T>(int index, out T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        ReadOnlySpan<object?> span = this.span;
        if (unchecked((uint)index >= (uint)span.Length))
        {
            value = default;
            return false;
        }
        value = CasterHelper<object?, T>.TryCast(span[index], out bool can);
        return can;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
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
