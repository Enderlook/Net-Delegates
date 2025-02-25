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
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        Invoke();
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => Helper.VoidArray;

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        Invoke();
        return null;
    }
#endif
}