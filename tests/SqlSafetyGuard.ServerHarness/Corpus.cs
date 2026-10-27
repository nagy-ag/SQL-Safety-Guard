using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record Case(string Id, string Source, string Sql);

/// <summary>
/// Collects T-SQL cases: every string literal (and literal + literal concatenation) in the C# test files that looks
/// like T-SQL, every sample .sql file (whole file, and each "-- #n" example in it), extra .sql files, and the
/// generated large scripts.
/// </summary>
internal static class Corpus
{
    private static readonly Regex LooksLikeSql = new(
        @"^\s*(?:(?:--[^\n]*\n|/\*.*?\*/)\s*)*(?:SELECT|INSERT|UPDATE|DELETE|MERGE|EXEC|EXECUTE|CREATE|ALTER|DROP|TRUNCATE|DBCC|BEGIN|COMMIT|ROLLBACK|SAVE|SET|DECLARE|GRANT|REVOKE|DENY|BULK|WITH|IF|WHILE|PRINT|USE|BACKUP|RESTORE|KILL|WAITFOR|RECEIVE|SEND|OPEN|FETCH|CLOSE|DEALLOCATE|THROW|RAISERROR|UPDATETEXT|WRITETEXT|READTEXT|CHECKPOINT|RECONFIGURE|SHUTDOWN|REVERT|RETURN|GOTO|:setvar|:exit|:on|:r|!!|\$\()\b?",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    public static List<Case> FromCSharp(IEnumerable<string> files)
    {
        var cases = new List<Case>();
        foreach (var file in files)
        {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            var root = tree.GetRoot();
            int n = 0;
            foreach (var node in root.DescendantNodes())
            {
                string? value = node switch
                {
                    // Outermost "+" chain made only of literals.
                    BinaryExpressionSyntax b when b.IsKind(SyntaxKind.AddExpression) && b.Parent is not BinaryExpressionSyntax => Concat(b),
                    LiteralExpressionSyntax l when l.IsKind(SyntaxKind.StringLiteralExpression) && l.Parent is not BinaryExpressionSyntax => l.Token.ValueText,
                    _ => null,
                };
                if (value == null || value.Length < 6 || !LooksLikeSql.IsMatch(value))
                    continue;
                var line = tree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
                cases.Add(new Case($"{Path.GetFileNameWithoutExtension(file)}:{line}#{++n}", Path.GetFileName(file), value));
            }
        }

        return cases;
    }

    private static string? Concat(ExpressionSyntax e) => e switch
    {
        BinaryExpressionSyntax b when b.IsKind(SyntaxKind.AddExpression) => Concat(b.Left) is { } l && Concat(b.Right) is { } r ? l + r : null,
        LiteralExpressionSyntax l when l.IsKind(SyntaxKind.StringLiteralExpression) => l.Token.ValueText,
        ParenthesizedExpressionSyntax p => Concat(p.Expression),
        _ => null,
    };

    private static readonly Regex ExampleHeader = new(@"^-- #(\d+)\b", RegexOptions.Multiline);

    public static List<Case> FromSqlFiles(IEnumerable<string> files)
    {
        var cases = new List<Case>();
        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            var name = Path.GetFileName(file);
            cases.Add(new Case($"{name}:whole", name, text));

            var headers = ExampleHeader.Matches(text).Cast<Match>().ToList();
            for (int i = 0; i < headers.Count; i++)
            {
                int start = headers[i].Index;
                int end = i + 1 < headers.Count ? headers[i + 1].Index : text.Length;
                cases.Add(new Case($"{name}:#{headers[i].Groups[1].Value}", name, text.Substring(start, end - start)));
            }
        }

        return cases;
    }

    public static List<Case> Dedupe(IEnumerable<Case> cases)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        return cases.Where(c => seen.Add(c.Sql.Replace("\r\n", "\n").Trim())).ToList();
    }
}
