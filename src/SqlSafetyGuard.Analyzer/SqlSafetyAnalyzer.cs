using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Entry point: classifies T-SQL text without executing it.
    /// </summary>
    public static class SqlSafetyAnalyzer
    {
        public const int MaxTextLength = 5 * 1024 * 1024;
        /// <summary>Name of the ScriptDom parser in use, e.g. TSql180Parser.</summary>
        public static string ParserName => ParserFactory.ParserName;

        public static AnalysisResult Analyze(string sql, AnalyzerOptions options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (sql != null && sql.Length > MaxTextLength)
                return new AnalysisResult(new[] { new Finding(1, 1, 0, 0, "NOT ANALYZED", string.Empty, null, Verdict.Unknown,
                    $"too large to analyze ({sql.Length * 2d / (1024 * 1024):N1} MB of UTF-16 text); no safety check was performed") }, new SqlParseError[0]);
            AnalysisResult result = null;
            Exception failure = null;
            var worker = new Thread(() =>
            {
                try
                {
                    new AnalysisWork(cancellationToken);
                    result = AnalyzeCore(sql, options);
                }
                catch (Exception exception) { failure = exception; }
            }, 64 * 1024 * 1024) { IsBackground = true, Name = "SQL Safety Guard analysis" };
            worker.Start();
            worker.Join();
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
            return result;
        }

        private static AnalysisResult AnalyzeCore(string sql, AnalyzerOptions options)
        {
            options = options ?? AnalyzerOptions.Default;
            var findings = new List<Finding>();
            var errors = new List<SqlParseError>();
            var skipped = new List<SkippedRegion>();
            foreach (var warning in options.AllowList.Warnings.Concat(options.TrustedProcedures.Warnings).Concat(options.ConfigurationWarnings))
                findings.Add(new Finding(1, 1, 0, 0, "SETTINGS WARNING", string.Empty, null, Verdict.Unknown, warning));

            try
            {
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
            }
            catch (AnalysisLimitException exception)
            {
                findings.Add(new Finding(1, 1, 0, 0, "NOT ANALYZED", string.Empty, null, Verdict.Unknown, exception.Message));
            }

            // Text order (stable, so findings of the same dynamic SQL keep their order).
            var ordered = findings.OrderBy(f => f.StartOffset).ToList();
            return new AnalysisResult(ordered, errors.OrderBy(e => e.Offset).ToList(), skipped.OrderBy(s => s.StartOffset).ToList());
        }
    }
}
