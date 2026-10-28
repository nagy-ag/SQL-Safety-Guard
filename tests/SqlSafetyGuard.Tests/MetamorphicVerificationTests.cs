using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlSafetyGuard.Analyzer;
using Xunit.Abstractions;

namespace SqlSafetyGuard.Tests;

public class MetamorphicVerificationTests(ITestOutputHelper output)
{
    private const int Seed = 20261028;
    private static readonly string[] KnownStreamingInputs = [
        "EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1';",
        "EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1'"];
    private static readonly Regex SqlKeyword = new(@"\b(SELECT|INSERT|UPDATE|DELETE|CREATE|DROP|ALTER|EXEC|SET|BEGIN|COMMIT|ROLLBACK|DBCC|TRUNCATE|PRINT|GRANT|REVOKE|DENY|MERGE|WITH|DECLARE)\b", RegexOptions.IgnoreCase);
    private static readonly Regex BatchOnly = new(@"\bGO\b|^\s*[:!]|\b(CREATE|ALTER)\s+(OR\s+ALTER\s+)?(PROC(?:EDURE)?|FUNCTION|TRIGGER|VIEW)\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private static TSqlParser Parser() => (TSqlParser)Activator.CreateInstance(
        typeof(TSqlParser).Assembly.GetType("Microsoft.SqlServer.TransactSql.ScriptDom." + SqlSafetyAnalyzer.ParserName)!, new object[] { true })!;

    private static IList<TSqlParserToken> Tokens(string sql) => Parser().GetTokenStream(new StringReader(sql), out _);

    private static IEnumerable<(string Source, string Sql)> Corpus()
    {
        string[] seeds = ["SELECT 1;", "SELECT * FROM dbo.Orders;", "CREATE TABLE #a (Qty int); INSERT #a VALUES (1); SELECT * FROM #a;",
            "UPDATE dbo.Orders SET Qty = Qty WHERE 1 = 0;", "UPDATE a SET Qty = Qty FROM dbo.Orders AS a WHERE 1 = 0;",
            "DECLARE @a TABLE (Qty int); INSERT @a VALUES (1);", "EXEC sys.sp_help;", "DBCC CHECKIDENT ('dbo.Orders', NORESEED);",
            "BEGIN TRAN; SELECT 1; COMMIT;", "IF 1=0 UPDATE dbo.Orders SET Qty=0; ELSE SELECT 1;",
            "SELECT NEXT VALUE FOR dbo.Seq;", "EXEC(N'SELECT 1;');", "SET NOCOUNT ON; SELECT 1;",
            "CREATE TABLE ##a (Qty int);", "RETURN;", "THROW 50000, 'x', 1;", "GOTO finish; finish: SELECT 1;"];
        foreach (string sql in seeds)
            yield return ("seed", sql);
        foreach (string path in Directory.GetFiles(Path.Combine(RuleInventoryTests.Root, "tests/SqlSafetyGuard.Tests"), "*.cs").Order())
        {
            if (Path.GetFileName(path) is nameof(MetamorphicVerificationTests) + ".cs")
                continue;
            string text = File.ReadAllText(path);
            foreach (Match match in Regex.Matches(text, "\"\"\"([\\s\\S]*?)\"\"\"|@\"(?:[^\"]|\"\")*\"|\"(?:\\\\.|[^\"\\\\])*\""))
            {
                string literal = match.Value;
                string? sql;
                if (literal.StartsWith("\"\"\"", StringComparison.Ordinal))
                    sql = literal[3..^3].Trim();
                else if (literal.StartsWith("@\"", StringComparison.Ordinal))
                    sql = literal[2..^1].Replace("\"\"", "\"");
                else
                {
                    try { sql = JsonSerializer.Deserialize<string>(literal); }
                    catch (JsonException) { continue; }
                }
                if (sql is { Length: > 5 and < 2500 } && SqlKeyword.IsMatch(sql))
                    yield return (Path.GetFileName(path), sql);
            }
        }
        string docs = File.ReadAllText(Path.Combine(RuleInventoryTests.Root, "docs/statement-verdicts.md"));
        foreach (Match match in Regex.Matches(docs, "`([^`\\r\\n]+)`"))
            if (SqlKeyword.IsMatch(match.Groups[1].Value))
                yield return ("statement-verdicts.md", match.Groups[1].Value);
        foreach (string path in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Coverage"), "*.json").Order())
        {
            using var json = JsonDocument.Parse(File.ReadAllText(path));
            foreach (string sql in FixtureSql(json.RootElement))
                yield return (Path.GetFileName(path), sql);
        }
    }

    private static IEnumerable<string> FixtureSql(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
            foreach (var property in element.EnumerateObject())
            {
                if (property.Name.Equals("sql", StringComparison.OrdinalIgnoreCase) && property.Value.ValueKind == JsonValueKind.String)
                    yield return property.Value.GetString()!;
                else
                    foreach (string sql in FixtureSql(property.Value))
                        yield return sql;
            }
        else if (element.ValueKind == JsonValueKind.Array)
            foreach (var item in element.EnumerateArray())
                foreach (string sql in FixtureSql(item))
                    yield return sql;
    }

    private static string RewriteTokens(string sql, Func<TSqlParserToken, string> rewrite) =>
        string.Concat(Tokens(sql).Where(token => token.TokenType != TSqlTokenType.EndOfFile).Select(rewrite));

    private static bool Identifier(TSqlParserToken token) => token.TokenType is TSqlTokenType.Identifier or TSqlTokenType.QuotedIdentifier;

    private static IEnumerable<(string Name, string Sql)> Rewrites(string sql)
    {
        var fragment = Parser().Parse(new StringReader(sql), out var errors);
        if (errors.Count > 0 || fragment is not TSqlScript script)
            yield break;
        var identifiers = new IdentifierCollector();
        fragment.Accept(identifiers);
        bool IsIdentifier(TSqlParserToken token) => Identifier(token) && identifiers.Offsets.Contains(token.Offset);
        yield return ("identifier-case", RewriteTokens(sql, token => IsIdentifier(token) ? token.Text.ToUpperInvariant() : token.Text));
        yield return ("brackets", RewriteTokens(sql, token =>
        {
            if (!IsIdentifier(token)) return token.Text;
            if (token.TokenType == TSqlTokenType.Identifier)
                return "[" + token.Text.Replace("]", "]]") + "]";
            if (token.TokenType != TSqlTokenType.QuotedIdentifier || !token.Text.StartsWith('['))
                return token.Text;
            string name = token.Text[1..^1].Replace("]]", "]");
            var plain = Tokens(name).Where(candidate => candidate.TokenType != TSqlTokenType.EndOfFile).ToArray();
            return plain.Length == 1 && plain[0].TokenType == TSqlTokenType.Identifier ? name : token.Text;
        }));
        yield return ("whitespace-comments", RewriteTokens(sql, token => token.TokenType == TSqlTokenType.WhiteSpace
            ? token.Text.Contains('\n') || token.Text.Contains('\r') ? "\n /* UPDATE dbo.Decoy SET Qty=0; GO DROP TABLE #decoy */ " : " /* EXEC('DELETE dbo.Decoy'); */ "
            : token.Text));
        if (!Regex.IsMatch(sql, @"\bEXEC(?:UTE)?\b", RegexOptions.IgnoreCase))
            yield return ("temp-rename", RewriteTokens(sql, token =>
        {
            if (!IsIdentifier(token)) return token.Text;
            bool bracket = token.Text.StartsWith('[');
            string name = bracket ? token.Text[1..^1].Replace("]]", "]") : token.Text;
            if (!name.StartsWith('#') || name.StartsWith("##", StringComparison.Ordinal)) return token.Text;
            string renamed = "#zz_" + name[1..];
            return bracket ? "[" + renamed.Replace("]", "]]") + "]" : renamed;
        }));
        var aliases = new AliasCollector();
        fragment.Accept(aliases);
        if (aliases.Names.Count > 0 && !aliases.Names.Overlaps(identifiers.NonAliasNames))
            yield return ("alias-rename", RewriteTokens(sql, token => IsIdentifier(token) && aliases.Names.Contains(token.Text)
                ? "zz_alias_" + token.Text : token.Text));
        bool ordinary = !BatchOnly.IsMatch(sql);
        if (ordinary)
        {
            yield return ("begin-end", "BEGIN\n" + sql + "\nEND;");
            if (!Regex.IsMatch(sql, @"\bEXEC(?:UTE)?\b", RegexOptions.IgnoreCase))
                yield return ("literal-exec", "EXEC(N'" + sql.Replace("'", "''") + "');");
        }
        var statements = script.Batches.SelectMany(batch => batch.Statements).ToArray();
        if (ordinary && statements.Length == 1 && !Regex.IsMatch(sql, @"\b(MERGE|THROW|WITH)\b", RegexOptions.IgnoreCase))
        {
            yield return ("optional-semicolons", sql.TrimEnd().EndsWith(';') ? sql.TrimEnd().TrimEnd(';') : sql + "\n;");
        }
        if (ordinary && statements.Length > 1 && !Regex.IsMatch(sql, @"[@#]|\b(DECLARE|GOTO|RETURN|THROW|IF|WHILE|TRY|CATCH|SET|USE|TRAN|TRANSACTION)\b", RegexOptions.IgnoreCase))
        {
            string split = string.Join("\nGO\n", statements.Select(statement => sql.Substring(statement.StartOffset, statement.FragmentLength)));
            yield return ("go-split", split);
        }
    }

    private sealed class AliasCollector : TSqlFragmentVisitor
    {
        public HashSet<string> Names { get; } = new(StringComparer.OrdinalIgnoreCase);
        public override void Visit(NamedTableReference node)
        {
            if (node.Alias is { QuoteType: QuoteType.NotQuoted } alias)
                Names.Add(alias.Value);
        }
    }

    private sealed class IdentifierCollector : TSqlFragmentVisitor
    {
        public HashSet<int> Offsets { get; } = [];
        public HashSet<string> NonAliasNames { get; } = new(StringComparer.OrdinalIgnoreCase);
        public override void Visit(Identifier node) => Offsets.Add(node.StartOffset);
        public override void Visit(SchemaObjectName node)
        {
            foreach (var identifier in node.Identifiers)
                NonAliasNames.Add(identifier.Value);
        }
        public override void Visit(ColumnReferenceExpression node)
        {
            if (node.MultiPartIdentifier?.Identifiers.LastOrDefault() is { } identifier)
                NonAliasNames.Add(identifier.Value);
        }
    }

    [Fact]
    public void SeededVerdictPreservingRewritesAndWriteMonotonicity()
    {
        int count = int.TryParse(Environment.GetEnvironmentVariable("SSG_REWRITE_COUNT"), CultureInfo.InvariantCulture, out int parsed)
            ? Math.Clamp(parsed, 1, 20000) : 120;
        var random = new Random(Seed);
        var corpus = Corpus().Where(item => item.Sql.Length < 4000).DistinctBy(item => item.Sql).ToArray();
        var selected = corpus.Take(17).Concat(corpus.Skip(17).OrderBy(_ => random.Next()).Take(count)).ToArray();
        int attempts = 0, checkedRewrites = 0, invalid = 0, baselineInvalid = 0, monotonic = 0, known = 0;
        var failures = new List<string>();
        var origins = new Dictionary<string, int>();
        foreach (var item in selected)
        {
            var baseline = SqlSafetyAnalyzer.Analyze(item.Sql);
            if (baseline.ParseErrors.Count > 0) { baselineInvalid++; continue; }
            origins[item.Source] = origins.GetValueOrDefault(item.Source) + 1;
            foreach (var rewrite in Rewrites(item.Sql).Where(rewrite => rewrite.Sql != item.Sql))
            {
                attempts++;
                var result = SqlSafetyAnalyzer.Analyze(rewrite.Sql);
                if (result.ParseErrors.Count > 0) { invalid++; continue; }
                checkedRewrites++;
                if (baseline.Overall != result.Overall)
                {
                    if (rewrite.Name == "begin-end" && KnownStreamingInputs.Contains(item.Sql) &&
                        baseline.Overall == Verdict.RealWrite && result.Overall == Verdict.Unknown)
                    {
                        known++;
                        output.WriteLine("KNOWN W1-001: " + item.Sql);
                    }
                    else
                        failures.Add($"{rewrite.Name}: {baseline.Overall} -> {result.Overall}\nSQL: {item.Sql}\nREWRITE: {rewrite.Sql}");
                }
            }
            if (baseline.IsGreen && !BatchOnly.IsMatch(item.Sql))
            {
                string appended = item.Sql + "\n; UPDATE dbo.Orders SET Qty = Qty WHERE 1 = 0;";
                var result = SqlSafetyAnalyzer.Analyze(appended);
                if (result.ParseErrors.Count == 0)
                {
                    monotonic++;
                    if (result.IsGreen)
                        failures.Add("monotonic: green -> green\nSQL: " + appended);
                }
            }
        }
        output.WriteLine($"Seed {Seed}; available {corpus.Length}; selected {selected.Length}; invalid baselines {baselineInvalid}; attempted {attempts}; valid rewrites {checkedRewrites}; inapplicable parse {invalid}; monotonic {monotonic}; known discrepancies {known}.");
        foreach (var origin in origins.OrderBy(pair => pair.Key))
            output.WriteLine($"Corpus {origin.Key}: {origin.Value}");
        foreach (string failure in failures)
            output.WriteLine(failure);
        Assert.True(checkedRewrites >= 30, "No meaningful rewrite corpus was exercised.");
        Assert.True(monotonic > 0);
        Assert.True(failures.Count == 0, string.Join("\n\n", failures));
    }

    [Theory]
    [InlineData("EXEC sys.sp_create_streaming_job @name=N'x', @statement=N'';")]
    [InlineData("EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1';")]
    public void KnownWaveOneStreamingWrapperDiscrepancy(string sql)
    {
        var baseline = SqlSafetyAnalyzer.Analyze(sql);
        var wrapped = SqlSafetyAnalyzer.Analyze("BEGIN\n" + sql + "\nEND;");
        Assert.Empty(baseline.ParseErrors);
        Assert.Empty(wrapped.ParseErrors);
        output.WriteLine($"W1-001: {baseline.Overall} -> {wrapped.Overall}; {sql}");
        Assert.Equal(Verdict.RealWrite, baseline.Overall);
        Assert.Equal(Verdict.Unknown, wrapped.Overall);
    }

    [Theory]
    [InlineData("EXECUTE AS USER = 'u';", Verdict.ReadOnly)]
    [InlineData("SET IDENTITY_INSERT dbo.Items ON;", Verdict.ReadOnly)]
    [InlineData("ROLLBACK;", Verdict.ReadOnly)]
    [InlineData("WRITETEXT #t.Body @ptr 'x';", Verdict.RealWrite)]
    [InlineData("UPDATE STATISTICS dbo.Items;", Verdict.Unknown)]
    [InlineData("RAISERROR ('x', 10, 1) WITH LOG;", Verdict.ReadOnly)]
    [InlineData("SET ANSI_DEFAULTS ON; SELECT * FROM dbo.Items;", Verdict.ReadOnly)]
    [InlineData("CREATE TABLE ##a (Qty int);", Verdict.TempOnly)]
    public void PolicyQuestionsRecordCurrentOfflineVerdict(string sql, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(expected, result.Overall);
    }
}
