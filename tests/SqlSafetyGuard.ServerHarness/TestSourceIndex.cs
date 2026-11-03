using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record TestSourceReference(string Name, string File, int Line);

/// <summary>Offline declaration index. No analyzer loading, SQL connections, or case execution.</summary>
internal static class TestSourceIndex
{
    internal static IEnumerable<TestSourceReference> Read(string source, string file)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        if (tree.GetDiagnostics().Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
            throw new InvalidDataException("Cannot index malformed C#: " + file);
        foreach (var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (!method.AttributeLists.SelectMany(list => list.Attributes).Any(attribute =>
                attribute.Name.ToString().Split('.').Last() is "Fact" or "FactAttribute" or "Theory" or "TheoryAttribute")) continue;
            var namespaces = method.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(node => node.Name.ToString());
            var types = method.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().Select(node => node.Identifier.ValueText);
            var prefix = string.Join(".", namespaces);
            var name = (prefix.Length == 0 ? "" : prefix + ".") + string.Join("+", types) + "." + method.Identifier.ValueText;
            yield return new(name, file, tree.GetLineSpan(method.Identifier.Span).StartLinePosition.Line + 1);
        }
    }

    internal static void Write(string directory, string output)
    {
        var entries = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(path => !Path.GetRelativePath(directory, path).Split(Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj"))
            .Order(StringComparer.Ordinal)
            .SelectMany(path => Read(File.ReadAllText(path), Path.GetRelativePath(directory, path).Replace('\\', '/'))).ToArray();
        Directory.CreateDirectory(output);
        File.WriteAllText(Path.Combine(output, "test-index.json"), JsonSerializer.Serialize(entries));
    }
}
