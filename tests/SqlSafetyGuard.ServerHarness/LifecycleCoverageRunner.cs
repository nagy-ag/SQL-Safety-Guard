using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal static class LifecycleCoverageRunner
{
    public static int Run(Args options, AnalyzerHost analyzer)
    {
        if (options.Keep || options.Coverage != null || options.IsolatedCoverage != null || options.Filter != null ||
            options.Limit != 0 || options.Tests.Count + options.Sql.Count + options.Samples.Count != 0)
            throw new ArgumentException("Lifecycle probes must run alone against their owned container, with no discovery or selection options.");
        if (options.ValidateCoverage)
        {
            CoverageCorpus.StatementTypes("KILL 100; SHUTDOWN WITH NOWAIT;");
            return 0;
        }
        var container = Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONTAINER") ?? "";
        if (!Regex.IsMatch(container, "^sqlguard-cov-[a-f0-9]{16}$"))
            throw new InvalidOperationException("Run-owned container name required.");
        using var inspection = JsonDocument.Parse(Docker("inspect", container));
        var inspected = inspection.RootElement[0];
        if (inspected.GetProperty("Config").GetProperty("Labels").GetProperty("sqlguard.coverage").GetString() != "disposable" ||
            inspected.GetProperty("Mounts").GetArrayLength() != 0)
            throw new InvalidOperationException("Disposable ownership label and absence of mounts required.");
        var containerId = inspected.GetProperty("Id").GetString();
        var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONNECTION") ?? "")
        { InitialCatalog = "master", Pooling = false, ConnectTimeout = 10 };
        using var admin = new SqlConnection(builder.ConnectionString);
        admin.Open();
        if (Convert.ToString(Scalar(admin, "SELECT CONVERT(nvarchar(128),SERVERPROPERTY('MachineName'));")) != container)
            throw new InvalidOperationException("SQL endpoint does not match the run-owned container.");
        var serverVersion = admin.ServerVersion;
        var startedUtc = DateTimeOffset.UtcNow;
        var results = new List<object>();
        var failed = false;
        var database = "SqlGuard_Lifecycle_" + Guid.NewGuid().ToString("N");
        Execute(admin, $"CREATE DATABASE [{database}];");
        string? killError = null;
        var killSql = "";
        var killVerify = "";
        var victimSpid = 0;
        int? before = null;
        int? after = null;
        int? rolledBackValue = null;
        Verdict? killVerdict = null;
        try
        {
            Execute(admin, $"CREATE TABLE [{database}].dbo.Counter(Value int NOT NULL); INSERT [{database}].dbo.Counter VALUES(0);");
            builder.InitialCatalog = database;
            using var victim = new SqlConnection(builder.ConnectionString);
            victim.Open();
            victimSpid = Convert.ToInt32(Scalar(victim, "SELECT @@SPID;"));
            if (victimSpid == Convert.ToInt32(Scalar(admin, "SELECT @@SPID;")) || victimSpid <= 50)
                throw new InvalidOperationException("Invalid owned victim session.");
            Execute(victim, "BEGIN TRANSACTION; UPDATE dbo.Counter SET Value=1;");
            killSql = $"KILL {victimSpid};";
            killVerify = $"SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE session_id={victimSpid};";
            killVerdict = analyzer.Analyze(killSql);
            before = Convert.ToInt32(Scalar(admin, killVerify));
            Execute(admin, killSql);
            after = Convert.ToInt32(Scalar(admin, killVerify));
            rolledBackValue = Convert.ToInt32(Scalar(admin, $"SELECT Value FROM [{database}].dbo.Counter;"));
            if (before != 1 || after != 0 || rolledBackValue != 0 || killVerdict.Overall != "RealWrite" || killVerdict.Error != null)
                throw new InvalidDataException("Owned KILL did not cancel its victim and roll back its uncommitted write as expected.");
        }
        catch (Exception exception) { killError = exception.Message; failed = true; }
        finally
        {
            Execute(admin, $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}];");
        }
        results.Add(new { Id = "owned-session-kill", StatementTypes = new[] { "KillStatement" }, Sql = killSql, VerifySql = killVerify,
            Status = killError == null ? "pass" : "failure", Before = before, After = after, RolledBackValue = rolledBackValue,
            VictimSpid = victimSpid, Verdict = killVerdict, Error = killError });
        Save();
        var shutdownSql = "SHUTDOWN WITH NOWAIT;";
        var shutdownVerdict = analyzer.Analyze(shutdownSql);
        string? transport = null;
        var shutdownAt = DateTimeOffset.UtcNow;
        try { Execute(admin, shutdownSql); }
        catch (SqlException exception) { transport = exception.Message; }
        bool stopped = false;
        int? exitCode = null;
        DateTimeOffset? finishedAt = null;
        var healthyExitState = false;
        for (var attempt = 0; attempt < 30; attempt++)
        {
            using var stateDocument = JsonDocument.Parse(Docker("inspect", container));
            var stateContainer = stateDocument.RootElement[0];
            if (stateContainer.GetProperty("Id").GetString() != containerId)
                throw new InvalidOperationException("Container identity changed during shutdown observation.");
            var state = stateContainer.GetProperty("State");
            if (!state.GetProperty("Running").GetBoolean())
            {
                stopped = true;
                exitCode = state.GetProperty("ExitCode").GetInt32();
                finishedAt = DateTimeOffset.Parse(state.GetProperty("FinishedAt").GetString()!);
                healthyExitState = !state.GetProperty("OOMKilled").GetBoolean() &&
                    !state.GetProperty("Dead").GetBoolean() && string.IsNullOrEmpty(state.GetProperty("Error").GetString());
                break;
            }
            Thread.Sleep(1000);
        }
        var shutdownLog = Docker("logs", "--since", shutdownAt.ToString("O"), container);
        var requestedShutdownLogged = shutdownLog.Contains("Server shut down by NOWAIT request from login", StringComparison.Ordinal);
        var shutdownPassed = stopped && healthyExitState && requestedShutdownLogged && finishedAt >= shutdownAt &&
            shutdownVerdict.Overall == "RealWrite" && shutdownVerdict.Error == null;
        failed |= !shutdownPassed;
        results.Add(new { Id = "owned-server-shutdown", StatementTypes = new[] { "ShutdownStatement" }, Sql = shutdownSql,
            Status = shutdownPassed ? "pass" : "failure", Before = "running", After = stopped ? "stopped" : "running",
            ExitCode = exitCode, FinishedAt = finishedAt, HealthyExitState = healthyExitState, RequestedShutdownLogged = requestedShutdownLogged,
            ShutdownLog = shutdownLog, Verdict = shutdownVerdict, ExpectedTransportDisconnect = transport,
            Error = shutdownPassed ? null : "Owned container stop, shutdown log, state or verdict assertion failed." });
        Save();
        Console.WriteLine($"Lifecycle probes: KILL={(killError == null ? "pass" : killError)}; SHUTDOWN={(shutdownPassed ? "pass" : "failure")}");
        return failed || options.RequireComplete ? 1 : 0;

        void Save() => File.WriteAllText(Path.Combine(options.Out, "lifecycle-results.json"), JsonSerializer.Serialize(new
        {
            StartedUtc = startedUtc, RunUtc = DateTimeOffset.UtcNow, Container = container, ContainerId = containerId, ServerVersion = serverVersion,
            AnalyzerSha256 = Hash(analyzer.AssemblyPath), HarnessSha256 = Hash(typeof(LifecycleCoverageRunner).Assembly.Location),
            Complete = false, Results = results
        }, CoverageCorpus.Json));
    }

    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static object? Scalar(SqlConnection connection, string sql)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = 15 };
        return command.ExecuteScalar();
    }

    private static void Execute(SqlConnection connection, string sql)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        command.ExecuteNonQuery();
    }

    private static string Docker(params string[] arguments)
    {
        var start = new ProcessStartInfo("docker") { RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0) throw new InvalidOperationException("Docker ownership/status check failed: " + error);
        return output;
    }
}
