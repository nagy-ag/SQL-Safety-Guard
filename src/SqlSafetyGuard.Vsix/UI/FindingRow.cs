using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.UI
{
    /// <summary>A finding with its position converted to editor coordinates, for the grid and the dialog.</summary>
    internal sealed class FindingRow
    {
        public FindingRow(Finding finding, QuerySnapshot snapshot)
        {
            Finding = finding;
            Line = snapshot.ToEditorLine(finding.Line);
            Column = snapshot.ToEditorColumn(finding.Line, finding.Column);
        }

        public Finding Finding { get; }

        public int Line { get; }

        public int Column { get; }

        public string LineText => "Line " + Line;

        public string Action => Finding.Action;

        public string Target => Finding.DisplayTarget;

        public string VerdictText => Finding.Verdict.DisplayName();

        public string Note => Finding.Note;

        public string NoteSuffix => string.IsNullOrEmpty(Finding.Note) ? string.Empty : "  (" + Finding.Note + ")";

        public Brush Marker => VerdictBrushes.For(Finding.Verdict);

        public bool IsWarning => !Finding.Verdict.IsGreen();

        /// <summary>False when the statement is in a batch with a syntax error (shown dimmed).</summary>
        public bool WillRun => Finding.WillRun;

        public Visibility AllRowsVisibility => Finding.AffectsAllRows ? Visibility.Visible : Visibility.Collapsed;

        public string AllRowsText => Finding.AllRowsSummary;
    }

    internal sealed class ConfirmationLedgerRow
    {
        private ConfirmationLedgerRow(IReadOnlyList<Finding> findings, QuerySnapshot snapshot)
        {
            var first = findings[0];
            string target = first.ResolvedTarget ?? first.Target;
            ObjectName = !string.IsNullOrEmpty(target) ? PaneGroup.ObjectDisplayName(target) :
                first.Action == "BEGIN TRAN" || first.Action == "COMMIT" ||
                first.Action == "ROLLBACK" || first.Action == "SAVE TRAN" ? "Transaction" : "—";
            Statements = string.Join(" · ", findings.GroupBy(finding => finding.Action)
                .Select(group => group.Key + (group.Count() > 1 ? " ×" + group.Count() : string.Empty)));
            Lines = string.Join(", ", findings.Select(finding => snapshot.ToEditorLine(finding.Line)).Distinct());
            Verdict = first.Verdict;
            Opacity = first.WillRun ? 1.0 : 0.55;
        }

        public string ObjectName { get; }
        public string Statements { get; }
        public string Lines { get; }
        public Verdict Verdict { get; }
        public double Opacity { get; }

        internal static IReadOnlyList<ConfirmationLedgerRow> Create(AnalysisResult result, QuerySnapshot snapshot, string database) =>
            result.Warnings.OrderBy(finding => finding.StartOffset)
                .GroupBy(finding => new
                {
                    Target = PaneGroup.Qualify(string.IsNullOrEmpty(finding.ResolvedTarget ?? finding.Target)
                        ? finding.Action : finding.ResolvedTarget ?? finding.Target, database).ToUpperInvariant(),
                    finding.Verdict,
                    finding.WillRun
                })
                .Select(group => new ConfirmationLedgerRow(group.ToList(), snapshot)).ToList();
    }
}
