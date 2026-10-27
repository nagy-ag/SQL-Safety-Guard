using System.Collections;
using System.Reflection;

namespace SqlSafetyGuard.ServerHarness;

/// <summary>One analyzer verdict, read by reflection so any branch's build can be loaded.</summary>
internal sealed record Verdict(string Overall, bool IsGreen, List<string> Findings, double Milliseconds, string? Error);

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

    public Verdict Analyze(string sql)
    {
        var parameters = _analyze.GetParameters();
        var args = new object?[parameters.Length];
        args[0] = sql;
        for (int i = 1; i < parameters.Length; i++)
            args[i] = parameters[i].HasDefaultValue ? parameters[i].DefaultValue : DefaultOf(parameters[i].ParameterType);

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
        if (result.GetType().GetProperty("Findings")!.GetValue(result) is IEnumerable list)
        {
            foreach (var f in list)
            {
                string P(string name) => f.GetType().GetProperty(name)?.GetValue(f)?.ToString() ?? "";
                findings.Add($"L{P("Line")} {P("Verdict")} {P("Action")} {P("Target")} {P("Note")}".Trim());
            }
        }

        return new Verdict(overall, green, findings, ms, null);
    }

    private static object? DefaultOf(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
}
