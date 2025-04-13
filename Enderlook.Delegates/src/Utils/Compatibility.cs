namespace System.Diagnostics.CodeAnalysis;

#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
    public NotNullIfNotNullAttribute(string parameterName) { }
}

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public NotNullWhenAttribute(bool returnValue) { }
}
#endif

#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class UnscopedRefAttribute : Attribute { }
#endif