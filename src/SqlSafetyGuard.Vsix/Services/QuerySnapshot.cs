using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Services
{
    /// <summary>
    /// The text SSMS would execute (selection, or the whole window) and where it starts in the editor,
    /// so analyzer positions can be mapped back to editor lines.
    /// </summary>
    internal sealed class QuerySnapshot
    {
        public QuerySnapshot(EnvDTE.Document document, string text, bool isSelection, int startLine, int startColumn)
        {
            Document = document;
            Text = text ?? string.Empty;
            IsSelection = isSelection;
            StartLine = startLine;
            StartColumn = startColumn;
        }

        public EnvDTE.Document Document { get; }

        public string Text { get; }

        public bool IsSelection { get; }

        /// <summary>1-based editor line of the first analyzed character.</summary>
        public int StartLine { get; }

        /// <summary>1-based editor column of the first analyzed character.</summary>
        public int StartColumn { get; }

        public int ToEditorLine(int analyzedLine) => StartLine + analyzedLine - 1;

        public int ToEditorColumn(int analyzedLine, int analyzedColumn) =>
            analyzedLine == 1 ? StartColumn + analyzedColumn - 1 : analyzedColumn;

        /// <summary>Editor line/column of an offset in the analyzed text.</summary>
        public void ToEditorPosition(int offset, out int line, out int column)
        {
            TextPosition.FromOffset(Text, offset, out var analyzedLine, out var analyzedColumn);
            line = ToEditorLine(analyzedLine);
            column = ToEditorColumn(analyzedLine, analyzedColumn);
        }

        public string Describe()
        {
            if (!IsSelection)
                return "whole window";
            // A selection that ends at column 1 of the next line does not include that line.
            TextPosition.FromOffset(Text, Text.TrimEnd('\r', '\n').Length, out var lastLine, out _);
            int first = StartLine, last = ToEditorLine(lastLine);
            return first == last ? $"selection (line {first})" : $"selection (lines {first}–{last})";
        }
    }
}
