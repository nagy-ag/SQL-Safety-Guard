using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class StatementVerdictsTests
{
    [Fact]
    public void CommittedStatementVerdictsMatchCurrentAnalyzer()
    {
        var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var path = Path.Combine(repository, "docs/statement-verdicts.md");
        var generated = Generate(Path.Combine(repository, "tests/SqlSafetyGuard.ServerHarness/Coverage"));
        if (Environment.GetEnvironmentVariable("UPDATE_STATEMENT_VERDICTS") == "1")
            File.WriteAllText(path, generated, new UTF8Encoding(false));
        Assert.True(File.Exists(path) && File.ReadAllText(path).Replace("\r\n", "\n") == generated,
            "docs/statement-verdicts.md is stale. Regenerate in PowerShell: " +
            "$env:UPDATE_STATEMENT_VERDICTS='1'; dotnet test tests/SqlSafetyGuard.Tests " +
            "--filter FullyQualifiedName~StatementVerdictsTests; Remove-Item Env:UPDATE_STATEMENT_VERDICTS");
    }

    private static string Generate(string directory)
    {
        var manifest = Read(directory, "manifest.json", "entries");
        Assert.Equal(334, manifest.Length);
        Assert.Equal(334, manifest.Select(Kind).Distinct().Count());
        var astOnly = Read(directory, "ast-only-fixtures.json").ToDictionary(Kind);
        var dialects = Read(directory, "dialect-fixtures.json").ToDictionary(Kind);
        Assert.Equal(7, astOnly.Count);
        Assert.Equal(9, dialects.Count);
        var candidates = new Dictionary<string, List<string>>();
        foreach (var fixture in Read(directory, "parser-fixtures.json"))
        {
            var sql = fixture.GetProperty("sql").GetString()!;
            var fragment = (TSqlScript)new TSql180Parser(true).Parse(new StringReader(sql), out var errors);
            Assert.Empty(errors);
            Assert.Single(fragment.Batches.SelectMany(batch => batch.Statements));
            candidates[Kind(fixture)] = new() { sql };
        }
        foreach (var filename in new[] { "fixtures.json", "expanded-fixtures.json", "parser-fixtures.json",
            "isolated-fixtures.json", "additional-isolated-fixtures.json", "remaining-isolated-fixtures.json" })
        foreach (var fixture in Read(directory, filename))
        {
            var sql = fixture.GetProperty("sql").GetString()!;
            var fragment = new TSql180Parser(true).Parse(new StringReader(sql), out var errors);
            Assert.Empty(errors);
            foreach (var statement in Statements(fragment))
            {
                var kind = statement.GetType().Name;
                if (astOnly.ContainsKey(kind) || dialects.ContainsKey(kind)) continue;
                var example = sql.Substring(statement.StartOffset, statement.FragmentLength).Trim();
                if (!ParsesAs(example, kind))
                {
                    if (kind != "IndexDefinition") continue;
                    example = sql;
                }
                if (!candidates.TryGetValue(kind, out var examples)) candidates[kind] = examples = new();
                examples.Add(example);
            }
        }
        var rows = new List<(int Order, string Label, string Kind, string Example, string Note)>();
        foreach (var entry in manifest)
        {
            var kind = Kind(entry);
            if (astOnly.TryGetValue(kind, out var ast))
            {
                rows.Add((3, Label(kind), kind, "", "No standalone SQL: " + ast.GetProperty("reason").GetString()));
                continue;
            }
            string example;
            var note = "";
            if (dialects.TryGetValue(kind, out var dialect))
            {
                example = dialect.GetProperty("sql").GetString()!;
                TSqlParser parser = dialect.GetProperty("dialect").GetString() switch
                {
                    "80" => new TSql80Parser(true),
                    "FabricDW" => new TSqlFabricDWParser(true),
                    _ => throw new InvalidDataException("Unknown dialect")
                };
                Assert.Contains(kind, CoverageCorpus.StatementTypes(example, parser));
                note = "Alternate-dialect fixture (" + dialect.GetProperty("dialect").GetString() +
                    "); colour is the default analyzer response, including parse errors.";
            }
            else
            {
                Assert.True(Examples.ContainsKey(kind) || candidates.ContainsKey(kind), "Missing standalone fixture: " + kind);
                example = Examples.GetValueOrDefault(kind) ?? candidates[kind]
                    .OrderBy(sql => sql.Contains('#')).ThenBy(sql => sql.Length).ThenBy(sql => sql, StringComparer.Ordinal).First();
                Assert.True(ParsesAs(example, kind), $"Example does not parse as {kind}: {example}");
                note = Context(kind, example);
            }
            var singleLine = Inline(example);
            Assert.DoesNotContain("\n", singleLine);
            Assert.DoesNotContain("\r", singleLine);
            var result = SqlSafetyAnalyzer.Analyze(singleLine, new AnalyzerOptions());
            if (!dialects.ContainsKey(kind)) Assert.Empty(result.ParseErrors);
            rows.Add((Order(result.Overall), Label(kind), kind, singleLine, note));
        }
        var output = new StringBuilder("# Statement verdicts\n\n");
        output.AppendLine("🟢 Green = read-only or temp-only, runs without asking; 🟡 Yellow = can't tell, asks; 🔴 Red = changes permanent data/schema/server (or is conservatively treated as doing so), asks.");
        output.AppendLine("Verdicts use default settings: no allow-list and no environment rules; explicitly configured exceptions are noted.");
        output.AppendLine("One representative example per statement kind, not every clause combination; snippets are analyzed, not executed, without fixture setup.");
        output.AppendLine("See [live-server evidence](verification/2026-10-27-all-kinds/README.md); AST-only kinds have no standalone SQL verdict and appear last.");
        output.AppendLine("\n| Verdict | Statement | Example | When it differs | Kind |");
        output.AppendLine("|---|---|---|---|---|");
        foreach (var row in rows.OrderBy(row => row.Order).ThenBy(row => row.Label, StringComparer.Ordinal)
            .ThenBy(row => row.Kind, StringComparer.Ordinal))
            output.AppendLine($"| {Colour(row.Order)} | {row.Label} | {(row.Example.Length == 0 ? "—" : Code(row.Example))} | {row.Note} | {Code(row.Kind)} |");
        output.AppendLine("\nGenerated by StatementVerdictsTests from the coverage manifest and fixtures. To regenerate in PowerShell:");
        output.AppendLine("\n~~~powershell\n$env:UPDATE_STATEMENT_VERDICTS='1'\ndotnet test tests/SqlSafetyGuard.Tests --filter FullyQualifiedName~StatementVerdictsTests\nRemove-Item Env:UPDATE_STATEMENT_VERDICTS\n~~~");
        return output.ToString().Replace("\r\n", "\n");
    }

    private static JsonElement[] Read(string directory, string filename, string property = "fixtures")
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, filename)));
        return document.RootElement.GetProperty(property).EnumerateArray().Select(item => item.Clone()).ToArray();
    }

    private static string Kind(JsonElement item) => item.GetProperty("statementType").GetString()!;

    private static IEnumerable<TSqlStatement> Statements(TSqlFragment fragment)
    {
        if (fragment is TSqlStatement statement) yield return statement;
        foreach (var property in fragment.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0 || property.Name == "ScriptTokenStream") continue;
            var value = property.GetValue(fragment);
            if (value is TSqlFragment child)
            {
                foreach (var nested in Statements(child)) yield return nested;
            }
            else if (value is IEnumerable children && value is not string)
                foreach (var item in children.OfType<TSqlFragment>())
                    foreach (var nested in Statements(item)) yield return nested;
        }
    }

    private static bool ParsesAs(string sql, string kind)
    {
        var fragment = new TSql180Parser(true).Parse(new StringReader(sql), out var errors);
        return errors.Count == 0 && Statements(fragment).Any(statement => statement.GetType().Name == kind);
    }

    private static string Inline(string sql)
    {
        var tokens = new TSql180Parser(true).GetTokenStream(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var output = new StringBuilder();
        var pendingSpace = false;
        foreach (var token in tokens.Where(token => token.TokenType != TSqlTokenType.EndOfFile))
        {
            if (token.TokenType is TSqlTokenType.WhiteSpace or TSqlTokenType.SingleLineComment or TSqlTokenType.MultilineComment)
            {
                pendingSpace = output.Length > 0;
                continue;
            }
            if (pendingSpace) output.Append(' ');
            output.Append(token.Text);
            pendingSpace = false;
        }
        return output.ToString();
    }

    private static string Code(string sql) => "\u0060" + sql.Replace("|", "&#124;").Replace("\u0060", "&#96;") + "\u0060";
    private static int Order(Verdict verdict) => verdict switch
    {
        Verdict.ReadOnly or Verdict.TempOnly or Verdict.AllowListed => 0,
        Verdict.Unknown => 1,
        Verdict.RealWrite => 2,
        _ => throw new InvalidDataException("Unknown verdict")
    };
    private static string Colour(int order) => new[] { "🟢 Green", "🟡 Yellow", "🔴 Red", "—" }[order];

    private static string Context(string kind, string example)
    {
        var notes = new List<string>();
        if (kind is "BeginTransactionStatement" or "CommitTransactionStatement")
            notes.Add("Unmatched transaction boundary in isolation. " +
                Variant("for a balanced transaction", "BEGIN TRAN; COMMIT;"));
        if (kind is "UpdateStatement" or "DeleteStatement" or "InsertStatement" or "MergeStatement" or "TruncateTableStatement"
            or "DropTableStatement" or "CreateTableStatement")
        {
            notes.Add(Variant("on a #temp table", example.Replace("dbo.Items", "#Items")));
            if (kind is "UpdateStatement" or "DeleteStatement" or "InsertStatement" or "MergeStatement")
                notes.Add(Variant("on a @table variable", example.Replace("dbo.Items", "@Items")));
            if (kind is not "CreateTableStatement" and not "DropTableStatement")
                notes.Add(Variant("with the target allow-listed in settings", example, new AnalyzerOptions(new[] { "dbo.Items" })));
        }
        if (kind == "SelectStatement")
        {
            notes.Add(Variant("for SELECT INTO a permanent table", "SELECT 1 AS Id INTO dbo.Items;"));
            notes.Add(Variant("for SELECT INTO a #temp table", "SELECT 1 AS Id INTO #Items;"));
            notes.Add(Variant("when advancing a sequence", "SELECT NEXT VALUE FOR dbo.Seq;"));
        }
        if (kind == "ExecuteStatement")
        {
            notes.Add(Variant("for a built-in read-only system procedure", "EXEC sys.sp_help;"));
            notes.Add(Variant("when this procedure is trusted in settings", example, new AnalyzerOptions(trustedProcedures: new[] { "dbo.CoverageProc" })));
            notes.Add(Variant("for literal dynamic SQL that reads", "EXEC(N'SELECT 1');"));
            notes.Add(Variant("for literal dynamic SQL that writes", "EXEC(N'DELETE dbo.Items');"));
            notes.Add(Variant("for a known writing system procedure", "EXEC sys.sp_configure N'maxdop', 1;"));
        }
        if (kind == "DbccStatement")
        {
            notes.Add(Variant("for a changing command", "DBCC FREEPROCCACHE;"));
            notes.Add(Variant("with repair", "DBCC CHECKDB (N'Coverage', REPAIR_ALLOW_DATA_LOSS);"));
            notes.Add(Variant("for an unrecognized command", "DBCC SSG_UNKNOWN;"));
        }
        if (kind == "IndexDefinition")
            notes.Add("Inline INDEX inside CREATE TABLE, not standalone SQL; the enclosing table is analyzed. " +
                Variant("inside a #temp table definition", "CREATE TABLE #Items (Id int, INDEX IX_Items (Id));"));
        if (kind is "IfStatement" or "WhileStatement" or "BeginEndBlockStatement" or "BeginEndAtomicBlockStatement" or "TryCatchStatement")
            notes.Add("Contained statements are analyzed too; their effects can change the colour.");
        if (kind is "CreateProcedureStatement" or "AlterProcedureStatement" or "CreateOrAlterProcedureStatement" or "DropProcedureStatement")
            notes.Add(Variant("for a #temp procedure", Regex.Replace(example, @"(?i)(PROCEDURE\s+)(?:dbo\.)?\w+", "$1#TempProc")));
        var fragment = new TSql180Parser(true).Parse(new StringReader(example), out _);
        var targetStatement = Statements(fragment).First(statement => statement.GetType().Name == kind);
        var target = targetStatement switch
        {
            AlterTableStatement alter => alter.SchemaObjectName,
            CreateIndexStatement create => create.OnName,
            CreateColumnStoreIndexStatement create => create.OnName,
            AlterIndexStatement alter => alter.OnName,
            CreateStatisticsStatement create => create.OnName,
            BulkInsertBase bulk => bulk.To,
            _ => null
        };
        if (target != null)
        {
            var temporary = example.Remove(target.StartOffset, target.FragmentLength).Insert(target.StartOffset, "#Items");
            notes.Add(Variant("on a #temp table", temporary));
            if (targetStatement is BulkInsertBase)
                notes.Add(Variant("with the target allow-listed in settings", example,
                    new AnalyzerOptions(new[] { string.Join(".", target.Identifiers.Select(identifier => identifier.Value)) })));
        }
        if (kind == "DropIndexStatement") notes.Add(Variant("on a #temp table", "DROP INDEX IX_Items ON #Items;"));
        if (kind == "DropStatisticsStatement") notes.Add(Variant("on a #temp table", "DROP STATISTICS #Items.Stats;"));
        if (kind == "AlterTableSwitchStatement")
            notes.Add("The analyzer checks the source target here, even with a permanent destination; this is not a safety guarantee.");
        return string.Join(" ", notes);
    }

    private static readonly Dictionary<string, string> Examples = new()
    {
        ["SelectStatement"] = "SELECT 1 AS Value;",
        ["InsertStatement"] = "INSERT dbo.Items VALUES (3,30);",
        ["UpdateStatement"] = "UPDATE dbo.Items SET Qty = 20 WHERE Id = 1;",
        ["DeleteStatement"] = "DELETE FROM dbo.Items WHERE Id = 1;",
        ["MergeStatement"] = "MERGE dbo.Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;",
        ["TruncateTableStatement"] = "TRUNCATE TABLE dbo.Items;",
        ["DropTableStatement"] = "DROP TABLE dbo.Items;",
        ["CreateTableStatement"] = "CREATE TABLE dbo.Items (Id int);",
        ["ExecuteStatement"] = "EXEC dbo.CoverageProc;",
        ["DbccStatement"] = "DBCC CHECKDB (N'Coverage');",
        ["DeclareVariableStatement"] = "DECLARE @value int;",
        ["WaitForStatement"] = "WAITFOR DELAY '00:00:01';",
        ["BulkInsertStatement"] = "BULK INSERT dbo.Items FROM 'C:\\data\\items.csv';",
        ["AlterPartitionFunctionStatement"] = "ALTER PARTITION FUNCTION pfItems() MERGE RANGE (100);",
        ["CreateMasterKeyStatement"] = "CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Example-only-password!';",
        ["CreateFullTextIndexStatement"] = "CREATE FULLTEXT INDEX ON dbo.Items (Description) KEY INDEX PK_Items;",
        ["DropStatisticsStatement"] = "DROP STATISTICS dbo.Items.Stats;"
    };

    private static string Label(string kind)
    {
        if (Labels.TryGetValue(kind, out var label)) return label;
        return Regex.Replace(Regex.Replace(kind, @"Statement(?:80)?$", ""), "(?<=[a-z0-9])(?=[A-Z])", " ")
            .ToUpperInvariant().Replace("FULL TEXT", "FULLTEXT").Replace("STOP LIST", "STOPLIST")
            .Replace("COLUMN STORE", "COLUMNSTORE").Replace("FILE GROUP", "FILEGROUP");
    }

    private static readonly Dictionary<string, string> Labels = new()
    {
        ["BackupTransactionLogStatement"] = "BACKUP LOG",
        ["CopyStatement"] = "COPY INTO",
        ["RestoreStatement"] = "RESTORE DATABASE / LOG",
        ["AlterTableAlterColumnStatement"] = "ALTER TABLE … ALTER COLUMN",
        ["AlterTableAlterIndexStatement"] = "ALTER TABLE … ALTER INDEX",
        ["AlterTableRebuildStatement"] = "ALTER TABLE … REBUILD",
        ["AlterTableSetStatement"] = "ALTER TABLE … SET",
        ["AlterTableSwitchStatement"] = "ALTER TABLE … SWITCH",
        ["AlterTableAlterPartitionStatement"] = "ALTER TABLE … SPLIT / MERGE RANGE",
        ["AlterServerConfigurationSetSoftNumaStatement"] = "ALTER SERVER CONFIGURATION SET SOFTNUMA",
        ["CreateExternalFunctionStatement"] = "CREATE FUNCTION … AS EXTERNAL FUNCTION",
        ["AlterExternalFunctionStatement"] = "ALTER FUNCTION … AS EXTERNAL FUNCTION",
        ["CreateOrAlterExternalFunctionStatement"] = "CREATE OR ALTER FUNCTION … AS EXTERNAL FUNCTION",
        ["OpenCursorStatement"] = "OPEN",
        ["CloseCursorStatement"] = "CLOSE",
        ["FetchCursorStatement"] = "FETCH",
        ["DeallocateCursorStatement"] = "DEALLOCATE",
        ["DbccStatement"] = "DBCC CHECKDB",
        ["ExecuteStatement"] = "EXEC",
        ["BeginEndBlockStatement"] = "BEGIN … END",
        ["BeginEndAtomicBlockStatement"] = "BEGIN ATOMIC … END",
        ["TryCatchStatement"] = "BEGIN TRY … END TRY / BEGIN CATCH … END CATCH",
        ["DeclareTableVariableStatement"] = "DECLARE @table TABLE",
        ["DeclareVariableStatement"] = "DECLARE @variable",
        ["DeclareCursorStatement"] = "DECLARE … CURSOR",
        ["SetVariableStatement"] = "SET @variable",
        ["PredicateSetStatement"] = "SET (session option)",
        ["SetCommandStatement"] = "SET (session value)",
        ["SetRowCountStatement"] = "SET ROWCOUNT",
        ["SetTextSizeStatement"] = "SET TEXTSIZE",
        ["SetErrorLevelStatement"] = "SET ERRLVL",
        ["SetIdentityInsertStatement"] = "SET IDENTITY_INSERT",
        ["SetUserStatement"] = "SETUSER",
        ["RaiseErrorStatement"] = "RAISERROR",
        ["RaiseErrorLegacyStatement"] = "RAISERROR (legacy)",
        ["ReadTextStatement"] = "READTEXT",
        ["WriteTextStatement"] = "WRITETEXT",
        ["UpdateTextStatement"] = "UPDATETEXT",
        ["LineNoStatement"] = "LINENO",
        ["GoToStatement"] = "GOTO",
        ["WaitForStatement"] = "WAITFOR",
        ["LabelStatement"] = "label:",
        ["RenameEntityStatement"] = "RENAME OBJECT",
        ["CreateTypeTableStatement"] = "CREATE TYPE … AS TABLE",
        ["CreateTypeUddtStatement"] = "CREATE TYPE … FROM",
        ["CreateTypeUdtStatement"] = "CREATE TYPE … EXTERNAL NAME",
        ["IndexDefinition"] = "CREATE TABLE … INDEX",
        ["CreateExternalStreamingJobStatement"] = "EXEC sys.sp_create_streaming_job",
        ["EnableDisableTriggerStatement"] = "ENABLE / DISABLE TRIGGER",
        ["AlterTableAddTableElementStatement"] = "ALTER TABLE … ADD",
        ["AlterTableDropTableElementStatement"] = "ALTER TABLE … DROP",
        ["AlterTableConstraintModificationStatement"] = "ALTER TABLE … CHECK / NOCHECK CONSTRAINT",
        ["AlterTableTriggerModificationStatement"] = "ALTER TABLE … ENABLE / DISABLE TRIGGER",
        ["AlterTableChangeTrackingModificationStatement"] = "ALTER TABLE … ENABLE / DISABLE CHANGE_TRACKING",
        ["AlterTableFileTableNamespaceStatement"] = "ALTER TABLE … ENABLE / DISABLE FILETABLE_NAMESPACE",
        ["AlterTableAddClusterByStatement"] = "ALTER TABLE … WITH (CLUSTER BY)",
        ["AlterLoginAddDropCredentialStatement"] = "ALTER LOGIN … ADD / DROP CREDENTIAL",
        ["AlterLoginEnableDisableStatement"] = "ALTER LOGIN … ENABLE / DISABLE",
        ["AlterLoginOptionsStatement"] = "ALTER LOGIN … WITH",
        ["AlterDatabasePerformCutoverStatement"] = "ALTER DATABASE … PERFORM_CUTOVER",
        ["SelectStatementSnippet"] = "SELECT snippet (AST only)",
        ["TSqlStatementSnippet"] = "SQL snippet (AST only)"
    };

    private static string Variant(string description, string sql, AnalyzerOptions? options = null)
    {
        sql = Inline(sql);
        var result = SqlSafetyAnalyzer.Analyze(sql, options ?? new AnalyzerOptions());
        Assert.Empty(result.ParseErrors);
        return $"{Colour(Order(result.Overall))} {description} ({Code(sql)}).";
    }
}
