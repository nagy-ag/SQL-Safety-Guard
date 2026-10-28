using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record CampaignCase(string Id, string Sql, string Setup = "", string Database = "Core", string Principal = "runner",
    string? ExpectedClassification = null, string[]? RuleIds = null, int Seed = 1, string? ControlVerdict = null, bool Privileged = false, string? Family = null,
    CampaignAnalyzerOptions? LocalProbeOptions = null, string? ExecutionLane = null, JsonElement? LocalObservation = null);
internal sealed class CampaignResult
{
    public required CampaignCase Case { get; init; }
    public required string Sql { get; init; }
    public required Verdict Verdict { get; init; }
    public required string EffectiveClassification { get; init; }
    public required string[] Families { get; init; }
    public string Outcome { get; set; } = "CRASH";
    public string Stage { get; set; } = "clone";
    public List<string> Errors { get; init; } = [];
    public CampaignObservation? Observation { get; set; }
    public CampaignObservation? ExternalObservation { get; set; }
    public CampaignSession? Session { get; set; }
    public double Milliseconds { get; set; }
    public double ExecutionMilliseconds { get; set; }
    public string? ShrunkSql { get; set; }
    public int ShrinkTrials { get; set; }
    public double ShrinkMilliseconds { get; set; }
    public bool ReusedDatabaseSet { get; set; }
    public bool IsolationVerified { get; set; }
    public List<CampaignEffectEdge> EffectEdges { get; set; } = [];
    public bool ExpectationMismatch { get; set; }
    public bool Control => Case.ControlVerdict != null;
}

internal sealed class CampaignLease
{
    internal CampaignDatabaseSet? Set { get; set; }
    internal CampaignSnapshot? Pristine { get; set; }
}

internal static class CampaignRunner
{
    internal static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
    private static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
    public static int Run(Args options, AnalyzerHost analyzer)
    {
        var inputFiles = File.Exists(options.Campaign) ? new[] { Path.GetFullPath(options.Campaign) } : Directory.GetFiles(options.Campaign!, "*.jsonl", SearchOption.AllDirectories).Order(StringComparer.Ordinal).ToArray();
        var allCases = CampaignCaseLoader.Load(inputFiles, options);
        if (options.CampaignValidateOnly) return 0;
        if (options.CampaignOffline) return CampaignCaseLoader.RunOffline(allCases, options, analyzer);
        if (options.CampaignIsolated && options.Workers != 1) throw new InvalidOperationException("Privileged campaigns require --workers 1.");
        var configured = options.CampaignIsolated ? Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONNECTION") : null;
        if (options.CampaignIsolated && string.IsNullOrWhiteSpace(configured)) throw new InvalidOperationException("Container connection required.");
        var connectionString = configured ?? new SqlConnectionStringBuilder { DataSource = options.Server, IntegratedSecurity = true, TrustServerCertificate = true, Pooling = false }.ConnectionString;
        using var admin = CampaignSql.Open(connectionString);
        if (Convert.ToInt32(CampaignSql.Scalar(admin, "SELECT IS_SRVROLEMEMBER('sysadmin')")) != 1) throw new InvalidOperationException("Provisioning requires sysadmin.");
        var container = options.CampaignIsolated ? CampaignSql.VerifyContainer(admin) : null;
        if (Convert.ToInt32(CampaignSql.Scalar(admin, "DECLARE @result int; EXEC @result=sys.sp_getapplock @Resource=N'SqlSafetyGuard.ServerHarness.Campaign',@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0; SELECT @result;")) < 0)
            throw new InvalidOperationException("Another campaign holds the SQL instance lease.");
        EnsureResultsDatabase(admin, connectionString);
        CampaignDatabaseSet.Recover(connectionString);
        var scriptDirectory = Path.Combine(AppContext.BaseDirectory, "Campaign");
        var world = new CampaignWorld(connectionString, scriptDirectory);
        world.Provision();
        if (options.PrepareWorld) { Console.WriteLine("World templates and checked backups ready."); return 0; }
        if (container != null) ConfigureLoopback(admin, connectionString);
        var selected = allCases.Where(item => options.Filter == null || item.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase)).OrderBy(item => Hash(item.Seed + ":" + item.Id), StringComparer.Ordinal).ToArray();
        if (options.Limit > 0) selected = selected.Take(options.Limit).ToArray();
        if (selected.Length == 0) throw new InvalidDataException("No selected campaign cases.");
        var identity = new
        {
            SchemaVersion = 1, AnalyzerSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(analyzer.AssemblyPath))),
            HarnessSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(typeof(CampaignRunner).Assembly.Location))),
            InputSha256 = Hash(string.Join("\n", inputFiles.Select(File.ReadAllText))),
            WorldSha256 = CampaignWorld.ScriptHash(scriptDirectory),
            Server = container == null ? Convert.ToString(CampaignSql.Scalar(admin, "SELECT CONVERT(nvarchar(128),SERVERPROPERTY('ServerName'))")) : "verified-disposable-docker", admin.ServerVersion,
            ContainerImage = container == null ? null : ContainerImage(container), Timeout = options.Timeout, options.Filter, options.Limit, options.ShrinkAttempts, options.CampaignPrincipal
        };
        var campaignKey = Hash(JsonSerializer.Serialize(identity, Json));
        var directory = Directory.CreateDirectory(options.Out).FullName;
        var identityPath = Path.Combine(directory, "campaign-identity.json");
        var serializedIdentity = JsonSerializer.Serialize(new { CampaignKey = campaignKey, Identity = identity, Workers = options.Workers }, Json);
        if (File.Exists(identityPath))
        {
            using var previous = JsonDocument.Parse(File.ReadAllText(identityPath));
            if (previous.RootElement.GetProperty("CampaignKey").GetString() != campaignKey) throw new InvalidOperationException("Output belongs to different inputs/binaries/settings. Choose a new --out directory.");
        }
        File.WriteAllText(identityPath, serializedIdentity);
        EnsureResultsDatabase(admin, connectionString);
        using var resultsConnection = CampaignSql.Open(connectionString, "SgRun_Results");
        var results = CampaignJournal.Merge(Load(resultsConnection, campaignKey), CampaignJournal.Load(directory, campaignKey));
        var completedIds = results.Select(result => result.Case.Id).ToHashSet(StringComparer.Ordinal);
        var selectedIds = selected.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        if (!completedIds.IsSubsetOf(selectedIds)) throw new InvalidDataException("Receipt contains an unselected case.");
        var pending = selected.Where(item => !completedIds.Contains(item.Id)).ToArray();
        File.WriteAllText(Path.Combine(directory, "invocation-" + DateTimeOffset.UtcNow.ToString("yyyyMMddTHHmmssfffffff") + ".json"),
            JsonSerializer.Serialize(new { Container = container, admin.ServerVersion, StartedUtc = DateTimeOffset.UtcNow }, Json));
        var resultLock = new object();
        using var stop = new CancellationTokenSource();
        if (options.BudgetMinutes > 0) stop.CancelAfter(TimeSpan.FromMinutes(options.BudgetMinutes));
        ConsoleCancelEventHandler cancel = (_, eventArgs) => { eventArgs.Cancel = true; stop.Cancel(); };
        Console.CancelKeyPress += cancel;
        using var leases = new ThreadLocal<CampaignLease>(() => new CampaignLease(), trackAllValues: true);
        var started = DateTimeOffset.UtcNow;
        WriteArtifacts(directory, results, selected.Length, options.Workers, started, campaignKey);
        try
        {
            Parallel.ForEach(pending, new ParallelOptions { MaxDegreeOfParallelism = options.Workers }, item =>
            {
                if (stop.IsCancellationRequested) return;
                var result = Execute(options, analyzer, world, connectionString, container, item, stop.Token, options.CampaignIsolated ? null : leases.Value);
                if (stop.IsCancellationRequested && result.Outcome is "TIMEOUT" or "CRASH") return;
                lock (resultLock)
                {
                    CampaignJournal.Save(directory, campaignKey, result);
                    results.Add(result);
                    try { Save(resultsConnection, campaignKey, result); }
                    catch (SqlException exception)
                    {
                        result.Errors.Add("SQL result persistence failed; external receipt retained: " + exception.Message);
                        result.Outcome = "CRASH";
                        CampaignJournal.Save(directory, campaignKey, result);
                        stop.Cancel();
                    }
                }
                if ((result.Outcome is "FALSE GREEN" or "WRONG TARGET") && !stop.IsCancellationRequested)
                    Shrink(options, analyzer, world, connectionString, container, result, stop.Token);
                lock (resultLock)
                {
                    CampaignJournal.Save(directory, campaignKey, result);
                    if (!stop.IsCancellationRequested)
                    {
                        try { Save(resultsConnection, campaignKey, result, update: true); }
                        catch (SqlException exception)
                        {
                            result.Errors.Add("SQL result update failed; external receipt retained: " + exception.Message);
                            result.Outcome = "CRASH";
                            CampaignJournal.Save(directory, campaignKey, result);
                            stop.Cancel();
                        }
                    }
                    if (results.Count % 100 == 0) WriteArtifacts(directory, results, selected.Length, options.Workers, started, campaignKey);
                    Console.WriteLine($"[{results.Count}/{selected.Length}] {item.Id}: {result.Outcome}; changed={result.Observation?.Changed}; {result.Milliseconds:F0}ms; {string.Join("; ", result.Errors)}");
                }
            });
        }
        finally
        {
            var cleanupErrors = new List<Exception>();
            foreach (var lease in leases.Values)
            {
                try { lease.Set?.Dispose(); }
                catch (Exception exception) { cleanupErrors.Add(exception); }
            }
            Console.CancelKeyPress -= cancel;
            WriteArtifacts(directory, results, selected.Length, options.Workers, started, campaignKey);
            if (cleanupErrors.Count > 0) throw new AggregateException("Worker database cleanup failed.", cleanupErrors);
        }
        Console.WriteLine("Campaign report: " + Path.Combine(directory, "report.md"));
        return results.Count != selected.Length || results.Any(result => !result.Control && (result.Outcome is "FALSE GREEN" or "WRONG TARGET" or "CRASH" or "TIMEOUT" or "NOT EXECUTABLE" || result.ExpectationMismatch || result.Errors.Count > 0)) ? 1 : 0;
    }

    private static string ContainerImage(string container)
    {
        using var process = Process.Start(new ProcessStartInfo("docker")
        {
            ArgumentList = { "inspect", "--format", "{{.Image}}", container },
            RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Cannot inspect verified container image.");
        var image = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        if (process.ExitCode != 0 || !System.Text.RegularExpressions.Regex.IsMatch(image, "^sha256:[a-f0-9]{64}$"))
            throw new InvalidOperationException("Cannot establish immutable container image identity.");
        return image;
    }

    private static CampaignResult Execute(Args options, AnalyzerHost analyzer, CampaignWorld world, string connectionString, string? container, CampaignCase item, CancellationToken stop, CampaignLease? lease = null)
    {
        var watch = Stopwatch.StartNew();
        var verdict = analyzer.Analyze(item.Sql);
        var result = new CampaignResult { Case = item, Sql = item.Sql, Verdict = verdict, EffectiveClassification = item.ControlVerdict ?? verdict.Overall, Families = Families(item.Sql) };
        CampaignDatabaseSet? set = null;
        try
        {
            if (lease?.Set != null && (!string.IsNullOrWhiteSpace(item.Setup) || item.LocalProbeOptions?.DefaultSchema != null))
            {
                lease.Set.Dispose();
                lease.Set = null;
            }
            bool reused = lease?.Set != null;
            set = lease?.Set ?? world.Clone();
            if (lease != null) lease.Set = null;
            var sql = CampaignWorld.Expand(item.Sql, set.Databases, item.Seed);
            var principal = options.CampaignPrincipal ?? item.Principal;
            var context = item.LocalProbeOptions?.Expand(set.Databases, item.Seed) ?? new CampaignAnalyzerOptions();
            var databaseRole = context.CurrentDatabase == null ? item.Database : set.Databases.SingleOrDefault(pair => string.Equals(pair.Value, context.CurrentDatabase, StringComparison.OrdinalIgnoreCase)).Key
                ?? throw new InvalidDataException("Connection context must name one of this case's four owned databases.");
            var defaultSchema = principal == "admin" || item.Privileged ? "dbo" : principal == "low_user" ? "sales" : "app";
            context = context with { CurrentDatabase = set.Databases[databaseRole], DefaultSchema = context.DefaultSchema ?? defaultSchema };
            if ((principal == "admin" || item.Privileged) && context.DefaultSchema != "dbo") throw new InvalidDataException("Sysadmin execution resolves unqualified names as dbo; cannot emulate a different default schema.");
            verdict = analyzer.Analyze(sql, context);
            result = new CampaignResult { Case = item, Sql = sql, Verdict = verdict, EffectiveClassification = item.ControlVerdict ?? verdict.Overall, Families = Families(sql) };
            result.Stage = "setup";
            using (var setup = CampaignSql.Open(connectionString, set.Databases[item.Database]))
            {
                CampaignSql.Execute(setup, CampaignWorld.Expand(item.Setup, set.Databases, item.Seed), options.Timeout, stop);
                if (Convert.ToInt32(CampaignSql.Scalar(setup, "SELECT @@TRANCOUNT")) != 0) throw new InvalidDataException("Setup left an open transaction.");
            }
            if (context.DefaultSchema != defaultSchema)
                foreach (var database in set.Databases.Values)
                {
                    using var mapping = CampaignSql.Open(connectionString, database);
                    CampaignSql.Execute(mapping, $"ALTER USER {CampaignSql.Quote(principal)} WITH DEFAULT_SCHEMA={CampaignSql.Quote(context.DefaultSchema!)};");
                }
            var effects = verdict.Overall is "RealWrite" or "AllowListed" ? CampaignAttribution.Capture(connectionString, set.Databases, options.Timeout) : [];
            effects.AddRange(CampaignAttribution.Transfers(sql, context.CurrentDatabase!, context.DefaultSchema!));
            result.EffectEdges = effects;
            var before = CampaignOracle.Capture(connectionString, set.Databases, options.Timeout, container);
            if (container != null) effects.AddRange(CampaignAttribution.VerifiedLoopback(set.Databases, before.Objects.Keys));
            result.ReusedDatabaseSet = reused;
            if (reused && lease?.Pristine != null && !CampaignOracle.Equivalent(lease.Pristine, before) && !CampaignOracle.OnlyExternalDatabaseChange(lease.Pristine, before))
                throw new InvalidDataException("Isolation control: reused database set differs from its pristine snapshot.");
            result.IsolationVerified = reused;
            CampaignSession baseline;
            using (var connection = set.OpenCase(databaseRole, principal, item.Privileged))
            {
                baseline = CampaignOracle.Session(connection);
                result.Stage = "execute";
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(stop);
                deadline.CancelAfter(TimeSpan.FromSeconds(options.Timeout));
                var execution = Stopwatch.StartNew();
                try { CampaignSql.Execute(connection, sql, options.Timeout, deadline.Token, context.SqlCmdMode ?? SqlCmdLite.LooksLikeSqlCmd(sql)); }
                catch (Exception exception)
                {
                    result.Outcome = deadline.IsCancellationRequested || exception is OperationCanceledException || exception is SqlException { Number: -2 } ? "TIMEOUT" : "NOT EXECUTABLE";
                    result.Errors.Add(exception.GetType().Name + ": " + exception.Message);
                }
                result.ExecutionMilliseconds = execution.Elapsed.TotalMilliseconds;
                try { result.Session = CampaignOracle.Session(connection); }
                catch (Exception exception) { result.Errors.Add("Session observation: " + exception.Message); }
            }
            result.Stage = "observe";
            var after = CampaignOracle.Capture(connectionString, set.Databases, options.Timeout, container);
            var externalOnly = container == null && CampaignOracle.OnlyExternalDatabaseChange(before, after);
            if (externalOnly)
            {
                result.ExternalObservation = CampaignOracle.Compare(before, after, baseline, result.Session);
                before = before with { Server = after.Server };
            }
            result.Observation = CampaignOracle.Compare(before, after, baseline, result.Session);
            result.ExpectationMismatch = item.ExpectedClassification != null && item.ExpectedClassification != verdict.Overall;
            var settingsMismatches = new List<string>();
            CampaignCaseLoader.CheckObservation(item.LocalObservation, "configured", verdict, settingsMismatches);
            CampaignCaseLoader.CheckObservation(item.LocalObservation, "default", analyzer.Analyze(sql), settingsMismatches);
            result.ExpectationMismatch |= settingsMismatches.Count != 0;
            result.Errors.AddRange(settingsMismatches);
            if (verdict.Error != null) { result.Outcome = "CRASH"; result.Errors.Add(verdict.Error); }
            else if (result.Session == null && result.Outcome == "CRASH") result.Outcome = "OBSERVATION GAP";
            else if (result.Errors.Count != 0 && !result.Observation.Changed) { }
            else if (externalOnly && !result.Observation.Changed) result.Outcome = "EXTERNAL CHANGE";
            else if (result.Observation.Changed && result.EffectiveClassification == "AllowListed")
            {
                var modified = result.Observation.ModifiedObjects.Where(name => !name.Contains('<')).ToArray();
                var direct = modified.Length > 0 && modified.All(name => verdict.Targets.Any(target => CampaignAttribution.Matches(target, name, context.CurrentDatabase!, context.DefaultSchema!, before.Objects.Keys)));
                var scopeChanges = result.Observation.ModifiedObjects.Any(name => name.Contains('<'));
                if (result.Errors.Count == 0)
                    result.Outcome = direct && !scopeChanges && result.Observation.SessionLeftovers.Length == 0 ? "PASS" : "POLICY EFFECT";
            }
            else if (result.Observation.Changed && verdict.Findings.Any(finding => finding.Contains("trusted", StringComparison.OrdinalIgnoreCase)) && context.TrustedProcedures?.Length > 0) result.Outcome = "POLICY EFFECT";
            else if (result.Observation.Changed && new[] { "ReadOnly", "TempOnly" }.Contains(result.EffectiveClassification)) result.Outcome = "FALSE GREEN";
            else if (result.Errors.Count == 0)
            {
                var modified = result.Observation.ModifiedObjects.Where(name => !name.Contains('<')).ToArray();
                result.Outcome = result.Observation.Changed && modified.Length > 0 && verdict.Overall == "RealWrite"
                    ? CampaignAttribution.Classify(verdict.Targets, modified, context.CurrentDatabase!, effects, context.DefaultSchema!, before.Objects.Keys)
                    : !result.Observation.Changed && !verdict.IsGreen ? "NOISE" : "PASS";
            }
            result.Stage = "complete";
            if (lease != null && !result.Observation.Changed && result.Errors.Count == 0 &&
                CampaignOracle.Equivalent(before, after) &&
                result.Observation.SessionLeftovers.Length == 0 && string.IsNullOrWhiteSpace(item.Setup) && context.DefaultSchema == defaultSchema)
            {
                lease.Set = set;
                lease.Pristine = before;
                set = null;
            }
        }
        catch (Exception exception) { result.Errors.Add(result.Stage + ": " + exception.GetType().Name + ": " + exception.Message); result.Outcome = "CRASH"; }
        finally
        {
            try { set?.Dispose(); }
            catch (Exception exception) { result.Errors.Add(exception.Message); result.Outcome = "CRASH"; result.Stage = "cleanup"; }
            result.Milliseconds = watch.Elapsed.TotalMilliseconds;
        }
        return result;
    }
    private static string[] Families(string sql)
    {
        try { return CoverageCorpus.StatementTypes(sql).Order().ToArray(); }
        catch { return ["parse-error"]; }
    }
    private static IEnumerable<string> Reductions(string sql)
    {
        var parser = new TSql180Parser(true);
        var fragment = parser.Parse(new StringReader(sql), out var errors);
        if (errors.Count != 0 || fragment is not TSqlScript script) yield break;
        var statements = script.Batches.SelectMany(batch => batch.Statements).ToArray();
        foreach (var statement in statements)
            if (statements.Length > 1) yield return sql.Remove(statement.StartOffset, statement.FragmentLength).Trim();
        var visitor = new ClauseVisitor();
        fragment.Accept(visitor);
        foreach (var clause in visitor.Clauses) yield return sql.Remove(clause.StartOffset, clause.FragmentLength).Trim();
    }
    private sealed class ClauseVisitor : TSqlFragmentVisitor
    {
        public List<TSqlFragment> Clauses { get; } = [];
        public override void ExplicitVisit(WhereClause node) { Clauses.Add(node); }
        public override void ExplicitVisit(OrderByClause node) { Clauses.Add(node); }
        public override void ExplicitVisit(TopRowFilter node) { Clauses.Add(node); }
    }
    private static void Shrink(Args options, AnalyzerHost analyzer, CampaignWorld world, string connectionString, string? container, CampaignResult original, CancellationToken stop)
    {
        var watch = Stopwatch.StartNew();
        var smallest = original.Case.Sql;
        var progress = true;
        while (progress && original.ShrinkTrials < options.ShrinkAttempts && !stop.IsCancellationRequested)
        {
            progress = false;
            foreach (var candidate in Reductions(smallest).Where(candidate => candidate.Length > 0 && candidate.Length < smallest.Length).Distinct().OrderBy(candidate => candidate.Length))
            {
                if (original.ShrinkTrials >= options.ShrinkAttempts || stop.IsCancellationRequested) break;
                original.ShrinkTrials++;
                var reduced = Execute(options, analyzer, world, connectionString, container, original.Case with { Sql = candidate }, stop);
                if (reduced.Outcome != original.Outcome || reduced.Errors.Count > 0) continue;
                smallest = candidate;
                progress = true;
                break;
            }
        }
        original.ShrunkSql = smallest;
        original.ShrinkMilliseconds = watch.Elapsed.TotalMilliseconds;
    }
    private static void EnsureResultsDatabase(SqlConnection admin, string connectionString)
    {
        if (CampaignSql.Scalar(admin, "SELECT DB_ID(N'SgRun_Results')") is DBNull)
        {
            CampaignSql.Execute(admin, "CREATE DATABASE SgRun_Results;");
            CampaignSql.Execute(admin, "USE SgRun_Results; EXEC sys.sp_addextendedproperty @name=N'SgCampaignResults',@value=N'1';");
            admin.ChangeDatabase("master");
        }
        using var results = CampaignSql.Open(connectionString, "SgRun_Results");
        if (Convert.ToString(CampaignSql.Scalar(results, "SELECT value FROM sys.extended_properties WHERE class=0 AND name=N'SgCampaignResults'")) != "1") throw new InvalidOperationException("Refusing unowned SgRun_Results database.");
        CampaignSql.Execute(results, "IF OBJECT_ID(N'dbo.CampaignResults') IS NULL CREATE TABLE dbo.CampaignResults(CampaignKey char(64) NOT NULL,CaseId nvarchar(400) NOT NULL,CompletedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),Outcome varchar(32) NOT NULL,Payload nvarchar(max) NOT NULL,CONSTRAINT PK_CampaignResults PRIMARY KEY(CampaignKey,CaseId));");
        CampaignSql.Execute(results, "IF OBJECT_ID(N'dbo.CampaignOwnedSets') IS NULL CREATE TABLE dbo.CampaignOwnedSets(IdentityKey char(32) NOT NULL PRIMARY KEY,CreatedUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME());");
    }
    private static void ConfigureLoopback(SqlConnection admin, string connectionString)
    {
        CampaignSql.Execute(admin, "IF NOT EXISTS(SELECT 1 FROM sys.servers WHERE name=N'SgLoop') BEGIN EXEC sys.sp_addlinkedserver @server=N'SgLoop',@srvproduct=N'',@provider=N'MSOLEDBSQL',@datasrc=N'127.0.0.1,1433',@provstr=N'encrypt=optional;TrustServerCertificate=yes'; EXEC sys.sp_serveroption N'SgLoop',N'rpc out',N'true'; END");
        using var command = new SqlCommand("EXEC sys.sp_addlinkedsrvlogin @rmtsrvname=N'SgLoop',@useself=N'false',@locallogin=NULL,@rmtuser=N'sa',@rmtpassword=@password;", admin);
        command.Parameters.AddWithValue("@password", new SqlConnectionStringBuilder(connectionString).Password);
        command.ExecuteNonQuery();
    }
    private static List<CampaignResult> Load(SqlConnection connection, string campaignKey)
    {
        using var command = new SqlCommand("SELECT Payload FROM dbo.CampaignResults WHERE CampaignKey=@key ORDER BY CaseId", connection);
        command.Parameters.AddWithValue("@key", campaignKey);
        using var reader = command.ExecuteReader();
        var results = new List<CampaignResult>();
        while (reader.Read()) results.Add(JsonSerializer.Deserialize<CampaignResult>(reader.GetString(0), Json)!);
        return results;
    }
    private static void Save(SqlConnection connection, string key, CampaignResult result, bool update = false)
    {
        using var command = new SqlCommand(update ? "UPDATE dbo.CampaignResults SET Outcome=@outcome,Payload=@payload WHERE CampaignKey=@key AND CaseId=@id" : "INSERT dbo.CampaignResults(CampaignKey,CaseId,Outcome,Payload) VALUES(@key,@id,@outcome,@payload)", connection);
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@id", result.Case.Id);
        command.Parameters.AddWithValue("@outcome", result.Outcome);
        command.Parameters.AddWithValue("@payload", JsonSerializer.Serialize(result, Json));
        command.ExecuteNonQuery();
    }
    private static void WriteArtifacts(string directory, List<CampaignResult> results, int planned, int workers, DateTimeOffset started, string campaignKey)
    {
        var output = Path.Combine(directory, "results.jsonl");
        File.WriteAllLines(output + ".tmp", results.Select(result => JsonSerializer.Serialize(result, Json)));
        File.Move(output + ".tmp", output, true);
        var report = new StringBuilder("# SQL safety campaign\n\n");
        report.AppendLine($"- Campaign: {campaignKey}\n- Generated UTC: {DateTimeOffset.UtcNow:O}\n- This invocation started UTC: {started:O}\n- Completed: {results.Count}/{planned}; workers: {workers}; complete: {results.Count == planned}\n- Controls: {results.Count(result => result.Control)} (excluded from real false-green counts)\n- Real false greens: {results.Count(result => !result.Control && result.Outcome == "FALSE GREEN")}\n");
        report.AppendLine($"- Reused pristine sets: {results.Count(result => result.ReusedDatabaseSet)}; successful before-case isolation checks: {results.Count(result => result.IsolationVerified)}\n");
        report.AppendLine("## Outcomes\n\n| Outcome | Real | Controls |\n|---|---:|---:|");
        foreach (var outcome in new[] { "PASS", "FALSE GREEN", "WRONG TARGET", "INDIRECT EFFECT", "UNRESOLVED TARGET", "EXTERNAL CHANGE", "POLICY EFFECT", "NOISE", "OBSERVATION GAP", "CRASH", "TIMEOUT", "NOT EXECUTABLE" }) report.AppendLine($"| {outcome} | {results.Count(result => !result.Control && result.Outcome == outcome)} | {results.Count(result => result.Control && result.Outcome == outcome)} |");
        report.AppendLine("\nNOISE means a conservative verdict with no observed mutation; it is not automatically an analyzer defect. WRONG TARGET means at least one changed object is neither named by a red finding nor reachable through a pre-execution synonym, single-base view, trigger-write, temporal or cascading-FK edge. INDIRECT EFFECT records those reachable secondary changes. UNRESOLVED TARGET requires review (for example a Broker dialog variable); it is not a proven wrong static target. Catalog-only changes remain scope-level evidence. PASS is representative evidence, not a complete rule proof.\n");
        foreach (var grouping in new[] { (Title: "Rule ids", Rows: results.SelectMany(result => (result.Case.RuleIds ?? ["unassigned"]).Select(rule => (Key: rule, Result: result)))), (Title: "Statement families", Rows: results.SelectMany(result => result.Families.Select(family => (Key: family, Result: result)))) })
        {
            report.AppendLine("## " + grouping.Title + "\n");
            foreach (var group in grouping.Rows.GroupBy(row => row.Key).OrderBy(group => group.Key)) report.AppendLine("- " + group.Key + ": " + string.Join(", ", group.GroupBy(row => row.Result.Outcome).Select(outcome => outcome.Key + "=" + outcome.Count())));
        }
        report.AppendLine("\n## Cases and timing\n");
        var timings = results.Select(result => result.Milliseconds).Order().ToArray();
        if (timings.Length > 0) report.AppendLine($"\nCase elapsed ms: min={timings[0]:F1}, median={timings[timings.Length / 2]:F1}, p95={timings[Math.Min(timings.Length-1,(int)Math.Ceiling(timings.Length*0.95)-1)]:F1}, max={timings[^1]:F1}; shrink trials={results.Sum(result => result.ShrinkTrials)}, shrink worker ms={results.Sum(result => result.ShrinkMilliseconds):F1}.\n");
        report.AppendLine("| Id | Outcome | Case ms | SQL ms | Analyze ms | Modified objects |\n|---|---|---:|---:|---:|---|");
        foreach (var result in results.OrderBy(result => result.Case.Id)) report.AppendLine($"| {result.Case.Id} | {result.Outcome}{(result.Control ? " (CONTROL)" : "")} | {result.Milliseconds:F1} | {result.ExecutionMilliseconds:F1} | {result.Verdict.Milliseconds:F1} | {string.Join(", ", result.Observation?.ModifiedObjects ?? []).Replace("|", " ")} |");
        foreach (var result in results.Where(result => result.ShrunkSql != null).OrderBy(result => result.ShrunkSql!.Length).Take(20))
        {
            var filename = "repro-" + Hash(result.Case.Id)[..12] + ".sql";
            File.WriteAllText(Path.Combine(directory, filename), result.ShrunkSql);
            report.AppendLine($"\n## Repro {result.Case.Id} ({result.ShrinkTrials} trials; control={result.Control})\n\nTemplates: use original JSON setup/context with this SQL.\n\n~~~sql\n{result.ShrunkSql}\n~~~\n");
        }
        report.AppendLine("\n## Errors, leftovers, expectations\n");
        foreach (var result in results)
        {
            foreach (var error in result.Errors) report.AppendLine($"- {result.Case.Id} [{result.Stage}]: {error.Replace('\n',' ')}");
            foreach (var leftover in result.Observation?.SessionLeftovers ?? []) report.AppendLine($"- {result.Case.Id}: {leftover}");
            if (result.ExpectationMismatch) report.AppendLine($"- {result.Case.Id}: expected {result.Case.ExpectedClassification}, got {result.Verdict.Overall}");
        }
        File.WriteAllText(Path.Combine(directory, "report.md.tmp"), report.ToString());
        File.Move(Path.Combine(directory, "report.md.tmp"), Path.Combine(directory, "report.md"), true);
    }
}
