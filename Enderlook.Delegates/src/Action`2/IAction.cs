namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter.
/// </summary>
/// <typeparam name="T1">Type of the parameter.</typeparam>
/// <typeparam name="T2">Type of the parameter.</typeparam>
public interface IAction<in T1, in T2> : IDelegate
#if NET9_0_OR_GREATER
    where T1 : allows ref struct
    where T2 : allows ref struct
#endif
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg1">Argument to pass as parameter.</param>
    /// <param name="arg2">Argument to pass as parameter.</param>
    public abstract void Invoke(T1 arg1, T2 arg2);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
    ReadOnlySpan<Type> IDelegate.GetDynamicSignature() => SignatureVoid<T1, T2>.Array;

    /// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
    bool IDelegate.SupportsInvocationHelper<THelper>(in THelper helper)
    {
        return helper.ParametersCount == 1
            && helper.AcceptsParameterType(0, typeof(T1))
            && helper.AcceptsParameterType(1, typeof(T2))
            && (!helper.AcceptsReturn || helper.AcceptsReturnType(typeof(object)));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
    void IDelegate.DynamicInvoke<THelper>(scoped ref THelper helper)
    {
        Invoke(helper.GetParameter<T1>(0), helper.GetParameter<T2>(1));
        helper.SetResult(default(object));
    }
#endif
}
