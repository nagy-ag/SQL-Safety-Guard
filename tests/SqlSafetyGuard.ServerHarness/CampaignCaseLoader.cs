using System.Text.Json;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record CampaignAnalyzerOptions(string[]? AllowListPatterns = null, bool TempdbIsTemp = false,
    int MaxDynamicSqlDepth = 3, bool? SqlCmdMode = null, string? CurrentDatabase = null,
    string? DefaultSchema = null, string[]? TrustedProcedures = null, string? EnvironmentRules = null,
    string? Server = null, string Mode = "YellowRed")
{
    public CampaignAnalyzerOptions Expand(IReadOnlyDictionary<string, string> databases, int seed)
    {
        string? ExpandValue(string? value) => value == null ? null : CampaignWorld.Expand(value, databases, seed);
        return this with
        {
            CurrentDatabase = ExpandValue(CurrentDatabase), DefaultSchema = ExpandValue(DefaultSchema),
            Server = ExpandValue(Server), EnvironmentRules = ExpandValue(EnvironmentRules),
            AllowListPatterns = AllowListPatterns?.Select(value => ExpandValue(value)!).ToArray(),
            TrustedProcedures = TrustedProcedures?.Select(value => ExpandValue(value)!).ToArray()
        };
    }
}

internal static class CampaignCaseLoader
{
    public static CampaignCase[] Load(IEnumerable<string> files, Args options)
    {
        var cases = new List<CampaignCase>();
        var rejected = new List<object>();
        var identifiers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in files)
        {
            var lineNumber = 0;
            foreach (var line in File.ReadLines(file))
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var item = JsonSerializer.Deserialize<CampaignCase>(line, CampaignRunner.Json) ?? throw new InvalidDataException("Null case.");
                    if (string.IsNullOrWhiteSpace(item.Id) || item.Id.Length > 400 || item.Sql == null) throw new InvalidDataException("Nonempty id (max 400) and non-null SQL required; empty SQL is a valid no-op.");
                    if (!CampaignWorld.Roles.Contains(item.Database) || !new[] { "runner", "app_user", "low_user", "admin" }.Contains(item.Principal)) throw new InvalidDataException("Unknown database role or principal.");
                    if (item.ControlVerdict != null && item.ControlVerdict != "ReadOnly") throw new InvalidDataException("Only ReadOnly negative controls are supported.");
                    if (!options.CampaignValidateOnly && !options.CampaignOffline && (item.ExecutionLane == "offline" || ((item.Privileged || item.ExecutionLane == "isolated") && !options.CampaignIsolated))) throw new InvalidDataException("Case does not belong to this execution lane.");
                    if (item.LocalProbeOptions is { } settings && (!new[] { "Off", "Red", "YellowRed" }.Contains(settings.Mode) || settings.MaxDynamicSqlDepth < 0)) throw new InvalidDataException("Invalid analyzer option bounds.");
                    if (!identifiers.Add(item.Id)) throw new InvalidDataException("Duplicate case id: " + item.Id);
                    cases.Add(item);
                }
                catch (Exception exception) when (exception is JsonException or InvalidDataException or ArgumentException)
                {
                    rejected.Add(new { File = Path.GetFullPath(file), Line = lineNumber, Error = exception.Message, Raw = line });
                }
            }
        }
        Directory.CreateDirectory(options.Out);
        File.WriteAllLines(Path.Combine(options.Out, "rejected.jsonl"), rejected.Select(item => JsonSerializer.Serialize(item, CampaignRunner.Json)));
        File.WriteAllText(Path.Combine(options.Out, "validation.json"), JsonSerializer.Serialize(new { Accepted = cases.Count, Rejected = rejected.Count }, CampaignRunner.Json));
        Console.WriteLine($"Validated {cases.Count} cases; rejected {rejected.Count} (see rejected.jsonl).");
        if (cases.Count == 0) throw new InvalidDataException("No valid campaign cases.");
        return cases.ToArray();
    }

    public static int RunOffline(CampaignCase[] cases, Args options, AnalyzerHost analyzer)
    {
        var databases = CampaignWorld.Roles.ToDictionary(role => role, role => "SgPlay_" + role);
        var selected = cases.Where(item => options.Filter == null || item.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase));
        if (options.Limit > 0) selected = selected.Take(options.Limit);
        var results = selected.Select(item =>
        {
            var sql = CampaignWorld.Expand(item.Sql, databases, item.Seed);
            var baseline = analyzer.Analyze(sql);
            var configured = analyzer.Analyze(sql, item.LocalProbeOptions?.Expand(databases, item.Seed));
            var mismatches = new List<string>();
            if (item.ExpectedClassification != null && item.ExpectedClassification != configured.Overall)
                mismatches.Add($"Expected classification {item.ExpectedClassification}, actual {configured.Overall}");
            CheckObservation(item.LocalObservation, "default", baseline, mismatches);
            CheckObservation(item.LocalObservation, "configured", configured, mismatches);
            return new { item.Id, Lane = "offline-analyzer-only", Sql = sql, Default = baseline, Configured = configured, Mismatches = mismatches };
        }).ToArray();
        File.WriteAllLines(Path.Combine(options.Out, "offline-results.jsonl"), results.Select(result => JsonSerializer.Serialize(result, CampaignRunner.Json)));
        Console.WriteLine($"Offline checks: {results.Length}; no SQL executed.");
        return results.Any(result => result.Default.Error != null || result.Configured.Error != null || result.Mismatches.Count != 0) ? 1 : 0;
    }

    internal static void CheckObservation(JsonElement? observation, string name, Verdict actual, List<string> mismatches)
    {
        if (observation is not { } root || !root.TryGetProperty(name, out var expected)) return;
        if (expected.TryGetProperty("verdict", out var verdict) && verdict.GetString() != actual.Overall)
            mismatches.Add($"{name}.verdict: expected {verdict}, actual {actual.Overall}");
        if (expected.TryGetProperty("isGreen", out var green) && green.GetBoolean() != actual.IsGreen)
            mismatches.Add($"{name}.isGreen: expected {green}, actual {actual.IsGreen}");
        if (!expected.TryGetProperty("environment", out var environment)) return;
        if (environment.TryGetProperty("mode", out var mode) && mode.GetString() != actual.Mode)
            mismatches.Add($"{name}.mode: expected {mode}, actual {actual.Mode}");
        if (environment.TryGetProperty("shouldWarn", out var warning) && warning.GetBoolean() != actual.ShouldWarn)
            mismatches.Add($"{name}.shouldWarn: expected {warning}, actual {actual.ShouldWarn}");
        if (environment.TryGetProperty("typed", out var typed) && typed.GetBoolean() != actual.NeedsTypedConfirmation)
            mismatches.Add($"{name}.typed: expected {typed}, actual {actual.NeedsTypedConfirmation}");
    }
}
