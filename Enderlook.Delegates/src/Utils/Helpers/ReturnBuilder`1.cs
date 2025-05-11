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
}
