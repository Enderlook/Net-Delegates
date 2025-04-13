namespace Enderlook.Delegates.Builder;

/// <summary>
/// Implementation detail of the library, should not be implemented by consumers.
/// </summary>
public interface IDelegateInvocationHelperParameterBuilder
{
    public int ParametersCount { get; }

    public bool AcceptsParameterType(int i, Type type);

    public T GetParameter<T>(int i)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;

    public T? TryGetParameter<T>(int i, out bool can)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
}