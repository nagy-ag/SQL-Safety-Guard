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

            try
            {
                _running = true;
                cancelDefault = ShouldCancel();
            }
            catch (Exception ex)
            {
                // Fail open: an extension bug must never block SSMS.
                OutputLog.Error("The F5 guard failed, so the query ran without a check.", ex);
                cancelDefault = false;
            }
            finally
            {
                _running = false;
            }
        }

        /// <summary>Returns true to stop the execution.</summary>
        private bool ShouldCancel()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var options = _package.Options;
            if (options.GuardMode == GuardMode.Off)
                return false;

#if DEBUG
            // Lets the fail-open path be tested: start SSMS with SQLSAFETYGUARD_FAULT_INJECTION=1 (Debug builds only).
            if (Environment.GetEnvironmentVariable("SQLSAFETYGUARD_FAULT_INJECTION") == "1")
                throw new InvalidOperationException("Fault injected by SQLSAFETYGUARD_FAULT_INJECTION=1.");
#endif

            var snapshot = SqlEditorAccess.TryCapture(_package.Dte);
            if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.Text))
                return false;

            switch (AnalysisRunner.Run(snapshot.Text, _package.CreateAnalyzerOptions(), out var result))
            {
                case AnalysisOutcome.TooLarge:
                    OutputLog.Warning($"Script is larger than {AnalysisRunner.MaxTextLength / (1024 * 1024)} MB; it ran without a safety check.");
                    return false;
                case AnalysisOutcome.Cancelled:
                    OutputLog.Info("Analysis was cancelled; the query was not executed.");
                    return true;
            }

            _package.UpdateResultsIfOpen(snapshot, result);

            bool warn = options.GuardMode == GuardMode.WarnOnRedOnly
                ? result.Overall == Verdict.RealWrite
                : !result.IsGreen;
            if (!warn)
                return false;

            var connection = options.ShowServerAndDatabase ? ConnectionInfoReader.TryDescribe(_package) : null;
            var dialog = new ConfirmExecutionDialog(result, snapshot, connection, options.GuardMode);
            bool runAnyway = dialog.ShowModal() == true;

            OutputLog.Info(runAnyway
                ? $"Run anyway: {result.Summary}."
                : $"Execution cancelled: {result.Summary}.");

            if (!runAnyway && dialog.NavigateTo != null)
                SqlEditorAccess.Navigate(snapshot, dialog.NavigateTo.StartOffset);

            return !runAnyway;
        }
    }
}
