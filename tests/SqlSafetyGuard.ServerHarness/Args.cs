using System.Text;
using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

internal sealed class Args
{
    public string Analyzer { get; set; } = "";
    public List<string> Tests { get; } = new();
    public List<string> Samples { get; } = new();
    public List<string> Sql { get; } = new();
    public string Server { get; set; } = @".\SQLEXPRESS";
    public string Out { get; set; } = "harness-out";
    public int Limit { get; set; }
    public string? Filter { get; set; }
    public int Timeout { get; set; } = 10;
    public bool Keep { get; set; }
    public bool LegacyDiscovery { get; set; }
    public string? TestIndex { get; set; }
    public string? Coverage { get; set; }
    public bool ValidateCoverage { get; set; }
    public bool RequireComplete { get; set; }
    public string? IsolatedCoverage { get; set; }
    public bool IsolatedLifecycle { get; set; }
    public string? Campaign { get; set; }
    public int Workers { get; set; } = Math.Clamp(Environment.ProcessorCount / 2, 1, 4);
    public int BudgetMinutes { get; set; } = 480;
    public int ShrinkAttempts { get; set; } = 24;
    public bool CampaignIsolated { get; set; }
    public bool PrepareWorld { get; set; }
    public bool CampaignOffline { get; set; }
    public bool CampaignValidateOnly { get; set; }
    public string? CampaignPrincipal { get; set; }

    public static Args Parse(string[] args)
    {
        var a = new Args();
        for (int i = 0; i < args.Length; i++)
        {
            string Next() => ++i < args.Length ? args[i] : throw new ArgumentException("Missing argument value.");
            switch (args[i])
            {
                case "--analyzer": a.Analyzer = Next(); break;
                case "--tests": a.Tests.Add(Next()); break;
                case "--samples": a.Samples.Add(Next()); break;
                case "--sql": a.Sql.Add(Next()); break;
                case "--server": a.Server = Next(); break;
                case "--out": a.Out = Next(); break;
                case "--limit": a.Limit = int.Parse(Next()); break;
                case "--filter": a.Filter = Next(); break;
                case "--timeout": a.Timeout = int.Parse(Next()); break;
                case "--keep": a.Keep = true; break;
                case "--legacy-discovery": a.LegacyDiscovery = true; break;
                case "--test-index": a.TestIndex = Next(); break;
                case "--coverage": a.Coverage = Next(); break;
                case "--isolated-coverage": a.IsolatedCoverage = Next(); break;
                case "--isolated-lifecycle": a.IsolatedLifecycle = true; break;
                case "--validate-coverage": a.ValidateCoverage = true; break;
                case "--require-complete": a.RequireComplete = true; break;
                case "--campaign": a.Campaign = Next(); break;
                case "--workers": a.Workers = int.Parse(Next()); break;
                case "--budget-minutes": a.BudgetMinutes = int.Parse(Next()); break;
                case "--shrink-attempts": a.ShrinkAttempts = int.Parse(Next()); break;
                case "--campaign-isolated": a.CampaignIsolated = true; break;
                case "--prepare-world": a.PrepareWorld = true; break;
                case "--campaign-offline": a.CampaignOffline = true; break;
                case "--campaign-validate": a.CampaignValidateOnly = true; break;
                case "--campaign-principal": a.CampaignPrincipal = Next(); break;
                default: throw new ArgumentException($"Unknown argument {args[i]}");
            }
        }

        var coverageModes = (a.Coverage != null ? 1 : 0) + (a.IsolatedCoverage != null ? 1 : 0) + (a.IsolatedLifecycle ? 1 : 0);
        if ((a.Campaign != null ? 1 : 0) + coverageModes + (a.LegacyDiscovery ? 1 : 0) + (a.TestIndex != null ? 1 : 0) != 1)
            throw new ArgumentException("Select exactly one mode: --campaign, --coverage, --isolated-coverage, --isolated-lifecycle, --test-index, or --legacy-discovery.");
        if (a.LegacyDiscovery && (!args.Contains("--server") || string.IsNullOrWhiteSpace(a.Server)))
            throw new ArgumentException("Legacy discovery recreates fixed databases and requires --server <disposable instance> explicitly.");
        if (!a.LegacyDiscovery && a.Tests.Count + a.Sql.Count + a.Samples.Count != 0)
            throw new ArgumentException("Discovery inputs require --legacy-discovery.");
        if (a.Campaign != null && (coverageModes != 0 || a.Keep || a.Tests.Count + a.Sql.Count + a.Samples.Count != 0 || a.ValidateCoverage || a.RequireComplete))
            throw new ArgumentException("Campaign mode cannot combine coverage or discovery modes.");
        if (a.Workers < 1 || a.Workers > 16 || a.BudgetMinutes < 0 || a.Timeout < 1 || a.ShrinkAttempts < 0 || a.Limit < 0)
            throw new ArgumentException("Invalid campaign bounds.");
        if (a.CampaignPrincipal != null && a.CampaignPrincipal != "admin") throw new ArgumentException("Only explicit admin replay is supported.");
        if ((a.CampaignOffline || a.CampaignValidateOnly || a.CampaignPrincipal != null) && a.Campaign == null) throw new ArgumentException("Campaign switches require --campaign.");
        if (a.CampaignIsolated && a.Campaign == null) throw new ArgumentException("--campaign-isolated requires --campaign.");
        if (a.PrepareWorld && a.Campaign == null) throw new ArgumentException("--prepare-world requires --campaign.");
        if (coverageModes > 1 || (coverageModes == 0 && (a.ValidateCoverage || a.RequireComplete)))
            throw new ArgumentException("Select exactly one coverage mode when using coverage-only options.");
        if (a.IsolatedCoverage != null && (a.Keep || a.Tests.Count + a.Sql.Count + a.Samples.Count != 0))
            throw new ArgumentException("Isolated coverage cannot combine discovery inputs or --keep.");
        if (string.IsNullOrEmpty(a.Analyzer) && a.TestIndex == null)
            throw new ArgumentException("--analyzer <path to SqlSafetyGuard.Analyzer.dll> is required");
        return a;
    }
}
