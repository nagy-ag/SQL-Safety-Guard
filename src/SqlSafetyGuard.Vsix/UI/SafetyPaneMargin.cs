using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Services;
using Shapes = System.Windows.Shapes;
using Task = System.Threading.Tasks.Task;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// The safety pane inside one query window. Re-analyzes (off the UI thread) a moment after the text or the
    /// selection changes, highlights statements in the editor, and follows the caret. Never executes anything.
    /// </summary>
    internal sealed partial class SafetyPaneMargin : IWpfTextViewMargin
    {
        public const string MarginName = "SqlSafetyGuardPane";
        public const string HighlightLayerName = "SqlSafetyGuardHighlight";

        private const double CollapsedWidth = 28;

        // Shared by all query windows for the session, so a resize or collapse sticks.
        private static double _expandedWidth = 300;
        private static bool _collapsed;
        private static readonly List<SafetyPaneMargin> Live = new List<SafetyPaneMargin>();

        private readonly IWpfTextView _view;
        private readonly string _documentPath;
        private readonly IAdornmentLayer _layer;
        private readonly SafetyPaneControl _control;
        private readonly DispatcherTimer _debounce;
        private readonly DispatcherTimer _clearSelection;

        private int _version;
        private CancellationTokenSource _analysis;
        private List<PaneGroup> _groups = new List<PaneGroup>();
        private PaneGroup _selected;
        private bool _clearingFromEditorBackground;
        private bool _disposed;
        private long _lastEditTimestamp;
        private string _reason = "open";
        private Span? _analyzedSelection; // null = whole window

        // Settings for this window's connection. SSMS only exposes the connection of the active query window, so
        // they are refreshed while this window is active and reused while it isn't.
        private ConnectionContext _connection;
        private GuardSettings _settings;
        private AnalyzerOptions _options;
        private PaneSettings _paneSettings;

        public SafetyPaneMargin(IWpfTextView view, string documentPath)
        {
            _view = view;
            _documentPath = documentPath;
            _layer = view.GetAdornmentLayer(HighlightLayerName);
            Live.Add(this);

            _control = new SafetyPaneControl();
            _control.GroupSelected += OnGroupSelected;
            _control.GroupActivated += OnGroupActivated;
            _control.CollapseToggled += OnCollapseToggled;
            _control.WidthDragged += OnWidthDragged;
            _control.EditOptions += OnEditOptions;
            ApplyWidth();

            _debounce = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(250) };
            _debounce.Tick += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                _debounce.Stop();
                StartAnalysis();
            };

            _clearSelection = new DispatcherTimer(DispatcherPriority.Input);
            _clearSelection.Tick += (sender, args) =>
            {
                _clearSelection.Stop();
                _clearingFromEditorBackground = false;
                if (!_disposed) ClearGroupSelection();
            };

            _view.TextBuffer.Changed += OnBufferChanged;
            _view.Selection.SelectionChanged += OnSelectionChanged;
            _view.Caret.PositionChanged += OnCaretMoved;
            _view.VisualElement.PreviewMouseLeftButtonDown += OnEditorMouseDown;
            _view.LayoutChanged += OnLayoutChanged;
            _view.GotAggregateFocus += OnGotFocus;
            _view.Closed += (s, e) => Dispose();

            _control.ShowPlaceholder("Analyzing…", "Nothing is executed.");
            StartAnalysis();

            // A new query window is created before SSMS makes it the active one, so its server and database can't
            // be read yet. Look again once the window has settled.
            var settle = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            settle.Tick += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                settle.Stop();
                if (!_disposed && (_connection == null || _connection.Server == null))
                    Schedule("settle");
            };
            settle.Start();
            StartTestChannel();
        }

        /// <summary>Debug builds: see SafetyPaneMargin.TestChannel.cs.</summary>
        partial void StartTestChannel();

        public FrameworkElement VisualElement => _control;

        public double MarginSize => _control.ActualWidth;

        public bool Enabled => true;

        public ITextViewMargin GetTextViewMargin(string marginName) =>
            string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Live.Remove(this);
            _debounce.Stop();
            _clearSelection.Stop();
            _analysis?.Cancel();
            _view.TextBuffer.Changed -= OnBufferChanged;
            _view.Selection.SelectionChanged -= OnSelectionChanged;
            _view.Caret.PositionChanged -= OnCaretMoved;
            _view.VisualElement.PreviewMouseLeftButtonDown -= OnEditorMouseDown;
            _view.LayoutChanged -= OnLayoutChanged;
            _view.GotAggregateFocus -= OnGotFocus;
        }

        /// <summary>
        /// Analyze Query Safety (Ctrl+Shift+F5): expands the pane of the given query window and re-analyzes it now.
        /// Returns false when that window has no pane.
        /// </summary>
        internal static bool ShowAndRefresh(string documentPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var margin = Live.FirstOrDefault(m => string.Equals(m._documentPath, documentPath, StringComparison.OrdinalIgnoreCase));
            if (margin == null)
                return false;
            if (_collapsed)
                SetCollapsedEverywhere(false);
            margin._reason = "command";
            margin.StartAnalysis();
            return true;
        }

        // ---- analysis -------------------------------------------------------------------------------------

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            _lastEditTimestamp = Stopwatch.GetTimestamp();
            Schedule("edit");
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            // Moving the caret also raises this; only a change in what F5 would run needs a new analysis.
            if (!Equals(CurrentSelection(), _analyzedSelection))
                Schedule("selection");
        }

        private Span? CurrentSelection()
        {
            var selection = _view.Selection;
            return selection.IsEmpty || selection.SelectedSpans.Count != 1 ? (Span?)null : selection.SelectedSpans[0].Span;
        }

        private void OnGotFocus(object sender, EventArgs e) => Schedule("focus");

        private void Schedule(string reason)
        {
            if (_disposed)
                return;
            _reason = reason;
            ++_version;
            _analysis?.Cancel();
            _control.ShowAnalysisPending();
            _debounce.Stop();
            _debounce.Start();
        }

        private void StartAnalysis()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_disposed)
                return;
            _debounce.Stop();
            _control.ShowAnalysisPending();

            // Same rule as Query.Execute: the selection if there is one, otherwise the whole window.
            var snapshot = _view.TextSnapshot;
            _analyzedSelection = CurrentSelection();
            Span span = _analyzedSelection ?? new Span(0, snapshot.Length);
            bool isSelection = _analyzedSelection.HasValue && span.Length != snapshot.Length;

            var uiTime = Stopwatch.StartNew();
            var package = TryGetPackage();
            if (package == null || !RefreshSettings())
            {
                _control.ShowPlaceholder("Starting…", "SQL Safety Guard is still loading.");
                Schedule("package");
                return;
            }

            string text = snapshot.GetText(span);
            int version = ++_version;
            long editTimestamp = _lastEditTimestamp;
            var options = _options;
            var settings = _settings;
            var connection = _connection;
            var paneSettings = _paneSettings;

            _analysis?.Cancel();
            _analysis = new CancellationTokenSource();
            var cancellation = _analysis.Token;
            PaneTrace.Write($"analyze #{version} reason={_reason} chars={text.Length} selection={isSelection} " +
                            $"server={(connection.Server == null ? "unknown" : "known")} uiThreadMs={uiTime.ElapsedMilliseconds}");

            _ = package.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await Task.Run(() => SqlSafetyAnalyzer.Analyze(text, options, cancellation), cancellation);
                    long elapsed = stopwatch.ElapsedMilliseconds;
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (_disposed || version != _version)
                        return;
                    if (elapsed > 1000)
                        OutputLog.Info($"Safety pane: analyzed {text.Length:N0} characters in {elapsed:N0} ms.");
                    var applyTime = Stopwatch.StartNew();
                    Apply(snapshot, span, isSelection, result, settings, connection, paneSettings);
                    var sinceEdit = editTimestamp == 0 ? "-" :
                        ((Stopwatch.GetTimestamp() - editTimestamp) * 1000 / Stopwatch.Frequency).ToString();
                    if (editTimestamp == _lastEditTimestamp)
                        _lastEditTimestamp = 0; // Measured; later analyses weren't caused by this edit.
                    PaneTrace.Write($"done #{version} analyzeMs={elapsed} applyMs={applyTime.ElapsedMilliseconds} editToScreenMs={sinceEdit} " +
                                    $"findings={result.Findings.Count} groups={_groups.Count} overall={result.Overall}");
                }
                catch (OperationCanceledException)
                {
                    // Superseded by a newer analysis, or the window closed.
                }
                catch (Exception ex)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    OutputLog.Error("Safety pane: analysis failed.", ex);
                    if (!_disposed && version == _version)
                        _control.ShowPlaceholder("Analysis failed", "See the SQL Safety Guard Output pane. The F5 guard still checks before running.");
                }
            });
        }

        /// <summary>Reads the connection and the settings that apply to it. False while the package isn't loaded yet.</summary>
        private bool RefreshSettings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var package = TryGetPackage();
            if (package == null)
                return _options != null;

            try
            {
                if (_options == null || IsActiveDocument(package))
                {
                    // Not active and never read: unknown connection, so the global settings apply (as for F5).
                    var connection = IsActiveDocument(package) ? ConnectionInfoReader.TryRead(package) : new ConnectionContext();
                    var settings = package.ResolveSettings(connection, out var rules);
                    _options = package.CreateAnalyzerOptions(connection, settings, rules);
                    _settings = settings;
                    _connection = connection;
                    _paneSettings = new PaneSettings(connection.Server, connection.Database, settings, _options,
                        new AllowList(AllowList.SplitLines(package.Options.AllowList)).Patterns.Count,
                        new AllowList(AllowList.SplitLines(package.Options.TrustedProcedures), procedures: true).Patterns.Count);
                }
            }
            catch (Exception ex)
            {
                OutputLog.Error("Safety pane: reading the settings failed.", ex);
                if (_options == null)
                    return false;
            }

            return true;
        }

        private void Apply(ITextSnapshot snapshot, Span analyzed, bool isSelection, AnalysisResult result, GuardSettings settings,
            ConnectionContext connection, PaneSettings paneSettings)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var findings = result.Findings.Select(f => new PaneFinding(f, snapshot, analyzed.Start, connection.Database)).ToList();

            string selectedKey = _selected?.Key;
            _groups = findings
                .GroupBy(f => f.GroupKey, StringComparer.OrdinalIgnoreCase)
                .Select(g => new PaneGroup(g.Key, g.ToList()))
                .OrderByDescending(g => g.Verdict)
                .ThenBy(g => g.FirstOffset)
                .ToList();

            int firstLine = snapshot.GetLineNumberFromPosition(analyzed.Start) + 1;
            int lastLine = snapshot.GetLineNumberFromPosition(Math.Max(analyzed.Start, analyzed.End - 1)) + 1;
            var scope = isSelection ? $"Selection, lines {firstLine}–{lastLine}" : "Whole window";
            _control.ShowContext(scope, paneSettings);

            if (result.Findings.Count == 0 && result.ParseErrors.Count == 0)
            {
                bool blank = snapshot.GetText(analyzed).Trim().Length == 0;
                _control.ShowNothingToRun(blank ? "Nothing to analyze" : "Nothing that runs",
                    blank ? $"{scope} is empty." : $"{scope} has only comments.");
                _selected = null;
                Redraw();
                return;
            }

            string f5 = settings.Mode == ExecutionMode.Off ? "F5 guard is off"
                : !settings.ShouldWarn(result) ? "F5 runs without asking"
                : settings.NeedsTypedConfirmation(result) ? "F5 asks you to type the database name"
                : "F5 will ask first";

            _control.ShowResult(result.Summary, f5, _groups);

            _selected = selectedKey == null ? null : _groups.FirstOrDefault(g => string.Equals(g.Key, selectedKey, StringComparison.OrdinalIgnoreCase));
            _control.Select(_selected);
            Redraw();
        }

        private void OnEditOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try { TryGetPackage()?.ShowOptionPage(typeof(Options.GeneralOptionsPage)); }
            catch (Exception exception) { OutputLog.Error("Safety pane: opening options failed.", exception); }
        }

        // ---- pane → editor ------------------------------------------------------------------------------

        private void OnGroupSelected(PaneGroup group)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _selected = group;
            Redraw();
            PaneTrace.Write($"select '{group?.Key}' source=pane");
            if (group != null)
            {
                var span = group.Findings[0].SpanIn(_view.TextSnapshot);
                _view.ViewScroller.EnsureSpanVisible(span, EnsureSpanVisibleOptions.ShowStart);
            }
        }

        private void OnGroupActivated(PaneGroup group)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (group == null)
                return;
            var span = group.Findings[0].SpanIn(_view.TextSnapshot);
            _view.Caret.MoveTo(span.Start);
            _view.ViewScroller.EnsureSpanVisible(span, EnsureSpanVisibleOptions.ShowStart);
            ((UIElement)_view).Focus();
            PaneTrace.Write($"activate '{group.Key}' caretLine={_view.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1}");
        }

        // ---- editor → pane ------------------------------------------------------------------------------

        private void OnEditorMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_disposed || _view.IsClosed || _view.InLayout) return;
            var location = e.GetPosition(_view.VisualElement);
            var line = _view.TextViewLines.GetTextViewLineContainingYCoordinate(location.Y + _view.ViewportTop);
            var position = line?.GetBufferPositionFromXCoordinate(location.X + _view.ViewportLeft);
            if (position.HasValue && position.Value.Position < position.Value.Snapshot.Length &&
                !char.IsWhiteSpace(position.Value.Snapshot[position.Value.Position])) return;
            _clearingFromEditorBackground = true;
            ClearGroupSelection();
            _clearSelection.Start();
        }

        private void ClearGroupSelection()
        {
            _selected = null;
            _control.Select(null);
            Redraw();
        }

        private void OnCaretMoved(object sender, CaretPositionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_clearingFromEditorBackground) return;
            var caret = e.NewPosition.BufferPosition;
            var hit = _groups.FirstOrDefault(g => g.Findings.Any(f => f.SpanIn(caret.Snapshot).IntersectsWith(new Span(caret.Position, 0)) ||
                                                                       f.SpanIn(caret.Snapshot).Contains(caret.Position)));
            if (hit == _selected)
                return;

            // Follow the caret: select and highlight, but don't scroll (the user is already looking there).
            _selected = hit;
            _control.Select(hit);
            Redraw();
            PaneTrace.Write($"select '{hit?.Key}' source=caret line={caret.GetContainingLine().LineNumber + 1}");
        }

        // ---- highlights ---------------------------------------------------------------------------------

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) => Redraw();

        private void Redraw()
        {
            if (_disposed || _view.IsClosed || _view.InLayout)
                return;
            var redrawTime = Stopwatch.StartNew();
            _layer.RemoveAllAdornments();

            foreach (var group in _groups.Where(group => !group.Verdict.IsGreen() || group == _selected))
            {
                bool selected = group == _selected;
                var color = ((SolidColorBrush)VerdictBrushes.For(group.Verdict)).Color;
                var fill = new SolidColorBrush(Color.FromArgb(selected ? (byte)0x48 : (byte)0x16, color.R, color.G, color.B));
                fill.Freeze();
                Pen pen = null;
                if (selected)
                {
                    pen = new Pen(new SolidColorBrush(Color.FromArgb(0xB0, color.R, color.G, color.B)), 1);
                    pen.Freeze();
                }

                foreach (var finding in group.Findings)
                {
                    var span = finding.SpanIn(_view.TextSnapshot);
                    if (!_view.TextViewLines.IntersectsBufferSpan(span))
                        continue;
                    var geometry = _view.TextViewLines.GetMarkerGeometry(span);
                    if (geometry == null)
                        continue;
                    var path = new Shapes.Path { Data = geometry, Fill = fill, Stroke = pen?.Brush, StrokeThickness = pen?.Thickness ?? 0 };
                    _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, path, null);
                }
            }

            if (redrawTime.ElapsedMilliseconds > 8)
                PaneTrace.Write($"redraw slow: {redrawTime.ElapsedMilliseconds} ms for {_groups.Sum(g => g.Findings.Count)} highlights");
        }

        // ---- size ---------------------------------------------------------------------------------------

        private void OnCollapseToggled()
        {
            SetCollapsedEverywhere(!_collapsed);
            PaneTrace.Write($"collapsed={_collapsed}");
        }

        private static void SetCollapsedEverywhere(bool collapsed)
        {
            _collapsed = collapsed;
            foreach (var margin in Live)
                margin.ApplyWidth();
        }

        private void OnWidthDragged(double delta)
        {
            if (_collapsed)
                return;
            _expandedWidth = Math.Max(240, Math.Min(700, _expandedWidth - delta));
            foreach (var margin in Live)
                margin.ApplyWidth();
            PaneTrace.Write($"width={_expandedWidth:0}");
        }

        private void ApplyWidth()
        {
            _control.SetCollapsed(_collapsed);
            _control.Width = _collapsed ? CollapsedWidth : _expandedWidth;
        }

        /// <summary>True when SSMS's active document is this query window.</summary>
        private bool IsActiveDocument(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var active = package.Dte?.ActiveDocument;
                return active != null && _documentPath != null &&
                       string.Equals(active.FullName, _documentPath, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static SqlSafetyGuardPackage TryGetPackage()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!(ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) is IVsShell shell))
                return null;
            var guid = new Guid(PackageGuids.PackageString);
            return shell.IsPackageLoaded(ref guid, out var package) == VSConstants.S_OK ? package as SqlSafetyGuardPackage : null;
        }
    }
}
