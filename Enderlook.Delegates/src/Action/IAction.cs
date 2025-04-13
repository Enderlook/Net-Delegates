namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback.
/// </summary>
public interface IAction : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    public abstract void Invoke();

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
    ReadOnlySpan<Type> IDelegate.GetDynamicSignature() => Helper.VoidArray;

    /// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
    bool IDelegate.SupportsInvocationHelper<THelper>(in THelper helper)
    {
        return helper.ParametersCount == 0
            && (!helper.AcceptsReturn || helper.AcceptsReturnType(typeof(object)));
    }

    /// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
    void IDelegate.DynamicInvoke<THelper>(scoped ref THelper helper)
    {
        Invoke();
        helper.SetResult(default(object));
    }
#endif
}