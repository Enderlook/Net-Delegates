using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace StockManager.SourceGenerators;

public static class IndentedTextWriterExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Append(this IndentedTextWriter source, object input)
    {
        source.Write(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Append(this IndentedTextWriter source, string input)
    {
        source.Write(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Append(this IndentedTextWriter source, char input)
    {
        source.Write(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter AppendLine(this IndentedTextWriter source, object input)
    {
        source.WriteLine(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter AppendLine(this IndentedTextWriter source, string input)
    {
        source.WriteLine(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter AppendLine(this IndentedTextWriter source, char input)
    {
        source.WriteLine(input);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter AppendLine(this IndentedTextWriter source)
    {
        source.WriteLine();
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Brace(this IndentedTextWriter source)
    {
        source.AppendLine('{');
        source.Indent++;
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Unbrace(this IndentedTextWriter source)
    {
        source.Indent--;
        source.AppendLine('}');
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Indent(this IndentedTextWriter source, int depth = 1)
    {
        source.Indent += depth;
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndentedTextWriter Unindent(this IndentedTextWriter source, int depth = 1)
    {
        source.Indent -= depth;
        return source;
    }
}