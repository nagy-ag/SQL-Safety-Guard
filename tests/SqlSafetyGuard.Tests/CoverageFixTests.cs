using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>The false alarms and under-warnings found by the statement coverage review (docs/statement-coverage.md).</summary>
public class FalseAlarmTests
{
    [Theory]
    [InlineData("READTEXT dbo.Docs.Body @ptr 0 10")]
    [InlineData("OPEN SYMMETRIC KEY k DECRYPTION BY CERTIFICATE c")]
    [InlineData("CLOSE SYMMETRIC KEY k")]
    [InlineData("OPEN MASTER KEY DECRYPTION BY PASSWORD = 'x'")]
    [InlineData("CLOSE MASTER KEY")]
    [InlineData("LINENO 10")]
    [InlineData("GET CONVERSATION GROUP @g FROM dbo.q")]
    [InlineData("WAITFOR (GET CONVERSATION GROUP @g FROM dbo.q), TIMEOUT 100")]
    [InlineData("USE FEDERATION f (k = 1) WITH FILTERING = OFF, RESET")]
    public void ReadOrSessionOnly_IsGreen(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Theory]
    [InlineData("DROP STATISTICS #T.st", Verdict.TempOnly)]
    [InlineData("DROP STATISTICS dbo.T.st", Verdict.RealWrite)]
    [InlineData("DROP STATISTICS #A.s1, dbo.B.s2", Verdict.RealWrite)]
    public void DropStatistics_FollowsTempRules(string sql, Verdict expected)
    {
        Assert.Equal(expected, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }
}

public class TransactionTests
{
    private static AnalysisResult Analyze(string sql) => SqlSafetyAnalyzer.Analyze(sql);

    [Fact]
    public void LoneCommit_IsUnknown()
    {
        var result = Analyze("COMMIT");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains("started outside this text", Assert.Single(result.Findings).Note);
    }

    [Fact]
    public void ExtraCommit_IsUnknown()
    {
        var result = Analyze("BEGIN TRAN; SELECT 1; COMMIT; COMMIT;");
        Assert.Equal(1, result.Findings.Count(f => f.Action.StartsWith("COMMIT", StringComparison.Ordinal) && f.Verdict == Verdict.Unknown));
    }

    [Fact]
    public void BalancedTransaction_IsGreen()
    {
        var result = Analyze("SET NOCOUNT ON; BEGIN TRAN; SELECT 1; COMMIT");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void NestedTransactions_Balanced()
    {
        Assert.Equal(Verdict.ReadOnly, Analyze("BEGIN TRAN; BEGIN TRAN; SELECT 1; COMMIT; COMMIT;").Overall);
    }

    [Fact]
    public void BeginWithoutCommit_ReportsOpenTransaction()
    {
        var sql = "SELECT 1;\nBEGIN TRAN;\nUPDATE dbo.T SET a = 1 WHERE b = 2;\n";
        var result = Analyze(sql);

        var open = Assert.Single(result.Findings, f => f.Action == "OPEN TRANSACTION");
        Assert.Equal(Verdict.Unknown, open.Verdict);
        Assert.Equal(2, open.Line);
        Assert.Contains("stays open", open.Note);
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void RollbackCloses()
    {
        var result = Analyze("BEGIN TRAN; UPDATE #t SET a = 1; ROLLBACK;");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void RollbackToSavepoint_KeepsTransactionOpen()
    {
        var result = Analyze("BEGIN TRAN; SAVE TRAN sp1; UPDATE #t SET a = 1; ROLLBACK TRAN sp1;");
        Assert.Contains(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void IfCommitElseRollback_IsBalanced()
    {
        var result = Analyze("BEGIN TRAN; UPDATE #t SET a = 1; IF @@ERROR <> 0 ROLLBACK ELSE COMMIT;");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void TryCatchPattern_IsBalanced()
    {
        var sql = "BEGIN TRY BEGIN TRAN; UPDATE #t SET a = 1; COMMIT; END TRY BEGIN CATCH IF @@TRANCOUNT > 0 ROLLBACK; THROW; END CATCH";
        var result = Analyze(sql);
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void CommitInCatch_AfterTryOpened_IsNotLone()
    {
        var result = Analyze("BEGIN TRY BEGIN TRAN; UPDATE #t SET a = 1; END TRY BEGIN CATCH COMMIT; END CATCH");
        Assert.DoesNotContain(result.Findings, f => f.Action == "COMMIT" && f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void GuardedCommit_IsNotLone()
    {
        var result = Analyze("IF @@TRANCOUNT > 0 COMMIT;");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Fact]
    public void XactStateGuard_ClosesTheTransaction()
    {
        var result = Analyze("BEGIN TRAN; UPDATE #t SET a = 1; IF XACT_STATE() <> 0 ROLLBACK;");
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void UnguardedIf_StaysConservative()
    {
        // Only one branch commits: the transaction may stay open.
        var result = Analyze("BEGIN TRAN; UPDATE #t SET a = 1; IF @x = 1 COMMIT;");
        Assert.Contains(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void TransactionSpansBatches()
    {
        Assert.Equal(Verdict.TempOnly, Analyze("BEGIN TRAN;\nGO\nUPDATE #t SET a = 1;\nGO\nCOMMIT;\n").Overall);
    }

    [Fact]
    public void ImplicitTransactions_LeftOpen_IsReported()
    {
        var result = Analyze("SET IMPLICIT_TRANSACTIONS ON;\nDELETE FROM #t WHERE a = 1;\n");
        var open = Assert.Single(result.Findings, f => f.Action == "OPEN TRANSACTION");
        Assert.Equal(2, open.Line);
        Assert.Contains("IMPLICIT_TRANSACTIONS", open.Note);
    }

    [Fact]
    public void ImplicitTransactions_Committed_IsFine()
    {
        var result = Analyze("SET IMPLICIT_TRANSACTIONS ON; DELETE FROM #t WHERE a = 1; COMMIT;");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void TransactionsInBrokenBatch_DontCount()
    {
        // The batch with BEGIN TRAN has a syntax error, so it never runs: no open transaction; the COMMIT is lone.
        var result = Analyze("BEGIN TRAN;\nSELECT * FROM WHERE;\nGO\nCOMMIT;\n");
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
        Assert.Contains(result.Findings, f => f.Action.StartsWith("COMMIT", StringComparison.Ordinal) && f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void TransactionInLiteralDynamicSql_Counts()
    {
        Assert.Contains(Analyze("EXEC('BEGIN TRAN')").Findings, f => f.Action == "OPEN TRANSACTION");
    }
}

public class SystemProcedureTests
{
    [Theory]
    [InlineData("EXEC sp_rename 'dbo.T', 'T2'", "renames an object")]
    [InlineData("EXEC sp_configure 'show advanced options', 1", "changes server configuration")]
    [InlineData("EXEC xp_cmdshell 'dir'", "operating-system command")]
    [InlineData("EXEC msdb.dbo.sp_add_job @job_name = N'x'", "SQL Agent job")]
    [InlineData("EXEC msdb.dbo.sp_send_dbmail @recipients = 'a@b.c'", "sends an email")]
    [InlineData("EXEC [master].[sys].[sp_addsrvrolemember] 'l', 'sysadmin'", "server role membership")]
    public void KnownWriters_AreRed(string sql, string reason)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
        Assert.Contains(reason, finding.Note);
    }

    [Theory]
    [InlineData("EXEC sp_set_session_context @key = N'a', @value = 1")]
    [InlineData("EXEC sp_readerrorlog")]
    [InlineData("EXEC msdb.dbo.sp_help_job")]
    public void MoreReadOnlyProcedures_AreGreen(string sql)
    {
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Fact]
    public void UnknownProcedure_StaysYellow()
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze("EXEC dbo.usp_Whatever").Overall);
    }

    [Fact]
    public void InsertExecOfKnownWriter_IsRed()
    {
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("INSERT #r EXEC xp_cmdshell 'dir'").Overall);
    }
}

public class DbccTests
{
    [Theory]
    [InlineData("DBCC SHOW_STATISTICS ('dbo.T', ix)")]
    [InlineData("DBCC INPUTBUFFER (55)")]
    [InlineData("DBCC OPENTRAN")]
    [InlineData("DBCC SQLPERF (LOGSPACE)")]
    [InlineData("DBCC USEROPTIONS")]
    [InlineData("DBCC TRACESTATUS")]
    [InlineData("DBCC CHECKDB WITH NO_INFOMSGS")]
    [InlineData("DBCC CHECKTABLE ('dbo.T')")]
    [InlineData("DBCC CHECKIDENT ('dbo.T', NORESEED)")]
    public void ReadOnlyDbcc_IsGreen(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Theory]
    [InlineData("DBCC FREEPROCCACHE", "plan cache")]
    [InlineData("DBCC DROPCLEANBUFFERS", "buffer pool")]
    [InlineData("DBCC SHRINKFILE (1, 10)", "shrinks")]
    [InlineData("DBCC CHECKIDENT ('dbo.T', RESEED, 0)", "reseed")]
    [InlineData("DBCC CHECKIDENT ('dbo.T')", "reseed")]
    [InlineData("DBCC CHECKDB ('x', REPAIR_ALLOW_DATA_LOSS)", "REPAIR")]
    [InlineData("DBCC TRACEON (1222, -1)", "trace flags")]
    public void ChangingDbcc_IsRed(string sql, string reason)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
        Assert.Contains(reason, finding.Note);
    }

    [Fact]
    public void OtherDbcc_StaysYellow()
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze("DBCC MEMORYMAP").Overall);
    }
}
