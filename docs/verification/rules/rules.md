# Analyzer rule inventory

Generated from rules.json; regenerate with `python tools/verification/generate_inventory.py`.

Every entry is a reviewed semantic rule or dispatch-table member. Retired line-level checkpoints are recorded in `rule-id-migrations.json`. Named tests are associations, not server or branch coverage.

| Category | Entries |
| --- | ---: |
| action-name | 31 |
| aggregation | 3 |
| allow-list | 9 |
| changing-dbcc | 33 |
| classification | 8 |
| entry | 4 |
| environment | 10 |
| green-statement | 43 |
| hidden-effects | 7 |
| limits | 7 |
| msdb-procedure | 19 |
| options | 1 |
| parser | 1 |
| positions | 1 |
| presentation | 1 |
| recovery | 1 |
| recovery-keyword | 18 |
| resolution | 10 |
| safe-dbcc | 28 |
| safe-procedure | 49 |
| scope | 6 |
| set-family | 20 |
| set-option | 25 |
| sqlcmd | 3 |
| sqlcmd-command | 18 |
| unknown-dbcc | 48 |
| unknown-statement | 3 |
| walking | 48 |
| writing-procedure | 72 |

## action-name.altertableaddclusterbystatement

ActionNames contains AlterTableAddClusterByStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:256`.
- Existing tests: No reviewed association.

## action-name.altertableaddtableelementstatement

ActionNames contains AlterTableAddTableElementStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:244`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablealtercolumnstatement

ActionNames contains AlterTableAlterColumnStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:245`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablealterindexstatement

ActionNames contains AlterTableAlterIndexStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:254`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablealterpartitionstatement

ActionNames contains AlterTableAlterPartitionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:255`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablechangetrackingmodificationstatement

ActionNames contains AlterTableChangeTrackingModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:252`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertableconstraintmodificationstatement

ActionNames contains AlterTableConstraintModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:247`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertabledroptableelementstatement

ActionNames contains AlterTableDropTableElementStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:246`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablefiletablenamespacestatement

ActionNames contains AlterTableFileTableNamespaceStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:253`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablerebuildstatement

ActionNames contains AlterTableRebuildStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:250`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertablesetstatement

ActionNames contains AlterTableSetStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:248`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertableswitchstatement

ActionNames contains AlterTableSwitchStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:249`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.altertabletriggermodificationstatement

ActionNames contains AlterTableTriggerModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:251`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.backupdatabasestatement

ActionNames contains BackupDatabaseStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:240`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.backuptransactionlogstatement

ActionNames contains BackupTransactionLogStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:241`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.begintransactionstatement

ActionNames contains BeginTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:232`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.bulkinsertstatement

ActionNames contains BulkInsertStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:257`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.committransactionstatement

ActionNames contains CommitTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:233`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.dbccstatement

ActionNames contains DbccStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:239`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.declarecursorstatement

ActionNames contains DeclareCursorStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:231`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.declaretablevariablestatement

ActionNames contains DeclareTableVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:230`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.declarevariablestatement

ActionNames contains DeclareVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:229`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.executeasstatement

ActionNames contains ExecuteAsStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:242`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.gotostatement

ActionNames contains GoToStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:238`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.insertbulkstatement

ActionNames contains InsertBulkStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:258`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.raiseerrorlegacystatement

ActionNames contains RaiseErrorLegacyStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:237`.
- Existing tests: No reviewed association.

## action-name.raiseerrorstatement

ActionNames contains RaiseErrorStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:236`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.rollbacktransactionstatement

ActionNames contains RollbackTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:234`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.savetransactionstatement

ActionNames contains SaveTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:235`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.setvariablestatement

ActionNames contains SetVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:228`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## action-name.updatestatisticsstatement

ActionNames contains UpdateStatisticsStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:243`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## aggregate.max

Overall is the maximum finding severity, with parse errors flooring it at Unknown. Blocked findings remain included.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:25`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.Warnings_AndSummary @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:263

## aggregate.presentation

Warnings exclude green findings and sort by severity then offset; summary counts runnable writes, nondiagnostic unknowns, syntax errors and blocked warnings separately without changing Overall.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:50`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.Warnings_AndSummary @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:263

## verdict.severity

Severity is ReadOnly < TempOnly < AllowListed < Unknown < RealWrite; only the first three are green. Display names do not change severity.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:6`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.Warnings_AndSummary @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:263

## allow.cross-database

A cross-database target needs at least a three-part pattern unless its database equals current context.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:48`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.AllowListHonorsQualificationAndExactIdentifierEnd @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200

## allow.default-context

Resolve absent schema/database using supplied default schema/current database, never invent dbo.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:44`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.ProcedureTrustRequiresResolvedQualification @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84

## allow.empty-name

A missing target name never matches.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:43`.
- Existing tests: SqlSafetyGuard.Tests.AllowListTests.Matches @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:26

## allow.glob

Glob * and ? match identifier parts case-insensitively with exact start/end and 100ms regex timeout.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:58`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.AllowListHonorsQualificationAndExactIdentifierEnd @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200

## allow.ignore-comments

Ignore empty, --comment and #comment pattern lines.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.AllowListTests.IgnoresBlankAndCommentLines @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:33

## allow.invalid

Reject unterminated quoting, empty parts, more than four parts, wildcard-only names and unsafe trusted-procedure qualifiers; report warnings.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.WildcardOnlyAllowListIsIgnoredVisibly @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:214; SqlSafetyGuard.Tests.EnvironmentRuleTests.ProcedureTrustRequiresResolvedQualification @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84

## allow.match

Quote-aware name parsing and anchored case-insensitive glob matching restrict explicit cross-database/server targets; invalid patterns warn.

- Outcomes: AllowListed, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:35`.
- Existing tests: SqlSafetyGuard.Tests.AllowListTests.Matches @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:26; SqlSafetyGuard.Tests.AllowListTests.IgnoresBlankAndCommentLines @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:33; SqlSafetyGuard.Tests.AllowListTests.EmptyAllowList_MatchesNothing @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:40

## allow.quoted-parts

Quoted dots stay within identifiers, doubled delimiters unescape, trailing junk or unclosed quote rejects pattern.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:65`.
- Existing tests: SqlSafetyGuard.Tests.AllowListTests.Matches @ tests/SqlSafetyGuard.Tests/AllowListTests.cs:26

## allow.remote

A remote target needs an explicit four-part pattern; a shorter pattern cannot confer trust.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:47`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.AllowListHonorsQualificationAndExactIdentifierEnd @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200

## changing-dbcc.addextendedproc

ChangingDbcc contains AddExtendedProc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:162`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.cleantable

ChangingDbcc contains CleanTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:146`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.clearspacecaches

ChangingDbcc contains ClearSpaceCaches; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:169`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dbrecover

ChangingDbcc contains DBRecover; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:168`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dbreindex

ChangingDbcc contains DBReindex; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:147`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dbreindexall

ChangingDbcc contains DBReindexAll; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:148`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dbrepair

ChangingDbcc contains DBRepair; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:154`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.detachdb

ChangingDbcc contains DetachDB; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:155`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dropcleanbuffers

ChangingDbcc contains DropCleanBuffers; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:143`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.dropextendedproc

ChangingDbcc contains DropExtendedProc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:163`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.errorlog

ChangingDbcc contains ErrorLog; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:156`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.fixallocation

ChangingDbcc contains FixAllocation; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:167`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.flushprocindb

ChangingDbcc contains FlushProcInDB; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:157`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.forceghostcleanup

ChangingDbcc contains ForceGhostCleanup; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:158`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.free

ChangingDbcc contains Free; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:170`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.freeproccache

ChangingDbcc contains FreeProcCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:140`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.freesessioncache

ChangingDbcc contains FreeSessionCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:141`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.freesystemcache

ChangingDbcc contains FreeSystemCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:142`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.freezeio

ChangingDbcc contains FreezeIO; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:164`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.indexdefrag

ChangingDbcc contains IndexDefrag; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:149`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.movepage

ChangingDbcc contains MovePage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:166`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.pintable

ChangingDbcc contains PinTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:159`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.renamecolumn

ChangingDbcc contains RenameColumn; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:161`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.setcpuweight

ChangingDbcc contains SetCpuWeight; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:171`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.setioweight

ChangingDbcc contains SetIOWeight; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:172`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.shrinkdatabase

ChangingDbcc contains ShrinkDatabase; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:144`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.shrinkfile

ChangingDbcc contains ShrinkFile; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:145`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.thawio

ChangingDbcc contains ThawIO; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:165`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.traceoff

ChangingDbcc contains TraceOff; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:152`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.traceon

ChangingDbcc contains TraceOn; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:151`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.unpintable

ChangingDbcc contains UnpinTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:160`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.updateusage

ChangingDbcc contains UpdateUsage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:150`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## changing-dbcc.writepage

ChangingDbcc contains WritePage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:153`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## dbcc.checkident

CHECKIDENT is green only for second IdentifierLiteral NORESEED; otherwise may reseed.

- Outcomes: ReadOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:192`.
- Existing tests: SqlSafetyGuard.Tests.DbccTests.ReadOnlyDbcc_IsGreen @ tests/SqlSafetyGuard.Tests/DbccTests.cs:17; SqlSafetyGuard.Tests.DbccTests.ChangingDbcc_IsRed @ tests/SqlSafetyGuard.Tests/DbccTests.cs:32

## dbcc.clear

SQLPERF CLEAR overrides its otherwise safe table entry.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:200`.
- Existing tests: SqlSafetyGuard.Tests.DbccTests.ChangingDbcc_IsRed @ tests/SqlSafetyGuard.Tests/DbccTests.cs:32

## dbcc.dll

DBCC DLL form unloads an extended procedure DLL.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:180`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.UnknownDbccNameIsClassifiedAsDllUnload @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:55

## dbcc.fallback

An unlisted DBCC command is Unknown.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.DbccTests.OtherDbcc_StaysYellow @ tests/SqlSafetyGuard.Tests/DbccTests.cs:40

## dbcc.repair

Any literal REPAIR_ argument after first parameter overrides safe DBCC classification.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:186`.
- Existing tests: SqlSafetyGuard.Tests.DbccTests.ChangingDbcc_IsRed @ tests/SqlSafetyGuard.Tests/DbccTests.cs:32

## temp.name

A # prefix includes both local and global temporary names; no server qualifier is permitted by IsTemp.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:261`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.TableDdlAndTruncate @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180

## temp.tempdb-opt-in

TempdbIsTemp opt-in treats a tempdb-qualified permanent target as temporary without a remote server.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:263`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.PermanentTempdbObjectsRequireExplicitOptIn @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:194

## write.allow-list

Write precedence: temporary target, then eligible allow-list match, otherwise permanent write. DDL disables allow-list.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:274`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.AllowList_DoesNotApplyToDdl @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:251; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.AllowList_AppliesToTruncateAndSelectInto @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:257

## entry.empty

Empty or whitespace SQL produces no actions when settings are valid.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:52`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.EmptyText_IsReadOnlyWithNoFindings @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:18

## entry.length

Text over 5 Mi UTF-16 characters is not analyzed; return a warning rather than green.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.OversizedScriptsFollowExplicitModePolicy @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:204

## entry.limit

Work-limit failure adds Unknown without discarding earlier findings.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:81`.
- Existing tests: No reviewed association.

## entry.settings

Invalid allow list, procedure trust or configuration warnings contribute Unknown even on empty SQL.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:47`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.BadRulesAreReportedAndIgnored @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32

## environment.confirm-failure

Confirmation exception cancels execution; negative confirmation also cancels.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardDecision.cs:28`.
- Existing tests: SqlSafetyGuard.Tests.GuardExecutionContractTests.ConfirmationFailureAlwaysPreventsExecution @ tests/SqlSafetyGuard.Tests/GuardExecutionContractTests.cs:82

## environment.invalid-line

Ignore malformed environment lines with visible settings warnings; do not partially apply them.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:29`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.BadRulesAreReportedAndIgnored @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32

## environment.invalid-list

An invalid allow/trusted list rejects the whole environment rule.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:38`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.BadRulesAreReportedAndIgnored @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32

## environment.off

Off mode never requests warning confirmation; this does not change analyzer Overall.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardSettings.cs:28`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.ModesControlWarnings @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113

## environment.presentation-failure

Presentation exception is reported but cannot suppress an already-required confirmation.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardDecision.cs:25`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.PresentationOrLoggingFailuresCannotAuthorizeWarning @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:139

## environment.red

Red mode asks only for RealWrite; Unknown can execute without confirmation in this explicit policy.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardSettings.cs:29`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.ModesControlWarnings @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113

## environment.selection

First matching environment wins; missing context uses global settings; per-rule lists add without leaking.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:54`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.FirstMatchingRuleWinsAndUnknownContextUsesGlobal @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:16; SqlSafetyGuard.Tests.EnvironmentRuleTests.PerRuleListsAddToGlobalWithoutLeakingToOtherEnvironments @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:65

## environment.typed-disabled

Typed database-name confirmation is always false, including legacy true configuration.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardSettings.cs:31`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.LegacyTypedSettingNeverRequiresTyping @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:123; SqlSafetyGuard.Tests.EnvironmentRuleTests.LegacyRuleFieldIsAcceptedAndIgnored @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:129

## environment.unknown-context

Missing server or database bypasses environment matching and returns global policy.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:57`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.FirstMatchingRuleWinsAndUnknownContextUsesGlobal @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:16

## environment.yellow-red

YellowRed mode asks for every nongreen result.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/GuardSettings.cs:29`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.ModesControlWarnings @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113

## green-statement.begintransactionstatement

ReadOnlyStatementTypes contains BeginTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.breakstatement

ReadOnlyStatementTypes contains BreakStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.closecursorstatement

ReadOnlyStatementTypes contains CloseCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.closemasterkeystatement

ReadOnlyStatementTypes contains CloseMasterKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:122`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.closesymmetrickeystatement

ReadOnlyStatementTypes contains CloseSymmetricKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:121`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.committransactionstatement

ReadOnlyStatementTypes contains CommitTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.continuestatement

ReadOnlyStatementTypes contains ContinueStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.cursorstatement

ReadOnlyStatementTypes contains CursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: No reviewed association.

## green-statement.deallocatecursorstatement

ReadOnlyStatementTypes contains DeallocateCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.declarecursorstatement

ReadOnlyStatementTypes contains DeclareCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.declaretablevariablestatement

ReadOnlyStatementTypes contains DeclareTableVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.declarevariablestatement

ReadOnlyStatementTypes contains DeclareVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.executeasstatement

ReadOnlyStatementTypes contains ExecuteAsStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:118`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.fetchcursorstatement

ReadOnlyStatementTypes contains FetchCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.getconversationgroupstatement

ReadOnlyStatementTypes contains GetConversationGroupStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:122`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.gotostatement

ReadOnlyStatementTypes contains GoToStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.labelstatement

ReadOnlyStatementTypes contains LabelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.linenostatement

ReadOnlyStatementTypes contains LineNoStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:122`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.opencursorstatement

ReadOnlyStatementTypes contains OpenCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.openmasterkeystatement

ReadOnlyStatementTypes contains OpenMasterKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:121`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.opensymmetrickeystatement

ReadOnlyStatementTypes contains OpenSymmetricKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:121`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.printstatement

ReadOnlyStatementTypes contains PrintStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.raiseerrorlegacystatement

ReadOnlyStatementTypes contains RaiseErrorLegacyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: No reviewed association.

## green-statement.raiseerrorstatement

ReadOnlyStatementTypes contains RaiseErrorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.readtextstatement

ReadOnlyStatementTypes contains ReadTextStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:121`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.returnstatement

ReadOnlyStatementTypes contains ReturnStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.revertstatement

ReadOnlyStatementTypes contains RevertStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:118`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.rollbacktransactionstatement

ReadOnlyStatementTypes contains RollbackTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.savetransactionstatement

ReadOnlyStatementTypes contains SaveTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.setcommandstatement

ReadOnlyStatementTypes contains SetCommandStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.seterrorlevelstatement

ReadOnlyStatementTypes contains SetErrorLevelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.setidentityinsertstatement

ReadOnlyStatementTypes contains SetIdentityInsertStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.setonoffstatement

ReadOnlyStatementTypes contains SetOnOffStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: No reviewed association.

## green-statement.setrowcountstatement

ReadOnlyStatementTypes contains SetRowCountStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.settextsizestatement

ReadOnlyStatementTypes contains SetTextSizeStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.settransactionisolationlevelstatement

ReadOnlyStatementTypes contains SetTransactionIsolationLevelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.setuserstatement

ReadOnlyStatementTypes contains SetUserStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.setvariablestatement

ReadOnlyStatementTypes contains SetVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.throwstatement

ReadOnlyStatementTypes contains ThrowStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.transactionstatement

ReadOnlyStatementTypes contains TransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: No reviewed association.

## green-statement.usefederationstatement

ReadOnlyStatementTypes contains UseFederationStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:122`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.usestatement

ReadOnlyStatementTypes contains UseStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## green-statement.waitforstatement

ReadOnlyStatementTypes contains WaitForStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:118`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## hidden.adhoc

OPENDATASOURCE source warns about opaque remote effects.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:102`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.RemoteSources_AreUnknown @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30

## hidden.bulk-file

OPENROWSET BULK file reading is intentionally not a remote side effect.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:105`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.OpenRowsetBulk_ReadsAFile_IsReadOnly @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:57

## hidden.deduplicate

Resolve direct/alias/CTE/output write fragments so remote writes are not double-counted as opaque read sources.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:41`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.RemoteWriteTarget_IsReportedOnceAsRealWrite @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:49

## hidden.internal-openrowset

Internal OPENROWSET source warns; remote effects not inspected.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:99`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.RemoteSources_AreUnknown @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30

## hidden.openquery

OPENQUERY source can execute remote pass-through text and warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:92`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.OpenQuerySource_IsUnknown @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:11

## hidden.openrowset

OPENROWSET provider query source warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:96`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.RemoteSources_AreUnknown @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30

## hidden.sequence

NEXT VALUE FOR adds permanent sequence-advance finding independently of main statement target.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:107`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.NextValueFor_AnywhereThatRuns_IsRealWrite @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:96

## limits.action-reset

Top-level action tokens reset condition nesting but do not reset total control-flow count.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:54`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.NestingThatDefeatsStatementCountersIsRejectedBeforeParsing @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:58

## limits.cancel

Cancellation is propagated rather than silently returning green.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:33`.
- Existing tests: No reviewed association.

## limits.control-count

More than 5000 IF/WHILE/ELSE keywords per batch rejects analysis; only GO resets count.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:71`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.ControlFlowKeywordBudgetHasExactBatchBoundary @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:78; SqlSafetyGuard.Tests.AnalysisResourceTests.ControlFlowKeywordBudgetResetsOnlyAtGo @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:90

## limits.nesting-count

Reject parentheses/blocks above 256 or condition/else-condition nesting above 512.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:73`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.ExcessiveNestingReturnsUnknownWithoutParsing @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:144

## limits.syntax

Bound parentheses/blocks, conditions and control tokens before ScriptDom recursion; GO resets nesting.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:39`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.ExcessiveNestingReturnsUnknownWithoutParsing @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:144

## limits.work

Cancellation propagates; 40 million work units or ten seconds abort to conservative warning.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:29`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.LongSingleLineSelectAndLargeBrokenBatchAreBounded @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:211

## parser.lexical-errors

Combine tokenization and parser errors instead of dropping lexical diagnostics.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:109`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.UnrecognizedCharacter_DoesNotHideWhatFollows @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:147

## msdb-procedure.sp_add_job

Explicit msdb.sys/msdb.dbo sp_add_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_add_jobschedule

Explicit msdb.sys/msdb.dbo sp_add_jobschedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_add_jobserver

Explicit msdb.sys/msdb.dbo sp_add_jobserver qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_add_jobstep

Explicit msdb.sys/msdb.dbo sp_add_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_add_schedule

Explicit msdb.sys/msdb.dbo sp_add_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_attach_schedule

Explicit msdb.sys/msdb.dbo sp_attach_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_delete_backuphistory

Explicit msdb.sys/msdb.dbo sp_delete_backuphistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_delete_job

Explicit msdb.sys/msdb.dbo sp_delete_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_delete_jobstep

Explicit msdb.sys/msdb.dbo sp_delete_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_help_job

Explicit msdb.sys/msdb.dbo sp_help_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_help_jobhistory

Explicit msdb.sys/msdb.dbo sp_help_jobhistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_help_jobstep

Explicit msdb.sys/msdb.dbo sp_help_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_help_schedule

Explicit msdb.sys/msdb.dbo sp_help_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_purge_jobhistory

Explicit msdb.sys/msdb.dbo sp_purge_jobhistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_send_dbmail

Explicit msdb.sys/msdb.dbo sp_send_dbmail qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:999`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_start_job

Explicit msdb.sys/msdb.dbo sp_start_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_stop_job

Explicit msdb.sys/msdb.dbo sp_stop_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1001`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_update_job

Explicit msdb.sys/msdb.dbo sp_update_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## msdb-procedure.sp_update_jobstep

Explicit msdb.sys/msdb.dbo sp_update_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1000`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SystemNamesOutsideSystemNamespaceAreUnknown @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121; SqlSafetyGuard.Tests.SystemProcedureTests.KnownWriters_AreRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:14

## options.defaults

Defaults are empty allow/trust lists, permanent tempdb, dynamic depth three and unknown SQLCMD mode; negative depth clamps to zero and caller context is preserved.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:11`.
- Existing tests: No reviewed association.

## parser.newest

Select newest numeric ScriptDom parser with quoted identifiers on; fallback parser if discovery cannot construct it.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:13`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UsesNewestParser @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:11

## position.offset

Offsets clamp at text length; CRLF is one newline and CR/LF independently advance line numbers. Empty text starts at line one, column one.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:8`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.Positions_AreRelativeToAnalyzedText @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:272

## finding.metadata

Finding stores as-written Target independently from ResolvedTarget; display arrows and all-rows notes are presentation only and must not be parsed as object identity. Null strings normalize to empty.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:14`.
- Existing tests: SqlSafetyGuard.Tests.NoWhereTests.AllRowsSummary_DescribesTheActualChange @ tests/SqlSafetyGuard.Tests/NoWhereTests.cs:68

## recovery.regions

Bound retries/time/text size, preserve newline/offsets, and avoid treating malformed module bodies as executable scripts.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:185`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.GiveUp_WholeBatchIsNotAnalyzed_AndScanned @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:158

## recovery-keyword.alter

Unparsed non-trivia Alter token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.bulk

Unparsed non-trivia Bulk token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.create

Unparsed non-trivia Create token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.dbcc

Unparsed non-trivia Dbcc token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.delete

Unparsed non-trivia Delete token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.deny

Unparsed non-trivia Deny token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.drop

Unparsed non-trivia Drop token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.execute

Unparsed non-trivia Execute token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.grant

Unparsed non-trivia Grant token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.insert

Unparsed non-trivia Insert token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.kill

Unparsed non-trivia Kill token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.merge

Unparsed non-trivia Merge token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.reconfigure

Unparsed non-trivia Reconfigure token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.restore

Unparsed non-trivia Restore token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.revoke

Unparsed non-trivia Revoke token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.shutdown

Unparsed non-trivia Shutdown token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.truncate

Unparsed non-trivia Truncate token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## recovery-keyword.update

Unparsed non-trivia Update token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114

## resolve.alias

Resolve aliases before # name spelling and recursively resolve CTE/derived sources; explicit aliases outrank unaliased base names.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:257`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UpdateAliasPointingToCte_ResolvesThroughBoth @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:76; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.DeleteViaAliasWithoutFromKeyword_ResolvesToBaseTable @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:26

## resolve.alias-priority

Explicit alias wins over unaliased base-name match; an aliased table does not expose its original name.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:218`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.AllowListHonorsQualificationAndExactIdentifierEnd @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200

## resolve.cte-budget

More than 256 simultaneously visited CTE names aborts bounded analysis.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:135`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.DoublingCteGraphIsMemoizedAndDeduplicated @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:156

## resolve.cte-cache

Memoize resolved CTE bases; merge provenance and deduplicate identity without exponential expansion.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:123`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.DoublingCteGraphIsMemoizedAndDeduplicated @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:156

## resolve.cte-cycle

Recursive CTE visits stop on cycles; anchor sources already collected remain.

- Outcomes: Unknown, RealWrite, TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:129`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.RecursiveCte_DoesNotLoopForever @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:59

## resolve.derived

Follow derived query sources and include via-derived-table provenance.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:153`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UpdateViaDerivedTable_ResolvesToBaseTable @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:67

## resolve.join

Join source resolution follows both sides; parenthesized joins unwrap.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:163`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UpdateAliasPointingToCte_ResolvesThroughBoth @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:76

## resolve.no-base

No base found behind alias/CTE/derived target yields explicit unresolved target, not green.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:104`.
- Existing tests: No reviewed association.

## resolve.union

Binary query targets combine both branches conservatively.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:199`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.CteReferencingCte_ResolvesRecursively @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:42

## resolve.variable

Variable table targets resolve to table-variable kind.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:92`.
- Existing tests: SqlSafetyGuard.Tests.RequiredCasesTests.Case04_InsertTableVariable_IsTempOnly @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:37

## safe-dbcc.activecursors

ReadOnlyDbcc contains ActiveCursors; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.cachestats

ReadOnlyDbcc contains CacheStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:133`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checkalloc

ReadOnlyDbcc contains CheckAlloc; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checkcatalog

ReadOnlyDbcc contains CheckCatalog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checkconstraints

ReadOnlyDbcc contains CheckConstraints; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checkdb

ReadOnlyDbcc contains CheckDB; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checkfilegroup

ReadOnlyDbcc contains CheckFileGroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.checktable

ReadOnlyDbcc contains CheckTable; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.cursorstats

ReadOnlyDbcc contains CursorStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:133`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.extentinfo

ReadOnlyDbcc contains ExtentInfo; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:133`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.help

ReadOnlyDbcc contains Help; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.inputbuffer

ReadOnlyDbcc contains InputBuffer; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.loginfo

ReadOnlyDbcc contains LogInfo; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.memorystatus

ReadOnlyDbcc contains MemoryStatus; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.opentran

ReadOnlyDbcc contains OpenTran; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.outputbuffer

ReadOnlyDbcc contains OutputBuffer; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.perfmonstats

ReadOnlyDbcc contains PerfMonStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:133`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.proccache

ReadOnlyDbcc contains ProcCache; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showcontig

ReadOnlyDbcc contains ShowContig; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showfilestats

ReadOnlyDbcc contains ShowFileStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showoffrules

ReadOnlyDbcc contains ShowOffRules; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:134`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showonrules

ReadOnlyDbcc contains ShowOnRules; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:134`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showstatistics

ReadOnlyDbcc contains ShowStatistics; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showtext

ReadOnlyDbcc contains ShowText; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:134`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.showweights

ReadOnlyDbcc contains ShowWeights; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:134`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.sqlperf

ReadOnlyDbcc contains SqlPerf; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.tracestatus

ReadOnlyDbcc contains TraceStatus; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-dbcc.useroptions

ReadOnlyDbcc contains UserOptions; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_column_privileges

ReadOnlyProcedures contains sp_column_privileges; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_columns

ReadOnlyProcedures contains sp_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_databases

ReadOnlyProcedures contains sp_databases; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_depends

ReadOnlyProcedures contains sp_depends; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_describe_first_result_set

ReadOnlyProcedures contains sp_describe_first_result_set; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_describe_undeclared_parameters

ReadOnlyProcedures contains sp_describe_undeclared_parameters; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_fkeys

ReadOnlyProcedures contains sp_fkeys; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_getapplock

ReadOnlyProcedures contains sp_getapplock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_help

ReadOnlyProcedures contains sp_help; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_help_job

ReadOnlyProcedures contains sp_help_job; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_help_jobhistory

ReadOnlyProcedures contains sp_help_jobhistory; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_help_jobstep

ReadOnlyProcedures contains sp_help_jobstep; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_help_schedule

ReadOnlyProcedures contains sp_help_schedule; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpconstraint

ReadOnlyProcedures contains sp_helpconstraint; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpdb

ReadOnlyProcedures contains sp_helpdb; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpdbfixedrole

ReadOnlyProcedures contains sp_helpdbfixedrole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpextendedproc

ReadOnlyProcedures contains sp_helpextendedproc; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpfile

ReadOnlyProcedures contains sp_helpfile; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpfilegroup

ReadOnlyProcedures contains sp_helpfilegroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpindex

ReadOnlyProcedures contains sp_helpindex; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helplanguage

ReadOnlyProcedures contains sp_helplanguage; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helplogins

ReadOnlyProcedures contains sp_helplogins; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpntgroup

ReadOnlyProcedures contains sp_helpntgroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helprole

ReadOnlyProcedures contains sp_helprole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helprolemember

ReadOnlyProcedures contains sp_helprolemember; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helprotect

ReadOnlyProcedures contains sp_helprotect; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpserver

ReadOnlyProcedures contains sp_helpserver; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpsort

ReadOnlyProcedures contains sp_helpsort; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpsrvrole

ReadOnlyProcedures contains sp_helpsrvrole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpsrvrolemember

ReadOnlyProcedures contains sp_helpsrvrolemember; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helptext

ReadOnlyProcedures contains sp_helptext; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helptrigger

ReadOnlyProcedures contains sp_helptrigger; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_helpuser

ReadOnlyProcedures contains sp_helpuser; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_lock

ReadOnlyProcedures contains sp_lock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_pkeys

ReadOnlyProcedures contains sp_pkeys; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_readerrorlog

ReadOnlyProcedures contains sp_readerrorlog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_releaseapplock

ReadOnlyProcedures contains sp_releaseapplock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_server_info

ReadOnlyProcedures contains sp_server_info; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_set_session_context

ReadOnlyProcedures contains sp_set_session_context; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_spaceused

ReadOnlyProcedures contains sp_spaceused; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_special_columns

ReadOnlyProcedures contains sp_special_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_sproc_columns

ReadOnlyProcedures contains sp_sproc_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_statistics

ReadOnlyProcedures contains sp_statistics; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_stored_procedures

ReadOnlyProcedures contains sp_stored_procedures; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_table_privileges

ReadOnlyProcedures contains sp_table_privileges; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_tables

ReadOnlyProcedures contains sp_tables; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_who

ReadOnlyProcedures contains sp_who; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.sp_who2

ReadOnlyProcedures contains sp_who2; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## safe-procedure.xp_readerrorlog

ReadOnlyProcedures contains xp_readerrorlog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## scope.batch-offset

GO batch substrings preserve original offset and line deltas.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:88`.
- Existing tests: SqlSafetyGuard.Tests.RequiredCasesTests.Case15_SecondGoBatch_ReportsCorrectLine @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:137

## scope.blocked

Recovered batch retains blocking syntax-error line for WillRun metadata.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:95`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.OnlyTheBrokenBatchIsBlocked @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:38

## scope.display

Display text collapses whitespace and truncates long snippets; substitutions are offset-aware and do not confer trust.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:144`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.Positions_AreRelativeToAnalyzedText @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:272

## scope.dynamic

Nested dynamic findings inherit outer execution anchor, context and blocking error.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:98`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.DynamicSqlInsideBatch_ReportedAtExecLine @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:283

## scope.newlines

CRLF is one line break; CR and LF individually also advance source positions.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:121`.
- Existing tests: SqlSafetyGuard.Tests.RecoveryTests.RecoveredFindings_KeepExactPositions @ tests/SqlSafetyGuard.Tests/RecoveryTests.cs:128

## scope.target-truncate

Display targets truncate at 200 characters; classification must use AST rather than display.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:152`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.DbccUsesParsedArgumentsBeyondDisplayTruncation @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:94

## set-family.generalsetcommandtype.contextinfo

SET CONTEXT_INFO 0x01 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.datefirst

SET DATEFIRST 1 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.dateformat

SET DATEFORMAT ymd inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.deadlockpriority

SET DEADLOCK_PRIORITY LOW inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.language

SET LANGUAGE us_english inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.locktimeout

SET LOCK_TIMEOUT 1000 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.generalsetcommandtype.querygovernorcostlimit

SET QUERY_GOVERNOR_COST_LIMIT 0 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.compute

SET OFFSETS COMPUTE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.execute

SET OFFSETS EXECUTE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.from

SET OFFSETS FROM ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.order

SET OFFSETS ORDER ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.param

SET OFFSETS PARAM ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.procedure

SET OFFSETS PROCEDURE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.select

SET OFFSETS SELECT ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.statement

SET OFFSETS STATEMENT ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setoffsets.table

SET OFFSETS TABLE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setstatisticsoptions.io

SET STATISTICS IO ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setstatisticsoptions.profile

SET STATISTICS PROFILE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setstatisticsoptions.time

SET STATISTICS TIME ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-family.setstatisticsoptions.xml

SET STATISTICS XML ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansidefaults

SET ANSI_DEFAULTS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansinulldfltoff

SET ANSI_NULL_DFLT_OFF ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansinulldflton

SET ANSI_NULL_DFLT_ON ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansinulls

SET ANSI_NULLS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansipadding

SET ANSI_PADDING ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.ansiwarnings

SET ANSI_WARNINGS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.arithabort

SET ARITHABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.arithignore

SET ARITHIGNORE ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.concatnullyieldsnull

SET CONCAT_NULL_YIELDS_NULL ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.cursorcloseoncommit

SET CURSOR_CLOSE_ON_COMMIT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.disabledefcnstchk

SET DISABLE_DEF_CNST_CHK ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.fmtonly

SET FMTONLY ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.forceplan

SET FORCEPLAN ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.implicittransactions

SET IMPLICIT_TRANSACTIONS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.nobrowsetable

SET NO_BROWSETABLE ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.nocount

SET NOCOUNT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.noexec

SET NOEXEC ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.numericroundabort

SET NUMERIC_ROUNDABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.parseonly

SET PARSEONLY ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.quotedidentifier

SET QUOTED_IDENTIFIER ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.remoteproctransactions

SET REMOTE_PROC_TRANSACTIONS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.showplanall

SET SHOWPLAN_ALL ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.showplantext

SET SHOWPLAN_TEXT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.showplanxml

SET SHOWPLAN_XML ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## set-option.xactabort

SET XACT_ABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## sqlcmd.ordered-definitions

Only preceding setvar definitions supply display values; later definitions cannot retroactively grant trust.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:89`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SqlCmdDefinitionsDoNotApplyRetroactivelyAndOffsetsStayOriginal @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:143

## sqlcmd.process

Blank directive lines without shifting offsets, substitute only identifier-safe display names; unresolved references remain warnings.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:62`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.SqlCmdScript_ParsesWithoutErrors @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:20

## sqlcmd.remove-definition

A setvar without value removes the variable, leaving later references unresolved.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:96`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.connect

SQLCMD mode recognizes :connect; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.ed

SQLCMD mode recognizes :ed; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.error

SQLCMD mode recognizes :error; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.exit

SQLCMD mode recognizes :exit; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.help

SQLCMD mode recognizes :help; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.list

SQLCMD mode recognizes :list; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.listvar

SQLCMD mode recognizes :listvar; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.on

SQLCMD mode recognizes :on; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.os

SQLCMD mode recognizes !!; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:10`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.out

SQLCMD mode recognizes :out; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.perftrace

SQLCMD mode recognizes :perftrace; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.quit

SQLCMD mode recognizes :quit; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.r

SQLCMD mode recognizes :r; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.reset

SQLCMD mode recognizes :reset; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.serverlist

SQLCMD mode recognizes :serverlist; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.setvar

SQLCMD mode recognizes :setvar; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.variable

SQLCMD mode recognizes $(); connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:19`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## sqlcmd-command.xml

SQLCMD mode recognizes :xml; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42; SqlSafetyGuard.Tests.SoundnessRegressionTests.UnexpandedSqlCmdExecutionIsNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137

## unknown-dbcc.addinstance

DBCC AddInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.auditevent

DBCC AuditEvent is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.autopilot

DBCC AutoPilot is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.buffer

DBCC Buffer is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.bytes

DBCC Bytes is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.cacheprofile

DBCC CacheProfile is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.callfulltext

DBCC CallFullText is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.checkprimaryfile

DBCC CheckPrimaryFile is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.collectstats

DBCC CollectStats is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.concurrencyviolation

DBCC ConcurrencyViolation is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.debugbreak

DBCC DebugBreak is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.deleteinstance

DBCC DeleteInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumpconfig

DBCC DumpConfig is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumpdbinfo

DBCC DumpDBInfo is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumpdbtable

DBCC DumpDBTable is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumplock

DBCC DumpLock is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumplog

DBCC DumpLog is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumppage

DBCC DumpPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumpresource

DBCC DumpResource is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.dumptrigger

DBCC DumpTrigger is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.fileheader

DBCC FileHeader is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.flush

DBCC Flush is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.icecapquery

DBCC IcecapQuery is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.incrementinstance

DBCC IncrementInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.ind

DBCC Ind is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.invalidatetextptr

DBCC InvalidateTextptr is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.invalidatetextptrobjid

DBCC InvalidateTextptrObjid is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.latch

DBCC Latch is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.mapallocunit

DBCC MapAllocUnit is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.memobjlist

DBCC MemObjList is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.memorymap

DBCC MemoryMap is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.metadata

DBCC Metadata is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.notextptr

DBCC NoTextptr is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.optimizerwhatif

DBCC OptimizerWhatIf is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.persiststackhash

DBCC PersistStackHash is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.prtipage

DBCC PrtiPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.readpage

DBCC ReadPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.ruleoff

DBCC RuleOff is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.ruleon

DBCC RuleOn is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.semetadata

DBCC SeMetadata is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.setinstance

DBCC SetInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.showdbaffinity

DBCC ShowDBAffinity is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.showtableaffinity

DBCC ShowTableAffinity is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.sqlmgrstats

DBCC SqlMgrStats is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.stackdump

DBCC StackDump is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.tec

DBCC Tec is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.throttleio

DBCC ThrottleIO is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-dbcc.useplan

DBCC UsePlan is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:214`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## unknown-statement.backupstatement

UnknownStatementTypes contains BackupStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:221`.
- Existing tests: No reviewed association.

## unknown-statement.checkpointstatement

UnknownStatementTypes contains CheckpointStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:223`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## unknown-statement.updatestatisticsstatement

UnknownStatementTypes contains UpdateStatisticsStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:222`.
- Existing tests: SqlSafetyGuard.Tests.StatementVerdictsTests.CommittedStatementVerdictsMatchCurrentAnalyzer @ tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:16

## exec.dispatch

Remote/variable execution is Unknown; qualify system names before built-in writer/safe tables or trusted procedures.

- Outcomes: ReadOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:811`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.LinkedServerExec_IsUnknown @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:226; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.ProcedureNameInVariable_IsUnknown @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:134; SqlSafetyGuard.Tests.EnvironmentRuleTests.ProcedureTrustRequiresResolvedQualification @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84

## exec.dynamic

Literal strings and literal concatenation recurse with outer EXEC location; runtime text and excess nesting warn Unknown.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1016`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.DynamicSqlConcatenatedLiterals_AreAnalyzed @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:101; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.DynamicSqlWithVariablePart_IsUnknown @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:108; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.NestedDynamicSql_IsLimitedByDepth @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:115

## exec.empty

No findings from literal dynamic text yields empty-dynamic-SQL green finding.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1028`.
- Existing tests: No reviewed association.

## exec.linked

EXEC AT linked server is opaque even when text is literal.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:820`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.LinkedServerExec_IsUnknown @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:226

## exec.missing

Missing executable specification cannot be analyzed and warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:814`.
- Existing tests: No reviewed association.

## exec.namespace

System dispatch permits local sys/dbo and qualified master; explicit msdb supports the enumerated Agent/mail/history names only.

- Outcomes: ReadOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:988`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.SchemaQualifiedReadOnlyProc_IsReadOnly @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:141

## exec.ordinary

Unrecognized procedure calls warn; INSERT EXEC warning is in addition to destination write.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:877`.
- Existing tests: SqlSafetyGuard.Tests.SystemProcedureTests.UnknownProcedure_StaysYellow @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:31; SqlSafetyGuard.Tests.RequiredCasesTests.Case13_InsertExec_IsUnknown @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:120

## exec.sp-executesql

Only qualified system sp_executesql with literal first/@stmt argument recurses; runtime argument warns.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:842`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.SpExecuteSqlWithLiteral_IsAnalyzed @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:86; SqlSafetyGuard.Tests.RequiredCasesTests.Case10_SpExecuteSqlWithVariable_IsUnknown @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:93

## exec.spaceused

sp_spaceused updateusage absent/false/0 is green; unknown or enabled usage correction is red.

- Outcomes: ReadOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1005`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.RealWritesAreNeverGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:75; SqlSafetyGuard.Tests.SoundnessRegressionTests.OrdinaryReadsRemainReadOnly @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:107

## exec.streaming-wrapper

System sp_create_streaming_job is a writing procedure independently of batch-level parser rewriting, control-flow wrappers, or literal dynamic SQL.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:811`.
- Existing tests: SqlSafetyGuard.Tests.MetamorphicVerificationTests.StreamingProcedureIsRedInEveryWrapper @ tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:224; SqlSafetyGuard.Tests.MetamorphicVerificationTests.StreamingParserHotswapIsOnlyAtBatchLevel @ tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:265; SqlSafetyGuard.Tests.MetamorphicVerificationTests.ProcedureTableVerdictsAreIndependentOfNesting @ tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:250

## exec.trusted

A qualified trusted-procedure pattern grants ReadOnly after built-in writer overrides. Body not inspected.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:870`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.ProcedureTrustRequiresResolvedQualification @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84

## sqlcmd.mode-off

When mode is off, recognizable SQLCMD syntax adds a mode-off hint at first actual syntax occurrence.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:346`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.ModeOff_SqlCmdLinesAreSyntaxErrors_WithHint @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:72; SqlSafetyGuard.Tests.SqlCmdTests.ModeOff_DollarParenInAString_IsJustAString @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:97

## sqlcmd.walk

connect invalidates target context; include/OS/query-exit and variable expansion warn; ordinary directives and plain exit/quit are green.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:351`.
- Existing tests: SqlSafetyGuard.Tests.SqlCmdTests.Directives_AreReported @ tests/SqlSafetyGuard.Tests/SqlCmdTests.cs:42

## txn.begin

Track nested depth and highest opening position; implicit mode can add an extra nesting level.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:70`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.BalancedTransaction_IsGreen @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:25; SqlSafetyGuard.Tests.TransactionTests.NestedTransactions_Balanced @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:33

## txn.commit

Commit decreases tracked depth; walker warns for lone or extra unguarded commits.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:86`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.LoneCommit_IsUnknown @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:10; SqlSafetyGuard.Tests.TransactionTests.ExtraCommit_IsUnknown @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:18

## txn.finish

Warn for open or uncertain transactions at end, including implicit mode when work opened a transaction.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:157`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.BeginWithoutCommit_ReportsOpenTransaction @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:39; SqlSafetyGuard.Tests.TransactionTests.ImplicitTransactions_LeftOpen_IsReported @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:119

## txn.if

Both IF branches are analyzed; TRANCOUNT/XACT_STATE predicates refine transaction path state.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:910`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.GuardedCommit_IsNotLone @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:91; SqlSafetyGuard.Tests.TransactionTests.XactStateGuard_ClosesTheTransaction @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:98

## txn.implicit-start

Only enumerated operations start an implicit transaction; named SELECT tables are checked.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:139`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.ImplicitTransactions_Committed_IsFine @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:128

## txn.merge

Merge branch depth, peaks, implicit and uncertainty; terminated paths are handled separately.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:117`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.IfCommitElseRollback_IsBalanced @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:67; SqlSafetyGuard.Tests.TransactionTests.UnguardedIf_StaysConservative @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:105

## txn.rollback

Full rollback clears depth; known savepoint rollback preserves the open transaction.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:95`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.RollbackCloses @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:52; SqlSafetyGuard.Tests.TransactionTests.RollbackToSavepoint_KeepsTransactionOpen @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:60

## walk.blocked-transaction

A syntactically blocked batch must not change tracked transaction state.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:495`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.TransactionsInBrokenBatch_DontCount @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:136

## walk.catch-exit

CATCH termination with open or uncertain transaction adds TRANSACTION FLOW warning.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:550`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.PossiblyOpenTransactionsAreReported @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:160

## walk.composable

Composable nested DML adds independent base-target and OUTPUT INTO findings.

- Outcomes: TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:744`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.NestedModificationOutputIsAlsoClassified @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:83

## walk.depth

Statement recursion exceeding 1000 terminates analysis conservatively.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:470`.
- Existing tests: No reviewed association.

## walk.dml-targets

Resolve each DML base; unresolved target is Unknown, remote base RealWrite, table variable TempOnly.

- Outcomes: TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1117`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.RemoteTargets_AreRealWrite @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:220; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UpdateViaAliasOfTemp_IsTempOnly @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:35

## walk.drop-index

Legacy index names use the table portion for temporary classification, not the index name.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:787`.
- Existing tests: No reviewed association.

## walk.drop-procedures

DROP PROCEDURE classifies each object separately and disallows table allow-list.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:724`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.TableDdlAndTruncate @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180

## walk.drop-statistics

For table.statistics names use penultimate identifier to detect temporary tables.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:677`.
- Existing tests: SqlSafetyGuard.Tests.FalseAlarmTests.DropStatistics_FollowsTempRules @ tests/SqlSafetyGuard.Tests/FalseAlarmTests.cs:28

## walk.fallback

All unrecognized statement kinds default RealWrite; module definitions receive body-not-run note.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:730`.
- Existing tests: SqlSafetyGuard.Tests.AstOnlyKindTests.ConstructedAstUsesReviewedFallbackWithoutClaimingSqlExecution @ tests/SqlSafetyGuard.Tests/AstOnlyKindTests.cs:20

## walk.guess-target

Fallback extracts a module/drop/name/database target via ordered reflection; failing getters are ignored.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1240`.
- Existing tests: No reviewed association.

## walk.hidden

Scan execution expressions for remote sources and sequence increments; skip already-counted write fragments.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:738`.
- Existing tests: SqlSafetyGuard.Tests.HiddenSideEffectTests.RemoteSources_AreUnknown @ tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30

## walk.insert-exec

INSERT EXEC combines destination verdict with execution effects; nested composable DML is traversed.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:769`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.InsertExecOfReadOnlyProc_IsTempOnly @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:147; SqlSafetyGuard.Tests.SystemProcedureTests.InsertExecOfKnownWriter_IsRed @ tests/SqlSafetyGuard.Tests/SystemProcedureTests.cs:37

## walk.module

Classify module definition itself; do not execute or independently scan stored body statements.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1237`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.DefinitionAndSecurityChanges_AreRealWrite @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:197

## walk.named-target

Named DDL/TRUNCATE/INTO/bulk targets use resolved qualified names and eligibility flags.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1192`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.TableDdlAndTruncate @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180

## walk.output

OUTPUT INTO writes its own target independently of the main DML target.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1184`.
- Existing tests: SqlSafetyGuard.Tests.RequiredCasesTests.Case07_OutputIntoPermanentTable_IsRealWrite @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:65; SqlSafetyGuard.Tests.SoundnessRegressionTests.NestedModificationOutputIsAlsoClassified @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:83

## walk.parse

Top-level parse failures recover bounded batches; literal dynamic parse failure remains Unknown.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:179`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.ParseErrorInOneBatch_OtherBatchesStillAnalyzed @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:232; SqlSafetyGuard.Tests.AdditionalAnalyzerTests.UnparseableDynamicSql_IsUnknown @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:126

## walk.repeat

A GO repetition count other than one requires warning; do not assume a single execution.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:445`.
- Existing tests: No reviewed association.

## walk.return

RETURN marks current transaction path terminated, but does not suppress classification of later writes.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:517`.
- Existing tests: SqlSafetyGuard.Tests.AnalysisResourceTests.EarlyReturnPreservesContinuingTransactionPaths @ tests/SqlSafetyGuard.Tests/AnalysisResourceTests.cs:125

## walk.rowfilter

WHERE, non-percent TOP, restricting joins and CTE/derived resolution change all-rows notes; percent TOP and preserved outer sides do not establish restriction.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1035`.
- Existing tests: SqlSafetyGuard.Tests.NoWhereTests.AllRowsSummary_DescribesTheActualChange @ tests/SqlSafetyGuard.Tests/NoWhereTests.cs:68

## walk.select-into

SELECT INTO classifies its destination; plain SELECT begins ReadOnly then scans hidden effects.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:598`.
- Existing tests: SqlSafetyGuard.Tests.RequiredCasesTests.Case06_SelectIntoPermanentTable_IsRealWrite @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:57; SqlSafetyGuard.Tests.RequiredCasesTests.Case02_SelectIntoTempThenDeleteTemp_IsTempOnly @ tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:20

## walk.switch-both

ALTER TABLE SWITCH classifies source and destination independently, without allow-list.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:648`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.SwitchClassifiesBothTables @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:15

## walk.target-switch

USE or federation/context switch invalidates contextual allow lists and trusted procedures for following statements.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:437`.
- Existing tests: SqlSafetyGuard.Tests.EnvironmentRuleTests.TargetSwitchesDisableContextualTrust @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:99; SqlSafetyGuard.Tests.EnvironmentRuleTests.FederationSwitchInvalidatesTargetContext @ tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:222

## walk.temp-procedure

A temporary procedure definition is TempOnly; its stored body does not execute now.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:719`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.TemporaryProcedureDefinitionsDoNotExecuteBodies @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:186

## walk.throw

THROW terminates the transaction path; following statement findings remain conservatively included.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:521`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.TransactionUncertaintyIsNotGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:177

## walk.transfer

BREAK/CONTINUE/GOTO are Unknown while a transaction or implicit mode may skip cleanup, otherwise ReadOnly.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:525`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.TransactionUncertaintyIsNotGreen @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:177

## walk.try-peak

CATCH begins with potential peak depth from TRY, including a TRY that later committed.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:541`.
- Existing tests: SqlSafetyGuard.Tests.TransactionTests.CommitInCatch_AfterTryOpened_IsNotLone @ tests/SqlSafetyGuard.Tests/TransactionTests.cs:84

## walk.waitfor

WAITFOR with an embedded statement recursively uses that statement verdict.

- Outcomes: ReadOnly, TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:592`.
- Existing tests: SqlSafetyGuard.Tests.SoundnessRegressionTests.WaitForPreservesInnerStatementVerdict @ tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:31

## walk.while-zero

Merge loop state with pre-loop state to account for zero iterations and scan loop predicate effects.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:509`.
- Existing tests: SqlSafetyGuard.Tests.AdditionalAnalyzerTests.WhileAndTryCatch_AreWalked @ tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:153

## writing-procedure.sp_add_job

WritingProcedures contains sp_add_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:86`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_add_jobschedule

WritingProcedures contains sp_add_jobschedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:92`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_add_jobserver

WritingProcedures contains sp_add_jobserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:95`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_add_jobstep

WritingProcedures contains sp_add_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:89`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_add_schedule

WritingProcedures contains sp_add_schedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:93`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addextendedproc

WritingProcedures contains sp_addextendedproc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:66`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addextendedproperty

WritingProcedures contains sp_addextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:63`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addlinkedserver

WritingProcedures contains sp_addlinkedserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:58`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addlinkedsrvlogin

WritingProcedures contains sp_addlinkedsrvlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:60`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addlogin

WritingProcedures contains sp_addlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:44`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addrolemember

WritingProcedures contains sp_addrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:40`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_addsrvrolemember

WritingProcedures contains sp_addsrvrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:42`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_adduser

WritingProcedures contains sp_adduser; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:48`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_attach_db

WritingProcedures contains sp_attach_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:78`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_attach_schedule

WritingProcedures contains sp_attach_schedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:94`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_attach_single_file_db

WritingProcedures contains sp_attach_single_file_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:79`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_bindefault

WritingProcedures contains sp_bindefault; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:70`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_bindrule

WritingProcedures contains sp_bindrule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:68`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_change_users_login

WritingProcedures contains sp_change_users_login; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:55`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_changedbowner

WritingProcedures contains sp_changedbowner; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:56`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_changeobjectowner

WritingProcedures contains sp_changeobjectowner; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:57`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_configure

WritingProcedures contains sp_configure; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:35`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_create_streaming_job

WritingProcedures contains sp_create_streaming_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:104`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_createstats

WritingProcedures contains sp_createstats; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:100`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_cycle_errorlog

WritingProcedures contains sp_cycle_errorlog; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:83`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dboption

WritingProcedures contains sp_dboption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:81`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_defaultdb

WritingProcedures contains sp_defaultdb; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:47`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_delete_backuphistory

WritingProcedures contains sp_delete_backuphistory; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:84`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_delete_job

WritingProcedures contains sp_delete_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:88`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_delete_jobstep

WritingProcedures contains sp_delete_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:91`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_denylogin

WritingProcedures contains sp_denylogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:54`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_detach_db

WritingProcedures contains sp_detach_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:80`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dropextendedproc

WritingProcedures contains sp_dropextendedproc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:67`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dropextendedproperty

WritingProcedures contains sp_dropextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:65`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_droplinkedsrvlogin

WritingProcedures contains sp_droplinkedsrvlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:61`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_droplogin

WritingProcedures contains sp_droplogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:45`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_droprolemember

WritingProcedures contains sp_droprolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:41`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dropserver

WritingProcedures contains sp_dropserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:59`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dropsrvrolemember

WritingProcedures contains sp_dropsrvrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:43`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_dropuser

WritingProcedures contains sp_dropuser; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:49`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_grantdbaccess

WritingProcedures contains sp_grantdbaccess; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:50`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_grantlogin

WritingProcedures contains sp_grantlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:52`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_monitor

WritingProcedures contains sp_monitor; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:33`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_msforeachdb

WritingProcedures contains sp_MSforeachdb; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:103`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_msforeachtable

WritingProcedures contains sp_MSforeachtable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:102`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_oacreate

WritingProcedures contains sp_OACreate; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:38`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_oamethod

WritingProcedures contains sp_OAMethod; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:39`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_password

WritingProcedures contains sp_password; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:46`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_procoption

WritingProcedures contains sp_procoption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:74`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_purge_jobhistory

WritingProcedures contains sp_purge_jobhistory; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:85`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_recompile

WritingProcedures contains sp_recompile; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:77`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_refreshsqlmodule

WritingProcedures contains sp_refreshsqlmodule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:76`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_refreshview

WritingProcedures contains sp_refreshview; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:75`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_rename

WritingProcedures contains sp_rename; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:34`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_resetstatus

WritingProcedures contains sp_resetstatus; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:82`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_revokedbaccess

WritingProcedures contains sp_revokedbaccess; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:51`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_revokelogin

WritingProcedures contains sp_revokelogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:53`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_send_dbmail

WritingProcedures contains sp_send_dbmail; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:37`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_serveroption

WritingProcedures contains sp_serveroption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:62`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_settriggerorder

WritingProcedures contains sp_settriggerorder; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:73`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_start_job

WritingProcedures contains sp_start_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:96`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_stop_job

WritingProcedures contains sp_stop_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:97`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_tableoption

WritingProcedures contains sp_tableoption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:72`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_trace_create

WritingProcedures contains sp_trace_create; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:98`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_trace_setstatus

WritingProcedures contains sp_trace_setstatus; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:99`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_unbindefault

WritingProcedures contains sp_unbindefault; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:71`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_unbindrule

WritingProcedures contains sp_unbindrule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:69`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_update_job

WritingProcedures contains sp_update_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:87`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_update_jobstep

WritingProcedures contains sp_update_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:90`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_updateextendedproperty

WritingProcedures contains sp_updateextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:64`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.sp_updatestats

WritingProcedures contains sp_updatestats; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:101`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory

## writing-procedure.xp_cmdshell

WritingProcedures contains xp_cmdshell; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:36`.
- Existing tests: SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory
