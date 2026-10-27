using System.Text;

namespace SqlSafetyGuard.ServerHarness;

/// <summary>
/// Realistic large scripts like the ones on the company server: hundreds to thousands of lines, many temp tables, CTE
/// chains, nested derived tables, UPDATE ... FROM joins, MERGE into temp tables, TRY/CATCH, table variables, and one
/// very long single-line SELECT ... JOIN ... WHERE. "gen:green" scripts only write temp objects; "gen:red" scripts bury
/// one real write somewhere in the middle. Every script runs without errors on the sandbox schema.
/// </summary>
internal static class Generator
{
    public static IEnumerable<Case> All()
    {
        yield return new Case("gen:green-600", "generated", Script(600));
        yield return new Case("gen:green-1000", "generated", Script(1000));
        yield return new Case("gen:green-4500", "generated", Script(4500));
        yield return new Case("gen:green-9000", "generated", Script(9000));
        yield return new Case("gen:green-longline", "generated", "SET NOCOUNT ON;\n" + LongLine(25_000) + "\n");
        yield return new Case("gen:green-nogo-2000", "generated", Script(2000, goEvery: 0));
        foreach (int steps in new[] { 100, 330, 700 })
        {
            yield return new Case($"gen:green-semicolon-free-{steps}", "generated", SemicolonFree(steps, false));
            yield return new Case($"gen:red-semicolon-free-{steps}", "generated", SemicolonFree(steps, true));
        }
        yield return new Case("gen:green-early-return", "generated",
            "DECLARE @flag bit = 0; IF @flag = 1 RETURN; SELECT 1;");
        yield return new Case("gen:red-early-return-open-transaction", "generated",
            "DECLARE @flag bit = 0; IF @flag = 1 RETURN; BEGIN TRAN; UPDATE dbo.Orders SET Qty = Qty + 1 WHERE Id = 7;");

        yield return new Case("gen:red-buried-update-5000", "generated", Script(5000, buried:
            "UPDATE o\nSET o.Qty = o.Qty + 1\nFROM dbo.Orders AS o\nJOIN #t3 AS t ON t.Id = o.Id\nWHERE t.Qty >= 0;\n"));
        yield return new Case("gen:red-nested-dml-1000", "generated", Script(1000, buried:
            "DROP TABLE IF EXISTS #audit;\nCREATE TABLE #audit (Id int);\nINSERT INTO #audit (Id)\nSELECT d.Id\nFROM (DELETE FROM dbo.AuditLog OUTPUT deleted.Id WHERE Id > 10) AS d;\n"));
        yield return new Case("gen:red-hash-alias-1000", "generated", Script(1000, buried:
            "UPDATE [#t1]\nSET Qty = [#t1].Qty + 1\nFROM dbo.Orders AS [#t1]\nWHERE [#t1].Id = 1;\n"));
        yield return new Case("gen:red-tempdb-permanent-1000", "generated", Script(1000, buried:
            "UPDATE tempdb.dbo.SharedTable\nSET Qty = Qty + 1;\n"));
        yield return new Case("gen:red-longline-tail-write", "generated",
            "SET NOCOUNT ON;\n" + LongLine(20_000) + " UPDATE dbo.Products SET Price = Price + 1 WHERE Id = 3;\n");
        yield return new Case("gen:red-open-transaction-1000", "generated", Script(1000, buried:
            "BEGIN TRAN;\nUPDATE dbo.Customers SET Name = Name + N'!' WHERE Id = 2;\n", noCommit: true));
    }

    public static bool ExpectGreen(Case c) => c.Id.StartsWith("gen:green", StringComparison.Ordinal);

    public static bool ExpectWrite(Case c) => c.Id.StartsWith("gen:red", StringComparison.Ordinal);

    private static string SemicolonFree(int steps, bool write)
    {
        var sql = new StringBuilder("SET NOCOUNT ON\n");
        for (int index = 0; index < steps; index++)
        {
            if (write && index == steps / 2)
                sql.AppendLine("UPDATE dbo.Orders SET Qty = Qty + 1 WHERE Id = 7");
            sql.AppendLine($@"IF OBJECT_ID('tempdb..#work{index}') IS NOT NULL DROP TABLE #work{index}
SELECT o.Id, o.Qty
INTO #work{index}
FROM dbo.Orders o
JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE o.Qty >= 0
UPDATE work SET Qty = source.Qty
FROM #work{index} work
JOIN dbo.Orders source ON source.Id = work.Id
IF EXISTS (SELECT 1 FROM #work{index} WHERE Qty > 100)
    UPDATE #work{index} SET Qty = Qty - 1 WHERE Qty > 100
SELECT COUNT(*) FROM #work{index}
DROP TABLE #work{index}");
        }
        return sql.ToString();
    }

    private static string Script(int lines, string? buried = null, int goEvery = 20, bool noCommit = false)
    {
        var sb = new StringBuilder();
        sb.Append("/* Generated nightly-style job: ").Append(lines).Append(" lines, temp tables, CTEs, nested queries. */\n");
        sb.Append("SET NOCOUNT ON;\nSET XACT_ABORT ON;\n\n");
        int unit = 0;
        bool buriedDone = buried == null;
        while (CountLines(sb) < lines)
        {
            if (!buriedDone && CountLines(sb) > lines * 0.6)
            {
                sb.Append("\n-- maintenance step\n").Append(buried).Append('\n');
                buriedDone = true;
            }

            sb.Append(Unit(unit, goEvery > 0 ? unit % 25 : unit));
            if (unit % 6 == 5)
                sb.Append(LongLine(3_000)).Append('\n');
            unit++;
            if (goEvery > 0 && unit % goEvery == 0)
                sb.Append("GO\n");
        }

        if (!buriedDone)
            sb.Append(buried);
        sb.Append("\n-- cleanup\n");
        for (int k = 0; k < (goEvery > 0 ? Math.Min(unit, 25) : unit); k++)
            sb.Append($"DROP TABLE IF EXISTS #t{k};\nDROP TABLE IF EXISTS #agg{k};\n");
        if (noCommit)
            sb.Append("-- (no COMMIT: left for the operator)\n");
        return sb.ToString();
    }

    private static int CountLines(StringBuilder sb)
    {
        int n = 0;
        for (int i = 0; i < sb.Length; i++)
            if (sb[i] == '\n') n++;
        return n;
    }

    private static string Unit(int i, int k) => $@"
-- step {i}
DROP TABLE IF EXISTS #t{k};
SELECT o.Id,
       o.CustomerId,
       o.Qty,
       o.Status
INTO #t{k}
FROM dbo.Orders AS o
WHERE o.Qty >= {i % 5};

DROP TABLE IF EXISTS #agg{k};
;WITH c1 AS (
    SELECT t.CustomerId, SUM(t.Qty) AS q
    FROM #t{k} AS t
    GROUP BY t.CustomerId
), c2 AS (
    SELECT c1.CustomerId, c1.q, cu.Name
    FROM c1
    JOIN dbo.Customers AS cu ON cu.Id = c1.CustomerId
)
SELECT c2.CustomerId, c2.q, c2.Name
INTO #agg{k}
FROM c2;

UPDATE t
SET t.Qty = x.q
FROM #t{k} AS t
JOIN (
    SELECT d.CustomerId, MAX(d.q) AS q
    FROM (SELECT o.CustomerId, o.Qty AS q FROM dbo.Orders AS o WHERE o.Status = 'open') AS d
    GROUP BY d.CustomerId
) AS x ON x.CustomerId = t.CustomerId;

MERGE #t{k} AS tgt
USING (SELECT o.Id, o.Qty FROM dbo.Orders AS o WHERE o.Id % 7 = {i % 7}) AS src
   ON tgt.Id = src.Id
WHEN MATCHED THEN UPDATE SET tgt.Qty = src.Qty
WHEN NOT MATCHED THEN INSERT (Id, CustomerId, Qty, Status) VALUES (src.Id, 0, src.Qty, 'x');

DELETE t
FROM #t{k} AS t
JOIN dbo.Products AS p ON p.Id = t.Id
WHERE p.Price < 0;

DECLARE @v{i} TABLE (Id int, Qty int);
INSERT @v{i} (Id, Qty)
SELECT TOP (5) Id, Qty FROM #t{k} ORDER BY Id;

IF EXISTS (SELECT 1 FROM @v{i} WHERE Qty > 100)
BEGIN
    UPDATE #agg{k} SET q = q + 1 WHERE CustomerId = 1;
END;

BEGIN TRY
    INSERT INTO #agg{k} (CustomerId, q, Name)
    SELECT TOP (1) c.Id, 0, c.Name
    FROM dbo.Customers AS c
    WHERE c.Id IN (SELECT a.CustomerId FROM (SELECT CustomerId FROM #t{k} GROUP BY CustomerId) AS a);
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH;
";

    /// <summary>One line: SELECT with many joins and a long WHERE, about <paramref name="length"/> characters.</summary>
    private static string LongLine(int length)
    {
        var sb = new StringBuilder("SELECT TOP (10) o.Id, o.Qty, c.Name");
        for (int j = 1; j <= 30; j++)
            sb.Append($", p{j}.Name AS n{j}");
        sb.Append(" FROM dbo.Orders AS o JOIN dbo.Customers AS c ON c.Id = o.CustomerId");
        for (int j = 1; j <= 30; j++)
            sb.Append($" LEFT JOIN dbo.Products AS p{j} ON p{j}.Id = (o.Id + {j}) % 40 + 1");
        sb.Append(" WHERE (o.Qty >= 0 OR o.Status = 'open')");
        int n = 0;
        while (sb.Length < length)
            sb.Append($" AND (o.Id <> {100000 + n++} OR c.Name LIKE N'%x{n}%')");
        sb.Append(" ORDER BY o.Id;");
        return sb.ToString();
    }
}
