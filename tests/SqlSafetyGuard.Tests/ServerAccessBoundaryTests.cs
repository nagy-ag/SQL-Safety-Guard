using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlSafetyGuard.Tests;

/// <summary>Regression guardrails for production's text-only boundary, not a hostile-code sandbox.</summary>
public class ServerAccessBoundaryTests
{
    [Fact]
    public void ProductionProjectsCannotAcquireSqlOrNetworkClientDependencies()
    {
        var allowed = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.SqlServer.TransactSql.ScriptDom", "Microsoft.VisualStudio.SDK",
            "Microsoft.VSSDK.BuildTools", "Newtonsoft.Json",
        };
        foreach (var project in Directory.EnumerateFiles(Path.Combine(TestRepository.Root, "src"), "*.csproj", SearchOption.AllDirectories))
        {
            var document = XDocument.Load(project);
            foreach (var package in document.Descendants().Where(element => element.Name.LocalName == "PackageReference"))
                Assert.Contains((string)package.Attribute("Include")!, allowed);
            foreach (var reference in document.Descendants().Where(element => element.Name.LocalName == "ProjectReference"))
                Assert.EndsWith("SqlSafetyGuard.Analyzer.csproj", (string)reference.Attribute("Include")!);
        }
        Assert.DoesNotContain(typeof(SqlSafetyGuard.Analyzer.SqlSafetyAnalyzer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name!.Contains("SqlClient", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductionSourceHasNoSqlExecutionOrNetworkClientApis()
    {
        var prohibited = new HashSet<string>(StringComparer.Ordinal)
        {
            "SqlConnection", "SqlCommand", "SqlConnectionStringBuilder", "SqlClient", "DbConnection", "DbCommand",
            "IDbCommand", "DbProviderFactories", "ServerConnection", "Smo", "SMO", "TcpClient", "Socket", "HttpClient", "WebRequest",
        };
        var commandMethods = new HashSet<string>(StringComparer.Ordinal)
        {
            "CreateCommand", "ExecuteReader", "ExecuteReaderAsync", "ExecuteNonQuery", "ExecuteNonQueryAsync",
            "ExecuteScalar", "ExecuteScalarAsync", "Open", "OpenAsync", "ChangeDatabase", "BeginTransaction",
        };
        foreach (var file in Directory.EnumerateFiles(Path.Combine(TestRepository.Root, "src"), "*.cs", SearchOption.AllDirectories)
                     .Where(path => !Path.GetRelativePath(TestRepository.Root, path).Split(Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj")))
        {
            // Parse both configurations so disabled Debug hooks are checked too. Comments and SQL string literals are excluded.
            foreach (var symbols in new[] { Array.Empty<string>(), new[] { "DEBUG" } })
            {
                var syntax = CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(preprocessorSymbols: symbols)).GetRoot();
                foreach (var token in syntax.DescendantTokens())
                    Assert.False(prohibited.Contains(token.ValueText) && token.RawKind == (int)SyntaxKind.IdentifierToken, $"Server/client API {token.ValueText}: {file}");
                foreach (var invocation in syntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var name = invocation.Expression switch
                    {
                        MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                        _ => "",
                    };
                    Assert.False(commandMethods.Contains(name), $"SQL/connection method {name}: {file}");
                }
            }
        }
    }
}
