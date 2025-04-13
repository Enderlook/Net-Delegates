using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

internal abstract class CasterHelper<TFrom, TTo>
#if NET9_0_OR_GREATER
    where TFrom : allows ref struct
    where TTo : allows ref struct
#endif
{
#if NET9_0_OR_GREATER
    private static readonly CasterHelper<TFrom, TTo>? Impl = (typeof(TFrom).IsByRefLike || typeof(TTo).IsByRefLike || (typeof(TFrom).IsValueType && typeof(TTo) == typeof(TFrom)))
        ? null
        : (CasterHelper<TFrom, TTo>?)Activator.CreateInstance(typeof(CastHelperImplementation<,>).MakeGenericType(new Type[] { typeof(TFrom), typeof(TTo) }));
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(from))]
    public static TTo Cast(scoped in TFrom from)
    {
        if (typeof(TFrom).IsValueType && typeof(TFrom) == typeof(TTo))
        {
#if NET9_0_OR_GREATER
            return Unsafe.BitCast<TFrom, TTo>(from);
#else
            return Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in from));
#endif
        }

#if NET9_0_OR_GREATER
        if (typeof(TFrom).IsByRefLike || typeof(TTo).IsByRefLike)
            Helper.ThrowInvalidCastException();
#endif

#if NET9_0_OR_GREATER
        return Impl!.Cast_(from);
#else
        return (TTo)(object)from;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo? TryCast(scoped in TFrom from, out bool can)
    {
        // Return value rather than boolean because this avoids a write barrier.

        if (typeof(TFrom).IsValueType && typeof(TFrom) == typeof(TTo))
        {
            can = true;
#if NET9_0_OR_GREATER
            return Unsafe.BitCast<TFrom, TTo>(from);
#else
            return Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in from));
#endif
        }

#if NET9_0_OR_GREATER
        if (typeof(TFrom).IsByRefLike || typeof(TTo).IsByRefLike)
        {
            can = false;
            return default;
        }
#endif

#if NET9_0_OR_GREATER
        return Impl!.TryCast_(from, out can);
#else
        if (from is TTo v)
        {
            can = true;
            return v;
        }
        can = !typeof(TTo).IsValueType && from is null;
        return default;
#endif
    }

#if NET9_0_OR_GREATER
    [return: NotNullIfNotNull(nameof(from))]
    protected abstract TTo Cast_(scoped in TFrom from);

    protected abstract TTo TryCast_(scoped in TFrom from, out bool can);
#endif
}

#if NET9_0_OR_GREATER
internal sealed class CastHelperImplementation<TFrom, TTo> : CasterHelper<TFrom, TTo>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override TTo Cast_(scoped in TFrom from) => (TTo)(object)from;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override TTo TryCast_(scoped in TFrom from, out bool can)
    {
        if (from is TTo v)
        {
            can = true;
            return v;
        }
        can = !typeof(TTo).IsValueType && from is null;
        return default!;
    }
}
#endif