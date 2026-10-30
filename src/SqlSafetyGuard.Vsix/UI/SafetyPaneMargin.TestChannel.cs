#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// Debug builds only: lets a test script drive the pane from inside SSMS, so testing needs no real
    /// keyboard, mouse or clipboard (which would disturb whatever else is running on the desktop).
    /// Commands are lines "documentName|verb|argument" in %TEMP%\SqlSafetyGuard-pane-cmd.txt; results go to the trace.
    /// </summary>
    internal sealed partial class SafetyPaneMargin
    {
        private static readonly string CommandFile = Path.Combine(
            Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY") ?? Path.GetTempPath(), "SqlSafetyGuard-pane-cmd.txt");

        partial void StartTestChannel()
        {
            var poll = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            poll.Tick += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (_disposed)
                {
                    poll.Stop();
                    return;
                }

                PollCommands();
            };
            poll.Start();
        }

        private void PollCommands()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string[] lines;
            try
            {
                if (!File.Exists(CommandFile))
                    return;
                lines = File.ReadAllLines(CommandFile);
                var name = Path.GetFileName(_documentPath ?? string.Empty);
                if (lines.Length == 0 || !lines.All(l => l.StartsWith(name + "|", StringComparison.OrdinalIgnoreCase)))
                    return; // For another window.
                File.Delete(CommandFile);
            }
            catch (IOException)
            {
                return; // Still being written.
            }

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { '|' }, 3);
                var verb = parts.Length > 1 ? parts[1] : string.Empty;
                var arg = parts.Length > 2 ? parts[2] : string.Empty;
                try
                {
                    RunCommand(verb, arg);
                    ExtensionCheckTrace.Write("ack", verb);
                }
                catch (Exception ex)
                {
                    PaneTrace.Write($"cmd {verb} FAILED: {ex.GetType().Name}: {ex.Message}");
                    ExtensionCheckTrace.Write("error", verb, ex.ToString());
                }
            }
        }

        private PaneGroup FindCommandGroup(string target)
        {
            var qualified = PaneGroup.Qualify(target, _connection?.Database);
            return _groups.First(group => string.Equals(group.Key, target, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(group.Key, qualified, StringComparison.OrdinalIgnoreCase) ||
                group.Findings.Any(finding => string.Equals(finding.Finding.ResolvedTarget ?? finding.Finding.Target,
                    target, StringComparison.OrdinalIgnoreCase)));
        }

        private void RunCommand(string verb, string arg)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var snapshot = _view.TextSnapshot;
            PaneTrace.Write($"cmd {verb} {(verb == "append" ? arg.Length + " chars" : arg)}");
            switch (verb)
            {
                case "settingsexport":
                    Commands.SettingsCommands.ExportFile(TryGetPackage(), arg);
                    break;
                case "settingsimportconfirm":
                case "settingsimportcancel":
                    SmokeSettingsImport(arg, verb == "settingsimportconfirm");
                    break;
                case "replacefile":
                    _view.TextBuffer.Replace(new Span(0, snapshot.Length), File.ReadAllText(arg));
                    _view.Selection.Clear();
                    break;
                case "document":
                    TryGetPackage().Dte.Documents.Item(arg).Activate();
                    break;
                case "open":
                    TryGetPackage().Dte.ItemOperations.OpenFile(arg);
                    break;
                case "execute":
                    TryGetPackage().Dte.ExecuteCommand(arg);
                    break;
                case "mode":
                    TryGetPackage().Options.GuardMode = (Options.GuardMode)int.Parse(arg);
                    break;
                case "rulesfile":
                    TryGetPackage().Options.EnvironmentRules = File.ReadAllText(arg);
                    break;
                case "commands":
                    var commands = new List<string>();
                    foreach (EnvDTE.Command command in TryGetPackage().Dte.Commands)
                    {
                        if (command.Name.StartsWith("Query.", StringComparison.OrdinalIgnoreCase))
                            commands.Add(command.Name + "\t" + command.Guid + "\t" + command.ID + "\t" +
                                string.Join(";", ((object[])command.Bindings).Select(binding => binding.ToString())));
                    }
                    File.WriteAllLines(arg, commands);
                    break;
                case "dismisswelcome":
                    foreach (EnvDTE.Window window in TryGetPackage().Dte.Windows)
                    {
                        ExtensionCheckTrace.Write("window", window.Caption);
                        if (string.Equals(window.Caption, "What's new", StringComparison.OrdinalIgnoreCase))
                            window.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
                    }
                    break;
                case "state":
                    var package = TryGetPackage();
                    var captured = Services.SqlEditorAccess.TryCapture(package.Dte);
                    ExtensionCheckTrace.Write("state", package.Dte.ActiveDocument?.FullName,
                        Services.SqlCmdModeReader.TryRead(package).ToString(), captured == null ? "none" : ExtensionCheckTrace.Hash(captured.Text),
                        _control.HeaderRedCount.Text, _control.HeaderYellowCount.Text, _control.Summary,
                        _control.WindowSettings?.Database, _control.WindowSettings?.WhenToAsk);
                    break;
                case "close":
                    foreach (EnvDTE.Document document in TryGetPackage().Dte.Documents)
                        document.Saved = true;
                    TryGetPackage().Dte.Quit();
                    break;
                case "caret":
                    _view.Selection.Clear();
                    _view.Caret.MoveTo(snapshot.GetLineFromLineNumber(int.Parse(arg) - 1).Start);
                    break;
                case "select":
                    var lines = arg.Split(' ').Select(int.Parse).ToArray();
                    var from = snapshot.GetLineFromLineNumber(lines[0] - 1).Start;
                    var to = snapshot.GetLineFromLineNumber(lines[1] - 1).End;
                    _view.Selection.Select(new SnapshotSpan(from, to), false);
                    _view.Caret.MoveTo(to);
                    break;
                case "clear":
                    _view.Selection.Clear();
                    break;
                case "append":
                    _view.TextBuffer.Insert(snapshot.Length, arg.Replace("\\n", Environment.NewLine));
                    break;
                case "appendfile":
                    _view.TextBuffer.Insert(snapshot.Length, File.ReadAllText(arg));
                    break;
                case "pick":
                    _control.ClickGroup(FindCommandGroup(arg));
                    break;
                case "activate":
                    OnGroupActivated(FindCommandGroup(arg));
                    break;
                case "collapse":
                    OnCollapseToggled();
                    break;
                case "widen":
                    OnWidthDragged(-double.Parse(arg));
                    break;
                case "page":
                    _control.SetPage(string.Equals(arg, "settings", StringComparison.OrdinalIgnoreCase));
                    break;
                case "safe":
                    _control.SafeSection.IsExpanded = !_control.SafeSection.IsExpanded;
                    break;
                case "dump":
                    PaneTrace.Write($"  header: {_control.ScopeText.Text} page={_control.ActivePage} environment={_control.EnvironmentText.Text} tag={_control.EnvironmentTag.Visibility}");
                    PaneTrace.Write($"  counts: red={_control.HeaderRedCount.Text} yellow={_control.HeaderYellowCount.Text} status={_control.HeaderStatus.Visibility} glyph={_control.HeaderGlyph.Text}");
                    PaneTrace.Write($"  summary: {_control.Summary?.Replace(Environment.NewLine, " | ")}");
                    PaneTrace.Write($"  safe: {_control.SafeHeader.Text} expanded={_control.SafeSection.IsExpanded}");
                    var settings = _control.WindowSettings;
                    if (settings != null)
                    {
                        PaneTrace.Write($"  connection: server={settings.Server} database={settings.Database} environment={settings.EnvironmentDescription}");
                        PaneTrace.Write($"  F5 guard: {settings.WhenToAsk}; {settings.TypedConfirmation}; tempdb={settings.Tempdb}");
                        foreach (var pattern in settings.SafeTables)
                            PaneTrace.Write($"  safe table: {pattern.Pattern} [{pattern.Source}]");
                        foreach (var pattern in settings.TrustedProcedures)
                            PaneTrace.Write($"  trusted procedure: {pattern.Pattern} [{pattern.Source}]");
                    }
                    foreach (var section in _groups.GroupBy(group => group.Section))
                        PaneTrace.Write($"  section: {section.Key} {section.Count()}");
                    PaneTrace.Write($"  selected: {_selected?.Key ?? "(none)"} · caret line {_view.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1}");
                    PaneTrace.Write($"  width: {_control.Width:0} collapsed={_collapsed}");
                    foreach (var group in _groups)
                        PaneTrace.Write($"  group: {group.Key} [{group.Verdict}] {group.Subtitle}; object={group.ObjectName}; line={group.Line}; expanded={group.IsExpanded}; wholeTarget={group.IsWholeTarget}; reason={group.Reason}; operations={group.OperationSummary}; lines={group.LineSummary}; statements={group.Tooltip.Replace(Environment.NewLine, " | ")}");
                    break;
                default:
                    PaneTrace.Write($"cmd unknown verb '{verb}'");
                    break;
            }
        }

        private void SmokeSettingsImport(string path, bool confirm)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var directory = Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY");
            if (string.IsNullOrEmpty(directory)) throw new InvalidOperationException("Settings smoke checks require a private test directory.");
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                var dialog = System.Windows.Application.Current.Windows.OfType<SettingsImportDialog>().FirstOrDefault();
                if (dialog == null) return;
                timer.Stop();
                try
                {
                    var import = (Options.SettingsImport)dialog.DataContext;
                    ExtensionCheckTrace.Write("settings-preview", "changes=" + import.Changes.Count, "warnings=" + import.Warnings.Count,
                        "ignored=" + import.IgnoredKeys.Count, "cancelDefault=" + dialog.CancelButton.IsDefault,
                        "warningVisible=" + dialog.WarningSection.Visibility);
                    var bitmap = new System.Windows.Media.Imaging.RenderTargetBitmap((int)dialog.ActualWidth, (int)dialog.ActualHeight,
                        96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                    bitmap.Render(dialog);
                    var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));
                    using (var stream = File.Create(path + ".preview.png")) encoder.Save(stream);
                    if (confirm && dialog.ImportButton.IsEnabled)
                        dialog.ImportButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    else dialog.DialogResult = false;
                }
                catch (Exception ex)
                {
                    ExtensionCheckTrace.Write("settings-preview-error", ex.ToString());
                    dialog.DialogResult = false;
                }
            };
            timer.Start();
            try { ExtensionCheckTrace.Write("settings-import", Commands.SettingsCommands.ImportFile(TryGetPackage(), path).ToString()); }
            finally { timer.Stop(); }
        }
    }

    internal static class ExtensionCheckTrace
    {
        internal static string Hash(string text)
        {
            using (var hash = SHA256.Create())
                return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLowerInvariant();
        }

        internal static void Write(params string[] fields)
        {
            var directory = Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY");
            if (string.IsNullOrEmpty(directory)) return;
            try
            {
                File.AppendAllText(Path.Combine(directory, "events.tsv"), DateTime.UtcNow.ToString("O") + "\t" +
                    string.Join("\t", fields.Select(field => (field ?? "").Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))) + Environment.NewLine);
            }
            catch (IOException) { }
        }
    }
}
#endif
