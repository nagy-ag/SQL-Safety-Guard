> **Snapshot — not maintained.** Current state: [verification/status.md](../verification/status.md). All current docs: [docs/README.md](../README.md).

# T-SQL statement coverage

This is an implementation inventory, not a proof that SQL Safety Guard can predict every SQL Server side effect.
The analyzer uses ScriptDom 180.x (`TSql180Parser`) and does not connect to SQL Server for metadata or execution.

## Scope of verification

An earlier reflection inventory found **334 concrete statement kinds** in the pinned ScriptDom library. The
appendix preserves that inventory with updated handling notes. Recognizing a statement class does not establish
correct target resolution, argument semantics, reachability or runtime behavior. Unrecognized statement types
are conservatively red, while opaque execution and analysis limits are yellow.

The soundness/rules changes are checked by automated analyzer, policy, resource-budget and installer regressions.
The October 27, 2026 combined verification accounts for **all 334 kinds statically**: 318 default-parser SQL
cases, nine alternate-dialect cases, and seven constructed-AST-only helper/snippet nodes. **267 kinds have
passing representative live execution**, three have failed attempts only, and 64 have no live execution evidence.
The six authoritative runtime reports contain 293 fixture executions: 289 passes and four failed attempts
(including one subsequently corrected Resource Governor setup). The final unit suite passes **793/793** tests.
See the [combined execution evidence and limitations](2026-10-27-all-kinds/README.md),
[per-kind matrix](2026-10-27-all-kinds/statement-matrix.md), and
[reproducible runners](../../tests/SqlSafetyGuard.ServerHarness/Coverage/README.md). The earlier
[118-fixture / 103-kind report](2026-10-27-strict-coverage.md) is historical, not the current total.
These results do not prove every clause combination or every possible side effect. SSMS UI acceptance is
documented separately in [release acceptance](../release-acceptance.md); headless policy tests do not replace it.

## By category

| Category | Examples | Result |
|---|---|---|
| Reads | `SELECT`, `READTEXT`, `OPENJSON`, `OPENROWSET(BULK …)` | Green unless a recognized expression/source side effect is present. |
| Data changes | `INSERT/UPDATE/DELETE/MERGE`, `SELECT INTO`, `BULK INSERT`, `OUTPUT INTO` | Targets resolved through aliases/CTEs/derived tables, then temp/allow-listed/real. Composable DML sources and their OUTPUT targets are independently classified. |
| Table & index DDL | `CREATE/ALTER/DROP TABLE`, table-targeted index/statistics DDL | Permanent targets red; recognized temp targets green. DDL does not use the table allow-list. |
| Modules | `CREATE/ALTER/CREATE OR ALTER/DROP PROCEDURE` | Temporary procedures (`#p`/`##p`) green; permanent procedures red. Definition bodies do not execute and are not walked. Function/trigger/view definitions remain red. |
| Other object/security DDL | sequences, databases, external objects, `GRANT/REVOKE/DENY`, logins/users/roles | Red by default. Session-only key opening/closing and `EXECUTE AS/REVERT` are green. |
| Transactions | `BEGIN/COMMIT/ROLLBACK/SAVE TRAN`, `SET IMPLICIT_TRANSACTIONS` | Parsed, conservative state tracking. A possibly open transaction or commit of outside work is yellow; balanced visible transactions are green. |
| Session settings | `SET NOCOUNT`, isolation level, `LINENO` | Green. `USE`/`USE FEDERATION` are yellow target switches, not trusted as unchanged context. |
| Control flow | `IF/ELSE`, `WHILE`, `BEGIN…END`, `TRY…CATCH`, `WAITFOR`, `RETURN/GOTO/THROW` | Contained writes are analyzed; transaction closure must not depend on unreachable/uncertain paths. ELSE-IF chains are iterative. |
| Cursors & variables | `DECLARE/OPEN/FETCH/CLOSE/DEALLOCATE`, table variables | Session-only operations green; hidden expression effects scanned; cursor-target writes classified separately. |
| Procedures/dynamic SQL | `EXEC proc`, `sp_help`, `sp_rename`, `sp_executesql`, `EXEC(…) AT srv` | Unknown ordinary procedures yellow; namespace-gated built-ins and explicit trusted entries green/red as appropriate. Literal dynamic SQL analyzed to three levels; runtime/remote SQL yellow. |
| Service Broker | `SEND/RECEIVE`, `BEGIN DIALOG`, `END CONVERSATION` | Red; `GET CONVERSATION GROUP` remains green. |
| Server/maintenance | `DBCC`, `BACKUP`, `RESTORE`, `CHECKPOINT`, `KILL` | DBCC arguments parsed (below). BACKUP/CHECKPOINT/UPDATE STATISTICS yellow; RESTORE/KILL/SHUTDOWN/RECONFIGURE red. |
| Inside expressions | `NEXT VALUE FOR`, remote pass-through sources, scalar functions, four-part reads | Sequences red, pass-through yellow, ordinary remote-table reads green. Scalar/CLR function effects are not resolved. |
| SQLCMD | `:setvar`, `$(var)`, `:exit(query)`, `:connect`, `:r`, `!!` | Mode-on/unknown preprocessing preserves offsets. Every substitution is yellow minimum; directives that execute/include unknown text are yellow. Mode-off text is parsed as sent. |

## Semantic safeguards

### Targets and trusted names

- Aliases/CTEs resolve **before** spelling-based temp checks: `UPDATE [#t] … FROM dbo.Orders AS [#t]` is red even
  if a real temp table of that name exists. CTE resolution is memoized, deduplicated, cycle-checked and bounded.
- `INSERT INTO #audit … FROM (DELETE dbo.Customers OUTPUT deleted.Id) AS c` is red for the inner DELETE.
  Nested OUTPUT INTO targets are classified too. A temp outer target never suppresses an inner real write.
- Permanent `tempdb.dbo.X` targets are red by default. `TempdbIsTemp` is an explicit opt-in, exposed in Options;
  actual `#temp`/`##temp` names and table variables remain temp-only.
- Allow-list matches use strict whole-part anchors; omitted server parts never match linked-server targets.
  Omitted database parts match unqualified or known-current-database targets only. Schema-less query names
  match one-part patterns only unless a default schema is supplied. The VSIX does not discover that schema.
  Wildcard-only/invalid patterns are ignored with SETTINGS WARNING findings.
- Trusted procedures require schema-qualified patterns, with wildcards only in the last part. They assert
  read-only behavior without inspecting bodies. Global and per-environment trust lists are additive.
- Built-in recognition only accepts unqualified, sys/dbo, master.sys/master.dbo (and explicitly recognized msdb)
  namespaces. `OtherDb.custom.sp_executesql` is an ordinary unknown procedure. `sp_spaceused` with true or unknown
  `@updateusage` (also positional argument two) is red.
- `USE`/`USE FEDERATION`/`:connect` produce TARGET SWITCH warnings and disable contextual allow/trusted matches
  throughout the analysis. Environment mode is selected from the initial connection, not reselected per statement.

Tests: `SoundnessRegressionTests`, `AllowListTests`, `TempTableTests`, `EnvironmentRuleTests`,
`SystemProcedureTests`, `AdditionalAnalyzerTests`, `RecoveryTests` and existing DML/sample suites.

### DBCC arguments, not display substrings

- Read-only commands include `SHOW_STATISTICS`, `INPUTBUFFER`, `OPENTRAN`, `USEROPTIONS`, `TRACESTATUS` and
  `CHECKDB/CHECKTABLE/…` without repair arguments. An object called `dbo.REPAIR_Archive` is not a repair option.
- `CHECKIDENT` is red unless an actual parsed NORESEED argument establishes the read-only form. NORESEED in a
  table name or comment cannot override RESEED.
- Repair arguments and `SQLPERF(…, CLEAR)` are red, including quoted CLEAR literals. Long comments/display
  truncation do not hide arguments. Other recognized cache/shrink/trace-changing commands are red; remaining
  commands stay yellow.

Tests: `DbccTests`, `SoundnessRegressionTests.DbccUsesParsedArgumentsBeyondDisplayTruncation` and data-driven
`RealWritesAreNeverGreen` / `OrdinaryReadsRemainReadOnly` cases.

### Conservative transactions and SQLCMD

- A COMMIT with no visible BEGIN may commit outside work and is yellow. A transaction possibly open at the end
  gets OPEN TRANSACTION. Literal dynamic SQL shares tracking; broken batches do not execute for tracking purposes.
- Parsed `@@TRANCOUNT`/`XACT_STATE()` expressions, not substring occurrences in strings/comments, identify guards.
  Unreachable branches, loops that may run zero times and statements after RETURN cannot close a transaction.
  GO repeat counts and uncertain control transfer add uncertainty rather than proving closure.
- With implicit transactions on, an explicit BEGIN can produce two levels; one COMMIT leaves a warning. A
  tableless `SELECT 1` does not start an implicit transaction. This is not a session-aware/control-flow simulator.
- SQLCMD variables are **not expanded for semantic trust**. Every `$(…)` is at least yellow, including strings,
  concatenated identifiers, unknown-at-that-point values and statement-injecting values. Ordered `:setvar` values
  provide display hints only. `:exit(query)` is yellow because it can execute text; included files are not opened.

Tests: `TransactionTests`, `SoundnessRegressionTests.PossiblyOpenTransactionsAreReported`,
`TransactionUncertaintyIsNotGreen`, `UnexpandedSqlCmdExecutionIsNeverGreen` and
`SqlCmdDefinitionsDoNotApplyRetroactivelyAndOffsetsStayOriginal`; `RecoveryTests` covers SQLCMD modes.

## Bounded analysis and recovery

- Parsing/traversal use a dedicated background thread with a **64 MiB reserved stack**. Cancellation propagates
  into text/token reads and traversal. A shared 40-million-work-unit / 10-second budget bounds total analysis.
- Preflight rejects more than 256 nested parentheses or BEGIN/CASE blocks, 512 IF tokens per GO batch, or
  20,000 significant tokens per statement. CTE target resolution also has a depth cap of 256. Exceeding a budget
  yields yellow NOT ANALYZED (too complex), not a safe result or a claim about SQL Server validity.
- The size limit is 5,242,880 characters (roughly 10 MiB UTF-16). Oversize returns yellow NOT ANALYZED with a size,
  prompting under default YellowRed policy. Red-only/Off are explicit user overrides.
- Smaller broken batches use at most 25 recovery passes / 3 seconds, preserving editor positions. Broken batches
  over 32 Ki characters instead use NOT ANALYZED and tokenized possible-write findings. This scales roughly
  linearly but does **not** recover detailed target classification for every statement. Recovery is heuristic.
- Resource regressions cover chained ELSE-IF, deeply nested valid/invalid text, doubling CTE graphs, cancellation,
  realistic 1,000/5,000-line scripts, a 20,000-character line and 3,000 broken statements. Timing assertions are
  generous regression bounds, not SSMS UI latency guarantees.

Tests: `AnalysisResourceTests`, `EnvironmentRuleTests.OversizedScriptsFollowExplicitModePolicy` and `RecoveryTests`.

## Execution policy and remaining limits

Environment rules are first-match, case-insensitive server/database patterns; unknown connection context uses
**global** settings. Per-rule allow/trust entries add to global lists. Typed database confirmation applies to
red UPDATE/DELETE without WHERE unless a `TOP (n)` or an inner join on the target limits the rows (CTE, derived
table, `TOP ... PERCENT` and outer/cross-join-only variants still count as whole-target), TRUNCATE and any red
DROP action. It does not validate which database a target switch will reach. See README for the rule format.

Presentation failures cannot authorize an already computed red/yellow warning, and a failed dialog cancels.
Failures before classification remain fail-open-but-visible (degraded health, log and attempted once/session
InfoBar). Off and red-only policy intentionally skip some warnings. These are usability safeguards, **not a
security boundary**.

The analyzer cannot see triggers, arbitrary procedure/CLR function side effects, view/synonym base metadata,
actual transaction session state or affected-row counts. User trust settings and resource/recovery limits add
other uncertainty. New SQL grammar and SSMS internals require continuing review and runtime validation; server
metadata alone would not close every gap.

## Appendix: reviewed ScriptDom inventory (334 kinds)

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
| `AlterProcedure` | deep | temp procedure or real definition change (body not walked) |  |
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
| `CreateOrAlterProcedure` | deep | temp procedure or real definition change (body not walked) |  |
| `CreateOrAlterTrigger` | red (default) | definition change (body not walked) |  |
| `CreateOrAlterView` | red (default) | definition change (body not walked) |  |
| `CreatePartitionFunction` | red (default) | default: real write |  |
| `CreatePartitionScheme` | red (default) | default: real write |  |
| `CreateProcedure` | deep | temp procedure or real definition change (body not walked) |  |
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
| `Dbcc` | deep | parsed command/arguments: read-only green, repairs/reseed/SQLPERF CLEAR red, others yellow |  |
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
| `DropProcedure` | deep | temp or real, per procedure |  |
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
| `UseFederation` | yellow | TARGET SWITCH; invalidates contextual trust |  |
| `Use` | yellow | TARGET SWITCH; invalidates contextual trust |  |
| `WaitFor` | deep | walked (inner RECEIVE etc.) or read-only |  |
| `While` | deep | walked: condition scanned, body walked |  |
| `WriteText` | red (default) | default: real write |  |

