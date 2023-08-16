using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace Enderlook.Delegates;

[Generator(LanguageNames.CSharp)]
internal sealed class InterfaceDelegatesGenerator : IIncrementalGenerator
{
    public const string ATTRIBUTE_NAME = "GenerateInterfaceDelegateAttribute";
    private const string FULL_ATTRIBUTE_NAME = $"{DelegatesGenerator.ATTRIBUTE_NAMESPACE}.{ATTRIBUTE_NAME}";

    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation.
        context.RegisterPostInitializationOutput(AddAttribute);

        // Filter interfaces which has our marker attribute.
        IncrementalValuesProvider<(InterfaceDeclarationSyntax? DeclarationSyntax, AttributeData? AttributeData)> interfaceDeclarations = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(FULL_ATTRIBUTE_NAME, IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration)
            .Where(static e => e.DeclarationSyntax is not null);

        // Combine the selected interfaces with the compilation.
        IncrementalValueProvider<(Compilation, ImmutableArray<(InterfaceDeclarationSyntax? DeclarationSyntax, AttributeData? AttributeData)>)> compilationAndInterfaces = context
            .CompilationProvider
            .Combine(interfaceDeclarations.Collect());

        // Generate the source code using the compilation and fields.
        context.RegisterSourceOutput(compilationAndInterfaces, static (context, source) => Execute(context, source.Item1, source.Item2));
    }

    private static void AddAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource($"{ATTRIBUTE_NAME}.g.cs",
            SourceText.From(
@$"
#nullable enable

namespace {DelegatesGenerator.ATTRIBUTE_NAMESPACE};

[global::System.Diagnostics.ConditionalAttribute(""__DebugAssert"")]
[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Interface)]
public sealed class {ATTRIBUTE_NAME} : global::System.Attribute
{{
    public {ATTRIBUTE_NAME}(global::System.Type signature) {{ }}
}}",
                Encoding.UTF8
            )
        );
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
        => node is InterfaceDeclarationSyntax;

    private static (InterfaceDeclarationSyntax? DeclarationSyntax, AttributeData? AttributeSyntax) GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        ImmutableArray<AttributeData> attributes = context.Attributes;
        if (attributes.Length == 1)
        {
            // The cast is valid because we already checked that in `IsSyntaxTargetForGeneration`.
            return ((InterfaceDeclarationSyntax)context.TargetNode, attributes[0]);
        }

        // There is more than 1 attribute or is not in the interface.
        return default;
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<(InterfaceDeclarationSyntax? DeclarationSyntax, AttributeData? AttributeData)> interfaces)
    {
        if (interfaces.IsDefaultOrEmpty)
        {
            // No content to work with.
            return;
        }

        INamedTypeSymbol? generatorAttribute = compilation.GetTypeByMetadataName(FULL_ATTRIBUTE_NAME);
        if (generatorAttribute is null)
        {
            // For some reason compilation couldn't find the attributes.
            return;
        }

        CancellationToken cancellationToken = context.CancellationToken;

        StringBuilder nameBuilder = new();
        StringWriter sourceBuilderWriter = new();
        IndentedTextWriter sourceBuilder = new(sourceBuilderWriter);

        foreach ((InterfaceDeclarationSyntax? DeclarationSyntax, AttributeData? AttributeData) @interface in interfaces)
        {

        }
    }
}
