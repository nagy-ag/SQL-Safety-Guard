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
using Microsoft.VisualStudio.Text.Editor;
using SqlSafetyGuard.Analyzer;
using Shapes = System.Windows.Shapes;

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
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                CaptureNative.PreventActivation();
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
                case "showconfirm":
                case "showfailure":
                case "showlisteditor":
                case "showimportpreview":
                case "showoptions":
                case "compactsettingscheck":
                case "compactsettingsscroll":
                case "settingsappearancecheck":
                case "setcapturetheme":
                case "capture":
                case "closedialogs":
                case "preparecaptureclose":
                    RunCaptureCommand(verb, arg);
                    break;
                case "guardfailurecheck":
                    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                        throw new InvalidOperationException("Failure checks require a private test directory.");
                    SmokeGuardFailure(arg);
                    break;
                case "settingsnative":
                    SmokeNativeSettings(arg, false);
                    break;
                case "settingsnativewrite":
                    SmokeNativeSettings(arg, true);
                    break;
                case "settingsuicheck":
                    SmokeSettingsUi(arg);
                    break;
                case "settingslistcheck":
                    SmokeSettingsLists();
                    break;
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
                        _control.FilterCount(PaneVerdictFilter.Writes).ToString(), _control.FilterCount(PaneVerdictFilter.Unknown).ToString(), _control.Summary,
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
                case "showqualifiedroutine":
                    ShowCaptureDialog("qualifiedroutine", new PatternListDialog("Trusted routines", Commands.SettingsCommands.TrustedProceduresDescription,
                        "FUNCTION Host.Sales.dbo.GetTotal", true, "Host", "Sales"));
                    break;
                case "qualifiedtrustcheck":
                    SmokeQualifiedTrust();
                    break;
                case "highlightcheck":
                    SmokePaneHighlights();
                    break;
                case "automaticpanecheck":
                    SmokeAutomaticPane();
                    break;
                case "collapsedsummarycheck":
                    SmokeCollapsedSummary();
                    break;
                case "panepolishcheck":
                    SmokePanePolish();
                    break;
                case "labels":
                    _control.SetFilterLabels((PaneFilterLabels)Enum.Parse(typeof(PaneFilterLabels), arg, true));
                    break;
                case "codebodiescheck":
                    SmokeCodeBodies(false);
                    break;
                case "nestedcodebodiescheck":
                    SmokeCodeBodies(true);
                    break;
                case "procedurebodycheck":
                    SmokeProcedureBody();
                    break;
                case "inlinedynamiccheck":
                    SmokeInlineDynamic();
                    break;
                case "inlinecompactcheck":
                    var procedure = _groups.Single(g => g.Bodies.Count == 1);
                    var definition = procedure.Findings.Single(f => f.Body != null);
                    var call = procedure.Findings.Single(f => f.IsProcedureCall);
                    if (procedure.Counts.Writes != 1 || procedure.Counts.Unknown != 1 || definition.Body.Findings.Count != 3 ||
                        procedure.DisplayCounts.Total != 2 || procedure.BodyBadge.Counts.Writes != 1 ||
                        procedure.BodyBadge.Counts.Unknown != 1 || procedure.BodyBadge.Counts.Safe != 1 ||
                        !ReferenceEquals(procedure.Marker, VerdictBrushes.Red) || procedure.BodyBadge.Context != "Body" ||
                        call.Finding.Verdict != Verdict.Unknown || call.EditorLine != 20 || definition.EditorLine != 12)
                        throw new InvalidOperationException("The compact sample must keep Definition, yellow Call, and three separate preview operations.");
                    procedure.IsExpanded = definition.IsExpanded = true;
                    OnGroupSelected(procedure);
                    var compactContext = definition.DefinitionContextIn(snapshot);
                    if (!compactContext.HasValue || compactContext.Value.Start.GetContainingLine().LineNumber != 11 ||
                        compactContext.Value.End.GetContainingLine().LineNumber != 17 ||
                        !BodyContextMarkers.Any(e => e.VisualSpan == compactContext))
                        throw new InvalidOperationException("A top-level click must outline the complete definition, excluding GO and the separate EXEC.");
                    if (PrimaryMarkers.Count != 2 || PrimaryMarkers.Any(element => element.VisualSpan.Value.Start.GetContainingLine().LineNumber != element.VisualSpan.Value.End.GetContainingLine().LineNumber))
                        throw new InvalidOperationException("The procedure object must highlight its header and call without tinting the body.");
                    ClearGroupSelection();
                    ExtensionCheckTrace.Write("inline-compact", "PASS");
                    break;
                case "highlightassertclear":
                    Redraw();
                    if (_selected != null || PrimaryMarkers.Count != 0)
                        throw new InvalidOperationException("Reanalysis restored a cleared highlight.");
                    ExtensionCheckTrace.Write("pane-highlights", "reanalysis-clear", "PASS");
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
                case "filter":
                    _control.SetFilter((PaneVerdictFilter)Enum.Parse(typeof(PaneVerdictFilter), arg, true));
                    break;
                case "balancedcheck":
                    SmokeBalancedPane();
                    break;
                case "minimumwidthcheck":
                    var filterButtons = _control.FilterBar.Children.OfType<System.Windows.Controls.Primitives.ToggleButton>().ToList();
                    var filterOrigins = filterButtons.Select(button => button.TranslatePoint(new System.Windows.Point(), _control)).ToList();
                    if (_control.ActualWidth + 1 < _control.MinimumExpandedWidth ||
                        filterOrigins.Any(p => Math.Abs(p.Y - filterOrigins[0].Y) >= 1) ||
                        filterOrigins.Last().X + filterButtons.Last().ActualWidth > _control.ActualWidth + 1)
                        throw new InvalidOperationException($"Filter row clipped: actual={_control.ActualWidth}, minimum={_control.MinimumExpandedWidth}, lastEdge={filterOrigins.Last().X + filterButtons.Last().ActualWidth}.");
                    ExtensionCheckTrace.Write("pane-minimum-width", _control.ActualWidth.ToString(), "PASS");
                    break;
                case "dump":
                    PaneTrace.Write($"  header: {_control.ScopeText.Text} page={_control.ActivePage} environment={_control.EnvironmentText.Text} tag={_control.EnvironmentTag.Visibility}");
                    PaneTrace.Write($"  counts: red={_control.FilterCount(PaneVerdictFilter.Writes).ToString()} yellow={_control.FilterCount(PaneVerdictFilter.Unknown).ToString()} collapsedBadges={_control.CollapsedCounts.Children.Count}");
                    PaneTrace.Write($"  summary: {_control.Summary?.Replace(Environment.NewLine, " | ")}");
                    PaneTrace.Write($"  filter: {_control.Filter}; minimumWidth={_control.MinimumExpandedWidth}");
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
                    PaneTrace.Write($"  selected: {_selected?.Key ?? "(none)"} Â· caret line {_view.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1}");
                    PaneTrace.Write($"  width: {_control.Width:0} collapsed={_collapsed}");
                    foreach (var group in _groups)
                        PaneTrace.Write($"  group: {group.Key} [{group.Verdict}] {group.Subtitle}; object={group.ObjectName}; line={group.Line}; expanded={group.IsExpanded}; wholeTarget={group.IsWholeTarget}; reason={group.Reason}; operations={group.OperationSummary}; lines={group.LineSummary}; statements={group.Tooltip.Replace(Environment.NewLine, " | ")}");
                    break;
                default:
                    PaneTrace.Write($"cmd unknown verb '{verb}'");
                    break;
            }
        }

        private void SmokeBalancedPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Balanced-pane checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            var snapshot = _view.TextSnapshot;
            if (snapshot.LineCount < 4) throw new InvalidOperationException("Balanced-pane fixture needs four source lines.");
            PaneFinding Fixture(int index, Verdict verdict)
            {
                var line = snapshot.GetLineFromLineNumber(index);
                return new PaneFinding(new Finding(index + 1, 1, line.Start.Position, line.Length,
                    verdict.IsGreen() ? "SELECT" : verdict.IsYellow() ? "EXEC" : index == 1 ? "DELETE" : "UPDATE",
                    "dbo.MixedFixture", null, verdict, "Synthetic UI fixture; not analyzer evidence.", affectsAllRows: index == 1), snapshot, 0);
            }
            var operations = new[] { Fixture(0, Verdict.RealWrite), Fixture(1, Verdict.RealWrite), Fixture(2, Verdict.Unknown), Fixture(3, Verdict.ReadOnly) };
            var mixed = new PaneGroup("dbo.MixedFixture", operations);
            try
            {
                _control.SetFilter(PaneVerdictFilter.All);
                _control.ShowResult("Synthetic mixed UI fixture", "F5 warning unchanged", new[] { mixed });
                Check(!mixed.IsExpanded, "A fresh object must start collapsed.");
                Check(mixed.Counts.Writes == 2 && mixed.Counts.Unknown == 1 && mixed.Counts.Safe == 1, "An object must retain all three verdict counts.");
                Check(_control.GroupList.Items.Count == 1, "A mixed object must occur only once.");
                string red = _control.FilterCount(PaneVerdictFilter.Writes).ToString(), yellow = _control.FilterCount(PaneVerdictFilter.Unknown).ToString();
                foreach (var filter in new[] { PaneVerdictFilter.Writes, PaneVerdictFilter.Unknown, PaneVerdictFilter.Safe, PaneVerdictFilter.All })
                {
                    _control.SetFilter(filter);
                    Check(_selected == null && _selectedBodyFinding == null && PrimaryMarkers.Count == 0, "A filter change must clear every stale highlight.");
                    var visible = mixed.VisibleFindings;
                    Check(visible.Count == (filter == PaneVerdictFilter.All ? 4 : filter == PaneVerdictFilter.Writes ? 2 : 1), "Only matching operations may be visible.");
                    Check(mixed.Verdict == Verdict.RealWrite && mixed.Counts.Total == 4 &&
                        _control.FilterCount(PaneVerdictFilter.Writes).ToString() == red && _control.FilterCount(PaneVerdictFilter.Unknown).ToString() == yellow &&
                        _control.CollapsedCounts.Children.Count == 3, "Filtering must never turn the overall warning green or reduce totals.");
                    var focused = System.Windows.Input.Keyboard.FocusedElement;
                    _control.ClickGroup(mixed);
                    Check(_view.Caret.Position.BufferPosition == visible[0].ExecutionSpanIn(snapshot).Start, "Object navigation must use its first visible operation.");
                    if (filter == PaneVerdictFilter.Writes)
                    {
                        var firstBounds = _view.TextViewLines.GetNormalizedTextBounds(visible[0].ExecutionSpanIn(snapshot)).Last();
                        var nextBounds = _view.TextViewLines.GetNormalizedTextBounds(visible[1].ExecutionSpanIn(snapshot)).First();
                        var outline = (Shapes.Path)_layer.Elements.Single(e => Equals(e.Tag, "outline")).Adornment;
                        double left = Math.Max(firstBounds.Left, nextBounds.Left), right = Math.Min(firstBounds.Right, nextBounds.Right);
                        Check(Math.Abs(firstBounds.Bottom - nextBounds.Top) < 0.1 && right > left,
                            "The fixture must expose adjacent rendered SQL rows.");
                        Check(!outline.Data.GetWidenedPathGeometry(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1))
                            .FillContains(new System.Windows.Point((left + right) / 2, nextBounds.Top)),
                            "Consecutive highlighted rows must have no internal outline in the real editor.");
                    }
                    Check(ReferenceEquals(focused, System.Windows.Input.Keyboard.FocusedElement), "Navigation must retain pane focus for the second click.");
                    Check(visible.All(f => PrimaryMarkers.Any(e => e.VisualSpan == f.ExecutionSpanIn(snapshot))) &&
                        PrimaryMarkers.All(e => visible.Any(f => e.VisualSpan == f.ExecutionSpanIn(snapshot))), "Object highlights must include only visible operations.");
                }
                mixed.IsExpanded = true;
                _control.UpdateLayout();
                var row = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>().First(e => ReferenceEquals(e.DataContext, operations[1]));
                var header = (System.Windows.Controls.Button)row.Template.FindName("RowHeader", row);
                var arrow = (System.Windows.Controls.Button)row.Template.FindName("RowArrow", row);
                void Click(System.Windows.Controls.Button button) => button.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                Click(header);
                Check(_selectedBodyFinding == operations[1] && !operations[1].IsExpanded &&
                    _view.Caret.Position.BufferPosition == operations[1].SpanIn(snapshot).Start, "A statement click must navigate without opening its reason.");
                Check(PrimaryMarkers.Count > 0 && PrimaryMarkers.All(e => e.VisualSpan == operations[1].SpanIn(snapshot)), "A statement click must highlight only its own SQL.");
                header.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice, 0,
                    System.Windows.Input.MouseButton.Left) { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
                Check(operations[1].IsExpanded && mixed.IsExpanded, "A double click must expand only the chosen reason.");
                Click(arrow);
                Check(!operations[1].IsExpanded && mixed.IsExpanded, "The arrow must collapse its own reason without affecting its object.");
                _view.Selection.Select(new SnapshotSpan(snapshot, 1, 3), false);
                var selection = CurrentSelection();
                Click(header);
                Check(Equals(selection, CurrentSelection()), "A statement click must preserve the exact execution selection.");
                _control.SetFilter(PaneVerdictFilter.Writes);
                _control.ClickGroup(mixed);
                Check(Equals(selection, CurrentSelection()), "Filtered object navigation must preserve the exact execution selection.");
                _view.Selection.Clear();
                var green = new PaneGroup("dbo.GreenFixture", new[] { operations[3] });
                _control.ShowResult("Green fixture", "No execution", new[] { green });
                Check(_control.GroupList.Items.Count == 0 && _control.EmptyText.Visibility == System.Windows.Visibility.Visible,
                    "An empty filter must show an explicit empty state.");
                _control.SetFilter(PaneVerdictFilter.All);
                Check(_control.GroupList.Items.Count == 1, "All must recover objects hidden by an empty filter.");
                _control.ShowResult("Mixed fixture", "No execution", new[] { mixed });
                _control.SetFilter(PaneVerdictFilter.Writes);
                mixed.IsExpanded = true; operations[1].IsExpanded = true;
                var refreshed = new PaneGroup(mixed.Key, operations.Select(f => new PaneFinding(f.Finding, snapshot, 0)).ToList());
                _control.ShowResult("Refreshed fixture", "No execution", new[] { refreshed });
                Check(refreshed.IsExpanded && refreshed.Findings[1].IsExpanded && refreshed.VisibleFindings.Count == 2 &&
                    _control.Filter == PaneVerdictFilter.Writes, "Fresh analysis must preserve filter, object expansion and statement expansion.");
                ExtensionCheckTrace.Write("balanced-pane", checks.ToString(), "PASS");
            }
            finally
            {
                ClearGroupSelection();
                _control.SetFilter(PaneVerdictFilter.All);
                _control.ShowResult("Restored analysis", "Execution verdict unchanged", _groups);
            }
            StartHighlightPersistenceCheck(_groups.First(), "balanced-pane");
        }

        private void StartHighlightPersistenceCheck(PaneGroup group, string scope, PaneFinding operation = null)
        {
            var finding = operation ?? group.Findings.First();
            string key = finding.StateKey;
            int stage = 0;
            void Choose(bool operation)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (operation) OnBodyFindingActivated(finding); else OnGroupSelected(group);
                StartAnalysis(); // A passive analysis must keep the selection and use the new verdict models.
            }
            Choose(false);
            var observation = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.2) };
            observation.Tick += (sender, args) =>
            {
                try
                {
                    if (PrimaryMarkers.Count == 0 || (stage == 0 ? _selected?.Key != group.Key : _selectedBodyFinding?.StateKey != key))
                        throw new InvalidOperationException("Highlight must persist beyond the former timeout and through passive analysis.");
                    var contexts = (stage == 0 ? _selected.VisibleFindings.AsEnumerable() : new[] { _selectedBodyFinding })
                        .Select(f => f.DefinitionContextIn(_view.TextSnapshot)).Where(s => s.HasValue).Select(s => s.Value);
                    if (contexts.Any(context => !BodyContextMarkers.Any(e => e.VisualSpan == context)))
                        throw new InvalidOperationException("Full-definition context must persist through passive reanalysis.");
                    var row = new System.Windows.Controls.Border { DataContext = stage == 0 ? (object)_selected : _selectedBodyFinding };
                    ClearHighlightOutside(row);
                    if (_selected == null && _selectedBodyFinding == null)
                        throw new InvalidOperationException("Clicking the chosen row must retain its highlight.");
                    ClearHighlightOutside(new System.Windows.Controls.Border());
                    if (_selected != null || _selectedBodyFinding != null || PrimaryMarkers.Count != 0 || BodyContextMarkers.Count != 0)
                        throw new InvalidOperationException("A click elsewhere must clear every highlight.");
                    if (stage++ == 0) Choose(true);
                    else
                    {
                        observation.Stop();
                        ExtensionCheckTrace.Write(scope, "highlight-persistence", "PASS");
                    }
                }
                catch (Exception exception)
                {
                    observation.Stop();
                    ExtensionCheckTrace.Write("error", scope + "-highlight-persistence", exception.ToString());
                }
            };
            observation.Start();
        }

        private void SmokeAutomaticPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            _control.SetFilter(PaneVerdictFilter.All);
            var roots = _groups.SelectMany(g => g.Findings).ToArray();
            var definitions = roots.Where(f => f.Body != null).ToArray();
            var trigger = definitions.Single(f => f.Body.Kind == CodeBodyKind.Trigger);
            var group = _groups.Single(g => g.Findings.Contains(trigger));
            Check(group.Counts.Writes == 1 && group.BodyBadge.InsideTotal == 2, "The trigger object must show one submitted write and two purple inside operations.");
            Check(ReferenceEquals(group.Marker, VerdictBrushes.Red) && ReferenceEquals(trigger.Body.Marker, VerdictBrushes.CodeContainer), "Actual risk stays left on the object; its body uses a purple circle.");
            Check(trigger.Body.DisplayCounts.Writes == 2, "The trigger body must expose two red operations, not a second unscoped object count.");
            group.IsExpanded = trigger.IsExpanded = true;
            _control.GroupList.ScrollIntoView(group);
            _control.UpdateLayout();
            var previewRow = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>()
                .Single(e => ReferenceEquals(e.DataContext, trigger.Body));
            var header = (System.Windows.Controls.Button)previewRow.Template.FindName("RowHeader", previewRow);
            var visuals = CapturePreview.VisualDescendants(header).ToArray();
            Check(visuals.OfType<System.Windows.Shapes.Ellipse>().Any(e => ReferenceEquals(e.Fill, VerdictBrushes.CodeContainer)), "A visible circular purple marker must identify the body.");
            Check(visuals.OfType<System.Windows.Controls.TextBlock>().Any(t => t.Text == "2"), "The visible body row must show its red count.");
            header.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(_selectedPreview == trigger.Body && _view.Selection.IsEmpty && trigger.Body.IsExpanded, "Single click on the body must highlight without collapsing or selecting execution text.");
            header.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
                { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
            Check(!trigger.Body.IsExpanded, "Double click must collapse the preview independently.");
            trigger.Body.IsExpanded = true;
            var functions = definitions.Where(f => f.Body.Kind == CodeBodyKind.ScalarFunction || f.Body.Kind == CodeBodyKind.InlineTableFunction || f.Body.Kind == CodeBodyKind.TableFunction).ToArray();
            Check(functions.Length == 4 && functions.All(f => f.Body.Findings.Count > 0), "All function forms must retain visible body or unavailable-body previews.");
            _control.SetFilter(PaneVerdictFilter.Safe | PaneVerdictFilter.Inside);
            var function = functions.First(f => f.Body.Kind == CodeBodyKind.ScalarFunction && f.Body.IsComplete);
            Check(function.IsVisible && !function.MatchesFilter && function.Body.VisibleFindings.All(f => f.Finding.Verdict.IsGreen()), "A red definition must remain as an ancestor when its safe preview matches.");
            Check(_control.GroupList.Items.Cast<PaneGroup>().Any(g => g.Findings.Contains(function)), "Safe body matches must remain reachable through their containing object.");
            _control.SetFilter(PaneVerdictFilter.Writes | PaneVerdictFilter.Unknown | PaneVerdictFilter.Inside | PaneVerdictFilter.Remote);
            Check(_groups.SelectMany(g => g.MatchingOperations).All(f => !f.Finding.Verdict.IsGreen()), "Writes plus Unknown must exclude green operations at every depth.");
            Check(trigger.Body.VisibleFindings.Count == 2, "Writes filtering must retain both trigger writes.");
            _control.SetFilter(PaneVerdictFilter.Inside | PaneVerdictFilter.Context | PaneVerdictFilter.Remote);
            Check(_control.GroupList.Items.Count == 0 && _control.EmptyText.Visibility == System.Windows.Visibility.Visible, "Scope toggles alone must not invent a risk match.");
            _control.SetFilter(PaneVerdictFilter.All);
            var inside = _control.FilterInside;
            inside.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(inside, null);
            Check(!trigger.Body.IsVisible && trigger.MatchesFilter && group.BodyBadge.InsideTotal == 2, "Disabling Inside hides previews while preserving the real definition write and complete counts.");
            inside.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(inside, null);
            Check(trigger.Body.IsVisible && trigger.Body.IsExpanded, "Re-enabling Inside must restore the body's previous expansion.");
            foreach (var labels in new[] { PaneFilterLabels.Text, PaneFilterLabels.Circles, PaneFilterLabels.Automatic })
            {
                _control.SetFilterLabels(labels);
                Check(_control.Filter == PaneVerdictFilter.All, "Changing label styles must preserve every enabled filter.");
            }
            Check(_control.FilterCount(PaneVerdictFilter.Writes) == roots.Count(f => f.Finding.Verdict.IsRed()) &&
                _control.FilterCount(PaneVerdictFilter.Safe) == roots.Count(f => f.Finding.Verdict.IsGreen()), "Risk-control counts must exclude previews.");
            foreach (var definition in definitions) definition.IsExpanded = false;
            trigger.IsExpanded = true;
            _control.GroupList.ScrollIntoView(group);
            _control.UpdateLayout();
            ExtensionCheckTrace.Write("automatic-pane", checks.ToString(), "PASS");
        }

        private void SmokeCodeBodies(bool nested)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Code body checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string description) { if (!condition) throw new InvalidOperationException(description); checks++; }
            var result = SqlSafetyAnalyzer.Analyze(_view.TextSnapshot.GetText());
            Check(_control.FilterCount(PaneVerdictFilter.Writes).ToString() == _groups.SelectMany(g => g.Findings).Count(f => f.Finding.Verdict.IsRed()).ToString() &&
                _control.FilterCount(PaneVerdictFilter.Unknown).ToString() == _groups.SelectMany(g => g.Findings).Count(f => f.Finding.Verdict.IsYellow()).ToString() &&
                _groups.Sum(g => g.Findings.Count) == result.Findings.Count &&
                _control.FilterCount(PaneVerdictFilter.Writes) + _control.FilterCount(PaneVerdictFilter.Unknown) + _control.FilterCount(PaneVerdictFilter.Safe) == result.Findings.Count, "Preview effects must stay out of execution totals.");
            var definitions = _groups.SelectMany(g => g.Findings).Where(f => f.Body != null).ToList();
            foreach (var definition in definitions) definition.IsExpanded = false;
            Check(definitions.Count == (nested ? 1 : 10), "All visible code bodies must be attached exactly once.");
            System.Windows.Controls.Button Header(PaneFinding finding)
            {
                _control.GroupList.ScrollIntoView(_groups.Single(g => g.Findings.Any(f => f.DescendantsAndSelf().Contains(finding))));
                _control.UpdateLayout();
                var row = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>()
                    .Single(e => ReferenceEquals(e.DataContext, finding));
                return (System.Windows.Controls.Button)row.Template.FindName("RowHeader", row);
            }
            void Click(PaneFinding finding) => Header(finding).RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            void DoubleClick(PaneFinding finding) => Header(finding).RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(
                System.Windows.Input.Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
                { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
            foreach (var definition in definitions)
            {
                var group = _groups.Single(g => g.Findings.Contains(definition));
                group.IsExpanded = true;
                Check(!definition.IsExpanded, "New definitions and embedded commands must start compact.");
                Click(definition);
                Check(!definition.IsExpanded && _view.Selection.IsEmpty, "Single click navigates without opening details or selecting execution text.");
                DoubleClick(definition);
                Check(definition.IsExpanded, "Double click opens the existing inline preview.");
                var body = definition.Body;
                Check(body.ContextLabel.EndsWith("preview") && !string.IsNullOrEmpty(body.ContextDescription), "Every preview needs its execution context.");
                Check(ReferenceEquals(group.Marker, VerdictBrushes.For(group.Verdict)),
                    "Every object must retain its actual submitted-risk marker on the left.");
                Check(ReferenceEquals(definition.Marker, VerdictBrushes.For(definition.Finding.Verdict)),
                    "Definition and embedded-operation markers must keep the actual verdict.");
                Check(group.DisplayCounts.Total == group.Findings.Count &&
                    group.BodyBadge.Counts.Total == group.Bodies.SelectMany(b => b.Findings).Sum(f => f.DescendantsAndSelf().Count()) &&
                    body.DisplayCounts.Total == body.Findings.Sum(f => f.DescendantsAndSelf().Count()),
                    "Submitted counts and complete preview counts must remain separate.");
                Check(body.Badge.Context == (body.IsDeferred ? "Body" : "Remote") &&
                    body.Badge.Tooltip.StartsWith(body.Badge.Context + " preview:"),
                    "Body/Remote context must remain available in the count tooltip.");
                var headerVisuals = CapturePreview.VisualDescendants(Header(definition)).OfType<System.Windows.Controls.TextBlock>().ToList();
                Check(new[] { body.DisplayCounts.Total }.Where(n => n > 0)
                    .All(n => headerVisuals.Any(t => t.IsVisible && t.Text == n.ToString())) &&
                    !headerVisuals.Any(t => t.IsVisible && (t.Text == "Body" || t.Text == "Remote" || t.Text == "SQL")),
                    "Containing-operation headers must render preview counts without Body/Remote/SQL labels.");
                Check(!CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.TextBlock>()
                    .Any(t => ReferenceEquals(t.DataContext, body) && t.Text == body.ContextLabel),
                    "The body must have no extra context-heading row.");
                var context = definition.DefinitionContextIn(_view.TextSnapshot);
                Check(body.IsEmbedded ? !context.HasValue : context.HasValue && context.Value.Length > definition.ExecutionSpanIn(_view.TextSnapshot).Length,
                    "Every visible module definition needs its complete parsed extent, including forms without BEGIN/END.");
                Check(!context.HasValue || BodyContextMarkers.Any(e => e.VisualSpan == context &&
                    e.Adornment is Shapes.Path p && p.Fill == null && ReferenceEquals(p.Stroke, VerdictBrushes.CodeContainer)),
                    "Definition context must use a purple outer outline with no risk fill.");
                var operation = nested ? body.Findings.FirstOrDefault(f => f.Body != null) ?? body.Findings.First() : body.Findings.First();
                if (nested)
                {
                    while (operation.Body != null)
                    {
                        DoubleClick(operation);
                        operation = operation.Body.Findings.FirstOrDefault(f => f.Body != null) ?? operation.Body.Findings.First();
                    }
                }
                Click(operation);
                Check(_selectedBodyFinding == operation && _view.Caret.Position.BufferPosition == operation.SpanIn(_view.TextSnapshot).Start &&
                    _view.Selection.IsEmpty && !operation.IsExpanded, "Every body row uses the same single-click navigation.");
                Check(PrimaryMarkers.Count > 0 && PrimaryMarkers.All(e => operation.PrimarySpansIn(_view.TextSnapshot).Any(s => e.VisualSpan == s)),
                    "Body highlighting must use only the selected operation's proven source.");
                var enclosing = operation.DefinitionContextIn(_view.TextSnapshot);
                Check(!enclosing.HasValue || BodyContextMarkers.Any(e => e.VisualSpan == enclosing),
                    "Selecting an inner operation must retain the enclosing module outline, including nested remote/job commands.");
                DoubleClick(operation);
                Check(operation.IsExpanded, "Every body row uses the same double-click reason.");
                if (body.IsEmbedded)
                {
                    Check(operation.HasSource && operation.ExecutionLocation.StartsWith(body.Kind == CodeBodyKind.JobCommand ? "Stored at:" : "Sent at:"),
                        "Embedded SQL needs its source and the correct containing-operation label.");
                    Check(operation.SpanIn(_view.TextSnapshot) != operation.ExecutionSpanIn(_view.TextSnapshot), "Embedded SQL must navigate to the literal, independently of EXEC.");
                }
                if (nested)
                {
                    Check(operation.Finding.Action == "DELETE" && operation.HasSource, "Nested stored and remote previews must expose the original SQL string.");
                    Check(definition.DescendantsAndSelf().Select(f => f.StateKey).Distinct().Count() == definition.DescendantsAndSelf().Count(), "Nested operation identities must be independent.");
                    var expandedKeys = definition.DescendantsAndSelf().Where(f => f.IsExpanded).Select(f => f.StateKey).ToArray();
                    var refreshed = PaneGroup.Create(result.Findings.Select((f, i) => new PaneFinding(f, _view.TextSnapshot, 0, null, i))).ToList();
                    foreach (var current in refreshed)
                        current.Bodies = result.CodeBodies.Where(b => current.Findings.Any(f => ReferenceEquals(f.Finding, b.Definition)))
                            .Select(b => new PaneCodeBody(b, _view.TextSnapshot, 0, null)).ToList();
                    _control.ShowResult(result.Summary, "No execution", refreshed);
                    Check(expandedKeys.SequenceEqual(refreshed.SelectMany(g => g.Findings).SelectMany(f => f.DescendantsAndSelf())
                        .Where(f => f.IsExpanded).Select(f => f.StateKey)), "Reanalysis must retain expansion at every nested body level.");
                    _control.ShowResult(result.Summary, "No execution", _groups);
                    ExtensionCheckTrace.Write("nested-code-bodies", checks.ToString(), "PASS");
                    StartHighlightPersistenceCheck(group, "nested-code-bodies", operation);
                    return;
                }
            }
            Check(definitions.Single(f => f.Body.Kind == CodeBodyKind.Trigger).Body.ContextLabel.StartsWith("When fired"), "Trigger context must describe firing.");
            Check(definitions.Single(f => f.Body.Kind == CodeBodyKind.View).Body.ContextLabel.StartsWith("Query when read"), "View context must describe reads.");
            Check(definitions.Count(f => !f.Body.IsComplete) == 1, "The external function must show an unavailable body.");
            Check(_groups.SelectMany(g => g.Findings).Count(f => f.Finding.Action == "FUNCTION" && f.Finding.Verdict == Verdict.Unknown) == 4,
                "Uninspected function calls need separate yellow rows.");
            Check(CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Primitives.ToggleButton>()
                .Count(b => b.Tag is string tag && new[] { "Writes", "Unknown", "Safe", "Inside", "Context", "Remote" }.Contains(tag)) == 6,
                "Code body previews must reuse the single global filter bar.");
            ExtensionCheckTrace.Write("code-bodies", checks.ToString(), "PASS");
        }

        private void SmokeProcedureBody()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Procedure body checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string description)
            {
                if (!condition) throw new InvalidOperationException(description);
                checks++;
            }
            var group = _groups.Single(g => g.Bodies.Count > 0);
            var body = group.Bodies.Single();
            var definition = group.Findings.Single(f => f.Body == body);
            Check(!group.IsExpanded && !definition.IsExpanded, "New definitions must start compact.");
            Check(definition.CompactAction == "CREATE/ALTER PROCEDURE", "The containing operation must use its actual SQL verb.");
            group.IsExpanded = true;
            definition.IsExpanded = true;
            Check(_selected == null && _selectedBodyFinding == null && PrimaryMarkers.Count == 0,
                "Opening the preview must not select or highlight SQL.");
            Check(body.IsComplete && body.Counts.Writes > 0 && body.Counts.Unknown > 0 && body.Counts.Safe > 0,
                "Body preview must preserve all three verdicts.");
            Check(body.Findings.Count == SqlSafetyAnalyzer.Analyze(_view.TextSnapshot.GetText()).ProcedureBodies.Single().Analysis.Findings.Count,
                "Body operations must not be grouped or discarded.");
            Check(body.Findings.Select(f => f.Finding.StartOffset).SequenceEqual(body.Findings.Select(f => f.Finding.StartOffset).OrderBy(n => n)),
                "Body operations must remain in source order.");
            Check(_control.FilterCount(PaneVerdictFilter.Writes).ToString() == "1" && _control.FilterCount(PaneVerdictFilter.Unknown).ToString() == "0",
                "Deferred effects must not enter submitted-operation counts.");
            var repeated = body.Findings.Where(f => f.Finding.Target == "dbo.DemoOrders").ToList();
            Check(repeated.Count == 2, "Repeated UPDATEs need separate statement rows.");
            var finding = body.Findings.First(f => f.Finding.Action == "DELETE");
            Check(finding.Reason.Contains("No WHERE clause"), "The whole-target warning belongs in the expanded reason.");
            Check(body.Findings.Where(f => f.Finding.Verdict.IsGreen()).All(f => !f.IsPermanentWholeTarget),
                "Green body operations must not show permanent-write warnings.");
            _control.UpdateLayout();
            var rows = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>().ToList();
            Check(rows.Count(row => row.DataContext is PaneCodeBody) == 1, "The preview must expose one scoped body row with its own circle and risk counts.");
            Check(rows.Count(row => row.DataContext is PaneGroup) == 1, "The preview must have no nested object rows.");
            Check(CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Primitives.ToggleButton>()
                .Count(button => button.Tag is string tag && new[] { "Writes", "Unknown", "Safe", "Inside", "Context", "Remote" }.Contains(tag)) == 6,
                "Only the global filter bar may be rendered.");
            Check(body.Findings.All(f => rows.Count(row => ReferenceEquals(row.DataContext, f)) == 1),
                "Every body operation must render exactly once directly below Definition.");
            var definitionRow = rows.Single(row => ReferenceEquals(row.DataContext, definition));
            var operationRow = rows.Single(row => ReferenceEquals(row.DataContext, finding));
            System.Windows.Controls.Button Header(System.Windows.Controls.Expander row) =>
                (System.Windows.Controls.Button)row.Template.FindName("RowHeader", row);
            void Click(System.Windows.Controls.Button button) => button.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            void DoubleClick(System.Windows.Controls.Button button) => button.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(
                System.Windows.Input.Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
                { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
            Click(Header(definitionRow));
            var headerSpan = definition.SpanIn(_view.TextSnapshot);
            Check(headerSpan.Start.GetContainingLine().LineNumber == headerSpan.End.GetContainingLine().LineNumber,
                "Definition navigation must highlight only the header, never the whole body.");
            Check(PrimaryMarkers.Count > 0 && PrimaryMarkers.All(e => e.VisualSpan == headerSpan),
                "Definition highlights must use the short header span.");
            Check(BodyContextMarkers.Any(e => e.VisualSpan == definition.DefinitionContextIn(_view.TextSnapshot)),
                "The full definition must also have a context outline.");
            Click(Header(operationRow));
            Check(_selectedBodyFinding == finding && _view.Caret.Position.BufferPosition == finding.SpanIn(_view.TextSnapshot).Start && !finding.IsExpanded,
                "A body click must navigate and highlight without expanding.");
            Check(_view.Selection.IsEmpty && PrimaryMarkers.Count > 0 && PrimaryMarkers.All(e => e.VisualSpan == finding.SpanIn(_view.TextSnapshot)),
                "A body click must highlight only its SQL without selecting execution text.");
            DoubleClick(Header(operationRow));
            Check(finding.IsExpanded && definition.IsExpanded && group.IsExpanded,
                "A body double-click must open only its short reason.");
            _view.Selection.Select(new SnapshotSpan(_view.TextSnapshot, 0, 4), false);
            var selection = CurrentSelection();
            Click(Header(operationRow));
            Check(Equals(selection, CurrentSelection()), "Navigation must preserve existing execution selections exactly.");
            _view.Selection.Clear();
            _control.SetFilter(PaneVerdictFilter.Writes);
            Check(body.Findings.Count == body.Counts.Total && body.Findings.Any(f => f.Finding.Verdict.IsYellow()) && body.Findings.Any(f => f.Finding.Verdict.IsGreen()),
                "Body previews stay complete under the submitted-operation filter.");
            _control.SetFilter(PaneVerdictFilter.All);
            var refreshedResult = SqlSafetyAnalyzer.Analyze(_view.TextSnapshot.GetText());
            var refreshedFindings = refreshedResult.Findings.Select((f, i) => new PaneFinding(f, _view.TextSnapshot, 0, null, i)).ToList();
            var refreshedGroups = PaneGroup.Create(refreshedFindings);
            foreach (var refreshedGroup in refreshedGroups)
                refreshedGroup.Bodies = refreshedResult.ProcedureBodies.Where(b => refreshedGroup.Findings.Any(f => ReferenceEquals(f.Finding, b.Definition)))
                    .Select(b => new PaneCodeBody(b, _view.TextSnapshot, 0, null)).ToList();
            _control.ShowResult("Refresh", "No execution", refreshedGroups);
            var refreshedDefinition = refreshedGroups.Single(g => g.Bodies.Count > 0).Findings.Single(f => f.Body != null);
            Check(refreshedDefinition.IsExpanded && refreshedDefinition.Body.Findings.Single(f => f.StateKey == finding.StateKey).IsExpanded,
                "Reanalysis must retain Definition and individual reason expansion.");
            _control.ShowResult("Restored analysis", "No execution", _groups);
            OnEditorKeyDown(_view.VisualElement, null);
            Check(_selectedBodyFinding == null && PrimaryMarkers.Count == 0 && definition.IsExpanded,
                "Clicking elsewhere must clear highlights and preserve open details.");
            ExtensionCheckTrace.Write("procedure-body", checks.ToString(), "PASS");
            StartHighlightPersistenceCheck(group, "procedure-body", finding);
        }

        private System.Collections.Generic.IReadOnlyList<IAdornmentLayerElement> PrimaryMarkers =>
            _layer.Elements.Where(e => Equals(e.Tag, "primary")).ToArray();
        private System.Collections.Generic.IReadOnlyList<IAdornmentLayerElement> BodyContextMarkers =>
            _layer.Elements.Where(e => Equals(e.Tag, "body-context")).ToArray();

        private void SmokeInlineDynamic()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Dynamic pane checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            Check(_groups.Count == 4, "Separate EXEC blocks and the ordinary declaration must stay distinct.");
            var decoded = _groups.First(g => g.ObjectName == "Dynamic SQL");
            var runtime = _groups.Single(g => g.ObjectName == "Runtime SQL");
            Check(decoded.Findings.Count == 3 && decoded.Counts.Writes == 2 && decoded.Counts.Safe == 1,
                "The decoded block must retain repeated writes and safe operations.");
            Check(decoded.Findings.Select(f => f.StateKey).Distinct().Count() == 3,
                "Operations sharing one source span need independent expansion identities.");
            Check(runtime.Verdict == Verdict.Unknown && runtime.Findings.Count == 1 && runtime.Bodies.Count == 0,
                "Runtime SQL must stay unknown without an invented body.");
            decoded.IsExpanded = true;
            _control.UpdateLayout();
            var operation = decoded.Findings[1];
            var row = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>()
                .Single(item => ReferenceEquals(item.DataContext, operation));
            var button = (System.Windows.Controls.Button)row.Template.FindName("RowHeader", row);
            button.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(_view.Caret.Position.BufferPosition == operation.SpanIn(_view.TextSnapshot).Start && _view.Selection.IsEmpty,
                "Decoded-operation navigation must use its SQL source without selecting execution text.");
            Check(PrimaryMarkers.Count > 0 && PrimaryMarkers.All(element => element.VisualSpan == operation.SpanIn(_view.TextSnapshot)),
                "Decoded-operation highlighting must use its contributing SQL string.");
            button.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
                { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
            Check(operation.IsExpanded && !decoded.Findings[0].IsExpanded, "Repeated decoded UPDATE reasons must expand independently.");
            _control.UpdateLayout();
            Check(operation.SourceSpansIn(_view.TextSnapshot).Count > 0 && operation.SpanIn(_view.TextSnapshot) != operation.ExecutionSpanIn(_view.TextSnapshot),
                "Decoded rows need a proven string source independent of EXEC.");
            Check(_layer.Elements.Any(e => Equals(e.Tag, "support") && e.Adornment is Shapes.Path p && p.Fill == null),
                "The supporting EXEC must have a neutral outline without verdict fill.");
            Check(PrimaryMarkers.All(e => e.Adornment is Shapes.Path p && p.Stroke == null) &&
                _layer.Elements.Count(e => Equals(e.Tag, "outline")) == 1,
                "Primary fills must share one union outline rather than individual borders.");
            var sourceLink = CapturePreview.VisualDescendants(row).OfType<System.Windows.Controls.Button>()
                .Single(b => Equals(b.Content, operation.SourceLocation));
            var executionLink = CapturePreview.VisualDescendants(row).OfType<System.Windows.Controls.Button>()
                .Single(b => Equals(b.Content, operation.ExecutionLocation));
            executionLink.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(_view.Caret.Position.BufferPosition == operation.ExecutionSpanIn(_view.TextSnapshot).Start && _view.Selection.IsEmpty,
                "The execution link must navigate to EXEC without changing SQL selection.");
            sourceLink.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(_view.Caret.Position.BufferPosition == operation.SpanIn(_view.TextSnapshot).Start && operation.IsExpanded,
                "The source link must navigate back without collapsing details.");
            OnGroupSelected(decoded);
            Check(PrimaryMarkers.Count == 1 && PrimaryMarkers.Single().Adornment is Shapes.Path marker &&
                marker.Fill is System.Windows.Media.SolidColorBrush fill &&
                fill.Color.R == ((System.Windows.Media.SolidColorBrush)VerdictBrushes.Red).Color.R &&
                fill.Color.G == ((System.Windows.Media.SolidColorBrush)VerdictBrushes.Red).Color.G,
                "A shared EXEC anchor must use the worst visible verdict, not the last child's colour.");
            _control.SetFilter(PaneVerdictFilter.Safe);
            Check(decoded.VisibleFindings.Count == 1 && decoded.Counts.Writes == 2 && decoded.Verdict == Verdict.RealWrite,
                "Safe filtering must keep the full risk and counts on a mixed dynamic block.");
            OnGroupSelected(decoded);
            Check(PrimaryMarkers.Count == 1 && PrimaryMarkers.Single().Adornment is Shapes.Path safeMarker &&
                safeMarker.Fill is System.Windows.Media.SolidColorBrush safeFill &&
                safeFill.Color.G == ((System.Windows.Media.SolidColorBrush)VerdictBrushes.Green).Color.G,
                "Safe filtering must highlight only the green visible operation at its EXEC anchor.");
            _control.SetFilter(PaneVerdictFilter.All);
            ExtensionCheckTrace.Write("inline-dynamic", checks.ToString(), "PASS");
            StartHighlightPersistenceCheck(decoded, "inline-dynamic", operation);
        }

        private void SmokePaneHighlights()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Highlight checks require a private test directory.");
            var first = _groups.First(group => group.Verdict.IsRed());
            var second = _groups.First(group => group.Verdict.IsYellow());
            int checks = 0;
            void Check(bool condition, string description)
            {
                if (!condition) throw new InvalidOperationException(description);
                checks++;
            }
            bool OnlyGroup(PaneGroup group) => _selected == group && PrimaryMarkers.Count > 0 &&
                PrimaryMarkers.All(element => element.VisualSpan.HasValue &&
                    group.Findings.Any(finding => finding.ExecutionSpanIn(_view.TextSnapshot) == element.VisualSpan.Value));
            bool Clear() => _selected == null && PrimaryMarkers.Count == 0;

            Check(Clear(), "Fresh analysis must not tint red/yellow statements.");
            _view.Caret.MoveTo(new SnapshotPoint(_view.TextSnapshot, _view.TextSnapshot.Length));
            Check(Clear(), "Moving the caret must not activate a highlight.");
            _control.ClickGroup(first);
            Check(OnlyGroup(first), "Choosing red must draw only that group's statements.");
            Check(_view.Selection.IsEmpty, "Choosing a group must not select SQL text.");
            _control.ClickGroup(second);
            Check(OnlyGroup(second), "Choosing yellow must replace the red highlight.");
            _view.VisualElement.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(
                System.Windows.Input.Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
                { RoutedEvent = System.Windows.UIElement.PreviewMouseDownEvent });
            Check(Clear(), "An editor click must clear the highlight.");
            _control.ClickGroup(first);
            OnEditorKeyDown(_view.VisualElement, null);
            Check(Clear(), "An editor key must clear the highlight.");
            _control.ClickGroup(first);
            _view.Caret.MoveTo(_view.TextSnapshot.GetLineFromLineNumber(1).Start);
            Check(Clear(), "Caret navigation must clear, without selecting another group.");
            _control.ClickGroup(first);
            OnGroupActivated(first);
            Check(OnlyGroup(first) && _view.Selection.IsEmpty, "Pane activation must keep the highlight without selecting SQL.");
            _view.Selection.Select(first.Findings[0].SpanIn(_view.TextSnapshot), false);
            Check(Clear(), "Changing execution selection must clear the highlight.");
            var executionSelection = CurrentSelection();
            _control.ClickGroup(second);
            Check(OnlyGroup(second) && Equals(executionSelection, CurrentSelection()), "Choosing a group must preserve existing execution selection.");
            OnSelectionChanged(_view.Selection, EventArgs.Empty);
            Check(OnlyGroup(second), "A selection notification without an execution-scope change must keep the chosen highlight.");
            OnGroupActivated(second);
            Check(OnlyGroup(second) && Equals(executionSelection, CurrentSelection()), "Pane activation must preserve existing execution selection.");
            _control.Select(null);
            OnGroupSelected(null);
            Check(Clear(), "Pane background deselection must clear the highlight.");
            _view.Selection.Clear();
            _control.ClickGroup(first);
            SetCollapsedEverywhere(true);
            Check(Clear(), "Collapsing the pane must clear the highlight.");
            SetCollapsedEverywhere(false);
            Redraw();
            Check(Clear(), "Expanding and redrawing must not restore highlights.");
            _control.UpdateLayout();
            var objectRow = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Expander>()
                .First(row => ReferenceEquals(row.DataContext, first));
            var objectButton = (System.Windows.Controls.Button)objectRow.Template.FindName("RowHeader", objectRow);
            bool wasExpanded = first.IsExpanded;
            objectButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(first.Findings.Count == 2 && first.IsExpanded == wasExpanded && OnlyGroup(first) &&
                first.Findings.All(item => PrimaryMarkers.Any(element => element.VisualSpan == item.ExecutionSpanIn(_view.TextSnapshot))),
                "An outer row single click must highlight all its statements without toggling details.");
            objectButton.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice, 0,
                System.Windows.Input.MouseButton.Left) { RoutedEvent = System.Windows.Controls.Control.MouseDoubleClickEvent });
            Check(first.IsExpanded != wasExpanded && OnlyGroup(first), "An outer double click must toggle details without losing the whole-object highlight.");
            _control.UpdateLayout();
            var individual = first.Findings.Last();
            var statementButton = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Button>()
                .First(button => ReferenceEquals(button.DataContext, individual));
            statementButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(_selected == null && _selectedBodyFinding == individual && PrimaryMarkers.Count > 0 &&
                PrimaryMarkers.All(element => element.VisualSpan == individual.SpanIn(_view.TextSnapshot)),
                "An outer statement detail must highlight only that statement.");
            objectButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Check(OnlyGroup(first) && first.IsExpanded, "Clicking the selected outer row again must restore all its statements without collapsing.");
            _control.ClickGroup(first);
            _view.TextBuffer.Insert(_view.TextSnapshot.Length, Environment.NewLine + "-- highlight check edit");
            Check(Clear(), "A real buffer edit must clear the highlight immediately.");
            ExtensionCheckTrace.Write("pane-highlights", checks.ToString(), "PASS");
        }

        private void SmokeGuardFailure(string action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (action != "Cancel" && action != "Run anyway") throw new ArgumentException("Expected a button name.");
            bool seen = false;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (sender, args) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var dialog = System.Windows.Application.Current.Windows.OfType<GuardFailureDialog>().FirstOrDefault();
                if (dialog == null || !dialog.IsVisible) return;
                timer.Stop();
                try
                {
                    if (!dialog.CancelButton.IsDefault || !dialog.CancelButton.IsCancel || dialog.RunButton.IsDefault)
                        throw new InvalidOperationException("Unsafe failure-dialog defaults.");
                    if (string.IsNullOrEmpty(dialog.ReasonText.Text)) throw new InvalidOperationException("Missing failure reason.");
                    seen = true;
                    ExtensionCheckTrace.Write("guard-failure-box", "visible", "Cancel default/Escape", dialog.ReasonText.Text, action);
                    var button = action == "Cancel" ? dialog.CancelButton : dialog.RunButton;
                    var peer = new System.Windows.Automation.Peers.ButtonAutomationPeer(button);
                    ((System.Windows.Automation.Provider.IInvokeProvider)peer.GetPattern(
                        System.Windows.Automation.Peers.PatternInterface.Invoke)).Invoke();
                }
                catch (Exception ex)
                {
                    ExtensionCheckTrace.Write("guard-failure-box-error", ex.ToString());
                    dialog.DialogResult = false;
                }
            };
            timer.Start();
            try { new Guard.ExecuteGuard(TryGetPackage()).CheckFailureForTest(); }
            finally { timer.Stop(); }
            if (!seen) throw new InvalidOperationException("The failure dialog was not observed.");
        }

        // Service GUID from SSMS 22's SVsUnifiedSettingsManager; Debug checks only.
        [System.Runtime.InteropServices.Guid("e3684f31-344e-42ea-9047-b620fdc7ac25")]
        private sealed class NativeSettingsService { }

        private void SmokeCollapsedSummary()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Summary checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            Check(_control.FindName("HeaderCounts") == null && _control.ScopeText.Text == "Whole window",
                "The expanded header must show its scope without color badges.");
            var bits = new[] { PaneVerdictFilter.Writes, PaneVerdictFilter.Unknown, PaneVerdictFilter.Safe,
                PaneVerdictFilter.Inside, PaneVerdictFilter.Context, PaneVerdictFilter.Remote };
            Check(bits.All(bit => _control.FilterCount(bit) > 0), "The summary fixture must contain all six colors.");
            var originalFilter = _control.Filter;
            try
            {
                foreach (var filter in new[] { PaneVerdictFilter.All, PaneVerdictFilter.Writes, PaneVerdictFilter.None })
                {
                    _control.SetFilter(filter);
                    var badges = _control.CollapsedCounts.Children.OfType<System.Windows.Controls.Grid>().ToArray();
                    Check(badges.Select(b => (PaneVerdictFilter)b.Tag).SequenceEqual(bits),
                        "Every present category must keep its order even when filters hide all operations.");
                    foreach (var badge in badges)
                    {
                        var bit = (PaneVerdictFilter)badge.Tag;
                        var number = (System.Windows.Controls.TextBlock)badge.Children.OfType<System.Windows.Controls.Viewbox>().Single().Child;
                        Check(number.Text == _control.FilterCount(bit).ToString() && badge.Children.OfType<Shapes.Ellipse>().Count() == 1,
                            "Each passive circle must contain the filter's full operation count.");
                        var tip = (System.Windows.Controls.StackPanel)((System.Windows.Controls.ToolTip)badge.ToolTip).Content;
                        Check(tip.Children.OfType<System.Windows.Controls.TextBlock>().Last().Text == SafetyPaneControl.FilterMeaning(bit),
                            "Collapsed hover descriptions must match the filter's color meaning.");
                    }
                }
                _control.ShowAnalysisPending();
                Check(_control.CollapsedCounts.Children.Count == 0 && _control.CollapsedStatus.Visibility == System.Windows.Visibility.Visible,
                    "Pending analysis must clear every stale count and show a neutral marker.");
                _control.ShowResult("Summary fixture", "No SQL executed", _groups);
                Check(_control.CollapsedCounts.Children.Count == 6 && _control.CollapsedStatus.Visibility == System.Windows.Visibility.Collapsed,
                    "Fresh analysis must replace the pending marker with six numbered circles.");
            }
            finally { _control.SetFilter(originalFilter); }
            ExtensionCheckTrace.Write("collapsed-summary", checks.ToString(), "PASS");
        }

        private void SmokePanePolish()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Pane checks require a private test directory.");
            int checks = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            _control.SetFilter(PaneVerdictFilter.All);
            var roots = _groups.SelectMany(g => g.Findings).ToArray();
            var context = roots.Single(f => f.Finding.IsContextChange && !PaneOperationPresentation.IsDynamic(f.Finding));
            var dynamic = _groups.Single(g => g.Findings.Any(f => PaneOperationPresentation.IsDynamic(f.Finding)));
            Check(ReferenceEquals(context.Marker, VerdictBrushes.Context) && context.Finding.Verdict == Verdict.ReadOnly,
                "Context must be blue without changing its analyzer verdict.");
            Check(dynamic.DisplayCounts.Context == 1 && dynamic.DisplayCounts.Safe == 1,
                "A dynamic block must show blue context and green SELECT separately.");
            Check(_control.FilterCount(PaneVerdictFilter.Context) == 2 &&
                _control.FilterCount(PaneVerdictFilter.Safe) == roots.Count(f => !f.Finding.IsContextChange && f.Finding.Verdict.IsGreen()),
                "Context must be counted once, separately from Safe.");
            var gutter = _control.LineGutterWidth;
            Check(gutter.Value > 30 && gutter.Value < 50, "Four-digit lines need a measured, compact gutter.");
            _control.SetFilter(PaneVerdictFilter.Context);
            Check(_groups.SelectMany(g => g.MatchingOperations).Count() == 2 &&
                _groups.SelectMany(g => g.MatchingOperations).All(f => f.Finding.IsContextChange),
                "Context must work with Safe switched off.");
            Check(_control.LineGutterWidth == gutter, "Filtering must preserve the gutter width.");
            _control.SetFilter(PaneVerdictFilter.Safe);
            Check(!_groups.SelectMany(g => g.MatchingOperations).Any(f => f.Finding.IsContextChange),
                "Safe alone must exclude USE.");
            _control.SetFilter(PaneVerdictFilter.All);
            foreach (var group in _groups)
            {
                group.IsExpanded = true;
                foreach (var finding in group.Findings.SelectMany(f => f.DescendantsAndSelf())) finding.IsExpanded = true;
            }
            _control.UpdateLayout();
            var headers = CapturePreview.VisualDescendants(_control).OfType<System.Windows.Controls.Grid>()
                .Where(grid => grid.ColumnDefinitions.Count == 4 && grid.DataContext is object &&
                    (grid.DataContext is PaneGroup || grid.DataContext is PaneFinding || grid.DataContext is PaneCodeBody)).ToArray();
            Check(headers.Any(h => h.DataContext is PaneCodeBody), "The gutter check must include a body preview header.");
            var ends = headers.Select(h => h.TranslatePoint(new System.Windows.Point(
                h.ColumnDefinitions.Take(3).Sum(c => c.ActualWidth), 0), _control).X).ToArray();
            Check(ends.Length >= 8 && ends.Max() - ends.Min() < 1,
                "Object, operation, and preview counts must share the same gutter edge.");
            foreach (var toggle in new[] { _control.FilterWrites, _control.FilterUnknown, _control.FilterSafe,
                _control.FilterInside, _control.FilterContext, _control.FilterRemote })
            {
                var bit = (PaneVerdictFilter)Enum.Parse(typeof(PaneVerdictFilter), (string)toggle.Tag);
                var tip = (System.Windows.Controls.ToolTip)toggle.ToolTip;
                var text = ((System.Windows.Controls.StackPanel)tip.Content).Children.OfType<System.Windows.Controls.TextBlock>().ToArray();
                Check(text.Length == 2 && text[0].Text == bit + " \u00b7 " + _control.FilterCount(bit) &&
                    text[1].Text == SafetyPaneControl.FilterMeaning(bit), "Every toggle must expose its name, count, and color meaning.");
            }
            ExtensionCheckTrace.Write("pane-polish", checks.ToString(), "PASS");
        }

        private void SmokeQualifiedTrust()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Qualified trust checks require the private test directory.");
            int checks = 0, saved = 0;
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; }
            var dialog = new PatternListDialog("Trusted routines", "Test", "dbo.R", true, "Host", "Sales") { Save = text => saved++ };
            var entry = dialog.Entries.Single();
            Check(entry.NeedsQualification && dialog.CurrentText == "dbo.R", "Legacy rules must stay unchanged and inactive.");
            Check(dialog.MigrationHint.Text.StartsWith("Inactive:") && dialog.QualifyButton.Visibility == System.Windows.Visibility.Visible,
                "The existing editor must explain explicit migration.");
            dialog.QualifyButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            Check(entry.Pattern == "Host.Sales.dbo.R" && !entry.NeedsQualification && !entry.HasProblem, "Explicit migration must fill only known missing context.");
            Check(saved == 0, "Qualifying must not persist until Save.");
            dialog.RoutineType.SelectedIndex = 1;
            Check(entry.Pattern == "FUNCTION Host.Sales.dbo.R" && dialog.NameLabel.Text == "Function", "The kind selector must preserve identity and select function authorization.");
            dialog.LoadText("R");
            dialog.QualifyButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            Check(dialog.Entries.Single().Schema.Length == 0 && dialog.Entries.Single().HasProblem, "Migration must not guess dbo.");
            dialog.Close();
            var settingsControl = new CompactSettingsControl(TryGetPackage().Options, () => { }, id => { });
            Check(settingsControl.EffectMeanings.Items.Count == 3 && settingsControl.ContextMeanings.Items.Count == 3,
                "Settings must display six structured color meanings.");
            var context = _groups.Single(g => g.Findings.All(f => f.Finding.IsContextChange));
            Check(context.ObjectName == "USE Sandbox" && context.ContextVisibility == System.Windows.Visibility.Visible,
                "Database context needs a clean USE label and blue accent.");
            Check(ReferenceEquals(context.Marker, VerdictBrushes.Context) && context.Counts.Safe == 1 &&
                context.DisplayCounts.Safe == 0 && context.DisplayCounts.Context == 1,
                "Context must be blue in the pane while preserving its analyzer verdict.");
            bool expanded = context.IsExpanded;
            OnGroupSelected(context);
            Redraw();
            Check(context.IsExpanded == expanded && _selected == context && PrimaryMarkers.Count == 1,
                "A single context click must navigate and highlight without expanding.");
            var outline = (Shapes.Path)_layer.Elements.Single(e => Equals(e.Tag, "outline")).Adornment;
            var color = ((System.Windows.Media.SolidColorBrush)outline.Stroke).Color;
            var blue = ((System.Windows.Media.SolidColorBrush)VerdictBrushes.Context).Color;
            Check(color.R == blue.R && color.G == blue.G && color.B == blue.B, "The selected USE operation needs a blue context outline.");
            Check(context.Findings.Single().ContextTooltip == SafetyDictionary.Context, "The accent must expose the dictionary meaning.");
            ExtensionCheckTrace.Write("qualified-trust", checks.ToString(), "PASS");
        }

        private void SmokeSettingsLists()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Settings editor checks require a private test directory.");
            int passed = 0, allowListRuns = 0;
            foreach (int id in new[] { PackageIds.EditAllowListCommand, PackageIds.EditTrustedProceduresCommand,
                PackageIds.EditEnvironmentRulesCommand, PackageIds.EditAllowListCommand })
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                timer.Tick += (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    var dialog = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>()
                        .FirstOrDefault(window => window is PatternListDialog || window is RulesDialog);
                    if (dialog == null) return;
                    timer.Stop();
                    try
                    {
                        void Click(System.Windows.Controls.Button button) =>
                            button.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
                        if (dialog is PatternListDialog patterns)
                        {
                            Check(patterns.CancelButton.IsCancel && !patterns.CancelButton.IsDefault && !patterns.SaveButton.IsDefault,
                                "Enter must neither save nor discard; Escape cancels.");
                            if (id == PackageIds.EditTrustedProceduresCommand)
                            {
                                patterns.LoadText("unqualified");
                                Check(patterns.Entries.Single().HasProblem, "A schema-less procedure must be marked.");
                                Click(patterns.SaveButton);
                                Check(patterns.Error.Visibility == System.Windows.Visibility.Visible, "Invalid trust was not rejected.");
                                patterns.DialogResult = false;
                            }
                            else if (allowListRuns++ == 1)
                            {
                                // Second visit: unchanged text survives exactly, problems block Save, order is kept.
                                const string odd = "-- Staging\n[dbo] . [Odd.Name]\n\"sales\".Orders";
                                patterns.LoadText(odd);
                                Check(patterns.CurrentText == odd, "Unchanged entries must keep their exact text: " + patterns.CurrentText);
                                Check(patterns.NameBox.Text == "Odd.Name" && patterns.SchemaBox.Text == "dbo" && patterns.NoteBox.Text == "Staging",
                                    "The form must show the selected entry as parts.");
                                Click(patterns.AddButton);
                                patterns.DatabaseBox.Text = "Sandbox";
                                patterns.NameBox.Text = "Test_*";
                                Check(patterns.FormProblem.Visibility == System.Windows.Visibility.Visible, "A database without a schema must be explained.");
                                var stored = TryGetPackage().Options.AllowList;
                                Click(patterns.SaveButton);
                                Check(patterns.IsVisible && patterns.Error.Visibility == System.Windows.Visibility.Visible &&
                                    TryGetPackage().Options.AllowList == stored, "Save must refuse an entry with a problem.");
                                patterns.ServerBox.Text = "Host";
                                patterns.SchemaBox.Text = "dbo";
                                Click(patterns.UpButton);
                                Check(patterns.CurrentText == "Host.Sandbox.dbo.Test_*\n-- Staging\n[dbo] . [Odd.Name]\n\"sales\".Orders",
                                    "Unexpected order: " + patterns.CurrentText);
                                patterns.DialogResult = false;
                                Check(TryGetPackage().Options.AllowList == stored, "Cancel must not store edits.");
                            }
                            else
                            {
                                // Fill the form as a user would: Add, type the parts, test a name, Save.
                                patterns.LoadText("");
                                Click(patterns.AddButton);
                                patterns.ServerBox.Text = "Host";
                                patterns.DatabaseBox.Text = "Sandbox";
                                patterns.SchemaBox.Text = "dbo";
                                patterns.NameBox.Text = "SgSettings_*";
                                patterns.NoteBox.Text = "Settings check";
                                Click(patterns.AddButton);
                                patterns.ServerBox.Text = "Host";
                                patterns.DatabaseBox.Text = "Sandbox";
                                patterns.SchemaBox.Text = "dbo";
                                patterns.NameBox.Text = "SgSettings_Extra";
                                patterns.TryBox.Text = "dbo.SgSettings_Extra";
                                Check(patterns.TryResult.Text.StartsWith("Matches on connection", StringComparison.Ordinal), "Try a name did not match: " + patterns.TryResult.Text);
                                const string expected = "-- Settings check\nHost.Sandbox.dbo.SgSettings_*\nHost.Sandbox.dbo.SgSettings_Extra";
                                Check(patterns.CurrentText == expected, "Unexpected composed text: " + patterns.CurrentText);
                                Click(patterns.SaveButton);
                                Check(patterns.Error.Visibility != System.Windows.Visibility.Visible, patterns.Error.Text);
                                Check(TryGetPackage().Options.AllowList.Replace("\r\n", "\n") == expected, "The saved allow-list differs from the editor.");
                            }
                        }
                        else if (dialog is RulesDialog rules)
                        {
                            var before = TryGetPackage().Options.EnvironmentRules;
                            rules.LoadText("");
                            Click(rules.AddButton);
                            rules.ServerBox.Text = "SGCHECK-*";
                            rules.LabelBox.Text = "Check";
                            rules.ModeRed.IsChecked = true;
                            rules.SafeTablesBox.Text = "Host.Sandbox.dbo.Check_*";
                            Check(rules.CurrentText == "SGCHECK-* |  | Check | Red | false | Host.Sandbox.dbo.Check_*", "Unexpected rule text: " + rules.CurrentText);
                            Check(!rules.Rules.Single().HasProblem, "A complete rule must be valid.");
                            rules.DialogResult = false;
                            Check(TryGetPackage().Options.EnvironmentRules == before, "Cancel must not store rule edits.");
                        }
                        passed++;
                        ExtensionCheckTrace.Write("settings-list", id.ToString(), "PASS");
                    }
                    catch (Exception ex)
                    {
                        ExtensionCheckTrace.Write("settings-list-error", ex.ToString());
                        if (dialog.IsVisible) ((Microsoft.VisualStudio.PlatformUI.DialogWindow)dialog).DialogResult = false;
                    }
                };
                timer.Start();
                object input = null, output = null;
                try { TryGetPackage().Dte.Commands.Raise(PackageGuids.CommandSetString, id, ref input, ref output); }
                finally { timer.Stop(); }
            }
            if (passed != 4) throw new InvalidOperationException("Settings editor checks failed: " + passed + " of 4 passed.");
            ExtensionCheckTrace.Write("settings-lists", passed.ToString(), "PASS");
        }

        private void SmokeSettingsUi(string path)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("UI checks require a private test directory.");
            var lines = new List<string>();
            int image = 0;
            foreach (System.Windows.PresentationSource source in System.Windows.PresentationSource.CurrentSources)
            {
                if (!(source.RootVisual is System.Windows.FrameworkElement root)) continue;
                var texts = new List<string>();
                Visit(root);
                lines.AddRange(texts);
                if (!texts.Any(text => text.Contains("F5 warnings")) || root.ActualWidth < 300 || root.ActualHeight < 200) continue;
                var bitmap = new System.Windows.Media.Imaging.RenderTargetBitmap((int)root.ActualWidth, (int)root.ActualHeight,
                    96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                bitmap.Render(root);
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));
                using (var stream = File.Create(path + "." + image++ + ".png")) encoder.Save(stream);

                void Visit(System.Windows.DependencyObject element)
                {
                    if (element is System.Windows.Controls.TextBlock text && text.IsVisible && !string.IsNullOrEmpty(text.Text))
                        texts.Add("text: " + text.Text);
                    if (element is System.Windows.Controls.Button button && button.IsVisible)
                        texts.Add("button: " + button.Content);
                    for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(element); i++)
                        Visit(System.Windows.Media.VisualTreeHelper.GetChild(element, i));
                }
            }
            File.WriteAllLines(path, lines);
        }

        private void SmokeNativeSettings(string path, bool write)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Native settings checks require a private test directory.");
            var manager = (Microsoft.VisualStudio.Utilities.UnifiedSettings.ISettingsManager)
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(NativeSettingsService));
            if (write)
            {
                var writer = manager.GetWriter("SQL Safety Guard native settings check");
                foreach (var property in Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path)).Properties())
                {
                    if (!property.Name.StartsWith("sqlSafetyGuard.", StringComparison.Ordinal))
                        throw new InvalidOperationException("Only SQL Safety Guard test settings may be changed.");
                    var result = property.Value.Type == Newtonsoft.Json.Linq.JTokenType.Boolean
                        ? writer.EnqueueChange(property.Name, (bool)property.Value)
                        : writer.EnqueueChange(property.Name, (string)property.Value);
                    ExtensionCheckTrace.Write("native-write", property.Name, result.Outcome.ToString());
                }
                ExtensionCheckTrace.Write("native-commit", writer.RequestCommit("Native settings smoke check").Outcome.ToString());
            }
            else
            {
                var reader = manager.GetReader();
                var result = new Newtonsoft.Json.Linq.JObject();
                foreach (var name in new[] { "guardMode", "allowList", "trustedProcedures", "environmentRules" })
                {
                    var value = reader.GetValue<string>("sqlSafetyGuard." + name);
                    result[name] = value.Outcome + ": " + value.Value;
                }
                foreach (var name in new[] { "showConnection", "tempdbIsTemp" })
                {
                    var value = reader.GetValue<bool>("sqlSafetyGuard." + name);
                    result[name] = value.Outcome + ": " + value.Value;
                }
                var package = TryGetPackage();
                result["activeMode"] = package.Options.GuardMode.ToString();
                result["activeAllowList"] = package.Options.AllowList;
                result["activeTrustedProcedures"] = package.Options.TrustedProcedures;
                result["activeEnvironmentRules"] = package.Options.EnvironmentRules;
                result["activeShowConnection"] = package.Options.ShowServerAndDatabase;
                result["activeTempdbIsTemp"] = package.Options.TempdbIsTemp;
                package.RefreshHealth();
                result["toolbar"] = package.ToolbarText;
                result["health"] = package.HealthText;
                File.WriteAllText(path, result.ToString());
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
