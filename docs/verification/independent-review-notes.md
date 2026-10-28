# Independent safety review — 2026-10-28

## Scope and independence

Base: main `3fff9b0a1280f2ec65fb44e5c1cedeb593ed4b46`; delivery branch: `verify/independent-cases`. Designed independently from analyzer source and the permitted rule inventory, contract, policy and world/schema documents. Did not read Variants.cs, preexisting Campaign/cases files, or Regression/Adversarial test files. No SQL Server connection, campaign execution or subagent was used. Analyzer source is unchanged. All effects below are **expected hypotheses, not server observations**. The desired contract, not current green opt-ins, defines the challenge.

## Corpus and execution handoff

600 unique cases, 20 families, 284 distinct valid rule IDs. Every row includes SQL, setup (possibly empty), database, principal, seed, family, ruleIds and expectedServerEffect. No expectedClassification or forced controlVerdict is supplied: server evidence decides. All newly designed cases are retained, including conservative results, likely semantic rejections and parser negatives.

- `cases/independent/shared/`: **432** cases intended for the owned four-database shared campaign context.
- `cases/independent/isolated/`: **109** cases, all privileged=true; only the verified disposable isolated container, one worker. Includes system/tempdb/global/linked-server and BULK probes.
- `cases/independent/offline/`: **59** SQLCMD, stress, lexical-mode and environment-only probes. Do not send these to the campaign runner unchanged.

**Never pass the independent root to a recursive runner.** Select lanes separately, review privileged setup and feature prerequisites, and serialize campaigns. This work does not start or schedule tonight's run. The documented runner ignores custom metadata: executionLane, validity, expectedServerEffect, localProbeOptions and localObservation. Directory selection and privileged=true enforce the meaningful separation here.

**58 cases need nondefault analyzer options for their configured verdicts.** localProbeOptions is an explicit handoff specification, not a supported campaign option. Running these under the current default-only runner tests only their recorded default verdict. Apply matching settings in a separately supported execution path before claiming a configured false green; do not silently compare configured analysis with default execution. Environment-rule resolution is offline.

## Local observation method

Scratch console outside the repository, .NET SDK 10.0.403, net10.0, project reference to the unchanged analyzer, ScriptDom package 180.140.0 / TSql170Parser(true). Expanded {{Core}}/{{Sales}}/{{Audit}}/{{Archive}} to SgIndependent_Core/Sales/Audit/Archive, and {{seed}} to 171. Default means Analyze(sql, null); configured means the exact row's localProbeOptions. Dynamic depth defaults to 3. Each normal Analyze call had a 20-second cancellation token; the auxiliary environment decision call used the analyzer default token. No server binding/semantic validation was performed.

**658 completed local runs:** 600 default + 58 configured. Default: ReadOnly 275, TempOnly 42, AllowListed 0, Unknown 88, RealWrite 195. Configured: ReadOnly 8, TempOnly 1, AllowListed 21, Unknown 13, RealWrite 15. No caught exceptions, cancellation or crash. Each row's localObservation preserves verdict, elapsed milliseconds, parse errors, finding count, first eight findings and environment decision where relevant. A cap on findings avoids duplicating thousands of stress statements; it is explicitly not the full result. SQL remains unexpanded in the case row for runtime substitution.

17 distinct setup scripts parse cleanly with TSql170Parser. This does not validate dynamic CREATE bodies, object existence, permissions, provider setup or SQL Server semantics. 11 default cases and four additional configured SQLCMD runs have parse errors, enumerated below. Parser-negative cases are inconclusive for false-green claims.

## Family results

Counts are **ReadOnly / TempOnly / AllowListed / Unknown / RealWrite**. Configured counts are additional runs, not additional cases.

| Family | Cases | Default counts | Configured counts |
| --- | ---: | --- | --- |
| metadata | 93 | 70/0/0/2/21 | — |
| dbcc | 43 | 28/1/0/0/14 | — |
| set-options | 83 | 77/0/0/4/2 | — |
| sequences | 22 | 2/2/0/0/18 | — |
| temporary | 39 | 1/32/0/1/5 | — |
| resolution | 25 | 0/1/0/1/23 | — |
| dynamic | 66 | 17/5/0/23/21 | 0/0/0/0/1 |
| session | 29 | 26/0/0/3/0 | — |
| cursors | 15 | 12/0/0/0/3 | — |
| transactions | 25 | 7/0/0/13/5 | — |
| persistent-targets | 17 | 0/0/0/0/17 | — |
| broker | 9 | 1/0/0/2/6 | — |
| physical | 13 | 1/1/0/1/10 | — |
| remote | 12 | 1/0/0/9/2 | — |
| allow-list | 30 | 0/0/0/0/30 | 0/1/20/0/9 |
| trust | 8 | 0/0/0/8/0 | 5/0/0/3/0 |
| environment | 9 | 1/0/0/4/4 | 0/0/1/5/3 |
| lexical | 22 | 9/0/0/4/9 | — |
| sqlcmd | 26 | 16/0/0/7/3 | 3/0/0/5/2 |
| stress | 14 | 6/0/0/6/2 | — |

### metadata

Every safe-procedure inventory entry has a target, with schema/database qualification, named arguments, metadata lookups, exact/quick statistics and msdb writing-procedure controls. sp_monitor is the strongest default persistent-state suspect; procedure-name membership alone does not establish parameter safety.

### dbcc

All 28 safe DBCC names, CHECKIDENT NORESEED versus default/reseed, quoted repair arguments, SQLPERF CLEAR versus read, variable CLEAR, session/server trace flags and repair/usage controls. Several legacy/undocumented commands may be unsupported on the execution server.

### set-options

All reviewed SET option/family targets, ON/OFF forms, isolation levels, implicit transactions, ANSI defaults, statistics/showplan switches, identity insertion and session lifetime. Many green classifications intentionally challenge the no-session-residue contract; obsolete forms are retained as parser-negative controls.

### sequences

NEXT VALUE FOR in SELECT, DECLARE, SET, PRINT, predicates, ORDER BY, temp/table-variable INSERT, defaults, cross-database SQL, rollback and dynamic literals. Direct legal sequence expressions are generally RealWrite; green RETURN and temp-table-default candidates need semantic acceptance checks.

### temporary

Local # and global ## DDL/DML, table variables, temp procedures/modules, mixed temp/permanent statements, aliases/CTEs, SELECT INTO, OUTPUT and composable nested DML. Global objects are isolated despite TempOnly because they are shared, not session-local.

### resolution

Bracketed # aliases and CTE names, permanent tables reached through aliases, same-named table variables, dbo/app defaults, tempdb permanent objects, synonyms, cross-database names and alias/OUTPUT destinations. Compare runtime base identities with finding targets; parser success is not binding proof.

### dynamic

Literal EXEC/sp_executesql, nested EXEC, concatenation, variables, REPLACE/QUOTENAME construction, temp/module bodies, sequence allocation, monitor calls and configured depth boundaries. Unresolved construction should remain Unknown rather than be assumed inert.

### session

EXECUTE AS/REVERT, SETUSER, application locks and unreachable release, session context/read-only keys, current database, open keys, RAISERROR/THROW, retained options and cursors. WITH LOG challenges persistent logging separately from business-table mutation.

### cursors

Read-only versus FOR UPDATE declarations, OPEN/FETCH/CLOSE/DEALLOCATE, local/global lifetime, permanent and temp positioned writes with WHERE CURRENT OF. Standalone cleanup and unsupported cursor combinations may fail execution and remain inconclusive.

### transactions

BEGIN/COMMIT/ROLLBACK, IF branches, RETURN/GOTO bypasses, TRY/CATCH, THROW/RAISERROR and XACT_ABORT paths; sequence/identity side effects and work left open. Observe before disposal or rollback; syntactic rollback is not proof that all effects were undone.

### persistent-targets

Positive write/target controls on base tables, aliases, views, INSTEAD OF triggers, synonyms, temporal tables, partitioned tables and permissions. All 17 default results are RealWrite; finding identity can still differ from physical mutation destinations.

### broker

SEND, RECEIVE, END CONVERSATION, GET CONVERSATION GROUP, bounded WAITFOR and prequeued self-dialog fixtures. Valid RECEIVE INTO table-variable controls accompany the intentional invalid temp-table destination. Asynchronous delivery and conversation locks require extra oracle care.

### physical

Identity, explicit statistics, index/partition operations, BULK INSERT and OPENROWSET(BULK) into read/temp/permanent destinations. BULK uses deliberately absent files: capability/parser probes, not executable end-to-end import fixtures.

### remote

Isolated SgLoop OPENQUERY reads/writes/sequences, EXEC AT, four-part SELECT/DML/procedure calls, OPENROWSET and OPENDATASOURCE. Providers/authentication/result-set shape can prevent execution; never count such rejection as safety.

### allow-list

Exact/wildcard and malformed patterns, 1–4-part identities, mismatched default-schema context, OUTPUT, tempdb opt-in, trigger/cascade/temporal/sequence-default effects and synonyms. Configured greens authorize spellings without proving additional resolved-effect authorization.

### trust

Trusted procedure patterns, exact/schema identities, writing/sequence fixtures and conservative default results. Configured ReadOnly from a deliberately false administrator assertion is a contract limitation, not independent proof of a classifier defect.

### environment

Matching/nonmatching rules, first-match precedence, warning-mode suppression, missing context and invalid-rule/configuration warnings. Classification and ShouldWarn are recorded separately; Off or Red mode suppressing a yellow prompt is not green.

### lexical

Quoted identifiers, strings/comments containing separators/write keywords, nested comments, escaped quotes, malformed input and writes before/after invalid text. Intentional parse errors test fail-closed behavior, not valid T-SQL writes.

### sqlcmd

setvar/substitution, unset variables, disabled SQLCMD mode, GO repetition, commands and separators in strings/comments. Offline only: runner supports plain T-SQL/nonrepeated GO and must not execute a different expansion than the analyzer.

### stress

6,000 SELECTs, long CTE chains and nested blocks, long strings/comments and budget/depth boundaries. These are bounded offline availability samples, not a performance benchmark or a proof against stack/memory exhaustion.

## Ranked candidate queue

Ranking prioritizes likely persistent/externally visible violations, followed by shared/session residue. Configured candidates are conditional on the options stated. The SQL snippets are exact case SQL; setup and context must accompany reproduction. None is a confirmed server defect.

### 1. ind-metadata-monitor-master

- Local verdict: **ReadOnly** (default). Lane: isolated; context: Core/runner.
- Exact case SQL:

```sql
EXEC master.dbo.sp_monitor;
```

- Expected effect: Hypothesis: master.dbo.spt_monitor refreshed; requires isolated observation of master.
- Reasoning / limitation: A supposedly safe procedure may deliberately refresh master.dbo.spt_monitor. Highest-priority persistent system-table hypothesis; inspect master in the isolated container, not only the four restored databases.

### 2. ind-session-raise-log

- Local verdict: **ReadOnly** (default). Lane: isolated; context: Core/runner.
- Exact case SQL:

```sql
RAISERROR(N'independent log probe',10,1) WITH LOG;
```

- Expected effect: Error/event log write if authorized; isolated sysadmin case.
- Reasoning / limitation: An explicitly requested log write is intentional, unlike incidental engine telemetry. Requires isolated privileged execution; Linux/container logging behavior may differ.

### 3. ind-allow-list-trigger

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
UPDATE sales.Orders SET Qty=Qty+1 WHERE Id=1;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["sales.Orders"]}
```

- Expected effect: Orders authorized, trigger writes local/cross-Audit audit.Changes not authorized.
- Reasoning / limitation: Only sales.Orders is authorized. Its local and cross-database audit trigger destinations are separate effects.

### 4. ind-allow-list-instead-of

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
UPDATE app.OrderEntry SET Qty=Qty+1 WHERE Id=1;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["app.OrderEntry"]}
```

- Expected effect: View spelling authorized but base/trigger destinations not authorized.
- Reasoning / limitation: Authorizing the view spelling does not itself authorize sales.Orders or the downstream audit destinations reached through its INSTEAD OF trigger.

### 5. ind-allow-list-cross-synonym

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
DELETE app.RemoteCounter;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["app.RemoteCounter"]}
```

- Expected effect: Archive.dbo.Counter changes, not allow-listed synonym identity.
- Reasoning / limitation: The changed base object is Archive.dbo.Counter, not the synonym spelling in the allow list; compare canonical resolved identities.

### 6. ind-allow-list-cascade

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
DELETE dbo.IndParent WHERE Id=1;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["dbo.IndParent"]}
```

- Expected effect: Child row cascade not authorized.
- Reasoning / limitation: The authorized parent delete cascades into the separately created dbo.IndChild fixture.

### 7. ind-allow-list-temporal

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
DELETE app.Accounts;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["app.Accounts"]}
```

- Expected effect: History table insert not explicitly authorized.
- Reasoning / limitation: The authorized current-table operation can insert history in audit.AccountHistory.

### 8. ind-allow-list-insert-default

- Local verdict: **AllowListed** (configured; default RealWrite). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
INSERT dbo.IndSequenceDefault DEFAULT VALUES;
```

Local options (tokens expanded before analysis):

```json
{"currentDatabase":"{{Core}}","defaultSchema":"dbo","allowListPatterns":["dbo.IndSequenceDefault"]}
```

- Expected effect: Authorized row insert allocates nonauthorized sequence.
- Reasoning / limitation: The authorized row insert invokes a default using app.OrderSequence, an independently persistent allocation.

### 9. ind-temporary-global-insert

- Local verdict: **TempOnly** (default). Lane: isolated; context: Core/runner.
- Exact case SQL:

```sql
CREATE TABLE ##independent_171(Id int, Value int); INSERT ##independent_171 VALUES(1,11);
```

- Expected effect: Shared global temporary object visible to other sessions; inspect even if later dropped.
- Reasoning / limitation: Expected shared-global-temp mutation violates the narrower TempOnly promise; it is not persistent business-data mutation.

### 10. ind-session-ansi-defaults-read

- Local verdict: **ReadOnly** (default). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
SET ANSI_DEFAULTS ON; SELECT * FROM dbo.IndTarget;
```

- Expected effect: ANSI_DEFAULTS can enable implicit transactions: inspect options and transaction count before disposal.
- Reasoning / limitation: ANSI_DEFAULTS can enable implicit transactions; inspect @@TRANCOUNT and retained options on the same connection. This is a session-state hypothesis, not persistent-table mutation.

### 11. ind-metadata-statistics-exact

- Local verdict: **ReadOnly** (default). Lane: shared; context: Core/runner.
- Exact case SQL:

```sql
EXEC sys.sp_statistics @table_name=N'IndTarget',@table_owner=N'dbo',@accuracy='E';
```

- Expected effect: Exact statistics request may refresh statistics; compare stats dates/blobs, feature-dependent.
- Reasoning / limitation: The exact-accuracy request may explicitly refresh statistics. Lower-confidence, implementation/feature-dependent hypothesis; compare dates/blobs and distinguish incidental automatic statistics.

## Lower-confidence candidates and boundaries

- `ind-dbcc-sqlperf-variable`: `DECLARE @option varchar(10)='CLEAR'; DBCC SQLPERF('sys.dm_os_wait_stats',@option);` is locally ReadOnly. If accepted in that argument position, counters could be cleared. Server grammar/semantics may reject it; do not treat parser acceptance as execution proof.
- Sequence defaults on local temp tables and NEXT VALUE FOR inside RETURN/subqueries have green paths but are likely server-disallowed. Keep them as semantic-rejection probes, not headline defects. Other direct sequence uses and the rollback literal control are RealWrite.
- EXECUTE AS followed by RETURN before REVERT; a session-owned application lock followed by RETURN before release; immutable sp_set_session_context keys; open keys/global cursors: all exercise green session residue. Inspect identity/keys/cursors/locks/options before pooling or disposal erases it.
- `ind-allow-list-wrong-default` deliberately analyzes with dbo while app_user resolves unqualified names under app. Its configured AllowListed is a context-mismatch test, not an intrinsic defect under the contract's matching-context precondition.
- TempdbIsTemp=true explicitly permits permanent tempdb objects as TempOnly; trusted writing/sequence procedures yield ReadOnly only through deliberately false administrator assertions. Report known contract discrepancies without presenting those assumptions as proofs.
- Environment mode Off and mode Red can suppress prompts without green classification. The environment observation records classification, warning mode and shouldWarn separately.
- OUTPUT to a nonauthorized permanent destination, DDL despite a DML allow list, remote incomplete patterns and direct persistent-target controls generally remain non-green. Do not discard these successful controls.

## Availability observations

Final sequential sample: 6,000 SELECT statements ReadOnly in **152.8247 ms**; first cold analysis RealWrite in **129.5672 ms**; CTE chain 1,024 Unknown in **37.9818 ms**; nested literal monitor ReadOnly in **29.6600 ms**; CTE chain 256 RealWrite in **23.2339 ms**; 1,000 SELECTs ReadOnly in **20.7447 ms**; 2,048 nested blocks Unknown in **20.1095 ms**. Earlier cold/stress samples varied (roughly 112–265 ms for 6,000 statements), so these are observations, not benchmark thresholds. Unknown on budget/depth limits is fail-closed, not a crash. No local analyzer failure or pathological multi-second slowdown was observed for this bounded corpus. Larger inputs or different runtime stacks remain untested.

## Parser-negative observations

- `ind-lexical-split-write` (default): Incorrect syntax near 'dbo'. Incorrect syntax near 'Value'.
- `ind-lexical-semicolon-go` (default): Incorrect syntax near 'GO'.
- `ind-lexical-unterminated-comment` (default): Expected but did not find an end comment mark '*/'.
- `ind-lexical-unterminated-string` (default): Incorrect syntax near 'SELECT'. Expected but did not find a closing quotation mark after the character string N'abc; UPDATE dbo.IndTarget SET Value=1;.
- `ind-lexical-write-after-invalid` (default): Incorrect syntax near 'FROM'.
- `ind-lexical-write-before-invalid` (default): Incorrect syntax near 'FROM'.
- `ind-sqlcmd-setvar-read` (configured): Incorrect syntax near ':'. Incorrect syntax near 'FROM'.
- `ind-sqlcmd-setvar-write` (configured): Incorrect syntax near ':'. Incorrect syntax near 'UPDATE'.
- `ind-sqlcmd-setvar-temp` (configured): Incorrect syntax near ':'. Incorrect syntax near '$(Target)'.
- `ind-sqlcmd-variable-unset` (configured): Incorrect syntax near 'FROM'.
- `ind-broker-receive-temp` (default): Incorrect syntax near '#ind'.
- `ind-session-raise-legacy` (default): Incorrect syntax near '50000'.
- `ind-set-options-disabledefcnstchk-on` (default): Incorrect syntax near 'DISABLE_DEF_CNST_CHK'.
- `ind-set-options-disabledefcnstchk-off` (default): Incorrect syntax near 'DISABLE_DEF_CNST_CHK'.
- `ind-set-options-errorlevel` (default): Incorrect syntax near 'ERRORLEVEL'.

## Rule targeting and its limits

Of 1,501 inventory entries, 245 non-needs-review entries advertise ReadOnly/TempOnly/AllowListed. **All 245 have at least one explicit target here**, including all safe-procedure, safe-DBCC and reviewed green statement/SET families. The inventory also contains 980 needs-review source checkpoints; these are not certified semantics and are not claimed exhaustively covered. Other annotation/red-only entries are targeted opportunistically.

The crosswalk is **design traceability, not instrumented branch coverage**. A target can be aggregated away, stop at parsing, or map to an abstract/obsolete AST member. CursorStatement/TransactionStatement bases and legacy SET/RAISERROR/USE FEDERATION forms cannot be assumed reached merely from ruleIds. A red overall result does not mean its green-capable subrule was never visited; a green overall result does not prove every targeted branch was hit. First-case selection below is deterministic corpus order, not the strongest case. Verdict column is default / configured where present.

| Target rule | Cases | Example case | Local overall verdict |
| --- | ---: | --- | --- |
| `aggregate.max` | 14 | `ind-allow-list-trigger` | RealWrite / AllowListed |
| `allow.match` | 36 | `ind-allow-list-exact` | RealWrite / AllowListed |
| `dbcc.checkident` | 5 | `ind-dbcc-identity-noreseed` | ReadOnly |
| `temp.name` | 38 | `ind-sequences-temp-insert` | RealWrite |
| `temp.tempdb-opt-in` | 2 | `ind-allow-list-tempdb-optin-false` | RealWrite / RealWrite |
| `write.allow-list` | 28 | `ind-allow-list-exact` | RealWrite / AllowListed |
| `entry.empty` | 22 | `ind-lexical-empty` | ReadOnly |
| `environment.selection` | 9 | `ind-environment-matching` | RealWrite / AllowListed |
| `green-statement.begintransactionstatement` | 1 | `ind-transactions-begin` | Unknown |
| `green-statement.breakstatement` | 1 | `ind-session-while-break` | ReadOnly |
| `green-statement.closecursorstatement` | 5 | `ind-cursors-balanced-local-fast_forward` | ReadOnly |
| `green-statement.closemasterkeystatement` | 1 | `ind-session-close-master` | ReadOnly |
| `green-statement.closesymmetrickeystatement` | 1 | `ind-session-close-symmetric` | ReadOnly |
| `green-statement.committransactionstatement` | 1 | `ind-transactions-balanced` | ReadOnly |
| `green-statement.continuestatement` | 1 | `ind-session-while-continue` | ReadOnly |
| `green-statement.cursorstatement` | 5 | `ind-cursors-retained-local-fast_forward` | ReadOnly |
| `green-statement.deallocatecursorstatement` | 5 | `ind-cursors-balanced-local-fast_forward` | ReadOnly |
| `green-statement.declarecursorstatement` | 10 | `ind-cursors-retained-local-fast_forward` | ReadOnly |
| `green-statement.declaretablevariablestatement` | 6 | `ind-sequences-tablevar-insert` | RealWrite |
| `green-statement.declarevariablestatement` | 2 | `ind-sequences-variable` | RealWrite |
| `green-statement.executeasstatement` | 3 | `ind-session-execute-as` | ReadOnly |
| `green-statement.fetchcursorstatement` | 5 | `ind-cursors-retained-local-fast_forward` | ReadOnly |
| `green-statement.getconversationgroupstatement` | 8 | `ind-session-get-conversation` | ReadOnly |
| `green-statement.gotostatement` | 1 | `ind-session-goto` | ReadOnly |
| `green-statement.labelstatement` | 1 | `ind-session-goto` | ReadOnly |
| `green-statement.linenostatement` | 1 | `ind-session-lineno` | ReadOnly |
| `green-statement.opencursorstatement` | 6 | `ind-cursors-retained-local-fast_forward` | ReadOnly |
| `green-statement.openmasterkeystatement` | 1 | `ind-session-open-master` | ReadOnly |
| `green-statement.opensymmetrickeystatement` | 1 | `ind-session-open-symmetric` | ReadOnly |
| `green-statement.printstatement` | 2 | `ind-sequences-print` | RealWrite |
| `green-statement.raiseerrorlegacystatement` | 1 | `ind-session-raise-legacy` | Unknown |
| `green-statement.raiseerrorstatement` | 2 | `ind-session-raise-low` | ReadOnly |
| `green-statement.readtextstatement` | 1 | `ind-session-readtext` | ReadOnly |
| `green-statement.returnstatement` | 3 | `ind-session-return` | ReadOnly |
| `green-statement.revertstatement` | 3 | `ind-session-execute-as-balanced` | ReadOnly |
| `green-statement.rollbacktransactionstatement` | 1 | `ind-transactions-rollback` | ReadOnly |
| `green-statement.savetransactionstatement` | 1 | `ind-transactions-savepoint` | ReadOnly |
| `green-statement.setcommandstatement` | 20 | `ind-set-options-context_info` | ReadOnly |
| `green-statement.seterrorlevelstatement` | 1 | `ind-set-options-errorlevel` | Unknown |
| `green-statement.setidentityinsertstatement` | 1 | `ind-set-options-identityinsert` | ReadOnly |
| `green-statement.setonoffstatement` | 50 | `ind-set-options-ansidefaults-on` | ReadOnly |
| `green-statement.setrowcountstatement` | 1 | `ind-set-options-rowcount` | ReadOnly |
| `green-statement.settextsizestatement` | 1 | `ind-set-options-textsize` | ReadOnly |
| `green-statement.settransactionisolationlevelstatement` | 1 | `ind-set-options-isolation` | ReadOnly |
| `green-statement.setuserstatement` | 2 | `ind-session-setuser` | ReadOnly |
| `green-statement.setvariablestatement` | 2 | `ind-sequences-set` | RealWrite |
| `green-statement.throwstatement` | 1 | `ind-session-throw` | ReadOnly |
| `green-statement.transactionstatement` | 1 | `ind-transactions-begin` | Unknown |
| `green-statement.usefederationstatement` | 1 | `ind-session-federation` | Unknown |
| `green-statement.usestatement` | 1 | `ind-session-use` | Unknown |
| `green-statement.waitforstatement` | 1 | `ind-session-wait-delay` | ReadOnly |
| `hidden.bulk-file` | 13 | `ind-physical-identity-rollback` | RealWrite |
| `msdb-procedure.sp_add_job` | 1 | `ind-metadata-msdb-sp_add_job` | RealWrite |
| `msdb-procedure.sp_add_jobschedule` | 1 | `ind-metadata-msdb-sp_add_jobschedule` | RealWrite |
| `msdb-procedure.sp_add_jobserver` | 1 | `ind-metadata-msdb-sp_add_jobserver` | RealWrite |
| `msdb-procedure.sp_add_jobstep` | 1 | `ind-metadata-msdb-sp_add_jobstep` | RealWrite |
| `msdb-procedure.sp_add_schedule` | 1 | `ind-metadata-msdb-sp_add_schedule` | RealWrite |
| `msdb-procedure.sp_attach_schedule` | 1 | `ind-metadata-msdb-sp_attach_schedule` | RealWrite |
| `msdb-procedure.sp_delete_backuphistory` | 1 | `ind-metadata-msdb-sp_delete_backuphistory` | RealWrite |
| `msdb-procedure.sp_delete_job` | 1 | `ind-metadata-msdb-sp_delete_job` | RealWrite |
| `msdb-procedure.sp_delete_jobstep` | 1 | `ind-metadata-msdb-sp_delete_jobstep` | RealWrite |
| `msdb-procedure.sp_help_job` | 1 | `ind-metadata-msdb-sp_help_job` | ReadOnly |
| `msdb-procedure.sp_help_jobhistory` | 1 | `ind-metadata-msdb-sp_help_jobhistory` | ReadOnly |
| `msdb-procedure.sp_help_jobstep` | 1 | `ind-metadata-msdb-sp_help_jobstep` | ReadOnly |
| `msdb-procedure.sp_help_schedule` | 1 | `ind-metadata-msdb-sp_help_schedule` | ReadOnly |
| `msdb-procedure.sp_purge_jobhistory` | 1 | `ind-metadata-msdb-sp_purge_jobhistory` | RealWrite |
| `msdb-procedure.sp_send_dbmail` | 1 | `ind-metadata-msdb-sp_send_dbmail` | RealWrite |
| `msdb-procedure.sp_start_job` | 1 | `ind-metadata-msdb-sp_start_job` | RealWrite |
| `msdb-procedure.sp_stop_job` | 1 | `ind-metadata-msdb-sp_stop_job` | RealWrite |
| `msdb-procedure.sp_update_job` | 1 | `ind-metadata-msdb-sp_update_job` | RealWrite |
| `msdb-procedure.sp_update_jobstep` | 1 | `ind-metadata-msdb-sp_update_jobstep` | RealWrite |
| `parser.newest` | 25 | `ind-lexical-empty` | ReadOnly |
| `resolve.alias` | 5 | `ind-resolution-hash-alias` | RealWrite |
| `resolve.alias-priority` | 1 | `ind-resolution-cte-alias-priority` | RealWrite |
| `resolve.cte-cycle` | 1 | `ind-resolution-recursive-cycle` | RealWrite |
| `resolve.derived` | 1 | `ind-resolution-derived` | RealWrite |
| `resolve.join` | 1 | `ind-resolution-cte-join` | RealWrite |
| `resolve.union` | 1 | `ind-resolution-union` | RealWrite |
| `resolve.variable` | 13 | `ind-sequences-tablevar-insert` | RealWrite |
| `safe-dbcc.activecursors` | 1 | `ind-dbcc-activecursors` | ReadOnly |
| `safe-dbcc.cachestats` | 1 | `ind-dbcc-cachestats` | ReadOnly |
| `safe-dbcc.checkalloc` | 1 | `ind-dbcc-checkalloc` | ReadOnly |
| `safe-dbcc.checkcatalog` | 1 | `ind-dbcc-checkcatalog` | ReadOnly |
| `safe-dbcc.checkconstraints` | 1 | `ind-dbcc-checkconstraints` | RealWrite |
| `safe-dbcc.checkdb` | 1 | `ind-dbcc-checkdb` | ReadOnly |
| `safe-dbcc.checkfilegroup` | 1 | `ind-dbcc-checkfilegroup` | ReadOnly |
| `safe-dbcc.checktable` | 1 | `ind-dbcc-checktable` | ReadOnly |
| `safe-dbcc.cursorstats` | 1 | `ind-dbcc-cursorstats` | ReadOnly |
| `safe-dbcc.extentinfo` | 1 | `ind-dbcc-extentinfo` | ReadOnly |
| `safe-dbcc.help` | 1 | `ind-dbcc-help` | ReadOnly |
| `safe-dbcc.inputbuffer` | 1 | `ind-dbcc-inputbuffer` | ReadOnly |
| `safe-dbcc.loginfo` | 1 | `ind-dbcc-loginfo` | ReadOnly |
| `safe-dbcc.memorystatus` | 1 | `ind-dbcc-memorystatus` | ReadOnly |
| `safe-dbcc.opentran` | 1 | `ind-dbcc-opentran` | ReadOnly |
| `safe-dbcc.outputbuffer` | 1 | `ind-dbcc-outputbuffer` | ReadOnly |
| `safe-dbcc.perfmonstats` | 1 | `ind-dbcc-perfmonstats` | RealWrite |
| `safe-dbcc.proccache` | 1 | `ind-dbcc-proccache` | ReadOnly |
| `safe-dbcc.showcontig` | 1 | `ind-dbcc-showcontig` | ReadOnly |
| `safe-dbcc.showfilestats` | 1 | `ind-dbcc-showfilestats` | ReadOnly |
| `safe-dbcc.showoffrules` | 1 | `ind-dbcc-showoffrules` | ReadOnly |
| `safe-dbcc.showonrules` | 1 | `ind-dbcc-showonrules` | ReadOnly |
| `safe-dbcc.showstatistics` | 1 | `ind-dbcc-showstatistics` | RealWrite |
| `safe-dbcc.showtext` | 1 | `ind-dbcc-showtext` | ReadOnly |
| `safe-dbcc.showweights` | 1 | `ind-dbcc-showweights` | ReadOnly |
| `safe-dbcc.sqlperf` | 1 | `ind-dbcc-sqlperf` | ReadOnly |
| `safe-dbcc.tracestatus` | 1 | `ind-dbcc-tracestatus` | ReadOnly |
| `safe-dbcc.useroptions` | 1 | `ind-dbcc-useroptions` | ReadOnly |
| `safe-procedure.sp_column_privileges` | 1 | `ind-metadata-sp_column_privileges` | ReadOnly |
| `safe-procedure.sp_columns` | 1 | `ind-metadata-sp_columns` | ReadOnly |
| `safe-procedure.sp_databases` | 1 | `ind-metadata-sp_databases` | ReadOnly |
| `safe-procedure.sp_depends` | 1 | `ind-metadata-sp_depends` | ReadOnly |
| `safe-procedure.sp_describe_first_result_set` | 1 | `ind-metadata-sp_describe_first_result_set` | ReadOnly |
| `safe-procedure.sp_describe_undeclared_parameters` | 1 | `ind-metadata-sp_describe_undeclared_parameters` | ReadOnly |
| `safe-procedure.sp_fkeys` | 1 | `ind-metadata-sp_fkeys` | ReadOnly |
| `safe-procedure.sp_getapplock` | 2 | `ind-metadata-sp_getapplock` | ReadOnly |
| `safe-procedure.sp_help` | 8 | `ind-metadata-sp_help` | ReadOnly |
| `safe-procedure.sp_help_job` | 1 | `ind-metadata-sp_help_job` | ReadOnly |
| `safe-procedure.sp_help_jobhistory` | 1 | `ind-metadata-sp_help_jobhistory` | ReadOnly |
| `safe-procedure.sp_help_jobstep` | 1 | `ind-metadata-sp_help_jobstep` | ReadOnly |
| `safe-procedure.sp_help_schedule` | 1 | `ind-metadata-sp_help_schedule` | ReadOnly |
| `safe-procedure.sp_helpconstraint` | 1 | `ind-metadata-sp_helpconstraint` | ReadOnly |
| `safe-procedure.sp_helpdb` | 1 | `ind-metadata-sp_helpdb` | ReadOnly |
| `safe-procedure.sp_helpdbfixedrole` | 1 | `ind-metadata-sp_helpdbfixedrole` | ReadOnly |
| `safe-procedure.sp_helpextendedproc` | 1 | `ind-metadata-sp_helpextendedproc` | ReadOnly |
| `safe-procedure.sp_helpfile` | 1 | `ind-metadata-sp_helpfile` | ReadOnly |
| `safe-procedure.sp_helpfilegroup` | 1 | `ind-metadata-sp_helpfilegroup` | ReadOnly |
| `safe-procedure.sp_helpindex` | 1 | `ind-metadata-sp_helpindex` | ReadOnly |
| `safe-procedure.sp_helplanguage` | 1 | `ind-metadata-sp_helplanguage` | ReadOnly |
| `safe-procedure.sp_helplogins` | 1 | `ind-metadata-sp_helplogins` | ReadOnly |
| `safe-procedure.sp_helpntgroup` | 1 | `ind-metadata-sp_helpntgroup` | ReadOnly |
| `safe-procedure.sp_helprole` | 1 | `ind-metadata-sp_helprole` | ReadOnly |
| `safe-procedure.sp_helprolemember` | 1 | `ind-metadata-sp_helprolemember` | ReadOnly |
| `safe-procedure.sp_helprotect` | 1 | `ind-metadata-sp_helprotect` | ReadOnly |
| `safe-procedure.sp_helpserver` | 1 | `ind-metadata-sp_helpserver` | ReadOnly |
| `safe-procedure.sp_helpsort` | 1 | `ind-metadata-sp_helpsort` | ReadOnly |
| `safe-procedure.sp_helpsrvrole` | 1 | `ind-metadata-sp_helpsrvrole` | ReadOnly |
| `safe-procedure.sp_helpsrvrolemember` | 1 | `ind-metadata-sp_helpsrvrolemember` | ReadOnly |
| `safe-procedure.sp_helptext` | 1 | `ind-metadata-sp_helptext` | ReadOnly |
| `safe-procedure.sp_helptrigger` | 1 | `ind-metadata-sp_helptrigger` | ReadOnly |
| `safe-procedure.sp_helpuser` | 1 | `ind-metadata-sp_helpuser` | ReadOnly |
| `safe-procedure.sp_lock` | 1 | `ind-metadata-sp_lock` | ReadOnly |
| `safe-procedure.sp_monitor` | 4 | `ind-metadata-sp_monitor` | ReadOnly |
| `safe-procedure.sp_pkeys` | 1 | `ind-metadata-sp_pkeys` | ReadOnly |
| `safe-procedure.sp_readerrorlog` | 1 | `ind-metadata-sp_readerrorlog` | ReadOnly |
| `safe-procedure.sp_releaseapplock` | 2 | `ind-metadata-sp_releaseapplock` | ReadOnly |
| `safe-procedure.sp_server_info` | 1 | `ind-metadata-sp_server_info` | ReadOnly |
| `safe-procedure.sp_set_session_context` | 3 | `ind-metadata-sp_set_session_context` | ReadOnly |
| `safe-procedure.sp_spaceused` | 11 | `ind-metadata-sp_spaceused` | ReadOnly |
| `safe-procedure.sp_special_columns` | 1 | `ind-metadata-sp_special_columns` | ReadOnly |
| `safe-procedure.sp_sproc_columns` | 1 | `ind-metadata-sp_sproc_columns` | ReadOnly |
| `safe-procedure.sp_statistics` | 2 | `ind-metadata-sp_statistics` | ReadOnly |
| `safe-procedure.sp_stored_procedures` | 1 | `ind-metadata-sp_stored_procedures` | ReadOnly |
| `safe-procedure.sp_table_privileges` | 1 | `ind-metadata-sp_table_privileges` | ReadOnly |
| `safe-procedure.sp_tables` | 1 | `ind-metadata-sp_tables` | ReadOnly |
| `safe-procedure.sp_who` | 1 | `ind-metadata-sp_who` | ReadOnly |
| `safe-procedure.sp_who2` | 1 | `ind-metadata-sp_who2` | ReadOnly |
| `safe-procedure.xp_readerrorlog` | 1 | `ind-metadata-xp_readerrorlog` | ReadOnly |
| `scope.dynamic` | 53 | `ind-metadata-statistics-exact` | ReadOnly |
| `set-family.generalsetcommandtype.contextinfo` | 2 | `ind-set-options-context_info` | ReadOnly |
| `set-family.generalsetcommandtype.datefirst` | 1 | `ind-set-options-datefirst` | ReadOnly |
| `set-family.generalsetcommandtype.dateformat` | 1 | `ind-set-options-dateformat` | ReadOnly |
| `set-family.generalsetcommandtype.deadlockpriority` | 1 | `ind-set-options-deadlock_priority` | ReadOnly |
| `set-family.generalsetcommandtype.language` | 2 | `ind-set-options-language` | ReadOnly |
| `set-family.generalsetcommandtype.locktimeout` | 1 | `ind-set-options-lock_timeout` | ReadOnly |
| `set-family.generalsetcommandtype.querygovernorcostlimit` | 1 | `ind-set-options-query_governor_cost_limit` | ReadOnly |
| `set-family.setoffsets.compute` | 1 | `ind-set-options-offsets-compute` | ReadOnly |
| `set-family.setoffsets.execute` | 1 | `ind-set-options-offsets-execute` | ReadOnly |
| `set-family.setoffsets.from` | 1 | `ind-set-options-offsets-from` | ReadOnly |
| `set-family.setoffsets.order` | 1 | `ind-set-options-offsets-order` | ReadOnly |
| `set-family.setoffsets.param` | 1 | `ind-set-options-offsets-param` | ReadOnly |
| `set-family.setoffsets.procedure` | 1 | `ind-set-options-offsets-procedure` | ReadOnly |
| `set-family.setoffsets.select` | 1 | `ind-set-options-offsets-select` | ReadOnly |
| `set-family.setoffsets.statement` | 1 | `ind-set-options-offsets-statement` | ReadOnly |
| `set-family.setoffsets.table` | 1 | `ind-set-options-offsets-table` | ReadOnly |
| `set-family.setstatisticsoptions.io` | 1 | `ind-set-options-statistics-io` | ReadOnly |
| `set-family.setstatisticsoptions.profile` | 1 | `ind-set-options-statistics-profile` | ReadOnly |
| `set-family.setstatisticsoptions.time` | 1 | `ind-set-options-statistics-time` | ReadOnly |
| `set-family.setstatisticsoptions.xml` | 1 | `ind-set-options-statistics-xml` | ReadOnly |
| `set-option.ansidefaults` | 4 | `ind-set-options-ansidefaults-on` | ReadOnly |
| `set-option.ansinulldfltoff` | 2 | `ind-set-options-ansinulldfltoff-on` | ReadOnly |
| `set-option.ansinulldflton` | 2 | `ind-set-options-ansinulldflton-on` | ReadOnly |
| `set-option.ansinulls` | 2 | `ind-set-options-ansinulls-on` | ReadOnly |
| `set-option.ansipadding` | 2 | `ind-set-options-ansipadding-on` | ReadOnly |
| `set-option.ansiwarnings` | 2 | `ind-set-options-ansiwarnings-on` | ReadOnly |
| `set-option.arithabort` | 2 | `ind-set-options-arithabort-on` | ReadOnly |
| `set-option.arithignore` | 2 | `ind-set-options-arithignore-on` | ReadOnly |
| `set-option.concatnullyieldsnull` | 2 | `ind-set-options-concatnullyieldsnull-on` | ReadOnly |
| `set-option.cursorcloseoncommit` | 2 | `ind-set-options-cursorcloseoncommit-on` | ReadOnly |
| `set-option.disabledefcnstchk` | 2 | `ind-set-options-disabledefcnstchk-on` | Unknown |
| `set-option.fmtonly` | 2 | `ind-set-options-fmtonly-on` | ReadOnly |
| `set-option.forceplan` | 2 | `ind-set-options-forceplan-on` | ReadOnly |
| `set-option.implicittransactions` | 2 | `ind-set-options-implicittransactions-on` | ReadOnly |
| `set-option.nobrowsetable` | 2 | `ind-set-options-nobrowsetable-on` | ReadOnly |
| `set-option.nocount` | 2 | `ind-set-options-nocount-on` | ReadOnly |
| `set-option.noexec` | 3 | `ind-set-options-noexec-on` | ReadOnly |
| `set-option.numericroundabort` | 2 | `ind-set-options-numericroundabort-on` | ReadOnly |
| `set-option.parseonly` | 3 | `ind-set-options-parseonly-on` | ReadOnly |
| `set-option.quotedidentifier` | 2 | `ind-set-options-quotedidentifier-on` | ReadOnly |
| `set-option.remoteproctransactions` | 2 | `ind-set-options-remoteproctransactions-on` | ReadOnly |
| `set-option.showplanall` | 2 | `ind-set-options-showplanall-on` | ReadOnly |
| `set-option.showplantext` | 2 | `ind-set-options-showplantext-on` | ReadOnly |
| `set-option.showplanxml` | 3 | `ind-set-options-showplanxml-on` | ReadOnly |
| `set-option.xactabort` | 3 | `ind-set-options-xactabort-on` | ReadOnly |
| `sqlcmd-command.ed` | 1 | `ind-sqlcmd-ed` | ReadOnly |
| `sqlcmd-command.error` | 1 | `ind-sqlcmd-error` | ReadOnly |
| `sqlcmd-command.exit` | 1 | `ind-sqlcmd-exit` | ReadOnly |
| `sqlcmd-command.help` | 1 | `ind-sqlcmd-help` | ReadOnly |
| `sqlcmd-command.list` | 1 | `ind-sqlcmd-list` | ReadOnly |
| `sqlcmd-command.listvar` | 1 | `ind-sqlcmd-listvar` | ReadOnly |
| `sqlcmd-command.on` | 1 | `ind-sqlcmd-on` | ReadOnly |
| `sqlcmd-command.out` | 1 | `ind-sqlcmd-out` | ReadOnly |
| `sqlcmd-command.perftrace` | 1 | `ind-sqlcmd-perftrace` | ReadOnly |
| `sqlcmd-command.quit` | 1 | `ind-sqlcmd-quit` | ReadOnly |
| `sqlcmd-command.reset` | 1 | `ind-sqlcmd-reset` | ReadOnly |
| `sqlcmd-command.serverlist` | 1 | `ind-sqlcmd-serverlist` | ReadOnly |
| `sqlcmd-command.setvar` | 10 | `ind-sqlcmd-setvar-read` | Unknown / Unknown |
| `sqlcmd-command.xml` | 1 | `ind-sqlcmd-xml` | ReadOnly |
| `exec.dispatch` | 65 | `ind-metadata-sp_help` | ReadOnly |
| `exec.dynamic` | 64 | `ind-sequences-dynamic` | RealWrite |
| `exec.empty` | 3 | `ind-dynamic-empty-literal` | ReadOnly |
| `exec.namespace` | 76 | `ind-metadata-sp_help` | ReadOnly |
| `exec.sp-executesql` | 42 | `ind-dynamic-read-literal` | ReadOnly |
| `exec.spaceused` | 10 | `ind-metadata-spaceused-omitted` | ReadOnly |
| `exec.trusted` | 8 | `ind-trust-trusted-writer` | Unknown / ReadOnly |
| `sqlcmd.walk` | 25 | `ind-sqlcmd-setvar-read` | Unknown / Unknown |
| `txn.begin` | 1 | `ind-transactions-begin` | Unknown |
| `txn.commit` | 8 | `ind-set-options-implicit-balanced` | ReadOnly |
| `txn.if` | 4 | `ind-transactions-if-return` | Unknown |
| `txn.rollback` | 7 | `ind-sequences-scalar-rollback` | RealWrite |
| `walk.composable` | 3 | `ind-temporary-composable-local` | TempOnly |
| `walk.dml-targets` | 46 | `ind-sequences-default-permanent` | RealWrite |
| `walk.drop-index` | 4 | `ind-temporary-local-drop-index` | TempOnly |
| `walk.drop-procedures` | 2 | `ind-temporary-drop-two-procedures` | TempOnly |
| `walk.drop-statistics` | 3 | `ind-temporary-local-drop-statistics` | TempOnly |
| `walk.insert-exec` | 11 | `ind-dynamic-parameter-read` | ReadOnly |
| `walk.module` | 5 | `ind-temporary-temp-procedure` | Unknown |
| `walk.named-target` | 40 | `ind-sequences-temp-default` | TempOnly |
| `walk.output` | 3 | `ind-persistent-targets-output-side-target` | RealWrite |
| `walk.return` | 7 | `ind-sequences-return` | ReadOnly |
| `walk.select-into` | 3 | `ind-sequences-select-into` | RealWrite |
| `walk.switch-both` | 1 | `ind-persistent-targets-switch` | RealWrite |
| `walk.temp-procedure` | 10 | `ind-temporary-variable` | TempOnly |
| `walk.throw` | 2 | `ind-session-throw` | ReadOnly |
| `walk.transfer` | 5 | `ind-transactions-goto-over-commit` | Unknown |
| `walk.waitfor` | 9 | `ind-session-wait-delay` | ReadOnly |

## Server oracle and remaining gaps

Observe both rows and physical/catalog changes, identity/sequence state despite rollback, trigger/cascade/temporal destinations, Broker conversations and asynchronous effects. Wrong-target candidates require canonical database/schema/base-object comparison, not string equality with a synonym/view/alias. Same-connection session inspection must precede cleanup. A non-green verdict or SQL error is not a defect by itself; errors after effects do not erase a false green.

The four restored databases alone cannot observe master.spt_monitor, permanent tempdb objects, server counters, error/event logs, linked-server execution, external files or all session keys/locks/cursors. Add authorized isolated observations for these candidates; a PASS with an inadequate oracle is not safety proof. Global objects can be transient or shared; observe before connection disposal. Broker self-send delivery can race the bounded 1-second RECEIVE; an empty receive is inconclusive rather than evidence the capability is read-only.

Missing BULK files, disabled providers/ad-hoc distributed queries, unavailable SQL Agent/mail/legacy procedures, denied permissions and unsupported syntax are NOT EXECUTABLE, not PASS. No real BULK fixture or external mail/Agent target is supplied. No performance benchmark, full SQL grammar Cartesian product, complete unreviewed-inventory coverage or universal green-safety claim is made. A server run, configured-options integration and supplemental isolated/system/session oracles remain required.
