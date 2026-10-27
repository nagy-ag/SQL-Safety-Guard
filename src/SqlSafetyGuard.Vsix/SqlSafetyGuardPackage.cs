using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Commands;
using SqlSafetyGuard.Guard;
using SqlSafetyGuard.Options;
using SqlSafetyGuard.Services;
using SqlSafetyGuard.UI;
using Task = System.Threading.Tasks.Task;

namespace SqlSafetyGuard
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("SQL Safety Guard", "Analyzes T-SQL before it runs and warns about real data changes.", "1.0")]
    [Guid(PackageGuids.PackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ResultsToolWindow), Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindOutput, Orientation = ToolWindowOrientation.Bottom)]
    [ProvideOptionPage(typeof(GeneralOptionsPage), "SQL Safety Guard", "General", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideBindingPath]
    public sealed class SqlSafetyGuardPackage : AsyncPackage
    {
        private ExecuteGuard _guard;
        private ActiveDocumentWatcher _documentWatcher; // Kept alive for the selection-events subscription.
        private uint _selectionEventsCookie;

        /// <summary>Incremented whenever the results window gets new content; stale background refreshes are dropped.</summary>
        private int _resultsVersion;

        internal DTE2 Dte { get; private set; }

        internal GeneralOptionsPage Options => (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));

        /// <summary>Analyzer options for the active query window (allow-list setting and its SQLCMD mode).</summary>
        internal AnalyzerOptions CreateAnalyzerOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return AnalyzerOptions.FromAllowListText(Options.AllowList, sqlCmdMode: SqlCmdModeReader.TryRead(this));
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            OutputLog.Initialize(this);

            try
            {
                Dte = (DTE2)await GetServiceAsync(typeof(DTE));
                var commandService = (OleMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService));

                AnalyzeQueryCommand.Initialize(this, commandService);

                _guard = new ExecuteGuard(this);
                _guard.Attach();

                // Follow the active editor tab in the results window.
                if (await GetServiceAsync(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection)
                {
                    _documentWatcher = new ActiveDocumentWatcher(this);
                    monitorSelection.AdviseSelectionEvents(_documentWatcher, out _selectionEventsCookie);
                }

                OutputLog.Info($"Loaded (parser: {SqlSafetyAnalyzer.ParserName}, F5 guard: {Options.GuardMode}).");
            }
            catch (Exception ex)
            {
                OutputLog.Error("Initialization failed. The F5 guard is not active.", ex);
            }
        }

        /// <summary>Shows the results tool window with a fresh analysis.</summary>
        internal async Task ShowResultsAsync(QuerySnapshot snapshot, AnalysisResult result)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            var window = (ResultsToolWindow)await ShowToolWindowAsync(typeof(ResultsToolWindow), 0, true, DisposalToken);
            _resultsVersion++;
            window.Control.ShowResult(snapshot, result);
        }

        /// <summary>Updates the results tool window if it has already been created (never opens it).</summary>
        internal void UpdateResultsIfOpen(QuerySnapshot snapshot, AnalysisResult result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (FindToolWindow(typeof(ResultsToolWindow), 0, false) is ResultsToolWindow window)
            {
                _resultsVersion++;
                window.Control.ShowResult(snapshot, result);
            }
        }

        /// <summary>Refreshes the results window for the active tab (when the tab changes or the window is shown).</summary>
        internal void RequestResultsRefresh()
        {
            _ = JoinableTaskFactory.RunAsync(RefreshResultsForActiveDocumentAsync);
        }

        private async Task RefreshResultsForActiveDocumentAsync()
        {
            try
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
                if (!(FindToolWindow(typeof(ResultsToolWindow), 0, false) is ResultsToolWindow window) ||
                    !(window.Frame is IVsWindowFrame frame) || frame.IsVisible() != VSConstants.S_OK)
                    return; // Hidden: refreshed when it becomes visible.

                var control = window.Control;
                var document = Dte?.ActiveDocument;
                if (document == null)
                {
                    _resultsVersion++;
                    control.ShowPlaceholder("No query window is active", "Switch to a SQL query window to see its analysis.");
                    return;
                }

                var snapshot = SqlEditorAccess.TryCapture(Dte);
                if (snapshot == null)
                {
                    _resultsVersion++;
                    control.ShowPlaceholder($"{document.Name} isn't a SQL query window", "SQL Safety Guard only analyzes T-SQL query windows.");
                    return;
                }

                if (control.IsShowing(snapshot))
                    return; // Same tab, same text: nothing to do.

                int version = ++_resultsVersion;
                if (snapshot.Text.Length > AnalysisRunner.MaxTextLength)
                {
                    control.ShowPlaceholder($"{document.Name} is larger than {AnalysisRunner.MaxTextLength / (1024 * 1024)} MB",
                        "It is not analyzed; the F5 guard lets it run with a log message.");
                    return;
                }

                // Analyzed off the UI thread, so switching tabs never freezes SSMS. Nothing is executed.
                var options = CreateAnalyzerOptions();
                if (snapshot.Text.Length > 100 * 1024)
                    control.ShowPlaceholder($"Analyzing {document.Name}…", $"{snapshot.Text.Length / 1024:N0} KB of T-SQL, not executed");
                var result = await Task.Run(() => SqlSafetyAnalyzer.Analyze(snapshot.Text, options), DisposalToken);

                await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
                if (version == _resultsVersion)
                    control.ShowResult(snapshot, result);
            }
            catch (OperationCanceledException)
            {
                // SSMS is closing.
            }
            catch (Exception ex)
            {
                OutputLog.Error("Refreshing the results window failed.", ex);
            }
        }
    }
}
