# Analyzer rule inventory

Generated from rules.json; regenerate with `python tools/verification/generate_inventory.py`.

`decision.*` entries are overinclusive source checkpoints awaiting semantic review. They deliberately expose uncertainty; neither mappings nor coverage prove server behavior.

| Category | Entries |
| --- | ---: |
| action-name | 31 |
| aggregation | 33 |
| allow-list | 41 |
| changing-dbcc | 33 |
| classification | 53 |
| entry | 22 |
| environment | 41 |
| green-statement | 43 |
| hidden-effects | 30 |
| limits | 64 |
| msdb-procedure | 19 |
| options | 5 |
| parser | 18 |
| positions | 10 |
| presentation | 27 |
| recovery | 112 |
| recovery-keyword | 18 |
| resolution | 81 |
| safe-dbcc | 28 |
| safe-procedure | 50 |
| scope | 44 |
| set-family | 20 |
| set-option | 25 |
| sqlcmd | 23 |
| sqlcmd-command | 18 |
| unknown-dbcc | 48 |
| unknown-statement | 3 |
| walking | 491 |
| writing-procedure | 70 |

## action-name.altertableaddclusterbystatement

ActionNames contains AlterTableAddClusterByStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:254`.
- Existing tests: No reviewed association.

## action-name.altertableaddtableelementstatement

ActionNames contains AlterTableAddTableElementStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:242`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablealtercolumnstatement

ActionNames contains AlterTableAlterColumnStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:243`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablealterindexstatement

ActionNames contains AlterTableAlterIndexStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:252`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablealterpartitionstatement

ActionNames contains AlterTableAlterPartitionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:253`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablechangetrackingmodificationstatement

ActionNames contains AlterTableChangeTrackingModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:250`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertableconstraintmodificationstatement

ActionNames contains AlterTableConstraintModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:245`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertabledroptableelementstatement

ActionNames contains AlterTableDropTableElementStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:244`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablefiletablenamespacestatement

ActionNames contains AlterTableFileTableNamespaceStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:251`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablerebuildstatement

ActionNames contains AlterTableRebuildStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:248`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertablesetstatement

ActionNames contains AlterTableSetStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:246`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertableswitchstatement

ActionNames contains AlterTableSwitchStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:247`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.altertabletriggermodificationstatement

ActionNames contains AlterTableTriggerModificationStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:249`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.backupdatabasestatement

ActionNames contains BackupDatabaseStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:238`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.backuptransactionlogstatement

ActionNames contains BackupTransactionLogStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:239`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.begintransactionstatement

ActionNames contains BeginTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:230`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.bulkinsertstatement

ActionNames contains BulkInsertStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:255`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.committransactionstatement

ActionNames contains CommitTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:231`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.dbccstatement

ActionNames contains DbccStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:237`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.declarecursorstatement

ActionNames contains DeclareCursorStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:229`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.declaretablevariablestatement

ActionNames contains DeclareTableVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:228`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.declarevariablestatement

ActionNames contains DeclareVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:227`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.executeasstatement

ActionNames contains ExecuteAsStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:240`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.gotostatement

ActionNames contains GoToStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:236`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.insertbulkstatement

ActionNames contains InsertBulkStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:256`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.raiseerrorlegacystatement

ActionNames contains RaiseErrorLegacyStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:235`.
- Existing tests: No reviewed association.

## action-name.raiseerrorstatement

ActionNames contains RaiseErrorStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:234`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.rollbacktransactionstatement

ActionNames contains RollbackTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:232`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.savetransactionstatement

ActionNames contains SaveTransactionStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:233`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.setvariablestatement

ActionNames contains SetVariableStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:226`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## action-name.updatestatisticsstatement

ActionNames contains UpdateStatisticsStatement; choose this action label; verdict unchanged.

- Outcomes: annotation only; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:241`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## aggregate.max

Overall is the maximum finding severity, with parse errors flooring it at Unknown. Blocked findings remain included.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:25`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:263 (Warnings_AndSummary)

## decision.analysisresult.017207a826ef

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:72`.
- Existing tests: No reviewed association.
- Decision: `parts.Add(ParseErrors.Count == 1 ? "1 syntax error" : ParseErrors.Count + " syntax errors");`.

## decision.analysisresult.03739fc4ed7f

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:78`.
- Existing tests: No reviewed association.
- Decision: `var more = writes + unknown > 0 ? "more " : string.Empty;`.

## decision.analysisresult.086e5a84ca4d

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:58`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.TempOnly: return "Writes only to temp objects";`.

## decision.analysisresult.2f4fc67a8c77

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:70`.
- Existing tests: No reviewed association.
- Decision: `parts.Add(unknown + " unknown");`.

## decision.analysisresult.331530df59c9

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:82`.
- Existing tests: No reviewed association.
- Decision: `return text;`.

## decision.analysisresult.3d0cb8a92d17

In AnalysisResult, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:38`.
- Existing tests: No reviewed association.
- Decision: `public bool IsGreen => Overall.IsGreen();`.

## decision.analysisresult.3f8a3fe6cf38

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:71`.
- Existing tests: No reviewed association.
- Decision: `if (ParseErrors.Count > 0)`.

## decision.analysisresult.46ce08d83304

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:66`.
- Existing tests: No reviewed association.
- Decision: `int unknown = Findings.Count(f => f.WillRun && f.Verdict == Verdict.Unknown && !f.IsDiagnostic);`.

## decision.analysisresult.50f1668e5dd8

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:68`.
- Existing tests: No reviewed association.
- Decision: `parts.Add(writes == 1 ? "1 real write" : writes + " real writes");`.

## decision.analysisresult.60906e23d317

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:54`.
- Existing tests: No reviewed association.
- Decision: `if (IsGreen)`.

## decision.analysisresult.6234f26bebcc

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:60`.
- Existing tests: No reviewed association.
- Decision: `default: return "Read-only";`.

## decision.analysisresult.65fbead900fd

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:59`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.AllowListed: return "Writes only to allow-listed tables";`.

## decision.analysisresult.6b7fb61a21be

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:76`.
- Existing tests: No reviewed association.
- Decision: `if (blocked > 0)`.

## decision.analysisresult.8559c3d5ce74

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:43`.
- Existing tests: No reviewed association.
- Decision: `public IEnumerable<Finding> Warnings =>`.

## decision.analysisresult.95f361423602

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:65`.
- Existing tests: No reviewed association.
- Decision: `int writes = Findings.Count(f => f.WillRun && f.Verdict == Verdict.RealWrite);`.

## decision.analysisresult.c1ae6d7329bd

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:40`.
- Existing tests: No reviewed association.
- Decision: `public int Count(Verdict verdict) => Findings.Count(f => f.Verdict == verdict);`.

## decision.analysisresult.c2e49163389b

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:44`.
- Existing tests: No reviewed association.
- Decision: `Findings.Where(f => !f.Verdict.IsGreen()).OrderByDescending(f => f.Verdict).ThenBy(f => f.StartOffset);`.

## decision.analysisresult.c7f15cb19cd5

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:69`.
- Existing tests: No reviewed association.
- Decision: `if (unknown > 0)`.

## decision.analysisresult.dbd32ca4a1dd

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:67`.
- Existing tests: No reviewed association.
- Decision: `if (writes > 0)`.

## decision.analysisresult.f18698b1c450

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:79`.
- Existing tests: No reviewed association.
- Decision: `text += (text.Length == 0 ? string.Empty : " · ") + $"{blocked} {more}in a batch that will fail";`.

## decision.analysisresult.f2393f0a665b

In Count, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:47`.
- Existing tests: No reviewed association.
- Decision: `public int BlockedWarningCount => Findings.Count(f => !f.WillRun && !f.Verdict.IsGreen());`.

## decision.analysisresult.fa843db9fc3c

In AnalysisResult, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisResult.cs:23`.
- Existing tests: No reviewed association.
- Decision: `if (parseErrors.Count > 0)`.

## decision.verdict.110608503abb

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:42`.
- Existing tests: No reviewed association.
- Decision: `default: return verdict.ToString();`.

## decision.verdict.4ef550b4118b

In IsGreen, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:27`.
- Existing tests: No reviewed association.
- Decision: `public static bool IsGreen(this Verdict verdict) => verdict <= Verdict.AllowListed;`.

## decision.verdict.5f6a563604d1

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:38`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.TempOnly: return "Temp only";`.

## decision.verdict.7b435361c0d5

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:37`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.ReadOnly: return "Read-only";`.

## decision.verdict.94c040bb0e50

In Max, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:46`.
- Existing tests: No reviewed association.
- Decision: `public static Verdict Max(Verdict a, Verdict b) => a >= b ? a : b;`.

## decision.verdict.ab5c149c795a

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:40`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.Unknown: return "Unknown";`.

## decision.verdict.af02b062d38a

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:41`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.RealWrite: return "Real write";`.

## decision.verdict.cdd4baa9c52c

In DisplayName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:39`.
- Existing tests: No reviewed association.
- Decision: `case Verdict.AllowListed: return "Allow-listed";`.

## decision.verdict.ddcb78b46dac

In IsYellow, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:29`.
- Existing tests: No reviewed association.
- Decision: `public static bool IsYellow(this Verdict verdict) => verdict == Verdict.Unknown;`.

## decision.verdict.f860e22e8b33

In IsRed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Verdict.cs:31`.
- Existing tests: No reviewed association.
- Decision: `public static bool IsRed(this Verdict verdict) => verdict == Verdict.RealWrite;`.

## allow.cross-database

A cross-database target needs at least a three-part pattern unless its database equals current context.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:48`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200 (AllowListHonorsQualificationAndExactIdentifierEnd)

## allow.default-context

Resolve absent schema/database using supplied default schema/current database, never invent dbo.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:44`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84 (ProcedureTrustRequiresResolvedQualification)

## allow.empty-name

A missing target name never matches.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:43`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:13 (Matches); tests/SqlSafetyGuard.Tests/AllowListTests.cs:26 (Matches); tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:40 (Matches)

## allow.glob

Glob * and ? match identifier parts case-insensitively with exact start/end and 100ms regex timeout.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:58`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200 (AllowListHonorsQualificationAndExactIdentifierEnd)

## allow.ignore-comments

Ignore empty, --comment and #comment pattern lines.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:18`.
- Existing tests: tests/SqlSafetyGuard.Tests/AllowListTests.cs:33 (IgnoresBlankAndCommentLines)

## allow.invalid

Reject unterminated quoting, empty parts, more than four parts, wildcard-only names and unsafe trusted-procedure qualifiers; report warnings.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:214 (WildcardOnlyAllowListIsIgnoredVisibly); tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84 (ProcedureTrustRequiresResolvedQualification)

## allow.match

Quote-aware name parsing and anchored case-insensitive glob matching restrict explicit cross-database/server targets; invalid patterns warn.

- Outcomes: AllowListed, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:35`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:13 (Matches); tests/SqlSafetyGuard.Tests/AllowListTests.cs:26 (Matches); tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:40 (Matches); tests/SqlSafetyGuard.Tests/AllowListTests.cs:33 (IgnoresBlankAndCommentLines); tests/SqlSafetyGuard.Tests/AllowListTests.cs:40 (EmptyAllowList_MatchesNothing)

## allow.quoted-parts

Quoted dots stay within identifiers, doubled delimiters unescape, trailing junk or unclosed quote rejects pattern.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:65`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:13 (Matches); tests/SqlSafetyGuard.Tests/AllowListTests.cs:26 (Matches); tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:40 (Matches)

## allow.remote

A remote target needs an explicit four-part pattern; a shorter pattern cannot confer trust.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:47`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200 (AllowListHonorsQualificationAndExactIdentifierEnd)

## decision.allowlist.0295ac6c4fca

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:25`.
- Existing tests: No reviewed association.
- Decision: `Warnings.Add("Ignored invalid " + (procedures ? "trusted procedure" : "allow-list") + " pattern: " + pattern);`.

## decision.allowlist.0558189e6768

In SplitLines, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:38`.
- Existing tests: No reviewed association.
- Decision: `(text ?? string.Empty).Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);`.

## decision.allowlist.102b72dce219

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:90`.
- Existing tests: No reviewed association.
- Decision: `return parts.ToArray();`.

## decision.allowlist.1944979fd89c

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:79`.
- Existing tests: No reviewed association.
- Decision: `else if (character == '.') { parts.Add(closed ? value.ToString() : value.ToString().Trim()); value.Clear(); closed = false; }`.

## decision.allowlist.196a18650237

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:47`.
- Existing tests: No reviewed association.
- Decision: `if (pattern.Length < 4 && !string.IsNullOrEmpty(server)) continue;`.

## decision.allowlist.1b11e86f16e3

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:17`.
- Existing tests: No reviewed association.
- Decision: `var pattern = (raw ?? string.Empty).Trim();`.

## decision.allowlist.3e7aaa9e7e4b

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:22`.
- Existing tests: No reviewed association.
- Decision: `parts.All(part => part.All(character => character == '*' || character == '?')) ||`.

## decision.allowlist.41130042276c

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:70`.
- Existing tests: No reviewed association.
- Decision: `if (quoted != '\0')`.

## decision.allowlist.54aa482f649e

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:29`.
- Existing tests: No reviewed association.
- Decision: `Patterns.Add(pattern);`.

## decision.allowlist.55f756a7ab0d

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:88`.
- Existing tests: No reviewed association.
- Decision: `if (quoted != '\0') return null;`.

## decision.allowlist.64a2de30d45f

In SplitLines, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:37`.
- Existing tests: No reviewed association.
- Decision: `public static IEnumerable<string> SplitLines(string text) =>`.

## decision.allowlist.6d74dc145970

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:72`.
- Existing tests: No reviewed association.
- Decision: `if (character == quoted)`.

## decision.allowlist.854b1740a28e

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:28`.
- Existing tests: No reviewed association.
- Decision: `_patterns.Add(parts.Select(ToRegex).ToArray());`.

## decision.allowlist.86f2115b6473

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:75`.
- Existing tests: No reviewed association.
- Decision: `else { quoted = '\0'; closed = true; }`.

## decision.allowlist.926e8c82aebb

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:18`.
- Existing tests: No reviewed association.
- Decision: `if (pattern.Length == 0 || pattern.StartsWith("--", StringComparison.Ordinal) || pattern.StartsWith("#", StringComparison.Ordinal))`.

## decision.allowlist.93b32cb4ef21

In ToRegex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:58`.
- Existing tests: No reviewed association.
- Decision: `internal static Regex ToRegex(string part) => new Regex("\\A" + Regex.Escape(part).Replace("\\*", ".*").Replace("\\?", ".") + "\\z",`.

## decision.allowlist.9d2e17644a9c

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:35`.
- Existing tests: No reviewed association.
- Decision: `public bool IsEmpty => _patterns.Count == 0;`.

## decision.allowlist.9df0a51c6d97

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:77`.
- Existing tests: No reviewed association.
- Decision: `else value.Append(character);`.

## decision.allowlist.9df0a51c6d97.2

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:86`.
- Existing tests: No reviewed association.
- Decision: `else value.Append(character);`.

## decision.allowlist.9fc61e62a8f1

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:89`.
- Existing tests: No reviewed association.
- Decision: `parts.Add(closed ? value.ToString() : value.ToString().Trim());`.

## decision.allowlist.a4ec36febd31

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:48`.
- Existing tests: No reviewed association.
- Decision: `if (pattern.Length < 3 && !string.IsNullOrEmpty(database) &&`.

## decision.allowlist.a8b440e74ff5

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:53`.
- Existing tests: No reviewed association.
- Decision: `if (matches) return true;`.

## decision.allowlist.aebb4a67c04a

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:55`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.allowlist.b13ed996bd9f

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:21`.
- Existing tests: No reviewed association.
- Decision: `if (parts == null || parts.Length > 4 || parts.Any(part => part.Length == 0) ||`.

## decision.allowlist.b97d212f7dc2

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:83`.
- Existing tests: No reviewed association.
- Decision: `quoted = character == '[' ? ']' : '"';`.

## decision.allowlist.c9e8040414bd

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:23`.
- Existing tests: No reviewed association.
- Decision: `(procedures && (parts.Length < 2 || parts.Take(parts.Length - 1).Any(part => part.IndexOfAny(new[] { '*', '?' }) >= 0))))`.

## decision.allowlist.cab3af42af2d

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:85`.
- Existing tests: No reviewed association.
- Decision: `else if (closed) { if (!char.IsWhiteSpace(character)) return null; }`.

## decision.allowlist.ceb5b1431160

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:43`.
- Existing tests: No reviewed association.
- Decision: `if (string.IsNullOrEmpty(name)) return false;`.

## decision.allowlist.d1c618a1c94a

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:80`.
- Existing tests: No reviewed association.
- Decision: `else if ((character == '[' || character == '"') && value.ToString().Trim().Length == 0 && !closed)`.

## decision.allowlist.d79aa287ea4b

In Matches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:44`.
- Existing tests: No reviewed association.
- Decision: `var actual = new[] { name, string.IsNullOrEmpty(schema) ? defaultSchema : schema, database ?? currentDatabase, server };`.

## decision.allowlist.dc3e35d8c418

In AllowList, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:15`.
- Existing tests: No reviewed association.
- Decision: `foreach (var raw in patterns ?? Enumerable.Empty<string>())`.

## decision.allowlist.ee324a9c8e39

In SplitName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AllowList.cs:74`.
- Existing tests: No reviewed association.
- Decision: `if (index + 1 < pattern.Length && pattern[index + 1] == quoted) { value.Append(character); index++; }`.

## changing-dbcc.addextendedproc

ChangingDbcc contains AddExtendedProc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:160`.
- Existing tests: No reviewed association.

## changing-dbcc.cleantable

ChangingDbcc contains CleanTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:144`.
- Existing tests: No reviewed association.

## changing-dbcc.clearspacecaches

ChangingDbcc contains ClearSpaceCaches; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:167`.
- Existing tests: No reviewed association.

## changing-dbcc.dbrecover

ChangingDbcc contains DBRecover; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:166`.
- Existing tests: No reviewed association.

## changing-dbcc.dbreindex

ChangingDbcc contains DBReindex; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:145`.
- Existing tests: No reviewed association.

## changing-dbcc.dbreindexall

ChangingDbcc contains DBReindexAll; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:146`.
- Existing tests: No reviewed association.

## changing-dbcc.dbrepair

ChangingDbcc contains DBRepair; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:152`.
- Existing tests: No reviewed association.

## changing-dbcc.detachdb

ChangingDbcc contains DetachDB; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:153`.
- Existing tests: No reviewed association.

## changing-dbcc.dropcleanbuffers

ChangingDbcc contains DropCleanBuffers; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:141`.
- Existing tests: No reviewed association.

## changing-dbcc.dropextendedproc

ChangingDbcc contains DropExtendedProc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:161`.
- Existing tests: No reviewed association.

## changing-dbcc.errorlog

ChangingDbcc contains ErrorLog; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:154`.
- Existing tests: No reviewed association.

## changing-dbcc.fixallocation

ChangingDbcc contains FixAllocation; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:165`.
- Existing tests: No reviewed association.

## changing-dbcc.flushprocindb

ChangingDbcc contains FlushProcInDB; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:155`.
- Existing tests: No reviewed association.

## changing-dbcc.forceghostcleanup

ChangingDbcc contains ForceGhostCleanup; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:156`.
- Existing tests: No reviewed association.

## changing-dbcc.free

ChangingDbcc contains Free; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:168`.
- Existing tests: No reviewed association.

## changing-dbcc.freeproccache

ChangingDbcc contains FreeProcCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:138`.
- Existing tests: No reviewed association.

## changing-dbcc.freesessioncache

ChangingDbcc contains FreeSessionCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:139`.
- Existing tests: No reviewed association.

## changing-dbcc.freesystemcache

ChangingDbcc contains FreeSystemCache; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:140`.
- Existing tests: No reviewed association.

## changing-dbcc.freezeio

ChangingDbcc contains FreezeIO; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:162`.
- Existing tests: No reviewed association.

## changing-dbcc.indexdefrag

ChangingDbcc contains IndexDefrag; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:147`.
- Existing tests: No reviewed association.

## changing-dbcc.movepage

ChangingDbcc contains MovePage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:164`.
- Existing tests: No reviewed association.

## changing-dbcc.pintable

ChangingDbcc contains PinTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:157`.
- Existing tests: No reviewed association.

## changing-dbcc.renamecolumn

ChangingDbcc contains RenameColumn; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:159`.
- Existing tests: No reviewed association.

## changing-dbcc.setcpuweight

ChangingDbcc contains SetCpuWeight; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:169`.
- Existing tests: No reviewed association.

## changing-dbcc.setioweight

ChangingDbcc contains SetIOWeight; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:170`.
- Existing tests: No reviewed association.

## changing-dbcc.shrinkdatabase

ChangingDbcc contains ShrinkDatabase; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:142`.
- Existing tests: No reviewed association.

## changing-dbcc.shrinkfile

ChangingDbcc contains ShrinkFile; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:143`.
- Existing tests: No reviewed association.

## changing-dbcc.thawio

ChangingDbcc contains ThawIO; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:163`.
- Existing tests: No reviewed association.

## changing-dbcc.traceoff

ChangingDbcc contains TraceOff; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:150`.
- Existing tests: No reviewed association.

## changing-dbcc.traceon

ChangingDbcc contains TraceOn; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:149`.
- Existing tests: No reviewed association.

## changing-dbcc.unpintable

ChangingDbcc contains UnpinTable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:158`.
- Existing tests: No reviewed association.

## changing-dbcc.updateusage

ChangingDbcc contains UpdateUsage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:148`.
- Existing tests: No reviewed association.

## changing-dbcc.writepage

ChangingDbcc contains WritePage; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:151`.
- Existing tests: No reviewed association.

## dbcc.checkident

CHECKIDENT is green only for second IdentifierLiteral NORESEED; otherwise may reseed.

- Outcomes: ReadOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:190`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:230 (ReadOnlyDbcc_IsGreen); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:245 (ChangingDbcc_IsRed)

## dbcc.clear

SQLPERF CLEAR overrides its otherwise safe table entry.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:198`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:245 (ChangingDbcc_IsRed)

## dbcc.dll

DBCC DLL form unloads an extended procedure DLL.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:178`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:55 (UnknownDbccNameIsClassifiedAsDllUnload)

## dbcc.fallback

An unlisted DBCC command is Unknown.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:253 (OtherDbcc_StaysYellow)

## dbcc.repair

Any literal REPAIR_ argument after first parameter overrides safe DBCC classification.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:184`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:245 (ChangingDbcc_IsRed)

## decision.classifier.01592b8ed2a0

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:184`.
- Existing tests: No reviewed association.
- Decision: `if (arguments.Any(argument => argument.StartsWith("REPAIR_", StringComparison.OrdinalIgnoreCase)))`.

## decision.classifier.02b84ffe2658

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:302`.
- Existing tests: No reviewed association.
- Decision: `? type.Name.Substring(0, type.Name.Length - "Statement".Length)`.

## decision.classifier.038e2fd72df0

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:198`.
- Existing tests: No reviewed association.
- Decision: `if (dbcc.Command == DbccCommand.SqlPerf && arguments.Any(argument => string.Equals(argument, "CLEAR", StringComparison.OrdinalIgnoreCase)))`.

## decision.classifier.11f320c3559b

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:276`.
- Existing tests: No reviewed association.
- Decision: `if (allowListApplies && name?.BaseIdentifier != null &&`.

## decision.classifier.1e9508faeeee

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:296`.
- Existing tests: No reviewed association.
- Decision: `if (TypeOrBaseNamed(type, n => n == "SetOnOffStatement" || n == "SetCommandStatement") != null)`.

## decision.classifier.1eaba5e4d065

In TypeOrBaseNamed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:315`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.classifier.220b2fe2d409

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:203`.
- Existing tests: No reviewed association.
- Decision: `if (ReadOnlyDbcc.Contains(dbcc.Command))`.

## decision.classifier.22d176288295

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:177`.
- Existing tests: No reviewed association.
- Decision: `.Where(literal => literal != null).Select(literal => literal.Value).ToList();`.

## decision.classifier.26695b9cd6c1

In TypeOrBaseNamed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:312`.
- Existing tests: No reviewed association.
- Decision: `return t.Name;`.

## decision.classifier.380178cd83e3

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:274`.
- Existing tests: No reviewed association.
- Decision: `if (IsTemp(name, options))`.

## decision.classifier.429be7dd060e

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:178`.
- Existing tests: No reviewed association.
- Decision: `if (dbcc.DllName != null)`.

## decision.classifier.51f91896aea1

In IsTempName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:259`.
- Existing tests: No reviewed association.
- Decision: `public static bool IsTempName(string name) => !string.IsNullOrEmpty(name) && name[0] == '#';`.

## decision.classifier.53bb5481a177

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:280`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.RealWrite;`.

## decision.classifier.56efabd02330

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:181`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.RealWrite;`.

## decision.classifier.56efabd02330.2

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:187`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.RealWrite;`.

## decision.classifier.56efabd02330.3

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:201`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.RealWrite;`.

## decision.classifier.56efabd02330.4

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:210`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.RealWrite;`.

## decision.classifier.669abc033dd3

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:304`.
- Existing tests: No reviewed association.
- Decision: `return Regex.Replace(typeName, "(?<=[a-z0-9])(?=[A-Z])", " ").ToUpperInvariant();`.

## decision.classifier.681cfc101e9e

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:266`.
- Existing tests: No reviewed association.
- Decision: `return true;`.

## decision.classifier.6e988fc71b82

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:190`.
- Existing tests: No reviewed association.
- Decision: `if (dbcc.Command == DbccCommand.CheckIdent)`.

## decision.classifier.74a82d833e1c

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:279`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.AllowListed;`.

## decision.classifier.7e170c5df5f4

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:267`.
- Existing tests: No reviewed association.
- Decision: `return options.TempdbIsTemp && name.ServerIdentifier == null &&`.

## decision.classifier.7f06cbc77e29

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:295`.
- Existing tests: No reviewed association.
- Decision: `return name;`.

## decision.classifier.881673f8ae50

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:195`.
- Existing tests: No reviewed association.
- Decision: `return noReseed ? Verdict.ReadOnly : Verdict.RealWrite;`.

## decision.classifier.8d3630ed23f0

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:263`.
- Existing tests: No reviewed association.
- Decision: `if (name?.BaseIdentifier == null)`.

## decision.classifier.9533d13ee736

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:194`.
- Existing tests: No reviewed association.
- Decision: `note = noReseed ? "read-only DBCC command" : "may reseed the identity value";`.

## decision.classifier.99ba6208ad88

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:206`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.ReadOnly;`.

## decision.classifier.9bccdb68ebec

In Classifier, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:70`.
- Existing tests: No reviewed association.
- Decision: `["sp_unbindefault"] = "unbinds a default",`.

## decision.classifier.a42bcba9727f

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:294`.
- Existing tests: No reviewed association.
- Decision: `if (ActionNames.TryGetValue(type.Name, out var name))`.

## decision.classifier.aedd0c938811

In UnknownStatementNote, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:288`.
- Existing tests: No reviewed association.
- Decision: `return typeName == null ? null : UnknownStatementTypes[typeName];`.

## decision.classifier.b0b577d6017e

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:299`.
- Existing tests: No reviewed association.
- Decision: `return "ALTER TABLE";`.

## decision.classifier.b5a413ab98b0

In TypeOrBaseNamed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:311`.
- Existing tests: No reviewed association.
- Decision: `if (predicate(t.Name))`.

## decision.classifier.bf0b7c727b30

In Classifier, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:46`.
- Existing tests: No reviewed association.
- Decision: `["sp_defaultdb"] = "changes a login's default database",`.

## decision.classifier.c63eba4e9edc

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:213`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.Unknown;`.

## decision.classifier.caea0120be4f

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:265`.
- Existing tests: No reviewed association.
- Decision: `if (name.ServerIdentifier == null && IsTempName(name.BaseIdentifier.Value))`.

## decision.classifier.cfc01c23883c

In Classifier, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:69`.
- Existing tests: No reviewed association.
- Decision: `["sp_bindefault"] = "binds a default",`.

## decision.classifier.d2edc3c58d7f

In IsReadOnlyStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:283`.
- Existing tests: No reviewed association.
- Decision: `public static bool IsReadOnlyStatement(TSqlStatement statement) => TypeOrBaseNamed(statement.GetType(), ReadOnlyStatementTypes.Contains) != null;`.

## decision.classifier.d3842d6b0ae7

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:298`.
- Existing tests: No reviewed association.
- Decision: `if (TypeOrBaseNamed(type, n => n == "AlterTableStatement") != null)`.

## decision.classifier.d7b26961ed79

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:264`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.classifier.df0dd1647ecf

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:176`.
- Existing tests: No reviewed association.
- Decision: `var arguments = dbcc.Literals.Skip(1).Select(literal => literal.Value as Literal)`.

## decision.classifier.dfd60ae13930

In IsTemp, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:268`.
- Existing tests: No reviewed association.
- Decision: `string.Equals(name.DatabaseIdentifier?.Value, "tempdb", StringComparison.OrdinalIgnoreCase);`.

## decision.classifier.e18cd79a178f

In ActionName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:297`.
- Existing tests: No reviewed association.
- Decision: `return "SET";`.

## decision.classifier.f15c778de2e5

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:277`.
- Existing tests: No reviewed association.
- Decision: `options.AllowList.Matches(name.ServerIdentifier?.Value, name.DatabaseIdentifier?.Value, name.SchemaIdentifier?.Value, name.BaseIdentifier.Value,`.

## decision.classifier.f90172dc7f04

In ClassifyDbcc, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:209`.
- Existing tests: No reviewed association.
- Decision: `if (ChangingDbcc.TryGetValue(dbcc.Command, out note))`.

## decision.classifier.f95291e1240a

In ClassifyWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:275`.
- Existing tests: No reviewed association.
- Decision: `return Verdict.TempOnly;`.

## temp.name

A # prefix includes both local and global temporary names; no server qualifier is permitted by IsTemp.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:259`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180 (TableDdlAndTruncate)

## temp.tempdb-opt-in

TempdbIsTemp opt-in treats a tempdb-qualified permanent target as temporary without a remote server.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:261`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:194 (PermanentTempdbObjectsRequireExplicitOptIn)

## write.allow-list

Write precedence: temporary target, then eligible allow-list match, otherwise permanent write. DDL disables allow-list.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:272`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:251 (AllowList_DoesNotApplyToDdl); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:257 (AllowList_AppliesToTruncateAndSelectInto)

## decision.sqlsafetyanalyzer.4218633f7602

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:26`.
- Existing tests: No reviewed association.
- Decision: `var worker = new Thread(() =>`.

## decision.sqlsafetyanalyzer.439066d60f75

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:18`.
- Existing tests: No reviewed association.
- Decision: `public static AnalysisResult Analyze(string sql, AnalyzerOptions options = null, CancellationToken cancellationToken = default)`.

## decision.sqlsafetyanalyzer.46782bf7eabc

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:57`.
- Existing tests: No reviewed association.
- Decision: `if (!SqlCmdPreprocessor.LooksLikeSqlCmd(sql))`.

## decision.sqlsafetyanalyzer.49663d58a61f

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:83`.
- Existing tests: No reviewed association.
- Decision: `findings.Add(new Finding(1, 1, 0, 0, "NOT ANALYZED", string.Empty, null, Verdict.Unknown, exception.Message));`.

## decision.sqlsafetyanalyzer.4f3f9c0adcbb

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:81`.
- Existing tests: No reviewed association.
- Decision: `catch (AnalysisLimitException exception)`.

## decision.sqlsafetyanalyzer.6f5462ce43cc

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:52`.
- Existing tests: No reviewed association.
- Decision: `if (!string.IsNullOrWhiteSpace(sql))`.

## decision.sqlsafetyanalyzer.77a1bb8e062d

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:62`.
- Existing tests: No reviewed association.
- Decision: `else if (options.SqlCmdMode != false)`.

## decision.sqlsafetyanalyzer.78641c391790

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:38`.
- Existing tests: No reviewed association.
- Decision: `return result;`.

## decision.sqlsafetyanalyzer.88d6f3decb5b

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:22`.
- Existing tests: No reviewed association.
- Decision: `return new AnalysisResult(new[] { new Finding(1, 1, 0, 0, "NOT ANALYZED", string.Empty, null, Verdict.Unknown,`.

## decision.sqlsafetyanalyzer.aefdb7739a1c

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:48`.
- Existing tests: No reviewed association.
- Decision: `findings.Add(new Finding(1, 1, 0, 0, "SETTINGS WARNING", string.Empty, null, Verdict.Unknown, warning));`.

## decision.sqlsafetyanalyzer.b152b922c28b

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:88`.
- Existing tests: No reviewed association.
- Decision: `return new AnalysisResult(ordered, errors.OrderBy(e => e.Offset).ToList(), skipped.OrderBy(s => s.StartOffset).ToList());`.

## decision.sqlsafetyanalyzer.c6873cf3a5b8

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:87`.
- Existing tests: No reviewed association.
- Decision: `var ordered = findings.OrderBy(f => f.StartOffset).ToList();`.

## decision.sqlsafetyanalyzer.d99005c18a45

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:33`.
- Existing tests: No reviewed association.
- Decision: `catch (Exception exception) { failure = exception; }`.

## decision.sqlsafetyanalyzer.dee779564254

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:70`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.sqlsafetyanalyzer.e0d417378d7c

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:21`.
- Existing tests: No reviewed association.
- Decision: `if (sql != null && sql.Length > MaxTextLength)`.

## decision.sqlsafetyanalyzer.e80d782ca174

In SqlSafetyAnalyzer, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:16`.
- Existing tests: No reviewed association.
- Decision: `public static string ParserName => ParserFactory.ParserName;`.

## decision.sqlsafetyanalyzer.e9bc2d39dad0

In AnalyzeCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:43`.
- Existing tests: No reviewed association.
- Decision: `options = options ?? AnalyzerOptions.Default;`.

## decision.sqlsafetyanalyzer.ef39b4369094

In Analyze, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:37`.
- Existing tests: No reviewed association.
- Decision: `if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();`.

## entry.empty

Empty or whitespace SQL produces no actions when settings are valid.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:52`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:18 (EmptyText_IsReadOnlyWithNoFindings)

## entry.length

Text over 5 Mi UTF-16 characters is not analyzed; return a warning rather than green.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:166 (OversizedScriptsFollowExplicitModePolicy)

## entry.limit

Work-limit failure adds Unknown without discarding earlier findings.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:81`.
- Existing tests: No reviewed association.

## entry.settings

Invalid allow list, procedure trust or configuration warnings contribute Unknown even on empty SQL.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs:47`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32 (BadRulesAreReportedAndIgnored)

## decision.environmentrules.008de0d9f78a

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:80`.
- Existing tests: No reviewed association.
- Decision: `_rules.Add(new Rule`.

## decision.environmentrules.017706c8b699

In Resolve, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:97`.
- Existing tests: No reviewed association.
- Decision: `return rule.Settings.Add(global);`.

## decision.environmentrules.085d9114192b

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:85`.
- Existing tests: No reviewed association.
- Decision: `$"line {index + 1}: {fields[0]} | {(fields[1].Length == 0 ? "(any database)" : fields[1])}"),`.

## decision.environmentrules.22f86d786aa2

In Report, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:120`.
- Existing tests: No reviewed association.
- Decision: `try { reportFailure?.Invoke(exception); }`.

## decision.environmentrules.2ad900409536

In ShouldCancel, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:113`.
- Existing tests: No reviewed association.
- Decision: `if (!warn) return false;`.

## decision.environmentrules.3633c5a224fb

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:73`.
- Existing tests: No reviewed association.
- Decision: `var allowList = new AllowList(fields.Length > 5 ? AllowList.SplitLines(fields[5]) : null);`.

## decision.environmentrules.44cd3ad50e64

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:64`.
- Existing tests: No reviewed association.
- Decision: `if (line.Length == 0 || line.StartsWith("--", StringComparison.Ordinal)) continue;`.

## decision.environmentrules.44d4648112fe

In EnvironmentRules, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:25`.
- Existing tests: No reviewed association.
- Decision: `TrustedProcedures = (trustedProcedures ?? Enumerable.Empty<string>()).ToArray();`.

## decision.environmentrules.4e235731df98

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:41`.
- Existing tests: No reviewed association.
- Decision: `public GuardSettings Add(GuardSettings global) => new GuardSettings(Mode, Environment, RequireTypedConfirmation,`.

## decision.environmentrules.4e62191aa7a8

In Resolve, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:98`.
- Existing tests: No reviewed association.
- Decision: `return global;`.

## decision.environmentrules.5a35bdd3a558

In ShouldCancel, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:115`.
- Existing tests: No reviewed association.
- Decision: `catch (Exception exception) { Report(reportFailure, exception); return true; }`.

## decision.environmentrules.5bf213163c42

In ShouldCancel, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:114`.
- Existing tests: No reviewed association.
- Decision: `try { return !confirm(); }`.

## decision.environmentrules.694779980d2e

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:70`.
- Existing tests: No reviewed association.
- Decision: `warnings.Add($"Environment rule line {index + 1} ignored: expected server | database | label | Off/Red/YellowRed | true/false | allow-list | trusted procedures");`.

## decision.environmentrules.71f7f6e97d25

In EnvironmentRules, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:24`.
- Existing tests: No reviewed association.
- Decision: `AllowList = (allowList ?? Enumerable.Empty<string>()).ToArray();`.

## decision.environmentrules.78f4d199832e

In ShouldCancel, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:112`.
- Existing tests: No reviewed association.
- Decision: `catch (Exception exception) { Report(reportFailure, exception); }`.

## decision.environmentrules.822291981b6f

In NeedsTypedConfirmation, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:39`.
- Existing tests: No reviewed association.
- Decision: `public bool NeedsTypedConfirmation(AnalysisResult result) => false;`.

## decision.environmentrules.925d70cf8e9f

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:83`.
- Existing tests: No reviewed association.
- Decision: `Database = fields[1].Length == 0 ? null : AllowList.ToRegex(fields[1]),`.

## decision.environmentrules.a04ef54aaf64

In CancelAfterFailure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:105`.
- Existing tests: No reviewed association.
- Decision: `result != null && (settings?.ShouldWarn(result) ?? !result.IsGreen);`.

## decision.environmentrules.a245751c987c

In Resolve, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:96`.
- Existing tests: No reviewed association.
- Decision: `if (rule.Server.IsMatch(server) && (rule.Database == null || rule.Database.IsMatch(database)))`.

## decision.environmentrules.a663cdbf7147

In ShouldWarn, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:36`.
- Existing tests: No reviewed association.
- Decision: `public bool ShouldWarn(AnalysisResult result) => Mode != ExecutionMode.Off &&`.

## decision.environmentrules.a7c07cdd69e5

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:75`.
- Existing tests: No reviewed association.
- Decision: `if (allowList.Warnings.Count > 0 || trusted.Warnings.Count > 0)`.

## decision.environmentrules.af45223d9e81

In Report, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:121`.
- Existing tests: No reviewed association.
- Decision: `catch { }`.

## decision.environmentrules.b1cda85c26f6

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:66`.
- Existing tests: No reviewed association.
- Decision: `if (fields.Length < 5 || fields.Length > 7 || fields[0].Length == 0 || fields[2].Length == 0 ||`.

## decision.environmentrules.bfb87f571b65

In CancelAfterFailure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:104`.
- Existing tests: No reviewed association.
- Decision: `public static bool CancelAfterFailure(GuardSettings settings, AnalysisResult result) =>`.

## decision.environmentrules.c349b9a924b6

In Resolve, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:93`.
- Existing tests: No reviewed association.
- Decision: `if (global == null) throw new ArgumentNullException(nameof(global));`.

## decision.environmentrules.cb6a33aa2a6a

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:60`.
- Existing tests: No reviewed association.
- Decision: `var lines = (text ?? string.Empty).Replace("\r", string.Empty).Split('\n');`.

## decision.environmentrules.d0d07ce961ed

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:77`.
- Existing tests: No reviewed association.
- Decision: `warnings.Add($"Environment rule line {index + 1} ignored: " + string.Join("; ", allowList.Warnings.Concat(trusted.Warnings)));`.

## decision.environmentrules.d6675a1c60e3

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:74`.
- Existing tests: No reviewed association.
- Decision: `var trusted = new AllowList(fields.Length > 6 ? AllowList.SplitLines(fields[6]) : null, procedures: true);`.

## decision.environmentrules.d9cfd2a2aca9

In ShouldWarn, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:37`.
- Existing tests: No reviewed association.
- Decision: `(Mode == ExecutionMode.Red ? result.Overall == Verdict.RealWrite : !result.IsGreen);`.

## decision.environmentrules.dbdcc1e83edc

In Resolve, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:94`.
- Existing tests: No reviewed association.
- Decision: `if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database)) return global;`.

## decision.environmentrules.ebf529aa43fc

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:65`.
- Existing tests: No reviewed association.
- Decision: `var fields = line.Split('|').Select(field => field.Trim()).ToArray();`.

## environment.confirm-failure

Confirmation exception cancels execution; negative confirmation also cancels.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:114`.
- Existing tests: tests/SqlSafetyGuard.Tests/GuardExecutionContractTests.cs:57 (ConfirmationFailureAlwaysPreventsExecution)

## environment.invalid-line

Ignore malformed environment lines with visible settings warnings; do not partially apply them.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:66`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32 (BadRulesAreReportedAndIgnored)

## environment.invalid-list

An invalid allow/trusted list rejects the whole environment rule.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:75`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:32 (BadRulesAreReportedAndIgnored)

## environment.off

Off mode never requests warning confirmation; this does not change analyzer Overall.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:36`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113 (ModesControlWarnings)

## environment.presentation-failure

Presentation exception is reported but cannot suppress an already-required confirmation.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:111`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:139 (PresentationOrLoggingFailuresCannotAuthorizeWarning)

## environment.red

Red mode asks only for RealWrite; Unknown can execute without confirmation in this explicit policy.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:37`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113 (ModesControlWarnings)

## environment.selection

First matching environment wins; missing context uses global settings; per-rule lists add without leaking.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:91`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:16 (FirstMatchingRuleWinsAndUnknownContextUsesGlobal); tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:65 (PerRuleListsAddToGlobalWithoutLeakingToOtherEnvironments)

## environment.typed-disabled

Typed database-name confirmation is always false, including legacy true configuration.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:39`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:123 (LegacyTypedSettingNeverRequiresTyping); tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:129 (LegacyRuleFieldIsAcceptedAndIgnored)

## environment.unknown-context

Missing server or database bypasses environment matching and returns global policy.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:94`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:16 (FirstMatchingRuleWinsAndUnknownContextUsesGlobal)

## environment.yellow-red

YellowRed mode asks for every nongreen result.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs:37`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:113 (ModesControlWarnings)

## green-statement.begintransactionstatement

ReadOnlyStatementTypes contains BeginTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.breakstatement

ReadOnlyStatementTypes contains BreakStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.closecursorstatement

ReadOnlyStatementTypes contains CloseCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.closemasterkeystatement

ReadOnlyStatementTypes contains CloseMasterKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:120`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.closesymmetrickeystatement

ReadOnlyStatementTypes contains CloseSymmetricKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:119`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.committransactionstatement

ReadOnlyStatementTypes contains CommitTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.continuestatement

ReadOnlyStatementTypes contains ContinueStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.cursorstatement

ReadOnlyStatementTypes contains CursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: No reviewed association.

## green-statement.deallocatecursorstatement

ReadOnlyStatementTypes contains DeallocateCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.declarecursorstatement

ReadOnlyStatementTypes contains DeclareCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.declaretablevariablestatement

ReadOnlyStatementTypes contains DeclareTableVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.declarevariablestatement

ReadOnlyStatementTypes contains DeclareVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.executeasstatement

ReadOnlyStatementTypes contains ExecuteAsStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.fetchcursorstatement

ReadOnlyStatementTypes contains FetchCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.getconversationgroupstatement

ReadOnlyStatementTypes contains GetConversationGroupStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:120`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.gotostatement

ReadOnlyStatementTypes contains GoToStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.labelstatement

ReadOnlyStatementTypes contains LabelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.linenostatement

ReadOnlyStatementTypes contains LineNoStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:120`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.opencursorstatement

ReadOnlyStatementTypes contains OpenCursorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:111`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.openmasterkeystatement

ReadOnlyStatementTypes contains OpenMasterKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:119`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.opensymmetrickeystatement

ReadOnlyStatementTypes contains OpenSymmetricKeyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:119`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.printstatement

ReadOnlyStatementTypes contains PrintStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.raiseerrorlegacystatement

ReadOnlyStatementTypes contains RaiseErrorLegacyStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: No reviewed association.

## green-statement.raiseerrorstatement

ReadOnlyStatementTypes contains RaiseErrorStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.readtextstatement

ReadOnlyStatementTypes contains ReadTextStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:119`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.returnstatement

ReadOnlyStatementTypes contains ReturnStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:115`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.revertstatement

ReadOnlyStatementTypes contains RevertStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.rollbacktransactionstatement

ReadOnlyStatementTypes contains RollbackTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.savetransactionstatement

ReadOnlyStatementTypes contains SaveTransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:113`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.setcommandstatement

ReadOnlyStatementTypes contains SetCommandStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.seterrorlevelstatement

ReadOnlyStatementTypes contains SetErrorLevelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:109`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.setidentityinsertstatement

ReadOnlyStatementTypes contains SetIdentityInsertStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:109`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.setonoffstatement

ReadOnlyStatementTypes contains SetOnOffStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## green-statement.setrowcountstatement

ReadOnlyStatementTypes contains SetRowCountStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:109`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.settextsizestatement

ReadOnlyStatementTypes contains SetTextSizeStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:109`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.settransactionisolationlevelstatement

ReadOnlyStatementTypes contains SetTransactionIsolationLevelStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.setuserstatement

ReadOnlyStatementTypes contains SetUserStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:110`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.setvariablestatement

ReadOnlyStatementTypes contains SetVariableStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.throwstatement

ReadOnlyStatementTypes contains ThrowStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.transactionstatement

ReadOnlyStatementTypes contains TransactionStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:112`.
- Existing tests: No reviewed association.

## green-statement.usefederationstatement

ReadOnlyStatementTypes contains UseFederationStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:120`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.usestatement

ReadOnlyStatementTypes contains UseStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:114`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## green-statement.waitforstatement

ReadOnlyStatementTypes contains WaitForStatement; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:116`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## decision.sideeffectscanner.0c492af02e9d

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:77`.
- Existing tests: No reviewed association.
- Decision: `case MergeStatement merge:`.

## decision.sideeffectscanner.1c590bb5dbef

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:60`.
- Existing tests: No reviewed association.
- Decision: `written.Add(output.IntoTable);`.

## decision.sideeffectscanner.1db06c927b5b

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:59`.
- Existing tests: No reviewed association.
- Decision: `if (output?.IntoTable != null)`.

## decision.sideeffectscanner.2cf28fd70c44

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:73`.
- Existing tests: No reviewed association.
- Decision: `case DeleteStatement delete:`.

## decision.sideeffectscanner.363de174d4f9

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:99`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(InternalOpenRowset node) =>`.

## decision.sideeffectscanner.3c9caa8db8dd

In Scan, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:32`.
- Existing tests: No reviewed association.
- Decision: `fragment?.Accept(collector);`.

## decision.sideeffectscanner.51e12c22aa31

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:97`.
- Existing tests: No reviewed association.
- Decision: `RemoteSources.Add(new RemoteSource(node, "OPENROWSET", "runs a query through a remote provider; it may change data there"));`.

## decision.sideeffectscanner.63c5547b40eb

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:83`.
- Existing tests: No reviewed association.
- Decision: `return written;`.

## decision.sideeffectscanner.721cdd17ee08

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:107`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(NextValueForExpression node) => Sequences.Add(node);`.

## decision.sideeffectscanner.777ba3e271ed

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:47`.
- Existing tests: No reviewed association.
- Decision: `if (target == null)`.

## decision.sideeffectscanner.7d901d76f675

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:48`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.sideeffectscanner.9d6d47a143ff

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:94`.
- Existing tests: No reviewed association.
- Decision: `$"runs a pass-through query on linked server {node.LinkedServer?.Value}; it may change data there"));`.

## decision.sideeffectscanner.ab9363be5797

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:53`.
- Existing tests: No reviewed association.
- Decision: `written.Add(baseTarget.Fragment);`.

## decision.sideeffectscanner.b9d3f31f1e97

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:103`.
- Existing tests: No reviewed association.
- Decision: `RemoteSources.Add(new RemoteSource(node, "OPENDATASOURCE", "opens an ad hoc remote connection; the remote side is not analyzed"));`.

## decision.sideeffectscanner.bb17f32e542e

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:100`.
- Existing tests: No reviewed association.
- Decision: `RemoteSources.Add(new RemoteSource(node, "OPENROWSET", "runs through a remote provider; the remote side is not analyzed"));`.

## decision.sideeffectscanner.ca6e1d304556

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:69`.
- Existing tests: No reviewed association.
- Decision: `case UpdateStatement update:`.

## decision.sideeffectscanner.cb8c9e97a710

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:52`.
- Existing tests: No reviewed association.
- Decision: `if (baseTarget.Fragment != null)`.

## decision.sideeffectscanner.d5f24ffea0c3

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:102`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(AdHocTableReference node) =>`.

## decision.sideeffectscanner.da7187e02fed

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:65`.
- Existing tests: No reviewed association.
- Decision: `case InsertStatement insert:`.

## decision.sideeffectscanner.ed7bc3e50ca9

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:93`.
- Existing tests: No reviewed association.
- Decision: `RemoteSources.Add(new RemoteSource(node, "OPENQUERY",`.

## decision.sideeffectscanner.f1269b3c9d53

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:96`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(OpenRowsetTableReference node) =>`.

## decision.sideeffectscanner.f2a50bc9adad

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:92`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(OpenQueryTableReference node) =>`.

## decision.sideeffectscanner.ff8745f0c06a

In WrittenFragments, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:49`.
- Existing tests: No reviewed association.
- Decision: `written.Add(target);`.

## hidden.adhoc

OPENDATASOURCE source warns about opaque remote effects.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:102`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30 (RemoteSources_AreUnknown)

## hidden.bulk-file

OPENROWSET BULK file reading is intentionally not a remote side effect.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:105`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:57 (OpenRowsetBulk_ReadsAFile_IsReadOnly)

## hidden.deduplicate

Resolve direct/alias/CTE/output write fragments so remote writes are not double-counted as opaque read sources.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:41`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:49 (RemoteWriteTarget_IsReportedOnceAsRealWrite)

## hidden.internal-openrowset

Internal OPENROWSET source warns; remote effects not inspected.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:99`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30 (RemoteSources_AreUnknown)

## hidden.openquery

OPENQUERY source can execute remote pass-through text and warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:92`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:11 (OpenQuerySource_IsUnknown)

## hidden.openrowset

OPENROWSET provider query source warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:96`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30 (RemoteSources_AreUnknown)

## hidden.sequence

NEXT VALUE FOR adds permanent sequence-advance finding independently of main statement target.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs:107`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:96 (NextValueFor_AnywhereThatRuns_IsRealWrite)

## decision.analysiswork.006cc59f5e53

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:70`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.Go) { conditions = 0; elseConditions = 0; blocks = 0; controlFlowKeywords = 0; }`.

## decision.analysiswork.0152256303e3

In ReadToEnd, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:133`.
- Existing tests: No reviewed association.
- Decision: `public int Count { get { Check(); return _tokens.Count; } }`.

## decision.analysiswork.0178a2449593

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:112`.
- Existing tests: No reviewed association.
- Decision: `if (errors != null) combined.AddRange(errors);`.

## decision.analysiswork.026a573235cd

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:74`.
- Existing tests: No reviewed association.
- Decision: `throw new AnalysisLimitException("too complex to analyze: nesting/control-flow budget exceeded");`.

## decision.analysiswork.10dc4b7dc558

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:85`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Select:`.

## decision.analysiswork.14de93724b67

In Read, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:124`.
- Existing tests: No reviewed association.
- Decision: `public override int Read(char[] buffer, int index, int count) { Check(); return base.Read(buffer, index, count); }`.

## decision.analysiswork.15107026391e

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:56`.
- Existing tests: No reviewed association.
- Decision: `if (previous == TSqlTokenType.Begin && (token.TokenType == TSqlTokenType.Transaction || token.TokenType == TSqlTokenType.Tran ||`.

## decision.analysiswork.16f0a326f81b

In Remove, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:143`.
- Existing tests: No reviewed association.
- Decision: `public bool Remove(TSqlParserToken item) => throw new NotSupportedException();`.

## decision.analysiswork.1fe12b791c07

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:96`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Execute:`.

## decision.analysiswork.22b27225c4b4

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:101`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.analysiswork.23cb5875ae61

In ReadToEnd, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:125`.
- Existing tests: No reviewed association.
- Decision: `public override string ReadToEnd() { Check(); return base.ReadToEnd(); }`.

## decision.analysiswork.269cee6ff2af

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:67`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.While) conditions++;`.

## decision.analysiswork.29b296a58819

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:140`.
- Existing tests: No reviewed association.
- Decision: `public void Add(TSqlParserToken item) => throw new NotSupportedException();`.

## decision.analysiswork.3bc176ed294b

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:89`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Merge:`.

## decision.analysiswork.3fb47a0382da

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:97`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Return:`.

## decision.analysiswork.4026bfb09ed7

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:68`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.If || token.TokenType == TSqlTokenType.While || token.TokenType == TSqlTokenType.Else)`.

## decision.analysiswork.403337430d37

In IndexOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:135`.
- Existing tests: No reviewed association.
- Decision: `public int IndexOf(TSqlParserToken item) => _tokens.IndexOf(item);`.

## decision.analysiswork.4a63d9628040

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:54`.
- Existing tests: No reviewed association.
- Decision: `if (parentheses == 0 && StartsAction(token.TokenType))`.

## decision.analysiswork.4d20554641a2

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:100`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.analysiswork.51592837c148

In Contains, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:136`.
- Existing tests: No reviewed association.
- Decision: `public bool Contains(TSqlParserToken item) => _tokens.Contains(item);`.

## decision.analysiswork.56d459c564f8

In Check, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:32`.
- Existing tests: No reviewed association.
- Decision: `if (current == null) return;`.

## decision.analysiswork.5a4d5b137b35

In Check, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:35`.
- Existing tests: No reviewed association.
- Decision: `if (current._remaining < 0 || current._elapsed.Elapsed > TimeSpan.FromSeconds(10))`.

## decision.analysiswork.622d20d69c75

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:94`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Set:`.

## decision.analysiswork.66a225846b34

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:99`.
- Existing tests: No reviewed association.
- Decision: `return true;`.

## decision.analysiswork.6dd64bba8e06

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:91`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Alter:`.

## decision.analysiswork.7217e5d637a0

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:72`.
- Existing tests: No reviewed association.
- Decision: `throw new AnalysisLimitException("too complex to analyze: control-flow keyword budget exceeded");`.

## decision.analysiswork.727581783960

In GetEnumerator, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:139`.
- Existing tests: No reviewed association.
- Decision: `IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();`.

## decision.analysiswork.783c13c3dc95

In Check, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:36`.
- Existing tests: No reviewed association.
- Decision: `throw new AnalysisLimitException("too complex to analyze: work/time budget exceeded");`.

## decision.analysiswork.7ac4a19ee278

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:98`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Truncate:`.

## decision.analysiswork.7bd8a5603d55

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:73`.
- Existing tests: No reviewed association.
- Decision: `if (parentheses > 256 || blocks > 256 || conditions > 512 || elseConditions > 512)`.

## decision.analysiswork.7cb994e79946

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:62`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.If)`.

## decision.analysiswork.7f21dc4da8d8

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:117`.
- Existing tests: No reviewed association.
- Decision: `return fragment;`.

## decision.analysiswork.81bac7e29f53

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:58`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.LeftParenthesis) parentheses++;`.

## decision.analysiswork.8255daf667b2

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:71`.
- Existing tests: No reviewed association.
- Decision: `if (controlFlowKeywords > 5000)`.

## decision.analysiswork.8382695581d9

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:95`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Print:`.

## decision.analysiswork.86d902288d1b

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:86`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Insert:`.

## decision.analysiswork.94bc2165b57a

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:115`.
- Existing tests: No reviewed association.
- Decision: `errors = errors ?? new List<ParseError>();`.

## decision.analysiswork.9ab5da657e0f

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:65`.
- Existing tests: No reviewed association.
- Decision: `elseConditions = previous == TSqlTokenType.Else ? elseConditions + 1 : 0;`.

## decision.analysiswork.a066949e4685

In Insert, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:142`.
- Existing tests: No reviewed association.
- Decision: `public void Insert(int index, TSqlParserToken item) => throw new NotSupportedException();`.

## decision.analysiswork.bd06501b8ae1

In GetEnumerator, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:138`.
- Existing tests: No reviewed association.
- Decision: `public IEnumerator<TSqlParserToken> GetEnumerator() { foreach (var token in _tokens) { Check(); yield return token; } }`.

## decision.analysiswork.c2d27de5b93a

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:88`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Delete:`.

## decision.analysiswork.c371c8f8fe23

In ReadToEnd, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:132`.
- Existing tests: No reviewed association.
- Decision: `public TSqlParserToken this[int index] { get { Check(); return _tokens[index]; } set => throw new NotSupportedException(); }`.

## decision.analysiswork.c43b76ebadc2

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:77`.
- Existing tests: No reviewed association.
- Decision: `return new CheckedTokens(tokens);`.

## decision.analysiswork.cfe22035e100

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:59`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.RightParenthesis) parentheses = Math.Max(0, parentheses - 1);`.

## decision.analysiswork.d2bdc87287de

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:93`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Declare:`.

## decision.analysiswork.d4affc90e5a1

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:60`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.Begin || token.TokenType == TSqlTokenType.Case) blocks++;`.

## decision.analysiswork.d51422745295

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:109`.
- Existing tests: No reviewed association.
- Decision: `if (lexicalErrors != null && lexicalErrors.Count > 0)`.

## decision.analysiswork.d87b538d638f

In Read, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:123`.
- Existing tests: No reviewed association.
- Decision: `public override int Read() { Check(); return base.Read(); }`.

## decision.analysiswork.d9d3b0dc8a20

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:87`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Update:`.

## decision.analysiswork.e17ccb82cbf3

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:92`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Drop:`.

## decision.analysiswork.e22ed5a2cfe1

In ReadToEnd, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:134`.
- Existing tests: No reviewed association.
- Decision: `public bool IsReadOnly => true;`.

## decision.analysiswork.e5f8ea999362

In RemoveAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:144`.
- Existing tests: No reviewed association.
- Decision: `public void RemoveAt(int index) => throw new NotSupportedException();`.

## decision.analysiswork.e94d82b170b7

In Clear, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:141`.
- Existing tests: No reviewed association.
- Decision: `public void Clear() => throw new NotSupportedException();`.

## decision.analysiswork.eae14527874c

In StartsAction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:90`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Create:`.

## decision.analysiswork.ebf5619be347

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:52`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.WhiteSpace || token.TokenType == TSqlTokenType.SingleLineComment ||`.

## decision.analysiswork.ed5ea21d901b

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:61`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType == TSqlTokenType.End) blocks = Math.Max(0, blocks - 1);`.

## decision.analysiswork.f62ff4a64ba3

In CopyTo, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:137`.
- Existing tests: No reviewed association.
- Decision: `public void CopyTo(TSqlParserToken[] array, int arrayIndex) => _tokens.CopyTo(array, arrayIndex);`.

## limits.action-reset

Top-level action tokens reset condition nesting but do not reset total control-flow count.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:54`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:273 (NestingThatDefeatsStatementCountersIsRejectedBeforeParsing)

## limits.cancel

Cancellation is propagated rather than silently returning green.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:33`.
- Existing tests: No reviewed association.

## limits.control-count

More than 5000 IF/WHILE/ELSE keywords per batch rejects analysis; only GO resets count.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:71`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:293 (ControlFlowKeywordBudgetHasExactBatchBoundary); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:305 (ControlFlowKeywordBudgetResetsOnlyAtGo)

## limits.nesting-count

Reject parentheses/blocks above 256 or condition/else-condition nesting above 512.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:73`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:359 (ExcessiveNestingReturnsUnknownWithoutParsing)

## limits.syntax

Bound parentheses/blocks, conditions and control tokens before ScriptDom recursion; GO resets nesting.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:39`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:359 (ExcessiveNestingReturnsUnknownWithoutParsing)

## limits.work

Cancellation propagates; 40 million work units or ten seconds abort to conservative warning.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:29`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:423 (LongSingleLineSelectAndLargeBrokenBatchAreBounded)

## parser.lexical-errors

Combine tokenization and parser errors instead of dropping lexical diagnostics.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/AnalysisWork.cs:109`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:147 (UnrecognizedCharacter_DoesNotHideWhatFollows)

## msdb-procedure.sp_add_job

Explicit msdb.sys/msdb.dbo sp_add_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_add_jobschedule

Explicit msdb.sys/msdb.dbo sp_add_jobschedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_add_jobserver

Explicit msdb.sys/msdb.dbo sp_add_jobserver qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_add_jobstep

Explicit msdb.sys/msdb.dbo sp_add_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_add_schedule

Explicit msdb.sys/msdb.dbo sp_add_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_attach_schedule

Explicit msdb.sys/msdb.dbo sp_attach_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_delete_backuphistory

Explicit msdb.sys/msdb.dbo sp_delete_backuphistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_delete_job

Explicit msdb.sys/msdb.dbo sp_delete_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_delete_jobstep

Explicit msdb.sys/msdb.dbo sp_delete_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_help_job

Explicit msdb.sys/msdb.dbo sp_help_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_help_jobhistory

Explicit msdb.sys/msdb.dbo sp_help_jobhistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_help_jobstep

Explicit msdb.sys/msdb.dbo sp_help_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_help_schedule

Explicit msdb.sys/msdb.dbo sp_help_schedule qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_purge_jobhistory

Explicit msdb.sys/msdb.dbo sp_purge_jobhistory qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_send_dbmail

Explicit msdb.sys/msdb.dbo sp_send_dbmail qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:996`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_start_job

Explicit msdb.sys/msdb.dbo sp_start_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_stop_job

Explicit msdb.sys/msdb.dbo sp_stop_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:998`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_update_job

Explicit msdb.sys/msdb.dbo sp_update_job qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## msdb-procedure.sp_update_jobstep

Explicit msdb.sys/msdb.dbo sp_update_jobstep qualifies for system-procedure classification; remote and other schemas remain ineligible.

- Outcomes: ReadOnly, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:997`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:121 (SystemNamesOutsideSystemNamespaceAreUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:189 (KnownWriters_AreRed)

## decision.analyzeroptions.39e9ead66517

In AnalyzerOptions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:15`.
- Existing tests: No reviewed association.
- Decision: `AllowList = new AllowList(allowListPatterns ?? Enumerable.Empty<string>());`.

## decision.analyzeroptions.563b5833c82a

In FromAllowListText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:26`.
- Existing tests: No reviewed association.
- Decision: `public static AnalyzerOptions FromAllowListText(string allowListText, bool tempdbIsTemp = false, bool? sqlCmdMode = null) =>`.

## decision.analyzeroptions.7482abc3c151

In AnalyzerOptions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:22`.
- Existing tests: No reviewed association.
- Decision: `ConfigurationWarnings = (configurationWarnings ?? Enumerable.Empty<string>()).ToArray();`.

## decision.analyzeroptions.a29b0a425317

In AnalyzerOptions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:12`.
- Existing tests: No reviewed association.
- Decision: `bool? sqlCmdMode = null, string currentDatabase = null, string defaultSchema = null,`.

## decision.analyzeroptions.e1c35d66f703

In FromAllowListText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs:45`.
- Existing tests: No reviewed association.
- Decision: `public bool? SqlCmdMode { get; }`.

## decision.parserfactory.12fc54bbb619

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:47`.
- Existing tests: No reviewed association.
- Decision: `.OrderByDescending(x => x.Version)`.

## decision.parserfactory.24c5f283591a

In Create, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:23`.
- Existing tests: No reviewed association.
- Decision: `return (TSqlParser)ctor.Invoke(new object[] { true });`.

## decision.parserfactory.36bf6504b7c5

In Create, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:22`.
- Existing tests: No reviewed association.
- Decision: `if (ctor != null)`.

## decision.parserfactory.49585cbd38ca

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:44`.
- Existing tests: No reviewed association.
- Decision: `.Select(t => new { Type = t, Match = regex.Match(t.Name) })`.

## decision.parserfactory.5063bbaa16dd

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:59`.
- Existing tests: No reviewed association.
- Decision: `return Tuple.Create<ConstructorInfo, string>(null, nameof(TSql160Parser));`.

## decision.parserfactory.530865613c1f

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:48`.
- Existing tests: No reviewed association.
- Decision: `.Select(x => new { x.Type, Ctor = x.Type.GetConstructor(new[] { typeof(bool) }) })`.

## decision.parserfactory.5548133f2541

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:51`.
- Existing tests: No reviewed association.
- Decision: `if (best != null)`.

## decision.parserfactory.5ac172c68655

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:54`.
- Existing tests: No reviewed association.
- Decision: `catch`.

## decision.parserfactory.5e1c971f3fc8

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:52`.
- Existing tests: No reviewed association.
- Decision: `return Tuple.Create(best.Ctor, best.Type.Name);`.

## decision.parserfactory.719f24f53da3

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:49`.
- Existing tests: No reviewed association.
- Decision: `.FirstOrDefault(x => x.Ctor != null);`.

## decision.parserfactory.72e9062608e2

In ParserFactory, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:17`.
- Existing tests: No reviewed association.
- Decision: `public static string ParserName => Best.Value.Item2;`.

## decision.parserfactory.8509fc603ff3

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:43`.
- Existing tests: No reviewed association.
- Decision: `.Where(t => !t.IsAbstract && typeof(TSqlParser).IsAssignableFrom(t))`.

## decision.parserfactory.96f2943bdc88

In Create, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:24`.
- Existing tests: No reviewed association.
- Decision: `return new TSql160Parser(true);`.

## decision.parserfactory.a8c13fadc4d8

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:45`.
- Existing tests: No reviewed association.
- Decision: `.Where(x => x.Match.Success)`.

## decision.parserfactory.b688842d9d7f

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:46`.
- Existing tests: No reviewed association.
- Decision: `.Select(x => new { x.Type, Version = int.Parse(x.Match.Groups[1].Value) })`.

## decision.parserfactory.c6ee0337e3b7

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:36`.
- Existing tests: No reviewed association.
- Decision: `catch (ReflectionTypeLoadException ex)`.

## decision.parserfactory.ed8989cb21f7

In FindBest, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:38`.
- Existing tests: No reviewed association.
- Decision: `types = ex.Types.Where(t => t != null).ToArray();`.

## parser.newest

Select newest numeric ScriptDom parser with quoted identifiers on; fallback parser if discovery cannot construct it.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/ParserFactory.cs:13`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:11 (UsesNewestParser)

## decision.textposition.1466827fa9f1

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:43`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.textposition.3a97171ecd06

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:38`.
- Existing tests: No reviewed association.
- Decision: `else if (c == '\n')`.

## decision.textposition.521d4d501f9d

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:12`.
- Existing tests: No reviewed association.
- Decision: `if (string.IsNullOrEmpty(text))`.

## decision.textposition.7399b05460d1

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:21`.
- Existing tests: No reviewed association.
- Decision: `if (c == '\r')`.

## decision.textposition.8ab25e185e28

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:15`.
- Existing tests: No reviewed association.
- Decision: `if (offset > text.Length)`.

## decision.textposition.9634a5b7b958

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:13`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.textposition.9634a5b7b958.2

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:29`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.textposition.ce6f0c247796

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:23`.
- Existing tests: No reviewed association.
- Decision: `if (i + 1 < text.Length && text[i + 1] == '\n')`.

## decision.textposition.efb422d8724d

In CountLineBreaks, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:54`.
- Existing tests: No reviewed association.
- Decision: `return line - 1;`.

## decision.textposition.f6737678b334

In FromOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TextPosition.cs:25`.
- Existing tests: No reviewed association.
- Decision: `if (i + 1 == offset)`.

## decision.finding.0daf31e9b515

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:79`.
- Existing tests: No reviewed association.
- Decision: `if (Action.StartsWith("DROP ", StringComparison.Ordinal))`.

## decision.finding.156f62107d45

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:74`.
- Existing tests: No reviewed association.
- Decision: `return string.Empty;`.

## decision.finding.1bc4373491ad

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:91`.
- Existing tests: No reviewed association.
- Decision: `public string DisplayTarget =>`.

## decision.finding.1f81f8211bb7

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:134`.
- Existing tests: No reviewed association.
- Decision: `Reason = reason ?? string.Empty;`.

## decision.finding.218caad0db9b

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:23`.
- Existing tests: No reviewed association.
- Decision: `Target = target ?? string.Empty;`.

## decision.finding.2538c135954b

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:147`.
- Existing tests: No reviewed association.
- Decision: `public string Lines => StartLine == EndLine ? "line " + StartLine : $"lines {StartLine}–{EndLine}";`.

## decision.finding.4ab3d74dd2cd

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:82`.
- Existing tests: No reviewed association.
- Decision: `return "No WHERE clause: only the CTE or derived table limits which rows of " + DisplayTarget + " change";`.

## decision.finding.4b1d4872002f

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:96`.
- Existing tests: No reviewed association.
- Decision: `public override string ToString() =>`.

## decision.finding.566a460bf0aa

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:15`.
- Existing tests: No reviewed association.
- Decision: `string resolvedTarget, Verdict verdict, string note, bool willRun = true, int? blockingErrorLine = null,`.

## decision.finding.6b0eb32082aa

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:60`.
- Existing tests: No reviewed association.
- Decision: `public int? BlockingErrorLine { get; }`.

## decision.finding.6d7382d8959f

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:122`.
- Existing tests: No reviewed association.
- Decision: `public override string ToString() => $"Line {Line}, column {Column}: {Message}";`.

## decision.finding.6e157b1634f8

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:78`.
- Existing tests: No reviewed association.
- Decision: `return "Drops " + DisplayTarget + " and all its rows";`.

## decision.finding.7e94d7df39e2

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:80`.
- Existing tests: No reviewed association.
- Decision: `return "Drops " + DisplayTarget;`.

## decision.finding.8393343ab4ea

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:97`.
- Existing tests: No reviewed association.
- Decision: `$"{Line}:{Column} {Action} {DisplayTarget} [{Verdict}]{(WillRun ? string.Empty : " (won't run now)")}{(Note.Length > 0 ? " " + Note : string.Empty)}";`.

## decision.finding.8b6243eec967

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:77`.
- Existing tests: No reviewed association.
- Decision: `if (Action == "DROP TABLE")`.

## decision.finding.901d5e214931

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:81`.
- Existing tests: No reviewed association.
- Decision: `if (Note.Contains(CteOrDerivedTableLimitNote))`.

## decision.finding.a9b3aadab87a

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:22`.
- Existing tests: No reviewed association.
- Decision: `Action = action ?? string.Empty;`.

## decision.finding.a9de8c981cb3

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:83`.
- Existing tests: No reviewed association.
- Decision: `return "No WHERE clause: can affect every row of " + DisplayTarget;`.

## decision.finding.ac227cbdf2fc

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:76`.
- Existing tests: No reviewed association.
- Decision: `return "Removes every row of " + DisplayTarget;`.

## decision.finding.c7b24ed3ae62

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:73`.
- Existing tests: No reviewed association.
- Decision: `if (!AffectsAllRows)`.

## decision.finding.d919a3fd3095

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:109`.
- Existing tests: No reviewed association.
- Decision: `Message = message ?? string.Empty;`.

## decision.finding.de676ca102a2

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:93`.
- Existing tests: No reviewed association.
- Decision: `? (Target.Length == 0 ? ResolvedTarget : Target + " → " + ResolvedTarget)`.

## decision.finding.e783927b272d

In ToString, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:149`.
- Existing tests: No reviewed association.
- Decision: `public override string ToString() => $"{Lines}: {Reason}";`.

## decision.finding.f17e140aa7c7

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:75`.
- Existing tests: No reviewed association.
- Decision: `if (Action == "TRUNCATE TABLE")`.

## decision.finding.fd7ff288b793

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:26`.
- Existing tests: No reviewed association.
- Decision: `Note = note ?? string.Empty;`.

## decision.finding.fe888401008e

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:88`.
- Existing tests: No reviewed association.
- Decision: `public bool IsDiagnostic => Action == "PARSE ERROR" || Action == "NOT ANALYZED" || Action == "SQLCMD MODE OFF";`.

## decision.finding.ff42b9f483dc

In Finding, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Finding.cs:24`.
- Existing tests: No reviewed association.
- Decision: `ResolvedTarget = string.IsNullOrEmpty(resolvedTarget) ? null : resolvedTarget;`.

## decision.batchrecovery.05c68cd42894

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:489`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Procedure:`.

## decision.batchrecovery.07ffa56f2b31

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:100`.
- Existing tests: No reviewed association.
- Decision: `if (module != null)`.

## decision.batchrecovery.0935ab872d68

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:139`.
- Existing tests: No reviewed association.
- Decision: `if (inRegion.Count == 0)`.

## decision.batchrecovery.0e284ee32d01

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:464`.
- Existing tests: No reviewed association.
- Decision: `if (startIndex < 0)`.

## decision.batchrecovery.122361eb070f

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:390`.
- Existing tests: No reviewed association.
- Decision: `if (type == TSqlTokenType.LeftParenthesis)`.

## decision.batchrecovery.13cb37ef8d6a

In AddRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:171`.
- Existing tests: No reviewed association.
- Decision: `Regions.Add(region);`.

## decision.batchrecovery.1495c4c7bd42

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:378`.
- Existing tests: No reviewed association.
- Decision: `else if (pendingCte && (type == TSqlTokenType.Select || type == TSqlTokenType.Insert || type == TSqlTokenType.Update ||`.

## decision.batchrecovery.15d894a9ee92

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:143`.
- Existing tests: No reviewed association.
- Decision: `if (!recovery.Errors.Any(e => e.Offset == error.Offset && e.Message == error.Message))`.

## decision.batchrecovery.16e745a6688f

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:309`.
- Existing tests: No reviewed association.
- Decision: `else if (type == TSqlTokenType.End)`.

## decision.batchrecovery.17aee5072eb0

In StatementAround, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:428`.
- Existing tests: No reviewed association.
- Decision: `if (tokenOffset <= offset)`.

## decision.batchrecovery.1a97672cf188

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:347`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Alter:`.

## decision.batchrecovery.1cc2f002ad07

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:488`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Proc:`.

## decision.batchrecovery.1dfecc1a8307

In StatementAround, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:437`.
- Existing tests: No reviewed association.
- Decision: `return new Region(start, end, null);`.

## decision.batchrecovery.21b73119bfa7

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:325`.
- Existing tests: No reviewed association.
- Decision: `start = !pendingInsert && !pendingCte && !IsSetOperator(previous) && !(previous?.TokenType == TSqlTokenType.For);`.

## decision.batchrecovery.241b5806c150

In Blank, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:241`.
- Existing tests: No reviewed association.
- Decision: `return result;`.

## decision.batchrecovery.25aaea731f8e

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:302`.
- Existing tests: No reviewed association.
- Decision: `if (type == TSqlTokenType.Case)`.

## decision.batchrecovery.289f3813e04e

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:305`.
- Existing tests: No reviewed association.
- Decision: `if (expectStart)`.

## decision.batchrecovery.2e5f629269dd

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:343`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Set:`.

## decision.batchrecovery.30a4f4f7363c

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:398`.
- Existing tests: No reviewed association.
- Decision: `return starts;`.

## decision.batchrecovery.3406d637b704

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:84`.
- Existing tests: No reviewed association.
- Decision: `if (iteration >= MaxIterations || stopwatch.Elapsed > TimeBudget)`.

## decision.batchrecovery.349b832c52dc

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:316`.
- Existing tests: No reviewed association.
- Decision: `else if (AlwaysStatementStart.Contains(type))`.

## decision.batchrecovery.34b39225ede7

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:465`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.34b39225ede7.2

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:469`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.34b39225ede7.3

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:478`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.34b39225ede7.4

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:484`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.34b39225ede7.5

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:502`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.3cb07161d122

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:286`.
- Existing tests: No reviewed association.
- Decision: `if (IsTrivia(token))`.

## decision.batchrecovery.3dd450770088

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:370`.
- Existing tests: No reviewed association.
- Decision: `if (consumedByCte)`.

## decision.batchrecovery.3e141f54c937

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:479`.
- Existing tests: No reviewed association.
- Decision: `words.Add("ALTER");`.

## decision.batchrecovery.3eaf8117e1ef

In Blank, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:234`.
- Existing tests: No reviewed association.
- Decision: `if (run == Filler.Length)`.

## decision.batchrecovery.402e5a9b1e86

In Blank, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:226`.
- Existing tests: No reviewed association.
- Decision: `if (result[i] != '\r' && result[i] != '\n')`.

## decision.batchrecovery.4697fc6bcbe2

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:311`.
- Existing tests: No reviewed association.
- Decision: `if (caseDepth > 0)`.

## decision.batchrecovery.4703e51099ce

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:483`.
- Existing tests: No reviewed association.
- Decision: `if (i < 0)`.

## decision.batchrecovery.47a94bf76eb9

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:362`.
- Existing tests: No reviewed association.
- Decision: `starts.Add(i);`.

## decision.batchrecovery.4b1426ff57a2

In IsCteStart, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:408`.
- Existing tests: No reviewed association.
- Decision: `return next >= 0 && (tokens[next].TokenType == TSqlTokenType.Identifier || tokens[next].TokenType == TSqlTokenType.QuotedIdentifier);`.

## decision.batchrecovery.4c1bff91811b

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:215`.
- Existing tests: No reviewed association.
- Decision: `return result;`.

## decision.batchrecovery.4d371c6463a7

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:350`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.With:`.

## decision.batchrecovery.50571ac104a4

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:330`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Delete:`.

## decision.batchrecovery.508ad2835f6b

In HasCode, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:252`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.batchrecovery.5555f924cd8b

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:498`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.View:`.

## decision.batchrecovery.55ee21034973

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:525`.
- Existing tests: No reviewed association.
- Decision: `bool misplaced = tokens.Take(startIndex).Any(t => !IsTrivia(t));`.

## decision.batchrecovery.596a8f6d5b8e

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:124`.
- Existing tests: No reviewed association.
- Decision: `if (Score(errorsB) > Score(errorsA))`.

## decision.batchrecovery.5a0833c33748

In Contains, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:51`.
- Existing tests: No reviewed association.
- Decision: `public bool Contains(int offset) => offset >= Start && offset < End;`.

## decision.batchrecovery.5b4e0b3cfd52

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:193`.
- Existing tests: No reviewed association.
- Decision: `tokens = AnalysisWork.Tokenize(parser, position == 0 ? text : text.Substring(position), out errors);`.

## decision.batchrecovery.5be854cac042

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:140`.
- Existing tests: No reviewed association.
- Decision: `inRegion.Add(first);`.

## decision.batchrecovery.5d36ed03b6a1

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:473`.
- Existing tests: No reviewed association.
- Decision: `if (first == TSqlTokenType.Create && i >= 0 && tokens[i].TokenType == TSqlTokenType.Or)`.

## decision.batchrecovery.5dcb94c642d7

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:138`.
- Existing tests: No reviewed association.
- Decision: `var inRegion = errors.Where(e => chosen.Contains(e.Offset) || (e.Offset >= current.Length && chosen.End >= current.Length)).ToList();`.

## decision.batchrecovery.5e3f6a5c5358

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:361`.
- Existing tests: No reviewed association.
- Decision: `if (!consumedByCte)`.

## decision.batchrecovery.5e96cc4f264d

In HasCode, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:248`.
- Existing tests: No reviewed association.
- Decision: `if (!char.IsWhiteSpace(buffer[i]))`.

## decision.batchrecovery.601bc399ee1e

In IsTrivia, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:255`.
- Existing tests: No reviewed association.
- Decision: `private static bool IsTrivia(TSqlParserToken token) =>`.

## decision.batchrecovery.65527e34b698

In PreviousStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:444`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.batchrecovery.6bdcbcbb8cf9

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:514`.
- Existing tests: No reviewed association.
- Decision: `if (nameStart < 0)`.

## decision.batchrecovery.6c63143724d7

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:212`.
- Existing tests: No reviewed association.
- Decision: `position += Math.Max(errors.Min(e => e.Offset), 0) + 1;`.

## decision.batchrecovery.6e1de645d3be

In NextSignificant, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:419`.
- Existing tests: No reviewed association.
- Decision: `return -1;`.

## decision.batchrecovery.72fd26ad9661

In NextSignificant, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:416`.
- Existing tests: No reviewed association.
- Decision: `return i;`.

## decision.batchrecovery.75a1468f41dd

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:501`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.batchrecovery.766d198c3897

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:505`.
- Existing tests: No reviewed association.
- Decision: `words.Add(kind);`.

## decision.batchrecovery.7ab1d8b37301

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:356`.
- Existing tests: No reviewed association.
- Decision: `if (start)`.

## decision.batchrecovery.7ac9b0f5b69e

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:200`.
- Existing tests: No reviewed association.
- Decision: `if (token.Line == 1)`.

## decision.batchrecovery.7c79a23413f0

In PreviousStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:443`.
- Existing tests: No reviewed association.
- Decision: `if (at <= 0)`.

## decision.batchrecovery.7f386e3a483a

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:134`.
- Existing tests: No reviewed association.
- Decision: `if (!HasCode(buffer, chosen.Start, chosen.End))`.

## decision.batchrecovery.861427d5d5c4

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:292`.
- Existing tests: No reviewed association.
- Decision: `if (depth == 0)`.

## decision.batchrecovery.87480c754f7d

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:333`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Merge:`.

## decision.batchrecovery.8ee842e7f1a9

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:294`.
- Existing tests: No reviewed association.
- Decision: `if (type == TSqlTokenType.Semicolon)`.

## decision.batchrecovery.901db2faac17

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:88`.
- Existing tests: No reviewed association.
- Decision: `var first = errors.OrderBy(e => e.Offset).First();`.

## decision.batchrecovery.984bdc6c1e1f

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:527`.
- Existing tests: No reviewed association.
- Decision: `return new Region(regionStart, text.Length, new ModuleDefinition(string.Join(" ", words), name, misplaced));`.

## decision.batchrecovery.9bc3ac58085e

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:336`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Execute:`.

## decision.batchrecovery.9c51acd868f8

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:524`.
- Existing tests: No reviewed association.
- Decision: `string name = nameStart >= 0 ? text.Substring(nameStart, nameEnd - nameStart) : string.Empty;`.

## decision.batchrecovery.a0f504ab6e1c

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:329`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Update:`.

## decision.batchrecovery.a17dbf0ddf70

In IsSetOperator, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:401`.
- Existing tests: No reviewed association.
- Decision: `private static bool IsSetOperator(TSqlParserToken previous) =>`.

## decision.batchrecovery.a784ed0a6428

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:206`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType != TSqlTokenType.EndOfFile || errors == null || errors.Count == 0)`.

## decision.batchrecovery.ab6e01f9e8a5

In AddRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:172`.
- Existing tests: No reviewed association.
- Decision: `Regions.Sort((x, y) => x.Start.CompareTo(y.Start));`.

## decision.batchrecovery.ab89a821c0f1

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:324`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Select:`.

## decision.batchrecovery.ad232602acb9

In Score, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:175`.
- Existing tests: No reviewed association.
- Decision: `private static int Score(IList<ParseError> errors) =>`.

## decision.batchrecovery.ae653b71777e

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:458`.
- Existing tests: No reviewed association.
- Decision: `if (tokens[index].Offset <= errorOffset)`.

## decision.batchrecovery.b470ac2101bb

In NextSignificant, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:415`.
- Existing tests: No reviewed association.
- Decision: `if (!IsTrivia(tokens[i]))`.

## decision.batchrecovery.b50369369b73

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:207`.
- Existing tests: No reviewed association.
- Decision: `result.Add(token);`.

## decision.batchrecovery.b67812914150

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:120`.
- Existing tests: No reviewed association.
- Decision: `if (b != null && errorsA.Count > 0)`.

## decision.batchrecovery.b75e23763db6

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:179`.
- Existing tests: No reviewed association.
- Decision: `=> AnalysisWork.Parse(parser, text, out errors);`.

## decision.batchrecovery.b9e770a2ce51

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:477`.
- Existing tests: No reviewed association.
- Decision: `if (i < 0 || tokens[i].TokenType != TSqlTokenType.Alter)`.

## decision.batchrecovery.bafe722e7b29

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:313`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.bafe722e7b29.2

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:320`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.c34774242d2a

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:198`.
- Existing tests: No reviewed association.
- Decision: `if (position > 0)`.

## decision.batchrecovery.c42b3830c48c

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:328`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Insert:`.

## decision.batchrecovery.c8fb5f0dd104

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:475`.
- Existing tests: No reviewed association.
- Decision: `words.Add("OR");`.

## decision.batchrecovery.c906390098ca

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:154`.
- Existing tests: No reviewed association.
- Decision: `return recovery;`.

## decision.batchrecovery.caa34ddd9e26

In HasCode, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:249`.
- Existing tests: No reviewed association.
- Decision: `return true;`.

## decision.batchrecovery.cdedec590c49

In Tokenize, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:210`.
- Existing tests: No reviewed association.
- Decision: `if (errors == null || errors.Count == 0)`.

## decision.batchrecovery.cdf6dd9e54db

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:392`.
- Existing tests: No reviewed association.
- Decision: `else if (type == TSqlTokenType.RightParenthesis && depth > 0)`.

## decision.batchrecovery.ce65f3cf45a5

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:340`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Values:`.

## decision.batchrecovery.d17ef57be981

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:512`.
- Existing tests: No reviewed association.
- Decision: `if (type == TSqlTokenType.Identifier || type == TSqlTokenType.QuotedIdentifier || type == TSqlTokenType.Dot)`.

## decision.batchrecovery.d65a8ea727db

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:77`.
- Existing tests: No reviewed association.
- Decision: `if (errors.Count > 0 && text.Length > 32768)`.

## decision.batchrecovery.d7f264950d33

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:492`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Function:`.

## decision.batchrecovery.da750fdbcbb5

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:460`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.da750fdbcbb5.2

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:518`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.db46b258a16d

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:78`.
- Existing tests: No reviewed association.
- Decision: `return recovery.Failed(errors);`.

## decision.batchrecovery.db46b258a16d.2

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:85`.
- Existing tests: No reviewed association.
- Decision: `return recovery.Failed(errors);`.

## decision.batchrecovery.db46b258a16d.3

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:135`.
- Existing tests: No reviewed association.
- Decision: `return recovery.Failed(errors);`.

## decision.batchrecovery.dd65e2c236d3

In Failed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:161`.
- Existing tests: No reviewed association.
- Decision: `if (!Errors.Any(e => e.Offset == error.Offset && e.Message == error.Message))`.

## decision.batchrecovery.dfbd5a883843

In Failed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:162`.
- Existing tests: No reviewed association.
- Decision: `Errors.Add(error);`.

## decision.batchrecovery.e7bd60b0b84b

In Score, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:176`.
- Existing tests: No reviewed association.
- Decision: `errors.Count == 0 ? int.MaxValue : errors.Min(e => e.Offset);`.

## decision.batchrecovery.eab0361e1bbf

In Failed, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:166`.
- Existing tests: No reviewed association.
- Decision: `return this;`.

## decision.batchrecovery.eed6835835d3

In PreviousStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:445`.
- Existing tests: No reviewed association.
- Decision: `return new Region(tokens[starts[at - 1]].Offset, offset, null);`.

## decision.batchrecovery.f42b98654496

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:495`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Trigger:`.

## decision.batchrecovery.f582a9817f5f

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:144`.
- Existing tests: No reviewed association.
- Decision: `recovery.Errors.Add(error);`.

## decision.batchrecovery.f99463cb2214

In Run, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:106`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.fc47d00f5071

In Blank, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:233`.
- Existing tests: No reviewed association.
- Decision: `run = result[i] == ' ' ? run + 1 : 0;`.

## decision.batchrecovery.fd822028a4b1

In StatementStarts, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:346`.
- Existing tests: No reviewed association.
- Decision: `case TSqlTokenType.Drop:`.

## decision.batchrecovery.fe535ae22196

In FindModuleRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:468`.
- Existing tests: No reviewed association.
- Decision: `if (first != TSqlTokenType.Create && first != TSqlTokenType.Alter)`.

## decision.batchrecovery.ff1661572d02

In StatementAround, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:430`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.batchrecovery.ffe0642080a5

In PreviousStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:442`.
- Existing tests: No reviewed association.
- Decision: `int at = starts.FindIndex(i => tokens[i].Offset == offset);`.

## recovery.regions

Bound retries/time/text size, preserve newline/offsets, and avoid treating malformed module bodies as executable scripts.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/BatchRecovery.cs:185`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:158 (GiveUp_WholeBatchIsNotAnalyzed_AndScanned)

## recovery-keyword.alter

Unparsed non-trivia Alter token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.bulk

Unparsed non-trivia Bulk token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.create

Unparsed non-trivia Create token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.dbcc

Unparsed non-trivia Dbcc token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.delete

Unparsed non-trivia Delete token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.deny

Unparsed non-trivia Deny token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.drop

Unparsed non-trivia Drop token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.execute

Unparsed non-trivia Execute token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.grant

Unparsed non-trivia Grant token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:22`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.insert

Unparsed non-trivia Insert token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.kill

Unparsed non-trivia Kill token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.merge

Unparsed non-trivia Merge token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.reconfigure

Unparsed non-trivia Reconfigure token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.restore

Unparsed non-trivia Restore token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.revoke

Unparsed non-trivia Revoke token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:23`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.shutdown

Unparsed non-trivia Shutdown token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:24`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.truncate

Unparsed non-trivia Truncate token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## recovery-keyword.update

Unparsed non-trivia Update token adds a possible-write Unknown finding; this is not proof the statement executes.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:21`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:114 (KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings)

## decision.targetresolver.008950b37ba5

In Deduplicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:185`.
- Existing tests: No reviewed association.
- Decision: `: target.Fragment?.StartOffset.ToString()), StringComparer.OrdinalIgnoreCase)`.

## decision.targetresolver.0202024c3497

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:86`.
- Existing tests: No reviewed association.
- Decision: `return OrUnresolved(ResolveSource(named, empty, new HashSet<string>(StringComparer.OrdinalIgnoreCase)), named, new[] { "via CTE " + id });`.

## decision.targetresolver.0401af0cd7ef

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:156`.
- Existing tests: No reviewed association.
- Decision: `var derivedVia = alias != null && !viaList.Any(v => v.EndsWith(" " + alias, StringComparison.OrdinalIgnoreCase))`.

## decision.targetresolver.06a26814f976

In OrUnresolved, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:104`.
- Existing tests: No reviewed association.
- Decision: `if (resolved.Count > 0)`.

## decision.targetresolver.09d6fb8c78fe

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:121`.
- Existing tests: No reviewed association.
- Decision: `if (id != null && _ctes.TryGetValue(id, out var cte))`.

## decision.targetresolver.0abebc8a6f0b

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:251`.
- Existing tests: No reviewed association.
- Decision: `yield return withAlias;`.

## decision.targetresolver.0f21d53d0b06

In Deduplicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:182`.
- Existing tests: No reviewed association.
- Decision: `private static List<BaseTarget> Deduplicate(IEnumerable<BaseTarget> targets) => targets`.

## decision.targetresolver.10b74d2dac65

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:96`.
- Existing tests: No reviewed association.
- Decision: `if (IsRemote(target))`.

## decision.targetresolver.16c1e13f40bc

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:226`.
- Existing tests: No reviewed association.
- Decision: `else if (byName == null && tableReference is NamedTableReference named &&`.

## decision.targetresolver.177d5251d30f

In ResolveQuery, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:199`.
- Existing tests: No reviewed association.
- Decision: `case BinaryQueryExpression binary:`.

## decision.targetresolver.186f1021c078

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:179`.
- Existing tests: No reviewed association.
- Decision: `return Deduplicate(result);`.

## decision.targetresolver.253c242478f6

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:129`.
- Existing tests: No reviewed association.
- Decision: `if (!visitingCtes.Add(id))`.

## decision.targetresolver.308d18deefdd

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:168`.
- Existing tests: No reviewed association.
- Decision: `case JoinParenthesisTableReference parenthesis:`.

## decision.targetresolver.33e89da144ce

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:98`.
- Existing tests: No reviewed association.
- Decision: `return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Unresolved, target, null, empty, "could not resolve the target") };`.

## decision.targetresolver.376b8fb2c314

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:221`.
- Existing tests: No reviewed association.
- Decision: `if (tableReference.Alias != null)`.

## decision.targetresolver.3c98a4f70c4e

In ResolveQuery, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:209`.
- Existing tests: No reviewed association.
- Decision: `return result;`.

## decision.targetresolver.3deda599e038

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:227`.
- Existing tests: No reviewed association.
- Decision: `string.Equals(named.SchemaObject?.BaseIdentifier?.Value, id, StringComparison.OrdinalIgnoreCase))`.

## decision.targetresolver.3f680bbf0677

In ResolveQuery, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:204`.
- Existing tests: No reviewed association.
- Decision: `case QueryParenthesisExpression parenthesis:`.

## decision.targetresolver.424e36309078

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:246`.
- Existing tests: No reviewed association.
- Decision: `case JoinParenthesisTableReference parenthesis:`.

## decision.targetresolver.43032f557274

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:78`.
- Existing tests: No reviewed association.
- Decision: `if (exposed != null && !ReferenceEquals(exposed, named))`.

## decision.targetresolver.522622ef5f29

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:172`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.targetresolver.6052be62a9cc

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:135`.
- Existing tests: No reviewed association.
- Decision: `if (visitingCtes.Count > 256)`.

## decision.targetresolver.6618d4ea7df5

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:81`.
- Existing tests: No reviewed association.
- Decision: `var via = isAlias ? new[] { "via alias " + id } : empty;`.

## decision.targetresolver.670a05c5cbe1

In Deduplicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:183`.
- Existing tests: No reviewed association.
- Decision: `.GroupBy(target => target.Kind + ":" + (target.Name != null`.

## decision.targetresolver.68d391036ee3

In TargetResolver, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:56`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.targetresolver.68eeea6e5d78

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:145`.
- Existing tests: No reviewed association.
- Decision: `result.Add(new BaseTarget(BaseTargetKind.Named, named.SchemaObject, named.SchemaObject, viaList));`.

## decision.targetresolver.6a3d88cf004d

In TargetResolver, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:55`.
- Existing tests: No reviewed association.
- Decision: `if (with?.CommonTableExpressions == null)`.

## decision.targetresolver.6de02e556886

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:174`.
- Existing tests: No reviewed association.
- Decision: `result.Add(new BaseTarget(BaseTargetKind.Remote, source, null, viaList, "remote write"));`.

## decision.targetresolver.71d169f5a24c

In SinglePartName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:263`.
- Existing tests: No reviewed association.
- Decision: `return name.BaseIdentifier.Value;`.

## decision.targetresolver.7647da6dc85e

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:139`.
- Existing tests: No reviewed association.
- Decision: `result.AddRange(resolved.Select(target => new BaseTarget(target.Kind, target.Fragment, target.Name,`.

## decision.targetresolver.77c753b522d4

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:250`.
- Existing tests: No reviewed association.
- Decision: `case TableReferenceWithAlias withAlias:`.

## decision.targetresolver.78bfd7f09aae

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:242`.
- Existing tests: No reviewed association.
- Decision: `case JoinTableReference join:`.

## decision.targetresolver.7c20be9cc7c4

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:173`.
- Existing tests: No reviewed association.
- Decision: `if (IsRemote(source))`.

## decision.targetresolver.808a64aa4a3c

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:123`.
- Existing tests: No reviewed association.
- Decision: `if (_resolvedCtes.TryGetValue(id, out var cached))`.

## decision.targetresolver.830be71cc7dc

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:136`.
- Existing tests: No reviewed association.
- Decision: `throw new AnalysisLimitException("too complex to analyze: CTE nesting budget exceeded");`.

## decision.targetresolver.84f4572646ee

In TargetResolver, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:60`.
- Existing tests: No reviewed association.
- Decision: `if (!string.IsNullOrEmpty(name))`.

## decision.targetresolver.87000f86779e

In IsRemote, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:266`.
- Existing tests: No reviewed association.
- Decision: `internal static bool IsRemote(TableReference reference) =>`.

## decision.targetresolver.8984aa375b20

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:215`.
- Existing tests: No reviewed association.
- Decision: `if (from?.TableReferences == null)`.

## decision.targetresolver.8c3c6ceae5c7

In SinglePartName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:260`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.targetresolver.8c3c6ceae5c7.2

In SinglePartName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:262`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.targetresolver.8e1850ea69bd

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:92`.
- Existing tests: No reviewed association.
- Decision: `case VariableTableReference variable:`.

## decision.targetresolver.91b2c4971774

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:244`.
- Existing tests: No reviewed association.
- Decision: `yield return inner;`.

## decision.targetresolver.91b2c4971774.2

In Flatten, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:248`.
- Existing tests: No reviewed association.
- Decision: `yield return inner;`.

## decision.targetresolver.93b062aee62f

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:75`.
- Existing tests: No reviewed association.
- Decision: `if (id != null)`.

## decision.targetresolver.956f62dbfeea

In Deduplicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:184`.
- Existing tests: No reviewed association.
- Decision: `? string.Concat(target.Name.Identifiers.Select(identifier => identifier.Value.Length + ":" + identifier.Value))`.

## decision.targetresolver.97eb43d62506

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:89`.
- Existing tests: No reviewed association.
- Decision: `return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Named, name, name, empty) };`.

## decision.targetresolver.98d703307786

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:216`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.targetresolver.99ad40fdb0e4

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:149`.
- Existing tests: No reviewed association.
- Decision: `case VariableTableReference variable:`.

## decision.targetresolver.a1780a63f9ea

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:233`.
- Existing tests: No reviewed association.
- Decision: `return byName;`.

## decision.targetresolver.a6384cc364bb

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:85`.
- Existing tests: No reviewed association.
- Decision: `if (_ctes.ContainsKey(id))`.

## decision.targetresolver.a902c5de2577

In Deduplicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:186`.
- Existing tests: No reviewed association.
- Decision: `.Select(group => group.First()).ToList();`.

## decision.targetresolver.a99af18bd0dd

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:93`.
- Existing tests: No reviewed association.
- Decision: `return new List<BaseTarget> { new BaseTarget(BaseTargetKind.TableVariable, variable.Variable, null, empty) };`.

## decision.targetresolver.ac1a9672b74c

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:163`.
- Existing tests: No reviewed association.
- Decision: `case JoinTableReference join:`.

## decision.targetresolver.af2017297ca1

In SinglePartName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:261`.
- Existing tests: No reviewed association.
- Decision: `if (name.SchemaIdentifier != null || name.DatabaseIdentifier != null || name.ServerIdentifier != null)`.

## decision.targetresolver.b107d485c7a7

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:71`.
- Existing tests: No reviewed association.
- Decision: `case NamedTableReference named:`.

## decision.targetresolver.bc98db749560

In SinglePartName, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:259`.
- Existing tests: No reviewed association.
- Decision: `if (name == null || name.BaseIdentifier == null)`.

## decision.targetresolver.c96f00e0b5b8

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:224`.
- Existing tests: No reviewed association.
- Decision: `return tableReference;`.

## decision.targetresolver.c9db724d7a74

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:155`.
- Existing tests: No reviewed association.
- Decision: `var alias = derived.Alias?.Value;`.

## decision.targetresolver.cc5d0e398658

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:125`.
- Existing tests: No reviewed association.
- Decision: `result.AddRange(cached.Select(target => new BaseTarget(target.Kind, target.Fragment, target.Name,`.

## decision.targetresolver.cd79a4dca3a7

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:150`.
- Existing tests: No reviewed association.
- Decision: `result.Add(new BaseTarget(BaseTargetKind.TableVariable, variable.Variable, null, viaList));`.

## decision.targetresolver.d7e10a82b1ec

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:95`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.targetresolver.deacb97679b4

In OrUnresolved, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:105`.
- Existing tests: No reviewed association.
- Decision: `return resolved;`.

## decision.targetresolver.e1c27c2fcc9d

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:153`.
- Existing tests: No reviewed association.
- Decision: `case QueryDerivedTable derived:`.

## decision.targetresolver.e220afd82793

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:97`.
- Existing tests: No reviewed association.
- Decision: `return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Remote, target, null, empty, "remote write") };`.

## decision.targetresolver.e9d2fe94ef26

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:118`.
- Existing tests: No reviewed association.
- Decision: `case NamedTableReference named:`.

## decision.targetresolver.ee0b654f679c

In OrUnresolved, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:106`.
- Existing tests: No reviewed association.
- Decision: `return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Unresolved, at, null, via, "could not resolve the base table") };`.

## decision.targetresolver.ef344249d866

In ResolveWriteTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:82`.
- Existing tests: No reviewed association.
- Decision: `return OrUnresolved(ResolveSource(exposed, via, new HashSet<string>(StringComparer.OrdinalIgnoreCase)), named, via);`.

## decision.targetresolver.f2055dc2c016

In FindExposedTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:223`.
- Existing tests: No reviewed association.
- Decision: `if (string.Equals(tableReference.Alias.Value, id, StringComparison.OrdinalIgnoreCase))`.

## decision.targetresolver.f4dd5871e001

In TargetResolver, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:59`.
- Existing tests: No reviewed association.
- Decision: `var name = cte.ExpressionName?.Value;`.

## decision.targetresolver.f5b19d765a5e

In ResolveQuery, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:194`.
- Existing tests: No reviewed association.
- Decision: `case QuerySpecification spec when spec.FromClause != null:`.

## decision.targetresolver.fd8806473970

In ResolveSource, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:157`.
- Existing tests: No reviewed association.
- Decision: `? viaList.Concat(new[] { "via derived table " + alias })`.

## resolve.alias

Resolve aliases before # name spelling and recursively resolve CTE/derived sources; explicit aliases outrank unaliased base names.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:257`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:76 (UpdateAliasPointingToCte_ResolvesThroughBoth); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:26 (DeleteViaAliasWithoutFromKeyword_ResolvesToBaseTable)

## resolve.alias-priority

Explicit alias wins over unaliased base-name match; an aliased table does not expose its original name.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:218`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:200 (AllowListHonorsQualificationAndExactIdentifierEnd)

## resolve.cte-budget

More than 256 simultaneously visited CTE names aborts bounded analysis.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:135`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:371 (DoublingCteGraphIsMemoizedAndDeduplicated)

## resolve.cte-cache

Memoize resolved CTE bases; merge provenance and deduplicate identity without exponential expansion.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:123`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:371 (DoublingCteGraphIsMemoizedAndDeduplicated)

## resolve.cte-cycle

Recursive CTE visits stop on cycles; anchor sources already collected remain.

- Outcomes: Unknown, RealWrite, TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:129`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:59 (RecursiveCte_DoesNotLoopForever)

## resolve.derived

Follow derived query sources and include via-derived-table provenance.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:153`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:67 (UpdateViaDerivedTable_ResolvesToBaseTable)

## resolve.join

Join source resolution follows both sides; parenthesized joins unwrap.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:163`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:76 (UpdateAliasPointingToCte_ResolvesThroughBoth)

## resolve.no-base

No base found behind alias/CTE/derived target yields explicit unresolved target, not green.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:104`.
- Existing tests: No reviewed association.

## resolve.union

Binary query targets combine both branches conservatively.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:199`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:42 (CteReferencingCte_ResolvesRecursively)

## resolve.variable

Variable table targets resolve to table-variable kind.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/TargetResolver.cs:92`.
- Existing tests: tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:37 (Case04_InsertTableVariable_IsTempOnly)

## safe-dbcc.activecursors

ReadOnlyDbcc contains ActiveCursors; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: No reviewed association.

## safe-dbcc.cachestats

ReadOnlyDbcc contains CacheStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: No reviewed association.

## safe-dbcc.checkalloc

ReadOnlyDbcc contains CheckAlloc; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: No reviewed association.

## safe-dbcc.checkcatalog

ReadOnlyDbcc contains CheckCatalog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: No reviewed association.

## safe-dbcc.checkconstraints

ReadOnlyDbcc contains CheckConstraints; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: No reviewed association.

## safe-dbcc.checkdb

ReadOnlyDbcc contains CheckDB; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: No reviewed association.

## safe-dbcc.checkfilegroup

ReadOnlyDbcc contains CheckFileGroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: No reviewed association.

## safe-dbcc.checktable

ReadOnlyDbcc contains CheckTable; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:129`.
- Existing tests: No reviewed association.

## safe-dbcc.cursorstats

ReadOnlyDbcc contains CursorStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: No reviewed association.

## safe-dbcc.extentinfo

ReadOnlyDbcc contains ExtentInfo; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: No reviewed association.

## safe-dbcc.help

ReadOnlyDbcc contains Help; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: No reviewed association.

## safe-dbcc.inputbuffer

ReadOnlyDbcc contains InputBuffer; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:126`.
- Existing tests: No reviewed association.

## safe-dbcc.loginfo

ReadOnlyDbcc contains LogInfo; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:130`.
- Existing tests: No reviewed association.

## safe-dbcc.memorystatus

ReadOnlyDbcc contains MemoryStatus; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: No reviewed association.

## safe-dbcc.opentran

ReadOnlyDbcc contains OpenTran; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:126`.
- Existing tests: No reviewed association.

## safe-dbcc.outputbuffer

ReadOnlyDbcc contains OutputBuffer; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:126`.
- Existing tests: No reviewed association.

## safe-dbcc.perfmonstats

ReadOnlyDbcc contains PerfMonStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:131`.
- Existing tests: No reviewed association.

## safe-dbcc.proccache

ReadOnlyDbcc contains ProcCache; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: No reviewed association.

## safe-dbcc.showcontig

ReadOnlyDbcc contains ShowContig; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:127`.
- Existing tests: No reviewed association.

## safe-dbcc.showfilestats

ReadOnlyDbcc contains ShowFileStats; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:128`.
- Existing tests: No reviewed association.

## safe-dbcc.showoffrules

ReadOnlyDbcc contains ShowOffRules; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: No reviewed association.

## safe-dbcc.showonrules

ReadOnlyDbcc contains ShowOnRules; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: No reviewed association.

## safe-dbcc.showstatistics

ReadOnlyDbcc contains ShowStatistics; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:126`.
- Existing tests: No reviewed association.

## safe-dbcc.showtext

ReadOnlyDbcc contains ShowText; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: No reviewed association.

## safe-dbcc.showweights

ReadOnlyDbcc contains ShowWeights; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:132`.
- Existing tests: No reviewed association.

## safe-dbcc.sqlperf

ReadOnlyDbcc contains SqlPerf; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:127`.
- Existing tests: No reviewed association.

## safe-dbcc.tracestatus

ReadOnlyDbcc contains TraceStatus; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:127`.
- Existing tests: No reviewed association.

## safe-dbcc.useroptions

ReadOnlyDbcc contains UserOptions; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:127`.
- Existing tests: No reviewed association.

## safe-procedure.sp_column_privileges

ReadOnlyProcedures contains sp_column_privileges; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: No reviewed association.

## safe-procedure.sp_columns

ReadOnlyProcedures contains sp_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_databases

ReadOnlyProcedures contains sp_databases; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: No reviewed association.

## safe-procedure.sp_depends

ReadOnlyProcedures contains sp_depends; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: No reviewed association.

## safe-procedure.sp_describe_first_result_set

ReadOnlyProcedures contains sp_describe_first_result_set; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: No reviewed association.

## safe-procedure.sp_describe_undeclared_parameters

ReadOnlyProcedures contains sp_describe_undeclared_parameters; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: No reviewed association.

## safe-procedure.sp_fkeys

ReadOnlyProcedures contains sp_fkeys; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: No reviewed association.

## safe-procedure.sp_getapplock

ReadOnlyProcedures contains sp_getapplock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: No reviewed association.

## safe-procedure.sp_help

ReadOnlyProcedures contains sp_help; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_help_job

ReadOnlyProcedures contains sp_help_job; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: No reviewed association.

## safe-procedure.sp_help_jobhistory

ReadOnlyProcedures contains sp_help_jobhistory; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: No reviewed association.

## safe-procedure.sp_help_jobstep

ReadOnlyProcedures contains sp_help_jobstep; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: No reviewed association.

## safe-procedure.sp_help_schedule

ReadOnlyProcedures contains sp_help_schedule; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpconstraint

ReadOnlyProcedures contains sp_helpconstraint; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpdb

ReadOnlyProcedures contains sp_helpdb; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpdbfixedrole

ReadOnlyProcedures contains sp_helpdbfixedrole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpextendedproc

ReadOnlyProcedures contains sp_helpextendedproc; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:23`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpfile

ReadOnlyProcedures contains sp_helpfile; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpfilegroup

ReadOnlyProcedures contains sp_helpfilegroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpindex

ReadOnlyProcedures contains sp_helpindex; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helplanguage

ReadOnlyProcedures contains sp_helplanguage; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helplogins

ReadOnlyProcedures contains sp_helplogins; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpntgroup

ReadOnlyProcedures contains sp_helpntgroup; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helprole

ReadOnlyProcedures contains sp_helprole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helprolemember

ReadOnlyProcedures contains sp_helprolemember; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helprotect

ReadOnlyProcedures contains sp_helprotect; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpserver

ReadOnlyProcedures contains sp_helpserver; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpsort

ReadOnlyProcedures contains sp_helpsort; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:22`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpsrvrole

ReadOnlyProcedures contains sp_helpsrvrole; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpsrvrolemember

ReadOnlyProcedures contains sp_helpsrvrolemember; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helptext

ReadOnlyProcedures contains sp_helptext; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helptrigger

ReadOnlyProcedures contains sp_helptrigger; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:17`.
- Existing tests: No reviewed association.

## safe-procedure.sp_helpuser

ReadOnlyProcedures contains sp_helpuser; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:18`.
- Existing tests: No reviewed association.

## safe-procedure.sp_lock

ReadOnlyProcedures contains sp_lock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: No reviewed association.

## safe-procedure.sp_monitor

ReadOnlyProcedures contains sp_monitor; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: No reviewed association.

## safe-procedure.sp_pkeys

ReadOnlyProcedures contains sp_pkeys; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: No reviewed association.

## safe-procedure.sp_readerrorlog

ReadOnlyProcedures contains sp_readerrorlog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: No reviewed association.

## safe-procedure.sp_releaseapplock

ReadOnlyProcedures contains sp_releaseapplock; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: No reviewed association.

## safe-procedure.sp_server_info

ReadOnlyProcedures contains sp_server_info; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: No reviewed association.

## safe-procedure.sp_set_session_context

ReadOnlyProcedures contains sp_set_session_context; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: No reviewed association.

## safe-procedure.sp_spaceused

ReadOnlyProcedures contains sp_spaceused; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:16`.
- Existing tests: No reviewed association.

## safe-procedure.sp_special_columns

ReadOnlyProcedures contains sp_special_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: No reviewed association.

## safe-procedure.sp_sproc_columns

ReadOnlyProcedures contains sp_sproc_columns; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:24`.
- Existing tests: No reviewed association.

## safe-procedure.sp_statistics

ReadOnlyProcedures contains sp_statistics; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: No reviewed association.

## safe-procedure.sp_stored_procedures

ReadOnlyProcedures contains sp_stored_procedures; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:19`.
- Existing tests: No reviewed association.

## safe-procedure.sp_table_privileges

ReadOnlyProcedures contains sp_table_privileges; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:20`.
- Existing tests: No reviewed association.

## safe-procedure.sp_tables

ReadOnlyProcedures contains sp_tables; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_who

ReadOnlyProcedures contains sp_who; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.sp_who2

ReadOnlyProcedures contains sp_who2; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:15`.
- Existing tests: No reviewed association.

## safe-procedure.xp_readerrorlog

ReadOnlyProcedures contains xp_readerrorlog; classify as ReadOnly after specialized dispatch/qualification overrides.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:21`.
- Existing tests: No reviewed association.

## decision.scope.062836fa985d

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:154`.
- Existing tests: No reviewed association.
- Decision: `return text;`.

## decision.scope.064c6d84becb

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:111`.
- Existing tests: No reviewed association.
- Decision: `ReportAt ?? new Position(line + LineDelta, column, offset + OffsetDelta, length);`.

## decision.scope.0884cdf1f486

In ForText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:85`.
- Existing tests: No reviewed association.
- Decision: `public static Scope ForText(string text, string displayText = null, IReadOnlyDictionary<string, string> variables = null) =>`.

## decision.scope.111e872d87f8

In Scope, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:61`.
- Existing tests: No reviewed association.
- Decision: `public Position? ReportAt { get; }`.

## decision.scope.240a2704e945

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:124`.
- Existing tests: No reviewed association.
- Decision: `starts.Add(index + 1);`.

## decision.scope.2e17d6d21f90

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:146`.
- Existing tests: No reviewed association.
- Decision: `if (start < 0 || length <= 0 || start + length > DisplayText.Length)`.

## decision.scope.386119a8b230

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:131`.
- Existing tests: No reviewed association.
- Decision: `int line = found >= 0 ? found + 1 : ~found;`.

## decision.scope.3a74894a832c

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:106`.
- Existing tests: No reviewed association.
- Decision: `return new Position(1 + LineDelta, 1, OffsetDelta, 0);`.

## decision.scope.3f742c4dc85f

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:123`.
- Existing tests: No reviewed association.
- Decision: `if (index + 1 < Text.Length && Text[index + 1] == '\n') index++;`.

## decision.scope.416f00bfaa6c

In Blocked, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:95`.
- Existing tests: No reviewed association.
- Decision: `public Scope Blocked(int errorLine) =>`.

## decision.scope.59e5d179a0a3

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:107`.
- Existing tests: No reviewed association.
- Decision: `return new Position(fragment.StartLine + LineDelta, fragment.StartColumn, fragment.StartOffset + OffsetDelta, fragment.FragmentLength);`.

## decision.scope.5dc9489f2dfa

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:141`.
- Existing tests: No reviewed association.
- Decision: `return TextOf(fragment.StartOffset, fragment.FragmentLength);`.

## decision.scope.6003ffe4860a

In Scope, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:37`.
- Existing tests: No reviewed association.
- Decision: `private Scope(string text, string displayText, int offsetDelta, int lineDelta, Position? reportAt, int dynamicDepth, int? blockingErrorLine,`.

## decision.scope.62ca15f8ec63

In Scope, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:65`.
- Existing tests: No reviewed association.
- Decision: `public bool IsDynamic => DynamicDepth > 0;`.

## decision.scope.6597cc1ed87c

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:77`.
- Existing tests: No reviewed association.
- Decision: `var result = Regex.Replace(text, @"\$\((?<name>[A-Za-z_][A-Za-z0-9_]*)\)",`.

## decision.scope.704d69569d20

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:140`.
- Existing tests: No reviewed association.
- Decision: `return string.Empty;`.

## decision.scope.704d69569d20.2

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:147`.
- Existing tests: No reviewed association.
- Decision: `return string.Empty;`.

## decision.scope.72cdd73f4c7f

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:126`.
- Existing tests: No reviewed association.
- Decision: `else if (Text[index] == '\n') starts.Add(index + 1);`.

## decision.scope.85e39ed71d57

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:121`.
- Existing tests: No reviewed association.
- Decision: `if (Text[index] == '\r')`.

## decision.scope.88ba0ed0f17d

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:139`.
- Existing tests: No reviewed association.
- Decision: `if (fragment == null)`.

## decision.scope.93936b92bb3d

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:78`.
- Existing tests: No reviewed association.
- Decision: `m => Variables.TryGetValue((OffsetDelta + start + m.Index).ToString(System.Globalization.CultureInfo.InvariantCulture), out var value) ? value : m.Value);`.

## decision.scope.af91cf296a2f

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:79`.
- Existing tests: No reviewed association.
- Decision: `return result == text ? null : result;`.

## decision.scope.b16448fe7028

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:152`.
- Existing tests: No reviewed association.
- Decision: `if (text.Length > MaxTargetTextLength)`.

## decision.scope.c0fc97c2d9fd

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:73`.
- Existing tests: No reviewed association.
- Decision: `if (Variables == null || string.IsNullOrEmpty(text) || text.IndexOf("$(", System.StringComparison.Ordinal) < 0)`.

## decision.scope.c25fe58553ac

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:116`.
- Existing tests: No reviewed association.
- Decision: `if (_lineStarts == null)`.

## decision.scope.c2940eb06ee5

In ForBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:88`.
- Existing tests: No reviewed association.
- Decision: `public Scope ForBatch(int startOffset, int length, int lineBreaksBefore) =>`.

## decision.scope.cd7611964053

In TextOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:150`.
- Existing tests: No reviewed association.
- Decision: `if (text.IndexOfAny(new[] { '\r', '\n', '\t' }) >= 0)`.

## decision.scope.cd9b2e16abbe

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:74`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.scope.d55aba66c4ad

In WithText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:92`.
- Existing tests: No reviewed association.
- Decision: `public Scope WithText(string parsedText) =>`.

## decision.scope.dd8f0fb70236

In Scope, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:42`.
- Existing tests: No reviewed association.
- Decision: `DisplayText = displayText ?? text;`.

## decision.scope.de68b1e3ef9c

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:110`.
- Existing tests: No reviewed association.
- Decision: `public Position PositionOf(int line, int column, int offset, int length = 0) =>`.

## decision.scope.e69268c3c4f0

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:76`.
- Existing tests: No reviewed association.
- Decision: `if (start < 0) return null;`.

## decision.scope.e80a5203290d

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:103`.
- Existing tests: No reviewed association.
- Decision: `if (ReportAt.HasValue)`.

## decision.scope.efad0f38a80c

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:104`.
- Existing tests: No reviewed association.
- Decision: `return ReportAt.Value;`.

## decision.scope.f553a3fdec25

In PositionOf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:105`.
- Existing tests: No reviewed association.
- Decision: `if (fragment == null || fragment.StartOffset < 0)`.

## decision.scope.f8a11a4bc74d

In PositionOfOffset, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:133`.
- Existing tests: No reviewed association.
- Decision: `return PositionOf(line, column, offset, length);`.

## decision.scope.fb2b74d65ce9

In SubstituteVariables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:83`.
- Existing tests: No reviewed association.
- Decision: `public int? BlockingErrorLine { get; }`.

## decision.scope.ff7863723e7c

In ForDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:98`.
- Existing tests: No reviewed association.
- Decision: `public Scope ForDynamicSql(string sql, TSqlFragment executeStatement) =>`.

## scope.batch-offset

GO batch substrings preserve original offset and line deltas.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:88`.
- Existing tests: tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:137 (Case15_SecondGoBatch_ReportsCorrectLine)

## scope.blocked

Recovered batch retains blocking syntax-error line for WillRun metadata.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:95`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:38 (OnlyTheBrokenBatchIsBlocked)

## scope.display

Display text collapses whitespace and truncates long snippets; substitutions are offset-aware and do not confer trust.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:144`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:272 (Positions_AreRelativeToAnalyzedText)

## scope.dynamic

Nested dynamic findings inherit outer execution anchor, context and blocking error.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:98`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:283 (DynamicSqlInsideBatch_ReportedAtExecLine)

## scope.newlines

CRLF is one line break; CR and LF individually also advance source positions.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:121`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:128 (RecoveredFindings_KeepExactPositions)

## scope.target-truncate

Display targets truncate at 200 characters; classification must use AST rather than display.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/Scope.cs:152`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:94 (DbccUsesParsedArgumentsBeyondDisplayTruncation)

## set-family.generalsetcommandtype.contextinfo

SET CONTEXT_INFO 0x01 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.datefirst

SET DATEFIRST 1 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.dateformat

SET DATEFORMAT ymd inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.deadlockpriority

SET DEADLOCK_PRIORITY LOW inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.language

SET LANGUAGE us_english inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.locktimeout

SET LOCK_TIMEOUT 1000 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.generalsetcommandtype.querygovernorcostlimit

SET QUERY_GOVERNOR_COST_LIMIT 0 inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.compute

SET OFFSETS COMPUTE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.execute

SET OFFSETS EXECUTE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.from

SET OFFSETS FROM ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.order

SET OFFSETS ORDER ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.param

SET OFFSETS PARAM ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.procedure

SET OFFSETS PROCEDURE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.select

SET OFFSETS SELECT ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.statement

SET OFFSETS STATEMENT ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setoffsets.table

SET OFFSETS TABLE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setstatisticsoptions.io

SET STATISTICS IO ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setstatisticsoptions.profile

SET STATISTICS PROFILE ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setstatisticsoptions.time

SET STATISTICS TIME ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-family.setstatisticsoptions.xml

SET STATISTICS XML ON inherits green session SET classification; no exemption from strict session-residue contract is implied.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansidefaults

SET ANSI_DEFAULTS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansinulldfltoff

SET ANSI_NULL_DFLT_OFF ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansinulldflton

SET ANSI_NULL_DFLT_ON ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansinulls

SET ANSI_NULLS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansipadding

SET ANSI_PADDING ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.ansiwarnings

SET ANSI_WARNINGS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.arithabort

SET ARITHABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.arithignore

SET ARITHIGNORE ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.concatnullyieldsnull

SET CONCAT_NULL_YIELDS_NULL ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.cursorcloseoncommit

SET CURSOR_CLOSE_ON_COMMIT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.disabledefcnstchk

SET DISABLE_DEF_CNST_CHK ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.fmtonly

SET FMTONLY ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.forceplan

SET FORCEPLAN ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.implicittransactions

SET IMPLICIT_TRANSACTIONS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.nobrowsetable

SET NO_BROWSETABLE ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.nocount

SET NOCOUNT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.noexec

SET NOEXEC ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.numericroundabort

SET NUMERIC_ROUNDABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.parseonly

SET PARSEONLY ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.quotedidentifier

SET QUOTED_IDENTIFIER ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.remoteproctransactions

SET REMOTE_PROC_TRANSACTIONS ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.showplanall

SET SHOWPLAN_ALL ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.showplantext

SET SHOWPLAN_TEXT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.showplanxml

SET SHOWPLAN_XML ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## set-option.xactabort

SET XACT_ABORT ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:108`.
- Existing tests: No reviewed association.

## decision.sqlcmdpreprocessor.109c62de41d3

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:108`.
- Existing tests: No reviewed association.
- Decision: `if (plain) variables[match.Index.ToString(System.Globalization.CultureInfo.InvariantCulture)] = value;`.

## decision.sqlcmdpreprocessor.12e257be24c2

In SqlCmdPreprocessor, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:21`.
- Existing tests: No reviewed association.
- Decision: `private static readonly Regex SetVar = new Regex(@"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s+(?:""(?<value>[^""]*)""|(?<value>\S+)))?",`.

## decision.sqlcmdpreprocessor.159813d522b8

In FindSqlCmdSyntax, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:59`.
- Existing tests: No reviewed association.
- Decision: `variableReferences.Add(new Directive("$()", match.Groups["name"].Value, match.Index, match.Length));`.

## decision.sqlcmdpreprocessor.1c83bd96ab54

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:80`.
- Existing tests: No reviewed association.
- Decision: `if (buffer[i] != '\r' && buffer[i] != '\n')`.

## decision.sqlcmdpreprocessor.2e221d2de9e7

In FindSqlCmdSyntax, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:52`.
- Existing tests: No reviewed association.
- Decision: `if (string.IsNullOrEmpty(text))`.

## decision.sqlcmdpreprocessor.451f62a69f74

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:114`.
- Existing tests: No reviewed association.
- Decision: `return buffer.ToString();`.

## decision.sqlcmdpreprocessor.45b74518ebb8

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:106`.
- Existing tests: No reviewed association.
- Decision: `char first = plain && !adjacent && parts.Length == 1 && parts[0].StartsWith("#", StringComparison.Ordinal) ? '#' : '_';`.

## decision.sqlcmdpreprocessor.560d0eef56b9

In FindSqlCmdSyntax, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:53`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.sqlcmdpreprocessor.647543704fb5

In IsIdentifier, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:117`.
- Existing tests: No reviewed association.
- Decision: `private static bool IsIdentifier(char value) => char.IsLetterOrDigit(value) || value == '_' || value == '#' || value == '@' || value == '$' || value == '[' || value == ']';`.

## decision.sqlcmdpreprocessor.6795c546773b

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:77`.
- Existing tests: No reviewed association.
- Decision: `directives.Add(new Directive(command, arguments, start, end - start));`.

## decision.sqlcmdpreprocessor.6bbd847381b4

In LooksLikeSqlCmd, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:44`.
- Existing tests: No reviewed association.
- Decision: `public static bool LooksLikeSqlCmd(string text) =>`.

## decision.sqlcmdpreprocessor.851c3d25894a

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:92`.
- Existing tests: No reviewed association.
- Decision: `if (definition.Command != ":setvar") continue;`.

## decision.sqlcmdpreprocessor.9cd6f69cbd71

In SqlCmdPreprocessor, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:19`.
- Existing tests: No reviewed association.
- Decision: `private static readonly Regex Variable = new Regex(@"\$\((?<name>[^()\r\n]*)\)", RegexOptions.CultureInvariant);`.

## decision.sqlcmdpreprocessor.a9538bbd4df9

In SqlCmdPreprocessor, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: No reviewed association.
- Decision: `@"^[ \t]*(?:(?<cmd>:(?:setvar|r|on|connect|out|error|exit|quit|reset|ed|list|listvar|help|perftrace|serverlist|xml))(?=\s|\(|$)(?<args>.*)|(?<cmd>!!)(?<args>.*))$",`.

## decision.sqlcmdpreprocessor.ad64c739b375

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:97`.
- Existing tests: No reviewed association.
- Decision: `if (setVar.Groups["value"].Success) variables[name] = setVar.Groups["value"].Value;`.

## decision.sqlcmdpreprocessor.c5cdd8a2e4fc

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:94`.
- Existing tests: No reviewed association.
- Decision: `if (!setVar.Success) continue;`.

## decision.sqlcmdpreprocessor.de5853587de4

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:107`.
- Existing tests: No reviewed association.
- Decision: `directives.Add(new Directive("$()", match.Value, match.Index, match.Length));`.

## decision.sqlcmdpreprocessor.e4f9754bfc0f

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:103`.
- Existing tests: No reviewed association.
- Decision: `System.Text.RegularExpressions.Regex.IsMatch(value, @"\A(?:\[(?:[^\]]|\]\])+\]|[#\w]+)(?:\.(?:\[(?:[^\]]|\]\])+\]|[#\w]+)){0,3}\z");`.

## decision.sqlcmdpreprocessor.e54431aeee9f

In FindSqlCmdSyntax, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:56`.
- Existing tests: No reviewed association.
- Decision: `commands.Add(new Directive(match.Groups["cmd"].Value.ToLowerInvariant(), match.Groups["args"].Value.Trim(),`.

## decision.sqlcmdpreprocessor.fa078ca0d9d8

In Process, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:100`.
- Existing tests: No reviewed association.
- Decision: `var parts = value == null ? null : AllowList.SplitName(value);`.

## sqlcmd.ordered-definitions

Only preceding setvar definitions supply display values; later definitions cannot retroactively grant trust.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:89`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:143 (SqlCmdDefinitionsDoNotApplyRetroactivelyAndOffsetsStayOriginal)

## sqlcmd.process

Blank directive lines without shifting offsets, substitute only identifier-safe display names; unresolved references remain warnings.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:62`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:285 (SqlCmdScript_ParsesWithoutErrors)

## sqlcmd.remove-definition

A setvar without value removes the variable, leaving later references unresolved.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:96`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.connect

SQLCMD mode recognizes :connect; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.ed

SQLCMD mode recognizes :ed; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.error

SQLCMD mode recognizes :error; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.exit

SQLCMD mode recognizes :exit; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly, Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.help

SQLCMD mode recognizes :help; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.list

SQLCMD mode recognizes :list; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.listvar

SQLCMD mode recognizes :listvar; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.on

SQLCMD mode recognizes :on; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.os

SQLCMD mode recognizes !!; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:10`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.out

SQLCMD mode recognizes :out; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.perftrace

SQLCMD mode recognizes :perftrace; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.quit

SQLCMD mode recognizes :quit; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.r

SQLCMD mode recognizes :r; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.reset

SQLCMD mode recognizes :reset; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.serverlist

SQLCMD mode recognizes :serverlist; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.setvar

SQLCMD mode recognizes :setvar; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.variable

SQLCMD mode recognizes $(); connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:19`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## sqlcmd-command.xml

SQLCMD mode recognizes :xml; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.

- Outcomes: ReadOnly; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs:16`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:137 (UnexpandedSqlCmdExecutionIsNeverGreen)

## unknown-dbcc.addinstance

DBCC AddInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.auditevent

DBCC AuditEvent is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.autopilot

DBCC AutoPilot is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.buffer

DBCC Buffer is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.bytes

DBCC Bytes is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.cacheprofile

DBCC CacheProfile is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.callfulltext

DBCC CallFullText is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.checkprimaryfile

DBCC CheckPrimaryFile is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.collectstats

DBCC CollectStats is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.concurrencyviolation

DBCC ConcurrencyViolation is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.debugbreak

DBCC DebugBreak is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.deleteinstance

DBCC DeleteInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumpconfig

DBCC DumpConfig is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumpdbinfo

DBCC DumpDBInfo is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumpdbtable

DBCC DumpDBTable is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumplock

DBCC DumpLock is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumplog

DBCC DumpLog is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumppage

DBCC DumpPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumpresource

DBCC DumpResource is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.dumptrigger

DBCC DumpTrigger is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.fileheader

DBCC FileHeader is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.flush

DBCC Flush is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.icecapquery

DBCC IcecapQuery is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.incrementinstance

DBCC IncrementInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.ind

DBCC Ind is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.invalidatetextptr

DBCC InvalidateTextptr is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.invalidatetextptrobjid

DBCC InvalidateTextptrObjid is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.latch

DBCC Latch is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.mapallocunit

DBCC MapAllocUnit is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.memobjlist

DBCC MemObjList is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.memorymap

DBCC MemoryMap is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.metadata

DBCC Metadata is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.notextptr

DBCC NoTextptr is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.optimizerwhatif

DBCC OptimizerWhatIf is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.persiststackhash

DBCC PersistStackHash is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.prtipage

DBCC PrtiPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.readpage

DBCC ReadPage is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.ruleoff

DBCC RuleOff is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.ruleon

DBCC RuleOn is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.semetadata

DBCC SeMetadata is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.setinstance

DBCC SetInstance is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.showdbaffinity

DBCC ShowDBAffinity is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.showtableaffinity

DBCC ShowTableAffinity is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.sqlmgrstats

DBCC SqlMgrStats is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.stackdump

DBCC StackDump is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.tec

DBCC Tec is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.throttleio

DBCC ThrottleIO is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-dbcc.useplan

DBCC UsePlan is absent from both command tables and uses Unknown unless an argument override applies.

- Outcomes: Unknown, RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:212`.
- Existing tests: No reviewed association.

## unknown-statement.backupstatement

UnknownStatementTypes contains BackupStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:219`.
- Existing tests: No reviewed association.

## unknown-statement.checkpointstatement

UnknownStatementTypes contains CheckpointStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:221`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## unknown-statement.updatestatisticsstatement

UnknownStatementTypes contains UpdateStatisticsStatement; classify as Unknown after specialized dispatch/qualification overrides.

- Outcomes: Unknown; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:220`.
- Existing tests: tests/SqlSafetyGuard.Tests/StatementVerdictsTests.cs:15 (CommittedStatementVerdictsMatchCurrentAnalyzer)

## decision.statementwalker.00dbf5f8bab4

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:320`.
- Existing tests: No reviewed association.
- Decision: `if (current != null)`.

## decision.statementwalker.00dbf5f8bab4.2

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:325`.
- Existing tests: No reviewed association.
- Decision: `if (current != null)`.

## decision.statementwalker.01d41b979efe

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:835`.
- Existing tests: No reviewed association.
- Decision: `var baseName = name.BaseIdentifier?.Value ?? string.Empty;`.

## decision.statementwalker.022b6a59792b

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:628`.
- Existing tests: No reviewed association.
- Decision: `case MergeStatement merge:`.

## decision.statementwalker.03572d3d2158

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:951`.
- Existing tests: No reviewed association.
- Decision: `bool? answer = null;`.

## decision.statementwalker.0360762d518e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:588`.
- Existing tests: No reviewed association.
- Decision: `set.IsOn ? "IMPLICIT_TRANSACTIONS ON: later changes stay uncommitted until COMMIT" : null);`.

## decision.statementwalker.0360ae29648d

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:601`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, select, "SELECT", string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.0385d2495d6a

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1212`.
- Existing tests: No reviewed association.
- Decision: `notes.Add($"won't run now: this batch has a syntax error at line {scope.BlockingErrorLine.Value}");`.

## decision.statementwalker.03b84cf37e85

In IsSameTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1106`.
- Existing tests: No reviewed association.
- Decision: `return Same(name.BaseIdentifier, other.BaseIdentifier) &&`.

## decision.statementwalker.0449bf58a254

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:557`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, begin, Classifier.ActionName(begin), string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.0688799c9da3

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:987`.
- Existing tests: No reviewed association.
- Decision: `if (name.ServerIdentifier != null) return false;`.

## decision.statementwalker.0843ce4c74c7

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1062`.
- Existing tests: No reviewed association.
- Decision: `case QualifiedJoin join:`.

## decision.statementwalker.089e564865c3

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1169`.
- Existing tests: No reviewed association.
- Decision: `if (!string.IsNullOrEmpty(baseTarget.Note))`.

## decision.statementwalker.08f0f8ab78c1

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:379`.
- Existing tests: No reviewed association.
- Decision: `case ":quit":`.

## decision.statementwalker.09834f70ef1b

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:946`.
- Existing tests: No reviewed association.
- Decision: `if (!(expression is BooleanComparisonExpression comparison)) return null;`.

## decision.statementwalker.09d98c2ee9b0

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1265`.
- Existing tests: No reviewed association.
- Decision: `case SchemaObjectName schemaObjectName:`.

## decision.statementwalker.0a1c3c5389d1

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:501`.
- Existing tests: No reviewed association.
- Decision: `case BeginEndBlockStatement block: // includes BEGIN ATOMIC blocks`.

## decision.statementwalker.0a76f853e439

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:841`.
- Existing tests: No reviewed association.
- Decision: `var statementParameter = procedure.Parameters.FirstOrDefault(p =>`.

## decision.statementwalker.0a79f25193e6

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1211`.
- Existing tests: No reviewed association.
- Decision: `if (scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.0a8160e3644f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:509`.
- Existing tests: No reviewed association.
- Decision: `case WhileStatement whileStatement:`.

## decision.statementwalker.0b4b60c3f1e8

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:811`.
- Existing tests: No reviewed association.
- Decision: `if (spec == null)`.

## decision.statementwalker.0bf8be0fce5b

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:993`.
- Existing tests: No reviewed association.
- Decision: `if (string.Equals(database, "master", StringComparison.OrdinalIgnoreCase)) return schema != null;`.

## decision.statementwalker.0d8de92a5e81

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:600`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.0e1f39723573

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:564`.
- Existing tests: No reviewed association.
- Decision: `case CommitTransactionStatement commit:`.

## decision.statementwalker.0e52a5ff63a1

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:745`.
- Existing tests: No reviewed association.
- Decision: `var filter = modification is UpdateSpecification update ? new RowFilter(update.Target, update.WhereClause, update.FromClause, update.TopRowFilter) :`.

## decision.statementwalker.0e5ee2d12d9c

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1270`.
- Existing tests: No reviewed association.
- Decision: `return identifierOrValue.Value;`.

## decision.statementwalker.0f92fb1220e4

In InvalidateTargetContext, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:441`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, "TARGET SWITCH", string.Empty, null, Verdict.Unknown,`.

## decision.statementwalker.0fa6c2d6e87e

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:991`.
- Existing tests: No reviewed association.
- Decision: `!string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase)) return false;`.

## decision.statementwalker.10741931018f

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1321`.
- Existing tests: No reviewed association.
- Decision: `batches.Add(new Batch(start, text.Length - start, lineBreaksBefore));`.

## decision.statementwalker.1133e519ce8d

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1266`.
- Existing tests: No reviewed association.
- Decision: `return scope.TextOf(schemaObjectName);`.

## decision.statementwalker.11db6ba7723a

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1244`.
- Existing tests: No reviewed association.
- Decision: `return string.Join(", ", drop.Objects.Select(scope.TextOf));`.

## decision.statementwalker.11fc2b7e6c55

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:845`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.11fc2b7e6c55.2

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:887`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.121fc1061082

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:343`.
- Existing tests: No reviewed association.
- Decision: `if (first == null)`.

## decision.statementwalker.1309713b8b4b

In Begin, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:72`.
- Existing tests: No reviewed association.
- Decision: `if (Depth == 0)`.

## decision.statementwalker.152180ca6977

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:850`.
- Existing tests: No reviewed association.
- Decision: `if (system && string.Equals(baseName, "sp_spaceused", StringComparison.OrdinalIgnoreCase) && UpdatesUsage(procedure))`.

## decision.statementwalker.15314f76e3dd

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:963`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.LessThan: value = first < second; break;`.

## decision.statementwalker.154bc23c2af2

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:380`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, directive.Command, args, null, Verdict.ReadOnly, "ends the script here");`.

## decision.statementwalker.16fb3602cc9b

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:917`.
- Existing tests: No reviewed association.
- Decision: `if (before.Uncertain) condition = null;`.

## decision.statementwalker.17ee7f81c74f

In Copy, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:64`.
- Existing tests: No reviewed association.
- Decision: `public TransactionState Copy() => new TransactionState`.

## decision.statementwalker.1a6ca28b2c12

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:185`.
- Existing tests: No reviewed association.
- Decision: `var use = tokens.FirstOrDefault(token => token.TokenType == TSqlTokenType.Use);`.

## decision.statementwalker.1a89a8ee22ec

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1230`.
- Existing tests: No reviewed association.
- Decision: `return scope.TextOf(target);`.

## decision.statementwalker.1aa6c1137847

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:183`.
- Existing tests: No reviewed association.
- Decision: `var tokens = fragment?.ScriptTokenStream ?? BatchRecovery.Tokenize(parser, scope.Text);`.

## decision.statementwalker.1bccbeb4ba62

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:359`.
- Existing tests: No reviewed association.
- Decision: `case ":connect":`.

## decision.statementwalker.1bf1ec3336db

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1072`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.statementwalker.1bf1ec3336db.2

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1079`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.statementwalker.1c64799f82ac

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:190`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.1c64799f82ac.2

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:196`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.1c6e606005ea

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:870`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.ReadOnly, "trusted procedure");`.

## decision.statementwalker.1f4a5c26df83

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:979`.
- Existing tests: No reviewed association.
- Decision: `string.Equals(function.FunctionName?.Value, "XACT_STATE", StringComparison.OrdinalIgnoreCase))`.

## decision.statementwalker.1f86402f7ae9

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:674`.
- Existing tests: No reviewed association.
- Decision: `case DropStatisticsStatement dropStatistics:`.

## decision.statementwalker.21c87910f3e3

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:990`.
- Existing tests: No reviewed association.
- Decision: `if (schema != null && !string.Equals(schema, "sys", StringComparison.OrdinalIgnoreCase) &&`.

## decision.statementwalker.22134e1a4890

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:655`.
- Existing tests: No reviewed association.
- Decision: `case DropTableStatement dropTable:`.

## decision.statementwalker.23ccb72cc1f4

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1092`.
- Existing tests: No reviewed association.
- Decision: `return NamedTables(parenthesis.Join);`.

## decision.statementwalker.25464337a4bc

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1087`.
- Existing tests: No reviewed association.
- Decision: `case NamedTableReference named:`.

## decision.statementwalker.2723f3988307

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1274`.
- Existing tests: No reviewed association.
- Decision: `return string.Empty;`.

## decision.statementwalker.27ac38512da8

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:740`.
- Existing tests: No reviewed association.
- Decision: `var with = (writer as StatementWithCtesAndXmlNamespaces)?.WithCtesAndXmlNamespaces;`.

## decision.statementwalker.27cc94ef2bdf

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:924`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.28a36f5d7fed

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:367`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, ":r", args, null, Verdict.Unknown, "includes another script; it is not analyzed");`.

## decision.statementwalker.28edad8ddf0b

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1091`.
- Existing tests: No reviewed association.
- Decision: `case JoinParenthesisTableReference parenthesis:`.

## decision.statementwalker.2cc330644ab4

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1065`.
- Existing tests: No reviewed association.
- Decision: `case QualifiedJoinType.Inner:`.

## decision.statementwalker.2d04dc9235d2

In WalkStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:475`.
- Existing tests: No reviewed association.
- Decision: `throw new AnalysisLimitException("too complex to analyze: statements nested too deeply");`.

## decision.statementwalker.2f130852440a

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1040`.
- Existing tests: No reviewed association.
- Decision: `from.TableReferences.SelectMany(NamedTables).Any(source => IsSameTable(target, source));`.

## decision.statementwalker.2f28e6ea7000

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1213`.
- Existing tests: No reviewed association.
- Decision: `if (resolved == null && !action.StartsWith(":", StringComparison.Ordinal))`.

## decision.statementwalker.2f334801ff86

In AddOutputInto, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1184`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.2f528c177a05

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:127`.
- Existing tests: No reviewed association.
- Decision: `return continuing;`.

## decision.statementwalker.2f5c79a038cc

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1128`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("no WHERE: limited only by TOP");`.

## decision.statementwalker.2f8a653e90aa

In AddNamedWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1195`.
- Existing tests: No reviewed association.
- Decision: `if (verdict == Verdict.AllowListed)`.

## decision.statementwalker.300503ee880e

In UpdatesUsage, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1005`.
- Existing tests: No reviewed association.
- Decision: `string.Equals(value.Variable?.Name, "@updateusage", StringComparison.OrdinalIgnoreCase));`.

## decision.statementwalker.302bddb3ba2b

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:565`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue && _transactions.Depth == 0 && !_transactionGuard)`.

## decision.statementwalker.30553137a59a

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:780`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(TSqlFragment node) => AnalysisWork.Check();`.

## decision.statementwalker.3076308c9abe

In IsSameTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1100`.
- Existing tests: No reviewed association.
- Decision: `if (!(target is NamedTableReference named))`.

## decision.statementwalker.30b75bfb1453

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1070`.
- Existing tests: No reviewed association.
- Decision: `return RestrictsRows(join.SecondTableReference);`.

## decision.statementwalker.30d0d281e4a1

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:553`.
- Existing tests: No reviewed association.
- Decision: `_transactions = catchEnds ? afterTry : TransactionState.Merge(afterTry, _transactions);`.

## decision.statementwalker.3128c20b50f4

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1201`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOf(at), action, target, resolved, verdict, note);`.

## decision.statementwalker.315d6319ee3d

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:755`.
- Existing tests: No reviewed association.
- Decision: `if (!written.Contains(source.Reference))`.

## decision.statementwalker.3273b6126762

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:592`.
- Existing tests: No reviewed association.
- Decision: `case WaitForStatement waitFor when waitFor.Statement != null:`.

## decision.statementwalker.3310d0b1402c

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:203`.
- Existing tests: No reviewed association.
- Decision: `if (batch.Length == 0 || string.IsNullOrWhiteSpace(scope.Text.Substring(batch.Start, batch.Length)))`.

## decision.statementwalker.331ca50f38bc

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:264`.
- Existing tests: No reviewed association.
- Decision: `AddAt(blocked, position, module.Action, module.Name, null, Verdict.RealWrite, note);`.

## decision.statementwalker.33881e85a174

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:225`.
- Existing tests: No reviewed association.
- Decision: `.Where(e => e.Offset >= scope.OffsetDelta && e.Offset <= scope.OffsetDelta + scope.Text.Length)`.

## decision.statementwalker.33a1117c4c5b

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:547`.
- Existing tests: No reviewed association.
- Decision: `WalkStatements(tryCatch.CatchStatements?.Statements, scope);`.

## decision.statementwalker.34f325ff3f1d

In WalkStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:480`.
- Existing tests: No reviewed association.
- Decision: `if (terminated != null) _transactions = terminated;`.

## decision.statementwalker.35dad436dc9f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:577`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, rollback, Classifier.ActionName(rollback), string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.365e9154adb8

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:587`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, set, "SET", string.Empty, null, Verdict.ReadOnly,`.

## decision.statementwalker.36639951dae5

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:230`.
- Existing tests: No reviewed association.
- Decision: `if (recovery.Fragment == null)`.

## decision.statementwalker.367bdb7f4dd1

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:162`.
- Existing tests: No reviewed association.
- Decision: `if (_transactions.Depth <= 0 || !_transactions.OpenedAt.HasValue)`.

## decision.statementwalker.375a2a0f3d37

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:550`.
- Existing tests: No reviewed association.
- Decision: `if (catchEnds && (_transactions.Depth > 0 || _transactions.Uncertain))`.

## decision.statementwalker.379cd1602511

In StatementWalker, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:50`.
- Existing tests: No reviewed association.
- Decision: `public Position? OpenedAt { get; set; }`.

## decision.statementwalker.382681fe7e53

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:873`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.Unknown,`.

## decision.statementwalker.387d52a314d9

In AddSkippedRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:274`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, "NOT ANALYZED", region.Lines, null, Verdict.Unknown, reason);`.

## decision.statementwalker.399b6374d3ab

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1126`.
- Existing tests: No reviewed association.
- Decision: `if (filter.HasTop && !filter.TopIsPercent)`.

## decision.statementwalker.39c2cfd20a2b

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:863`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.RealWrite, "system procedure that " + writes);`.

## decision.statementwalker.39f591587de4

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:313`.
- Existing tests: No reviewed association.
- Decision: `if (current != null && current.Module == null && region.Module == null &&`.

## decision.statementwalker.3a931988d40f

In AddNamedWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1197`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOf(at), action, scope.TextOf(name), null, verdict, string.Join(", ", notes), allRows);`.

## decision.statementwalker.3b35c1ad03be

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:149`.
- Existing tests: No reviewed association.
- Decision: `return true;`.

## decision.statementwalker.3b532de05e31

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:544`.
- Existing tests: No reviewed association.
- Decision: `_transactions.OpenedAt = afterTry.OpenedAt ?? afterTry.PeakOpenedAt;`.

## decision.statementwalker.3c7f3e50d387

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1026`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, executeStatement, action, target, null, Verdict.ReadOnly, "empty dynamic SQL");`.

## decision.statementwalker.3c8cf000c490

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1068`.
- Existing tests: No reviewed association.
- Decision: `return RestrictsRows(join.FirstTableReference);`.

## decision.statementwalker.3df4b26ce74b

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:346`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOfOffset(first.Offset, first.Length), SqlCmdModeOffAction, string.Empty, null, Verdict.Unknown,`.

## decision.statementwalker.3e116a3c92dc

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1071`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.3e116a3c92dc.2

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1078`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.3fccb51a2acc

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1077`.
- Existing tests: No reviewed association.
- Decision: `return RestrictsRows(parenthesis.Join);`.

## decision.statementwalker.3fcd1780f61d

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1229`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.4052cd501c15

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:648`.
- Existing tests: No reviewed association.
- Decision: `case AlterTableSwitchStatement switchTable:`.

## decision.statementwalker.411e55a90f3a

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:977`.
- Existing tests: No reviewed association.
- Decision: `{ guard = true; return new[] { depth }; }`.

## decision.statementwalker.42df4b579858

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:976`.
- Existing tests: No reviewed association.
- Decision: `if (expression is GlobalVariableExpression variable && string.Equals(variable.Name, "@@TRANCOUNT", StringComparison.OrdinalIgnoreCase))`.

## decision.statementwalker.43ab37efe3c3

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:939`.
- Existing tests: No reviewed association.
- Decision: `if (expression is BooleanParenthesisExpression parenthesis)`.

## decision.statementwalker.448ba9706674

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:978`.
- Existing tests: No reviewed association.
- Decision: `if (expression is FunctionCall function && function.CallTarget == null && function.Parameters.Count == 0 &&`.

## decision.statementwalker.45395ca9dbfc

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1203`.
- Existing tests: No reviewed association.
- Decision: `private void AddAt(Scope scope, Position position, string action, string target, string resolved, Verdict verdict, string note,`.

## decision.statementwalker.45f1ee8eb43c

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1242`.
- Existing tests: No reviewed association.
- Decision: `return scope.TextOf(procedure.ProcedureReference.Name);`.

## decision.statementwalker.46675bcfd3c3

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:376`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, directive.Command, args, null, string.IsNullOrWhiteSpace(args) ? Verdict.ReadOnly : Verdict.Unknown,`.

## decision.statementwalker.46b4c93a8016

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:572`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, commit, Classifier.ActionName(commit), string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.4734c120fc43

In WalkScript, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:428`.
- Existing tests: No reviewed association.
- Decision: `if (!(fragment is TSqlScript script))`.

## decision.statementwalker.478a3ea68c4c

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1022`.
- Existing tests: No reviewed association.
- Decision: `if (!string.IsNullOrWhiteSpace(sql))`.

## decision.statementwalker.486c62d34f95

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:597`.
- Existing tests: No reviewed association.
- Decision: `case SelectStatement select:`.

## decision.statementwalker.489ad2904a1b

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:761`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, sequence, "NEXT VALUE FOR", scope.TextOf(sequence.SequenceName), null, Verdict.RealWrite,`.

## decision.statementwalker.49a465b88019

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1089`.
- Existing tests: No reviewed association.
- Decision: `case JoinTableReference join:`.

## decision.statementwalker.49b1e45917ba

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1157`.
- Existing tests: No reviewed association.
- Decision: `case BaseTargetKind.TableVariable:`.

## decision.statementwalker.49bb9bba9041

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:708`.
- Existing tests: No reviewed association.
- Decision: `if (unknownNote != null)`.

## decision.statementwalker.49bee2aef464

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:145`.
- Existing tests: No reviewed association.
- Decision: `return select.Into != null || tables.HasTable;`.

## decision.statementwalker.4e5f43d07b0f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:522`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, thrown, "THROW", string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.4ea6b24ceea7

In Same, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1110`.
- Existing tests: No reviewed association.
- Decision: `private static bool Same(Identifier a, Identifier b) =>`.

## decision.statementwalker.4eb6bac0f99c

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1067`.
- Existing tests: No reviewed association.
- Decision: `case QualifiedJoinType.LeftOuter:`.

## decision.statementwalker.4efab30f0e57

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1161`.
- Existing tests: No reviewed association.
- Decision: `case BaseTargetKind.Remote:`.

## decision.statementwalker.4f49384fdee5

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:528`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, Classifier.ActionName(statement), string.Empty, null,`.

## decision.statementwalker.5036246874bb

In Visit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:781`.
- Existing tests: No reviewed association.
- Decision: `public override void Visit(DataModificationTableReference node) => Specifications.Add(node.DataModificationSpecification);`.

## decision.statementwalker.5356fbc5561c

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1227`.
- Existing tests: No reviewed association.
- Decision: `case VariableTableReference variable:`.

## decision.statementwalker.545894ba04af

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1133`.
- Existing tests: No reviewed association.
- Decision: `else if (filter.HasRestrictingJoin)`.

## decision.statementwalker.549a42f30f32

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:210`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.54c9c7d4b43e

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:817`.
- Existing tests: No reviewed association.
- Decision: `if (spec.LinkedServer != null)`.

## decision.statementwalker.577dd7c13434

In Add, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1200`.
- Existing tests: No reviewed association.
- Decision: `private void Add(Scope scope, TSqlFragment at, string action, string target, string resolved, Verdict verdict, string note) =>`.

## decision.statementwalker.57969b13e441

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1090`.
- Existing tests: No reviewed association.
- Decision: `return NamedTables(join.FirstTableReference).Concat(NamedTables(join.SecondTableReference));`.

## decision.statementwalker.57ad9177900b

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1069`.
- Existing tests: No reviewed association.
- Decision: `case QualifiedJoinType.RightOuter:`.

## decision.statementwalker.580db1b2e15b

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:825`.
- Existing tests: No reviewed association.
- Decision: `case ExecutableProcedureReference procedure:`.

## decision.statementwalker.5904e0027d0c

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:377`.
- Existing tests: No reviewed association.
- Decision: `string.IsNullOrWhiteSpace(args) ? "ends the script here" : "SQLCMD executes this exit query; it is not analyzed");`.

## decision.statementwalker.5bd0a4dc5c96

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1088`.
- Existing tests: No reviewed association.
- Decision: `return new[] { named };`.

## decision.statementwalker.5e93c9c5648e

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1314`.
- Existing tests: No reviewed association.
- Decision: `if (next < text.Length && text[next] == '\n')`.

## decision.statementwalker.5eb5784965b2

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:296`.
- Existing tests: No reviewed association.
- Decision: `if (code.Count > 0)`.

## decision.statementwalker.5ee0ce2a8480

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1131`.
- Existing tests: No reviewed association.
- Decision: `else if (filter.HasTop)`.

## decision.statementwalker.5f2e0069fc55

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:801`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.5fbc6a5aa453

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:843`.
- Existing tests: No reviewed association.
- Decision: `if (statementParameter?.ParameterValue is StringLiteral literal)`.

## decision.statementwalker.5fef1ad9a4ef

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:193`.
- Existing tests: No reviewed association.
- Decision: `if (scope.IsDynamic)`.

## decision.statementwalker.5ff472fa47fc

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1177`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOf(at), action, written, resolved, verdict, string.Join(", ", notes), allRows);`.

## decision.statementwalker.5ffaeb5e8ad0

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:739`.
- Existing tests: No reviewed association.
- Decision: `fragment?.Accept(nested);`.

## decision.statementwalker.6045810f0bda

In AddSkippedRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:282`.
- Existing tests: No reviewed association.
- Decision: `var keyword = token.Text.ToUpperInvariant() == "EXEC" ? "EXECUTE" : token.Text.ToUpperInvariant();`.

## decision.statementwalker.6102dd41e5a5

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:659`.
- Existing tests: No reviewed association.
- Decision: `case CreateIndexStatement createIndex:`.

## decision.statementwalker.6132e2a8f54a

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:932`.
- Existing tests: No reviewed association.
- Decision: `_transactions = alternatives.Count == 0 ? before : alternatives.Aggregate(TransactionState.Merge);`.

## decision.statementwalker.630b38fae951

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1322`.
- Existing tests: No reviewed association.
- Decision: `return batches;`.

## decision.statementwalker.635bd1339089

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:830`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, scope.TextOf(reference), null, Verdict.Unknown, "procedure name is in a variable");`.

## decision.statementwalker.644e9715202b

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:150`.
- Existing tests: No reviewed association.
- Decision: `if (statement is BeginEndBlockStatement || statement is IfStatement || statement is WhileStatement ||`.

## decision.statementwalker.6596dc4468aa

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1171`.
- Existing tests: No reviewed association.
- Decision: `if (verdict == Verdict.AllowListed)`.

## decision.statementwalker.659e57004a51

In CheckBatchRepetition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:456`.
- Existing tests: No reviewed association.
- Decision: `if (int.TryParse(repeat, out int count) && count != 1)`.

## decision.statementwalker.663153526a43

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:958`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.Equals: value = first == second; break;`.

## decision.statementwalker.6684508dda2f

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:135`.
- Existing tests: No reviewed association.
- Decision: `return merged;`.

## decision.statementwalker.66f080e109bb

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:584`.
- Existing tests: No reviewed association.
- Decision: `_transactions.Save(save.Name?.Identifier?.Value ?? (save.Name?.ValueExpression as Literal)?.Value);`.

## decision.statementwalker.6802d3f2ab44

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:152`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.statementwalker.68bd289a5ed8

In AddOutputInto, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1183`.
- Existing tests: No reviewed association.
- Decision: `if (outputInto?.IntoTable == null)`.

## decision.statementwalker.68be163986cf

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:813`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, string.Empty, null, Verdict.Unknown, "could not analyze EXEC");`.

## decision.statementwalker.6930519d482e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:586`.
- Existing tests: No reviewed association.
- Decision: `case PredicateSetStatement set when (set.Options & SetOptions.ImplicitTransactions) == SetOptions.ImplicitTransactions:`.

## decision.statementwalker.69a089b14f89

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:962`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.GreaterThanOrEqualTo: value = first >= second; break;`.

## decision.statementwalker.6ac2f2f42563

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:959`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.NotEqualToBrackets:`.

## decision.statementwalker.6ae8a582e19e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:710`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.Unknown, unknownNote);`.

## decision.statementwalker.6c0b381c4665

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:970`.
- Existing tests: No reviewed association.
- Decision: `return answer;`.

## decision.statementwalker.6c3b99d86128

In CheckBatchRepetition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:454`.
- Existing tests: No reviewed association.
- Decision: `if (next < tokens.Count && tokens[next].Line == tokens[index].Line &&`.

## decision.statementwalker.6cd3f415518d

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:492`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.10

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:569`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.11

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:575`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.12

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:580`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.13

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:585`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.14

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:591`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.15

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:594`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.16

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:603`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.17

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:607`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.18

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:616`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.19

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:626`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.2

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:503`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.20

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:635`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.21

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:639`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.22

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:642`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.23

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:647`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.24

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:651`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.25

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:654`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.26

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:658`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.27

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:661`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.28

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:664`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.29

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:667`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.3

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:507`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.30

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:670`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.31

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:673`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.32

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:683`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.33

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:691`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.34

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:696`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.35

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:704`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.36

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:711`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.37

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:719`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.38

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:725`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.4

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:515`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.5

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:520`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.6

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:524`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.7

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:532`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.8

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:554`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6cd3f415518d.9

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:563`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.6e1ca61daa3e

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:989`.
- Existing tests: No reviewed association.
- Decision: `var database = name.DatabaseIdentifier?.Value;`.

## decision.statementwalker.6eb30d546372

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:491`.
- Existing tests: No reviewed association.
- Decision: `if (statement == null)`.

## decision.statementwalker.6ecc826240e5

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1303`.
- Existing tests: No reviewed association.
- Decision: `if (token.TokenType != TSqlTokenType.Go)`.

## decision.statementwalker.6ed1bea99dff

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:394`.
- Existing tests: No reviewed association.
- Decision: `if (errors.Count == 0)`.

## decision.statementwalker.6ffd52fb6fd3

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1241`.
- Existing tests: No reviewed association.
- Decision: `case ProcedureStatementBody procedure when procedure.ProcedureReference?.Name != null:`.

## decision.statementwalker.70886b1c00ef

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:556`.
- Existing tests: No reviewed association.
- Decision: `case BeginTransactionStatement begin:`.

## decision.statementwalker.709acd6748a4

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:981`.
- Existing tests: No reviewed association.
- Decision: `if (expression is IntegerLiteral integer && int.TryParse(integer.Value, out int value)) return new[] { value };`.

## decision.statementwalker.70ba4e1a9970

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:868`.
- Existing tests: No reviewed association.
- Decision: `name.SchemaIdentifier?.Value, baseName, _options.CurrentDatabase, _options.DefaultSchema))`.

## decision.statementwalker.7102e4a8670a

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:916`.
- Existing tests: No reviewed association.
- Decision: `bool? condition = EvaluateTransactionPredicate(conditional.Predicate, before.Depth, out bool guard);`.

## decision.statementwalker.719d6f3a6558

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1154`.
- Existing tests: No reviewed association.
- Decision: `if (baseTarget.Name.ServerIdentifier != null)`.

## decision.statementwalker.737a6270bd39

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:122`.
- Existing tests: No reviewed association.
- Decision: `if (exited.Depth == 0 && !exited.Uncertain)`.

## decision.statementwalker.73c8355e029f

In WalkStatements, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:465`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.741ea9b69969

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1141`.
- Existing tests: No reviewed association.
- Decision: `notes.Add(Finding.CteOrDerivedTableLimitNote);`.

## decision.statementwalker.743f8d71bace

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:526`.
- Existing tests: No reviewed association.
- Decision: `case ContinueStatement _:`.

## decision.statementwalker.745104112757

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:960`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.NotEqualToExclamation: value = first != second; break;`.

## decision.statementwalker.7483b60118e6

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1138`.
- Existing tests: No reviewed association.
- Decision: `else if (filter.HasFrom)`.

## decision.statementwalker.7483cd29bb6f

In AddNamedWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1196`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("matches allow-list");`.

## decision.statementwalker.75a08d8a5ab2

In CheckBatchRepetition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:450`.
- Existing tests: No reviewed association.
- Decision: `if (tokens[index].TokenType != TSqlTokenType.Go) continue;`.

## decision.statementwalker.76247ac806e9

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:517`.
- Existing tests: No reviewed association.
- Decision: `case ReturnStatement returned:`.

## decision.statementwalker.762d2fcbde77

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:558`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.762d2fcbde77.2

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:573`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.762d2fcbde77.3

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:578`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.762d2fcbde77.4

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:583`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.762d2fcbde77.5

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:589`.
- Existing tests: No reviewed association.
- Decision: `if (!scope.BlockingErrorLine.HasValue)`.

## decision.statementwalker.77a98961ccb9

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:889`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, "dynamic SQL", null, Verdict.Unknown, "dynamic SQL is built at runtime");`.

## decision.statementwalker.79b29008db88

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:923`.
- Existing tests: No reviewed association.
- Decision: `if (conditional.ElseStatement is IfStatement next) conditional = next;`.

## decision.statementwalker.79def1790357

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:121`.
- Existing tests: No reviewed association.
- Decision: `var exited = a.Terminated ? a : b;`.

## decision.statementwalker.7c3a302bc675

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:397`.
- Existing tests: No reviewed association.
- Decision: `if (scope.IsDynamic)`.

## decision.statementwalker.7c64a1662ae2

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:874`.
- Existing tests: No reviewed association.
- Decision: `insertExec ? "the procedure may write" : "procedure body is not analyzed");`.

## decision.statementwalker.7ca75d544aff

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:819`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, "AT " + spec.LinkedServer.Value, null, Verdict.Unknown, "runs on a linked server");`.

## decision.statementwalker.7d934996c6f1

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1174`.
- Existing tests: No reviewed association.
- Decision: `if (string.Equals(resolved, written, StringComparison.OrdinalIgnoreCase))`.

## decision.statementwalker.7dcc5524fa23

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:852`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.RealWrite, "updates space usage metadata");`.

## decision.statementwalker.7e4f72b2dc77

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1143`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("no WHERE: affects every row");`.

## decision.statementwalker.7eb69b7e5c11

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:119`.
- Existing tests: No reviewed association.
- Decision: `if (a.Terminated != b.Terminated)`.

## decision.statementwalker.8274e2c3a192

In WalkScript, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:429`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.82d17a82bebc

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:857`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.ReadOnly, "read-only system procedure");`.

## decision.statementwalker.847a9dd79a41

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:245`.
- Existing tests: No reviewed association.
- Decision: `if (recovery.Regions.Any(r => r.Contains(relative)))`.

## decision.statementwalker.849b742c7c6c

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1017`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, executeStatement, action, target, null, Verdict.Unknown, "dynamic SQL nested too deeply to analyze");`.

## decision.statementwalker.85e513ae4c65

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1151`.
- Existing tests: No reviewed association.
- Decision: `case BaseTargetKind.Named:`.

## decision.statementwalker.85f7d2b66b61

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:375`.
- Existing tests: No reviewed association.
- Decision: `case ":exit":`.

## decision.statementwalker.868af32a4140

In WalkInsert, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:773`.
- Existing tests: No reviewed association.
- Decision: `if (spec.InsertSource is ExecuteInsertSource executeSource)`.

## decision.statementwalker.892c2554b614

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:940`.
- Existing tests: No reviewed association.
- Decision: `return EvaluateTransactionPredicate(parenthesis.Expression, depth, out guard);`.

## decision.statementwalker.89910da5e652

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:363`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, "SQLCMD VARIABLE", args, null, Verdict.Unknown,`.

## decision.statementwalker.8a27dafda142

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1269`.
- Existing tests: No reviewed association.
- Decision: `case IdentifierOrValueExpression identifierOrValue:`.

## decision.statementwalker.8a4b2d944d8a

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:373`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, ":setvar", args, null, Verdict.ReadOnly, "SQLCMD variable");`.

## decision.statementwalker.8aabff260f02

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1093`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.8b41b91cb31f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:727`.
- Existing tests: No reviewed association.
- Decision: `var note = IsModuleDefinition(statement) ? "changes a definition; the body is not run now" : null;`.

## decision.statementwalker.8c18919768c7

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:828`.
- Existing tests: No reviewed association.
- Decision: `if (reference?.ProcedureVariable != null || reference?.ProcedureReference?.Name == null)`.

## decision.statementwalker.8cd591ef0315

In Save, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:106`.
- Existing tests: No reviewed association.
- Decision: `SavePoints.Add(name);`.

## decision.statementwalker.8d80d3222c20

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:576`.
- Existing tests: No reviewed association.
- Decision: `case RollbackTransactionStatement rollback:`.

## decision.statementwalker.8dae1f70a638

In Rollback, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:98`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.8e343981b166

In Commit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:90`.
- Existing tests: No reviewed association.
- Decision: `if (Depth == 0)`.

## decision.statementwalker.8e4641113917

In IsSameTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1104`.
- Existing tests: No reviewed association.
- Decision: `return name.SchemaIdentifier == null && Same(name.BaseIdentifier, source.Alias);`.

## decision.statementwalker.91729f44e96c

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:936`.
- Existing tests: No reviewed association.
- Decision: `private static bool? EvaluateTransactionPredicate(BooleanExpression expression, int depth, out bool guard)`.

## decision.statementwalker.91924c20f5d8

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:529`.
- Existing tests: No reviewed association.
- Decision: `_transactions.Depth > 0 || _transactions.Implicit ? Verdict.Unknown : Verdict.ReadOnly,`.

## decision.statementwalker.91cac0daf892

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:369`.
- Existing tests: No reviewed association.
- Decision: `case "!!":`.

## decision.statementwalker.924873d6ade9

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:598`.
- Existing tests: No reviewed association.
- Decision: `if (select.Into != null)`.

## decision.statementwalker.925a42407f23

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:228`.
- Existing tests: No reviewed association.
- Decision: `var blocked = scope.Blocked(firstError?.Line ?? scope.LineDelta + 1);`.

## decision.statementwalker.9280a1cd1a13

In UpdatesUsage, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1006`.
- Existing tests: No reviewed association.
- Decision: `if (parameter == null && procedure.Parameters.Count > 1 && procedure.Parameters[1].Variable == null)`.

## decision.statementwalker.92b6ecf879b6

In IsModuleDefinition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1234`.
- Existing tests: No reviewed association.
- Decision: `private static bool IsModuleDefinition(TSqlStatement statement) =>`.

## decision.statementwalker.9302fb33f7a9

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:304`.
- Existing tests: No reviewed association.
- Decision: `_skipped.Add(region);`.

## decision.statementwalker.9330ab601250

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:965`.
- Existing tests: No reviewed association.
- Decision: `default: return null;`.

## decision.statementwalker.9539d79cccba

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:521`.
- Existing tests: No reviewed association.
- Decision: `case ThrowStatement thrown:`.

## decision.statementwalker.95ba8b0218a9

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1025`.
- Existing tests: No reviewed association.
- Decision: `if (_findings.Count == before)`.

## decision.statementwalker.96527b4a9725

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:752`.
- Existing tests: No reviewed association.
- Decision: `var written = writer != null ? SideEffectScanner.WrittenFragments(writer) : new HashSet<TSqlFragment>();`.

## decision.statementwalker.96b8c4ee5401

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1216`.
- Existing tests: No reviewed association.
- Decision: `_findings.Add(new Finding(position.Line, position.Column, position.Offset, position.Length, action, target, resolved, verdict,`.

## decision.statementwalker.9723d0018502

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:839`.
- Existing tests: No reviewed association.
- Decision: `if (system && string.Equals(baseName, "sp_executesql", StringComparison.OrdinalIgnoreCase))`.

## decision.statementwalker.975a9864229e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:665`.
- Existing tests: No reviewed association.
- Decision: `case AlterIndexStatement alterIndex:`.

## decision.statementwalker.995fea0dccc9

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:748`.
- Existing tests: No reviewed association.
- Decision: `modification is MergeSpecification ? "MERGE" : "INSERT", modification.Target, resolver.ResolveWriteTarget(modification.Target, from), filter);`.

## decision.statementwalker.99701b9db3ad

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:928`.
- Existing tests: No reviewed association.
- Decision: `if (reachable) alternatives.Add(_transactions);`.

## decision.statementwalker.9a344ae2ff12

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:744`.
- Existing tests: No reviewed association.
- Decision: `FromClause from = (modification as UpdateSpecification)?.FromClause ?? (modification as DeleteSpecification)?.FromClause;`.

## decision.statementwalker.9a37c7704089

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:421`.
- Existing tests: No reviewed association.
- Decision: `_findings.Add(new Finding(position.Line, position.Column, position.Offset, 0, "PARSE ERROR", string.Empty, null,`.

## decision.statementwalker.9a82d600544d

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:814`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.10

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:892`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.11

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:897`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.2

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:820`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.3

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:831`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.4

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:847`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.5

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:853`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.6

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:858`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.7

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:864`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.8

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:871`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9a82d600544d.9

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:875`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.9b13022ed71b

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:867`.
- Existing tests: No reviewed association.
- Decision: `if (_options.TrustedProcedures.Matches(name.ServerIdentifier?.Value, name.DatabaseIdentifier?.Value,`.

## decision.statementwalker.9bc3b5a10c13

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1164`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.9c20b4a62fcb

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:982`.
- Existing tests: No reviewed association.
- Decision: `return null;`.

## decision.statementwalker.9d3327dd9076

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:855`.
- Existing tests: No reviewed association.
- Decision: `if (system && Classifier.ReadOnlyProcedures.Contains(baseName))`.

## decision.statementwalker.9ef38c17539c

In AddSkippedRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:280`.
- Existing tests: No reviewed association.
- Decision: `if (!WriteKeywords.Contains(token.TokenType))`.

## decision.statementwalker.9f43409c77e4

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1306`.
- Existing tests: No reviewed association.
- Decision: `batches.Add(new Batch(start, Math.Max(0, token.Offset - start), lineBreaksBefore));`.

## decision.statementwalker.a04e25bdeffb

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1258`.
- Existing tests: No reviewed association.
- Decision: `catch (Exception)`.

## decision.statementwalker.a19c3931817e

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:790`.
- Existing tests: No reviewed association.
- Decision: `case DropIndexClause modern:`.

## decision.statementwalker.a19f541e3d62

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:504`.
- Existing tests: No reviewed association.
- Decision: `case IfStatement ifStatement:`.

## decision.statementwalker.a1be170855fb

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:261`.
- Existing tests: No reviewed association.
- Decision: `? $"must be the first statement in its batch: add GO before it. Everything after it in this batch becomes its body"`.

## decision.statementwalker.a237c9d5b85f

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1207`.
- Existing tests: No reviewed association.
- Decision: `if (scope.IsDynamic)`.

## decision.statementwalker.a397567aa093

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1132`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("no WHERE: limited only by TOP PERCENT");`.

## decision.statementwalker.a476f968efc6

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:530`.
- Existing tests: No reviewed association.
- Decision: `_transactions.Depth > 0 || _transactions.Implicit ? "control transfer may skip transaction cleanup; balance cannot be verified" : null);`.

## decision.statementwalker.a527e9efb2bb

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:680`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, dropStatistics, "DROP STATISTICS", scope.TextOf(name), null, temp ? Verdict.TempOnly : Verdict.RealWrite, null);`.

## decision.statementwalker.a605f365ceff

In TransactionValue, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:980`.
- Existing tests: No reviewed association.
- Decision: `{ guard = true; return depth == 0 ? new[] { 0 } : new[] { -1, 1 }; }`.

## decision.statementwalker.a665083edf9a

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:994`.
- Existing tests: No reviewed association.
- Decision: `var procedure = name.BaseIdentifier?.Value ?? string.Empty;`.

## decision.statementwalker.a78be7ca2c29

In StatementWalker, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:52`.
- Existing tests: No reviewed association.
- Decision: `public Position? PeakOpenedAt { get; set; }`.

## decision.statementwalker.a87ea92ca564

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1225`.
- Existing tests: No reviewed association.
- Decision: `case NamedTableReference named:`.

## decision.statementwalker.a8d6658e3aea

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1228`.
- Existing tests: No reviewed association.
- Decision: `return scope.TextOf(variable.Variable);`.

## decision.statementwalker.a8f8d0256df0

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:366`.
- Existing tests: No reviewed association.
- Decision: `case ":r":`.

## decision.statementwalker.aa3285bcc519

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1210`.
- Existing tests: No reviewed association.
- Decision: `notes.Add(note);`.

## decision.statementwalker.aa33bb27d622

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:147`.
- Existing tests: No reviewed association.
- Decision: `if (statement is DataModificationStatement || statement is TruncateTableStatement ||`.

## decision.statementwalker.aaac602a548a

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1018`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.aaccc7747c9b

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:718`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.TempOnly, "temporary procedure definition; the body is not run now");`.

## decision.statementwalker.aacf4ca3064a

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:344`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.aba327ad7fc7

In UpdatesUsage, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1004`.
- Existing tests: No reviewed association.
- Decision: `var parameter = procedure.Parameters.FirstOrDefault(value =>`.

## decision.statementwalker.abe4df6e1420

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:338`.
- Existing tests: No reviewed association.
- Decision: `var rejectedVariable = variables.FirstOrDefault(v =>`.

## decision.statementwalker.ae0190a0696c

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:988`.
- Existing tests: No reviewed association.
- Decision: `var schema = name.SchemaIdentifier?.Value;`.

## decision.statementwalker.ae667249c4b5

In TargetText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1226`.
- Existing tests: No reviewed association.
- Decision: `return scope.TextOf(named.SchemaObject);`.

## decision.statementwalker.aeb246575499

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:311`.
- Existing tests: No reviewed association.
- Decision: `foreach (var region in regions.OrderBy(r => r.Start))`.

## decision.statementwalker.b0909ed80751

In AnalyzeDynamicSql, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1015`.
- Existing tests: No reviewed association.
- Decision: `if (scope.DynamicDepth >= _options.MaxDynamicSqlDepth)`.

## decision.statementwalker.b1168f29f4c6

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:581`.
- Existing tests: No reviewed association.
- Decision: `case SaveTransactionStatement save:`.

## decision.statementwalker.b17a57d61a0e

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1155`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("remote server " + baseTarget.Name.ServerIdentifier.Value);`.

## decision.statementwalker.b1d6b1b8d63f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:608`.
- Existing tests: No reviewed association.
- Decision: `case UpdateStatement update:`.

## decision.statementwalker.b29aee39f6c1

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:560`.
- Existing tests: No reviewed association.
- Decision: `if (_transactions.Implicit && _transactions.Depth == 0) _transactions.Begin(scope.PositionOf(begin), implicitly: true);`.

## decision.statementwalker.b2bbc4b70dba

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1250`.
- Existing tests: No reviewed association.
- Decision: `if (property == null)`.

## decision.statementwalker.b3272661bf81

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1135`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("no WHERE: rows limited only by the JOIN");`.

## decision.statementwalker.b4a5210175c7

In WalkStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:478`.
- Existing tests: No reviewed association.
- Decision: `var terminated = _transactions.Terminated ? _transactions.Copy() : null;`.

## decision.statementwalker.b4ade47179eb

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:810`.
- Existing tests: No reviewed association.
- Decision: `var action = insertExec ? "INSERT ... EXEC" : "EXEC";`.

## decision.statementwalker.b5aa766ba3e4

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:234`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.b60372e6207c

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:728`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.RealWrite, note);`.

## decision.statementwalker.b67a35791ac1

In Begin, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:79`.
- Existing tests: No reviewed association.
- Decision: `if (Depth > Peak)`.

## decision.statementwalker.b6b8f5f20326

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:226`.
- Existing tests: No reviewed association.
- Decision: `.OrderBy(e => e.Offset)`.

## decision.statementwalker.b6d76850158f

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:159`.
- Existing tests: No reviewed association.
- Decision: `if (_transactions.Uncertain)`.

## decision.statementwalker.b75e23763db6

In Parse, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:390`.
- Existing tests: No reviewed association.
- Decision: `=> AnalysisWork.Parse(parser, text, out errors);`.

## decision.statementwalker.b77a14a7ff46

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:400`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, null, "EXEC", "dynamic SQL", null, Verdict.Unknown,`.

## decision.statementwalker.b7abc4000ba4

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1208`.
- Existing tests: No reviewed association.
- Decision: `notes.Add(DynamicSqlNote);`.

## decision.statementwalker.b7e113f53f00

In WalkIf, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:921`.
- Existing tests: No reviewed association.
- Decision: `if (reachable && condition != false) alternatives.Add(_transactions);`.

## decision.statementwalker.b81963afdcce

In AddSkippedRegion, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:283`.
- Existing tests: No reviewed association.
- Decision: `AddAt(blocked, scope.PositionOfOffset(start + token.Offset, token.Text.Length), "possible " + keyword, string.Empty, null,`.

## decision.statementwalker.b880cab57611

In CheckBatchRepetition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:457`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOfOffset(tokens[index].Offset, tokens[index].Text.Length), "BATCH REPETITION", repeat,`.

## decision.statementwalker.b8b90248f8bb

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:662`.
- Existing tests: No reviewed association.
- Decision: `case CreateColumnStoreIndexStatement createColumnStore:`.

## decision.statementwalker.b97ed9bf7a45

In NamedTables, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1094`.
- Existing tests: No reviewed association.
- Decision: `return Enumerable.Empty<NamedTableReference>();`.

## decision.statementwalker.bb0459330389

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:141`.
- Existing tests: No reviewed association.
- Decision: `if (statement is SelectStatement select)`.

## decision.statementwalker.bb977ae99599

In StartsImplicitTransaction, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:153`.
- Existing tests: No reviewed association.
- Decision: `return !Classifier.IsReadOnlyStatement(statement) && Classifier.UnknownStatementNote(statement) == null;`.

## decision.statementwalker.bccadc4a9e0b

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:126`.
- Existing tests: No reviewed association.
- Decision: `continuing.PeakOpenedAt = a.Peak >= b.Peak ? a.PeakOpenedAt : b.PeakOpenedAt;`.

## decision.statementwalker.bec7bbea7b85

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:266`.
- Existing tests: No reviewed association.
- Decision: `module.Misplaced ? module.Action + " is not the first statement in its batch" : module.Action + " could not be parsed");`.

## decision.statementwalker.bed3879dc81a

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:896`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, scope.TextOf(spec.ExecutableEntity), null, Verdict.Unknown, "could not analyze EXEC");`.

## decision.statementwalker.bf06655418aa

In RecoverBatch, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:252`.
- Existing tests: No reviewed association.
- Decision: `if (module == null)`.

## decision.statementwalker.bf9171d2e31a

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1267`.
- Existing tests: No reviewed association.
- Decision: `case Identifier identifier:`.

## decision.statementwalker.c0a50d41c72a

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1268`.
- Existing tests: No reviewed association.
- Decision: `return identifier.Value;`.

## decision.statementwalker.c1a0c0414114

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:878`.
- Existing tests: No reviewed association.
- Decision: `case ExecutableStringList strings:`.

## decision.statementwalker.c1be305a32fd

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:163`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.c27330f21008

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:964`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.LessThanOrEqualTo: value = first <= second; break;`.

## decision.statementwalker.c32ca1c6208b

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:186`.
- Existing tests: No reviewed association.
- Decision: `if (use != null) InvalidateTargetContext(scope, scope.PositionOfOffset(use.Offset, use.Text.Length));`.

## decision.statementwalker.c3fd005bde23

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:533`.
- Existing tests: No reviewed association.
- Decision: `case TryCatchStatement tryCatch:`.

## decision.statementwalker.c43c6380e261

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1142`.
- Existing tests: No reviewed association.
- Decision: `else`.

## decision.statementwalker.c57d2592cdf2

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:582`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, save, Classifier.ActionName(save), string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.c59ae1f500b8

In IsSameTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1101`.
- Existing tests: No reviewed association.
- Decision: `return false;`.

## decision.statementwalker.c5e1b417159d

In WalkStatements, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:464`.
- Existing tests: No reviewed association.
- Decision: `if (statements == null)`.

## decision.statementwalker.c68bafcd6beb

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:362`.
- Existing tests: No reviewed association.
- Decision: `case "$()":`.

## decision.statementwalker.c7826fd7c566

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:895`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.c9ae3a9d5fe4

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:604`.
- Existing tests: No reviewed association.
- Decision: `case InsertStatement insert:`.

## decision.statementwalker.ca408327f213

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:370`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, "!!", args, null, Verdict.Unknown, "runs an operating-system command");`.

## decision.statementwalker.ca81ee93b0f7

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:796`.
- Existing tests: No reviewed association.
- Decision: `var ids = legacy.Index?.Identifiers;`.

## decision.statementwalker.cab7d3f6e238

In UpdatesUsage, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1008`.
- Existing tests: No reviewed association.
- Decision: `if (parameter == null) return false;`.

## decision.statementwalker.caca601981cc

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:502`.
- Existing tests: No reviewed association.
- Decision: `WalkStatements(block.StatementList?.Statements, scope);`.

## decision.statementwalker.cb121b348534

In Save, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:105`.
- Existing tests: No reviewed association.
- Decision: `if (name != null)`.

## decision.statementwalker.cbdb644c7398

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:538`.
- Existing tests: No reviewed association.
- Decision: `WalkStatements(tryCatch.TryStatements?.Statements, scope);`.

## decision.statementwalker.cd1fef1f6746

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1076`.
- Existing tests: No reviewed association.
- Decision: `case JoinParenthesisTableReference parenthesis:`.

## decision.statementwalker.cebe228f6f9d

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:541`.
- Existing tests: No reviewed association.
- Decision: `if (afterTry.Peak > _transactions.Depth)`.

## decision.statementwalker.cf10b0aa63a2

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:802`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, dropIndex, "DROP INDEX", scope.TextOf(clause), null, Verdict.RealWrite, null);`.

## decision.statementwalker.d0d3f5a8f1b0

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:341`.
- Existing tests: No reviewed association.
- Decision: `.OrderBy(d => d.Offset)`.

## decision.statementwalker.d124beaf7316

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:187`.
- Existing tests: No reviewed association.
- Decision: `if (errors.Count == 0)`.

## decision.statementwalker.d183c6d829fb

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:861`.
- Existing tests: No reviewed association.
- Decision: `if (system && Classifier.WritingProcedures.TryGetValue(baseName, out var writes))`.

## decision.statementwalker.d219ce031bef

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:420`.
- Existing tests: No reviewed association.
- Decision: `_errors.Add(new SqlParseError(position.Line, position.Column, position.Offset, error.Number, error.Message));`.

## decision.statementwalker.d254e09ed4a7

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:339`.
- Existing tests: No reviewed association.
- Decision: `_errors.Any(e => e.Offset >= v.Offset && e.Offset <= v.Offset + v.Length));`.

## decision.statementwalker.d31f5b2041c2

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:694`.
- Existing tests: No reviewed association.
- Decision: `case ExecuteStatement execute:`.

## decision.statementwalker.d38dc1f6dea6

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:701`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, Classifier.ActionName(statement), string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.d3b826729a22

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:746`.
- Existing tests: No reviewed association.
- Decision: `modification is DeleteSpecification delete ? new RowFilter(delete.Target, delete.WhereClause, delete.FromClause, delete.TopRowFilter) : null;`.

## decision.statementwalker.d42f47ba42d9

In AddSqlCmdModeOffHint, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:340`.
- Existing tests: No reviewed association.
- Decision: `var first = commands.Concat(rejectedVariable != null ? new[] { rejectedVariable } : new SqlCmdPreprocessor.Directive[0])`.

## decision.statementwalker.d4685a20ef79

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:166`.
- Existing tests: No reviewed association.
- Decision: `? "IMPLICIT_TRANSACTIONS is ON: this opens a transaction that stays open after the script, holding its locks; nothing is permanent until COMMIT"`.

## decision.statementwalker.d477631eb0df

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:395`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.d477631eb0df.2

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:402`.
- Existing tests: No reviewed association.
- Decision: `return;`.

## decision.statementwalker.d4a75fd407d7

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1074`.
- Existing tests: No reviewed association.
- Decision: `case UnqualifiedJoin join:`.

## decision.statementwalker.d62bc568c479

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1066`.
- Existing tests: No reviewed association.
- Decision: `return true;`.

## decision.statementwalker.d68a7de540a1

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:382`.
- Existing tests: No reviewed association.
- Decision: `default:`.

## decision.statementwalker.d69a8e16f8c7

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:130`.
- Existing tests: No reviewed association.
- Decision: `var merged = (a.Depth >= b.Depth ? a : b).Copy();`.

## decision.statementwalker.d6cace3f0326

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:690`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, dbcc, dbccAction, string.Empty, null, dbccVerdict, dbccNote);`.

## decision.statementwalker.d7c45edb7771

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:944`.
- Existing tests: No reviewed association.
- Decision: `return result.HasValue ? !result.Value : (bool?)null;`.

## decision.statementwalker.d80c2fd184f5

In UpdatesUsage, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1010`.
- Existing tests: No reviewed association.
- Decision: `return literal == null || !(string.Equals(literal.Value, "false", StringComparison.OrdinalIgnoreCase) || literal.Value == "0");`.

## decision.statementwalker.d850a5977255

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:756`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, source.Reference, source.Action, scope.TextOf(source.Reference), null, Verdict.Unknown, source.Note);`.

## decision.statementwalker.d89b4bfcbbd4

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:567`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, commit, Classifier.ActionName(commit), string.Empty, null, Verdict.Unknown,`.

## decision.statementwalker.d8d4fdbbc867

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:645`.
- Existing tests: No reviewed association.
- Decision: `case CreateTableStatement createTable:`.

## decision.statementwalker.d965bedbbef7

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1139`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("no WHERE: the FROM has no inner join limiting the rows");`.

## decision.statementwalker.d9b66eb39171

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:305`.
- Existing tests: No reviewed association.
- Decision: `return region;`.

## decision.statementwalker.da1fad7eeb8d

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:995`.
- Existing tests: No reviewed association.
- Decision: `return string.Equals(database, "msdb", StringComparison.OrdinalIgnoreCase) && schema != null &&`.

## decision.statementwalker.da61185e4731

In AddAt, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1209`.
- Existing tests: No reviewed association.
- Decision: `if (!string.IsNullOrEmpty(note))`.

## decision.statementwalker.db5d157782b3

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:668`.
- Existing tests: No reviewed association.
- Decision: `case DropIndexStatement dropIndex:`.

## decision.statementwalker.db83e832cdcd

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:637`.
- Existing tests: No reviewed association.
- Decision: `case TruncateTableStatement truncate:`.

## decision.statementwalker.dc371e2d77b1

In IsSystemProcedure, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:992`.
- Existing tests: No reviewed association.
- Decision: `if (database == null) return true;`.

## decision.statementwalker.dcf2a19d39e8

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:495`.
- Existing tests: No reviewed association.
- Decision: `if (_transactions.Implicit && _transactions.Depth == 0 && !scope.BlockingErrorLine.HasValue && StartsImplicitTransaction(statement))`.

## decision.statementwalker.dcf2d0a09c6c

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:961`.
- Existing tests: No reviewed association.
- Decision: `case BooleanComparisonType.GreaterThan: value = first > second; break;`.

## decision.statementwalker.df4303196ee6

In IsSameTable, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1103`.
- Existing tests: No reviewed association.
- Decision: `if (source.Alias != null)`.

## decision.statementwalker.dfdd074db2b5

In AddNamedWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1193`.
- Existing tests: No reviewed association.
- Decision: `if (allRows)`.

## decision.statementwalker.dfe3fb6d12ff

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:525`.
- Existing tests: No reviewed association.
- Decision: `case BreakStatement _:`.

## decision.statementwalker.e00ad6909b71

In SplitBatches, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1312`.
- Existing tests: No reviewed association.
- Decision: `if (next < text.Length && text[next] == '\r')`.

## decision.statementwalker.e0a068e018d2

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:941`.
- Existing tests: No reviewed association.
- Decision: `if (expression is BooleanNotExpression negated)`.

## decision.statementwalker.e21b35aeb387

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:527`.
- Existing tests: No reviewed association.
- Decision: `case GoToStatement _:`.

## decision.statementwalker.e293850368de

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:793`.
- Existing tests: No reviewed association.
- Decision: `case BackwardsCompatibleDropIndexClause legacy:`.

## decision.statementwalker.e3c7d89f6cf5

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:699`.
- Existing tests: No reviewed association.
- Decision: `if (Classifier.IsReadOnlyStatement(statement))`.

## decision.statementwalker.e4ba6a21a06f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:652`.
- Existing tests: No reviewed association.
- Decision: `case AlterTableStatement alterTable:`.

## decision.statementwalker.e7c2bb1c672e

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:383`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, position, directive.Command, args, null, Verdict.ReadOnly, "SQLCMD command");`.

## decision.statementwalker.e7fbe9c966e7

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1124`.
- Existing tests: No reviewed association.
- Decision: `if (filter != null && !filter.HasWhere)`.

## decision.statementwalker.e825708d1fc9

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:168`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, _transactions.OpenedAt.Value, "OPEN TRANSACTION", string.Empty, null, Verdict.Unknown, note);`.

## decision.statementwalker.e849d76842b8

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:967`.
- Existing tests: No reviewed association.
- Decision: `if (answer.HasValue && answer.Value != value) return null;`.

## decision.statementwalker.e918b276866a

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:716`.
- Existing tests: No reviewed association.
- Decision: `if (statement is ProcedureStatementBody procedureDefinition && Classifier.IsTemp(procedureDefinition.ProcedureReference?.Name, _options))`.

## decision.statementwalker.eb2d28ab7f4f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:671`.
- Existing tests: No reviewed association.
- Decision: `case CreateStatisticsStatement createStatistics:`.

## decision.statementwalker.eb7da19d3756

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:618`.
- Existing tests: No reviewed association.
- Decision: `case DeleteStatement delete:`.

## decision.statementwalker.eb9fc55bcd36

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1170`.
- Existing tests: No reviewed association.
- Decision: `notes.Add(baseTarget.Note);`.

## decision.statementwalker.ec8a0cef3700

In FinishTransactions, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:160`.
- Existing tests: No reviewed association.
- Decision: `AddAt(scope, scope.PositionOfOffset(0, 0), "TRANSACTION FLOW", string.Empty, null, Verdict.Unknown,`.

## decision.statementwalker.edebadd8bd4e

In ReportParseErrors, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:413`.
- Existing tests: No reviewed association.
- Decision: `if (offset > lastCode && lastCode >= 0)`.

## decision.statementwalker.ee2692b3224b

In Rollback, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:97`.
- Existing tests: No reviewed association.
- Decision: `if (name != null && SavePoints.Contains(name))`.

## decision.statementwalker.ef6427d69e0d

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:689`.
- Existing tests: No reviewed association.
- Decision: `var dbccAction = "DBCC " + (written.Success ? written.Groups[1].Value.ToUpperInvariant() : dbcc.Command.ToString().ToUpperInvariant());`.

## decision.statementwalker.efb81354f6bc

In AnalyzeText, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:208`.
- Existing tests: No reviewed association.
- Decision: `if (batchErrors.Count == 0)`.

## decision.statementwalker.f097dd178e0c

In AddNamedWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1194`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("removes every row");`.

## decision.statementwalker.f0b630e5d5e2

In AddSqlCmdDirectives, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:372`.
- Existing tests: No reviewed association.
- Decision: `case ":setvar":`.

## decision.statementwalker.f1600b51b11e

In GuessTarget, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1243`.
- Existing tests: No reviewed association.
- Decision: `case DropObjectsStatement drop when drop.Objects != null:`.

## decision.statementwalker.f178e208905c

In WalkStatement, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:474`.
- Existing tests: No reviewed association.
- Decision: `if (++_statementDepth > MaxStatementDepth)`.

## decision.statementwalker.f26ea6f7439d

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:846`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, statement, action, target, null, Verdict.Unknown, "dynamic SQL is built at runtime");`.

## decision.statementwalker.f39f730ed385

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1140`.
- Existing tests: No reviewed association.
- Decision: `else if (baseTarget.Via.Count > 0)`.

## decision.statementwalker.f3ca900bd1e3

In EvaluateTransactionPredicate, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:950`.
- Existing tests: No reviewed association.
- Decision: `if (!guard || left == null || right == null) return null;`.

## decision.statementwalker.f4370f634501

In RestrictsRows, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1075`.
- Existing tests: No reviewed association.
- Decision: `return join.UnqualifiedJoinType == UnqualifiedJoinType.CrossApply || RestrictsRows(join.FirstTableReference);`.

## decision.statementwalker.f558929e3f3a

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:551`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, tryCatch, "TRANSACTION FLOW", string.Empty, null, Verdict.Unknown,`.

## decision.statementwalker.f7323b1cc370

In Merge, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:124`.
- Existing tests: No reviewed association.
- Decision: `var continuing = (a.Terminated ? b : a).Copy();`.

## decision.statementwalker.f75a4585a86e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:721`.
- Existing tests: No reviewed association.
- Decision: `if (statement is DropProcedureStatement dropProcedures)`.

## decision.statementwalker.f7ddb3d2993b

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:321`.
- Existing tests: No reviewed association.
- Decision: `yield return current;`.

## decision.statementwalker.f7ddb3d2993b.2

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:326`.
- Existing tests: No reviewed association.
- Decision: `yield return current;`.

## decision.statementwalker.f835f189a77f

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:518`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, returned, "RETURN", string.Empty, null, Verdict.ReadOnly, null);`.

## decision.statementwalker.f8b5ce621c0e

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:640`.
- Existing tests: No reviewed association.
- Decision: `case BulkInsertBase bulkInsert:`.

## decision.statementwalker.f8ea7cf35f86

In AddHiddenSideEffects, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:747`.
- Existing tests: No reviewed association.
- Decision: `AddDmlWrite(scope, modification, modification is DeleteSpecification ? "DELETE" : modification is UpdateSpecification ? "UPDATE" :`.

## decision.statementwalker.f9752b4f2a48

In WalkDropIndex, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:798`.
- Existing tests: No reviewed association.
- Decision: `Add(scope, dropIndex, "DROP INDEX", scope.TextOf(legacy.Index), null, temp ? Verdict.TempOnly : Verdict.RealWrite, null);`.

## decision.statementwalker.f9b50d4a5926

In RecordSkipped, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:293`.
- Existing tests: No reviewed association.
- Decision: `.Where(t => t.TokenType != TSqlTokenType.WhiteSpace && t.TokenType != TSqlTokenType.SingleLineComment &&`.

## decision.statementwalker.fa3d26fe33bc

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:684`.
- Existing tests: No reviewed association.
- Decision: `case DbccStatement dbcc:`.

## decision.statementwalker.fad99ad06d90

In CheckBatchRepetition, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:447`.
- Existing tests: No reviewed association.
- Decision: `if (tokens == null) return;`.

## decision.statementwalker.fb50ed3d472d

In WalkStatementCore, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:579`.
- Existing tests: No reviewed association.
- Decision: `_transactions.Rollback(rollback.Name?.Identifier?.Value ?? (rollback.Name?.ValueExpression as Literal)?.Value);`.

## decision.statementwalker.fc577780e5ac

In Commit, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:88`.
- Existing tests: No reviewed association.
- Decision: `if (Depth > 0)`.

## decision.statementwalker.fcf19ff848e9

In AddDmlWrite, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1172`.
- Existing tests: No reviewed association.
- Decision: `notes.Add("matches allow-list");`.

## decision.statementwalker.fd7c1aa4d918

In WalkExecute, select the branch or fallback shown in sourceExpression; it can affect downstream verdict, note, target or traversal.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: needs-review.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:880`.
- Existing tests: No reviewed association.
- Decision: `if (strings.Strings.Count > 0 && strings.Strings.All(s => s is StringLiteral))`.

## exec.dispatch

Remote/variable execution is Unknown; qualify system names before built-in writer/safe tables or trusted procedures.

- Outcomes: ReadOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:808`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:226 (LinkedServerExec_IsUnknown); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:134 (ProcedureNameInVariable_IsUnknown); tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84 (ProcedureTrustRequiresResolvedQualification)

## exec.dynamic

Literal strings and literal concatenation recurse with outer EXEC location; runtime text and excess nesting warn Unknown.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1013`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:101 (DynamicSqlConcatenatedLiterals_AreAnalyzed); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:108 (DynamicSqlWithVariablePart_IsUnknown); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:115 (NestedDynamicSql_IsLimitedByDepth)

## exec.empty

No findings from literal dynamic text yields empty-dynamic-SQL green finding.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1025`.
- Existing tests: No reviewed association.

## exec.linked

EXEC AT linked server is opaque even when text is literal.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:817`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:226 (LinkedServerExec_IsUnknown)

## exec.missing

Missing executable specification cannot be analyzed and warns.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:811`.
- Existing tests: No reviewed association.

## exec.namespace

System dispatch permits local sys/dbo and qualified master; explicit msdb supports the enumerated Agent/mail/history names only.

- Outcomes: ReadOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:985`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:141 (SchemaQualifiedReadOnlyProc_IsReadOnly)

## exec.ordinary

Unrecognized procedure calls warn; INSERT EXEC warning is in addition to destination write.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:874`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:206 (UnknownProcedure_StaysYellow); tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:120 (Case13_InsertExec_IsUnknown)

## exec.sp-executesql

Only qualified system sp_executesql with literal first/@stmt argument recurses; runtime argument warns.

- Outcomes: ReadOnly, TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:839`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:86 (SpExecuteSqlWithLiteral_IsAnalyzed); tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:93 (Case10_SpExecuteSqlWithVariable_IsUnknown)

## exec.spaceused

sp_spaceused updateusage absent/false/0 is green; unknown or enabled usage correction is red.

- Outcomes: ReadOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1002`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:75 (RealWritesAreNeverGreen); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:107 (OrdinaryReadsRemainReadOnly)

## exec.streaming-wrapper

Known W1-001: BEGIN END wrapping sp_create_streaming_job can turn red into yellow without parse errors.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:808`.
- Existing tests: tests/SqlSafetyGuard.Tests/MetamorphicVerificationTests.cs:234 (KnownWaveOneStreamingWrapperDiscrepancy)

## exec.trusted

A qualified trusted-procedure pattern grants ReadOnly after built-in writer overrides. Body not inspected.

- Outcomes: ReadOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:867`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:84 (ProcedureTrustRequiresResolvedQualification)

## sqlcmd.mode-off

When mode is off, recognizable SQLCMD syntax adds a mode-off hint at first actual syntax occurrence.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:346`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:337 (ModeOff_SqlCmdLinesAreSyntaxErrors_WithHint); tests/SqlSafetyGuard.Tests/RecoveryTests.cs:362 (ModeOff_DollarParenInAString_IsJustAString)

## sqlcmd.walk

connect invalidates target context; include/OS/query-exit and variable expansion warn; ordinary directives and plain exit/quit are green.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:351`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:307 (Directives_AreReported)

## txn.begin

Track nested depth and highest opening position; implicit mode can add an extra nesting level.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:70`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:54 (BalancedTransaction_IsGreen); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:62 (NestedTransactions_Balanced)

## txn.commit

Commit decreases tracked depth; walker warns for lone or extra unguarded commits.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:86`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:39 (LoneCommit_IsUnknown); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:47 (ExtraCommit_IsUnknown)

## txn.finish

Warn for open or uncertain transactions at end, including implicit mode when work opened a transaction.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:157`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:68 (BeginWithoutCommit_ReportsOpenTransaction); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:148 (ImplicitTransactions_LeftOpen_IsReported)

## txn.if

Both IF branches are analyzed; TRANCOUNT/XACT_STATE predicates refine transaction path state.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:907`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:120 (GuardedCommit_IsNotLone); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:127 (XactStateGuard_ClosesTheTransaction)

## txn.implicit-start

Only enumerated operations start an implicit transaction; named SELECT tables are checked.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:139`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:157 (ImplicitTransactions_Committed_IsFine)

## txn.merge

Merge branch depth, peaks, implicit and uncertainty; terminated paths are handled separately.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:117`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:96 (IfCommitElseRollback_IsBalanced); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:134 (UnguardedIf_StaysConservative)

## txn.rollback

Full rollback clears depth; known savepoint rollback preserves the open transaction.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:95`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:81 (RollbackCloses); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:89 (RollbackToSavepoint_KeepsTransactionOpen)

## walk.blocked-transaction

A syntactically blocked batch must not change tracked transaction state.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:495`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:165 (TransactionsInBrokenBatch_DontCount)

## walk.catch-exit

CATCH termination with open or uncertain transaction adds TRANSACTION FLOW warning.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:550`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:160 (PossiblyOpenTransactionsAreReported)

## walk.composable

Composable nested DML adds independent base-target and OUTPUT INTO findings.

- Outcomes: TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:741`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:83 (NestedModificationOutputIsAlsoClassified)

## walk.depth

Statement recursion exceeding 1000 terminates analysis conservatively.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:470`.
- Existing tests: No reviewed association.

## walk.dml-targets

Resolve each DML base; unresolved target is Unknown, remote base RealWrite, table variable TempOnly.

- Outcomes: TempOnly, AllowListed, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1114`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:220 (RemoteTargets_AreRealWrite); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:35 (UpdateViaAliasOfTemp_IsTempOnly)

## walk.drop-index

Legacy index names use the table portion for temporary classification, not the index name.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:784`.
- Existing tests: No reviewed association.

## walk.drop-procedures

DROP PROCEDURE classifies each object separately and disallows table allow-list.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:721`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180 (TableDdlAndTruncate)

## walk.drop-statistics

For table.statistics names use penultimate identifier to detect temporary tables.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:674`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:28 (DropStatistics_FollowsTempRules)

## walk.fallback

All unrecognized statement kinds default RealWrite; module definitions receive body-not-run note.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:727`.
- Existing tests: tests/SqlSafetyGuard.Tests/AstOnlyKindTests.cs:20 (ConstructedAstUsesReviewedFallbackWithoutClaimingSqlExecution)

## walk.guess-target

Fallback extracts a module/drop/name/database target via ordered reflection; failing getters are ignored.

- Outcomes: RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1237`.
- Existing tests: No reviewed association.

## walk.hidden

Scan execution expressions for remote sources and sequence increments; skip already-counted write fragments.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:735`.
- Existing tests: tests/SqlSafetyGuard.Tests/HiddenSideEffectTests.cs:30 (RemoteSources_AreUnknown)

## walk.insert-exec

INSERT EXEC combines destination verdict with execution effects; nested composable DML is traversed.

- Outcomes: TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:766`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:147 (InsertExecOfReadOnlyProc_IsTempOnly); tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:212 (InsertExecOfKnownWriter_IsRed)

## walk.module

Classify module definition itself; do not execute or independently scan stored body statements.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1234`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:197 (DefinitionAndSecurityChanges_AreRealWrite)

## walk.named-target

Named DDL/TRUNCATE/INTO/bulk targets use resolved qualified names and eligibility flags.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1189`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:180 (TableDdlAndTruncate)

## walk.output

OUTPUT INTO writes its own target independently of the main DML target.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1181`.
- Existing tests: tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:65 (Case07_OutputIntoPermanentTable_IsRealWrite); tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:83 (NestedModificationOutputIsAlsoClassified)

## walk.parse

Top-level parse failures recover bounded batches; literal dynamic parse failure remains Unknown.

- Outcomes: Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:179`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:232 (ParseErrorInOneBatch_OtherBatchesStillAnalyzed); tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:126 (UnparseableDynamicSql_IsUnknown)

## walk.repeat

A GO repetition count other than one requires warning; do not assume a single execution.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:445`.
- Existing tests: No reviewed association.

## walk.return

RETURN marks current transaction path terminated, but does not suppress classification of later writes.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:517`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:340 (EarlyReturnPreservesContinuingTransactionPaths)

## walk.rowfilter

WHERE, non-percent TOP, restricting joins and CTE/derived resolution change all-rows notes; percent TOP and preserved outer sides do not establish restriction.

- Outcomes: annotation only; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:1032`.
- Existing tests: tests/SqlSafetyGuard.Tests/RecoveryTests.cs:232 (AllRowsSummary_DescribesTheActualChange)

## walk.select-into

SELECT INTO classifies its destination; plain SELECT begins ReadOnly then scans hidden effects.

- Outcomes: TempOnly, AllowListed, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:598`.
- Existing tests: tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:57 (Case06_SelectIntoPermanentTable_IsRealWrite); tests/SqlSafetyGuard.Tests/RequiredCasesTests.cs:20 (Case02_SelectIntoTempThenDeleteTemp_IsTempOnly)

## walk.switch-both

ALTER TABLE SWITCH classifies source and destination independently, without allow-list.

- Outcomes: TempOnly, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:648`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:15 (SwitchClassifiesBothTables)

## walk.target-switch

USE or federation/context switch invalidates contextual allow lists and trusted procedures for following statements.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:437`.
- Existing tests: tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:99 (TargetSwitchesDisableContextualTrust); tests/SqlSafetyGuard.Tests/EnvironmentRuleTests.cs:184 (FederationSwitchInvalidatesTargetContext)

## walk.temp-procedure

A temporary procedure definition is TempOnly; its stored body does not execute now.

- Outcomes: TempOnly; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:716`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:186 (TemporaryProcedureDefinitionsDoNotExecuteBodies)

## walk.throw

THROW terminates the transaction path; following statement findings remain conservatively included.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:521`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:177 (TransactionUncertaintyIsNotGreen)

## walk.transfer

BREAK/CONTINUE/GOTO are Unknown while a transaction or implicit mode may skip cleanup, otherwise ReadOnly.

- Outcomes: ReadOnly, Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:525`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:177 (TransactionUncertaintyIsNotGreen)

## walk.try-peak

CATCH begins with potential peak depth from TRY, including a TRY that later committed.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:541`.
- Existing tests: tests/SqlSafetyGuard.Tests/CoverageFixTests.cs:113 (CommitInCatch_AfterTryOpened_IsNotLone)

## walk.waitfor

WAITFOR with an embedded statement recursively uses that statement verdict.

- Outcomes: ReadOnly, TempOnly, Unknown, RealWrite; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:592`.
- Existing tests: tests/SqlSafetyGuard.Tests/SoundnessRegressionTests.cs:31 (WaitForPreservesInnerStatementVerdict)

## walk.while-zero

Merge loop state with pre-loop state to account for zero iterations and scan loop predicate effects.

- Outcomes: Unknown; review: reviewed-semantic.
- Source: `src/SqlSafetyGuard.Analyzer/StatementWalker.cs:509`.
- Existing tests: tests/SqlSafetyGuard.Tests/AdditionalAnalyzerTests.cs:153 (WhileAndTryCatch_AreWalked)

## writing-procedure.sp_add_job

WritingProcedures contains sp_add_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:85`.
- Existing tests: No reviewed association.

## writing-procedure.sp_add_jobschedule

WritingProcedures contains sp_add_jobschedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:91`.
- Existing tests: No reviewed association.

## writing-procedure.sp_add_jobserver

WritingProcedures contains sp_add_jobserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:94`.
- Existing tests: No reviewed association.

## writing-procedure.sp_add_jobstep

WritingProcedures contains sp_add_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:88`.
- Existing tests: No reviewed association.

## writing-procedure.sp_add_schedule

WritingProcedures contains sp_add_schedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:92`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addextendedproc

WritingProcedures contains sp_addextendedproc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:65`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addextendedproperty

WritingProcedures contains sp_addextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:62`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addlinkedserver

WritingProcedures contains sp_addlinkedserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:57`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addlinkedsrvlogin

WritingProcedures contains sp_addlinkedsrvlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:59`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addlogin

WritingProcedures contains sp_addlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:43`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addrolemember

WritingProcedures contains sp_addrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:39`.
- Existing tests: No reviewed association.

## writing-procedure.sp_addsrvrolemember

WritingProcedures contains sp_addsrvrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:41`.
- Existing tests: No reviewed association.

## writing-procedure.sp_adduser

WritingProcedures contains sp_adduser; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:47`.
- Existing tests: No reviewed association.

## writing-procedure.sp_attach_db

WritingProcedures contains sp_attach_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:77`.
- Existing tests: No reviewed association.

## writing-procedure.sp_attach_schedule

WritingProcedures contains sp_attach_schedule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:93`.
- Existing tests: No reviewed association.

## writing-procedure.sp_attach_single_file_db

WritingProcedures contains sp_attach_single_file_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:78`.
- Existing tests: No reviewed association.

## writing-procedure.sp_bindefault

WritingProcedures contains sp_bindefault; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:69`.
- Existing tests: No reviewed association.

## writing-procedure.sp_bindrule

WritingProcedures contains sp_bindrule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:67`.
- Existing tests: No reviewed association.

## writing-procedure.sp_change_users_login

WritingProcedures contains sp_change_users_login; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:54`.
- Existing tests: No reviewed association.

## writing-procedure.sp_changedbowner

WritingProcedures contains sp_changedbowner; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:55`.
- Existing tests: No reviewed association.

## writing-procedure.sp_changeobjectowner

WritingProcedures contains sp_changeobjectowner; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:56`.
- Existing tests: No reviewed association.

## writing-procedure.sp_configure

WritingProcedures contains sp_configure; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:34`.
- Existing tests: No reviewed association.

## writing-procedure.sp_createstats

WritingProcedures contains sp_createstats; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:99`.
- Existing tests: No reviewed association.

## writing-procedure.sp_cycle_errorlog

WritingProcedures contains sp_cycle_errorlog; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:82`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dboption

WritingProcedures contains sp_dboption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:80`.
- Existing tests: No reviewed association.

## writing-procedure.sp_defaultdb

WritingProcedures contains sp_defaultdb; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:46`.
- Existing tests: No reviewed association.

## writing-procedure.sp_delete_backuphistory

WritingProcedures contains sp_delete_backuphistory; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:83`.
- Existing tests: No reviewed association.

## writing-procedure.sp_delete_job

WritingProcedures contains sp_delete_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:87`.
- Existing tests: No reviewed association.

## writing-procedure.sp_delete_jobstep

WritingProcedures contains sp_delete_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:90`.
- Existing tests: No reviewed association.

## writing-procedure.sp_denylogin

WritingProcedures contains sp_denylogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:53`.
- Existing tests: No reviewed association.

## writing-procedure.sp_detach_db

WritingProcedures contains sp_detach_db; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:79`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dropextendedproc

WritingProcedures contains sp_dropextendedproc; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:66`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dropextendedproperty

WritingProcedures contains sp_dropextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:64`.
- Existing tests: No reviewed association.

## writing-procedure.sp_droplinkedsrvlogin

WritingProcedures contains sp_droplinkedsrvlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:60`.
- Existing tests: No reviewed association.

## writing-procedure.sp_droplogin

WritingProcedures contains sp_droplogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:44`.
- Existing tests: No reviewed association.

## writing-procedure.sp_droprolemember

WritingProcedures contains sp_droprolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:40`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dropserver

WritingProcedures contains sp_dropserver; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:58`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dropsrvrolemember

WritingProcedures contains sp_dropsrvrolemember; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:42`.
- Existing tests: No reviewed association.

## writing-procedure.sp_dropuser

WritingProcedures contains sp_dropuser; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:48`.
- Existing tests: No reviewed association.

## writing-procedure.sp_grantdbaccess

WritingProcedures contains sp_grantdbaccess; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:49`.
- Existing tests: No reviewed association.

## writing-procedure.sp_grantlogin

WritingProcedures contains sp_grantlogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:51`.
- Existing tests: No reviewed association.

## writing-procedure.sp_msforeachdb

WritingProcedures contains sp_MSforeachdb; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:102`.
- Existing tests: No reviewed association.

## writing-procedure.sp_msforeachtable

WritingProcedures contains sp_MSforeachtable; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:101`.
- Existing tests: No reviewed association.

## writing-procedure.sp_oacreate

WritingProcedures contains sp_OACreate; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:37`.
- Existing tests: No reviewed association.

## writing-procedure.sp_oamethod

WritingProcedures contains sp_OAMethod; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:38`.
- Existing tests: No reviewed association.

## writing-procedure.sp_password

WritingProcedures contains sp_password; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:45`.
- Existing tests: No reviewed association.

## writing-procedure.sp_procoption

WritingProcedures contains sp_procoption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:73`.
- Existing tests: No reviewed association.

## writing-procedure.sp_purge_jobhistory

WritingProcedures contains sp_purge_jobhistory; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:84`.
- Existing tests: No reviewed association.

## writing-procedure.sp_recompile

WritingProcedures contains sp_recompile; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:76`.
- Existing tests: No reviewed association.

## writing-procedure.sp_refreshsqlmodule

WritingProcedures contains sp_refreshsqlmodule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:75`.
- Existing tests: No reviewed association.

## writing-procedure.sp_refreshview

WritingProcedures contains sp_refreshview; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:74`.
- Existing tests: No reviewed association.

## writing-procedure.sp_rename

WritingProcedures contains sp_rename; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:33`.
- Existing tests: No reviewed association.

## writing-procedure.sp_resetstatus

WritingProcedures contains sp_resetstatus; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:81`.
- Existing tests: No reviewed association.

## writing-procedure.sp_revokedbaccess

WritingProcedures contains sp_revokedbaccess; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:50`.
- Existing tests: No reviewed association.

## writing-procedure.sp_revokelogin

WritingProcedures contains sp_revokelogin; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:52`.
- Existing tests: No reviewed association.

## writing-procedure.sp_send_dbmail

WritingProcedures contains sp_send_dbmail; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:36`.
- Existing tests: No reviewed association.

## writing-procedure.sp_serveroption

WritingProcedures contains sp_serveroption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:61`.
- Existing tests: No reviewed association.

## writing-procedure.sp_settriggerorder

WritingProcedures contains sp_settriggerorder; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:72`.
- Existing tests: No reviewed association.

## writing-procedure.sp_start_job

WritingProcedures contains sp_start_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:95`.
- Existing tests: No reviewed association.

## writing-procedure.sp_stop_job

WritingProcedures contains sp_stop_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:96`.
- Existing tests: No reviewed association.

## writing-procedure.sp_tableoption

WritingProcedures contains sp_tableoption; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:71`.
- Existing tests: No reviewed association.

## writing-procedure.sp_trace_create

WritingProcedures contains sp_trace_create; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:97`.
- Existing tests: No reviewed association.

## writing-procedure.sp_trace_setstatus

WritingProcedures contains sp_trace_setstatus; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:98`.
- Existing tests: No reviewed association.

## writing-procedure.sp_unbindefault

WritingProcedures contains sp_unbindefault; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:70`.
- Existing tests: No reviewed association.

## writing-procedure.sp_unbindrule

WritingProcedures contains sp_unbindrule; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:68`.
- Existing tests: No reviewed association.

## writing-procedure.sp_update_job

WritingProcedures contains sp_update_job; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:86`.
- Existing tests: No reviewed association.

## writing-procedure.sp_update_jobstep

WritingProcedures contains sp_update_jobstep; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:89`.
- Existing tests: No reviewed association.

## writing-procedure.sp_updateextendedproperty

WritingProcedures contains sp_updateextendedproperty; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:63`.
- Existing tests: No reviewed association.

## writing-procedure.sp_updatestats

WritingProcedures contains sp_updatestats; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:100`.
- Existing tests: No reviewed association.

## writing-procedure.xp_cmdshell

WritingProcedures contains xp_cmdshell; classify as RealWrite after specialized dispatch/qualification overrides.

- Outcomes: RealWrite; review: reviewed-table.
- Source: `src/SqlSafetyGuard.Analyzer/Classifier.cs:35`.
- Existing tests: No reviewed association.
