namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter and returns a value.
/// </summary>
/// <typeparam name="T">Type of the parameter.</typeparam>
/// <typeparam name="TResult">Type of return value.</typeparam>
public interface IFunc<in T, out TResult> : IDelegate
#if NET9_0_OR_GREATER
    where T : allows ref struct
    where TResult : allows ref struct
#endif
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <returns>Return value of the callback.</returns>
    public abstract TResult Invoke(T arg);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
    ReadOnlySpan<Type> IDelegate.GetDynamicSignature() => Signature<TResult>.Array;

    /// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
    bool IDelegate.SupportsInvocationHelper<THelper>(in THelper helper)
    {
        return helper.ParametersCount == 1
            && helper.AcceptsParameterType(0, typeof(T))
            && helper.AcceptsReturnType(typeof(TResult));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
    void IDelegate.DynamicInvoke<THelper>(scoped ref THelper helper)
    {
        helper.SetResult(Invoke(helper.GetParameter<T>(0)));
    }
#endif
}