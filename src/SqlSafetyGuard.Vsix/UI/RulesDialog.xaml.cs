using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using SqlSafetyGuard.Options;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// Environment rules as a list with a form per rule. Edits stay in the dialog until Save, which hands the
    /// composed text to the same validation as an import.
    /// </summary>
    public partial class RulesDialog : DialogWindow
    {
        private readonly ObservableCollection<RuleEntry> _rules = new ObservableCollection<RuleEntry>();
        private List<string> _trailing = new List<string>();
        private bool _loading;

        internal Action<string> Save { get; set; }

        internal RulesDialog(string text)
        {
            InitializeComponent();
            DialogChrome.Apply(this);
            RuleList.ItemsSource = _rules;
            LoadText(text);
        }

        /// <summary>The stored-format text for the current rules.</summary>
        internal string CurrentText => RuleText.Compose(_rules, _trailing);

        internal IReadOnlyList<RuleEntry> Rules => _rules;

        internal void LoadText(string text)
        {
            _rules.Clear();
            foreach (var rule in RuleText.Parse(text, out _trailing)) _rules.Add(rule);
            RuleList.SelectedIndex = _rules.Count > 0 ? 0 : -1;
            ShowSelected();
        }

        private RuleEntry Selected => RuleList.SelectedItem as RuleEntry;

        private void ShowSelected()
        {
            var rule = Selected;
            int index = RuleList.SelectedIndex;
            FormScroller.Visibility = rule == null ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = rule == null ? Visibility.Visible : Visibility.Collapsed;
            RemoveButton.IsEnabled = rule != null;
            UpButton.IsEnabled = index > 0;
            DownButton.IsEnabled = index >= 0 && index < _rules.Count - 1;
            if (rule == null) return;
            _loading = true;
            try
            {
                ServerBox.Text = rule.Server;
                DatabaseBox.Text = rule.Database;
                LabelBox.Text = rule.Label;
                ModeOff.IsChecked = rule.Mode == "Off";
                ModeRed.IsChecked = rule.Mode == "Red";
                ModeYellowRed.IsChecked = rule.Mode == "YellowRed";
                SafeTablesBox.Text = rule.SafeTables.Replace("\n", Environment.NewLine);
                TrustedBox.Text = rule.TrustedProcedures.Replace("\n", Environment.NewLine);
                NoteBox.Text = rule.Note;
                foreach (var field in new Control[] { ServerBox, DatabaseBox, LabelBox, ModeOff, ModeRed, ModeYellowRed, SafeTablesBox, TrustedBox })
                    field.IsEnabled = rule.Readable;
            }
            finally { _loading = false; }
            ShowProblem();
        }

        private void ShowProblem()
        {
            var rule = Selected;
            FormProblem.Text = rule?.Problem ?? "";
            FormProblem.Visibility = rule?.HasProblem == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => ShowSelected();

        private void OnFieldChanged(object sender, TextChangedEventArgs e)
        {
            var rule = Selected;
            if (_loading || rule == null) return;
            if (sender == ServerBox) rule.Server = ServerBox.Text;
            else if (sender == DatabaseBox) rule.Database = DatabaseBox.Text;
            else if (sender == LabelBox) rule.Label = LabelBox.Text;
            else if (sender == SafeTablesBox) rule.SafeTables = SafeTablesBox.Text;
            else rule.TrustedProcedures = TrustedBox.Text;
            Error.Visibility = Visibility.Collapsed;
            ShowProblem();
        }

        private void OnModeChecked(object sender, RoutedEventArgs e)
        {
            var rule = Selected;
            if (_loading || rule == null) return;
            rule.Mode = sender == ModeOff ? "Off" : sender == ModeRed ? "Red" : "YellowRed";
            ShowProblem();
        }

        private void OnNoteChanged(object sender, TextChangedEventArgs e)
        {
            if (_loading || Selected == null) return;
            Selected.Note = NoteBox.Text;
            ShowProblem();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var rule = RuleEntry.New();
            _rules.Insert(RuleList.SelectedIndex + 1, rule);
            RuleText.Renumber(_rules);
            RuleList.SelectedItem = rule;
            ServerBox.Focus();
        }

        private void OnUp(object sender, RoutedEventArgs e) => Move(-1);
        private void OnDown(object sender, RoutedEventArgs e) => Move(1);

        private void Move(int offset)
        {
            int index = RuleList.SelectedIndex, target = index + offset;
            if (index < 0 || target < 0 || target >= _rules.Count) return;
            _rules.Move(index, target);
            RuleText.Renumber(_rules);
            RuleList.SelectedIndex = target;
            ShowSelected();
        }

        private void OnRemove(object sender, RoutedEventArgs e)
        {
            int index = RuleList.SelectedIndex;
            if (index < 0) return;
            _rules.RemoveAt(index);
            RuleText.Renumber(_rules);
            RuleList.SelectedIndex = Math.Min(index, _rules.Count - 1);
            ShowSelected();
        }

        private void OnEditAsText(object sender, RoutedEventArgs e)
        {
            var editor = new SettingsListDialog("Environment rules as text",
                "One rule per line: server | database | label | Off/Red/YellowRed | false | safe tables | trusted procedures. " +
                "A line starting with -- is the note for the rule below it. Choosing Save returns to the list; nothing is stored until you choose Save there.",
                CurrentText);
            editor.Save = LoadText;
            editor.ShowModal();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (CapturePreview.IsPreview(this)) return;
#endif
            var invalid = _rules.FirstOrDefault(rule => rule.HasProblem);
            if (invalid != null)
            {
                RuleList.SelectedItem = invalid;
                ShowError("Fix or remove the marked rules first. " + invalid.Problem);
                return;
            }
            try
            {
                Save(CurrentText);
                DialogResult = true;
            }
            catch (Exception exception) { ShowError(exception.Message); }
        }

        private void ShowError(string message)
        {
            Error.Text = message;
            Error.Visibility = Visibility.Visible;
        }
    }
}
