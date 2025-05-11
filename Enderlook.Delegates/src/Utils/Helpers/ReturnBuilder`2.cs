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
}