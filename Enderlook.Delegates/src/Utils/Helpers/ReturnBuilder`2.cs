using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Bulder of a delegate invocation helper that have parameters and accepts a return value.
/// </summary>
/// <typeparam name="TArguments">Type that contains the arguments.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct ReturnBuilder<TArguments, TResult>
#if NET9_0_OR_GREATER
        where TArguments : IArgumentsBuilder, allows ref struct
        where TResult : allows ref struct
#else
        where TArguments : IArgumentsBuilder
#endif
{
    internal readonly TArguments arguments;

    internal ReturnBuilder(TArguments arguments) => this.arguments = arguments;

    /// <summary>
    /// Make the delegate invocation helper to have another parameter.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    /// <param name="arg">Argument to include.</param>
    /// <returns>New builder instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReturnBuilder<Next<TArguments, TArgument>, TResult> WithArgument<TArgument>(TArgument arg)
#if NET9_0_OR_GREATER
        where TArgument : allows ref struct
#endif
        => new(new(arguments, arg));

    /// <summary>
    /// Completes the construction of the delegate invocation helper.
    /// </summary>
    /// <param name="builder">Current delegate invocation helper builder to base from.</param>
    /// <returns>New constructed delegate invocation helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReturnHelper<TArguments, TResult> Build()
        => new(arguments);
}