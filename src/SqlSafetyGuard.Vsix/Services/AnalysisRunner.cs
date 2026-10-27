using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SqlSafetyGuard.Analyzer;
using Task = System.Threading.Tasks.Task;

namespace SqlSafetyGuard.Services
{
    internal enum AnalysisOutcome
    {
        Completed,
        Cancelled,
    }

    internal static class AnalysisRunner
    {
        public static AnalysisOutcome Run(string text, AnalyzerOptions options, out AnalysisResult result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            result = null;
            var stopwatch = Stopwatch.StartNew();
            var factory = ServiceProvider.GlobalProvider.GetService(typeof(SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;
            try
            {
                if (factory == null)
                    result = RunBackground(text, options, CancellationToken.None);
                else
                {
                    var progress = new ThreadedWaitDialogProgressData("Analyzing query safety…", $"{text.Length / 1024:N0} KB of T-SQL",
                        "SQL Safety Guard: analyzing query", isCancelable: true);
                    using (var session = factory.StartWaitDialog("SQL Safety Guard", progress, TimeSpan.FromMilliseconds(300)))
                        result = RunBackground(text, options, session.UserCancellationToken);
                }
            }
            catch (OperationCanceledException) { return AnalysisOutcome.Cancelled; }
            if (stopwatch.ElapsedMilliseconds > 500)
                OutputLog.Info($"Analyzed {text.Length:N0} characters in {stopwatch.ElapsedMilliseconds:N0} ms.");
            return AnalysisOutcome.Completed;
        }

        private static AnalysisResult RunBackground(string text, AnalyzerOptions options, CancellationToken cancellation) =>
            ThreadHelper.JoinableTaskFactory.Run(async () =>
                await Task.Run(() => SqlSafetyAnalyzer.Analyze(text, options, cancellation), cancellation));
    }
}
