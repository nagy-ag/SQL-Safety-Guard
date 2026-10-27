using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    internal struct Position
    {
        public Position(int line, int column, int offset, int length)
        {
            Line = line;
            Column = column;
            Offset = offset;
            Length = length;
        }

        public int Line { get; }

        public int Column { get; }

        public int Offset { get; }

        public int Length { get; }
    }

    /// <summary>
    /// The text currently being analyzed and how its positions map back to the original text:
    /// a GO batch parsed on its own (offset/line delta) or literal dynamic SQL (every finding is
    /// reported at the position of the EXEC that runs it).
    /// </summary>
    internal sealed class Scope
    {
        private const int MaxTargetTextLength = 200;

        private Scope(string text, string displayText, int offsetDelta, int lineDelta, Position? reportAt, int dynamicDepth, int? blockingErrorLine,
            IReadOnlyDictionary<string, string> variables)
        {
            Variables = variables;
            Text = text;
            DisplayText = displayText ?? text;
            OffsetDelta = offsetDelta;
            LineDelta = lineDelta;
            ReportAt = reportAt;
            DynamicDepth = dynamicDepth;
            BlockingErrorLine = blockingErrorLine;
        }

        /// <summary>The text that is parsed (SQLCMD lines blanked, $(variables) replaced; same length as <see cref="DisplayText"/>).</summary>
        public string Text { get; }

        /// <summary>The text as the user wrote it; used to show targets.</summary>
        public string DisplayText { get; }

        public int OffsetDelta { get; }

        public int LineDelta { get; }

        /// <summary>Set for dynamic SQL: all findings are reported at this position of the outer text.</summary>
        public Position? ReportAt { get; }

        public int DynamicDepth { get; }

                public bool IsDynamic => DynamicDepth > 0;

        /// <summary>SQLCMD :setvar values (null when the script has none).</summary>
        public IReadOnlyDictionary<string, string> Variables { get; }

        /// <summary>"$(Table)" with known :setvar values substituted, or null when nothing changes.</summary>
        public string SubstituteVariables(string text)
        {
            if (Variables == null || string.IsNullOrEmpty(text) || text.IndexOf("$(", System.StringComparison.Ordinal) < 0)
                return null;
            var result = Regex.Replace(text, @"\$\((?<name>[A-Za-z_][A-Za-z0-9_]*)\)",
                m => Variables.TryGetValue(m.Groups["name"].Value, out var value) ? value : m.Value);
            return result == text ? null : result;
        }

        /// <summary>Set when this text is a batch with a syntax error: nothing in it runs until the error is fixed.</summary>
        public int? BlockingErrorLine { get; }

        public static Scope ForText(string text, string displayText = null, IReadOnlyDictionary<string, string> variables = null) =>
            new Scope(text, displayText, 0, 0, null, 0, null, variables);

        public Scope ForBatch(int startOffset, int length, int lineBreaksBefore) =>
            new Scope(Text.Substring(startOffset, length), DisplayText.Substring(startOffset, length),
                OffsetDelta + startOffset, LineDelta + lineBreaksBefore, ReportAt, DynamicDepth, BlockingErrorLine, Variables);

        public Scope WithText(string parsedText) =>
            new Scope(parsedText, DisplayText, OffsetDelta, LineDelta, ReportAt, DynamicDepth, BlockingErrorLine, Variables);

        public Scope Blocked(int errorLine) =>
            new Scope(Text, DisplayText, OffsetDelta, LineDelta, ReportAt, DynamicDepth, errorLine, Variables);

        public Scope ForDynamicSql(string sql, TSqlFragment executeStatement) =>
            new Scope(sql, sql, 0, 0, PositionOf(executeStatement), DynamicDepth + 1, BlockingErrorLine, null);

        public Position PositionOf(TSqlFragment fragment)
        {
            if (ReportAt.HasValue)
                return ReportAt.Value;
            if (fragment == null || fragment.StartOffset < 0)
                return new Position(1 + LineDelta, 1, OffsetDelta, 0);
            return new Position(fragment.StartLine + LineDelta, fragment.StartColumn, fragment.StartOffset + OffsetDelta, fragment.FragmentLength);
        }

        public Position PositionOf(int line, int column, int offset, int length = 0) =>
            ReportAt ?? new Position(line + LineDelta, column, offset + OffsetDelta, length);

        /// <summary>Position of an offset in this scope's text.</summary>
        public Position PositionOfOffset(int offset, int length = 0)
        {
            TextPosition.FromOffset(Text, offset, out var line, out var column);
            return PositionOf(line, column, offset, length);
        }

        /// <summary>The fragment's text as written (whitespace collapsed, truncated when very long).</summary>
        public string TextOf(TSqlFragment fragment)
        {
            if (fragment == null)
                return string.Empty;
            return TextOf(fragment.StartOffset, fragment.FragmentLength);
        }

        public string TextOf(int start, int length)
        {
            if (start < 0 || length <= 0 || start + length > DisplayText.Length)
                return string.Empty;

            var text = DisplayText.Substring(start, length).Trim();
            if (text.IndexOfAny(new[] { '\r', '\n', '\t' }) >= 0)
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
            if (text.Length > MaxTargetTextLength)
                text = text.Substring(0, MaxTargetTextLength) + "…";
            return text;
        }
    }
}
