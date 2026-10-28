# Wave-3 validation ? October 28, 2026

## Scope and disposition

This is a qualified implementation/evidence handoff, **not completion of exhaustive semantic verification**. The requested full run is prepared, not started. `gaps-wave3.md` records unsupported, failed, unobserved and still-unprovisioned work; these are not passes. In particular, 98 of the 250 requested rules lack successful tagged positive/boundary server evidence, and 51 original statement kinds lack a live semantic pass. Fifty of those rules are API/client/AST constraints without a meaningful standalone SQL triple. Some other failures are inadequate recipes, not established platform impossibility.

Read `contract.md`, `findings-wave3.md`, `rule-coverage-wave3.md`, `gaps-wave3.md`, `policy-questions.md` and machine-readable `validation-wave3.json` together. All external artifact paths below are relative to `C:\Users\nagya\.t3\worktrees\safequery\harness-out\campaign` unless stated otherwise. The retained SQL/setup/options, binary hashes, world identity and observations are more precise than this summary.

## 1. World, principals and stratified replay

Restricted logins now have users in all four owned databases. LOGIN impersonation with NO REVERT permits cross-database work without granting unrestricted server access; runner/app_user are database owners in their owned set, while low_user retains its deliberate denial cases. An explicit admin campaign principal uses the provisioning sysadmin identity. No host TRUSTWORTHY or ownership-chaining change was necessary. The host campaign does not provision anything in SgExt_* or alter unrelated existing databases.

| Replay | Completed | NOT EXECUTABLE | Throughput | Pristine reuse / isolation checks |
| --- | ---: | ---: | ---: | ---: |
| Wave-2 original stratified sample | 316 | 83 | 1,986.78 cases/hour | historical controls |
| Wave-3 mapped restricted principals | 316 | 50 | about 1,712 cases/hour | 153 / 153 |
| Same sample, explicit admin replay | 316 | 0 | about 1,588.5 cases/hour | 167 / 165 |

The baseline lives at `C:\Users\nagya\dev\work_exim\ssmsextensions\harness-out\campaign\2026-10-28`; new runs are `wave3-validation-mapped` and `wave3-validation-admin`. The admin run has 206 PASS, 48 INDIRECT EFFECT, 50 NOISE, seven EXTERNAL CHANGE, one UNRESOLVED TARGET, two historical FALSE GREEN and two historical CRASH results. The four historical defect/crash IDs were replayed with the corrected analyzer/harness and all four are PASS in `wave3-host-replay`. Do not describe the admin run as 316 safety passes or erase its original receipts.

## 2. Focused rules

The generated focused corpus has 600 cases: positive, negative and boundary triples for 200 rules, split 426 host / 174 isolated. All 600 have retained server attempts. The remaining 50 API/client/AST rules have explicit non-SQL dispositions. **152/250 rules** have successful tagged positive/boundary server-associated evidence, not internal branch instrumentation. Generic negative SELECTs do not close rules.

Latest focused outcomes: 415 PASS, 105 NOT EXECUTABLE, 65 NOISE, six OBSERVATION GAP, four POLICY EFFECT, two INDIRECT EFFECT, one EXTERNAL CHANGE, one FALSE GREEN and one CRASH. The false green is the existing TempdbIsTemp opt-in discrepancy; DBCC DEBUGBREAK killed its disposable SQL server and is retained as CRASH, not a pass. Fresh-container resumption preserved all 120 receipts in `wave3-final-replay`. The six actual UPDATE/DELETE resolution recipes in `wave3-resolution-replay` replace stale SELECT stubs; server compilation rejects those non-updatable/cyclic derived targets, which cannot establish successful mutation coverage.

## 3. Statement kinds and Docker features

Of the original 67 gaps, wave 2 had 12 live kinds; wave 3 has **16/67 live, 51 remaining**. The feature corpus contains 71 fixtures. Latest feature receipts comprise 28 semantic passes, eight executed-unverified attempts and 35 execution/assertion failures. Three valid CLUSTER_TYPE=NONE availability-group create/alter/drop statements execute in the HADR-enabled container, but the pinned parser rejects that syntax; attempt-only results do not close semantic coverage.

Agent is enabled; full-text, self-built trusted CLR assembly/types, external tables/data sources/file formats, external models, notification kill, server configuration and HADR-style metadata were exercised or attempted. CREATE/ALTER/DROP EXTERNAL MODEL and KILL QUERY NOTIFICATION SUBSCRIPTION supply the four additional live kinds. Image identity is `sha256:f634a44c5d75eac1a13cc81a3dd9021b2c4517db552bac3eda7215bc9722f12b`, SQL Server 17.00.5035. See every original kind's exact stage/error in `gaps-wave3.md`. EKM packages, external-language packages, recovery/log rebuild and restart-aware observers remain unprovisioned; no impossibility claim is made for them. Sixteen AST-only/legacy nodes retain their pinned-parser limitations. Runs: `wave3-features-final`, `wave3-ag-attempt`.

## 4. Independent corpus and settings

All **600 independent cases** have a result: 432 host + 109 Docker attempts + 59 offline checks. Combined server outcomes (541): 373 PASS, 80 NOISE, 47 NOT EXECUTABLE, 17 POLICY EFFECT, ten INDIRECT EFFECT, seven OBSERVATION GAP, five UNRESOLVED TARGET, one EXTERNAL CHANGE and one FALSE GREEN. The 59 offline checks have zero recorded mismatches and zero analyzer errors; they execute no SQL.

All 58 configured probes now consume their per-case settings: 26 shared allow-list, eight shared trusted procedures, one shared dynamic-depth, four isolated allow-list, nine offline environment and ten offline SQLCMD cases. Supported settings also include TempdbIsTemp, default schema/current database, connection server and warning mode. Effective execution context and analyzer context are aligned, environment selection is reflected, and typed confirmation remains disabled. Empty SQL is a valid no-op, fixing both rejected independent cases. Invalid rows and duplicate IDs are reported without aborting accepted cases; upfront full-corpus validation is separate from execution filters.

NOT EXECUTABLE is still used honestly for deliberate permission-denial cases, invalid SQL and unsupported/missing provider features; it is not restricted to platform incompatibilities outside the stratified replay. Four low_user write-denial probes retain their intended principal rather than silently escalating. Linux OPENDATASOURCE reports inability to instantiate MSDASC. Dedicated loopback OPENQUERY write evidence is UNRESOLVED TARGET, not invented WRONG TARGET.

## 5?7. Defects, policy evidence and throughput

Confirmed default FALSE GREEN: `EXEC master.dbo.sp_monitor;` intentionally changes `master.dbo.spt_monitor` (checksum -1283799775 ? 1399735909 in `wave3-top-suspects-before`). The safe-procedure entry is removed and a writing-procedure entry now produces RealWrite. Eleven monitor wrapper unit regressions and nine live wrapper fixtures cover the correction. The host master database was not modified by these probes.

Physical-target regressions also fix `DROP STATISTICS dbo.IndTarget.IndValueStats;`, `DROP INDEX dbo.IndTarget.IndValueIndex;` and `ALTER SCHEMA app TRANSFER dbo.IndSink;`. The first two resolve the physical table, and schema transfer retains its source object as well as destination scope.

Unchanged policy discrepancies are explicit: allow-listed table/view/synonym entry points can cause indirect writes; trusted writers can mutate; `RAISERROR(N'Wave3OwnedLogMarker',10,1) WITH LOG;` writes the server error log while ReadOnly; and TempdbIsTemp=true permits permanent shared tempdb objects. The two retained contract false greens are `CREATE TABLE tempdb.dbo.IndOptIn_T(Id int);` and `SELECT 1 AS Id INTO tempdb.dbo.Wave3Focused;`, both with that explicit opt-in. These are not silently reclassified as passes or fixed as new default defects.

Throughput is **not demonstrated to improve over wave 2**. The comparable admin stratified replay is about 1,588.5 cases/hour versus 1,986.78 before; strengthened observations and isolation have a cost. A separate 89-case representative host replay completes in 219.47 seconds (about 1,460 cases/hour). Pristine reuse requires empty setup, matching schema/context, complete snapshot equality, no session residue and a new before-case security/isolation check. Any changed/error world destroys all four databases. Mutexes and the SQL instance application lock prohibit overlapping campaigns. Durable receipts precede SQL persistence and supersede stale SQL rows on resume. No isolation check was relaxed to improve throughput.

## 8. Full-run preparation and validation

The source-controlled `tools/verification/Freeze-Wave3.ps1` freezes committed Release output, SQL world resources, cases, scripts and SHA-256 manifest outside this worktree under `..\harness-out\campaign\bin\<source-sha>`. `Run-Wave3Full.ps1` validates all lanes first, then executes offline ? generated host (admin) ? focused/independent host ? isolated ? features sequentially, with no campaign time budget. It uses identity-matched durable resumption, fresh disposable containers after server death, per-case SQL timeouts, and rejects reports instead of aborting accepted cases because of one malformed row. A SQL timeout is a case limitation, not an overall run budget.

Frozen inventory: **33,035 campaign cases + 71 feature fixtures = 33,106 total**. This includes 31,004 generated host cases (all 30,000 random cases, rule attacks, pairwise/three-wise and company scripts), 831 generated offline cases, all 600 focused rule cases, and all 600 independent cases. No full run has been started. The exact frozen path and limited smoke receipts are recorded in the final frozen-build section after validation.

## Build checks

`dotnet test tests\SqlSafetyGuard.Tests --nologo`: **1,444 passed, zero failed/skipped**, including journal precedence, tolerant validation, settings, attribution and bounded isolated CLI regressions. Generator invariants: four passed. Release solution and frozen smoke outcomes are recorded after those commands finish; do not infer success from this document's existence.
