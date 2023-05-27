#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class NotNullAttribute : Attribute { }
#endif