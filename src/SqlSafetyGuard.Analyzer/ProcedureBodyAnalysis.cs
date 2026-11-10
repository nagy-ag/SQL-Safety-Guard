namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Potential effects when a procedure is called, separate from submitting its definition.
    /// Positions use the same coordinates as the containing analysis. Complete means the visible body
    /// was walked, not that all runtime behavior is known; Unknown findings still apply.
    /// </summary>
    public sealed class ProcedureBodyAnalysis
    {
        public ProcedureBodyAnalysis(Finding definition, AnalysisResult analysis, bool isComplete)
        {
            Definition = definition;
            Analysis = analysis;
            IsComplete = isComplete;
        }

        public Finding Definition { get; }
        public AnalysisResult Analysis { get; }
        public bool IsComplete { get; }
    }
}
