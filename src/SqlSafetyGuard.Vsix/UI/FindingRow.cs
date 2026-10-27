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

        public string AllRowsText => Finding.Action == "TRUNCATE TABLE"
            ? "Removes every row of " + Finding.DisplayTarget
            : "No WHERE clause: affects every row of " + Finding.DisplayTarget;
    }
}
