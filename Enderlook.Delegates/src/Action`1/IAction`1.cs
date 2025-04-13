namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter.
/// </summary>
/// <typeparam name="T">Type of the parameter.</typeparam>
public interface IAction<in T> : IDelegate
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// Take into account that if <typeparamref name="T"/> is not a by ref type (<see langword="ref"/> <see langword="struct"/>), then <typeparamref name="U"/> is not guaranted to support one that is.</typeparam>
    public void Invoke(T arg);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
    ReadOnlySpan<Type> IDelegate.GetDynamicSignature() => SignatureVoid<T>.Array;

    /// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
    bool IDelegate.SupportsInvocationHelper<THelper>(in THelper helper)
    {
        return helper.ParametersCount == 1
            && helper.AcceptsParameterType(0, typeof(T))
            && (!helper.AcceptsReturn || helper.AcceptsReturnType(typeof(object)));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
    void IDelegate.DynamicInvoke<THelper>(scoped ref THelper helper)
    {
        Invoke(helper.GetParameter<T>(0));
        helper.SetResult(default(object));
    }
#endif
}
