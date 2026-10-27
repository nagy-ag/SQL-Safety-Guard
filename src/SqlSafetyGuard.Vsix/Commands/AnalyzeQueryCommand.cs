using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SqlSafetyGuard.Services;
using SqlSafetyGuard.UI;
using Task = System.Threading.Tasks.Task;

namespace SqlSafetyGuard.Commands
{
    /// <summary>
    /// "Analyze Query Safety" (Tools menu, SQL Editor toolbar, Ctrl+Shift+F5) and "SQL Safety Guard Results" (View menu).
    /// Analyzes the selection, or the whole window, without executing anything.
    /// </summary>
    internal static class AnalyzeQueryCommand
    {
        public static void Initialize(SqlSafetyGuardPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var analyze = new OleMenuCommand((s, e) => Execute(package), new CommandID(PackageGuids.CommandSet, PackageIds.AnalyzeQueryCommand));
            analyze.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var document = package.Dte?.ActiveDocument;
                analyze.Enabled = document != null && SqlEditorAccess.IsSqlDocument(document);
            };
            commandService.AddCommand(analyze);

            var showResults = new MenuCommand((s, e) =>
            {
                _ = package.JoinableTaskFactory.RunAsync(async () =>
                    await package.ShowToolWindowAsync(typeof(ResultsToolWindow), 0, true, package.DisposalToken));
            }, new CommandID(PackageGuids.CommandSet, PackageIds.ShowResultsCommand));
            commandService.AddCommand(showResults);
        }

        private static void Execute(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var snapshot = SqlEditorAccess.TryCapture(package.Dte);
                if (snapshot == null)
                {
                    ShowMessage(package, "Open a SQL query window to analyze it.");
                    return;
                }

                switch (AnalysisRunner.Run(snapshot.Text, package.CreateAnalyzerOptions(), out var result))
                {
                    case AnalysisOutcome.TooLarge:
                        ShowMessage(package, $"The script is larger than {AnalysisRunner.MaxTextLength / (1024 * 1024)} MB and was not analyzed.");
                        return;
                    case AnalysisOutcome.Cancelled:
                        return;
                }

                _ = package.JoinableTaskFactory.RunAsync(() => package.ShowResultsAsync(snapshot, result));
            }
            catch (Exception ex)
            {
                OutputLog.Error("Analyze Query Safety failed.", ex);
                ShowMessage(package, "The analysis failed. See the \"SQL Safety Guard\" Output pane for details.");
            }
        }

        private static void ShowMessage(IServiceProvider serviceProvider, string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            VsShellUtilities.ShowMessageBox(serviceProvider, message, "SQL Safety Guard",
                OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
