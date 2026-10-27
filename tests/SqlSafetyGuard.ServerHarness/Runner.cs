using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record Execution(bool Ran, string? SkipReason, int Batches, int FailedBatches, List<string> Errors, long UncommittedLogRecords, bool TimedOut);

/// <summary>Runs one case on the sandbox as the low-privilege harness login, the way SSMS would (batch by batch, continuing after errors).</summary>
internal sealed class Runner
{
    private readonly Sandbox _sandbox;
    private readonly int _timeoutSeconds;
    private readonly SchemaDiscovery _schema;

    public Runner(Sandbox sandbox, SchemaDiscovery schema, int timeoutSeconds)
    {
        _sandbox = sandbox;
        _schema = schema;
        _timeoutSeconds = timeoutSeconds;
    }

    /// <summary>CREATE TABLE for the temp tables a snippet uses without creating, with the shared columns and three rows.</summary>
    private string ProvideTempTables(string sql)
    {
        var sb = new System.Text.StringBuilder();
        List<string> temps;
        try { temps = SchemaDiscovery.TempTablesToProvide(sql); } catch { return ""; }
        var columns = string.Concat(_schema.Columns.Where(c => !c.Equals("Id", StringComparison.OrdinalIgnoreCase) && !c.Equals("Qty", StringComparison.OrdinalIgnoreCase))
            .Select(c => $", [{c.Replace("]", "]]")}] nvarchar(100) NULL"));
        foreach (var t in temps)
        {
            var name = "[" + t.Replace("]", "]]") + "]";
            sb.Append($"CREATE TABLE {name} (Id int NULL, Qty int NULL{columns}); INSERT {name} (Id, Qty) VALUES (1, 1), (2, 2), (3, 3);\n");
        }
        return sb.ToString();
    }

    public Execution Run(string sql)
    {
        var script = SqlCmdLite.Split(sql, sqlCmdMode: SqlCmdLite.LooksLikeSqlCmd(sql));
        if (script.NotRunnable != null)
            return new Execution(false, script.NotRunnable, 0, 0, new List<string>(), 0, false);

        var errors = new List<string>();
        int failed = 0;
        bool timedOut = false;
        long uncommitted;

        using (var conn = new SqlConnection(_sandbox.ConnectionString(Sandbox.Prod)))
        {
            conn.Open();
            int spid;
            using (var cmd = new SqlCommand("SELECT @@SPID", conn))
                spid = Convert.ToInt32(cmd.ExecuteScalar());
            using (var cmd = new SqlCommand($"EXECUTE AS LOGIN = N'{Sandbox.Login}' WITH NO REVERT;", conn))
                cmd.ExecuteNonQuery();
            using (var cmd = new SqlCommand("SELECT IS_SRVROLEMEMBER('sysadmin')", conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) != 0)
                    throw new InvalidOperationException("Impersonation failed; refusing to run a case as sysadmin.");
            }

            var provide = ProvideTempTables(sql);
            if (provide.Length > 0)
            {
                try
                {
                    using var cmd = new SqlCommand(provide, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    // Best effort: the case runs either way.
                }
            }

            foreach (var batch in script.Batches)
            {
                if (batch.Error != null)
                {
                    failed++;
                    errors.Add(batch.Error);
                    continue;
                }

                for (int i = 0; i < Math.Max(1, batch.Repeat); i++)
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                        break;
                    try
                    {
                        using var cmd = new SqlCommand(batch.Sql, conn) { CommandTimeout = _timeoutSeconds };
                        using var reader = cmd.ExecuteReader();
                        do { while (reader.Read()) { } } while (reader.NextResult());
                    }
                    catch (SqlException ex)
                    {
                        failed++;
                        if (ex.Number == -2) timedOut = true;
                        errors.Add($"{ex.Number}: {ex.Message.Split('\n')[0]}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        failed++;
                        errors.Add(ex.Message);
                    }
                }
            }

            // Writes still uncommitted when the script ends (BEGIN TRAN without COMMIT, IMPLICIT_TRANSACTIONS): they are
            // real writes waiting for a COMMIT. tempdb is excluded (temp tables write there legitimately).
            uncommitted = 0;
            var rows = _sandbox.Lines(
                "SELECT ISNULL(SUM(d.database_transaction_log_record_count), 0) FROM sys.dm_tran_session_transactions s " +
                "JOIN sys.dm_tran_database_transactions d ON d.transaction_id = s.transaction_id " +
                $"WHERE s.session_id = {spid} AND d.database_id IN (DB_ID(N'{Sandbox.Prod}'), DB_ID(N'{Sandbox.Dev}'))");
            if (rows.Count > 0 && long.TryParse(rows[0], out var n))
                uncommitted = n;
        } // Closing the connection rolls back anything left open.

        return new Execution(true, null, script.Batches.Count, failed, errors.Take(5).ToList(), uncommitted, timedOut);
    }
}
