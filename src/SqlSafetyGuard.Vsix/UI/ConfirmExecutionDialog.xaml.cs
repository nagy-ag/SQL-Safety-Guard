using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.UI
{
    public partial class ConfirmExecutionDialog : DialogWindow
    {
        internal ConfirmExecutionDialog(AnalysisResult result, QuerySnapshot snapshot, ConnectionContext connection, GuardSettings settings, bool showConnection)
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height * 0.9;
            EnvironmentText.Text = settings.Environment + " · " + snapshot.Describe();
            if (showConnection || connection.Server == null || connection.Database == null)
            {
                ConnectionText.Text = connection.Description;
                ConnectionText.Visibility = Visibility.Visible;
            }

            var rows = ConfirmationLedgerRow.Create(result, snapshot, connection.Database);
            var red = rows.Where(row => row.Verdict.IsRed()).ToList();
            var yellow = rows.Where(row => row.Verdict.IsYellow()).ToList();
            RedList.ItemsSource = red;
            YellowList.ItemsSource = yellow;
            RedCount.Text = red.Count.ToString();
            YellowCount.Text = yellow.Count.ToString();
            RedSection.Visibility = red.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            YellowSection.Visibility = yellow.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            RedMarker.Fill = VerdictBrushes.For(Verdict.RealWrite);
            YellowMarker.Fill = VerdictBrushes.For(Verdict.Unknown);

            int blocked = result.Warnings.Count(finding => !finding.WillRun);
            if (blocked > 0)
            {
                BlockedText.Text = $"{blocked} finding(s) are in batches with syntax errors. Dimmed entries won't run in those batches now; review them again after fixing the errors.";
                BlockedText.Visibility = Visibility.Visible;
            }
            if (result.Findings.Any(finding => finding.Action == "SETTINGS WARNING"))
            {
                SettingsText.Text = "SQL Safety Guard settings contain errors. Review Tools > Options > SQL Safety Guard.";
                SettingsText.Visibility = Visibility.Visible;
            }
            TargetSwitchText.Visibility = result.Findings.Any(finding => finding.Action == "TARGET SWITCH") ? Visibility.Visible : Visibility.Collapsed;
            Loaded += (sender, args) => CancelButton.Focus();
        }

        private void OnRunAnyway(object sender, RoutedEventArgs args)
        {
            DialogResult = true;
        }
    }
}
