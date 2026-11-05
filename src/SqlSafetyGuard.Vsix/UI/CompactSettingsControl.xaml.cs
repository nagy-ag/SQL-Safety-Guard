using System;
using System.Windows;
using System.Windows.Controls;
using SqlSafetyGuard.Options;

namespace SqlSafetyGuard.UI
{
    /// <summary>Presentation only; the existing option page owns values and native persistence.</summary>
    public partial class CompactSettingsControl : UserControl
    {
        private readonly GeneralOptionsPage _page;
        private readonly Action _persist;
        private readonly Action<int> _command;
        private bool _refreshing;

        public CompactSettingsControl() : this(SqlSafetyGuardPackage.Instance.Options,
            SqlSafetyGuardPackage.Instance.Options.PersistChanges, SqlSafetyGuardPackage.Instance.InvokeSettingsCommand) { }

        internal CompactSettingsControl(GeneralOptionsPage page, Action persist, Action<int> command)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _persist = persist ?? throw new ArgumentNullException(nameof(persist));
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _refreshing = true;
            InitializeComponent();
            // SSMS's GEL content container marks preview focus events handled.
            // This content is an editable settings form: allow focus within our
            // own subtree, without changing the host or any other preview.
            AddHandler(System.Windows.Input.Keyboard.PreviewGotKeyboardFocusEvent,
                new System.Windows.Input.KeyboardFocusChangedEventHandler((s, e) => e.Handled = false), true);
            _refreshing = false;
            RefreshValues();
            Loaded += (s, e) => { _page.Changed += OnSettingsChanged; RefreshValues(); };
            Unloaded += (s, e) => _page.Changed -= OnSettingsChanged;
        }

        private void OnSettingsChanged(object sender, EventArgs e) => RefreshValues();

        private void RefreshValues()
        {
            _refreshing = true;
            try
            {
                WarningMode.SelectedIndex = (int)_page.GuardMode;
                ShowConnection.IsChecked = _page.ShowServerAndDatabase;
                if (AllowList.Text != _page.AllowList) AllowList.Text = _page.AllowList;
                if (TrustedProcedures.Text != _page.TrustedProcedures) TrustedProcedures.Text = _page.TrustedProcedures;
                if (EnvironmentRules.Text != _page.EnvironmentRules) EnvironmentRules.Text = _page.EnvironmentRules;
                TempdbIsTemp.IsChecked = _page.TempdbIsTemp;
            }
            finally { _refreshing = false; }
        }

        private void Edit(Action assign, Action restore)
        {
            if (_refreshing) return;
            try
            {
                assign();
                _persist();
                SaveError.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                restore();
                RefreshValues();
                SaveError.Text = "Settings were not saved. " + exception.Message;
                SaveError.Visibility = Visibility.Visible;
            }
        }

        private void OnModeChanged(object sender, SelectionChangedEventArgs e)
        {
            var before = _page.GuardMode;
            if (WarningMode.SelectedIndex >= 0) Edit(() => _page.GuardMode = (GuardMode)WarningMode.SelectedIndex, () => _page.GuardMode = before);
        }
        private void OnConnectionChanged(object sender, RoutedEventArgs e)
        {
            var before = _page.ShowServerAndDatabase;
            Edit(() => _page.ShowServerAndDatabase = ShowConnection.IsChecked == true, () => _page.ShowServerAndDatabase = before);
        }
        private void OnAllowListChanged(object sender, TextChangedEventArgs e)
        {
            var before = _page.AllowList;
            Edit(() => _page.AllowList = AllowList.Text, () => _page.AllowList = before);
        }
        private void OnTrustedChanged(object sender, TextChangedEventArgs e)
        {
            var before = _page.TrustedProcedures;
            Edit(() => _page.TrustedProcedures = TrustedProcedures.Text, () => _page.TrustedProcedures = before);
        }
        private void OnRulesChanged(object sender, TextChangedEventArgs e)
        {
            var before = _page.EnvironmentRules;
            Edit(() => _page.EnvironmentRules = EnvironmentRules.Text, () => _page.EnvironmentRules = before);
        }
        private void OnTempdbChanged(object sender, RoutedEventArgs e)
        {
            var before = _page.TempdbIsTemp;
            Edit(() => _page.TempdbIsTemp = TempdbIsTemp.IsChecked == true, () => _page.TempdbIsTemp = before);
        }
        private void OnCommand(object sender, RoutedEventArgs e)
        {
            try { _command(int.Parse((string)((Button)sender).Tag, System.Globalization.CultureInfo.InvariantCulture)); }
            catch (Exception exception)
            {
                SaveError.Text = exception.Message;
                SaveError.Visibility = Visibility.Visible;
            }
        }
    }
}
