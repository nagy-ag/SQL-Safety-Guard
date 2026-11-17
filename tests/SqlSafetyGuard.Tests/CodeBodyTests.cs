using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class CodeBodyTests
{
    [Theory]
    [InlineData("INSERT OPENQUERY(srv, 'SELECT Id FROM dbo.T') VALUES(1);")]
    [InlineData("UPDATE OPENQUERY(srv, 'SELECT Id FROM dbo.T') SET Id=2;")]
    [InlineData("DELETE r FROM OPENQUERY(srv, 'SELECT Id FROM dbo.T') r;")]
    public void RemoteWriteTargetsAlsoExposeTheQueryWithoutDuplicateWarnings(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, Assert.Single(result.Findings).Verdict);
        var body = Assert.Single(result.CodeBodies);
        Assert.Same(result.Findings[0], body.Definition);
        Assert.StartsWith(result.Findings[0].Action, body.OperationLabel);
        Assert.Equal("SELECT", Assert.Single(body.Analysis.Findings).Action);
    }

    [Theory]
    [InlineData("CREATE")]
    [InlineData("ALTER")]
    [InlineData("CREATE OR ALTER")]
    public void AllModuleVerbsPreservePreviewPositionsAcrossBatches(string verb)
    {
        foreach (var definition in new[] {
            "TRIGGER dbo.tr ON dbo.T AFTER DELETE AS DELETE dbo.Audit;",
            "FUNCTION dbo.f() RETURNS int AS BEGIN RETURN dbo.other(); END;",
            "FUNCTION dbo.tf() RETURNS TABLE AS RETURN SELECT 1 AS Id;",
            "VIEW dbo.v AS SELECT 1 AS Id;" })
        {
            var sql = "SELECT 0;\nGO\n" + verb + " " + definition;
            var result = SqlSafetyAnalyzer.Analyze(sql);
            Assert.Empty(result.ParseErrors);
            var body = Assert.Single(result.CodeBodies);
            Assert.True(body.IsComplete);
            Assert.Equal(3, body.Definition.Line);
            Assert.All(body.Analysis.Findings, f => Assert.InRange(f.StartOffset, sql.IndexOf(verb, StringComparison.Ordinal), sql.Length - 1));
        }
    }

    [Theory]
    [InlineData("SELECT dbo.f();")]
    [InlineData("DECLARE @x int=dbo.f();")]
    [InlineData("SET @x=dbo.f();")]
    [InlineData("IF dbo.f()=1 SELECT 1;")]
    [InlineData("WHILE dbo.f()=1 BREAK;")]
    [InlineData("INSERT @t VALUES(dbo.f());")]
    [InlineData("UPDATE #t SET X=dbo.f();")]
    [InlineData("DELETE #t WHERE dbo.f()=1;")]
    [InlineData("SELECT * FROM [db].[dbo].[tf]();")]
    [InlineData("SELECT @v.Method();")]
    public void FunctionWarningsSurviveWrappersAndTemporaryTargets(string statement)
    {
        foreach (var wrapper in new[] { statement, "BEGIN " + statement + " END", "BEGIN TRY " + statement + " END TRY BEGIN CATCH PRINT 'error'; END CATCH",
            "BEGIN TRAN; " + statement + " ROLLBACK;", "SELECT 1;\nGO\n" + statement,
            "EXEC(N'" + statement.Replace("'", "''") + "');", "EXEC sp_executesql N'" + statement.Replace("'", "''") + "';" })
        {
            var result = SqlSafetyAnalyzer.Analyze(wrapper);
            Assert.Empty(result.ParseErrors);
            Assert.Contains(result.Findings, f => f.Action == "FUNCTION" && f.Verdict == Verdict.Unknown);
            Assert.False(result.IsGreen);
        }
    }

    [Fact]
    public void NestedRemoteAndStoredPreviewsRetainOriginalStringCoordinates()
    {
        const string sql = "SELECT 0;\nGO\nCREATE PROC dbo.p AS EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @command=N'SELECT * FROM OPENQUERY(srv, ''DELETE dbo.T;'');';";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        var module = Assert.Single(result.CodeBodies);
        var job = Assert.Single(module.Analysis.CodeBodies);
        var remote = Assert.Single(job.Analysis.CodeBodies);
        Assert.Equal(CodeBodyKind.JobCommand, job.Kind);
        Assert.Equal(CodeBodyKind.RemoteQuery, remote.Kind);
        var deletion = Assert.Single(remote.Analysis.Findings);
        Assert.Equal(Verdict.RealWrite, deletion.Verdict);
        Assert.Equal("DELETE dbo.T;", string.Concat(deletion.SourceRanges.Select(r => sql.Substring(r.StartOffset, r.Length))));
        Assert.Equal(2, result.Findings.Count);
        Assert.DoesNotContain(result.Findings, f => f.Action == "DELETE");
        Assert.Single(module.Analysis.Findings);
    }

    [Theory]
    [InlineData("EXEC(N'UPDATE tempdb.dbo.T SET X=1; EXEC dbo.trusted;') AT srv;", CodeBodyKind.RemoteQuery)]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @command=N'UPDATE tempdb.dbo.T SET X=1; EXEC dbo.trusted;';", CodeBodyKind.JobCommand)]
    public void RemoteAndStoredContextsCannotInheritLocalTrust(string sql, CodeBodyKind kind)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(allowListPatterns: new[] { "*" }, tempdbIsTemp: true,
            trustedProcedures: new[] { "dbo.trusted" }, currentDatabase: "LocalDb", defaultSchema: "dbo"));
        var body = Assert.Single(result.CodeBodies);
        Assert.Equal(kind, body.Kind);
        Assert.Contains(body.Analysis.Findings, f => f.Action == "UPDATE" && f.Verdict == Verdict.RealWrite);
        Assert.Contains(body.Analysis.Findings, f => f.Action == "EXEC" && f.Verdict == Verdict.Unknown);
        Assert.DoesNotContain(body.Analysis.Findings, f => f.Verdict.IsGreen());
    }

    [Theory]
    [InlineData("EXEC msdb.dbo.sp_update_jobstep @job_name=N'demo', @step_id=1, @command=N'DELETE dbo.T;';")]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @subsystem=@runtime, @command=N'DELETE dbo.T;';")]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @command=N'SELECT ''$(ESCAPE_SQUOTE(A-DBN))''; DELETE dbo.T;';")]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @command=@runtime;")]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep NULL, N'demo', 1, N'cleanup', N'TSQL', N'DELETE dbo.T;';")]
    public void UnprovenJobLanguageTokensAndPositionalArgumentsStayOpaque(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        var body = Assert.Single(result.CodeBodies);
        Assert.Equal(CodeBodyKind.JobCommand, body.Kind);
        Assert.False(body.IsComplete);
        Assert.Equal(Verdict.Unknown, body.Analysis.Overall);
        Assert.DoesNotContain(body.Analysis.Findings, f => f.Action == "DELETE");
    }

    [Theory]
    [InlineData("CREATE TRIGGER dbo.tr ON dbo.T AFTER DELETE AS DELETE FROM;")]
    [InlineData("CREATE FUNCTION dbo.f() RETURNS int AS BEGIN RETURN (; END;")]
    [InlineData("CREATE VIEW dbo.v AS SELECT FROM;")]
    [InlineData("EXEC(N'DELETE FROM;') AT srv;")]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'demo', @command=N'DELETE FROM;';")]
    public void BrokenBodiesCannotLookCompleteOrSafe(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.False(result.IsGreen);
        var body = Assert.Single(result.CodeBodies);
        Assert.False(body.IsComplete);
        Assert.False(body.Analysis.IsGreen);
    }

    [Fact]
    public void PreviewNestingIsBoundedAndFailsClosed()
    {
        string sql = "DELETE dbo.T;";
        for (int i = 0; i < 10; i++) sql = "EXEC(N'" + sql.Replace("'", "''") + "') AT srv;";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        int levels = 0;
        while (result.CodeBodies.Count > 0)
        {
            var body = Assert.Single(result.CodeBodies);
            result = body.Analysis;
            levels++;
        }
        Assert.InRange(levels, 1, 9);
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Note.Contains("nesting limit"));
    }

    [Fact]
    public void NumberedInspectionExamplesCoverTheWholeFeatureSet()
    {
        var sql = File.ReadAllText(Path.Combine(TestRepository.Root, "samples/05-other-code-bodies.sql"));
        var sections = System.Text.RegularExpressions.Regex.Split(sql, @"(?m)^-- #\d+ ").Skip(1).ToArray();
        Assert.Equal(13, sections.Length);
        var results = sections.Select(s => SqlSafetyAnalyzer.Analyze("-- " + s)).ToArray();
        Assert.All(results, r => Assert.Empty(r.ParseErrors));
        Assert.Equal(new[] { CodeBodyKind.Trigger, CodeBodyKind.ScalarFunction, CodeBodyKind.InlineTableFunction, CodeBodyKind.TableFunction, CodeBodyKind.View },
            results.Take(5).Select(r => Assert.Single(r.CodeBodies).Kind));
        Assert.Equal(4, results[5].Findings.Count(f => f.Action == "FUNCTION" && f.Verdict == Verdict.Unknown));
        Assert.All(results.Skip(6).Take(3), r => Assert.Equal(CodeBodyKind.RemoteQuery, Assert.Single(r.CodeBodies).Kind));
        Assert.Equal(CodeBodyKind.JobCommand, Assert.Single(results[9].CodeBodies).Kind);
        Assert.False(Assert.Single(results[10].CodeBodies).IsComplete);
        Assert.Equal(Verdict.Unknown, results[11].Overall);
        Assert.Empty(results[12].CodeBodies);
        Assert.Equal(10, SqlSafetyAnalyzer.Analyze(sql).CodeBodies.Count);
    }

    [Theory]
    [InlineData("CREATE TRIGGER dbo.tr ON dbo.T AFTER DELETE AS INSERT dbo.Audit SELECT Id FROM deleted; DELETE dbo.Archive;", CodeBodyKind.Trigger, Verdict.RealWrite)]
    [InlineData("ALTER TRIGGER ddl ON DATABASE FOR CREATE_TABLE AS EXEC dbo.Other;", CodeBodyKind.Trigger, Verdict.Unknown)]
    [InlineData("CREATE TRIGGER logon ON ALL SERVER FOR LOGON AS SELECT 1;", CodeBodyKind.Trigger, Verdict.ReadOnly)]
    [InlineData("CREATE FUNCTION dbo.f() RETURNS int AS BEGIN RETURN (SELECT COUNT(*) FROM dbo.T); END;", CodeBodyKind.ScalarFunction, Verdict.ReadOnly)]
    [InlineData("CREATE FUNCTION dbo.f() RETURNS TABLE AS RETURN (SELECT Id FROM dbo.T);", CodeBodyKind.InlineTableFunction, Verdict.ReadOnly)]
    [InlineData("CREATE FUNCTION dbo.f() RETURNS @r TABLE(Id int) AS BEGIN INSERT @r VALUES(1); RETURN; END;", CodeBodyKind.TableFunction, Verdict.TempOnly)]
    [InlineData("CREATE VIEW dbo.v AS SELECT * FROM OPENQUERY(srv, 'DELETE dbo.T; SELECT 1');", CodeBodyKind.View, Verdict.Unknown)]
    public void DefinitionsHaveSeparateTypedPreviews(string sql, CodeBodyKind kind, Verdict previewVerdict)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        var definition = Assert.Single(result.Findings);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var body = Assert.Single(result.CodeBodies);
        Assert.Same(definition, body.Definition);
        Assert.Equal(kind, body.Kind);
        Assert.Equal(previewVerdict, body.Analysis.Overall);
        Assert.True(body.IsComplete);
    }

    [Theory]
    [InlineData("SELECT dbo.f(1);", "dbo.f")]
    [InlineData("SELECT * FROM dbo.tf(1);", "dbo.tf")]
    [InlineData("SELECT * FROM tf(1);", "tf")]
    [InlineData("SELECT Id FROM dbo.T WHERE dbo.f(Id)>0;", "dbo.f")]
    [InlineData("RETURN dbo.f();", "dbo.f")]
    [InlineData("SELECT * FROM dbo.T t CROSS APPLY dbo.tf(t.Id) f;", "dbo.tf")]
    public void UninspectedFunctionCallsCannotDisappearBehindGreenStatements(string sql, string target)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        var call = Assert.Single(result.Findings, f => f.Action == "FUNCTION");
        Assert.Equal(Verdict.Unknown, call.Verdict);
        Assert.Equal(target, call.Target);
        Assert.False(result.IsGreen);
        Assert.Contains(target, sql.Substring(call.StartOffset, call.Length));
    }

    [Theory]
    [InlineData("SELECT COUNT(*), ABS(-1), GETDATE() FROM dbo.T;")]
    [InlineData("SELECT * FROM STRING_SPLIT('a,b', ',');")]
    [InlineData("SELECT * FROM GENERATE_SERIES(1,3);")]
    public void RecognizedBuiltinsKeepTheirExistingBehaviour(string sql) => Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze(sql).Overall);

    [Theory]
    [InlineData("SELECT * FROM OPENQUERY(srv, 'DELETE dbo.T; SELECT 1 AS Id');")]
    [InlineData("SELECT * FROM OPENROWSET('MSOLEDBSQL','Server=srv;Trusted_Connection=yes;','DELETE dbo.T; SELECT 1 AS Id');")]
    [InlineData("EXEC(N'DELETE dbo.T;') AT srv;")]
    [InlineData("DECLARE @s nvarchar(max)=N'DELETE dbo.T;'; EXEC(@s) AT srv;")]
    public void RemotePreviewDoesNotTurnUnknownExecutionIntoProvenLocalSql(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.Unknown, result.Overall);
        var body = Assert.Single(result.CodeBodies);
        Assert.Equal(CodeBodyKind.RemoteQuery, body.Kind);
        var deletion = Assert.Single(body.Analysis.Findings, f => f.Action == "DELETE");
        Assert.Equal(Verdict.RealWrite, deletion.Verdict);
        Assert.NotEmpty(deletion.SourceRanges);
        Assert.Contains("DELETE dbo.T", string.Concat(deletion.SourceRanges.Select(r => sql.Substring(r.StartOffset, r.Length))));
    }

    [Theory]
    [InlineData("EXEC msdb.dbo.sp_add_jobstep @job_name=N'Example', @subsystem=N'TSQL', @command=N'DELETE dbo.T;';")]
    [InlineData("DECLARE @s nvarchar(max)=N'DELETE dbo.T;'; EXEC msdb.dbo.sp_update_jobstep @job_id='00000000-0000-0000-0000-000000000000', @step_id=1, @subsystem=N'TSQL', @command=@s;")]
    public void JobCommandIsStoredRatherThanExecutedNow(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Action == "DELETE");
        var body = Assert.Single(result.CodeBodies);
        Assert.Equal(CodeBodyKind.JobCommand, body.Kind);
        Assert.Equal(Verdict.RealWrite, body.Analysis.Overall);
        Assert.NotEmpty(body.Analysis.Findings[0].SourceRanges);
    }

    [Fact]
    public void ExternalAndUnprovenBodiesAreExplicitlyUnavailable()
    {
        var external = SqlSafetyAnalyzer.Analyze("CREATE FUNCTION dbo.f() RETURNS int AS EXTERNAL NAME a.b.c;");
        var body = Assert.Single(external.CodeBodies);
        Assert.False(body.IsComplete);
        Assert.Equal(Verdict.Unknown, body.Analysis.Overall);
        var job = SqlSafetyAnalyzer.Analyze("EXEC msdb.dbo.sp_add_jobstep @job_name=N'Example', @subsystem=N'PowerShell', @command=N'DELETE dbo.T;';");
        Assert.False(Assert.Single(job.CodeBodies).IsComplete);
        Assert.DoesNotContain(job.CodeBodies[0].Analysis.Findings, f => f.Action == "DELETE");
    }

    [Fact]
    public void VisibleDefinitionDoesNotProveACallAndCallsDoNotLeakOutOfDefinitions()
    {
        const string sql = "CREATE FUNCTION dbo.f() RETURNS int AS BEGIN RETURN dbo.other(); END;\nGO\nSELECT dbo.f();";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Single(result.Findings, f => f.Action == "FUNCTION" && f.Target == "dbo.f");
        Assert.DoesNotContain(result.Findings, f => f.Target == "dbo.other");
        Assert.Contains(Assert.Single(result.CodeBodies).Analysis.Findings, f => f.Target == "dbo.other" && f.Verdict == Verdict.Unknown);
    }
}
