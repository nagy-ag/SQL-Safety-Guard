using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    /// <summary>The view of the safety pane inside a query window. All logic lives in <see cref="SafetyPaneMargin"/>.</summary>
    public partial class SafetyPaneControl : UserControl
    {
        private bool _settingSelection;

        public SafetyPaneControl()
        {
            InitializeComponent();
            CollapsedRedDot.Fill = VerdictBrushes.Red;
            CollapsedRedCount.Foreground = VerdictBrushes.TextOn(Verdict.RealWrite);
            CollapsedYellowDot.Fill = VerdictBrushes.Yellow;
            CollapsedYellowCount.Foreground = VerdictBrushes.TextOn(Verdict.Unknown);
        }

        internal event Action<PaneGroup> GroupSelected;

        internal event Action<PaneGroup> GroupActivated;

        internal event Action CollapseToggled;

        internal event Action<double> WidthDragged;

        internal void ShowPlaceholder(string headline, string detail)
        {
            VerdictDot.Fill = System.Windows.Media.Brushes.Gray;
            VerdictGlyph.Text = "…";
            ShowCollapsedPending();
            ShowSummary(headline, detail);
            GroupList.ItemsSource = null;
            DetailCard.Visibility = Visibility.Collapsed;
            SafeHeader.Text = string.Empty;
        }

        internal void ShowResult(Verdict overall, string headline, string detail, string connection, IReadOnlyList<PaneGroup> groups,
            IReadOnlyList<PaneFinding> safe)
        {
            var brush = VerdictBrushes.For(overall);
            VerdictDot.Fill = brush;
            int redCount = groups.Count(group => group.Verdict.IsRed());
            int yellowCount = groups.Count(group => group.Verdict.IsYellow());
            bool allSafe = redCount == 0 && yellowCount == 0;
            CollapsedStatus.Visibility = allSafe ? Visibility.Visible : Visibility.Collapsed;
            CollapsedDot.Fill = VerdictBrushes.Green;
            CollapsedGlyph.Text = "\u2713";
            CollapsedRed.Visibility = redCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            CollapsedRedCount.Text = redCount.ToString();
            CollapsedYellow.Visibility = yellowCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            CollapsedYellowCount.Text = yellowCount.ToString();
            VerdictGlyph.Text = overall.IsGreen() ? "✓" : "!";
            VerdictGlyph.Foreground = VerdictBrushes.TextOn(overall);
            ShowSummary(headline, detail);
            ConnectionText.Text = connection ?? string.Empty;
            ConnectionText.Visibility = string.IsNullOrEmpty(connection) ? Visibility.Collapsed : Visibility.Visible;

            _settingSelection = true;
            try
            {
                GroupList.ItemsSource = groups;
            }
            finally
            {
                _settingSelection = false;
            }

            EmptyText.Visibility = groups.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SafeList.ItemsSource = safe;
            SafeHeader.Text = (SafeList.Visibility == Visibility.Visible ? "▾ " : "▸ ") + $"Temp operations and reads · {safe.Count}";
        }

        internal void ShowNothingToRun(string headline, string detail, string connection)
        {
            ShowPlaceholder(headline, detail);
            VerdictGlyph.Text = "–";
            ConnectionText.Text = connection ?? string.Empty;
            ConnectionText.Visibility = string.IsNullOrEmpty(connection) ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = Visibility.Collapsed;
            SafeList.ItemsSource = null;
        }

        /// <summary>Greys the collapsed strip while a newer analysis is pending; the expanded pane keeps the last result until then.</summary>
        internal void ShowAnalysisPending()
        {
            ShowCollapsedPending();
            CollapsedStrip.ToolTip = "Analyzing\u2026";
        }

        private void ShowSummary(string headline, string detail)
        {
            Headline.Text = headline;
            SubText.Text = detail;
            CollapsedStrip.ToolTip = headline + Environment.NewLine + detail;
        }

        private void ShowCollapsedPending()
        {
            CollapsedStatus.Visibility = Visibility.Visible;
            CollapsedDot.Fill = System.Windows.Media.Brushes.Gray;
            CollapsedGlyph.Text = string.Empty;
            CollapsedRed.Visibility = Visibility.Collapsed;
            CollapsedYellow.Visibility = Visibility.Collapsed;
        }

        /// <summary>Selects a group without raising <see cref="GroupSelected"/> as a user action (used for caret sync and refresh).</summary>
        internal void Select(PaneGroup group)
        {
            _settingSelection = true;
            try
            {
                GroupList.SelectedItem = group;
            }
            finally
            {
                _settingSelection = false;
            }

            if (group != null)
                GroupList.ScrollIntoView(group);
            ShowDetail(group);
        }

        /// <summary>Same as the user clicking the group (raises <see cref="GroupSelected"/>).</summary>
        internal void ClickGroup(PaneGroup group) => GroupList.SelectedItem = group;

        internal void SetCollapsed(bool collapsed)
        {
            Expanded.Visibility = collapsed ? Visibility.Collapsed : Visibility.Visible;
            CollapsedStrip.Visibility = collapsed ? Visibility.Visible : Visibility.Collapsed;
            Grip.Visibility = collapsed ? Visibility.Collapsed : Visibility.Visible;
            Grip.Cursor = collapsed ? Cursors.Arrow : Cursors.SizeWE;
        }

        private void ShowDetail(PaneGroup group)
        {
            DetailCard.Visibility = group == null ? Visibility.Collapsed : Visibility.Visible;
            if (group == null)
                return;
            DetailTitle.Text = group.DetailTitle;
            DetailItems.ItemsSource = group.Findings;
        }

        private void OnGroupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var group = GroupList.SelectedItem as PaneGroup;
            ShowDetail(group);
            if (!_settingSelection)
                GroupSelected?.Invoke(group);
        }

        private void OnGroupDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemsControl.ContainerFromElement(GroupList, e.OriginalSource as DependencyObject) is ListBoxItem item &&
                item.DataContext is PaneGroup group)
            {
                GroupActivated?.Invoke(group);
                e.Handled = true;
            }
        }

        private void OnGroupKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && GroupList.SelectedItem is PaneGroup group)
            {
                GroupActivated?.Invoke(group);
                e.Handled = true;
            }
        }

        private void OnSafeHeaderClicked(object sender, MouseButtonEventArgs e)
        {
            bool show = SafeList.Visibility != Visibility.Visible;
            SafeList.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SafeHeader.Text = (show ? "▾ " : "▸ ") + SafeHeader.Text.Substring(2);
        }

        private void OnToggleClicked(object sender, MouseButtonEventArgs e) => CollapseToggled?.Invoke();

        private void OnGripDragDelta(object sender, DragDeltaEventArgs e) => WidthDragged?.Invoke(e.HorizontalChange);
    }
}
