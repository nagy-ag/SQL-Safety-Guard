using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

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
