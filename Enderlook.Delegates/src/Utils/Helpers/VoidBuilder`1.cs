namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Bulder of a delegate invocation helper that have parameters but doesn't accepts a return value.
/// </summary>
/// <typeparam name="TArguments">Type that contains the arguments.</typeparam>
public readonly
#if NET9_0_OR_GREATER
    ref
#endif
    struct VoidBuilder<TArguments>
#if NET9_0_OR_GREATER
        where TArguments : IArgumentsBuilder, allows ref struct
#else
        where TArguments : IArgumentsBuilder
#endif
{
    internal readonly TArguments arguments;

    internal VoidBuilder(TArguments arguments) => this.arguments = arguments;
}
