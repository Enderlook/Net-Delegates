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
}
