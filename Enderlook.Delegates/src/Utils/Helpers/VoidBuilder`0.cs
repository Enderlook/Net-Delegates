using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Builder of a delegate invocation helper that doesn't have parameter nor accepts a return value.
/// </summary>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct VoidBuilder
{
    /// <summary>
    /// Make the delegate invocation helper builder to accept a return value.
    /// </summary>
    /// <returns>New builder instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReturnBuilder<TResult> WithReturn<TResult>()
#if NET9_0_OR_GREATER
        where TResult : allows ref struct
#endif
        => new();

    /// <summary>
    /// Make the delegate invocation helper to have a parameter.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    /// <param name="builder">Current delegate invocation helper builder to base from.</param>
    /// <param name="arg">Argument to include.</param>
    /// <returns>New builder instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VoidBuilder<Value<TArgument>> WithArgument<TArgument>(TArgument arg)
#if NET9_0_OR_GREATER
        where TArgument : allows ref struct
#endif
        => new(new(arg));

    /// <summary>
    /// Completes the construction of the delegate invocation helper.
    /// </summary>
    /// <returns>New constructed delegate invocation helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VoidHelper Build()
        => new();
}
