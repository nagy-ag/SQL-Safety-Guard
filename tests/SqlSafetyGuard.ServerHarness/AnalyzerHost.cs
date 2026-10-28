using System.Collections;
using System.Reflection;

namespace SqlSafetyGuard.ServerHarness;

/// <summary>One analyzer verdict, read by reflection so any branch's build can be loaded.</summary>
internal sealed record Verdict(string Overall, bool IsGreen, List<string> Findings, double Milliseconds, string? Error)
{
    public List<string> Targets { get; init; } = [];
    public CampaignAnalyzerOptions? Options { get; init; }
    public string? Mode { get; init; }
    public bool? ShouldWarn { get; init; }
    public bool? NeedsTypedConfirmation { get; init; }
}

/// <summary>Loads SqlSafetyGuard.Analyzer.dll from a path and calls SqlSafetyAnalyzer.Analyze(sql) with default options.</summary>
internal sealed class AnalyzerHost
{
    private readonly MethodInfo _analyze;

    public AnalyzerHost(string dllPath)
    {
        var assembly = Assembly.LoadFrom(Path.GetFullPath(dllPath));
        var type = assembly.GetType("SqlSafetyGuard.Analyzer.SqlSafetyAnalyzer", throwOnError: true)!;
        _analyze = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "Analyze" && m.GetParameters().FirstOrDefault()?.ParameterType == typeof(string))
            .OrderBy(m => m.GetParameters().Length)
            .First();
        AssemblyPath = assembly.Location;
    }

    public string AssemblyPath { get; }

    public Verdict Analyze(string sql, CampaignAnalyzerOptions? options = null)
    {
        try { return AnalyzeCore(sql, options); }
        catch (Exception exception)
        {
            return new Verdict("CRASH", false, [], 0, (exception.InnerException ?? exception).ToString()) { Options = options };
        }
    }

    private Verdict AnalyzeCore(string sql, CampaignAnalyzerOptions? options)
    {
        var parameters = _analyze.GetParameters();
        var args = new object?[parameters.Length];
        args[0] = sql;
        for (int i = 1; i < parameters.Length; i++)
            args[i] = parameters[i].HasDefaultValue ? parameters[i].DefaultValue : DefaultOf(parameters[i].ParameterType);

        var settingsType = _analyze.DeclaringType!.Assembly.GetType("SqlSafetyGuard.Analyzer.GuardSettings", true)!;
        var modeType = _analyze.DeclaringType!.Assembly.GetType("SqlSafetyGuard.Analyzer.ExecutionMode", true)!;
        object? settings = Activator.CreateInstance(settingsType, Enum.Parse(modeType, "YellowRed"), "Global", false, null, null, null);
        if (options != null)
        {
            var assembly = _analyze.DeclaringType!.Assembly;
            settings = Activator.CreateInstance(settingsType, Enum.Parse(modeType, options.Mode), "Global", false, options.AllowListPatterns, options.TrustedProcedures, null);
            string[] warnings = [];
            if (options.EnvironmentRules != null)
            {
                var rulesType = assembly.GetType("SqlSafetyGuard.Analyzer.EnvironmentRules", true)!;
                var rules = Activator.CreateInstance(rulesType, options.EnvironmentRules)!;
                settings = rulesType.GetMethod("Resolve")!.Invoke(rules, [options.Server, options.CurrentDatabase, settings]);
                warnings = ((IEnumerable<string>)rulesType.GetProperty("Warnings")!.GetValue(rules)!).ToArray();
            }
            var allowList = settingsType.GetProperty("AllowList")!.GetValue(settings);
            var trusted = settingsType.GetProperty("TrustedProcedures")!.GetValue(settings);
            var optionsType = assembly.GetType("SqlSafetyGuard.Analyzer.AnalyzerOptions", true)!;
            var configured = Activator.CreateInstance(optionsType, allowList, options.TempdbIsTemp, options.MaxDynamicSqlDepth, options.SqlCmdMode, options.CurrentDatabase, options.DefaultSchema, trusted, warnings);
            for (int index = 1; index < parameters.Length; index++)
                if (parameters[index].ParameterType == optionsType) args[index] = configured;
        }
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        object result;
        try
        {
            result = _analyze.Invoke(null, args)!;
        }
        catch (TargetInvocationException ex)
        {
            return new Verdict("CRASH", false, new List<string>(), stopwatch.Elapsed.TotalMilliseconds, ex.InnerException?.ToString());
        }

        double ms = stopwatch.Elapsed.TotalMilliseconds;
        var overall = result.GetType().GetProperty("Overall")!.GetValue(result)!.ToString()!;
        bool green = (bool)result.GetType().GetProperty("IsGreen")!.GetValue(result)!;
        var findings = new List<string>();
        var targets = new List<string>();
        if (result.GetType().GetProperty("Findings")!.GetValue(result) is IEnumerable list)
        {
            foreach (var f in list)
            {
                string P(string name) => f.GetType().GetProperty(name)?.GetValue(f)?.ToString() ?? "";
                findings.Add($"L{P("Line")} {P("Verdict")} {P("Action")} {P("Target")} {P("Note")}".Trim());
                string target = P("ResolvedTarget");
                if (target.Length == 0) target = P("Target");
                if (P("Verdict") is "RealWrite" or "AllowListed" && target.Length > 0) targets.Add(target);
            }
        }

        return new Verdict(overall, green, findings, ms, null)
        {
            Targets = targets, Options = options,
            Mode = settings?.GetType().GetProperty("Mode")!.GetValue(settings)?.ToString(),
            ShouldWarn = settings == null ? null : (bool)settings.GetType().GetMethod("ShouldWarn")!.Invoke(settings, [result])!,
            NeedsTypedConfirmation = settings == null ? null : (bool)settings.GetType().GetMethod("NeedsTypedConfirmation")!.Invoke(settings, [result])!
        };
    }

    private static object? DefaultOf(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
}
