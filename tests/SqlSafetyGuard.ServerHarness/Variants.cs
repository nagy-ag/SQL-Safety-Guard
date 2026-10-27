namespace SqlSafetyGuard.ServerHarness;

/// <summary>
/// One write written many ways: every write shape against every target, plain, and against a permanent and a temp
/// target inside each wrapper (transaction, TRY/CATCH, IF, loop, dynamic SQL, after GO). The verdict must not depend on
/// how the write is spelled or wrapped: permanent targets are "gen:red" (must be non-green and really write), temp
/// targets "gen:green" (must be green, write nothing and run without errors), so a variant that fails to run is caught.
/// </summary>
internal static class Variants
{
    private sealed record Target(string Key, string Sql, bool Permanent, string Setup = "");

    private sealed record Shape(string Key, Func<string, string> Sql, bool AlwaysWrites = false, bool TableVariableOk = true);

    private const string Columns = "Id, CustomerId, Qty, Status";

    private static readonly Target[] Targets =
    {
        new("perm", "dbo.Orders", true),
        new("schemaless", "Orders", true),
        new("bracketed", "[dbo].[Orders]", true),
        new("threepart", "SqlGuard_Prod.dbo.Orders", true),
        new("crossdb", "SqlGuard_Dev.dbo.Orders", true),
        new("temp", "#w", false, "SELECT * INTO #w FROM dbo.Orders;\n"),
        new("tablevar", "@w", false,
            "DECLARE @w TABLE (Id int PRIMARY KEY, CustomerId int, Qty int, Status varchar(20));\n" +
            $"INSERT INTO @w ({Columns}) SELECT {Columns} FROM dbo.Orders;\n"),
    };

    private static readonly Shape[] Shapes =
    {
        new("update-where", t => $"UPDATE {t} SET Qty = Qty + 1 WHERE Id = 7;"),
        new("update-join", t => $"UPDATE a SET Qty = a.Qty + 1 FROM {t} AS a JOIN #ids AS i ON i.Id = a.Id;"),
        new("update-hash-alias", t => $"UPDATE [#a] SET Qty = 1 FROM {t} AS [#a] WHERE [#a].Id = 7;"),
        new("update-cte", t => $"WITH c AS (SELECT * FROM {t} WHERE Id = 7) UPDATE c SET Qty = 1;"),
        new("update-hash-cte", t => $"WITH [#c] AS (SELECT * FROM {t} WHERE Id = 7) UPDATE [#c] SET Qty = 1;"),
        new("update-derived", t => $"UPDATE d SET Qty = 1 FROM (SELECT Id, Qty FROM {t} WHERE Id = 7) AS d;"),
        new("delete-where", t => $"DELETE FROM {t} WHERE Id = 7;"),
        new("delete-join", t => $"DELETE a FROM {t} AS a JOIN #ids AS i ON i.Id = a.Id;"),
        new("delete-top", t => $"DELETE TOP (1) FROM {t};"),
        new("insert-values", t => $"INSERT INTO {t} ({Columns}) VALUES (9001, 1, 1, 'new');"),
        new("insert-select", t => $"INSERT INTO {t} ({Columns}) SELECT Id + 10000, CustomerId, Qty, Status FROM dbo.Orders WHERE Id <= 3;"),
        new("merge", t => $"MERGE {t} AS t USING (SELECT 7 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN UPDATE SET Qty = 5;"),
        new("composable-delete", t => $"INSERT INTO #log (Id) SELECT Id FROM (DELETE FROM {t} OUTPUT deleted.Id WHERE Id = 7) AS x;"),
        new("output-into", t => $"DELETE FROM #scratch OUTPUT deleted.Id + 20000, deleted.CustomerId, deleted.Qty, deleted.Status INTO {t} ({Columns}) WHERE Id = 7;"),
        new("truncate", t => $"TRUNCATE TABLE {t};", TableVariableOk: false),
        new("sequence", t => $"UPDATE {t} SET Qty = NEXT VALUE FOR dbo.Seq WHERE Id = 7;", AlwaysWrites: true),
        // The sandbox pre-creates every table the corpus names, SELECT INTO targets included.
        new("select-into", t => $"DROP TABLE IF EXISTS {IntoName(t)}; SELECT {Columns} INTO {IntoName(t)} FROM dbo.Orders WHERE Id <= 3;",
            TableVariableOk: false),
    };

    private static readonly (string Key, Func<string, string> Wrap)[] Wrappers =
    {
        ("tran", x => $"BEGIN TRAN;\n{x}\nCOMMIT;"),
        ("trycatch", x => $"BEGIN TRY\n{x}\nEND TRY\nBEGIN CATCH\nTHROW;\nEND CATCH;"),
        ("if", x => $"IF EXISTS (SELECT 1 FROM dbo.Customers)\nBEGIN\n{x}\nEND;"),
        ("while", x => $"DECLARE @i int = 0;\nWHILE @i < 1\nBEGIN\n{x}\nSET @i += 1;\nEND;"),
        ("exec", x => $"EXEC (N'{x.Replace("'", "''")}');"),
        ("sp_executesql", x => $"EXEC sys.sp_executesql N'{x.Replace("'", "''")}';"),
        ("after-go", x => x),
    };

    private const string CommonSetup =
        "SET NOCOUNT ON;\n" +
        "CREATE TABLE #ids (Id int PRIMARY KEY); INSERT INTO #ids (Id) VALUES (7), (8);\n" +
        "CREATE TABLE #log (Id int);\n" +
        "SELECT * INTO #scratch FROM dbo.Orders;\n";

    public static IEnumerable<Case> All()
    {
        foreach (var shape in Shapes)
        {
            foreach (var target in Targets)
            {
                if (!Applies(shape, target))
                    continue;
                yield return Make(shape, target, "plain", shape.Sql(target.Sql), afterGo: false);
            }

            // Wrapped variants use #w, not @w: a table variable from the outer batch isn't visible inside dynamic SQL.
            foreach (var target in Targets.Where(t => t.Key is "perm" or "temp"))
            {
                foreach (var (key, wrap) in Wrappers)
                {
                    yield return Make(shape, target, key, wrap(shape.Sql(target.Sql)), afterGo: key == "after-go");
                }
            }
        }
    }

    private static bool Applies(Shape shape, Target target) => target.Key != "tablevar" || shape.TableVariableOk;

    private static Case Make(Shape shape, Target target, string wrapper, string body, bool afterGo)
    {
        bool writes = target.Permanent || shape.AlwaysWrites;
        var setup = CommonSetup + target.Setup;
        var sql = afterGo ? "SELECT 1;\nGO\n" + setup + body + "\n" : setup + body + "\n";
        return new Case($"gen:{(writes ? "red" : "green")}-var-{shape.Key}-{target.Key}-{wrapper}", "generated", sql);
    }

    /// <summary>SELECT INTO creates its target, so it gets a new name of the same kind.</summary>
    private static string IntoName(string target) => target switch
    {
        "#w" => "#w_copy",
        "Orders" => "Orders_Copy",
        _ => target.Replace("Orders]", "Orders_Copy]").Replace(".Orders", ".Orders_Copy"),
    };
}
