using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            Loaded += (s, e) =>
            {
                Visibility = IsStickyHeaderCopy(this) ? Visibility.Collapsed : Visibility.Visible;
                _page.Changed += OnSettingsChanged;
                RefreshValues();
            };
            Unloaded += (s, e) => _page.Changed -= OnSettingsChanged;
        }

        /// <summary>
        /// The settings page repeats a scrolled-past category, including its preview content, as a sticky header
        /// above the page. A second full settings form there would cover the scrolling page and take its wheel
        /// input, so that copy stays empty. Only the sticky CategoryView has a finite MaxVerticalExtent; anything
        /// unrecognized keeps the settings visible.
        /// </summary>
        internal static bool IsStickyHeaderCopy(DependencyObject element)
        {
            for (var node = element == null ? null : VisualTreeHelper.GetParent(element); node != null; node = VisualTreeHelper.GetParent(node))
            {
                if (node.GetType().Name != "CategoryView") continue;
                var extent = node.GetType().GetProperty("MaxVerticalExtent")?.GetValue(node) as double?;
                return extent.HasValue && !double.IsNaN(extent.Value);
            }
            return false;
        }

        private void OnSettingsChanged(object sender, EventArgs e) => RefreshValues();

        private void RefreshValues()
        {
            _refreshing = true;
            try
            {
                WarningMode.SelectedIndex = (int)_page.GuardMode;
                ShowConnection.IsChecked = _page.ShowServerAndDatabase;
                TempdbIsTemp.IsChecked = _page.TempdbIsTemp;
                ShowSummary(AllowListSummary, AllowListProblem, PatternText.Summarize(_page.AllowList, procedures: false), "pattern", "patterns");
                ShowSummary(TrustedSummary, TrustedProblem, PatternText.Summarize(_page.TrustedProcedures, procedures: true), "procedure", "procedures");
                ShowSummary(RulesSummary, RulesProblem, RuleText.Summarize(_page.EnvironmentRules), "rule", "rules");
            }
            finally { _refreshing = false; }
        }

        /// <summary>Only counts: the lists are edited in their own dialogs, so this row never grows.</summary>
        private static void ShowSummary(TextBlock summary, TextBlock problem, (int Count, int Problems) value, string one, string many)
        {
            summary.Text = value.Count == 0 ? "None" : value.Count + " " + (value.Count == 1 ? one : many);
            summary.FontStyle = value.Count == 0 ? FontStyles.Italic : FontStyles.Normal;
            summary.Opacity = value.Count == 0 ? 0.7 : 1;
            problem.Text = value.Problems == 0 ? "" : "⚠ " + value.Problems + " " + (value.Problems == 1 ? one + " has a problem and is" : many + " have a problem and are") +
                " ignored. Open the editor to fix " + (value.Problems == 1 ? "it." : "them.");
            problem.Visibility = value.Problems == 0 ? Visibility.Collapsed : Visibility.Visible;
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
