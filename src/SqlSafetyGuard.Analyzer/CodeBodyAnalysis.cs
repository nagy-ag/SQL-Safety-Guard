namespace SqlSafetyGuard.Analyzer
{
    public enum CodeBodyKind { Procedure, Trigger, ScalarFunction, InlineTableFunction, TableFunction, View, RemoteQuery, JobCommand }
    /// <summary>
    /// A visible module body or embedded command, separate from its containing operation.
    /// Positions use the same coordinates as the containing analysis. Complete means the visible body
    /// was walked, not that all runtime behavior is known; Unknown findings still apply.
    /// </summary>
    public sealed class CodeBodyAnalysis
    {
        public CodeBodyAnalysis(Finding definition, AnalysisResult analysis, bool isComplete, CodeBodyKind kind = CodeBodyKind.Procedure)
        {
            Definition = definition;
            Analysis = analysis;
            IsComplete = isComplete;
            Kind = kind;
        }

        public Finding Definition { get; }
        public AnalysisResult Analysis { get; }
        public bool IsComplete { get; }
        public CodeBodyKind Kind { get; }
        public bool IsEmbedded => Kind == CodeBodyKind.RemoteQuery || Kind == CodeBodyKind.JobCommand;
        public string OperationLabel => Kind == CodeBodyKind.RemoteQuery ?
            (Definition.Verdict.IsRed() ? Definition.Action + " \u00b7 remote query" : "Remote query") :
            Kind == CodeBodyKind.JobCommand ? "Stored command" : "Definition";
        public string ContextLabel => Kind == CodeBodyKind.Trigger ? "When fired" : Kind == CodeBodyKind.View ? "Query when read" :
            Kind == CodeBodyKind.RemoteQuery ? "Remote SQL" : Kind == CodeBodyKind.JobCommand ? "Stored command" :
            Kind == CodeBodyKind.Procedure ? "Visible body" : "Body when called";
        public string ContextDescription => Kind == CodeBodyKind.RemoteQuery ? "T-SQL interpretation only; the remote dialect and runtime are unverified." :
            Kind == CodeBodyKind.JobCommand ? "Stored for later job execution; this command does not run when the step is changed." :
            Kind == CodeBodyKind.Trigger ? "Potential effects when the trigger fires; defining it does not execute its body." :
            "Potential effects of the visible body; separate from submitting its definition.";
    }
}
