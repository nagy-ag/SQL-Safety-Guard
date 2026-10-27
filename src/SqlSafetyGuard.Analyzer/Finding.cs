using System;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// One classified action found in the analyzed text. Positions are relative to the analyzed text
    /// (1-based line and column, 0-based offset).
    /// </summary>
    public sealed class Finding
    {
        public Finding(int line, int column, int startOffset, int length, string action, string target,
            string resolvedTarget, Verdict verdict, string note, bool willRun = true, int? blockingErrorLine = null,
            bool affectsAllRows = false)
        {
            Line = line;
            Column = column;
            StartOffset = startOffset;
            Length = length;
            Action = action ?? string.Empty;
            Target = target ?? string.Empty;
            ResolvedTarget = string.IsNullOrEmpty(resolvedTarget) ? null : resolvedTarget;
            Verdict = verdict;
            Note = note ?? string.Empty;
            WillRun = willRun;
            BlockingErrorLine = blockingErrorLine;
            AffectsAllRows = affectsAllRows;
        }

        public int Line { get; }

        public int Column { get; }

        public int StartOffset { get; }

        public int Length { get; }

        /// <summary>For example UPDATE, DELETE, OUTPUT INTO, EXEC, CREATE PROCEDURE.</summary>
        public string Action { get; }

        /// <summary>The target as written in the query (may be an alias or CTE name).</summary>
        public string Target { get; }

        /// <summary>The base table the target resolves to (alias / CTE / derived table), or null when it is the target itself.</summary>
        public string ResolvedTarget { get; }

        public Verdict Verdict { get; }

        public string Note { get; }

        /// <summary>
        /// False when the statement is in a GO batch that has a syntax error: SQL Server compiles a batch as a
        /// whole, so nothing in it runs now. It will run once the error is fixed, so its verdict still counts.
        /// </summary>
        public bool WillRun { get; }

        /// <summary>Line of the syntax error that stops this statement's batch (when <see cref="WillRun"/> is false).</summary>
        public int? BlockingErrorLine { get; }

        /// <summary>Potential whole-target change: UPDATE/DELETE without WHERE, TRUNCATE, or DROP.</summary>
        public bool AffectsAllRows { get; }

        /// <summary>A PARSE ERROR or NOT ANALYZED row: describes the text rather than a statement that runs.</summary>
        public bool IsDiagnostic => Action == "PARSE ERROR" || Action == "NOT ANALYZED" || Action == "SQLCMD MODE OFF";

        /// <summary>"o → dbo.Orders" when resolved through an alias or CTE, otherwise the target as written.</summary>
        public string DisplayTarget =>
            ResolvedTarget != null && !string.Equals(ResolvedTarget, Target, StringComparison.OrdinalIgnoreCase)
                ? (Target.Length == 0 ? ResolvedTarget : Target + " → " + ResolvedTarget)
                : Target;

        public override string ToString() =>
            $"{Line}:{Column} {Action} {DisplayTarget} [{Verdict}]{(WillRun ? string.Empty : " (won't run now)")}{(Note.Length > 0 ? " " + Note : string.Empty)}";
    }

    /// <summary>A syntax error reported by the T-SQL parser.</summary>
    public sealed class SqlParseError
    {
        public SqlParseError(int line, int column, int offset, int number, string message)
        {
            Line = line;
            Column = column;
            Offset = offset;
            Number = number;
            Message = message ?? string.Empty;
        }

        public int Line { get; }

        public int Column { get; }

        public int Offset { get; }

        public int Number { get; }

        public string Message { get; }

        public override string ToString() => $"Line {Line}, column {Column}: {Message}";
    }

    /// <summary>A part of the text that could not be parsed, so its statements were not analyzed.</summary>
    public sealed class SkippedRegion
    {
        public SkippedRegion(int startLine, int endLine, int startOffset, int length, string reason)
        {
            StartLine = startLine;
            EndLine = endLine;
            StartOffset = startOffset;
            Length = length;
            Reason = reason ?? string.Empty;
        }

        public int StartLine { get; }

        public int EndLine { get; }

        public int StartOffset { get; }

        public int Length { get; }

        public string Reason { get; }

        public string Lines => StartLine == EndLine ? "line " + StartLine : $"lines {StartLine}–{EndLine}";

        public override string ToString() => $"{Lines}: {Reason}";
    }
}
