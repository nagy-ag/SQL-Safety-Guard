namespace SqlSafetyGuard.Analyzer
{
    public static class SafetyDictionary
    {
        public const string Writes = "Changes permanent data or object definitions.";
        public const string Unknown = "Effects cannot be proven from the available SQL.";
        public const string Safe = "Read-only, temporary-only, or explicitly approved by your settings. Approval can include writes.";
        public const string Inside = "Potential operations inside code stored for later execution, such as procedure or function bodies.";
        public const string Remote = "Operations previewed for another server or provider. The remote execution remains separately assessed.";
        public const string ContextMeaning = "Changes database context, such as USE, without itself changing permanent data.";
        public const string Context = "Database context: changes name resolution, not permanent data. Later operations retain their own safety verdicts.";
        public const string Definition = "Code definition: CREATE/ALTER writes the definition now. The purple outline shows the full code container; counts beside it preview potential effects.";
        public const string Legend = "Red — permanent change. Yellow — effects not proven. Green — read-only, temporary-only, or explicitly approved by your settings.\nBlue circle — database context (USE). Purple circle — operations inside deferred code. Gray circle — remote previews. These scopes are not risk verdicts. Object preview totals stay separate from submitted-operation counts; preview rows show the actual risk mix. Purple outlines retain the full code container.\nDefinition — stores or changes code now. Call — requests execution. Dynamic SQL — decoded text runs at EXEC; its declaration is supporting source. Remote — separate execution context, previewed when visible.\nApproved write target / procedure / function — separate rules for server.database.schema.object. Approval accepts effects; it does not prove read-only behavior.";
    }
}
