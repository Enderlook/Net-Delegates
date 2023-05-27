namespace Enderlook.Delegates;

/// <summary>
/// Interface used to represent a delegate.
/// </summary>
public interface IDelegate
{
    /// <summary>
    /// Executes the delegate.
    /// </summary>
    /// <param name="args">Argument passed to the delegate. Or <see langword="null"/>, if the delegate doesn't require arguments.</param>
    /// <returns>Return value of the delegate.</returns>
    public object? DynamicInvoke(params object?[]? args);
}