using System.Collections.Generic;
using System.Linq;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Entry point: classifies T-SQL text without executing it.
    /// </summary>
    public static class SqlSafetyAnalyzer
    {
        /// <summary>Name of the ScriptDom parser in use, e.g. TSql180Parser.</summary>
        public static string ParserName => ParserFactory.ParserName;

        public static AnalysisResult Analyze(string sql, AnalyzerOptions options = null)
        {
            options = options ?? AnalyzerOptions.Default;
            var findings = new List<Finding>();
            var errors = new List<SqlParseError>();
            var skipped = new List<SkippedRegion>();

            if (!string.IsNullOrWhiteSpace(sql))
            {
                var walker = new StatementWalker(options, findings, errors, skipped);

                Scope scope;
                if (!SqlCmdPreprocessor.LooksLikeSqlCmd(sql))
                {
                    scope = Scope.ForText(sql);
                    walker.AnalyzeText(scope);
                }
                else if (options.SqlCmdMode != false)
                {
                    // SQLCMD mode on (or unknown): parse a same-length copy with the SQLCMD lines blanked; show the original text.
                    var parsed = SqlCmdPreprocessor.Process(sql, out var directives, out var variables);
                    scope = Scope.ForText(parsed, sql, variables);
                    walker.AddSqlCmdDirectives(scope, directives, variables);
                    walker.AnalyzeText(scope);
                }
                else
                {
                    // SQLCMD mode off: the server gets these lines as they are, so they are syntax errors and their batches
                    // fail. Analyze exactly that, and say why.
                    scope = Scope.ForText(sql);
                    walker.AnalyzeText(scope);
                    walker.AddSqlCmdModeOffHint(scope);
                }

                walker.FinishTransactions(scope);
            }

            // Text order (stable, so findings of the same dynamic SQL keep their order).
            var ordered = findings.OrderBy(f => f.StartOffset).ToList();
            return new AnalysisResult(ordered, errors.OrderBy(e => e.Offset).ToList(), skipped.OrderBy(s => s.StartOffset).ToList());
        }
    }
}
