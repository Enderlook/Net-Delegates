using Enderlook.SourceGenerators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Linq;
using System.Text;

namespace Enderlook.Delegates.SourceGenerators
{
    [Generator]
    public class DelegateGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IndentedSourceBuilder builder = new();
            foreach (Info info in Info.Types)
            {
                builder
                    .Append(
$$"""
using System.Runtime.CompilerServices;

namespace Enderlook.Delegates;
""", AppendMode.Multiline);

                if (info.IsRef)
                    builder.Append("#if NET9_0_OR_GREATER", AppendMode.SingleLine);

                builder
                    .Append($"public unsafe {(info.IsRef ? " ref" : "")} partial struct {info.Name} {(info.Members is null ? "" : $" : IEquatable<{info.Name}>")}")
                    .OpenBrace();

                if (info.Dummy is null)
                {
                    builder.Append(
$$"""
/// <summary>
/// Checks the current instance is <see langword="null"/>.
/// </summary>
/// <returns><see langword="true"/> if the instance is <see langword="null"/>. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public bool IsNull() => callback == null;

""", AppendMode.Multiline);
                }
                else if (info.Dummy.Length > 0)
                {
                    builder.Append(
$$"""
/// <summary>
/// Checks the current instance is <see langword="default"/>.
/// </summary>
/// <returns><see langword="true"/> if the instance is <see langword="default"/>. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public bool IsDefault() => callback == null;

/// <summary>
/// When you create an instance with a zero-parameters constructor, a dummy callback is used to avoid having a <see langword="null"/> one.<br/>
/// Checks the current callback for the dummy one.
/// </summary>
/// <returns><see langword="true"/> if the callback is dummy. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public bool IsEmpty() => callback == {{info.Dummy}};

""", AppendMode.Multiline);
                }

                if (info.Members is not null)
                {
                    builder.Append(
$$"""
/// <summary>
/// Checks the current instance performs the same callback as <paramref name="other"/>.
/// </summary>
/// <param name="other">Instance to check to.</param>
/// <returns><see langword="true"/> if both instances are equal. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public bool Equals({{info.Name}} other)
    => {{string.Join(" && ", info.Members.Select(static e =>
        string.IsNullOrEmpty(e.Type)
        ? $"{e.Name} == other.{e.Name}"
        : $"EqualityComparer<{e.Type}>.Default.Equals({e.Name}, other.{e.Name})"))}};

/// <summary>
/// Checks the current instance performs the same callback as <paramref name="other"/>.
/// </summary>
/// <param name="other">Instance to check to.</param>
/// <returns><see langword="true"/> if both instances are equal. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public override bool Equals(object? obj) => {{(info.IsRef ? "false" : $"obj is {info.Name} obj_ && Equals(obj_)")}};

/// <summary>
/// Generates the hashcode of the current instance.
/// </summary>
/// <returns>Hashcode of the instance.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public override int GetHashCode()
{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    return HashCode.Combine({{string.Join(", ", info.Members.Select(e => e.Name))}});
#else
    return {{string.Join(" * 17 +", info.Members.Select(static e => e.Name + "?.GetHashCode() ?? 0"))}};
#endif
}

/// <summary>
/// Check that two instances performs the same callback.
/// </summary>
/// <param name="left">First instance to check.</param>
/// <param name="right">Second instance to check.</param>
/// <returns><see langword="true"/> if both instances are equal. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool operator ==({{info.Name}} left, {{info.Name}} right) => left.Equals(right);

/// <summary>
/// Check that two instances doesn't performs the same callback.
/// </summary>
/// <param name="left">First instance to check.</param>
/// <param name="right">Second instance to check.</param>
/// <returns><see langword="true"/> if both instances are not equal. Otherwise <see langword="false"/>.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool operator !=({{info.Name}} left, {{info.Name}} right) => !left.Equals(right);

""", AppendMode.Multiline);
                }

                foreach ((bool Explicit, string? Return, string? Parameter, string Expression, bool RequiresRef) in info.Conversions)
                {
                    if (RequiresRef)
                        builder.Append("#if NET9_0_OR_GREATER", AppendMode.SingleLine);

                    builder.Append(
$$"""    
/// <summary>
/// Convert the value to the specified type.
/// </summary>
/// <param name="callback">Value to convert.</param>
/// <returns>Converted value.</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static {{(Explicit ? "explicit" : "implicit")}} operator {{Return ?? info.Name}}({{Parameter ?? info.Name}} value) => {{Expression}};

""", AppendMode.Multiline);

                    if (RequiresRef)
                        builder.Append("#endif", AppendMode.SingleLine);
                }

                if (!string.IsNullOrEmpty(info.GetSignature))
                {
                    builder.Append(
$"""                                            
/// <inheritdoc cref="IDelegate.GetDynamicSignature"/>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public ReadOnlySpan<Type> GetDynamicSignature() => {info.GetSignature};

""", AppendMode.Multiline);
                }

                if (info.Parameters >= 0)
                {
                    builder.Append(
$$"""
/// <inheritdoc cref="IDelegate.SupportsInvocationHelper{THelper}(in THelper)"/>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public bool SupportsInvocationHelper<THelper>(in THelper helper)
#if NET9_0_OR_GREATER
    where THelper : ISafeDelegateInvocationHelper, allows ref struct
#else
    where THelper : ISafeDelegateInvocationHelper
#endif
{
    return !(
        helper.ParametersCount != {{info.Parameters}}
        || {{(info.DoesReturn ? $"!helper.AcceptsReturnType(typeof(TResult))" : $"helper.AcceptsReturn && !helper.AcceptsReturnType(typeof(object))")}}
""", AppendMode.Multiline).IncrementIndent();

                    for (int i = 0; i < info.Parameters; i++)
                        builder.Append($"|| !helper.AcceptsParameterType({i}, typeof({GetT(i)}))", AppendMode.SingleLine);

                    builder.Append(");", AppendMode.SingleLine).CloseBrace();

                    builder.Append(
$$"""

/// <inheritdoc cref="IDelegate.TryDynamicInvoke{THelper}(ref THelper)"/>
public bool TryDynamicInvoke<THelper>(scoped ref THelper helper)
#if NET9_0_OR_GREATER
    where THelper : ISafeDelegateInvocationHelper, allows ref struct
#else
    where THelper : ISafeDelegateInvocationHelper
#endif
{
    if (helper.ParametersCount != {{info.Parameters}}
        || ({{(info.DoesReturn ? $"!helper.AcceptsReturnType(typeof(TResult))" : $"helper.AcceptsReturn && !helper.AcceptsReturnType(typeof(object))")}})
""", AppendMode.Multiline)
                        .IncrementIndent();

                    for (int i = 0; i < info.Parameters; i++)
                        builder.Append($"|| !helper.TryGetParameter({i}, out {GetT(i)} arg{i})", AppendMode.SingleLine);

                    builder.Append(") return false;", AppendMode.SingleLine);

                    if (info.DoesReturn)
                        builder.Append("var return_ = ");
                    builder.Append("Invoke(");
                    for (int i = 0; i < info.Parameters; i++)
                        builder.Append("arg").Append(i).Append(", ");
                    if (info.Parameters > 0)
                        builder.Length -= ", ".Length;
                    builder.Append(");", AppendMode.SingleLine);

                    if (info.DoesReturn)
                        builder.Append("helper.SetResult(return_);", AppendMode.SingleLine);
                    else
                        builder.Append("helper.SetResult(default(object));", AppendMode.SingleLine);

                    builder.Append("return true;", AppendMode.SingleLine).CloseBrace();

                    builder.Append(
"""

/// <inheritdoc cref="IDelegate.DynamicInvoke{THelper}(ref THelper)"/>
public void DynamicInvoke<THelper>(scoped ref THelper helper)
#if NET9_0_OR_GREATER
    where THelper : IDelegateInvocationHelper, allows ref struct
#else
    where THelper : IDelegateInvocationHelper
#endif
""", AppendMode.Multiline).OpenBrace();

                    for (int i = 0; i < info.Parameters; i++)
                        builder.Append($"var arg{i} = helper.GetParameter<{GetT(i)}>({i});", AppendMode.SingleLine);

                    if (info.DoesReturn)
                        builder.Append("var return_ = ");
                    builder.Append("Invoke(");
                    for (int i = 0; i < info.Parameters; i++)
                        builder.Append("arg").Append(i).Append(", ");
                    if (info.Parameters > 0)
                        builder.Length -= ", ".Length;
                    builder.Append(");", AppendMode.SingleLine);

                    if (info.DoesReturn)
                        builder.Append("helper.SetResult(return_);", AppendMode.SingleLine);
                    else
                        builder.Append("helper.SetResult(default(object));", AppendMode.SingleLine);

                    builder.CloseBrace();
                }

                builder.CloseBrace();
                if (info.IsRef)
                    builder.Append("#endif", AppendMode.SingleLine);
                string text = builder.ToString();
                builder.Clear();
                context.RegisterPostInitializationOutput(context =>
                    context.AddSource($"{info.Name.Replace('<', '_').Replace('>', '_')}.g.cs", SourceText.From(text, Encoding.UTF8))
                );

                string GetT(int index) => info.Parameters == 1 ? "T" : $"T{index + 1}";
            }
        }

        public readonly struct Info(
            string name,
            string? dummy,
            (bool Explicit, string? Return, string? Parameter, string Expression, bool RequiresRef)[] conversor,
            (string Name, string? Type)[]? members,
            string getSignature,
            int parameters,
            bool doesReturn,
            bool isRef = false
            )
        {
            public static readonly Info[] Types =
            [
                new("ActionWrapper", "Helper.DelegateAction", [
                    (false, "Action", null, "value.callback", false),
                ], [("callback", null)], "Helper.VoidArray", 0, false),
                new("ActionPointer", "Helper.PointerAction", [
                    (false, "delegate* managed<void>", null, "value.callback", false),
                ], null, "Helper.VoidArray", 0, false),
                new("StatedActionPointer<TState>", null, [
                    ], null, "Helper.VoidArray", 0, false),
                new("StatedActionWrapper<TState>", null, [
                    ], null, "Helper.VoidArray", 0, false),

                new("ActionWrapper<T>", "SignatureVoid<T>.DelegateAction", [
                    (false, "Action<T>", null, "value.callback", false),
                ], [("callback", null)], "SignatureVoid<T>.Array", 1, false),
                new("ActionPointer<T>", "SignatureVoid<T>.PointerAction", [
                    (false, "delegate* managed<T, void>", null, "value.callback", false),
                ], null, "SignatureVoid<T>.Array", 1, false),
                new("StatedActionPointer<TState, T>", null, [
                    ], null, "SignatureVoid<T>.Array", 1, false),
                new("StatedActionWrapper<TState, T>", null, [
                    ], null, "SignatureVoid<T>.Array", 1, false),

                new("ActionWrapper<T1, T2>", "SignatureVoid<T1, T2>.DelegateAction", [
                    (false, "Action<T1, T2>", null, "value.callback", false),
                ], [("callback", null)], "SignatureVoid<T1, T2>.Array", 2, false),
                new("ActionPointer<T1, T2>", "SignatureVoid<T1, T2>.PointerAction", [
                    (false, "delegate* managed<T1, T2, void>", null, "value.callback", false),
                ], null, "SignatureVoid<T1, T2>.Array", 2, false),
                new("StatedActionPointer<TState, T1, T2>", null, [
                    ], null, "SignatureVoid<T1, T2>.Array", 2, false),
                new("StatedActionWrapper<TState, T1, T2>", null, [
                    ], null, "SignatureVoid<T1, T2>.Array", 2, false),

                new("FuncWrapper<TResult>", null, [
                    (false, "Func<TResult>", null, "value.callback", false),
                ], [("callback", null)], "Signature<TResult>.Array", 0, true),
                new("FuncPointer<TResult>", null, [
                    (false, "delegate* managed<TResult>", null, "value.callback", false),
                ], null, "Signature<TResult>.Array", 0, true),
                new("Immediate<TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], [("value", "TResult")], "Signature<TResult>.Array", 0, true),
                new("ImmediateRef<TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], null, "Signature<TResult>.Array", 0, true, isRef: true),
                new("StatedFuncWrapper<TState, TResult>", null, [], null, "Signature<TResult>.Array", 0, true),
                new("StatedFuncPointer<TState, TResult>", null, [], null, "Signature<TResult>.Array", 0, true),

                new("FuncWrapper<T, TResult>", null, [
                    (false, "Func<T, TResult>", null, "value.callback", false),
                ], [("callback", null)], "Signature<T, TResult>.Array", 1, true),
                new("FuncPointer<T, TResult>", null, [
                    (false, "delegate* managed<T, TResult>", null, "value.callback", false),
                ], null, "Signature<T, TResult>.Array", 1, true),
                new("Immediate<T, TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], [("value", "TResult")], "Signature<T, TResult>.Array", 1, true),
                new("ImmediateRef<T, TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], null, "Signature<T, TResult>.Array", 1, true, isRef: true),
                new("StatedFuncWrapper<TState, T, TResult>", null, [], null, "Signature<T, TResult>.Array", 1, true),
                new("StatedFuncPointer<TState, T, TResult>", null, [], null, "Signature<T, TResult>.Array", 1, true),

                new("FuncWrapper<T1, T2, TResult>", null, [
                    (false, "Func<T1, T2, TResult>", null, "value.callback", false),
                ], [("callback", null)], "Signature<T1, T2, TResult>.Array", 2, true),
                new("FuncPointer<T1, T2, TResult>", null, [
                    (false, "delegate* managed<T1, T2, TResult>", null, "value.callback", false),
                ], null, "Signature<T1, T2, TResult>.Array", 2, true),
                new("Immediate<T1, T2, TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], [("value", "TResult")], "Signature<T1, T2, TResult>.Array", 2, true),
                new("ImmediateRef<T1, T2, TResult>", "", [
                    (false, "TResult", null, "value.value", false),
                    (false, null, "TResult", "new(value)", false),
                ], null, "Signature<T1, T2, TResult>.Array", 2, true, isRef: true),
                new("StatedFuncWrapper<TState, T1, T2, TResult>", null, [], null, "Signature<T1, T2, TResult>.Array", 2, true),
                new("StatedFuncPointer<TState, T1, T2, TResult>", null, [], null, "Signature<T1, T2, TResult>.Array", 2, true),
            ];

            public readonly string Name = name;
            public readonly string? Dummy = dummy;
            public readonly (bool Explicit, string? Return, string? Parameter, string Expression, bool RequiresRef)[] Conversions = conversor;
            public readonly (string Name, string? Type)[]? Members = members;
            public readonly string GetSignature = getSignature;
            public readonly int Parameters = parameters;
            public readonly bool DoesReturn = doesReturn;
            public readonly bool IsRef = isRef;
        }
    }
}
