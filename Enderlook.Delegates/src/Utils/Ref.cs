namespace Enderlook.Delegates;

#if NET9_0_OR_GREATER
/// <summary>
/// Types such as <c><typeparamref name="T"/>&amp;</c> can't be used as generic arguments, this type is used to workaround this limitation. 
/// It's useful as a workarround for <see langword="ref"/> and <see langword="out"/>.
/// </summary>
/// <typeparam name="T">Type to get its reference.</typeparam>
public readonly ref struct Ref<T>(ref T value)
{
    /// <summary>
    /// Reference to the value.
    /// </summary>
    public readonly ref T Value = ref value;
}

/// <summary>
/// Types such as <c><typeparamref name="T"/>&amp;</c> can't be used as generic arguments, this type is used to workaround this limitation. 
/// It's useful as a workarround for <see langword="ref readonly"/> and <see langword="in"/>.
/// </summary>
/// <typeparam name="T">Type to get its reference.</typeparam>
public readonly ref struct ReadOnlyRef<T>(ref readonly T value)
{
    /// <summary>
    /// Reference to the value.
    /// </summary>
    public readonly ref readonly T Value = ref value;
}
#endif