using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.Builder;

/// <summary>
/// Represent the first argument of a delegate invocation helper builder.
/// </summary>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct DelegateInvocationHelperBuilderValue<TValue> : IDelegateInvocationHelperParameterBuilder
#if NET9_0_OR_GREATER
        where TValue : allows ref struct
#endif
{
    private readonly TValue value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal DelegateInvocationHelperBuilderValue(TValue value) => this.value = value;

    readonly int IDelegateInvocationHelperParameterBuilder.ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T IDelegateInvocationHelperParameterBuilder.GetParameter<T>(int i)
    {
        if (i != 0) Helper.ThrowArgumentException_Parameter();
        return CasterHelper<TValue, T>.Cast(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T? IDelegateInvocationHelperParameterBuilder.TryGetParameter<T>(int i, out bool can) where T : default
    {
        if (i != 0) Helper.ThrowArgumentException_Parameter();
        return CasterHelper<TValue?, T?>.TryCast(value, out can);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IDelegateInvocationHelperParameterBuilder.AcceptsParameterType(int i, Type type)
    {
        if (i != 0) Helper.ThrowArgumentException_Parameter();
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (type.IsByRefLike)
            return typeof(TValue) == type;
        else
#endif
            return type.IsAssignableFrom(typeof(TValue));
    }
}
