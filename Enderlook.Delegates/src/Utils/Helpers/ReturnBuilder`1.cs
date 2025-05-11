using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Builder of a delegate invocation helper that doesn't have parameter but accepts a return value.
/// </summary>
/// <typeparam name="TResult">Type of the return value.</typeparam>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct ReturnBuilder<TResult>
#if NET9_0_OR_GREATER
        where TResult : allows ref struct
#endif
{
    /// <summary>
    /// Make the delegate invocation helper to have a parameter.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    /// <param name="arg">Argument to include.</param>
    /// <returns>New builder instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReturnBuilder<Value<TArgument>, TResult> WithArgument<TArgument>(TArgument arg)
#if NET9_0_OR_GREATER
        where TArgument : allows ref struct
#endif
        => new(new(arg));

    /// <summary>
    /// Completes the construction of the delegate invocation helper.
    /// </summary>
    /// <returns>New constructed delegate invocation helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReturnHelper<TResult> Build()
        => new();

}
