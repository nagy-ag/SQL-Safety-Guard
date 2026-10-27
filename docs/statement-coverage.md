# T-SQL statement coverage

This document answers one question: of everything that can appear in a T-SQL query, what does SQL Safety Guard
handle, and how?

## How this was checked

1. **What exists:** the extension parses with Microsoft's own T-SQL parser, ScriptDom 180.x (`TSql180Parser`),
   which has one class per kind of statement. A reflection scan of that library found **334 concrete statement
   kinds**. That's the complete list of statements SQL Server / Azure SQL syntax allows, including SQL Server 2025
   additions such as `CREATE VECTOR INDEX`, `CREATE JSON INDEX` and `CREATE EXTERNAL MODEL`.
2. **What we do with each:** every kind was traced through the analyzer (`StatementWalker`, `Classifier`).
3. **Reality check:** about 100 real statements across all categories were run through the analyzer, comparing
   the verdict with what the statement actually does. All of them parsed.

First review 2026-10-26. The false alarms and under-warnings it found have since been fixed (see "Findings").

## Summary

| How it's handled | Statement kinds | Meaning |
|---|---|---|
| **Deep handling** | 42 | Read in detail. DML (SELECT/INSERT/UPDATE/DELETE/MERGE, BULK INSERT, TRUNCATE): targets resolved through aliases/CTEs/derived tables and classified temp/allow-listed/real, OUTPUT INTO, no-WHERE detection. Table/index/statistics DDL: temp or real. Control flow (IF/WHILE/BEGIN…END/TRY…CATCH/WAITFOR): walked into. EXEC: read-only and known-writing system procedures, literal dynamic SQL. DBCC: per command. Transactions: tracked across the script. |
| **Green list** | 38 | Never change data: SET options, DECLARE, cursors, PRINT/RAISERROR/THROW, USE, RETURN/BREAK/CONTINUE/GOTO, EXECUTE AS/REVERT, READTEXT, OPEN/CLOSE keys, LINENO, GET CONVERSATION GROUP, USE FEDERATION. |
| **Yellow** | 4 | Effect can't be seen: BACKUP DATABASE/LOG, UPDATE STATISTICS, CHECKPOINT. |
| **Red by default** | 250 | Everything else. Apart from about 5 parser-internal classes that never appear in a script, these all really change something: every CREATE/ALTER/DROP of an object, GRANT/REVOKE/DENY, RESTORE, KILL, SHUTDOWN, RECONFIGURE, Service Broker SEND/RECEIVE/…, WRITETEXT/UPDATETEXT, COPY INTO. |

**Bottom line:** every statement kind is handled; nothing passes silently.
- Anything not specifically recognized is red. That errs toward warning, so an unknown statement can't be wrongly
  shown as safe.
- After the fixes, the remaining gaps are the ones that need the server's metadata (end of this document).

## By category (from the real-statement check)

| Category | Examples | Result |
|---|---|---|
| Reads | `SELECT`, `READTEXT`, `SELECT … FROM sys.dm_*`, `OPENJSON`, `OPENROWSET(BULK …)` | Green. |
| Data changes | `INSERT`, `UPDATE`, `DELETE`, `MERGE`, `SELECT INTO`, `BULK INSERT`, `INSERT … OUTPUT`, `UPDATETEXT`, `WRITETEXT`, `COPY INTO` | Red, or green for temp/allow-listed targets. Aliases, CTEs, derived tables, OUTPUT INTO, no-WHERE all handled. |
| Table & index DDL | `CREATE/ALTER/DROP TABLE`, `CREATE/ALTER/DROP INDEX`, `CREATE/DROP STATISTICS`, `ALTER TABLE … SWITCH/REBUILD` | Red; temp objects green. |
| Other object DDL | sequences, synonyms, types, schemas, databases, partitions, full-text, XML/spatial/vector/JSON indexes, external objects | Red. |
| Modules | `CREATE/ALTER/DROP PROCEDURE/FUNCTION/TRIGGER/VIEW`, `CREATE OR ALTER …` | Red; the body isn't treated as running. |
| Security | `GRANT/REVOKE/DENY`, logins, users, roles, keys, certificates, `ALTER AUTHORIZATION` | Red. `OPEN/CLOSE SYMMETRIC KEY`, `OPEN/CLOSE MASTER KEY`, `EXECUTE AS`/`REVERT` green (session only). |
| Transactions | `BEGIN/COMMIT/ROLLBACK/SAVE TRAN`, `SET IMPLICIT_TRANSACTIONS` | Green when balanced. A `COMMIT` of a transaction started outside the text is yellow. A transaction left open at the end (explicit or implicit) gets a yellow **OPEN TRANSACTION** row. |
| Session settings | `SET NOCOUNT/NOEXEC/IDENTITY_INSERT/LANGUAGE/CONTEXT_INFO`, isolation level, `USE`, `LINENO` | Green. |
| Control flow | `IF/ELSE`, `WHILE`, `BEGIN…END`, `TRY…CATCH`, `WAITFOR`, `RETURN`, `GOTO`, `RAISERROR`, `THROW` | Walked into; writes inside branches are found. |
| Cursors & variables | `DECLARE/OPEN/FETCH/CLOSE/DEALLOCATE`, `UPDATE … WHERE CURRENT OF`, table variables | Handled. |
| Procedures & dynamic SQL | `EXEC proc`, `EXEC sp_help`, `sp_rename`, `xp_cmdshell`, `sp_executesql N'…'`, `EXEC(…) AT srv` | Unknown procedures yellow; read-only system procedures green; about 70 known-writing system procedures red, with the reason; literal dynamic SQL analyzed. |
| Service Broker | `SEND`, `RECEIVE`, `WAITFOR(RECEIVE…)`, `BEGIN DIALOG`, `END CONVERSATION`, `GET CONVERSATION GROUP` | Red (they change queues); `GET CONVERSATION GROUP` green. |
| Server & maintenance | `DBCC`, `BACKUP`, `RESTORE`, `CHECKPOINT`, `KILL`, `SHUTDOWN`, `RECONFIGURE` | DBCC per command (below). BACKUP/CHECKPOINT/UPDATE STATISTICS yellow; RESTORE/KILL/SHUTDOWN/RECONFIGURE red. |
| Inside expressions | `NEXT VALUE FOR`, `OPENQUERY/OPENROWSET/OPENDATASOURCE` as a source, scalar functions, 4-part-name reads | Sequences red, remote pass-through yellow, remote table reads green. Scalar functions green (see "Out of reach"). |
| Script-level | `GO` batches, syntax errors, SQLCMD lines, literal dynamic SQL (3 levels) | Covered (see README: recovery, "won't run now", SQLCMD mode detection). |

## Findings (all fixed)

### False alarms: 9, fixed

`READTEXT` · `OPEN SYMMETRIC KEY` · `CLOSE SYMMETRIC KEY` · `OPEN MASTER KEY` · `CLOSE MASTER KEY` · `LINENO` ·
`GET CONVERSATION GROUP` · `USE FEDERATION` are now green. `DROP STATISTICS` now follows the temp rules (green on
a #temp table). Tests: `FalseAlarmTests`.

### Under-warnings: fixed

1. **Transactions** are tracked across the whole script, including GO batches and literal dynamic SQL.
   - A `COMMIT` with no open transaction in the text is yellow: "commits a transaction started outside this text:
     uncommitted changes from an earlier run become permanent".
   - A `BEGIN TRAN` without a matching COMMIT/ROLLBACK, or `SET IMPLICIT_TRANSACTIONS ON` followed by changes and
     no COMMIT, gets a yellow **OPEN TRANSACTION** row at the place the transaction started.
   - The common patterns don't warn: `IF … COMMIT ELSE ROLLBACK`, TRY/CATCH with `IF @@TRANCOUNT > 0 ROLLBACK;
     THROW;`, and savepoints. Statements in a batch with a syntax error don't count, because they don't run.
   - Tests: `TransactionTests`.
2. **Known-writing system procedures** (about 70) are red with a reason: `sp_rename`, `sp_configure`, `xp_cmdshell`,
   `sp_send_dbmail`, role/login/user procedures, SQL Agent job procedures, `sp_MSforeachtable`, …. The list of
   read-only procedures also grew (`sp_readerrorlog`, `sp_help_job`, `sp_set_session_context`, …). Tests:
   `SystemProcedureTests`.
3. **DBCC per command:**
   - Read-only commands are green: `SHOW_STATISTICS`, `INPUTBUFFER`, `OPENTRAN`, `SQLPERF`, `USEROPTIONS`,
     `TRACESTATUS`, and `CHECKDB/CHECKTABLE/…` without REPAIR.
   - Changing ones are red with a reason: `FREEPROCCACHE`, `DROPCLEANBUFFERS`, `SHRINKFILE/DATABASE`, `TRACEON/OFF`,
     `CHECKIDENT` (unless `NORESEED`), any `REPAIR_*`, and others.
   - The rest stay yellow.
   - Tests: `DbccTests`.

Samples: `01-try-these.sql` examples #30–#36.

### Out of reach without the server

- Triggers fired by a write.
- What a procedure does.
- Which table a view or synonym points to.
- Whether a scalar function is CLR, and so could have side effects.
- How many rows are affected.

All of these need the server's metadata. That's the optional server-aware mode from the improvement list.

## Appendix: all 334 statement kinds

"deep" = read in detail; "green list" / "yellow" = classified by type; "red (default)" = treated as a real
change because it isn't a known read-only or opaque statement.

| Statement | Handling | How | Note |
|---|---|---|---|
| `AddSensitivityClassification` | red (default) | default: real write |  |
| `AddSignature` | red (default) | default: real write |  |
| `AlterApplicationRole` | red (default) | default: real write |  |
| `AlterAssembly` | red (default) | default: real write |  |
| `AlterAsymmetricKey` | red (default) | default: real write |  |
| `AlterAuthorization` | red (default) | default: real write |  |
| `AlterAvailabilityGroup` | red (default) | default: real write |  |
| `AlterBrokerPriority` | red (default) | default: real write |  |
| `AlterCertificate` | red (default) | default: real write |  |
| `AlterColumnEncryptionKey` | red (default) | default: real write |  |
| `AlterCredential` | red (default) | default: real write |  |
| `AlterCryptographicProvider` | red (default) | default: real write |  |
| `AlterDatabaseAddFileGroup` | red (default) | default: real write |  |
| `AlterDatabaseAddFile` | red (default) | default: real write |  |
| `AlterDatabaseAuditSpecification` | red (default) | default: real write |  |
| `AlterDatabaseCollate` | red (default) | default: real write |  |
| `AlterDatabaseEncryptionKey` | red (default) | default: real write |  |
| `AlterDatabaseModifyFileGroup` | red (default) | default: real write |  |
| `AlterDatabaseModifyFile` | red (default) | default: real write |  |
| `AlterDatabaseModifyName` | red (default) | default: real write |  |
| `AlterDatabasePerformCutover` | red (default) | default: real write |  |
| `AlterDatabaseRebuildLog` | red (default) | default: real write |  |
| `AlterDatabaseRemoveFileGroup` | red (default) | default: real write |  |
| `AlterDatabaseRemoveFile` | red (default) | default: real write |  |
| `AlterDatabaseScopedConfigurationClear` | red (default) | default: real write |  |
| `AlterDatabaseScopedConfigurationSet` | red (default) | default: real write |  |
| `AlterDatabaseSet` | red (default) | default: real write |  |
| `AlterEndpoint` | red (default) | default: real write |  |
| `AlterEventSession` | red (default) | default: real write |  |
| `AlterExternalDataSource` | red (default) | default: real write |  |
| `AlterExternalFunction` | red (default) | default: real write |  |
| `AlterExternalLanguage` | red (default) | default: real write |  |
| `AlterExternalLibrary` | red (default) | default: real write |  |
| `AlterExternalModel` | red (default) | default: real write |  |
| `AlterExternalResourcePool` | red (default) | default: real write |  |
| `AlterFederation` | red (default) | default: real write |  |
| `AlterFullTextCatalog` | red (default) | default: real write |  |
| `AlterFullTextIndex` | red (default) | default: real write |  |
| `AlterFullTextStopList` | red (default) | default: real write |  |
| `AlterFunction` | red (default) | definition change (body not walked) |  |
| `AlterIndex` | deep | temp or real (table) |  |
| `AlterLoginAddDropCredential` | red (default) | default: real write |  |
| `AlterLoginEnableDisable` | red (default) | default: real write |  |
| `AlterLoginOptions` | red (default) | default: real write |  |
| `AlterMasterKey` | red (default) | default: real write |  |
| `AlterMessageType` | red (default) | default: real write |  |
| `AlterPartitionFunction` | red (default) | default: real write |  |
| `AlterPartitionScheme` | red (default) | default: real write |  |
| `AlterProcedure` | red (default) | definition change (body not walked) |  |
| `AlterQueue` | red (default) | default: real write |  |
| `AlterRemoteServiceBinding` | red (default) | default: real write |  |
| `AlterResourceGovernor` | red (default) | default: real write |  |
| `AlterResourcePool` | red (default) | default: real write |  |
| `AlterRole` | red (default) | default: real write |  |
| `AlterRoute` | red (default) | default: real write |  |
| `AlterSchema` | red (default) | default: real write |  |
| `AlterSearchPropertyList` | red (default) | default: real write |  |
| `AlterSecurityPolicy` | red (default) | default: real write |  |
| `AlterSequence` | red (default) | default: real write |  |
| `AlterServerAuditSpecification` | red (default) | default: real write |  |
| `AlterServerAudit` | red (default) | default: real write |  |
| `AlterServerConfigurationSetBufferPoolExtension` | red (default) | default: real write |  |
| `AlterServerConfigurationSetDiagnosticsLog` | red (default) | default: real write |  |
| `AlterServerConfigurationSetExternalAuthentication` | red (default) | default: real write |  |
| `AlterServerConfigurationSetFailoverClusterProperty` | red (default) | default: real write |  |
| `AlterServerConfigurationSetHadrCluster` | red (default) | default: real write |  |
| `AlterServerConfigurationSetSoftNuma` | red (default) | default: real write |  |
| `AlterServerConfiguration` | red (default) | default: real write |  |
| `AlterServerRole` | red (default) | default: real write |  |
| `AlterServiceMasterKey` | red (default) | default: real write |  |
| `AlterService` | red (default) | default: real write |  |
| `AlterSymmetricKey` | red (default) | default: real write |  |
| `AlterTableAddClusterBy` | deep | temp or real |  |
| `AlterTableAddTableElement` | deep | temp or real |  |
| `AlterTableAlterColumn` | deep | temp or real |  |
| `AlterTableAlterIndex` | deep | temp or real |  |
| `AlterTableAlterPartition` | deep | temp or real |  |
| `AlterTableChangeTrackingModification` | deep | temp or real |  |
| `AlterTableConstraintModification` | deep | temp or real |  |
| `AlterTableDropTableElement` | deep | temp or real |  |
| `AlterTableFileTableNamespace` | deep | temp or real |  |
| `AlterTableRebuild` | deep | temp or real |  |
| `AlterTableSet` | deep | temp or real |  |
| `AlterTableSwitch` | deep | temp or real |  |
| `AlterTableTriggerModification` | deep | temp or real |  |
| `AlterTrigger` | red (default) | definition change (body not walked) |  |
| `AlterUser` | red (default) | default: real write |  |
| `AlterView` | red (default) | definition change (body not walked) |  |
| `AlterWorkloadGroup` | red (default) | default: real write |  |
| `AlterXmlSchemaCollection` | red (default) | default: real write |  |
| `BackupCertificate` | red (default) | default: real write |  |
| `BackupDatabase` | yellow | opaque: yellow |  |
| `BackupMasterKey` | red (default) | default: real write |  |
| `BackupServiceMasterKey` | red (default) | default: real write |  |
| `BackupTransactionLog` | yellow | opaque: yellow |  |
| `BeginConversationTimer` | red (default) | default: real write |  |
| `BeginDialog` | red (default) | default: real write |  |
| `BeginEndAtomicBlock` | deep | walked: block contents |  |
| `BeginEndBlock` | deep | walked: block contents |  |
| `BeginTransaction` | deep | green; tracked: open at the end -> yellow OPEN TRANSACTION |  |
| `Break` | green list | read-only list |  |
| `BulkInsert` | deep | target classified (temp / allow-list / real) |  |
| `Checkpoint` | yellow | opaque: yellow |  |
| `CloseCursor` | green list | read-only list |  |
| `CloseMasterKey` | green list | read-only list |  |
| `CloseSymmetricKey` | green list | read-only list |  |
| `CommitTransaction` | deep | green; yellow when it commits a transaction started outside the text |  |
| `Continue` | green list | read-only list |  |
| `Copy` | red (default) | default: real write |  |
| `CreateAggregate` | red (default) | default: real write |  |
| `CreateApplicationRole` | red (default) | default: real write |  |
| `CreateAssembly` | red (default) | default: real write |  |
| `CreateAsymmetricKey` | red (default) | default: real write |  |
| `CreateAvailabilityGroup` | red (default) | default: real write |  |
| `CreateBrokerPriority` | red (default) | default: real write |  |
| `CreateCertificate` | red (default) | default: real write |  |
| `CreateColumnEncryptionKey` | red (default) | default: real write |  |
| `CreateColumnMasterKey` | red (default) | default: real write |  |
| `CreateColumnStoreIndex` | deep | temp or real (table) |  |
| `CreateContract` | red (default) | default: real write |  |
| `CreateCredential` | red (default) | default: real write |  |
| `CreateCryptographicProvider` | red (default) | default: real write |  |
| `CreateDatabaseAuditSpecification` | red (default) | default: real write |  |
| `CreateDatabaseEncryptionKey` | red (default) | default: real write |  |
| `CreateDatabase` | red (default) | default: real write |  |
| `CreateDefault` | red (default) | default: real write |  |
| `CreateEndpoint` | red (default) | default: real write |  |
| `CreateEventNotification` | red (default) | default: real write |  |
| `CreateEventSession` | red (default) | default: real write |  |
| `CreateExternalDataSource` | red (default) | default: real write |  |
| `CreateExternalFileFormat` | red (default) | default: real write |  |
| `CreateExternalFunction` | red (default) | default: real write |  |
| `CreateExternalLanguage` | red (default) | default: real write |  |
| `CreateExternalLibrary` | red (default) | default: real write |  |
| `CreateExternalModel` | red (default) | default: real write |  |
| `CreateExternalResourcePool` | red (default) | default: real write |  |
| `CreateExternalStreamingJob` | red (default) | default: real write |  |
| `CreateExternalStream` | red (default) | default: real write |  |
| `CreateExternalTable` | red (default) | default: real write |  |
| `CreateFederation` | red (default) | default: real write |  |
| `CreateFullTextCatalog` | red (default) | default: real write |  |
| `CreateFullTextIndex` | red (default) | default: real write |  |
| `CreateFullTextStopList` | red (default) | default: real write |  |
| `CreateFunction` | red (default) | definition change (body not walked) |  |
| `CreateIndex` | deep | temp or real (table) |  |
| `CreateJsonIndex` | red (default) | default: real write |  |
| `CreateLogin` | red (default) | default: real write |  |
| `CreateMasterKey` | red (default) | default: real write |  |
| `CreateMessageType` | red (default) | default: real write |  |
| `CreateOrAlterExternalFunction` | red (default) | default: real write |  |
| `CreateOrAlterFunction` | red (default) | definition change (body not walked) |  |
| `CreateOrAlterProcedure` | red (default) | definition change (body not walked) |  |
| `CreateOrAlterTrigger` | red (default) | definition change (body not walked) |  |
| `CreateOrAlterView` | red (default) | definition change (body not walked) |  |
| `CreatePartitionFunction` | red (default) | default: real write |  |
| `CreatePartitionScheme` | red (default) | default: real write |  |
| `CreateProcedure` | red (default) | definition change (body not walked) |  |
| `CreateQueue` | red (default) | default: real write |  |
| `CreateRemoteServiceBinding` | red (default) | default: real write |  |
| `CreateResourcePool` | red (default) | default: real write |  |
| `CreateRole` | red (default) | default: real write |  |
| `CreateRoute` | red (default) | default: real write |  |
| `CreateRule` | red (default) | default: real write |  |
| `CreateSchema` | red (default) | default: real write |  |
| `CreateSearchPropertyList` | red (default) | default: real write |  |
| `CreateSecurityPolicy` | red (default) | default: real write |  |
| `CreateSelectiveXmlIndex` | red (default) | default: real write |  |
| `CreateSemanticIndex` | red (default) | default: real write |  |
| `CreateSequence` | red (default) | default: real write |  |
| `CreateServerAuditSpecification` | red (default) | default: real write |  |
| `CreateServerAudit` | red (default) | default: real write |  |
| `CreateServerRole` | red (default) | default: real write |  |
| `CreateService` | red (default) | default: real write |  |
| `CreateSpatialIndex` | red (default) | default: real write |  |
| `CreateStatistics` | deep | temp or real (table) |  |
| `CreateSymmetricKey` | red (default) | default: real write |  |
| `CreateSynonym` | red (default) | default: real write |  |
| `CreateTable` | deep | temp or real |  |
| `CreateTrigger` | red (default) | definition change (body not walked) |  |
| `CreateTypeTable` | red (default) | default: real write |  |
| `CreateTypeUddt` | red (default) | default: real write |  |
| `CreateTypeUdt` | red (default) | default: real write |  |
| `CreateUser` | red (default) | default: real write |  |
| `CreateVectorIndex` | red (default) | default: real write |  |
| `CreateView` | red (default) | definition change (body not walked) |  |
| `CreateWorkloadClassifier` | red (default) | default: real write |  |
| `CreateWorkloadGroup` | red (default) | default: real write |  |
| `CreateXmlIndex` | red (default) | default: real write |  |
| `CreateXmlSchemaCollection` | red (default) | default: real write |  |
| `Dbcc` | deep | per command: read-only green, changing red (also REPAIR, CHECKIDENT reseed), others yellow |  |
| `DeallocateCursor` | green list | read-only list |  |
| `DeclareCursor` | green list | read-only list |  |
| `DeclareTableVariable` | green list | read-only list |  |
| `DeclareVariable` | green list | read-only list |  |
| `Delete` | deep | DML: target resolved (alias/CTE/derived) + classified; no-WHERE; side effects scanned |  |
| `Deny` | red (default) | default: real write |  |
| `DenyStatement80` | red (default) | default: real write |  |
| `Disk` | red (default) | default: real write |  |
| `DropAggregate` | red (default) | default: real write |  |
| `DropApplicationRole` | red (default) | default: real write |  |
| `DropAssembly` | red (default) | default: real write |  |
| `DropAsymmetricKey` | red (default) | default: real write |  |
| `DropAvailabilityGroup` | red (default) | default: real write |  |
| `DropBrokerPriority` | red (default) | default: real write |  |
| `DropCertificate` | red (default) | default: real write |  |
| `DropColumnEncryptionKey` | red (default) | default: real write |  |
| `DropColumnMasterKey` | red (default) | default: real write |  |
| `DropContract` | red (default) | default: real write |  |
| `DropCredential` | red (default) | default: real write |  |
| `DropCryptographicProvider` | red (default) | default: real write |  |
| `DropDatabaseAuditSpecification` | red (default) | default: real write |  |
| `DropDatabaseEncryptionKey` | red (default) | default: real write |  |
| `DropDatabase` | red (default) | default: real write |  |
| `DropDefault` | red (default) | default: real write |  |
| `DropEndpoint` | red (default) | default: real write |  |
| `DropEventNotification` | red (default) | default: real write |  |
| `DropEventSession` | red (default) | default: real write |  |
| `DropExternalDataSource` | red (default) | default: real write |  |
| `DropExternalFileFormat` | red (default) | default: real write |  |
| `DropExternalLanguage` | red (default) | default: real write |  |
| `DropExternalLibrary` | red (default) | default: real write |  |
| `DropExternalModel` | red (default) | default: real write |  |
| `DropExternalResourcePool` | red (default) | default: real write |  |
| `DropExternalStreamingJob` | red (default) | default: real write |  |
| `DropExternalStream` | red (default) | default: real write |  |
| `DropExternalTable` | red (default) | default: real write |  |
| `DropFederation` | red (default) | default: real write |  |
| `DropFullTextCatalog` | red (default) | default: real write |  |
| `DropFullTextIndex` | red (default) | default: real write |  |
| `DropFullTextStopList` | red (default) | default: real write |  |
| `DropFunction` | red (default) | default: real write |  |
| `DropIndex` | deep | temp or real (table) |  |
| `DropLogin` | red (default) | default: real write |  |
| `DropMasterKey` | red (default) | default: real write |  |
| `DropMessageType` | red (default) | default: real write |  |
| `DropPartitionFunction` | red (default) | default: real write |  |
| `DropPartitionScheme` | red (default) | default: real write |  |
| `DropProcedure` | red (default) | default: real write |  |
| `DropQueue` | red (default) | default: real write |  |
| `DropRemoteServiceBinding` | red (default) | default: real write |  |
| `DropResourcePool` | red (default) | default: real write |  |
| `DropRole` | red (default) | default: real write |  |
| `DropRoute` | red (default) | default: real write |  |
| `DropRule` | red (default) | default: real write |  |
| `DropSchema` | red (default) | default: real write |  |
| `DropSearchPropertyList` | red (default) | default: real write |  |
| `DropSecurityPolicy` | red (default) | default: real write |  |
| `DropSensitivityClassification` | red (default) | default: real write |  |
| `DropSequence` | red (default) | default: real write |  |
| `DropServerAuditSpecification` | red (default) | default: real write |  |
| `DropServerAudit` | red (default) | default: real write |  |
| `DropServerRole` | red (default) | default: real write |  |
| `DropService` | red (default) | default: real write |  |
| `DropSignature` | red (default) | default: real write |  |
| `DropStatistics` | deep | temp or real (table) |  |
| `DropSymmetricKey` | red (default) | default: real write |  |
| `DropSynonym` | red (default) | default: real write |  |
| `DropTable` | deep | temp or real, per table |  |
| `DropTrigger` | red (default) | default: real write |  |
| `DropType` | red (default) | default: real write |  |
| `DropUser` | red (default) | default: real write |  |
| `DropView` | red (default) | default: real write |  |
| `DropWorkloadClassifier` | red (default) | default: real write |  |
| `DropWorkloadGroup` | red (default) | default: real write |  |
| `DropXmlSchemaCollection` | red (default) | default: real write |  |
| `EnableDisableTrigger` | red (default) | default: real write |  |
| `EndConversation` | red (default) | default: real write |  |
| `EventSession` | red (default) | default: real write | parser-internal (never in a script) |
| `ExecuteAs` | green list | read-only list |  |
| `Execute` | deep | EXEC: read-only and known writing sys procs, literal dynamic SQL parsed, else Unknown |  |
| `ExternalResourcePool` | red (default) | default: real write | parser-internal (never in a script) |
| `FetchCursor` | green list | read-only list |  |
| `GetConversationGroup` | green list | read-only list |  |
| `GoTo` | green list | read-only list |  |
| `Grant` | red (default) | default: real write |  |
| `GrantStatement80` | red (default) | default: real write |  |
| `If` | deep | walked: condition scanned, both branches walked |  |
| `IndexDefinition` | red (default) | default: real write | parser-internal (never in a script) |
| `InsertBulk` | deep | target classified (temp / allow-list / real) |  |
| `Insert` | deep | DML: target resolved + classified; OUTPUT INTO; INSERT...EXEC; side effects scanned |  |
| `KillQueryNotificationSubscription` | red (default) | default: real write |  |
| `Kill` | red (default) | default: real write |  |
| `KillStatsJob` | red (default) | default: real write |  |
| `Label` | green list | read-only list |  |
| `LineNo` | green list | read-only list |  |
| `Merge` | deep | DML: target resolved + classified; OUTPUT INTO; side effects scanned |  |
| `MoveConversation` | red (default) | default: real write |  |
| `OpenCursor` | green list | read-only list |  |
| `OpenMasterKey` | green list | read-only list |  |
| `OpenSymmetricKey` | green list | read-only list |  |
| `PredicateSet` | green list | read-only list |  |
| `Print` | green list | read-only list |  |
| `RaiseErrorLegacy` | green list | read-only list |  |
| `RaiseError` | green list | read-only list |  |
| `ReadText` | green list | read-only list |  |
| `Receive` | red (default) | default: real write |  |
| `Reconfigure` | red (default) | default: real write |  |
| `RenameEntity` | red (default) | default: real write |  |
| `ResourcePool` | red (default) | default: real write | parser-internal (never in a script) |
| `RestoreMasterKey` | red (default) | default: real write |  |
| `RestoreServiceMasterKey` | red (default) | default: real write |  |
| `Restore` | red (default) | default: real write |  |
| `Return` | green list | read-only list |  |
| `Revert` | green list | read-only list |  |
| `Revoke` | red (default) | default: real write |  |
| `RevokeStatement80` | red (default) | default: real write |  |
| `RollbackTransaction` | deep | green; tracked (savepoints kept open) |  |
| `SaveTransaction` | deep | green; savepoint tracked |  |
| `Select` | deep | SELECT: read-only, or SELECT INTO target classified; hidden side effects scanned |  |
| `SelectStatementSnippet` | deep | SELECT: read-only, or SELECT INTO target classified; hidden side effects scanned | parser-internal (never in a script) |
| `Send` | red (default) | default: real write |  |
| `SetCommand` | green list | read-only list |  |
| `SetErrorLevel` | green list | read-only list |  |
| `SetIdentityInsert` | green list | read-only list |  |
| `SetOffsets` | green list | read-only list |  |
| `SetRowCount` | green list | read-only list |  |
| `SetStatistics` | green list | read-only list |  |
| `SetTextSize` | green list | read-only list |  |
| `SetTransactionIsolationLevel` | green list | read-only list |  |
| `SetUser` | green list | read-only list |  |
| `SetVariable` | green list | read-only list |  |
| `Shutdown` | red (default) | default: real write |  |
| `Throw` | green list | read-only list |  |
| `TruncateTable` | deep | temp / allow-list / real; "removes every row" |  |
| `TryCatch` | deep | walked: TRY and CATCH |  |
| `TSqlStatementSnippet` | red (default) | default: real write | parser-internal (never in a script) |
| `Update` | deep | DML: target resolved (alias/CTE/derived) + classified; no-WHERE; side effects scanned |  |
| `UpdateStatistics` | yellow | opaque: yellow |  |
| `UpdateText` | red (default) | default: real write |  |
| `UseFederation` | green list | read-only list |  |
| `Use` | green list | read-only list |  |
| `WaitFor` | deep | walked (inner RECEIVE etc.) or read-only |  |
| `While` | deep | walked: condition scanned, body walked |  |
| `WriteText` | red (default) | default: real write |  |

