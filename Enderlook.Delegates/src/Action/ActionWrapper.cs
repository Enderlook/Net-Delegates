using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;

/// <summary>
/// Wraps a <see cref="Action"/> in order to implement <see cref="IAction"/>.
/// </summary>
/// <remarks>This type must always be constructed, calling any method on a <see langword="default"/> instance is undefined behaviour.</remarks>
public readonly struct ActionWrapper : IAction, IEquatable<ActionWrapper>
#if NET7_0_OR_GREATER
    , IAdditionOperators<ActionWrapper, ActionWrapper, ActionWrapper>
    , ISubtractionOperators<ActionWrapper, ActionWrapper, ActionWrapper>
    , IEqualityOperators<ActionWrapper, ActionWrapper, bool>
#endif
{
    private static class Container
    {
        public static readonly Action Shared = new(() => { });
    }

    internal readonly Action callback;

    /// <summary>
    /// If <see langword="true"/>, this instance is <see langword="default"/> and so its methods can't be used.
    /// </summary>
    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => callback is null;
    }

    /// <summary>
    /// Wraps a dummy callback which does nothing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper() => callback = Container.Shared;

    /// <summary>
    /// Wraps an <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ActionWrapper([NotNull] Action callback)
    {
        if (callback is null) Helper.ThrowArgumentNullException_Callback();
        this.callback = callback;
    }

    /// <inheritdoc cref="IAction.Invoke"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke() => callback();

    /// <inheritdoc cref="IDelegate.DynamicInvoke(object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicInvoke(params object?[]? args)
    {
        Helper.GetParameters(args);
        callback();
        return null;
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc cref="IDelegate.DynamicTupleInvoke{TTuple}(TTuple)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? IDelegate.DynamicTupleInvoke<TTuple>(TTuple args)
    {
        Helper.GetParameters(args);
        callback();
        return null;
    }
#endif

    /// <summary>
    /// Get the hashcode of this instance.
    /// </summary>
    /// <returns>Hashcode of the instance.</returns>
#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => callback is null ? 0 : callback.GetHashCode();
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(callback);
#endif

    /// <summary>
    /// Determines whether the specified callback is equal to this instance.
    /// </summary>
    /// <param name="other">Callback to compare.</param>
    /// <returns><see langword="true"/> if <see langword="this"/> is equal to <paramref name="other"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? other) => other is ActionWrapper wrapper && wrapper.callback == callback;

    /// <summary>
    /// Determines whether the specified callback is equal to this instance.
    /// </summary>
    /// <param name="other">Callback to compare.</param>
    /// <returns><see langword="true"/> if <see langword="this"/> is equal to <paramref name="other"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ActionWrapper other) => callback == other.callback;

    /// <summary>
    /// Extract the wrapped callback.
    /// </summary>
    /// <param name="callback">Wrapper to open.</param>
    /// <returns>Wrapped callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Action(ActionWrapper callback) => callback.callback;

    /// <summary>
    /// Wrap an callback.
    /// </summary>
    /// <param name="callback">Callback to wrap.</param>
    /// <returns>Wrapper of callback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ActionWrapper([NotNull] Action callback) => new(callback);

    /// <summary>
    /// Determines whether the specified callbacks are equal.
    /// </summary>
    /// <param name="a">First callback to compare.</param>
    /// <param name="b">Second callback to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is equal to <paramref name="b"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ActionWrapper a, ActionWrapper b) => a.callback == b.callback;

    /// <summary>
    /// Determines whether the specified callbacks are not equal.
    /// </summary>
    /// <param name="a">First callback to compare.</param>
    /// <param name="b">Second callback to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> isn't equal to <paramref name="b"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ActionWrapper a, ActionWrapper b) => a.callback != b.callback;

    /// <summary>
    /// Determines whether the specified callbacks are equal.
    /// </summary>
    /// <param name="a">First callback to compare.</param>
    /// <param name="b">Second callback to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is equal to <paramref name="b"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ActionWrapper a, Action b) => a.callback == b;

    /// <summary>
    /// Determines whether the specified callbacks are not equal.
    /// </summary>
    /// <param name="a">First callback to compare.</param>
    /// <param name="b">Second callback to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> isn't equal to <paramref name="b"/>. Otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ActionWrapper a, Action b) => a.callback != b;

    /// <summary>
    /// Merge two callbacks.
    /// </summary>
    /// <param name="a">First callback to merge.</param>
    /// <param name="b">Second callback to merge.</param>
    /// <returns>Merged callbacks.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ActionWrapper operator +(ActionWrapper a, ActionWrapper b) => new(a.callback + b.callback);

    /// <summary>
    /// Removes the last occurrence of the invocation list of a callback from the invocation list of another callback.
    /// </summary>
    /// <param name="source">The callback from which to remove the invocation list of <paramref name="value"/>.</param>
    /// <param name="value">The callback that supplies the invocation list to remove from the invocation list of <paramref name="source"/>.</param>
    /// <returns>A new callback with an invocation list formed by taking the invocation list of <paramref name="source"/> and removing the last ocurrence of the invocation list of <paramref name="value"/>, if the invocation list of <paramref name="value"/> is fround within the invocation list of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ActionWrapper operator -(ActionWrapper source, ActionWrapper value) => new((source.callback - value.callback) ?? Container.Shared);
}
