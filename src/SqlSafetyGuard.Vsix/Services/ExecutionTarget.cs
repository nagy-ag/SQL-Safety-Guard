using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using SqlSafetyGuard.UI;

namespace SqlSafetyGuard.Services
{
    internal sealed class ExecutionTarget
    {
        public QuerySnapshot Snapshot { get; private set; } = new QuerySnapshot(null, string.Empty, false, 1, 1);
        public ConnectionContext Connection { get; private set; } = new ConnectionContext();
        public bool? SqlCmdMode { get; private set; }
        public string DocumentName { get; private set; } = "unknown execution target";
        public string Warning { get; private set; }
        private const string SqlEditorType = "Microsoft.SqlServer.Management.UI.VSIntegration.Editors.SqlScriptEditorControl";

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        private static object EditorFromHandle(IntPtr handle)
        {
            for (var control = Control.FromChildHandle(handle); control != null; control = control.Parent)
                if (control.GetType().FullName == SqlEditorType) return control;
            return null;
        }

        public static ExecutionTarget Capture(IServiceProvider services, DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var target = new ExecutionTarget();
            var problems = new List<string>();
            try
            {
                var monitor = services.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
                if (monitor == null || ErrorHandler.Failed(monitor.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out var selected)) ||
                    !(selected is IVsWindowFrame frame))
                    throw new InvalidOperationException("The command-target window frame is unavailable.");
                ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var editor));
                var focusedEditor = EditorFromHandle(GetFocus());
                if (focusedEditor == null) problems.Add("The focused SQL command-target owner is unavailable.");
                else
                {
                    if (!ReferenceEquals(editor, focusedEditor)) problems.Add("The focused SQL command target and selected window frame disagree.");
                    editor = focusedEditor;
                }
                if (editor?.GetType().FullName != SqlEditorType || !(editor is IVsCodeWindow codeWindow))
                    throw new InvalidOperationException("The command-target frame is not a supported SQL editor.");
                if (ErrorHandler.Failed(monitor.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_DocumentFrame, out var selectedDocument)) ||
                    !(selectedDocument is IVsWindowFrame documentFrame) ||
                    ErrorHandler.Failed(documentFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var documentEditor)))
                    problems.Add("The selected document frame is unavailable.");
                else if (!ReferenceEquals(documentEditor, editor))
                    problems.Add("The selected document frame and focused SQL command target disagree.");
                ErrorHandler.ThrowOnFailure(codeWindow.GetLastActiveView(out var view));
                ErrorHandler.ThrowOnFailure(view.GetBuffer(out var buffer));
                target.DocumentName = Property(editor, "Moniker") as string ?? target.DocumentName;
                Document document = null;
                foreach (Document candidate in dte.Documents)
                    if (string.Equals(candidate.FullName, target.DocumentName, StringComparison.OrdinalIgnoreCase)) document = candidate;
                ErrorHandler.ThrowOnFailure(view.GetSelection(out var startLine, out var startColumn, out var endLine, out var endColumn));
                if (startLine > endLine || (startLine == endLine && startColumn > endColumn))
                {
                    var swapLine = startLine; var swapColumn = startColumn;
                    startLine = endLine; startColumn = endColumn; endLine = swapLine; endColumn = swapColumn;
                }
                bool selection = startLine != endLine || startColumn != endColumn;
                if (!selection)
                {
                    startLine = 0; startColumn = 0;
                    ErrorHandler.ThrowOnFailure(buffer.GetLastLineIndex(out endLine, out endColumn));
                }
                ErrorHandler.ThrowOnFailure(buffer.GetLineText(startLine, startColumn, endLine, endColumn, out var text));
                target.Snapshot = new QuerySnapshot(document, text, selection, startLine + 1, startColumn + 1);
                var span = editor.GetType().GetMethod("GetSelectedTextSpan", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(editor, null);
                if (!string.Equals(Property(span, "Text") as string, text, StringComparison.Ordinal))
                    problems.Add("The SQL editor execution span and view selection disagree.");
                if (!string.Equals(dte.ActiveDocument?.FullName, target.DocumentName, StringComparison.OrdinalIgnoreCase))
                    problems.Add("DTE's active document and the command-target frame disagree.");
                var manager = services.GetService(typeof(SVsTextManager)) as IVsTextManager;
                if (manager == null) problems.Add("The active text-view service is unavailable.");
                else
                {
#if DEBUG
                    TraceView(manager, 0, view);
                    TraceView(manager, 1, view);
#endif
                    CheckView(manager, view, editor, problems);
                }
                target.SqlCmdMode = Property(editor, "IsSqlCmdModeEnabled") as bool?;
                if (!target.SqlCmdMode.HasValue) problems.Add("The target editor's SQLCMD mode is unavailable.");
                var connections = Property(editor, "ConnectionInfoList") as ICollection;
                if (connections == null || connections.Count != 1) problems.Add("A single target connection could not be verified.");
                var connection = Field(editor, "m_connection") as IDbConnection;
                target.Connection = new ConnectionContext { Server = Property(connection, "DataSource") as string, Database = connection?.Database };
                if (string.IsNullOrWhiteSpace(target.Connection.Server) || string.IsNullOrWhiteSpace(target.Connection.Database))
                    problems.Add("The target editor's live server/database is unavailable.");
            }
            catch (Exception exception)
            {
                problems.Add("Target lookup unavailable (" + exception.GetType().Name + ").");
            }
            target.Warning = problems.Count == 0 ? null : "The guard could not verify which window will run. " + string.Join(" ", problems) +
                " The analysis below is only for the captured candidate; Cancel and select the intended SQL editor before retrying.";
#if DEBUG
            ExtensionCheckTrace.Write("target", target.DocumentName, ExtensionCheckTrace.Hash(target.Snapshot.Text), target.Warning ?? "verified",
                target.SqlCmdMode.ToString(), target.Connection.Server, target.Connection.Database);
#endif
            return target;
        }

        private static void CheckView(IVsTextManager manager, IVsTextView expected, object editor, List<string> problems)
        {
            int status = manager.GetActiveView(1, null, out var active);
            if (ErrorHandler.Failed(status) || active == null)
            {
                problems.Add("The focused text view is unavailable.");
                return;
            }
            ErrorHandler.ThrowOnFailure(active.GetBuffer(out var activeBuffer));
            ErrorHandler.ThrowOnFailure(expected.GetBuffer(out var expectedBuffer));
            activeBuffer.GetLanguageServiceID(out var activeLanguage);
            expectedBuffer.GetLanguageServiceID(out var expectedLanguage);
            if (activeLanguage != expectedLanguage)
            {
                var handle = active.GetWindowHandle();
                if (!ReferenceEquals(EditorFromHandle(handle), editor)) problems.Add("The focused results view does not belong to the SQL command target.");
                return;
            }
            if (!SameIdentity(activeBuffer, expectedBuffer)) problems.Add("The focused view and command-target frame disagree.");
            else if (!SameIdentity(active, expected))
            {
                ErrorHandler.ThrowOnFailure(active.GetSelection(out var activeStartLine, out var activeStartColumn, out var activeEndLine, out var activeEndColumn));
                ErrorHandler.ThrowOnFailure(expected.GetSelection(out var expectedStartLine, out var expectedStartColumn, out var expectedEndLine, out var expectedEndColumn));
                if (activeStartLine != expectedStartLine || activeStartColumn != expectedStartColumn || activeEndLine != expectedEndLine || activeEndColumn != expectedEndColumn)
                    problems.Add("The focused view and command-target selection disagree.");
            }
        }

#if DEBUG
        private static void TraceView(IVsTextManager manager, int mustHaveFocus, IVsTextView expected)
        {
            int status = manager.GetActiveView(mustHaveFocus, null, out var active);
            if (ErrorHandler.Failed(status) || active == null)
            {
                ExtensionCheckTrace.Write("target-view", mustHaveFocus.ToString(), "unavailable");
                return;
            }
            ErrorHandler.ThrowOnFailure(active.GetBuffer(out var buffer));
            ErrorHandler.ThrowOnFailure(expected.GetBuffer(out var expectedBuffer));
            ErrorHandler.ThrowOnFailure(buffer.GetLastLineIndex(out var endLine, out var endColumn));
            ErrorHandler.ThrowOnFailure(buffer.GetLineText(0, 0, endLine, endColumn, out var text));
            buffer.GetLanguageServiceID(out var language);
            expectedBuffer.GetLanguageServiceID(out var expectedLanguage);
            ExtensionCheckTrace.Write("target-view", mustHaveFocus.ToString(), SameIdentity(buffer, expectedBuffer).ToString(), ExtensionCheckTrace.Hash(text), language.ToString(), expectedLanguage.ToString(), text.Length.ToString());
        }
#endif

        private static bool SameIdentity(object first, object second)
        {
            IntPtr firstIdentity = Marshal.GetIUnknownForObject(first);
            try
            {
                IntPtr secondIdentity = Marshal.GetIUnknownForObject(second);
                try { return firstIdentity == secondIdentity; }
                finally { Marshal.Release(secondIdentity); }
            }
            finally { Marshal.Release(firstIdentity); }
        }

        private static object Property(object instance, string name) => instance?.GetType()
            .GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance);

        private static object Field(object instance, string name)
        {
            for (var type = instance.GetType(); type != null; type = type.BaseType)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null) return field.GetValue(instance);
            }
            return null;
        }
    }
}
