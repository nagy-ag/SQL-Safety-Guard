using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using SqlSafetyGuard.Options;

namespace SqlSafetyGuard.UI
{
    public partial class SettingsImportDialog : DialogWindow
    {
        internal SettingsImportDialog(SettingsImport import)
        {
            InitializeComponent();
#if DEBUG
            // The explicitly enabled in-process smoke channel must never take foreground focus.
            if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY"))) ShowActivated = false;
#endif
            DataContext = import;
            MaxHeight = SystemParameters.WorkArea.Height * 0.9;
            WarningSection.BorderBrush = VerdictBrushes.Yellow;
            WarningSection.Visibility = import.Warnings.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            IgnoredKeys.Text = import.IgnoredKeys.Count == 0 ? "" : "Ignored extra keys (not imported): " + string.Join(", ", import.IgnoredKeys);
            IgnoredKeys.Visibility = import.IgnoredKeys.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            if (import.Changes.Count == 0) ChangesHeading.Text = "No settings will change.";
            ImportButton.IsEnabled = import.Changes.Count > 0;
        }

        private void OnImport(object sender, RoutedEventArgs e) { DialogResult = true; }
    }
}
