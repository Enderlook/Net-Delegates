using Enderlook.Delegates.Builder;

using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Helper methods for types which implements <see cref="IDelegate"/>.
/// </summary>
public static class DelegateHelpers
{
#if NET9_0_OR_GREATER
    /// <summary>
    /// Executes the delegate.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="args">Argument passed to the delegate.</param>
    /// <param name="result">Result of the executed delegate, or <see langword="null"/> if the delegate returns <see cref="void"/>.</param>
    /// <returns><see langword="true"/> if the invocation helper was valid for this delegate. Otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Throw when <paramref name="callback"/> didn't set a result.</exception>
    public static bool TryDynamicInvoke<TDelegate>(this TDelegate callback, out object? result, params ReadOnlySpan<object?> args)
        where TDelegate : IDelegate
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        ReadOnlySpanAdapter<object?> adapter = new(args);
        bool success = callback.TryDynamicInvoke(ref adapter);
        if (!adapter.TryGetResult(out result)) Helper.ThrowArgumentException_NoReturn();
        return success;
    }

    /// <summary>
    /// Executes the delegate unsafety without doing checks to validate the signature nor check if the <paramref name="callback"/> is not <see langword="null"/>.<br/>
    /// If the invocation arguments are not valid for the specific delegate, it's undefined.<br/>
    /// You should ensure that the arguments are supported by the signature specified at <see cref="IDelegate.GetDynamicSignature"/>.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="args">Argument passed to the delegate.</param>
    /// <returns>Result of the executed delegate, or <see langword="null"/> if the delegate returns <see cref="void"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when number of arguments is insufficient or the argument types types doesn't match the required by <paramref name="callback"/>.</exception>
    /// <exception cref="InvalidOperationException">Throw when <paramref name="callback"/> didn't set a result.</exception>
    public static object? DynamicInvokeUnsafe<TDelegate>(this TDelegate callback, params ReadOnlySpan<object?> args)
        where TDelegate : IDelegate
    {
        ReadOnlySpanAdapter<object?> adapter = new(args);
        callback.DynamicInvoke(ref adapter);
        if (!adapter.TryGetResult(out object? result)) Helper.ThrowArgumentException_NoReturn();
        return result;
    }
#endif

    /// <summary>
    /// Executes the delegate.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="args">Argument passed to the delegate.</param>
    /// <param name="result">Result of the executed delegate, or <see langword="null"/> if the delegate returns <see cref="void"/>.</param>
    /// <returns><see langword="true"/> if the invocation helper was valid for this delegate. Otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Throw when <paramref name="callback"/> didn't set a result.</exception>
    public static bool TryDynamicInvoke<TDelegate>(this TDelegate callback, out object? result, object?[] args)
        where TDelegate : IDelegate
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        ReadOnlySpanAdapter<object?> adapter = new(args);
        bool success = callback.TryDynamicInvoke(ref adapter);
        if (!adapter.TryGetResult(out result)) Helper.ThrowArgumentException_NoReturn();
        return success;
    }

    /// <summary>
    /// Executes the delegate unsafety without doing checks to validate the signature nor check if the <paramref name="callback"/> is not <see langword="null"/>.<br/>
    /// If the invocation arguments are not valid for the specific delegate, it's left undefined if the delegated was executed or not.<br/>
    /// You should ensure that the arguments are supported by the signature specified at <see cref="IDelegate.GetDynamicSignature"/>.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="args">Argument passed to the delegate.</param>
    /// <returns>Result of the executed delegate, or <see langword="null"/> if the delegate returns <see cref="void"/>.
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when number of arguments is insufficient or the argument types types doesn't match the required by <paramref name="callback"/>.</exception>
    /// <exception cref="InvalidOperationException">Throw when <paramref name="callback"/> didn't set a result.</exception>
    public static object? DynamicInvokeUnsafe<TDelegate>(this TDelegate callback, params object?[] args)
        where TDelegate : IDelegate
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        ReadOnlySpanAdapter<object?> adapter = new(args);
        callback.DynamicInvoke(ref adapter);
        if (!adapter.TryGetResult(out object? result)) Helper.ThrowArgumentException_NoReturn();
        return result;
    }

    /// <summary>
    /// Creates an instance of an invocation helper that doesn't have parameters not accepts a return value.
    /// </summary>
    /// <returns>New instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NoArgumentsInvocationHelper WithoutReturn() => new();

    /// <summary>
    /// Creates an instance of an invocation helper that doesn't have parameters and accepts a return value.
    /// </summary>
    /// <returns>New instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NoArgumentsInvocationHelper<TResult> WithReturn<TResult>()
#if NET9_0_OR_GREATER
        where TResult : allows ref struct
#endif
        => new();

    /// <summary>
    /// Creates a builder for a delegate invocation helper and set the first argument.
    /// </summary>
    /// <typeparam name="TArg">Type of the first argument.</typeparam>
    /// <param name="argument">Value of the first argument.</param>
    /// <returns>New builder.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DelegateInvocationHelperBuilderValue<TArg> WithArgument<TArg>(TArg argument)
#if NET9_0_OR_GREATER
        where TArg : allows ref struct
#endif
        => new(argument);

    /// <summary>
    /// Chains the following argument to the builder of the delegate invocation helper.
    /// </summary>
    /// <typeparam name="TArgs">Type of current builder.</typeparam>
    /// <typeparam name="TArg">Type of the new argument.</typeparam>
    /// <param name="previous">Current builder.</param>
    /// <param name="next">Value of the next argument.</param>
    /// <returns>New builder with the additional argument.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DelegateInvocationHelperBuilderNext<TArgs, TArg> NextArgument<TArgs, TArg>(this TArgs previous, TArg next)
#if NET9_0_OR_GREATER
        where TArgs : IDelegateInvocationHelperParameterBuilder, allows ref struct
        where TArg : allows ref struct
#else
        where TArgs : IDelegateInvocationHelperParameterBuilder
#endif
        => new(previous, next);

    /// <summary>
    /// Completes the delegate invocation helper builder with a return value.
    /// </summary>
    /// <typeparam name="TArgs">Type of current builder.</typeparam>
    /// <typeparam name="TResult">Type of the return value</typeparam>
    /// <param name="arguments">Current builder.</param>
    /// <returns>New completed delegate invocation helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DelegateInvocationHelper<TArgs, TResult> WithResult<TArgs, TResult>(this TArgs arguments)
#if NET9_0_OR_GREATER
        where TArgs : IDelegateInvocationHelperParameterBuilder, allows ref struct
        where TResult : allows ref struct
#else
        where TArgs : IDelegateInvocationHelperParameterBuilder
#endif
        => new(arguments);

    /// <summary>
    /// Completes the delegate invocation helper builder with a return value.
    /// </summary>
    /// <typeparam name="TArgs">Type of current builder.</typeparam>
    /// <param name="arguments">Current builder.</param>
    /// <returns>New completed delegate invocation helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DelegateInvocationHelper<TArgs> WithoutResult<TArgs>(this TArgs arguments)
#if NET9_0_OR_GREATER
        where TArgs : IDelegateInvocationHelperParameterBuilder, allows ref struct
#else
        where TArgs : IDelegateInvocationHelperParameterBuilder
#endif
        => new(arguments);
}