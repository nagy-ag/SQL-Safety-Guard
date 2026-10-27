using System.Collections.Generic;
using System.Linq;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class AnalysisResult
    {
        public AnalysisResult(IReadOnlyList<Finding> findings, IReadOnlyList<SqlParseError> parseErrors)
            : this(findings, parseErrors, new SkippedRegion[0])
        {
        }

        public AnalysisResult(IReadOnlyList<Finding> findings, IReadOnlyList<SqlParseError> parseErrors, IReadOnlyList<SkippedRegion> skippedRegions)
        {
            Findings = findings;
            ParseErrors = parseErrors;
            SkippedRegions = skippedRegions;

            // Findings in a failing batch count too: they run as soon as the syntax error is fixed.
            var overall = Verdict.ReadOnly;
            foreach (var f in findings)
                overall = VerdictExtensions.Max(overall, f.Verdict);
            if (parseErrors.Count > 0)
                overall = VerdictExtensions.Max(overall, Verdict.Unknown);
            Overall = overall;
        }

        public IReadOnlyList<Finding> Findings { get; }

        public IReadOnlyList<SqlParseError> ParseErrors { get; }

        /// <summary>Parts of the text that could not be parsed (their statements are not in <see cref="Findings"/>).</summary>
        public IReadOnlyList<SkippedRegion> SkippedRegions { get; }

        /// <summary>The most severe verdict of all findings (at least Unknown when there are parse errors).</summary>
        public Verdict Overall { get; }

        public bool IsGreen => Overall.IsGreen();

        public int Count(Verdict verdict) => Findings.Count(f => f.Verdict == verdict);

        /// <summary>Findings that are not green (Unknown and RealWrite), most severe first, in text order.</summary>
        public IEnumerable<Finding> Warnings =>
            Findings.Where(f => !f.Verdict.IsGreen()).OrderByDescending(f => f.Verdict).ThenBy(f => f.StartOffset);

        /// <summary>Non-green findings in batches that have a syntax error (they run once it is fixed).</summary>
        public int BlockedWarningCount => Findings.Count(f => !f.WillRun && !f.Verdict.IsGreen());

        /// <summary>Short summary such as "2 real writes, 1 unknown · 3 more in a batch that will fail" or "Read-only".</summary>
        public string Summary
        {
            get
            {
                if (IsGreen)
                {
                    switch (Overall)
                    {
                        case Verdict.TempOnly: return "Writes only to temp objects";
                        case Verdict.AllowListed: return "Writes only to allow-listed tables";
                        default: return "Read-only";
                    }
                }

                var parts = new List<string>();
                int writes = Findings.Count(f => f.WillRun && f.Verdict == Verdict.RealWrite);
                int unknown = Findings.Count(f => f.WillRun && f.Verdict == Verdict.Unknown && !f.IsDiagnostic);
                if (writes > 0)
                    parts.Add(writes == 1 ? "1 real write" : writes + " real writes");
                if (unknown > 0)
                    parts.Add(unknown + " unknown");
                if (ParseErrors.Count > 0)
                    parts.Add(ParseErrors.Count == 1 ? "1 syntax error" : ParseErrors.Count + " syntax errors");

                var text = string.Join(", ", parts);
                int blocked = BlockedWarningCount;
                if (blocked > 0)
                {
                    var more = writes + unknown > 0 ? "more " : string.Empty;
                    text += (text.Length == 0 ? string.Empty : " · ") + $"{blocked} {more}in a batch that will fail";
                }

                return text;
            }
        }
    }
}
