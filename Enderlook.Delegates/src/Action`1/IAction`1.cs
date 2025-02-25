namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter.
/// </summary>
/// <typeparam name="T">Type of the parameter.</typeparam>
public interface IAction<in T> : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg">Argument to pass as parameter.</param>
    /// <typeparam name="U">Specialized type of <typeparamref name="T"/>, useful to avoid boxing or improve devirtualization.</typeparam>
    public abstract void Invoke<U>(U arg) where U : T;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T arg1);
        Invoke(arg1);
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => SignatureVoid<T>.Array;

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{T}(T)"/>
    object? IDelegate.DynamicTupleInvoke<U>(U args)
    {
        Helper.GetParameters(args, out T arg1);
        Invoke(arg1);
        return null;
    }
#endif
}
