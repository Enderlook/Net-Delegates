using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Enderlook.Delegates;

[Generator(LanguageNames.CSharp)]
internal sealed class DelegatesGenerator : IIncrementalGenerator
{
    public const string ATTRIBUTE_NAMESPACE = "Enderlook.Delegates";

    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
    }
}
