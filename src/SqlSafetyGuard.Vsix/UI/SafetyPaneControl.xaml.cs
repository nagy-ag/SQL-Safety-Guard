using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    public partial class SafetyPaneControl : UserControl
    {
        private bool _settingSelection;
        private bool _analysisPending = true;
        private IReadOnlyList<PaneGroup> _groups = Array.Empty<PaneGroup>();
        internal PaneVerdictFilter Filter { get; private set; } = PaneVerdictFilter.All;
        internal PaneFilterLabels FilterLabels { get; private set; } = PaneFilterLabels.Automatic;
        internal bool CompactFilterLabels { get; private set; }
        private readonly Dictionary<PaneVerdictFilter, int> _filterCounts = new Dictionary<PaneVerdictFilter, int>();
        internal event Action ViewFilterChanged;
        internal event Action MinimumWidthChanged;
        internal double MinimumExpandedWidth { get; private set; } = 240;
        private readonly Brush _slate = new SolidColorBrush(Color.FromRgb(0x5c, 0x6b, 0x7a));

        public SafetyPaneControl()
        {
            InitializeComponent();
            SizeChanged += (sender, args) => RefreshFilterLayout();
            AnalysisButton.ContextMenu = FilterBar.ContextMenu;

        }

        internal event Action<PaneGroup> GroupSelected;
        internal event Action<PaneGroup> GroupActivated;
        internal event Action<PaneFinding> BodyFindingActivated;
        internal event Action<PaneCodeBody> BodyPreviewActivated;
        internal event Action<PaneFinding, bool> SqlLocationActivated;
        private void OnSqlSourceClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is PaneFinding finding) SqlLocationActivated?.Invoke(finding, false);
            e.Handled = true;
        }
        private void OnSqlExecutionClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is PaneFinding finding) SqlLocationActivated?.Invoke(finding, true);
            e.Handled = true;
        }
        internal event Action CollapseToggled;
        internal event Action<double> WidthDragged;
        internal event Action EditOptions;
        internal PaneSettings WindowSettings { get; private set; }
        internal string Summary { get; private set; }
        internal string ActivePage => SettingsPage.Visibility == Visibility.Visible ? "Settings" : "Analysis";

        internal void ShowContext(string scope, PaneSettings settings)
        {
            ScopeText.Text = scope;
            ScopeText.ToolTip = scope;
            WindowSettings = settings;
            SettingsPage.DataContext = settings;
            string label = settings.EnvironmentLabel;
            var visibility = string.IsNullOrEmpty(label) ? Visibility.Collapsed : Visibility.Visible;
            EnvironmentTag.Visibility = EnvironmentLine.Visibility = visibility;
            EnvironmentText.Text = label ?? string.Empty;
            var color = label != null && label.IndexOf("PROD", StringComparison.OrdinalIgnoreCase) >= 0 ? VerdictBrushes.Red : _slate;
            EnvironmentTag.Background = EnvironmentLine.Background = color;
            EnvironmentTag.ToolTip = settings.EnvironmentDescription;
        }

        internal void ShowPlaceholder(string headline, string detail)
        {
            ShowCollapsedPending();
            ShowSummary(headline, detail);
            _settingSelection = true;
            try { _groups = Array.Empty<PaneGroup>(); GroupList.ItemsSource = null; }
            finally { _settingSelection = false; }
            FilterBar.Visibility = Visibility.Collapsed;
            EmptyText.Text = headline + ". " + detail;
            EmptyText.Visibility = Visibility.Visible;
        }

        internal void ShowResult(string summary, string f5, IReadOnlyList<PaneGroup> groups)
        {
            _analysisPending = false;
            ShowSummary(summary, f5);
            _settingSelection = true;
            try
            {
                var previous = _groups.ToDictionary(g => g.Key, StringComparer.OrdinalIgnoreCase);
                foreach (var group in groups)
                {
                    if (previous.TryGetValue(group.Key, out var old)) RestoreExpansion(old, group);
                }
                _groups = groups;
                MeasureLineGutter();
                ApplyViewFilter();
            }
            finally { _settingSelection = false; }
            FilterBar.Visibility = Visibility.Visible;
        }

        private static void RestoreExpansion(PaneGroup old, PaneGroup current)
        {
            current.IsExpanded = old.IsExpanded;
            RestoreFindings(old.Findings, current.Findings);
            RestoreScopeRows(old.ScopeRows, current.ScopeRows);
        }

        private static void RestoreScopeRows(PaneScopeRows old, PaneScopeRows current)
        {
            if (old == null || current == null) return;
            var previous = old.Groups.ToDictionary(g => g.Key, StringComparer.OrdinalIgnoreCase);
            foreach (var group in current.Groups)
                if (previous.TryGetValue(group.Key, out var match))
                {
                    group.IsExpanded = match.IsExpanded;
                    RestoreScopeRows(match.ScopeRows, group.ScopeRows);
                }
        }

        private static void RestoreFindings(IReadOnlyList<PaneFinding> old, IReadOnlyList<PaneFinding> current)
        {
            var previousByKey = old.GroupBy(f => f.StateKey).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
            foreach (var finding in current)
            {
                if (!previousByKey.TryGetValue(finding.StateKey, out var previous)) continue;
                finding.IsExpanded = previous.IsExpanded;
                if (previous.Body != null && finding.Body != null)
                {
                    finding.Body.IsExpanded = previous.Body.IsExpanded;
                    RestoreScopeRows(previous.Body.ScopeRows, finding.Body.ScopeRows);
                    RestoreFindings(previous.Body.Findings, finding.Body.Findings);
                }
            }
        }

        internal void SetFilter(PaneVerdictFilter filter)
        {
            Filter = filter;
            ViewFilterChanged?.Invoke(); // Clears highlights before any view is replaced; never navigates.
            ApplyViewFilter();
        }

        private void ApplyViewFilter()
        {
            _settingSelection = true;
            try
            {
                foreach (var group in _groups) group.Filter = Filter;
                GroupList.ItemsSource = _groups.Where(g => g.VisibleFindings.Count > 0).ToList();
                GroupList.SelectedItem = null;
            }
            finally { _settingSelection = false; }
            var operations = _groups.SelectMany(g => g.Findings).ToArray();
            var counts = new PaneDisplayCounts(operations.Select(f => f.Finding));
            var all = operations.SelectMany(f => f.DescendantsAndSelf()).ToArray();
            _filterCounts[PaneVerdictFilter.Writes] = counts.Writes;
            _filterCounts[PaneVerdictFilter.Unknown] = counts.Unknown;
            _filterCounts[PaneVerdictFilter.Safe] = counts.Safe;
            _filterCounts[PaneVerdictFilter.Inside] = all.Where(f => f.Body != null && f.Body.IsDeferred).Sum(f => f.Body.Findings.Count);
            _filterCounts[PaneVerdictFilter.Context] = counts.Context;
            _filterCounts[PaneVerdictFilter.Remote] = all.Where(f => f.Body != null && !f.Body.IsDeferred).Sum(f => f.Body.Findings.Count);
            RefreshFilterLayout();
            RefreshCollapsedCounts();
            EmptyText.Text = "No operations match the enabled filters. Enable a color for the content you want to inspect.";
            EmptyText.Visibility = GroupList.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private IEnumerable<ToggleButton> FilterButtons => FilterBar.Children.OfType<ToggleButton>();
        internal int FilterCount(PaneVerdictFilter bit) => _filterCounts.TryGetValue(bit, out var count) ? count : 0;
        private static Brush FilterColor(PaneVerdictFilter bit) => bit == PaneVerdictFilter.Writes ? VerdictBrushes.Red :
            bit == PaneVerdictFilter.Unknown ? VerdictBrushes.Yellow : bit == PaneVerdictFilter.Safe ? VerdictBrushes.Green :
            bit == PaneVerdictFilter.Inside ? VerdictBrushes.CodeContainer : bit == PaneVerdictFilter.Context ? VerdictBrushes.Context : VerdictBrushes.Remote;

        internal static string FilterMeaning(PaneVerdictFilter bit) => bit == PaneVerdictFilter.Writes ? SafetyDictionary.Writes :
            bit == PaneVerdictFilter.Unknown ? SafetyDictionary.Unknown : bit == PaneVerdictFilter.Safe ? SafetyDictionary.Safe :
            bit == PaneVerdictFilter.Inside ? SafetyDictionary.Inside : bit == PaneVerdictFilter.Context ? SafetyDictionary.ContextMeaning : SafetyDictionary.Remote;

        public static readonly DependencyProperty LineGutterWidthProperty = DependencyProperty.Register(
            nameof(LineGutterWidth), typeof(GridLength), typeof(SafetyPaneControl), new PropertyMetadata(new GridLength(16)));
        public GridLength LineGutterWidth { get => (GridLength)GetValue(LineGutterWidthProperty); private set => SetValue(LineGutterWidthProperty, value); }
        private void MeasureLineGutter()
        {
            int max = _groups.SelectMany(g => g.Findings).SelectMany(f => f.DescendantsAndSelf())
                .Select(f => Math.Max(f.EditorLine, f.ExecutionLine)).DefaultIfEmpty(1).Max();
            var label = new TextBlock { Text = max.ToString(), FontFamily = FontFamily, FontSize = FontSize };
            System.Windows.Documents.Typography.SetNumeralAlignment(label, FontNumeralAlignment.Tabular);
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            LineGutterWidth = new GridLength(Math.Ceiling(label.DesiredSize.Width) + 8);
        }

        internal void SetFilterLabels(PaneFilterLabels labels)
        {
            FilterLabels = labels;
            RefreshFilterLayout();
        }

        private void RefreshFilterLayout()
        {
            if (FilterBar == null) return;
            var buttons = FilterButtons.ToArray();
            double compact = FilterBar.Margin.Left + FilterBar.Margin.Right + Grip.Width + 12;
            double text = compact;
            foreach (var button in buttons)
            {
                var bit = (PaneVerdictFilter)Enum.Parse(typeof(PaneVerdictFilter), (string)button.Tag);
                var count = FilterCount(bit).ToString();
                compact += MeasureFilterText(count, button) + 8 + 4 + button.Padding.Left + button.Padding.Right + button.Margin.Right + 2;
                text += MeasureFilterText(bit + " " + count, button) + button.Padding.Left + button.Padding.Right + button.Margin.Right + 2;
            }
            double minimum = Math.Max(240, Math.Ceiling(FilterLabels == PaneFilterLabels.Text ? text : compact));
            if (minimum != MinimumExpandedWidth)
            {
                MinimumExpandedWidth = minimum;
                MinimumWidthChanged?.Invoke();
            }
            bool showText = FilterLabels == PaneFilterLabels.Text || FilterLabels == PaneFilterLabels.Automatic && ActualWidth >= text + 2;
            CompactFilterLabels = !showText;
            foreach (var button in buttons)
            {
                var bit = (PaneVerdictFilter)Enum.Parse(typeof(PaneVerdictFilter), (string)button.Tag);
                bool enabled = (Filter & bit) != 0;
                button.IsChecked = enabled;
                var panel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                if (!showText)
                    panel.Children.Add(new System.Windows.Shapes.Ellipse { Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 4, 0), Fill = enabled ? FilterColor(bit) : Brushes.Transparent, Stroke = FilterColor(bit), StrokeThickness = 1 });
                panel.Children.Add(new TextBlock { Text = (showText ? bit + " " : string.Empty) + FilterCount(bit), VerticalAlignment = VerticalAlignment.Center });
                button.Content = panel;
                string meaning = FilterMeaning(bit);
                button.ToolTip = CountToolTip(bit);
                System.Windows.Automation.AutomationProperties.SetName(button, bit + ", " + FilterCount(bit));
                System.Windows.Automation.AutomationProperties.SetHelpText(button, meaning);
            }
            LabelsAutomatic.IsChecked = FilterLabels == PaneFilterLabels.Automatic;
            LabelsText.IsChecked = FilterLabels == PaneFilterLabels.Text;
            LabelsCircles.IsChecked = FilterLabels == PaneFilterLabels.Circles;
        }

        private ToolTip CountToolTip(PaneVerdictFilter bit)
        {
            var tip = new StackPanel { MaxWidth = 340 };
            tip.Children.Add(new TextBlock { Text = bit + " \u00b7 " + FilterCount(bit), FontWeight = FontWeights.SemiBold });
            tip.Children.Add(new TextBlock { Text = FilterMeaning(bit), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 0) });
            return new ToolTip { Content = tip };
        }

        private void RefreshCollapsedCounts()
        {
            if (_analysisPending) return;
            CollapsedCounts.Children.Clear();
            bool scopeStarted = false;
            foreach (var bit in new[] { PaneVerdictFilter.Writes, PaneVerdictFilter.Unknown, PaneVerdictFilter.Safe,
                PaneVerdictFilter.Inside, PaneVerdictFilter.Context, PaneVerdictFilter.Remote })
            {
                if (FilterCount(bit) == 0) continue;
                bool isScope = bit == PaneVerdictFilter.Inside || bit == PaneVerdictFilter.Context || bit == PaneVerdictFilter.Remote;
                double groupGap = isScope && !scopeStarted && CollapsedCounts.Children.Count > 0 ? 8 : 0;
                scopeStarted |= isScope;
                var badge = new Grid { Width = 20, Height = 20, Margin = new Thickness(0, groupGap, 0, 6), Tag = bit,
                    ToolTip = CountToolTip(bit) };
                badge.Children.Add(new System.Windows.Shapes.Ellipse { Fill = FilterColor(bit) });
                var number = new TextBlock { Text = FilterCount(bit).ToString(), FontWeight = FontWeights.Bold,
                    Foreground = bit == PaneVerdictFilter.Unknown || bit == PaneVerdictFilter.Context || bit == PaneVerdictFilter.Remote
                        ? Brushes.Black : Brushes.White };
                badge.Children.Add(new Viewbox { Margin = new Thickness(2), StretchDirection = StretchDirection.DownOnly, Child = number });
                System.Windows.Automation.AutomationProperties.SetName(badge, bit + ", " + FilterCount(bit));
                System.Windows.Automation.AutomationProperties.SetHelpText(badge, FilterMeaning(bit));
                CollapsedCounts.Children.Add(badge);
            }
            CollapsedCounts.Visibility = CollapsedCounts.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            CollapsedStatus.Visibility = CollapsedCounts.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static double MeasureFilterText(string text, ToggleButton button)
        {
            var label = new TextBlock { Text = text, FontFamily = button.FontFamily, FontSize = button.FontSize, FontWeight = FontWeights.SemiBold };
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return label.DesiredSize.Width;
        }

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            var bit = (PaneVerdictFilter)Enum.Parse(typeof(PaneVerdictFilter), (string)button.Tag);
            SetFilter(button.IsChecked == true ? Filter | bit : Filter & ~bit);
        }

        private void OnFilterLabelsClick(object sender, RoutedEventArgs e)
        {
            SetFilterLabels((PaneFilterLabels)Enum.Parse(typeof(PaneFilterLabels), (string)((MenuItem)sender).Tag));
            e.Handled = true;
        }

        internal void ShowNothingToRun(string headline, string detail)
        {
            ShowPlaceholder(headline, detail);
        }

        internal void ShowAnalysisPending()
        {
            ShowCollapsedPending();
            CollapsedStrip.ToolTip = "Analyzing…";
        }

        private void ShowSummary(string headline, string detail)
        {
            Summary = headline + Environment.NewLine + detail;
            CollapsedStrip.ToolTip = Summary;
        }

        private void ShowCollapsedPending()
        {
            _analysisPending = true;
            CollapsedStatus.Visibility = Visibility.Visible;
            CollapsedCounts.Visibility = Visibility.Collapsed;
            CollapsedCounts.Children.Clear();
        }

        internal void Select(PaneGroup group)
        {
            _settingSelection = true;
            try
            {
                GroupList.SelectedItem = GroupList.Items.Contains(group) ? group : null;
                if (group != null && GroupList.Items.Contains(group))
                {
                    GroupList.ScrollIntoView(group);
                }
            }
            finally { _settingSelection = false; }
        }

        internal void ClearListSelection()
        {
            _settingSelection = true;
            try { GroupList.SelectedItem = null; }
            finally { _settingSelection = false; }
        }

        internal void ClickGroup(PaneGroup group)
        {
            if (group != null && group.VisibleFindings.Count == 0) return;
            if (GroupList.Items.Contains(group)) Select(group);
            else ClearListSelection();
            GroupSelected?.Invoke(group);
        }

        internal void ToggleGroup(PaneGroup group)
        {
            ClickGroup(group);
            group.IsExpanded = !group.IsExpanded;
        }

        private IEnumerable<PaneGroup> AllGroups() =>
            _groups;

        private void OnRowClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is PaneGroup group) ClickGroup(group);
            else if (((FrameworkElement)sender).DataContext is PaneFinding finding) BodyFindingActivated?.Invoke(finding);
            else if (((FrameworkElement)sender).DataContext is PaneCodeBody body) BodyPreviewActivated?.Invoke(body);
            e.Handled = true;
        }

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (((FrameworkElement)sender).DataContext is PaneGroup group) ToggleGroup(group);
            else if (((FrameworkElement)sender).DataContext is PaneFinding finding) finding.IsExpanded = !finding.IsExpanded;
            else if (((FrameworkElement)sender).DataContext is PaneCodeBody body) body.IsExpanded = !body.IsExpanded;
            e.Handled = true;
        }

        private void OnRowKeyDown(object sender, KeyEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is PaneCodeBody body)
            {
                if (e.Key == Key.Right || e.Key == Key.Left) { body.IsExpanded = e.Key == Key.Right; e.Handled = true; }
                return;
            }
            if (((FrameworkElement)sender).DataContext is PaneFinding finding)
            {
                if (e.Key == Key.Right || e.Key == Key.Left)
                {
                    finding.IsExpanded = e.Key == Key.Right;
                    e.Handled = true;
                }
                return;
            }
            if (!(((FrameworkElement)sender).DataContext is PaneGroup group)) return;
            if (e.Key == Key.Right || e.Key == Key.Left)
            {
                group.IsExpanded = e.Key == Key.Right;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                GroupActivated?.Invoke(group);
                e.Handled = true;
            }
        }

        private void OnRowArrowClick(object sender, RoutedEventArgs e)
        {
            var data = ((FrameworkElement)sender).DataContext;
            if (data is PaneGroup group) group.IsExpanded = !group.IsExpanded;
            else if (data is PaneFinding finding) finding.IsExpanded = !finding.IsExpanded;
            else if (data is PaneCodeBody body) body.IsExpanded = !body.IsExpanded;
            e.Handled = true;
        }

        private void OnAnalysisBackgroundMouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            while (source != null && source != AnalysisPage)
            {
                if (source is ListBoxItem || source is ButtonBase || source is ScrollBar) return;
                source = source is Visual ? VisualTreeHelper.GetParent(source) : LogicalTreeHelper.GetParent(source);
            }
            Select(null);
            GroupSelected?.Invoke(null);
        }

        internal void SetCollapsed(bool collapsed)
        {
            Expanded.Visibility = collapsed ? Visibility.Collapsed : Visibility.Visible;
            CollapsedStrip.Visibility = collapsed ? Visibility.Visible : Visibility.Collapsed;
            Grip.Visibility = collapsed ? Visibility.Collapsed : Visibility.Visible;
            Grip.Cursor = collapsed ? Cursors.Arrow : Cursors.SizeWE;
        }

        internal void SetPage(bool settings)
        {
            if (AnalysisPage == null || SettingsPage == null) return;
            ViewFilterChanged?.Invoke();
            AnalysisPage.Visibility = settings ? Visibility.Collapsed : Visibility.Visible;
            SettingsPage.Visibility = settings ? Visibility.Visible : Visibility.Collapsed;
            AnalysisButton.IsChecked = !settings;
            SettingsButton.IsChecked = settings;
        }

        private void OnGroupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_settingSelection) return;
            var list = (ListBox)sender;
            var group = list.SelectedItem as PaneGroup;
            if (group == null) return;
            GroupSelected?.Invoke(group);
        }

        private void OnBodyFindingClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is PaneFinding finding)
                BodyFindingActivated?.Invoke(finding);
            e.Handled = true;
        }

        private void OnGroupKeyDown(object sender, KeyEventArgs e)
        {
            // Statement buttons and nested body expanders own their keyboard gestures.
            var list = (ListBox)sender;
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is ListBoxItem))
            {
                if (source is Button || (source is Expander expander && !list.Items.Contains(expander.DataContext))) return;
                source = source is Visual ? VisualTreeHelper.GetParent(source) : LogicalTreeHelper.GetParent(source);
            }
            if ((e.Key == Key.Left || e.Key == Key.Right) && list.SelectedItem is PaneGroup selected)
            {
                selected.IsExpanded = e.Key == Key.Right;
                e.Handled = true;
            }
            else if (e.Key == Key.Space && list.SelectedItem is PaneGroup chosen)
            {
                ClickGroup(chosen);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && list.SelectedItem is PaneGroup group)
            {
                GroupActivated?.Invoke(group);
                e.Handled = true;
            }
        }

        private void OnAnalysisPage(object sender, RoutedEventArgs e) => SetPage(false);
        private void OnSettingsPage(object sender, RoutedEventArgs e) => SetPage(true);
        private void OnEditOptions(object sender, RoutedEventArgs e) => EditOptions?.Invoke();
        private void OnCollapseClicked(object sender, RoutedEventArgs e) => CollapseToggled?.Invoke();
        private void OnToggleClicked(object sender, MouseButtonEventArgs e) => CollapseToggled?.Invoke();
        private void OnGripDragDelta(object sender, DragDeltaEventArgs e) => WidthDragged?.Invoke(e.HorizontalChange);
    }
}
