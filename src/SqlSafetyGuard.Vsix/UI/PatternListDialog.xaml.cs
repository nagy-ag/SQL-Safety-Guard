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
    /// Safe tables or trusted procedures as a list of named-part entries. Edits stay in the dialog until Save,
    /// which hands the composed text to the same validation as an import.
    /// </summary>
    public partial class PatternListDialog : DialogWindow
    {
        private readonly bool _procedures;
        private readonly string _title;
        private readonly ObservableCollection<PatternEntry> _entries = new ObservableCollection<PatternEntry>();
        private List<string> _trailing = new List<string>();
        private bool _loading;

        internal Action<string> Save { get; set; }

        internal PatternListDialog(string title, string description, string text, bool procedures)
        {
            InitializeComponent();
            DialogChrome.Apply(this);
            _procedures = procedures;
            _title = title;
            Title = "SQL Safety Guard — " + title;
            Description.Text = description;
            TrustWarning.Visibility = procedures ? Visibility.Visible : Visibility.Collapsed;
            NameLabel.Text = procedures ? "Procedure" : "Table";
            PartsHint.Text = procedures
                ? "Schema is required. * and ? are allowed only in the procedure name. Leave Server and Database empty for the usual case."
                : "Leave Server and Database empty for the usual case, and Schema empty to match any schema. * and ? are allowed in every part.";
            EntryList.ItemsSource = _entries;
            LoadText(text);
        }

        /// <summary>The stored-format text for the current entries.</summary>
        internal string CurrentText => PatternText.Compose(_entries, _trailing);

        internal IReadOnlyList<PatternEntry> Entries => _entries;

        internal void LoadText(string text)
        {
            _entries.Clear();
            foreach (var entry in PatternText.Parse(text, _procedures, out _trailing)) _entries.Add(entry);
            EntryList.SelectedIndex = _entries.Count > 0 ? 0 : -1;
            ShowSelected();
        }

        private PatternEntry Selected => EntryList.SelectedItem as PatternEntry;

        private void ShowSelected()
        {
            var entry = Selected;
            int index = EntryList.SelectedIndex;
            FormScroller.Visibility = entry == null ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = entry == null ? Visibility.Visible : Visibility.Collapsed;
            RemoveButton.IsEnabled = entry != null;
            UpButton.IsEnabled = index > 0;
            DownButton.IsEnabled = index >= 0 && index < _entries.Count - 1;
            if (entry == null) return;
            _loading = true;
            try
            {
                ServerBox.Text = entry.Server;
                DatabaseBox.Text = entry.Database;
                SchemaBox.Text = entry.Schema;
                NameBox.Text = entry.Name;
                NoteBox.Text = entry.Note;
            }
            finally { _loading = false; }
            ShowDetails();
        }

        private void ShowDetails()
        {
            var entry = Selected;
            if (entry == null) return;
            FormProblem.Text = entry.Problem ?? "";
            FormProblem.Visibility = entry.HasProblem ? Visibility.Visible : Visibility.Collapsed;
            ExplainText.Text = PatternText.Describe(entry);
            ExplainBox.Visibility = entry.HasProblem ? Visibility.Collapsed : Visibility.Visible;
            TryResult.Text = PatternText.Try(entry, TryBox.Text);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => ShowSelected();

        private void OnPartChanged(object sender, TextChangedEventArgs e)
        {
            var entry = Selected;
            if (_loading || entry == null) return;
            if (sender == ServerBox) entry.Server = ServerBox.Text;
            else if (sender == DatabaseBox) entry.Database = DatabaseBox.Text;
            else if (sender == SchemaBox) entry.Schema = SchemaBox.Text;
            else entry.Name = NameBox.Text;
            Error.Visibility = Visibility.Collapsed;
            ShowDetails();
        }

        private void OnNoteChanged(object sender, TextChangedEventArgs e)
        {
            if (_loading || Selected == null) return;
            Selected.Note = NoteBox.Text;
            ShowDetails();
        }

        private void OnTryChanged(object sender, TextChangedEventArgs e) => ShowDetails();

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var entry = new PatternEntry(_procedures);
            _entries.Insert(EntryList.SelectedIndex + 1, entry);
            EntryList.SelectedItem = entry;
            (_procedures ? SchemaBox : NameBox).Focus();
        }

        private void OnUp(object sender, RoutedEventArgs e) => Move(-1);
        private void OnDown(object sender, RoutedEventArgs e) => Move(1);

        private void Move(int offset)
        {
            int index = EntryList.SelectedIndex, target = index + offset;
            if (index < 0 || target < 0 || target >= _entries.Count) return;
            _entries.Move(index, target);
            EntryList.SelectedIndex = target;
            ShowSelected();
        }

        private void OnRemove(object sender, RoutedEventArgs e)
        {
            int index = EntryList.SelectedIndex;
            if (index < 0) return;
            _entries.RemoveAt(index);
            EntryList.SelectedIndex = Math.Min(index, _entries.Count - 1);
            ShowSelected();
        }

        private void OnEditAsText(object sender, RoutedEventArgs e)
        {
            var editor = new SettingsListDialog(_title + " as text",
                "One entry per line. A line starting with -- is the note for the entry below it. Choosing Save returns to the list; " +
                "nothing is stored until you choose Save there.", CurrentText);
            editor.Save = LoadText;
            editor.ShowModal();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (CapturePreview.IsPreview(this)) return;
#endif
            var invalid = _entries.FirstOrDefault(entry => entry.HasProblem);
            if (invalid != null)
            {
                EntryList.SelectedItem = invalid;
                ShowError("Fix or remove the marked entries first. " + invalid.Problem);
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
