using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class AnalyzerOptions
    {
        public static AnalyzerOptions Default { get; } = new AnalyzerOptions();

        public AnalyzerOptions(IEnumerable<string> allowListPatterns = null, bool tempdbIsTemp = false, int maxDynamicSqlDepth = 3,
            bool? sqlCmdMode = null, string currentDatabase = null, string defaultSchema = null,
            IEnumerable<string> trustedProcedures = null, IEnumerable<string> configurationWarnings = null)
        {
            AllowList = new AllowList(allowListPatterns ?? Enumerable.Empty<string>());
            TempdbIsTemp = tempdbIsTemp;
            MaxDynamicSqlDepth = Math.Max(0, maxDynamicSqlDepth);
            SqlCmdMode = sqlCmdMode;
            CurrentDatabase = currentDatabase;
            DefaultSchema = defaultSchema;
            TrustedProcedures = new AllowList(trustedProcedures, procedures: true);
            ConfigurationWarnings = (configurationWarnings ?? Enumerable.Empty<string>()).ToArray();
        }

        /// <summary>Builds options from the multi-line allow-list text of the settings page (one pattern per line).</summary>
        public static AnalyzerOptions FromAllowListText(string allowListText, bool tempdbIsTemp = false, bool? sqlCmdMode = null) =>
            new AnalyzerOptions(AllowList.SplitLines(allowListText), tempdbIsTemp, sqlCmdMode: sqlCmdMode);

        public AllowList AllowList { get; }
        public AllowList TrustedProcedures { get; }
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
