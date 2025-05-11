namespace Enderlook.Delegates.Builder;

/// <summary>
/// Implementation detail of the library, should not be implemented by consumers.
/// </summary>
public interface IDelegateInvocationHelperParameterBuilder
{
    internal int ParametersCount { get; }

    internal bool AcceptsParameterType(int i, Type type);

    internal T GetParameter<T>(int i)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;

    internal T? TryGetParameter<T>(int i, out bool can)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
}