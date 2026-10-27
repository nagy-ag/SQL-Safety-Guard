using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;
using SqlSafetyGuard.Services;
using SqlSafetyGuard.UI;

namespace SqlSafetyGuard.Guard
{
    /// <summary>
    /// Intercepts SSMS's Query.Execute (F5, Ctrl+E, Alt+X, toolbar, menu) through DTE CommandEvents.BeforeExecute.
    ///
    /// Why CommandEvents rather than IVsRegisterPriorityCommandTarget: the event is filtered to this one
    /// command (no QueryStatus traffic for every command on every keystroke), CancelDefault is a supported way
    /// to stop the command, and it is the hook SSMS extensions have used successfully since SSMS 17. A priority
    /// command target would see every command in the IDE and a bug in it could disable unrelated commands.
    ///
    /// "Run anyway" does not re-issue the command: the dialog is shown while SSMS is still inside the original
    /// dispatch, so the guard simply leaves CancelDefault false and the paused execution continues. A
    /// re-entrancy flag protects against the command being raised again while the guard is running.
    /// </summary>
    internal sealed class ExecuteGuard
    {
        private readonly SqlSafetyGuardPackage _package;
        private CommandEvents _events; // Must be kept alive, or the COM event sink is garbage collected.
        private bool _running;

        private sealed class ExecutionCheck
        {
            public GuardSettings Settings;
            public AnalysisResult Result;
            public bool Cancelled;
        }

        public ExecuteGuard(SqlSafetyGuardPackage package)
        {
            _package = package;
        }

        public void Attach()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string guid = SsmsCommands.QueryEditorCommandSet.ToString("B");
            int id = SsmsCommands.ExecuteQuery;
            try
            {
                var command = _package.Dte.Commands.Item(SsmsCommands.ExecuteCommandName);
                guid = command.Guid;
                id = command.ID;
            }
            catch (ArgumentException)
            {
                OutputLog.Warning($"Command {SsmsCommands.ExecuteCommandName} not found by name; using the known SSMS id.");
            }

            _events = _package.Dte.Events.CommandEvents[guid, id];
            _events.BeforeExecute += OnBeforeExecute;
            OutputLog.Info($"F5 guard attached to {SsmsCommands.ExecuteCommandName} {guid}:{id}.");
        }

        private void OnBeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_running)
                return;

            var check = new ExecutionCheck();
            try
            {
                _running = true;
                cancelDefault = ShouldCancel(check);
            }
            catch (Exception ex)
            {
                cancelDefault = check.Cancelled || GuardDecision.CancelAfterFailure(check.Settings, check.Result);
                try { _package.ReportGuardFailure(ex); }
                catch { }
            }
            finally
            {
                _running = false;
            }
        }

        /// <summary>Returns true to stop the execution.</summary>
        private bool ShouldCancel(ExecutionCheck check)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var options = _package.Options;
            var connection = ConnectionInfoReader.TryRead(_package);
            var settings = _package.ResolveSettings(connection, out var rules);
            check.Settings = settings;
            _package.RefreshHealth(settings);
            if (settings.Mode == ExecutionMode.Off)
                return false;

#if DEBUG
            // Lets the fail-open path be tested: start SSMS with SQLSAFETYGUARD_FAULT_INJECTION=1 (Debug builds only).
            if (Environment.GetEnvironmentVariable("SQLSAFETYGUARD_FAULT_INJECTION") == "1")
                throw new InvalidOperationException("Fault injected by SQLSAFETYGUARD_FAULT_INJECTION=1.");
#endif

            var snapshot = SqlEditorAccess.TryCapture(_package.Dte);
            if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.Text))
                return false;

            switch (AnalysisRunner.Run(snapshot.Text, _package.CreateAnalyzerOptions(connection, settings, rules), out check.Result))
            {
                case AnalysisOutcome.Cancelled:
                    check.Cancelled = true;
                    OutputLog.Info("Analysis was cancelled; the query was not executed.");
                    return true;
            }

            var result = check.Result;
            return GuardDecision.ShouldCancel(settings, result,
                () => _package.RefreshHealth(settings, successfulCheck: true),
                () =>
                {
                    var dialog = new ConfirmExecutionDialog(result, snapshot, connection, settings, options.ShowServerAndDatabase);
                    bool runAnyway = dialog.ShowModal() == true;
                    OutputLog.Info(runAnyway ? $"Run anyway: {result.Summary}." : $"Execution cancelled: {result.Summary}.");
                    if (!runAnyway && dialog.NavigateTo != null)
                    {
                        try { SqlEditorAccess.Navigate(snapshot, dialog.NavigateTo.StartOffset); }
                        catch (Exception ex) { _package.ReportGuardFailure(ex); }
                    }
                    return runAnyway;
                }, _package.ReportGuardFailure);
        }
    }
}
