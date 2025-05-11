using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Represent the next argument of a delegate invocation helper builder.
/// </summary>
/// <typeparam name="TPrevious">Type of the previous builder.</typeparam>
/// <typeparam name="TValue">Type of the next argument.</typeparam>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct Next<TPrevious, TValue> : IArgumentsBuilder
#if NET9_0_OR_GREATER
    where TPrevious : IArgumentsBuilder, allows ref struct
#else
    where TPrevious: IArgumentsBuilder
#endif
#if NET9_0_OR_GREATER
    where TValue : allows ref struct
#endif
{
    private readonly TValue next;
    private readonly TPrevious previous;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Next(TPrevious previous, TValue next)
    {
        this.next = next;
        this.previous = previous;
    }

    readonly int IArgumentsBuilder.ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1 + previous.ParametersCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T IArgumentsBuilder.GetParameter<T>(int i)
    {
        if (i == 0) return CasterHelper<TValue, T>.Cast(next);
        return previous.GetParameter<T>(i - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T? IArgumentsBuilder.TryGetParameter<T>(int i, out bool can) where T: default
    {
        if (i == 0) return CasterHelper<TValue?, T?>.TryCast(this.next, out can);
        return previous.TryGetParameter<T>(i - 1, out can);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IArgumentsBuilder.AcceptsParameterType(int i, Type type)
    {
        if (i == 0)
        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (type.IsByRefLike)
                return typeof(TValue) == type;
            else
#endif
                return type.IsAssignableFrom(typeof(TValue));
        }
        return previous.AcceptsParameterType(i - 1, type);
    }
}
