using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class GuardSettings
    {
        public GuardSettings(ExecutionMode mode = ExecutionMode.YellowRed, string environment = "Global",
            bool requireTypedConfirmation = false, IEnumerable<string> allowList = null, IEnumerable<string> trustedProcedures = null,
            string matchedRule = null)
        {
            Mode = mode;
            Environment = environment;
            RequireTypedConfirmation = false;
            AllowList = (allowList ?? Enumerable.Empty<string>()).ToArray();
            TrustedProcedures = (trustedProcedures ?? Enumerable.Empty<string>()).ToArray();
            MatchedRule = matchedRule;
        }

        public ExecutionMode Mode { get; }
        public string Environment { get; }
        public bool RequireTypedConfirmation { get; }
        public IReadOnlyList<string> AllowList { get; }
        public IReadOnlyList<string> TrustedProcedures { get; }
        public string MatchedRule { get; }

        public bool ShouldWarn(AnalysisResult result) => Mode != ExecutionMode.Off &&
            (Mode == ExecutionMode.Red ? result.Overall == Verdict.RealWrite : !result.IsGreen);

        public bool NeedsTypedConfirmation(AnalysisResult result) => false;

        public GuardSettings Add(GuardSettings global) => new GuardSettings(Mode, Environment, RequireTypedConfirmation,
            global.AllowList.Concat(AllowList), global.TrustedProcedures.Concat(TrustedProcedures), MatchedRule);
    }
}
