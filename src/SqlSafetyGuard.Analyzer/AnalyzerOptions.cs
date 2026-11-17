using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class AnalyzerOptions
    {
        private readonly Dictionary<string, AllowList> _contextLists = new Dictionary<string, AllowList>(StringComparer.Ordinal);
        public static AnalyzerOptions Default { get; } = new AnalyzerOptions();

        public AnalyzerOptions(IEnumerable<string> allowListPatterns = null, bool tempdbIsTemp = false, int maxDynamicSqlDepth = 3,
            bool? sqlCmdMode = null, string currentDatabase = null, string defaultSchema = null,
            IEnumerable<string> trustedProcedures = null, IEnumerable<string> configurationWarnings = null, string currentServer = null,
            EnvironmentRules environmentRules = null, GuardSettings globalSettings = null, IEnumerable<string> possibleDatabases = null)
        {
            AllowList = new AllowList(allowListPatterns ?? Enumerable.Empty<string>(), requireQualification: true);
            TempdbIsTemp = tempdbIsTemp;
            MaxDynamicSqlDepth = Math.Max(0, maxDynamicSqlDepth);
            SqlCmdMode = sqlCmdMode;
            CurrentDatabase = currentDatabase;
            DefaultSchema = defaultSchema;
            TrustedProcedures = new AllowList(trustedProcedures, procedures: true, requireQualification: true);
            TrustedFunctions = new AllowList(trustedProcedures, procedures: true, requireQualification: true, functions: true);
            CurrentServer = currentServer;
            EnvironmentRules = environmentRules;
            GlobalSettings = globalSettings;
            PossibleDatabases = (possibleDatabases ?? new[] { currentDatabase }).Distinct(StringComparer.Ordinal).ToArray();
            ConfigurationWarnings = (configurationWarnings ?? Enumerable.Empty<string>()).ToArray();
        }

        /// <summary>Builds options from the multi-line allow-list text of the settings page (one pattern per line).</summary>
        public static AnalyzerOptions FromAllowListText(string allowListText, bool tempdbIsTemp = false, bool? sqlCmdMode = null) =>
            new AnalyzerOptions(AllowList.SplitLines(allowListText), tempdbIsTemp, sqlCmdMode: sqlCmdMode);

        public AllowList AllowList { get; }
        public AllowList TrustedProcedures { get; }
        public AllowList TrustedFunctions { get; }
        public string CurrentServer { get; }
        internal EnvironmentRules EnvironmentRules { get; }
        internal GuardSettings GlobalSettings { get; }
        internal IReadOnlyList<string> PossibleDatabases { get; }

        internal AnalyzerOptions WithDatabase(string database) => new AnalyzerOptions(AllowList.Patterns, TempdbIsTemp,
            MaxDynamicSqlDepth, SqlCmdMode, CurrentDatabase, DefaultSchema, TrustedProcedures.Patterns, ConfigurationWarnings,
            CurrentServer, EnvironmentRules, GlobalSettings, PossibleDatabases.Concat(new[] { database }));

        internal bool IsApproved(string server, string database, string schema, string name, bool routine = false, bool function = false)
        {
            // A default schema alone does not prove which object exists; SQL can fall back to dbo (or master for sp_ calls).
            if (!string.IsNullOrEmpty(server) || string.IsNullOrEmpty(CurrentServer) || string.IsNullOrEmpty(schema)) return false;
            return PossibleDatabases.Count > 0 && (database == null ? PossibleDatabases : new[] { database }).All(db =>
            {
                if (string.IsNullOrEmpty(db)) return false;
                var list = routine ? (function ? TrustedFunctions : TrustedProcedures) : AllowList;
                if (EnvironmentRules != null && GlobalSettings != null)
                {
                    var key = (routine ? (function ? "F:" : "P:") : "W:") + db;
                    if (!_contextLists.TryGetValue(key, out list))
                    {
                        var settings = EnvironmentRules.Resolve(CurrentServer, db, GlobalSettings);
                        _contextLists[key] = list = new AllowList(routine ? settings.TrustedProcedures : settings.AllowList, routine, true, function);
                    }
                }
                return list.Matches(null, db, schema, name, db, DefaultSchema, CurrentServer);
            });
        }

        internal bool RequiresConfirmation(Verdict verdict, string target)
        {
            if (verdict.IsGreen() || EnvironmentRules == null || GlobalSettings == null) return false;
            var parts = AllowList.SplitName(target ?? "");
            if (parts?.Length == 4 && EnvironmentRules.HasRules) return true;
            var databases = parts != null && parts.Length == 3 ? new[] { parts[0] } : PossibleDatabases;
            return databases.Any(db =>
            {
                var mode = EnvironmentRules.Resolve(CurrentServer, db, GlobalSettings).Mode;
                return mode == ExecutionMode.YellowRed || (mode == ExecutionMode.Red && verdict == Verdict.RealWrite);
            });
        }
        public IReadOnlyList<string> ConfigurationWarnings { get; }
        public string CurrentDatabase { get; }
        public string DefaultSchema { get; }

        /// <summary>When true, objects in tempdb (tempdb.dbo.x, tempdb..x) count as temp objects.</summary>
        public bool TempdbIsTemp { get; }

        /// <summary>How many levels of literal dynamic SQL (EXEC('...') / sp_executesql N'...') are parsed.</summary>
        public int MaxDynamicSqlDepth { get; }

        /// <summary>
        /// SSMS's Query &gt; SQLCMD Mode for the window: true = SQLCMD lines are understood; false = they are what the
        /// server would see (syntax errors) and a hint is added; null = unknown, SQLCMD lines are understood.
        /// </summary>
        public bool? SqlCmdMode { get; }
    }
}
