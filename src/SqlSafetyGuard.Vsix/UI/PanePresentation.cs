using System;
using System.Collections.Generic;
using System.Linq;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    // Display-only. Never pass a filtered collection to the execution guard or analyzer.
    [Flags]
    internal enum PaneVerdictFilter { None = 0, Writes = 1, Unknown = 2, Safe = 4, Inside = 8, Context = 16, Remote = 32, All = 63 }
    internal enum PaneFilterLabels { Automatic, Text, Circles }

    internal static class PaneFilterRules
    {
        public static bool Includes(PaneVerdictFilter filter, Finding finding) => finding.IsContextChange
            ? (filter & PaneVerdictFilter.Context) != 0 : Includes(filter, finding.Verdict);
        public static bool Includes(PaneVerdictFilter filter, Verdict verdict)
        {
            var bit = verdict.IsRed() ? PaneVerdictFilter.Writes : verdict.IsYellow() ? PaneVerdictFilter.Unknown : PaneVerdictFilter.Safe;
            return (filter & bit) != 0;
        }

        // Preserve every action, including several actions at the same source position.
        // Findings do not carry a universal parent-statement ID (notably inside dynamic SQL),
        // so deduplicating by line or span could silently discard operations.
        public static IReadOnlyList<T> Select<T>(IEnumerable<T> operations, PaneVerdictFilter filter, Func<T, Verdict> verdict) =>
            operations.Where(operation => Includes(filter, verdict(operation))).ToList();
    }

    // Context is a display category. The original analyzer verdicts remain unchanged.
    internal sealed class PaneDisplayCounts
    {
        public PaneDisplayCounts(IEnumerable<Finding> findings)
        {
            var values = findings.ToArray();
            Total = values.Length;
            Context = values.Count(f => f.IsContextChange);
            Writes = values.Count(f => !f.IsContextChange && f.Verdict.IsRed());
            Unknown = values.Count(f => !f.IsContextChange && f.Verdict.IsYellow());
            Safe = values.Count(f => !f.IsContextChange && f.Verdict.IsGreen());
        }
        public int Total { get; }
        public int Writes { get; }
        public int Unknown { get; }
        public int Safe { get; }
        public int Context { get; }
        public string Description => $"{Writes} permanent writes, {Unknown} unknown, {Safe} safe operations, {Context} context changes";
    }

    internal sealed class PaneVerdictCounts
    {
        public PaneVerdictCounts(IEnumerable<Verdict> verdicts)
        {
            var values = verdicts.ToList();
            Total = values.Count;
            Writes = values.Count(v => v.IsRed());
            Unknown = values.Count(v => v.IsYellow());
            Safe = values.Count(v => v.IsGreen());
        }
        public int Total { get; }
        public int Writes { get; }
        public int Unknown { get; }
        public int Safe { get; }
        public string Description => $"{Writes} permanent writes, {Unknown} unknown, {Safe} safe operations";
    }

    // Presentation provenance only: every operation and verdict still comes from the analyzer.
    internal static class PaneOperationPresentation
    {
        public static bool IsDecodedDynamic(Finding finding) =>
            finding.Note.StartsWith("inside dynamic SQL", StringComparison.Ordinal);

        public static bool IsDynamic(Finding finding) => IsDecodedDynamic(finding) ||
            (finding.Action.StartsWith("EXEC", StringComparison.Ordinal) &&
             (finding.Target == "dynamic SQL" || finding.Note.IndexOf("dynamic SQL", StringComparison.Ordinal) >= 0));

        // The analyzer maps decoded operations to their outer EXEC. Keep distinct EXECs separate,
        // and retain every child even when several have identical positions and actions.
        public static string DynamicKey(Finding finding) =>
            "dynamic:" + finding.StartOffset + ":" + finding.Length;
    }
}
