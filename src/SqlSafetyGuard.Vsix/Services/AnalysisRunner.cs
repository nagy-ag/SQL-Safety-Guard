using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SqlSafetyGuard.Analyzer;
using Task = System.Threading.Tasks.Task;

namespace SqlSafetyGuard.Services
{
    internal enum AnalysisOutcome
    {
        Completed,
        TooLarge,
        Cancelled,
    }

    /// <summary>
    /// Runs the analyzer. Small texts are analyzed inline; larger ones on a background thread behind
    /// a cancellable wait dialog so SSMS stays responsive. Very large texts are skipped.
    /// </summary>
    internal static class AnalysisRunner
    {
        /// <summary>Scripts above this size are not analyzed (ScriptDom needs roughly 2 s per MB).</summary>
        public const int MaxTextLength = 5 * 1024 * 1024;

        /// <summary>Above this size the wait dialog is used (about 0.2 s of parsing).</summary>
        private const int InlineTextLength = 100 * 1024;

        public static AnalysisOutcome Run(string text, AnalyzerOptions options, out AnalysisResult result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            result = null;

            if (text.Length > MaxTextLength)
                return AnalysisOutcome.TooLarge;

            var stopwatch = Stopwatch.StartNew();
            if (text.Length <= InlineTextLength)
            {
                result = SqlSafetyAnalyzer.Analyze(text, options);
            }
            else
            {
                result = RunWithWaitDialog(text, options);
                if (result == null)
                    return AnalysisOutcome.Cancelled;
            }

            if (stopwatch.ElapsedMilliseconds > 500)
                OutputLog.Info($"Analyzed {text.Length:N0} characters in {stopwatch.ElapsedMilliseconds:N0} ms.");
            return AnalysisOutcome.Completed;
        }

        private static AnalysisResult RunWithWaitDialog(string text, AnalyzerOptions options)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var factory = (IVsThreadedWaitDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(SVsThreadedWaitDialogFactory));
            if (factory == null)
                return SqlSafetyAnalyzer.Analyze(text, options);

            var progress = new ThreadedWaitDialogProgressData(
                "Analyzing query safety…",
                $"{text.Length / 1024:N0} KB of T-SQL",
                "SQL Safety Guard: analyzing query",
                isCancelable: true);

            using (var session = factory.StartWaitDialog("SQL Safety Guard", progress, TimeSpan.FromMilliseconds(300)))
            {
                var cancellation = session.UserCancellationToken;
                return ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    var analysis = Task.Run(() => SqlSafetyAnalyzer.Analyze(text, options));
                    var cancelled = Task.Delay(Timeout.Infinite, cancellation);
                    var finished = await Task.WhenAny(analysis, cancelled);
                    return finished == analysis ? await analysis : null;
                });
            }
        }
    }
}
