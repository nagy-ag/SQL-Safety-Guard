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
        private bool _safeOpenedAutomatically;
        private readonly Brush _slate = new SolidColorBrush(Color.FromRgb(0x5c, 0x6b, 0x7a));

        public SafetyPaneControl()
        {
            InitializeComponent();
            CollapsedRedDot.Fill = HeaderRedDot.Fill = VerdictBrushes.Red;
            CollapsedRedCount.Foreground = VerdictBrushes.TextOn(Verdict.RealWrite);
            CollapsedYellowDot.Fill = HeaderYellowDot.Fill = VerdictBrushes.Yellow;
            CollapsedYellowCount.Foreground = VerdictBrushes.TextOn(Verdict.Unknown);
        }

        internal event Action<PaneGroup> GroupSelected;
        internal event Action<PaneGroup> GroupActivated;
        internal event Action<PaneFinding> BodyFindingActivated;
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
            HeaderStatus.Visibility = Visibility.Visible;
            HeaderDot.Fill = Brushes.Gray;
            HeaderGlyph.Text = string.Empty;
            HeaderRed.Visibility = HeaderYellow.Visibility = Visibility.Collapsed;
            HeaderRedCount.Text = HeaderYellowCount.Text = "0";
            ShowCollapsedPending();
            ShowSummary(headline, detail);
            _settingSelection = true;
            try { GroupList.ItemsSource = SafeList.ItemsSource = null; }
            finally { _settingSelection = false; }
            SafeSection.Visibility = Visibility.Collapsed;
            EmptyText.Text = headline + ". " + detail;
            EmptyText.Visibility = Visibility.Visible;
        }

        internal void ShowResult(string summary, string f5, IReadOnlyList<PaneGroup> groups)
        {
            int redCount = groups.Count(group => group.Verdict.IsRed());
            int yellowCount = groups.Count(group => group.Verdict.IsYellow());
            bool allSafe = redCount == 0 && yellowCount == 0;
            CollapsedStatus.Visibility = HeaderStatus.Visibility = allSafe ? Visibility.Visible : Visibility.Collapsed;
            CollapsedDot.Fill = HeaderDot.Fill = VerdictBrushes.Green;
            CollapsedGlyph.Text = HeaderGlyph.Text = "✓";
            CollapsedRed.Visibility = HeaderRed.Visibility = redCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            CollapsedRedCount.Text = HeaderRedCount.Text = redCount.ToString();
            CollapsedYellow.Visibility = HeaderYellow.Visibility = yellowCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            CollapsedYellowCount.Text = HeaderYellowCount.Text = yellowCount.ToString();
            ShowSummary(summary, f5);
            _settingSelection = true;
            try
            {
                var expandedKeys = new HashSet<string>(AllGroups().Where(group => group.IsExpanded).Select(group => group.Key), StringComparer.OrdinalIgnoreCase);
                var previousKeys = new HashSet<string>(AllGroups().Select(g => g.Key), StringComparer.OrdinalIgnoreCase);
                var oldBodies = AllGroups().SelectMany(g => g.Bodies).GroupBy(b => b.Definition.Target, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => new
                    {
                        g.First().IsExpanded,
                        g.First().IsSafeExpanded,
                        Groups = g.First().Sections.SelectMany(s => s.Items).Concat(g.First().Safe)
                            .ToDictionary(nested => nested.Key, nested => nested.IsExpanded, StringComparer.OrdinalIgnoreCase)
                    }, StringComparer.OrdinalIgnoreCase);
                bool singleBody = groups.Sum(g => g.Bodies.Count) == 1;
                bool hasExpandedGroup = groups.Any(group => expandedKeys.Contains(group.Key));
                foreach (var group in groups)
                {
                    group.IsExpanded = expandedKeys.Contains(group.Key) ||
                        (!hasExpandedGroup && singleBody && group.Bodies.Count > 0 && !previousKeys.Contains(group.Key));
                    foreach (var body in group.Bodies)
                    {
                        body.IsExpanded = singleBody;
                        if (!oldBodies.TryGetValue(body.Definition.Target, out var previous)) continue;
                        body.IsExpanded = previous.IsExpanded;
                        body.IsSafeExpanded = previous.IsSafeExpanded;
                        foreach (var nested in body.Sections.SelectMany(s => s.Items).Concat(body.Safe))
                            if (previous.Groups.TryGetValue(nested.Key, out bool expanded)) nested.IsExpanded = expanded;
                    }
                }
                var main = new ListCollectionView(groups.Where(group => group.Section != "Safe").ToList());
                main.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PaneGroup.Section)));
                GroupList.ItemsSource = main;
                var safe = groups.Where(group => group.Section == "Safe").ToList();
                SafeList.ItemsSource = safe;
                GroupList.SelectedItem = SafeList.SelectedItem = null;
                SafeHeader.Text = "No permanent changes detected · " + safe.Count;
                SafeSection.Visibility = safe.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                if (allSafe || _safeOpenedAutomatically) SafeSection.IsExpanded = allSafe;
                _safeOpenedAutomatically = allSafe;
            }
            finally { _settingSelection = false; }
            EmptyText.Visibility = Visibility.Collapsed;
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
            CollapsedStrip.ToolTip = HeaderCounts.ToolTip = Summary;
        }

        private void ShowCollapsedPending()
        {
            CollapsedStatus.Visibility = Visibility.Visible;
            CollapsedDot.Fill = Brushes.Gray;
            CollapsedGlyph.Text = string.Empty;
            CollapsedRed.Visibility = Visibility.Collapsed;
            CollapsedYellow.Visibility = Visibility.Collapsed;
        }

        internal void Select(PaneGroup group)
        {
            _settingSelection = true;
            try
            {
                GroupList.SelectedItem = GroupList.Items.Contains(group) ? group : null;
                SafeList.SelectedItem = SafeList.Items.Contains(group) ? group : null;
                if (group != null)
                {
                    var list = group.Section == "Safe" ? SafeList : GroupList;
                    if (list == SafeList) SafeSection.IsExpanded = true;
                    list.ScrollIntoView(group);
                }
            }
            finally { _settingSelection = false; }
        }

        internal void ClearListSelection()
        {
            _settingSelection = true;
            try { GroupList.SelectedItem = SafeList.SelectedItem = null; }
            finally { _settingSelection = false; }
        }

        internal void ClickGroup(PaneGroup group)
        {
            if (GroupList.Items.Contains(group) || SafeList.Items.Contains(group)) Select(group);
            else ClearListSelection();
            GroupSelected?.Invoke(group);
        }

        internal void ToggleGroup(PaneGroup group)
        {
            ClickGroup(group);
            group.IsExpanded = !group.IsExpanded;
        }

        private IEnumerable<PaneGroup> AllGroups() =>
            GroupList.Items.Cast<PaneGroup>().Concat(SafeList.Items.Cast<PaneGroup>());

        private void OnRowClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is PaneGroup group) ClickGroup(group);
            e.Handled = true;
        }

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (((FrameworkElement)sender).DataContext is PaneGroup group) ToggleGroup(group);
            e.Handled = true;
        }

        private void OnRowKeyDown(object sender, KeyEventArgs e)
        {
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
            _settingSelection = true;
            try { (list == GroupList ? SafeList : GroupList).SelectedItem = null; }
            finally { _settingSelection = false; }
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
            else if (e.Key == Key.Down && list == GroupList && list.SelectedIndex == list.Items.Count - 1 && SafeList.HasItems)
            {
                SafeSection.IsExpanded = true;
                SafeList.SelectedIndex = 0;
                SafeList.UpdateLayout();
                (SafeList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem)?.Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && list == SafeList && list.SelectedIndex == 0 && GroupList.HasItems)
            {
                GroupList.SelectedIndex = GroupList.Items.Count - 1;
                GroupList.ScrollIntoView(GroupList.SelectedItem);
                GroupList.UpdateLayout();
                (GroupList.ItemContainerGenerator.ContainerFromIndex(GroupList.SelectedIndex) as ListBoxItem)?.Focus();
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
