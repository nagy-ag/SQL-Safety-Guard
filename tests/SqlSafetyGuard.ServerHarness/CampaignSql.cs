using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal static class CampaignSql
{
    public static string Quote(string name) => "[" + name.Replace("]", "]]") + "]";
    public static string Literal(string value) => "N'" + value.Replace("'", "''") + "'";
    public static SqlConnection Open(string connectionString, string database = "master", bool? pooled = null)
    {
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = database, Pooling = pooled ?? database is "master" or "SgRun_Results", TrustServerCertificate = true, ConnectTimeout = 15, ApplicationName = "SqlGuardCampaign" };
        var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();
        return connection;
    }
    public static void Execute(SqlConnection connection, string sql, int timeout = 120, CancellationToken cancellation = default, bool sqlCmdMode = false)
    {
        var script = SqlCmdLite.Split(sql, sqlCmdMode);
        if (script.NotRunnable != null || script.Batches.Any(batch => batch.Error != null || batch.Repeat != 1))
            throw new InvalidDataException("Campaign accepts plain SQL and nonrepeated GO batches only.");
        foreach (var batch in script.Batches)
        {
            if (string.IsNullOrWhiteSpace(batch.Sql)) continue;
            using var command = new SqlCommand(batch.Sql, connection) { CommandTimeout = timeout };
            using var reader = command.ExecuteReaderAsync(cancellation).GetAwaiter().GetResult();
            do { while (reader.Read()) { cancellation.ThrowIfCancellationRequested(); } } while (reader.NextResult());
        }
    }
    public static object? Scalar(SqlConnection connection, string sql, int timeout = 30)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = timeout };
        return command.ExecuteScalar();
    }
    public static List<string[]> Rows(SqlConnection connection, string sql, int timeout = 30)
    {
        for (int attempt = 0; ; attempt++)
        {
            try { return ReadRows(connection, sql, timeout); }
            catch (SqlException exception) when (exception.Number == 1205 && attempt < 2) { Thread.Sleep(50 * (attempt + 1)); }
        }
    }
    private static List<string[]> ReadRows(SqlConnection connection, string sql, int timeout)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = timeout };
        using var reader = command.ExecuteReader();
        var rows = new List<string[]>();
        while (reader.Read()) rows.Add(Enumerable.Range(0, reader.FieldCount).Select(index => reader.IsDBNull(index) ? "<NULL>" : Convert.ToString(reader.GetValue(index), System.Globalization.CultureInfo.InvariantCulture)!).ToArray());
        return rows;
    }
    public static string Docker(string container, params string[] arguments)
    {
        var start = new ProcessStartInfo("docker") { RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument.Replace("{{container}}", container));
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Docker command timed out."); }
        if (process.ExitCode != 0) throw new InvalidOperationException(error.GetAwaiter().GetResult());
        return output.GetAwaiter().GetResult();
    }
    public static string VerifyContainer(SqlConnection admin)
    {
        var container = Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONTAINER") ?? "";
        if (!System.Text.RegularExpressions.Regex.IsMatch(container, "^sqlguard-cov-[a-f0-9]{16}$")) throw new InvalidOperationException("Owned container name required.");
        using var document = JsonDocument.Parse(Docker(container, "inspect", container));
        var inspected = document.RootElement[0];
        if (inspected.GetProperty("Config").GetProperty("Labels").GetProperty("sqlguard.coverage").GetString() != "disposable" || inspected.GetProperty("Mounts").GetArrayLength() != 0)
            throw new InvalidOperationException("Container label/mount ownership checks failed.");
        foreach (var port in inspected.GetProperty("NetworkSettings").GetProperty("Ports").EnumerateObject())
            if (port.Value.ValueKind == JsonValueKind.Array && port.Value.EnumerateArray().Any(binding => binding.GetProperty("HostIp").GetString() != "127.0.0.1"))
                throw new InvalidOperationException("Only IPv4 loopback bindings are allowed.");
        if (Convert.ToString(Scalar(admin, "SELECT SERVERPROPERTY('MachineName')")) != container) throw new InvalidOperationException("Container SQL identity mismatch.");
        return container;
    }
}
