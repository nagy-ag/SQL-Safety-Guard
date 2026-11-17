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
    /// selection changes, and highlights only the group explicitly chosen in the pane. Never executes anything.
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

        private int _version;
        private CancellationTokenSource _analysis;
        private List<PaneGroup> _groups = new List<PaneGroup>();
        private PaneGroup _selected;
        private PaneFinding _selectedBodyFinding;
        private bool _navigatingFromPane;
        private bool _disposed;
        private long _lastEditTimestamp;
        private string _reason = "open";
        private Span? _analyzedSelection; // null = whole window
        private Span? _highlightSelection; // Execution selection when the pane group was explicitly chosen.

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
            _control.BodyFindingActivated += OnBodyFindingActivated;
            _control.BodyPreviewActivated += OnBodyPreviewActivated;
            _control.SqlLocationActivated += OnSqlLocationActivated;
            _control.ViewFilterChanged += ClearGroupSelection;
            _control.MinimumWidthChanged += OnMinimumWidthChanged;
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

            _view.TextBuffer.Changed += OnBufferChanged;
            _view.Selection.SelectionChanged += OnSelectionChanged;
            _view.Caret.PositionChanged += OnCaretMoved;
            _view.VisualElement.PreviewMouseDown += OnEditorMouseDown;
            _view.VisualElement.PreviewKeyDown += OnEditorKeyDown;
            _view.LayoutChanged += OnLayoutChanged;
            _view.GotAggregateFocus += OnGotFocus;
            InputManager.Current.PreProcessInput += OnHostInput;
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
            _analysis?.Cancel();
            _view.TextBuffer.Changed -= OnBufferChanged;
            _view.Selection.SelectionChanged -= OnSelectionChanged;
            _view.Caret.PositionChanged -= OnCaretMoved;
            _view.VisualElement.PreviewMouseDown -= OnEditorMouseDown;
            _view.VisualElement.PreviewKeyDown -= OnEditorKeyDown;
            _view.LayoutChanged -= OnLayoutChanged;
            _view.GotAggregateFocus -= OnGotFocus;
            InputManager.Current.PreProcessInput -= OnHostInput;
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

        internal static void RefreshAllSettings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var margin in Live.ToArray())
            {
                margin._options = null;
                margin.Schedule("settings import");
            }
        }

        // ---- analysis -------------------------------------------------------------------------------------

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            ClearGroupSelection();
            _lastEditTimestamp = Stopwatch.GetTimestamp();
            Schedule("edit");
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var selection = CurrentSelection();
            if (!_navigatingFromPane && (_selected != null || _selectedBodyFinding != null) && !Equals(selection, _highlightSelection))
                ClearGroupSelection();
            // Moving the caret also raises this; only a change in what F5 would run needs a new analysis.
            if (!Equals(selection, _analyzedSelection))
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
#if DEBUG
                    ExtensionCheckTrace.Write("analysis", System.IO.Path.GetFileName(_documentPath),
                        ExtensionCheckTrace.Hash(text), result.Overall.ToString());
#endif
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
                    {
                        ClearGroupSelection();
                        _control.ShowPlaceholder("Analysis failed", "See the SQL Safety Guard Output pane. The F5 guard still checks before running.");
                    }
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
            var selectedGroup = _selected;
            string selectedFinding = _selectedBodyFinding?.StateKey;
            var findings = result.Findings.Select((f, index) => new PaneFinding(f, snapshot, analyzed.Start, connection.Database, index)).ToList();

            _groups = PaneGroup.Create(findings).ToList();

            foreach (var group in _groups)
                group.Bodies = result.CodeBodies.Where(body => group.Findings.Any(f => ReferenceEquals(f.Finding, body.Definition)))
                    .Select(body => new PaneCodeBody(body, snapshot, analyzed.Start, connection.Database)).ToList();

            int firstLine = snapshot.GetLineNumberFromPosition(analyzed.Start) + 1;
            int lastLine = snapshot.GetLineNumberFromPosition(Math.Max(analyzed.Start, analyzed.End - 1)) + 1;
            var scope = isSelection ? $"Selection, lines {firstLine}–{lastLine}" : "Whole window";
            _control.ShowContext(scope, paneSettings);

            if (result.Findings.Count == 0 && result.ParseErrors.Count == 0)
            {
                bool blank = snapshot.GetText(analyzed).Trim().Length == 0;
                _control.ShowNothingToRun(blank ? "Nothing to analyze" : "Nothing that runs",
                    blank ? $"{scope} is empty." : $"{scope} has only comments.");
                ClearGroupSelection();
                Redraw();
                return;
            }

            string f5 = settings.ShouldWarn(result) ? "F5 will ask first"
                : settings.Mode == ExecutionMode.Off ? "F5 guard is off" : "F5 runs without asking";

            _control.ShowResult(result.Summary, f5, _groups);
            // Background refreshes retain a chosen highlight; edits and execution-scope changes clear it earlier.
            _selected = selectedGroup == null ? null : _groups.FirstOrDefault(g =>
                string.Equals(g.Key, selectedGroup.Key, StringComparison.OrdinalIgnoreCase) &&
                g.Findings.Select(f => f.StateKey).SequenceEqual(selectedGroup.Findings.Select(f => f.StateKey)));
            _selectedBodyFinding = selectedFinding == null ? null : _groups.SelectMany(g => g.Findings.SelectMany(f => f.DescendantsAndSelf()))
                .FirstOrDefault(f => f.StateKey == selectedFinding);
            _control.Select(_selected);
            Redraw();
        }

        private void OnEditOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try { Commands.SettingsCommands.ShowSettings(TryGetPackage()); }
            catch (Exception exception) { OutputLog.Error("Safety pane: opening options failed.", exception); }
        }

        // ---- pane → editor ------------------------------------------------------------------------------

        private void OnGroupSelected(PaneGroup group)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_collapsed) return;
            _selectedBodyFinding = null;
            _selectedPreview = null;
            _selected = group;
            _highlightSelection = CurrentSelection();
            Redraw();
            PaneTrace.Write($"select '{group?.Key}' source=pane");
            if (group != null)
            {
                var finding = group.MatchingOperations.FirstOrDefault();
                if (finding == null) { ClearGroupSelection(); return; }
                NavigateFromPane(finding, false, true);
            }
        }

        private void OnGroupActivated(PaneGroup group)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (group == null)
                return;
            OnGroupSelected(group);
            var finding = group.MatchingOperations.FirstOrDefault();
            if (finding == null) return;
            NavigateFromPane(finding, true, true);
        }

        private void NavigateFromPane(PaneFinding finding, bool focusEditor, bool execution = false)
        {
            var span = execution ? finding.ExecutionSpanIn(_view.TextSnapshot) : finding.SpanIn(_view.TextSnapshot);
            // Pane navigation is the one caret move that keeps the explicitly requested highlight.
            // Do not select text: SSMS executes the selection when one exists.
            _navigatingFromPane = true;
            try
            {
                _view.Caret.MoveTo(span.Start);
                _view.ViewScroller.EnsureSpanVisible(span, EnsureSpanVisibleOptions.ShowStart);
                if (focusEditor) ((UIElement)_view).Focus();
            }
            finally { _navigatingFromPane = false; }
            PaneTrace.Write($"navigate '{finding.GroupKey}' caretLine={_view.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1}");
        }

        private PaneCodeBody _selectedPreview;
        private void OnBodyPreviewActivated(PaneCodeBody body)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_collapsed || body == null) return;
            _control.ClearListSelection();
            _selected = null;
            _selectedBodyFinding = body.Parent;
            _selectedPreview = body;
            _highlightSelection = CurrentSelection();
            var first = body.VisibleFindings.SelectMany(f => f.MatchingOperations).FirstOrDefault() ?? body.Parent;
            if (first != null) NavigateFromPane(first, false);
            Redraw();
        }

        private void OnBodyFindingActivated(PaneFinding finding)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_collapsed || finding == null) return;
            _control.ClearListSelection();
            _selected = null;
            _selectedBodyFinding = finding;
            _selectedPreview = null;
            _highlightSelection = CurrentSelection();
            // Keep focus on the clicked pane row so the second click can expand it.
            NavigateFromPane(finding, false);
            Redraw();
            PaneTrace.Write($"body activate '{finding.Finding.Action}' caretLine={finding.EditorLine}");
        }

        private void OnSqlLocationActivated(PaneFinding finding, bool execution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            OnBodyFindingActivated(finding);
            NavigateFromPane(finding, false, execution);
        }

        // ---- editor → pane ------------------------------------------------------------------------------

        private void OnEditorMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearGroupSelection();
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e) => ClearGroupSelection();

        private void OnHostInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input is MouseButtonEventArgs mouse && mouse.RoutedEvent == Mouse.PreviewMouseDownEvent)
                ClearHighlightOutside((mouse.OriginalSource ?? mouse.MouseDevice.DirectlyOver) as DependencyObject);
        }

        private void ClearHighlightOutside(DependencyObject source)
        {
            if (_selected == null && _selectedBodyFinding == null) return;
            while (source != null)
            {
                // WPF elements in this SSMS process only; no desktop input is synthesized or intercepted.
                if (source is FrameworkElement element && (element.DataContext is PaneGroup || element.DataContext is PaneFinding || element.DataContext is PaneCodeBody))
                {
                    if (ReferenceEquals(element.DataContext, _selected) || ReferenceEquals(element.DataContext, _selectedBodyFinding) || ReferenceEquals(element.DataContext, _selectedPreview)) return;
                    break;
                }
                source = source is Visual ? VisualTreeHelper.GetParent(source) : LogicalTreeHelper.GetParent(source);
            }
            ClearGroupSelection();
        }

        private void ClearGroupSelection()
        {
            if (_disposed || (_selected == null && _selectedBodyFinding == null)) return;
            _selected = null;
            _selectedBodyFinding = null;
            _selectedPreview = null;
            _highlightSelection = null;
            _control.ClearListSelection();
            Redraw();
        }

        private void OnCaretMoved(object sender, CaretPositionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!_navigatingFromPane) ClearGroupSelection();
        }

        // ---- highlights ---------------------------------------------------------------------------------

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) => Redraw();

        private void Redraw()
        {
            if (_disposed || _view.IsClosed || _view.InLayout)
                return;
            var redrawTime = Stopwatch.StartNew();
            _layer.RemoveAllAdornments();

            var highlights = _collapsed ? Enumerable.Empty<PaneFinding>() : _selectedPreview != null
                ? _selectedPreview.VisibleFindings.SelectMany(f => f.MatchingOperations) : _selectedBodyFinding != null
                ? new[] { _selectedBodyFinding } : _selected?.MatchingOperations ?? Enumerable.Empty<PaneFinding>();
            var snapshot = _view.TextSnapshot;
            bool child = _selectedBodyFinding != null;
            var operations = highlights.ToArray();
            // A full definition is navigation context. Only its header/selected child gets a risk fill.
            foreach (var context in operations.Select(f => f.DefinitionContextIn(snapshot)).Where(s => s.HasValue)
                .Select(s => s.Value).Distinct())
            {
                if (!_view.TextViewLines.IntersectsBufferSpan(context)) continue;
                var bounds = _view.TextViewLines.GetNormalizedTextBounds(context).ToArray();
                if (bounds.Length == 0) continue;
                var rectangle = new Rect(bounds.Min(b => b.Left), bounds.Min(b => b.Top),
                    Math.Max(1, bounds.Max(b => b.Right) - bounds.Min(b => b.Left)),
                    bounds.Max(b => b.Bottom) - bounds.Min(b => b.Top));
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, context, "body-context",
                    new Shapes.Path { Data = new RectangleGeometry(rectangle), Stroke = VerdictBrushes.CodeContainer,
                        StrokeThickness = 1, IsHitTestVisible = false }, null);
            }
            var primary = operations.SelectMany(f => (child ? f.PrimarySpansIn(snapshot) : new[] { f.ExecutionSpanIn(snapshot) })
                .Select(span => new { Span = span, f.Finding.Verdict })).ToArray();
            var rectangles = new List<Rect>();
            var visibleSpans = new List<SnapshotSpan>();
            foreach (var item in primary.GroupBy(p => p.Span).Select(g => g.OrderByDescending(p => p.Verdict).First())
                .OrderBy(p => p.Verdict))
            {
                var span = item.Span;
                if (!_view.TextViewLines.IntersectsBufferSpan(span)) continue;
                var bounds = _view.TextViewLines.GetNormalizedTextBounds(span)
                    .Select(b => new Rect(b.Left, b.Top, b.Width, b.Height)).ToArray();
                if (bounds.Length == 0) continue;
                var geometry = HighlightGeometry.Union(bounds);
                rectangles.AddRange(bounds);
                visibleSpans.Add(span);
                var color = ((SolidColorBrush)VerdictBrushes.For(item.Verdict)).Color;
                var fill = new SolidColorBrush(Color.FromArgb(0x30, color.R, color.G, color.B));
                fill.Freeze();
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, "primary",
                    new Shapes.Path { Data = geometry, Fill = fill, IsHitTestVisible = false }, null);
            }

            if (visibleSpans.Count > 0)
            {
                var color = ((SolidColorBrush)(operations.All(f => f.Finding.IsContextChange) ? VerdictBrushes.Context : VerdictBrushes.For(operations.Max(f => f.Finding.Verdict)))).Color;
                var stroke = new SolidColorBrush(Color.FromArgb(0xB0, color.R, color.G, color.B));
                stroke.Freeze();
                var anchor = new SnapshotSpan(snapshot, Span.FromBounds(visibleSpans.Min(s => s.Start.Position), visibleSpans.Max(s => s.End.Position)));
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, anchor, "outline",
                    new Shapes.Path { Data = HighlightGeometry.Union(rectangles), Stroke = stroke, StrokeThickness = 1, IsHitTestVisible = false }, null);
            }

            // Supporting provenance uses a neutral outline, never a verdict-coloured declaration.
            var support = operations.SelectMany(f => child && f.SourceSpansIn(snapshot).Count > 0
                ? new[] { f.ExecutionSpanIn(snapshot) } : child ? Array.Empty<SnapshotSpan>() : f.SourceSpansIn(snapshot));
            var supportSpans = support.Distinct().Where(s => _view.TextViewLines.IntersectsBufferSpan(s)).ToArray();
            if (supportSpans.Length > 0)
            {
                var geometry = HighlightGeometry.Union(supportSpans.SelectMany(s => _view.TextViewLines.GetNormalizedTextBounds(s))
                    .Select(b => new Rect(b.Left, b.Top, b.Width, b.Height)));
                var anchor = new SnapshotSpan(snapshot, Span.FromBounds(supportSpans.Min(s => s.Start.Position), supportSpans.Max(s => s.End.Position)));
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, anchor, "support",
                    new Shapes.Path { Data = geometry, Stroke = SystemColors.GrayTextBrush, StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection { 3, 2 }, IsHitTestVisible = false }, null);
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
            _expandedWidth = Math.Max(Live.Max(m => m._control.MinimumExpandedWidth), Math.Min(700, _expandedWidth - delta));
            foreach (var margin in Live)
                margin.ApplyWidth();
            PaneTrace.Write($"width={_expandedWidth:0}");
        }

        private void ApplyWidth()
        {
            if (_collapsed) ClearGroupSelection();
            _control.SetCollapsed(_collapsed);
            _control.Width = _collapsed ? CollapsedWidth : _expandedWidth;
        }

        private void OnMinimumWidthChanged()
        {
            _expandedWidth = Math.Max(_expandedWidth, Live.Max(m => m._control.MinimumExpandedWidth));
            foreach (var margin in Live) margin.ApplyWidth();
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
