using System.Runtime.CompilerServices;

namespace Enderlook.Delegates.InvocationHelpers;

/// <summary>
/// Represent a builded delegate invocation helper.
/// </summary>
/// <typeparam name="TArguments">Type that contains the arguments.</typeparam>
public
#if NET9_0_OR_GREATER
    ref
#endif
    struct VoidHelper<TArguments> : ISafeDelegateInvocationHelper
#if NET9_0_OR_GREATER
    where TArguments : IArgumentsBuilder, allows ref struct
#else
    where TArguments: IArgumentsBuilder
#endif
{
    private readonly TArguments arguments;
    private bool hasResult;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal VoidHelper(TArguments arguments) => this.arguments = arguments;

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.ParametersCount"/>
    public readonly int ParametersCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => arguments.ParametersCount;
    }

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsReturn"/>
    public readonly bool AcceptsReturn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsReturnType(Type)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsReturnType(Type type) => false;

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.AcceptsParameterType(int, Type)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AcceptsParameterType(int index, Type type) => arguments.AcceptsParameterType(index, type);

    /// <inheritdoc cref="IDelegateInvocationHelper.GetParameter{T}(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetParameter<T>(int index)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => arguments.GetParameter<T>(ParametersCount - index - 1);

    /// <inheritdoc cref="ISafeDelegateInvocationHelper.TryGetParameter{T}(int, out T?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetParameter<T>(int index, out T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        value = arguments.TryGetParameter<T>(ParametersCount - index - 1, out bool can);
        return can;
    }

    /// <inheritdoc cref="IDelegateInvocationHelper.SetResult{T}(T?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (typeof(T) != typeof(object) && value is not null)
            Helper.ThrowArgumentException_NoReturn();
        hasResult = true;
    }

    /// <summary>
    /// Determines if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed.
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="IDelegateInvocationHelper.SetResult{T}(T?)"/> was executed. Otherwise, <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetResult() => hasResult;
}
