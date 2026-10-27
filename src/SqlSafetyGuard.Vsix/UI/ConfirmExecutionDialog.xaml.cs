using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.UI
{
    /// <summary>Shown by the F5 guard when the query is yellow or red. Cancel is the default (Enter and Esc).</summary>
    public partial class ConfirmExecutionDialog : DialogWindow
    {
        private const int MaxListedFindings = 500;

        internal ConfirmExecutionDialog(AnalysisResult result, QuerySnapshot snapshot, string connection, GuardMode mode)
        {
            InitializeComponent();

            var warnings = result.Warnings.Select(f => new FindingRow(f, snapshot)).ToList();
            var runNow = warnings.Where(w => w.WillRun).ToList();
            var blocked = warnings.Where(w => !w.WillRun).ToList();

            HeaderText.Text = "⚠ " + result.Summary;
            HeaderBadge.Background = VerdictBrushes.For(result.Overall);

            var what = snapshot.Describe();
            if (runNow.Any(w => w.Finding.Verdict == Verdict.RealWrite && !w.Finding.IsDiagnostic))
                SubText.Text = $"The {what} changes permanent data or objects. It has not been executed.";
            else if (runNow.Any(w => !w.Finding.IsDiagnostic))
                SubText.Text = $"The analyzer cannot see everything the {what} does. It has not been executed.";
            else
                SubText.Text = $"The {what} has syntax errors. It has not been executed.";
            if (blocked.Count > 0)
                SubText.Text += " Some statements are in a batch with a syntax error: SQL Server won't run that batch now, but will once it's fixed.";

            if (!string.IsNullOrEmpty(connection))
            {
                ConnectionText.Text = connection;
                ConnectionText.Visibility = Visibility.Visible;
            }

            FindingsList.ItemsSource = runNow.Take(MaxListedFindings).ToList();
            FindingsList.Visibility = runNow.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            if (blocked.Count > 0)
            {
                BlockedList.ItemsSource = blocked.Take(MaxListedFindings).ToList();
                BlockedList.Visibility = Visibility.Visible;
                BlockedHeader.Visibility = Visibility.Visible;
            }

            if (runNow.Count > MaxListedFindings || blocked.Count > MaxListedFindings)
                FooterText.Text = $"Showing the first {MaxListedFindings} of each list. " + FooterText.Text;

            Loaded += (s, e) => CancelButton.Focus();
        }

        /// <summary>Set when the user double-clicked a finding to jump to it (the dialog then returns false).</summary>
        internal Finding NavigateTo { get; private set; }

        private void OnRunAnyway(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnFindingDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox list &&
                ItemsControl.ContainerFromElement(list, e.OriginalSource as DependencyObject) is ListBoxItem item &&
                item.DataContext is FindingRow row)
            {
                NavigateTo = row.Finding;
                DialogResult = false;
            }
        }
    }
}
