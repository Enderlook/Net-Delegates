﻿#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute { }
#endif