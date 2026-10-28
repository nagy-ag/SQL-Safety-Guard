using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>Name classification and statement naming helpers.</summary>
    internal static class Classifier
    {
        /// <summary>System procedures that only read metadata or session state.</summary>
        public static readonly HashSet<string> ReadOnlyProcedures = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "sp_help", "sp_helptext", "sp_helpindex", "sp_who", "sp_who2", "sp_columns", "sp_tables",
            "sp_spaceused", "sp_lock", "sp_helpdb", "sp_depends", "sp_describe_first_result_set",
            "sp_describe_undeclared_parameters", "sp_helpconstraint", "sp_helptrigger", "sp_helpfile",
            "sp_helpfilegroup", "sp_helpuser", "sp_helprole", "sp_helprolemember", "sp_helplogins",
            "sp_helpserver", "sp_fkeys", "sp_pkeys", "sp_stored_procedures", "sp_databases",
            "sp_column_privileges", "sp_table_privileges", "sp_server_info", "sp_statistics",
            "sp_readerrorlog", "xp_readerrorlog", "sp_monitor", "sp_helpsrvrole", "sp_helpsrvrolemember",
            "sp_helpntgroup", "sp_helpdbfixedrole", "sp_helprotect", "sp_helpsort", "sp_helplanguage",
            "sp_help_job", "sp_help_jobstep", "sp_help_jobhistory", "sp_help_schedule", "sp_helpextendedproc",
            "sp_set_session_context", "sp_getapplock", "sp_releaseapplock", "sp_sproc_columns", "sp_special_columns",
        };

        /// <summary>
        /// System procedures known to change something. Used only after checking system qualification,
        /// including the explicit msdb procedure namespace (e.g. msdb.dbo.sp_add_job).
        /// </summary>
        public static readonly Dictionary<string, string> WritingProcedures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["sp_rename"] = "renames an object",
            ["sp_configure"] = "changes server configuration",
            ["xp_cmdshell"] = "runs an operating-system command",
            ["sp_send_dbmail"] = "sends an email",
            ["sp_OACreate"] = "creates an OLE Automation object (code outside SQL Server)",
            ["sp_OAMethod"] = "runs an OLE Automation method (code outside SQL Server)",
            ["sp_addrolemember"] = "changes role membership",
            ["sp_droprolemember"] = "changes role membership",
            ["sp_addsrvrolemember"] = "changes server role membership",
            ["sp_dropsrvrolemember"] = "changes server role membership",
            ["sp_addlogin"] = "creates a login",
            ["sp_droplogin"] = "drops a login",
            ["sp_password"] = "changes a password",
            ["sp_defaultdb"] = "changes a login's default database",
            ["sp_adduser"] = "creates a user",
            ["sp_dropuser"] = "drops a user",
            ["sp_grantdbaccess"] = "grants database access",
            ["sp_revokedbaccess"] = "removes database access",
            ["sp_grantlogin"] = "grants a Windows login",
            ["sp_revokelogin"] = "removes a Windows login",
            ["sp_denylogin"] = "denies a Windows login",
            ["sp_change_users_login"] = "remaps users to logins",
            ["sp_changedbowner"] = "changes the database owner",
            ["sp_changeobjectowner"] = "changes an object's owner",
            ["sp_addlinkedserver"] = "creates a linked server",
            ["sp_dropserver"] = "drops a linked server",
            ["sp_addlinkedsrvlogin"] = "changes linked-server logins",
            ["sp_droplinkedsrvlogin"] = "changes linked-server logins",
            ["sp_serveroption"] = "changes a server option",
            ["sp_addextendedproperty"] = "changes extended properties",
            ["sp_updateextendedproperty"] = "changes extended properties",
            ["sp_dropextendedproperty"] = "changes extended properties",
            ["sp_addextendedproc"] = "registers an extended procedure",
            ["sp_dropextendedproc"] = "removes an extended procedure",
            ["sp_bindrule"] = "binds a rule",
            ["sp_unbindrule"] = "unbinds a rule",
            ["sp_bindefault"] = "binds a default",
            ["sp_unbindefault"] = "unbinds a default",
            ["sp_tableoption"] = "changes a table option",
            ["sp_settriggerorder"] = "changes trigger order",
            ["sp_procoption"] = "changes a procedure option",
            ["sp_refreshview"] = "refreshes a view's metadata",
            ["sp_refreshsqlmodule"] = "refreshes a module's metadata",
            ["sp_recompile"] = "marks objects for recompilation",
            ["sp_attach_db"] = "attaches a database",
            ["sp_attach_single_file_db"] = "attaches a database",
            ["sp_detach_db"] = "detaches a database",
            ["sp_dboption"] = "changes a database option",
            ["sp_resetstatus"] = "resets a database's status",
            ["sp_cycle_errorlog"] = "starts a new error log",
            ["sp_delete_backuphistory"] = "deletes backup history",
            ["sp_purge_jobhistory"] = "deletes job history",
            ["sp_add_job"] = "creates a SQL Agent job",
            ["sp_update_job"] = "changes a SQL Agent job",
            ["sp_delete_job"] = "deletes a SQL Agent job",
            ["sp_add_jobstep"] = "changes a SQL Agent job",
            ["sp_update_jobstep"] = "changes a SQL Agent job",
            ["sp_delete_jobstep"] = "changes a SQL Agent job",
            ["sp_add_jobschedule"] = "changes a SQL Agent job",
            ["sp_add_schedule"] = "creates a SQL Agent schedule",
            ["sp_attach_schedule"] = "changes a SQL Agent job",
            ["sp_add_jobserver"] = "changes a SQL Agent job",
            ["sp_start_job"] = "starts a SQL Agent job",
            ["sp_stop_job"] = "stops a SQL Agent job",
            ["sp_trace_create"] = "creates a server trace",
            ["sp_trace_setstatus"] = "starts or stops a server trace",
            ["sp_createstats"] = "creates statistics",
            ["sp_updatestats"] = "updates statistics in the whole database",
            ["sp_MSforeachtable"] = "runs a command for every table",
            ["sp_MSforeachdb"] = "runs a command for every database",
            ["sp_create_streaming_job"] = "creates an external streaming job",
        };

        /// <summary>Statement types (or base types) that never change data.</summary>
        private static readonly HashSet<string> ReadOnlyStatementTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "SetOnOffStatement", "SetCommandStatement", "SetVariableStatement", "SetTransactionIsolationLevelStatement",
            "SetRowCountStatement", "SetTextSizeStatement", "SetErrorLevelStatement", "SetIdentityInsertStatement",
            "SetUserStatement", "DeclareVariableStatement", "DeclareTableVariableStatement", "DeclareCursorStatement",
            "CursorStatement", "OpenCursorStatement", "FetchCursorStatement", "CloseCursorStatement",
            "DeallocateCursorStatement", "TransactionStatement", "BeginTransactionStatement",
            "CommitTransactionStatement", "RollbackTransactionStatement", "SaveTransactionStatement",
            "PrintStatement", "RaiseErrorStatement", "RaiseErrorLegacyStatement", "ThrowStatement", "UseStatement",
            "ReturnStatement", "BreakStatement", "ContinueStatement", "GoToStatement", "LabelStatement",
            "ExecuteAsStatement", "RevertStatement", "WaitForStatement",
            // Read or session-only: READTEXT reads; keys are only opened/closed for this session; LINENO only
            // changes error line numbers; GET CONVERSATION GROUP locks a group; USE FEDERATION is like USE.
            "ReadTextStatement", "OpenSymmetricKeyStatement", "CloseSymmetricKeyStatement", "OpenMasterKeyStatement",
            "CloseMasterKeyStatement", "LineNoStatement", "GetConversationGroupStatement", "UseFederationStatement",
        };

        /// <summary>DBCC commands that only read or check (without a REPAIR option).</summary>
        private static readonly HashSet<DbccCommand> ReadOnlyDbcc = new HashSet<DbccCommand>
        {
            DbccCommand.ShowStatistics, DbccCommand.InputBuffer, DbccCommand.OutputBuffer, DbccCommand.OpenTran,
            DbccCommand.SqlPerf, DbccCommand.TraceStatus, DbccCommand.UserOptions, DbccCommand.ShowContig,
            DbccCommand.ShowFileStats, DbccCommand.MemoryStatus, DbccCommand.ProcCache, DbccCommand.Help,
            DbccCommand.CheckDB, DbccCommand.CheckTable, DbccCommand.CheckAlloc, DbccCommand.CheckCatalog,
            DbccCommand.CheckFileGroup, DbccCommand.CheckConstraints, DbccCommand.LogInfo, DbccCommand.ActiveCursors,
            DbccCommand.CursorStats, DbccCommand.ExtentInfo, DbccCommand.PerfMonStats, DbccCommand.CacheStats,
            DbccCommand.ShowText, DbccCommand.ShowOnRules, DbccCommand.ShowOffRules, DbccCommand.ShowWeights,
        };

        /// <summary>DBCC commands that change server or database state.</summary>
        private static readonly Dictionary<DbccCommand, string> ChangingDbcc = new Dictionary<DbccCommand, string>
        {
            [DbccCommand.FreeProcCache] = "clears the plan cache: every query recompiles",
            [DbccCommand.FreeSessionCache] = "clears the distributed query cache",
            [DbccCommand.FreeSystemCache] = "clears system caches",
            [DbccCommand.DropCleanBuffers] = "empties the buffer pool: data is read from disk again",
            [DbccCommand.ShrinkDatabase] = "shrinks database files",
            [DbccCommand.ShrinkFile] = "shrinks or empties a database file",
            [DbccCommand.CleanTable] = "reclaims space from dropped columns",
            [DbccCommand.DBReindex] = "rebuilds indexes",
            [DbccCommand.DBReindexAll] = "rebuilds indexes",
            [DbccCommand.IndexDefrag] = "defragments an index",
            [DbccCommand.UpdateUsage] = "corrects space usage counts",
            [DbccCommand.TraceOn] = "turns trace flags on",
            [DbccCommand.TraceOff] = "turns trace flags off",
            [DbccCommand.WritePage] = "writes directly to a database page",
            [DbccCommand.DBRepair] = "repairs (drops) a damaged database",
            [DbccCommand.DetachDB] = "detaches a database",
            [DbccCommand.ErrorLog] = "starts a new error log",
            [DbccCommand.FlushProcInDB] = "clears a database's plan cache",
            [DbccCommand.ForceGhostCleanup] = "forces ghost record cleanup",
            [DbccCommand.PinTable] = "pins a table in memory",
            [DbccCommand.UnpinTable] = "unpins a table",
            [DbccCommand.RenameColumn] = "renames a column",
            [DbccCommand.AddExtendedProc] = "registers an extended procedure",
            [DbccCommand.DropExtendedProc] = "removes an extended procedure",
            [DbccCommand.FreezeIO] = "freezes database I/O",
            [DbccCommand.ThawIO] = "resumes database I/O",
            [DbccCommand.MovePage] = "moves a database page",
            [DbccCommand.FixAllocation] = "changes allocation structures",
            [DbccCommand.DBRecover] = "recovers a database",
            [DbccCommand.ClearSpaceCaches] = "clears space caches",
            [DbccCommand.Free] = "unloads an extended procedure DLL",
            [DbccCommand.SetCpuWeight] = "changes CPU weighting",
            [DbccCommand.SetIOWeight] = "changes I/O weighting",
        };

        /// <summary>Read-only, changing or unknown, by DBCC command (and its options).</summary>
        public static Verdict ClassifyDbcc(DbccStatement dbcc, out string note)
        {
            var arguments = dbcc.Literals.Skip(1).Select(literal => literal.Value as Literal)
                .Where(literal => literal != null).Select(literal => literal.Value).ToList();
            if (dbcc.DllName != null)
            {
                note = "unloads an extended procedure DLL";
                return Verdict.RealWrite;
            }

            if (arguments.Any(argument => argument.StartsWith("REPAIR_", StringComparison.OrdinalIgnoreCase)))
            {
                note = "REPAIR options can delete data to fix corruption";
                return Verdict.RealWrite;
            }

            if (dbcc.Command == DbccCommand.CheckIdent)
            {
                bool noReseed = dbcc.Literals.Count >= 2 && dbcc.Literals[1].Value is IdentifierLiteral option &&
                    string.Equals(option.Value, "NORESEED", StringComparison.OrdinalIgnoreCase);
                note = noReseed ? "read-only DBCC command" : "may reseed the identity value";
                return noReseed ? Verdict.ReadOnly : Verdict.RealWrite;
            }

            if (dbcc.Command == DbccCommand.SqlPerf && arguments.Any(argument => string.Equals(argument, "CLEAR", StringComparison.OrdinalIgnoreCase)))
            {
                note = "clears SQL Server performance counters";
                return Verdict.RealWrite;
            }
            if (ReadOnlyDbcc.Contains(dbcc.Command))
            {
                note = "read-only DBCC command";
                return Verdict.ReadOnly;
            }

            if (ChangingDbcc.TryGetValue(dbcc.Command, out note))
                return Verdict.RealWrite;

            note = "DBCC command whose effect isn't known to the guard";
            return Verdict.Unknown;
        }

        /// <summary>Statement types whose effect the analyzer cannot see.</summary>
        private static readonly Dictionary<string, string> UnknownStatementTypes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["BackupStatement"] = "BACKUP writes backup files and can affect the log chain",
            ["UpdateStatisticsStatement"] = "updates statistics",
            ["CheckpointStatement"] = "writes dirty pages to disk",
        };

        private static readonly Dictionary<string, string> ActionNames = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["SetVariableStatement"] = "SET",
            ["DeclareVariableStatement"] = "DECLARE",
            ["DeclareTableVariableStatement"] = "DECLARE",
            ["DeclareCursorStatement"] = "DECLARE CURSOR",
            ["BeginTransactionStatement"] = "BEGIN TRAN",
            ["CommitTransactionStatement"] = "COMMIT",
            ["RollbackTransactionStatement"] = "ROLLBACK",
            ["SaveTransactionStatement"] = "SAVE TRAN",
            ["RaiseErrorStatement"] = "RAISERROR",
            ["RaiseErrorLegacyStatement"] = "RAISERROR",
            ["GoToStatement"] = "GOTO",
            ["DbccStatement"] = "DBCC",
            ["BackupDatabaseStatement"] = "BACKUP DATABASE",
            ["BackupTransactionLogStatement"] = "BACKUP LOG",
            ["ExecuteAsStatement"] = "EXECUTE AS",
            ["UpdateStatisticsStatement"] = "UPDATE STATISTICS",
            ["AlterTableAddTableElementStatement"] = "ALTER TABLE",
            ["AlterTableAlterColumnStatement"] = "ALTER TABLE",
            ["AlterTableDropTableElementStatement"] = "ALTER TABLE",
            ["AlterTableConstraintModificationStatement"] = "ALTER TABLE",
            ["AlterTableSetStatement"] = "ALTER TABLE",
            ["AlterTableSwitchStatement"] = "ALTER TABLE SWITCH",
            ["AlterTableRebuildStatement"] = "ALTER TABLE REBUILD",
            ["AlterTableTriggerModificationStatement"] = "ALTER TABLE",
            ["AlterTableChangeTrackingModificationStatement"] = "ALTER TABLE",
            ["AlterTableFileTableNamespaceStatement"] = "ALTER TABLE",
            ["AlterTableAlterIndexStatement"] = "ALTER TABLE",
            ["AlterTableAlterPartitionStatement"] = "ALTER TABLE",
            ["AlterTableAddClusterByStatement"] = "ALTER TABLE",
            ["BulkInsertStatement"] = "BULK INSERT",
            ["InsertBulkStatement"] = "INSERT BULK",
        };

        public static bool IsTempName(string name) => !string.IsNullOrEmpty(name) && name[0] == '#';

        public static bool IsTemp(SchemaObjectName name, AnalyzerOptions options)
        {
            if (name?.BaseIdentifier == null)
                return false;
            if (name.ServerIdentifier == null && IsTempName(name.BaseIdentifier.Value))
                return true;
            return options.TempdbIsTemp && name.ServerIdentifier == null &&
                   string.Equals(name.DatabaseIdentifier?.Value, "tempdb", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>TempOnly / AllowListed / RealWrite for a write to a named object.</summary>
        public static Verdict ClassifyWrite(SchemaObjectName name, AnalyzerOptions options, bool allowListApplies)
        {
            if (IsTemp(name, options))
                return Verdict.TempOnly;
            if (allowListApplies && name?.BaseIdentifier != null &&
                options.AllowList.Matches(name.ServerIdentifier?.Value, name.DatabaseIdentifier?.Value, name.SchemaIdentifier?.Value, name.BaseIdentifier.Value,
                    options.CurrentDatabase, options.DefaultSchema))
                return Verdict.AllowListed;
            return Verdict.RealWrite;
        }

        public static bool IsReadOnlyStatement(TSqlStatement statement) => TypeOrBaseNamed(statement.GetType(), ReadOnlyStatementTypes.Contains) != null;

        public static string UnknownStatementNote(TSqlStatement statement)
        {
            var typeName = TypeOrBaseNamed(statement.GetType(), UnknownStatementTypes.ContainsKey);
            return typeName == null ? null : UnknownStatementTypes[typeName];
        }

        public static string ActionName(TSqlStatement statement)
        {
            var type = statement.GetType();
            if (ActionNames.TryGetValue(type.Name, out var name))
                return name;
            if (TypeOrBaseNamed(type, n => n == "SetOnOffStatement" || n == "SetCommandStatement") != null)
                return "SET";
            if (TypeOrBaseNamed(type, n => n == "AlterTableStatement") != null)
                return "ALTER TABLE";

            var typeName = type.Name.EndsWith("Statement", StringComparison.Ordinal)
                ? type.Name.Substring(0, type.Name.Length - "Statement".Length)
                : type.Name;
            return Regex.Replace(typeName, "(?<=[a-z0-9])(?=[A-Z])", " ").ToUpperInvariant();
        }

        private static string TypeOrBaseNamed(Type type, Func<string, bool> predicate)
        {
            for (var t = type; t != null && t != typeof(TSqlStatement) && t != typeof(object); t = t.BaseType)
            {
                if (predicate(t.Name))
                    return t.Name;
            }

            return null;
        }
    }
}
