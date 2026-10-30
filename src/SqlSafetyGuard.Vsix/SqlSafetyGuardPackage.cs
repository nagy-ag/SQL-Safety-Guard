using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Imaging;
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
    [ProvideOptionPage(typeof(GeneralOptionsPage), "SQL Safety Guard", "General", 110, 111, true)] // Names in VSPackage.resx.
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideBindingPath]
    public sealed class SqlSafetyGuardPackage : AsyncPackage
    {
        private ExecuteGuard _guard;
        private ActiveDocumentWatcher _documentWatcher; // Kept alive for the selection-events subscription.
        private uint _selectionEventsCookie;

        private bool _guardAttached;
        private bool _degraded;
        private bool _failureNoticeShown;
        private DateTime _healthRefreshedAt;
        internal string HealthText { get; private set; } = "SQL Safety Guard: starting";

        internal DTE2 Dte { get; private set; }

        internal GeneralOptionsPage Options => (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));

        /// <summary>Analyzer options for the active query window (allow-list setting and its SQLCMD mode).</summary>
        internal GuardSettings ResolveSettings(ConnectionContext connection, out EnvironmentRules rules)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var options = Options;
            rules = new EnvironmentRules(options.EnvironmentRules);
            var global = new GuardSettings((ExecutionMode)options.GuardMode, requireTypedConfirmation: options.RequireTypedConfirmation,
                allowList: AllowList.SplitLines(options.AllowList), trustedProcedures: AllowList.SplitLines(options.TrustedProcedures));
            return rules.Resolve(connection.Server, connection.Database, global);
        }

        internal AnalyzerOptions CreateAnalyzerOptions(ConnectionContext connection = null, GuardSettings settings = null, EnvironmentRules rules = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            connection = connection ?? ConnectionInfoReader.TryRead(this);
            if (settings == null) settings = ResolveSettings(connection, out rules);
            return new AnalyzerOptions(settings.AllowList, Options.TempdbIsTemp, sqlCmdMode: SqlCmdModeReader.TryRead(this),
                currentDatabase: connection.Database, trustedProcedures: settings.TrustedProcedures, configurationWarnings: rules?.Warnings);
        }

        internal void RefreshHealth(GuardSettings settings = null, bool successfulCheck = false, bool queryStatus = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (queryStatus && DateTime.UtcNow - _healthRefreshedAt < TimeSpan.FromSeconds(1)) return;
            _healthRefreshedAt = DateTime.UtcNow;
            try
            {
                var connection = ConnectionInfoReader.TryRead(this);
                settings = settings ?? ResolveSettings(connection, out _);
                if (successfulCheck) _degraded = false;
                var warnings = new EnvironmentRules(Options.EnvironmentRules).Warnings.Count > 0 ||
                    new AllowList(settings.AllowList).Warnings.Count > 0 || new AllowList(settings.TrustedProcedures, procedures: true).Warnings.Count > 0;
                HealthText = !_guardAttached || _degraded ? "SQL Safety Guard: degraded – last check failed" :
                    "SQL Safety Guard: " + (settings.Mode == ExecutionMode.Off ? "off" : "on · " + settings.Environment);
                if (connection.Server == null) HealthText += " · server unknown";
                if (warnings) HealthText += " · settings warning";
                (GetService(typeof(SVsStatusbar)) as IVsStatusbar)?.SetText(HealthText);
            }
            catch { HealthText = "SQL Safety Guard: degraded – status unavailable"; }
        }

        internal void ReportGuardFailure(Exception exception)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _degraded = true;
            try { OutputLog.Error("SQL Safety Guard failed. An unclassified query may run without a check; known warnings remain protected.", exception); }
            catch { }
            RefreshHealth();
            if (_failureNoticeShown) return;
            _failureNoticeShown = true;
            try
            {
                var shell = GetService(typeof(SVsShell)) as IVsShell;
                if (shell == null) return;
                shell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var hostObject);
                var host = hostObject as IVsInfoBarHost;
                var factory = GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
                if (host != null && factory != null)
                    host.AddInfoBar(factory.CreateInfoBar(new InfoBarModel(
                        "SQL Safety Guard is degraded: a check failed and unclassified queries may run without protection. See the SQL Safety Guard Output pane.",
                        KnownMonikers.StatusWarning, true)));
            }
            catch { }
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
                SettingsCommands.Initialize(this, commandService);

                _guard = new ExecuteGuard(this);
                _guard.Attach();
                _guardAttached = true;
                RefreshHealth();

                // Keep the status-bar health text in step with the active query window.
                if (await GetServiceAsync(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection)
                {
                    _documentWatcher = new ActiveDocumentWatcher(this);
                    monitorSelection.AdviseSelectionEvents(_documentWatcher, out _selectionEventsCookie);
                }

                OutputLog.Info($"Loaded (parser: {SqlSafetyAnalyzer.ParserName}, F5 guard: {Options.GuardMode}).");
            }
            catch (Exception ex)
            {
                ReportGuardFailure(ex);
            }
        }
    }
}
