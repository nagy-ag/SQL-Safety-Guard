#if DEBUG
using System;
using System.IO;
using System.Linq;
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
        private static readonly string CommandFile = Path.Combine(Path.GetTempPath(), "SqlSafetyGuard-pane-cmd.txt");

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
                }
                catch (Exception ex)
                {
                    PaneTrace.Write($"cmd {verb} FAILED: {ex.GetType().Name}: {ex.Message}");
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
    }
}
#endif
