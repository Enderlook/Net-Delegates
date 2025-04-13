namespace Enderlook.Delegates;

/// <summary>
/// Interface used to represent a delegate.
/// </summary>
public interface IDelegate
{
    /// <summary>
    /// Gets the signature of the delegate.<br/>
    /// The first element is the return type of the delegate.<br/>
    /// Subsequent elements are the parameters of the delegate.<br/>
    /// This is used for <see cref="DynamicInvoke{THelper}(ref THelper)"/>.
    /// </summary>
    /// <returns>A description of the signature of the delegate.</returns>
    public abstract ReadOnlySpan<Type> GetDynamicSignature();

    /// <summary>
    /// Determines if the invocation helper is valid for the specific delegate.<br/>
    /// </summary>
    /// <typeparam name="THelper">Type of invocation helper.</typeparam>
    /// <param name="helper">Invocation helper to check.</param>
    /// <returns><see langword="true"/> if the invocation helper is valid for this delegate. Otherwise <see langword="false"/>.</returns>
    public bool SupportsInvocationHelper<THelper>(in THelper helper)
        where THelper : ISafeDelegateInvocationHelper
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    {
        ReadOnlySpan<Type> signature = GetDynamicSignature();
        if (signature.Length != (helper.ParametersCount + 1))
            return false;
        Type requiredReturnType = signature[0];
        if ((requiredReturnType != typeof(void) || helper.AcceptsReturn) && !helper.AcceptsReturnType(requiredReturnType))
            return false;
        for (int i = 1; i < helper.ParametersCount; i++)
        {
            if (!helper.AcceptsParameterType(i, signature[i]))
                return false;
        }
        return true;
    }
#else
    ;
#endif

    /// <summary>
    /// Check that the invocation helper is valid for the given delegate and executes the delegate.
    /// </summary>
    /// <typeparam name="THelper">Type of invocation helper.</typeparam>
    /// <param name="helper">Instance that contains arguments of execution and stores return value, if any.</param>
    /// <returns><see langword="true"/> if the invocation helper was valid for this delegate. Otherwise, <see langword="false"/>.</returns>
    public bool TryDynamicInvoke<THelper>(scoped ref THelper helper)
        where THelper : ISafeDelegateInvocationHelper
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    {
        if (SupportsInvocationHelper(helper))
        {
            DynamicInvoke(ref helper);
            return true;
        }
        return false;
    }
#else
    ;
#endif

    /// <summary>
    /// Executes a delegate.<br/>
    /// If the invocation helper is not valid for the specific delegate, it's undefined the behaviour.<br/>
    /// You should ensure that your <paramref name="helper"/> support the signature using <see cref="SupportsInvocationHelper{THelper}(in THelper)"/> or alternatively call <see cref="TryDynamicInvoke{THelper}(ref THelper)"/>.
    /// </summary>
    /// <typeparam name="THelper">Type of invocation helper.</typeparam>
    /// <param name="helper">Instance that contains arguments of execution and stores return value, if any.</param>
    /// <exception cref="ArgumentException">Throw when the number of parameters in <paramref name="helper"/> does not have the correct number of arguments, their value can't be assigned to the required types, or the return value can't be assigned to the accepted values..</exception>
    public abstract void DynamicInvoke<THelper>(scoped ref THelper helper)
       where THelper : IDelegateInvocationHelper
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
       ;
}