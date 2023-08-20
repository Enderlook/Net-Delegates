using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

internal static class Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters(object?[]? args)
    {
        if ((args?.Length ?? 0) == 0)
            ThrowTargetParameterCountException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters<T>(object?[]? args, out T arg)
    {
        if (args is null || args.Length != 1)
            ThrowTargetParameterCountException();
        arg = Cast<T>(args[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters<T1, T2>(object?[]? args, out T1 arg1, out T2 arg2)
    {
        if (args is null || args.Length != 2)
            ThrowTargetParameterCountException();
        arg1 = Cast<T1>(args[0]);
        arg2 = Cast<T2>(args[1]);
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters<T>(T args)
        where T : ITuple
    {
        if (args is null)
            ThrowArgumentNullException_Args();
        if (args.Length == 0)
            ThrowTargetParameterCountException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters<T, T1>(T args, out T1 arg)
        where T : ITuple
    {
        if (args is null)
            ThrowArgumentNullException_Args();
        if (args.Length != 1)
            ThrowTargetParameterCountException();
        arg = Cast<T1>(args[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetParameters<T, T1, T2>(T args, out T1 arg1, out T2 arg2)
        where T : ITuple
    {
        if (args is null)
            ThrowArgumentNullException_Args();
        if (args.Length != 2)
            ThrowTargetParameterCountException();
        arg1 = Cast<T1>(args[0]);
        arg2 = Cast<T2>(args[1]);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Cast<T>(object? arg)
    {
        if (arg is null)
            return default!;

        try
        {
            return (T)arg;
        }
        catch (InvalidCastException)
        {
            Throw(arg);
            return default!;
        }

        [DoesNotReturn]
        static void Throw(object? arg) => throw new ArgumentException($"Object of type '{arg!.GetType()}' cannot be casted to type '{typeof(T)}'");
    }

    [DoesNotReturn]
    public static void ThrowTargetParameterCountException() => throw new TargetParameterCountException("Parameter count mismatch");

    [DoesNotReturn]
    public static void ThrowArgumentNullException_Callback()
        => throw new ArgumentNullException("callback");

    [DoesNotReturn]
    public static void ThrowArgumentNullException_Args()
        => throw new ArgumentNullException("args");
}