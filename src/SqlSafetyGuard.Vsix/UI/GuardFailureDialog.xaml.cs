using System;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace SqlSafetyGuard.UI
{
    public partial class GuardFailureDialog : DialogWindow
    {
        internal GuardFailureDialog(Exception exception)
        {
            InitializeComponent();
#if DEBUG
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                ShowActivated = false;
#endif
            // Host exception messages can contain SQL or be very long. Keep the reason short and predictable.
            ReasonText.Text = "The check failed (" + exception.GetType().Name + "). See SQL Safety Guard in Output for details.";
        }

        private void OnRunAnyway(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
