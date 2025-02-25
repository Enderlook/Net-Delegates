using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Interface used to represent a delegate.
/// </summary>
public interface IDelegate
{
    /// <summary>
    /// Executes the delegate.
    /// </summary>
    /// <param name="args">Argument passed to the delegate.<br/>
    /// If the delegate doesn't require arguments, it may be either <see langword="null"/> or an empty array.</param>
    /// <returns>Return value of the delegate.</returns>
    public abstract object? DynamicInvoke(params object?[]? args);

    /// <summary>
    /// Gets the signature of the delegate.<br/>
    /// The first element is the return type of the delegate.<br/>
    /// Subsequent elements are the parameters of the delegate.
    /// </summary>
    /// <returns>A description of the signature nof the delegate</returns>
    public abstract Memory<Type> GetSignature();

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Executes the delegate.
    /// </summary>
    /// <param name="args">Argument passed to the delegate. Or <see langword="null"/>, if the delegate doesn't require arguments.</param>
    /// <returns>Return value of the delegate.</returns>
    public virtual object? DynamicTupleInvoke<TTuple>(TTuple args)
        where TTuple : ITuple
    {
        if (args is null)
            Helper.ThrowArgumentNullException_Args();
        object?[] array = new object?[args.Length];
        for (int i = 0; i < array.Length; i++)
            array[i] = args[i];
        return DynamicInvoke(array);
    }
#endif
}