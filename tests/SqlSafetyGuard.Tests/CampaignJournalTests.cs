using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignJournalTests
{
    [Fact]
    public void NewerDurableReceiptSupersedesStaleSqlResult()
    {
        var persisted = new CampaignResult
        {
            Case = new CampaignCase("case", "SELECT 1;"), Sql = "SELECT 1;",
            Verdict = new Verdict("ReadOnly", true, [], 0, null),
            EffectiveClassification = "ReadOnly", Families = [], Outcome = "PASS"
        };
        var retained = new CampaignResult
        {
            Case = persisted.Case, Sql = persisted.Sql, Verdict = persisted.Verdict,
            EffectiveClassification = "ReadOnly", Families = [], Outcome = "CRASH"
        };
        var result = Assert.Single(CampaignJournal.Merge([persisted], [retained]));
        Assert.Same(retained, result);
        Assert.Equal("CRASH", result.Outcome);
    }

    [Fact]
    public void RoundTripsDurableReceiptAndIgnoresUnfinishedWrite()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SgRun_journal_" + Guid.NewGuid().ToString("N"));
        try
        {
            var result = new CampaignResult
            {
                Case = new CampaignCase("quoted/identifier", "SELECT 1;"), Sql = "SELECT 1;",
                Verdict = new Verdict("ReadOnly", true, [], 1, null), EffectiveClassification = "ReadOnly",
                Families = ["SelectStatement"], Outcome = "PASS", IsolationVerified = true
            };
            CampaignJournal.Save(directory, "identity", result);
            File.WriteAllText(Path.Combine(directory, "receipts", "interrupted.tmp"), "incomplete");
            var retained = Assert.Single(CampaignJournal.Load(directory, "identity"));
            Assert.Equal(result.Case, retained.Case);
            Assert.Equal("PASS", retained.Outcome);
            Assert.True(retained.IsolationVerified);
            Assert.Throws<InvalidDataException>(() => CampaignJournal.Load(directory, "other-build").ToArray());
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public void RejectsRenamedReceiptRatherThanTrustingItsPayload()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SgRun_journal_" + Guid.NewGuid().ToString("N"));
        try
        {
            CampaignJournal.Save(directory, "identity", new CampaignResult
            {
                Case = new CampaignCase("case", ""), Sql = "", Verdict = new Verdict("ReadOnly", true, [], 0, null),
                EffectiveClassification = "ReadOnly", Families = []
            });
            File.Move(Directory.GetFiles(Path.Combine(directory, "receipts"), "*.json").Single(), Path.Combine(directory, "receipts", "wrong.json"));
            Assert.Throws<InvalidDataException>(() => CampaignJournal.Load(directory, "identity").ToArray());
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }
}
