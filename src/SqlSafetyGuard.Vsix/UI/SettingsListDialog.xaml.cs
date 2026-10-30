using System;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace SqlSafetyGuard.UI
{
    public partial class SettingsListDialog : DialogWindow
    {
        internal Action<string> Save { get; set; }

        internal SettingsListDialog(string title, string description, string entries)
        {
            InitializeComponent();
            Title = "SQL Safety Guard — " + title;
            Description.Text = description;
            Entries.Text = entries.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (CapturePreview.IsPreview(this)) return;
#endif
            try
            {
                Save(Entries.Text);
                DialogResult = true;
            }
            catch (Exception exception)
            {
                Error.Text = exception.Message;
                Error.Visibility = Visibility.Visible;
            }
        }
    }
}
