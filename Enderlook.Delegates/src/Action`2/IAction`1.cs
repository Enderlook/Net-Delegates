namespace Enderlook.Delegates;

/// <summary>
/// Interface used to declare a callback which accepts a parameter.
/// </summary>
/// <typeparam name="T1">Type of the parameter.</typeparam>
/// <typeparam name="T2">Type of the parameter.</typeparam>
public interface IAction<in T1, in T2> : IDelegate
{
    /// <summary>
    /// Executes this callback.
    /// </summary>
    /// <param name="arg1">Argument to pass as parameter.</param>
    /// <param name="arg2">Argument to pass as parameter.</param>
    /// <typeparam name="U1">Specialized type of <typeparamref name="T1"/>, useful to avoid boxing or improve devirtualization.</typeparam>
    /// <typeparam name="U2">Specialized type of <typeparamref name="T2"/>, useful to avoid boxing or improve devirtualization.</typeparam>
    public abstract void Invoke<U1, U2>(U1 arg1, U2 arg2)
        where U1 : T1
        where U2 : T2;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        Invoke(arg1, arg2);
        return null;
    }

    /// <inheritdoc cref="IDelegate.GetSignature"/>
    Memory<Type> IDelegate.GetSignature() => SignatureVoid<T1, T2>.Array;

    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args, out T1 arg1, out T2 arg2);
        Invoke(arg1, arg2);
        return null;
    }
#endif
}
