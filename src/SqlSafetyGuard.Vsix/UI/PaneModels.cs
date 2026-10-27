using System;
using System.Collections.Generic;
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

        public PaneFinding(Finding finding, ITextSnapshot snapshot, int baseOffset)
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
            GroupKey = string.IsNullOrEmpty(target) ? finding.Action : target;
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

    /// <summary>All yellow/red findings on one object (table, procedure...).</summary>
    internal sealed class PaneGroup
    {
        public PaneGroup(string key, IReadOnlyList<PaneFinding> findings)
        {
            Key = key;
            Findings = findings.OrderBy(f => f.Finding.StartOffset).ToList();
            Verdict = findings.Max(f => f.Finding.Verdict);
            FirstOffset = Findings[0].Finding.StartOffset;
        }

        public string Key { get; }

        public IReadOnlyList<PaneFinding> Findings { get; }

        public Verdict Verdict { get; }

        public int FirstOffset { get; }

        public string Name => Key;

        public Brush Marker => VerdictBrushes.For(Verdict);

        public Visibility AllRowsVisibility => Findings.Any(f => f.Finding.AffectsAllRows) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>DROP for dropped objects; ALL ROWS for TRUNCATE and UPDATE/DELETE that can change every row.</summary>
        public string BadgeText => Findings.Any(f => f.Finding.AffectsAllRows && !f.Finding.Action.StartsWith("DROP ", StringComparison.Ordinal))
            ? "ALL ROWS"
            : "DROP";

        public string Subtitle
        {
            get
            {
                var actions = Findings.GroupBy(f => f.Finding.Action)
                    .Select(g => g.Count() == 1 ? g.Key : $"{g.Key} ×{g.Count()}");
                var lines = Findings.Select(f => f.EditorLine).Distinct().ToList();
                var shown = string.Join(", ", lines.Take(4)) + (lines.Count > 4 ? $" +{lines.Count - 4} more" : string.Empty);
                return string.Join(", ", actions) + " · " + (lines.Count == 1 ? "line " : "lines ") + shown;
            }
        }

        public override string ToString() => Key; // Name for UI Automation.

        public string DetailTitle => $"{Key} · {(Findings.Count == 1 ? "1 statement" : Findings.Count + " statements")}";
    }
}
