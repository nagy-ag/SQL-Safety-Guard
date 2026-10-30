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
    /// "Analyze Query Safety" (Tools menu, SQL Editor toolbar, Ctrl+Shift+F5): expands the safety pane of the active
    /// query window (if it was collapsed) and re-analyzes the selection, or the whole window, without executing anything.
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

            // Separate placement keeps the menu action stable while the toolbar shows compact guard health.
            var toolbar = new OleMenuCommand((s, e) => Execute(package), new CommandID(PackageGuids.CommandSet, PackageIds.ToolbarAnalyzeCommand));
            toolbar.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                package.RefreshHealth(queryStatus: true);
                toolbar.Text = package.ToolbarText;
                toolbar.AutomationName = "Analyze query. " + package.HealthText;
                var document = package.Dte?.ActiveDocument;
                toolbar.Enabled = document != null && SqlEditorAccess.IsSqlDocument(document);
            };
            commandService.AddCommand(toolbar);
        }

        private static void Execute(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var document = package.Dte?.ActiveDocument;
                if (document == null || !SqlEditorAccess.IsSqlDocument(document))
                {
                    ShowMessage(package, "Open a SQL query window to analyze it.");
                    return;
                }

                if (!SafetyPaneMargin.ShowAndRefresh(document.FullName))
                    ShowMessage(package, "This query window has no safety pane. Close and reopen it, or restart SSMS after installing.");
            }
            catch (Exception ex)
            {
                package.ReportGuardFailure(ex);
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
