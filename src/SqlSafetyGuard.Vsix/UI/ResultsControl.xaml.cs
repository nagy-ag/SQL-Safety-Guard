using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.UI
{
    public partial class ResultsControl : UserControl
    {
        private QuerySnapshot _snapshot;
        private List<FindingRow> _rows = new List<FindingRow>();

        public ResultsControl()
        {
            InitializeComponent();
            IsVisibleChanged += (s, e) =>
            {
                if ((bool)e.NewValue)
                    BecameVisible?.Invoke();
            };
        }

        /// <summary>Raised when the window is shown again (e.g. its tab is selected), so it can catch up with the active tab.</summary>
        internal event Action BecameVisible;

        /// <summary>True when this exact text of this document (same selection) is what's shown now.</summary>
        internal bool IsShowing(QuerySnapshot snapshot)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_snapshot == null)
                return false;
            try
            {
                return string.Equals(_snapshot.Document.FullName, snapshot.Document.FullName, StringComparison.OrdinalIgnoreCase) &&
                       _snapshot.StartLine == snapshot.StartLine && _snapshot.StartColumn == snapshot.StartColumn &&
                       string.Equals(_snapshot.Text, snapshot.Text, StringComparison.Ordinal);
            }
            catch (Exception)
            {
                return false; // The shown document was closed.
            }
        }

        /// <summary>A neutral message instead of results (no SQL tab active, too large, analyzing...).</summary>
        internal void ShowPlaceholder(string headline, string detail)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _snapshot = null;
            _rows = new List<FindingRow>();

            Banner.SetResourceReference(Border.BackgroundProperty, EnvironmentColors.ToolWindowBackgroundBrushKey);
            foreach (var element in new DependencyObject[] { BannerText, BannerDetail, WarningsOnly })
                ((FrameworkElement)element).SetResourceReference(TextElement.ForegroundProperty, EnvironmentColors.ToolWindowTextBrushKey);

            BannerText.Text = headline;
            BannerDetail.Text = detail;
            EmptyText.Visibility = Visibility.Collapsed;
            ApplyFilter();
        }

        internal void ShowResult(QuerySnapshot snapshot, AnalysisResult result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _snapshot = snapshot;
            _rows = result.Findings.Select(f => new FindingRow(f, snapshot)).ToList();

            var overall = result.Overall;
            var textBrush = VerdictBrushes.TextOn(overall);
            Banner.Background = VerdictBrushes.For(overall);
            BannerText.Foreground = textBrush;
            BannerDetail.Foreground = textBrush;
            WarningsOnly.Foreground = textBrush;

            BannerText.Text = result.IsGreen ? result.Summary + " — runs without a prompt" : result.Summary;

            var details = new List<string>
            {
                $"{Capitalize(snapshot.Describe())} of {SafeDocumentName(snapshot)}",
                result.Findings.Count == 1 ? "1 finding" : result.Findings.Count + " findings",
            };
            if (result.SkippedRegions.Count > 0)
                details.Add("not analyzed: " + string.Join(", ", result.SkippedRegions.Select(r => r.Lines)));
            details.Add("analyzed " + DateTime.Now.ToString("HH:mm:ss") + ", not executed");
            BannerDetail.Text = string.Join("  ·  ", details);

            EmptyText.Visibility = Visibility.Collapsed;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Grid.ItemsSource = WarningsOnly.IsChecked == true ? _rows.Where(r => r.IsWarning).ToList() : _rows;
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e) => ApplyFilter();

        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (ItemsControl.ContainerFromElement(Grid, e.OriginalSource as DependencyObject) is DataGridRow row &&
                row.Item is FindingRow finding)
            {
                NavigateTo(finding);
                e.Handled = true;
            }
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (e.Key == Key.Enter && Grid.SelectedItem is FindingRow finding)
            {
                NavigateTo(finding);
                e.Handled = true;
            }
        }

        private void NavigateTo(FindingRow row)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_snapshot != null)
                SqlEditorAccess.Navigate(_snapshot, row.Finding.StartOffset);
        }

        private static string SafeDocumentName(QuerySnapshot snapshot)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                return snapshot.Document.Name;
            }
            catch (Exception)
            {
                return "a closed window";
            }
        }

        private static string Capitalize(string text) =>
            string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text.Substring(1);
    }
}
