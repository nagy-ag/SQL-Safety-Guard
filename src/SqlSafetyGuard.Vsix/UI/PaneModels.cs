using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    /// <summary>One finding, tracked in the editor buffer so highlights follow edits until the next analysis.</summary>
    internal sealed class PaneFinding
    {
        private readonly ITrackingSpan _span;

        public PaneFinding(Finding finding, ITextSnapshot snapshot, int baseOffset, string currentDatabase = null)
        {
            Finding = finding;
            int start = Math.Min(snapshot.Length, baseOffset + finding.StartOffset);
            int length = Math.Min(snapshot.Length - start, finding.Length);
            var span = new Span(start, length);
            if (length == 0)
                span = snapshot.GetLineFromPosition(start).Extent.Span; // Parse errors etc.: the whole line.
            _span = snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);

            EditorLine = snapshot.GetLineNumberFromPosition(span.Start) + 1;
            var firstLine = snapshot.GetLineFromPosition(span.Start);
            var snippet = snapshot.GetText(Span.FromBounds(span.Start, Math.Min(span.End, firstLine.End.Position))).Trim();
            bool more = span.End > firstLine.End.Position;
            Snippet = snippet.Length > 70 ? snippet.Substring(0, 70) + "…" : more ? snippet + " …" : snippet;

            var target = finding.ResolvedTarget ?? finding.Target;
            GroupKey = string.IsNullOrEmpty(target) ? finding.Action : PaneGroup.Qualify(target, currentDatabase);
        }

        public Finding Finding { get; }

        public int EditorLine { get; }

        public string GroupKey { get; }

        public string Snippet { get; }

        public string Header => $"Line {EditorLine} · {Finding.Action}";

        /// <summary>The whole-target warning (same wording as the F5 dialog) followed by the analyzer's note.</summary>
        public string Note
        {
            get
            {
                if (!Finding.AffectsAllRows)
                    return Finding.Note;
                bool noteRepeatsSummary = string.IsNullOrEmpty(Finding.Note) || Finding.Note.Contains(Finding.CteOrDerivedTableLimitNote);
                return noteRepeatsSummary ? Finding.AllRowsSummary : Finding.AllRowsSummary + " · " + Finding.Note;
            }
        }

        public Visibility NoteVisibility => string.IsNullOrEmpty(Note) ? Visibility.Collapsed : Visibility.Visible;

        public Brush Marker => VerdictBrushes.For(Finding.Verdict);

        public string SafeText => $"Line {EditorLine} · {Finding.Action} {Finding.DisplayTarget}".TrimEnd();

        public SnapshotSpan SpanIn(ITextSnapshot snapshot) => _span.GetSpan(snapshot);

        public override string ToString() => SafeText; // Name for UI Automation.
    }

    internal sealed class PaneGroup : INotifyPropertyChanged
    {
        private bool _isExpanded;

        public PaneGroup(string key, IReadOnlyList<PaneFinding> findings)
        {
            Key = key;
            Findings = findings.OrderBy(finding => finding.Finding.StartOffset).ToList();
            Verdict = findings.Max(finding => finding.Finding.Verdict);
            FirstOffset = Findings[0].Finding.StartOffset;
            var parts = SplitName(key).Select(Unquote).ToList();
            int prefix = Math.Max(0, parts.Count - 2);
            DatabasePrefix = prefix == 0 ? string.Empty : string.Join(".", parts.Take(prefix)) + ".";
            ObjectName = Findings.All(finding => string.IsNullOrEmpty(finding.Finding.ResolvedTarget ?? finding.Finding.Target))
                ? Findings.All(finding => finding.Finding.Action == "BEGIN TRAN" || finding.Finding.Action == "COMMIT" ||
                    finding.Finding.Action == "ROLLBACK" || finding.Finding.Action == "SAVE TRAN") ? "Transaction" : "—"
                : string.Join(".", parts.Skip(prefix));
        }

        public string Key { get; }
        public IReadOnlyList<PaneFinding> Findings { get; }
        public Verdict Verdict { get; }
        public int FirstOffset { get; }
        public string DatabasePrefix { get; }
        public string ObjectName { get; }
        public string DisplayName => DatabasePrefix + ObjectName;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
            }
        }
        public Brush Marker => VerdictBrushes.For(Verdict);
        public bool IsAllowed => Verdict.IsGreen() && Findings.Any(finding =>
            finding.Finding.Verdict == Verdict.AllowListed ||
            (finding.Finding.Verdict == Verdict.ReadOnly && (finding.Finding.Note ?? string.Empty).IndexOf("trusted procedure", StringComparison.OrdinalIgnoreCase) >= 0));
        public string Section => Verdict.IsRed() ? "CHANGES TO PERMANENT OBJECTS" : Verdict.IsYellow() ? "CAN'T SEE INSIDE" :
            IsAllowed ? "ALLOWED BY YOUR SETTINGS" : "Safe";
        public bool IsWholeTarget => Findings.Any(finding => finding.Finding.Verdict.IsRed() && (finding.Finding.AffectsAllRows ||
            finding.Finding.Action.StartsWith("TRUNCATE", StringComparison.OrdinalIgnoreCase) ||
            finding.Finding.Action.StartsWith("DROP ", StringComparison.OrdinalIgnoreCase)));
        public string Reason => Verdict == Verdict.TempOnly ? "Temporary-object operations only. No permanent-object changes detected." :
            Findings.OrderByDescending(finding => finding.Finding.Verdict)
            .ThenByDescending(finding => finding.Finding.AffectsAllRows).Select(finding => finding.Note).FirstOrDefault(note => !string.IsNullOrEmpty(note)) ??
            (Verdict.IsRed() ? "Changes a permanent object." : Verdict.IsYellow() ? "The analyzer cannot see what runs." : "No permanent changes detected.");
        public string Subtitle
        {
            get
            {
                var actions = Findings.Select(finding => finding.Finding.Action).Distinct().ToList();
                return actions[0] + (actions.Count > 1 ? " +" + (actions.Count - 1) :
                    Findings.Count > 1 ? " ×" + Findings.Count : string.Empty);
            }
        }
        public int Line => Findings[0].EditorLine;
        public string OperationSummary => string.Join(" · ", Findings.GroupBy(finding => finding.Finding.Action)
            .Select(action => action.Key + (action.Count() > 1 ? " ×" + action.Count() : string.Empty)));
        public string LineSummary => "Lines " + string.Join(", ", Findings.Select(finding => finding.EditorLine).Distinct());
        public Visibility MultipleFindingsVisibility => Findings.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        public string Tooltip => Key + Environment.NewLine + string.Join(Environment.NewLine,
            Findings.Select(finding => finding.Header + (string.IsNullOrEmpty(finding.Note) ? string.Empty : " · " + finding.Note)));
        public override string ToString() => Key;

        /// <summary>Normalizes a target to [database].[schema].[name] so the same object groups together however it was written.
        /// #temp tables and @table variables are left as written: they don't belong to the current database.</summary>
        internal static string Qualify(string target, string database)
        {
            var parts = SplitName(target);
            string last = Unquote(parts[parts.Count - 1]);
            if (last.StartsWith("#", StringComparison.Ordinal) || last.StartsWith("@", StringComparison.Ordinal))
                return target;
            if (parts.Count < 3 && !string.IsNullOrEmpty(database))
            {
                if (parts.Count == 1) parts.Insert(0, string.Empty);
                parts.Insert(0, "[" + database.Replace("]", "]]") + "]");
            }
            return string.Join(".", parts.Select(part =>
            {
                string value = Unquote(part);
                return value.Length == 0 ? string.Empty : "[" + value.Replace("]", "]]") + "]";
            }));
        }

        /// <summary>One name part without [brackets] or "quotes", for display.</summary>
        private static string Unquote(string part)
        {
            string value = part.Trim();
            if (value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal))
                return value.Substring(1, value.Length - 2).Replace("]]", "]");
            if (value.Length > 1 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
                return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            return value;
        }

        internal static string ObjectDisplayName(string target) =>
            string.Join(".", SplitName(target).Select(Unquote).Reverse().Take(2).Reverse());

        private static List<string> SplitName(string name)
        {
            var parts = new List<string>();
            int start = 0;
            char quote = '\0';
            for (int index = 0; index < name.Length; index++)
            {
                char character = name[index];
                if (quote != '\0')
                {
                    if (character != quote) continue;
                    if (index + 1 < name.Length && name[index + 1] == quote) { index++; continue; }
                    quote = '\0';
                }
                else if (character == '[' || character == '"') quote = character == '[' ? ']' : '"';
                else if (character == '.') { parts.Add(name.Substring(start, index - start)); start = index + 1; }
            }
            parts.Add(name.Substring(start));
            return parts;
        }
    }

    internal sealed class PanePattern
    {
        public PanePattern(string pattern, string source) { Pattern = pattern; Source = source; }
        public string Pattern { get; }
        public string Source { get; }
    }

    internal sealed class PaneSettings
    {
        public PaneSettings(string server, string database, GuardSettings settings, AnalyzerOptions options,
            int globalTables, int globalProcedures)
        {
            Server = server ?? "Unknown";
            Database = database ?? "Unknown";
            EnvironmentLabel = settings.MatchedRule == null ? null : settings.Environment;
            EnvironmentDescription = EnvironmentLabel == null ? "global settings" : EnvironmentLabel + " rule · " + settings.MatchedRule;
            WhenToAsk = settings.Mode == ExecutionMode.Off ? "Off" : settings.Mode == ExecutionMode.Red ? "Red only" : "Red and yellow";
            TypedConfirmation = "No typing required";
            Tempdb = options.TempdbIsTemp ? "Treated as temp" : "Not treated as temp";
            SafeTables = options.AllowList.Patterns.Select((pattern, index) => new PanePattern(pattern,
                index < globalTables ? "global" : EnvironmentLabel + " rule")).ToList();
            TrustedProcedures = options.TrustedProcedures.Patterns.Select((pattern, index) => new PanePattern(pattern,
                index < globalProcedures ? "global" : EnvironmentLabel + " rule")).ToList();
        }
        public string Server { get; }
        public string Database { get; }
        public string EnvironmentLabel { get; }
        public string EnvironmentDescription { get; }
        public string WhenToAsk { get; }
        public string TypedConfirmation { get; }
        public string Tempdb { get; }
        public IReadOnlyList<PanePattern> SafeTables { get; }
        public IReadOnlyList<PanePattern> TrustedProcedures { get; }
        public string SafeTablesHeading => "Safe tables " + SafeTables.Count;
        public string TrustedProceduresHeading => "Trusted procedures " + TrustedProcedures.Count;
    }
}
