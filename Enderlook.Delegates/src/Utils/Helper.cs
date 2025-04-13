using System.Diagnostics.CodeAnalysis;

namespace Enderlook.Delegates;

internal static class Helper
{
    public static readonly Type[] VoidArray = [typeof(void)];
    public unsafe static readonly delegate* managed<void> PointerAction = &Dummy;
    public static readonly Action DelegateAction = new(() => { });

    private static void Dummy() { }

    public static void ThrowArgumentException_Return() => throw new ArgumentException("Return value can't be assigned, boxed nor unboxed to an acceptable return type.");

    public static void ThrowArgumentException_NoReturn() => throw new ArgumentException("Doesn't accepts a return value.");

    public static void ThrowArgumentException_Parameter() => throw new ArgumentException("Index out of range or parameter can't be assigned, boxed nor unboxed to the specified type.");

    [DoesNotReturn]
    public static void ThrowArgumentNullException_Callback() => throw new ArgumentNullException("callback");

    [DoesNotReturn]
    public static void ThrowInvalidCastException() => throw new InvalidCastException("Either TTo or TFrom is a ref struct (and aren't the same type), which is not valid.");

    public static void ThrowInvalidOperationException_NoResult() => throw new InvalidOperationException("Result of delegate invocation helper was never set.");

    public static void ThrowInvalidOperationException_AlreadyHasResult() => throw new InvalidOperationException("Already have a result, can't accepts another.");
}

internal static class SignatureVoid<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public static readonly Action<T> DelegateAction = new(_ => { });
    public unsafe static readonly delegate* managed<T, void> PointerAction = &Dummy;
    public static readonly Type[] Array = [typeof(void), typeof(T)];
    private static void Dummy(T arg) { }
}

internal static class SignatureVoid<T1, T2>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
#endif
{
    public static readonly Action<T1, T2> DelegateAction = new((_, _) => { });
    public unsafe static readonly delegate* managed<T1, T2, void> PointerAction = &Dummy;
    public static readonly Type[] Array = [typeof(void), typeof(T1), typeof(T2)];
    private static void Dummy(T1 arg1, T2 arg2) { }
}

internal static class Signature<TResult>
#if NET9_0_OR_GREATER
    where TResult : allows ref struct
#endif
{
    public static readonly Type[] Array = [typeof(TResult)];
}

internal static class Signature<T, TResult>
#if NET9_0_OR_GREATER
    where T : allows ref struct
    where TResult : allows ref struct
#endif
{
    public static readonly Type[] Array = [typeof(TResult), typeof(T)];
}

internal static class Signature<T1, T2, TResult>
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TResult : allows ref struct
#endif
{
    public static readonly Type[] Array = [typeof(TResult), typeof(T1), typeof(T2)];
}