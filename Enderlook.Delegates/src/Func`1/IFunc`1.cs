namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback that returns a value.
/// </summary>
/// <typeparam name="TResult">Type of return value.</typeparam>
public interface IFunc<out TResult> : IDelegate
#if NET9_0_OR_GREATER
    where TResult : allows ref struct
#endif
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <returns>Return value of the callback.</returns>
    public abstract TResult Invoke();

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
    ReadOnlySpan<Type> IDelegate.GetDynamicSignature() => Signature<TResult>.Array;

    /// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
    bool IDelegate.SupportsInvocationHelper<THelper>(in THelper helper)
    {
        return helper.ParametersCount == 0
            && helper.AcceptsReturnType(typeof(TResult));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
    void IDelegate.DynamicInvoke<THelper>(scoped ref THelper helper)
    {
        helper.SetResult(Invoke());
    }
#endif
}
