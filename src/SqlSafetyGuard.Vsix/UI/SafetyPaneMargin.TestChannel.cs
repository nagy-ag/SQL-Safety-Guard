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
                    _control.ClickGroup(_groups.First(g => string.Equals(g.Key, arg, StringComparison.OrdinalIgnoreCase)));
                    break;
                case "activate":
                    OnGroupActivated(_groups.First(g => string.Equals(g.Key, arg, StringComparison.OrdinalIgnoreCase)));
                    break;
                case "collapse":
                    OnCollapseToggled();
                    break;
                case "widen":
                    OnWidthDragged(-double.Parse(arg));
                    break;
                case "dump":
                    PaneTrace.Write($"  headline: {_control.Headline.Text}");
                    PaneTrace.Write($"  detail: {_control.SubText.Text}");
                    PaneTrace.Write($"  connection: {_control.ConnectionText.Text}");
                    PaneTrace.Write($"  selected: {_selected?.Key ?? "(none)"} · caret line {_view.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1}");
                    PaneTrace.Write($"  width: {_control.Width:0} collapsed={_collapsed}");
                    foreach (var group in _groups.Take(6))
                        PaneTrace.Write($"  group: {group.Key} [{group.Verdict}] {group.Subtitle}");
                    break;
                default:
                    PaneTrace.Write($"cmd unknown verb '{verb}'");
                    break;
            }
        }
    }
}
#endif
