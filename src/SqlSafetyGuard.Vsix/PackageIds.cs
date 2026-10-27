using System;

namespace SqlSafetyGuard
{
    internal static class PackageGuids
    {
        public const string PackageString = "71fd7bec-2851-4c61-92a4-4f2435cbd7a1";
        public const string CommandSetString = "84195f28-a147-4898-bcad-b1ad3c744ae2";
        public const string OutputPaneString = "4afdb3c6-06fe-4170-ae23-32d940ee4062";
        public const string OptionsPageString = "3d1b0a52-6f83-4c55-9a3e-2b7f0c9e1d44";

        public static readonly Guid CommandSet = new Guid(CommandSetString);
        public static readonly Guid OutputPane = new Guid(OutputPaneString);
    }

    internal static class PackageIds
    {
        public const int AnalyzeQueryCommand = 0x0100;
    }

    /// <summary>SSMS SQL editor command identifiers (from SQLEditors.dll's command table, SSMS 22).</summary>
    internal static class SsmsCommands
    {
        public const string ExecuteCommandName = "Query.Execute";

        /// <summary>
        /// guidQueryEditorCmd: Query.Execute is id 1, the "Available Databases" combo is id 4,
        /// Query.SQLCMDMode (a toggle) is id 17.
        /// </summary>
        public static readonly Guid QueryEditorCommandSet = new Guid("52692960-56BC-4989-B5D3-94C47A513E8D");

        public const int ExecuteQuery = 0x0001;
        public const int AvailableDatabasesCombo = 0x0004;
        public const uint SqlCmdMode = 0x0011;
    }
}
