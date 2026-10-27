# Systematic T-SQL verification - October 27, 2026

## Result and scope

**All 334 concrete statement kinds are accounted for, but not all 334 are live verified.**
This is a representative statement-kind campaign, not exhaustive coverage of clauses, arguments,
execution paths, engines, stored module bodies, or every possible side effect.

| Evidence | Result |
|---|---:|
| Static parser/analyzer or AST checks | 334 / 334 kinds |
| Default production parser with real SQL | 318 kinds |
| Alternate-dialect SQL plus production analyzer response | 9 kinds |
| Constructed AST only, not independently executable SQL | 7 kinds |
| Representative live execution with passing declared assertions | 267 kinds |
| Attempted, but no passing live result | 3 kinds |
| No live execution evidence | 64 kinds |
| Final automated test suite | 793 passed, 0 failed, 0 skipped |

The six authoritative runtime reports contain **293 fixture executions: 289 passes and four failed
attempts**. Counts of fixtures, executions and unique statement kinds are different. A successful retry
can give a kind live-pass status without erasing its earlier failure. Only explicitly linked test-phase
statement kinds earn credit; setup statements and unexecuted definition bodies do not.

The [readable matrix](statement-matrix.md) names every kind and gap. The
[machine-readable matrix](statement-matrix.json) records observations, report hashes, input hashes,
test evidence and binary provenance. The archived evidence contains the raw reports and source snapshot.

## Executed runs

| Archived report | Executions | Passes | Notes |
|---|---:|---:|---|
| strict-results.json | 216 | 216 | Restricted fresh database per fixture |
| server-results.json | 51 | 49 | Privileged probes only inside owned disposable Docker |
| additional-results.json | 20 | 20 | Additional database/server and filesystem probes in that container |
| remaining-results.json | 3 | 1 | BULK INSERT passed; two external-table setup failures |
| governor-retry-results.json | 1 | 1 | Corrected Resource Governor initial state |
| lifecycle-results.json | 2 | 2 | Owned victim-session KILL, then container SHUTDOWN WITH NOWAIT |

No concurrent database harnesses were used. Restricted fixtures provision random disposable databases,
execute as a database-only principal and compare independently observed persisted state after closing
the session. Privileged probes were never directed at the shared developer instance. They used the
verified, loopback-bound, unmounted container recorded in environment.json, SQL Server Developer
17.00.5035 on Linux, with the optional full-text package installed.

KILL removed the owned victim SPID and an independent read confirmed its uncommitted counter update
rolled back to zero. SHUTDOWN stopped that same container after the command and the SQL log explicitly
recorded the NOWAIT request. **The actual process exit code was 255, not zero.** Docker reported no
OOM/dead/error condition. The container was subsequently removed and its local password file deleted;
cleanup.json records the identity checks. No SSMS UI, theme or settings work was part of this campaign.

## Failures, corrections and outstanding prerequisites

- **AlterServerConfigurationSetSoftNumaStatement:** verdict correctly RealWrite; expected OFF to ON,
  but the observer returned OFF to OFF. This is not credited as a live pass or a proved mutation.
- **CreateExternalTableStatement and DropExternalTableStatement:** setup failed because the chosen
  sqlserver connector location prefix was unsupported. Neither target statement is credited as executed.
- **Resource Governor:** the first run assumed disabled initial state, but found it enabled. The fixture
  now explicitly disables it in setup; the isolated retry observed zero to one and passed. Both records remain.
- **Lifecycle observer correction:** the first shutdown observer incorrectly required exit zero. Its raw
  diagnostic report is archived separately, excluded from combined counts. After requiring same-container
  identity, time ordering, no Docker failure flags and a matching NOWAIT SQL log instead, the rebuilt runner
  repeated both lifecycle probes successfully. The analyzer binary did not change between these runs.
- The remaining 64 have individual reasons in the matrix and runtime-limitations.json. They include
  unavailable Azure/Fabric/warehouse/legacy engines, HA/cluster topology, reviewed CLR binaries, real EKM
  providers, external runtimes/services, special storage and destructive recovery prerequisites. No fake
  package, fabricated path, parser success or expected server rejection is substituted for successful execution.

The seven AST-only entries are DropExternalStreamingJobStatement, DropExternalStreamStatement,
EventSessionStatement, ExternalResourcePoolStatement, ResourcePoolStatement, SelectStatementSnippet
and TSqlStatementSnippet. These are helper/base/snippet/generator representations rather than seven
independent SQL grammar forms that can all be sent to this server. Their constructed-AST behavior is
tested, and their lack of live execution remains explicit.

## What observations establish

The restricted runner checks exact typed row snapshots, selected catalogs, session verification scalars,
transaction count, visible changes and independently persisted changes. Isolated probes check declared
before/after scalars and verdicts; lifecycle probes use out-of-process observations. Every setup, execution,
verification, cleanup or assertion failure fails its fixture. No unexpected green verdict was found among
passing fixtures under these declared observers. **That is not a universal false-negative guarantee.**

The observers do not capture all physical writes, logs, external files, network effects or cryptographic
material. In particular, unchanged key counts do not prove restoration of the actual key, and affinity
AUTO to AUTO does not prove an effective configuration delta. A passing no-op/control-flow fixture
establishes only its declared assertions, not that every possible invocation is harmless. Read-only,
temporary-only, permanent mutation, session/server effects and rollback are distinguished; not every
non-read statement changes source table rows.

GuardExecutionContractTests exercises the shared guard policy through a counted test adapter. This
does not establish installed VSIX behavior, DTE interception, actual SSMS dispatch, or UI exactly-once
execution. [SSMS release acceptance](../../release-acceptance.md) remains separate.

## Negative controls

- Deliberately wrong scalar: one failed fixture, process exit 1, other 117 selected fixtures passed.
- Deliberate execution error: one execution-error fixture, process exit 1, other 117 passed.
- Partial selection with require-complete: selected fixture passed, but the completion gate exited 1.
- Invalid/mixed coverage modes: rejected before SQL; Windows argument-exception exit -532462766.
- Matrix generator: rejected failing test evidence, an interrupted strict report, and mixed analyzer hashes.

The first three controls intentionally preserve a 118-fixture input snapshot. They validate failure
handling, not the current 216-fixture runtime total. Their raw results and exit codes are archived.

## Reproduction and provenance

Use the [runner safety and setup instructions](../../../tests/SqlSafetyGuard.ServerHarness/Coverage/README.md).
Do not run the legacy discovery mode to reproduce this campaign. Run privileged fixtures only inside
a newly provisioned, verified owned container; run lifecycle last because it stops SQL Server.
Reproduction on a differently provisioned engine can legitimately yield different feature failures.

```powershell
dotnet test tests/SqlSafetyGuard.Tests -c Release --logger "trx;LogFileName=all-kind-tests.trx" --results-directory harness-out/final-tests-complete
node tests/SqlSafetyGuard.ServerHarness/Coverage/generate-matrix.mjs docs/verification/2026-10-27-all-kinds harness-out/final-evidence/all-kind-tests.trx harness-out/final-evidence/strict-results.json harness-out/final-evidence/server-results.json harness-out/final-evidence/additional-results.json harness-out/final-evidence/remaining-results.json harness-out/final-evidence/governor-retry-results.json harness-out/final-evidence/lifecycle-results.json
```

All authoritative runtime reports use analyzer SHA-256
06086DE72295F47DBEFEDF78B8A52054C26603E50B1596849BA2D95133DFF2D7.
Harness hashes differ because coverage guards and the lifecycle observer were refined between runs;
each report retains its own hash. Source changes are uncommitted against base commit
3857a9abad9edc804c421f9482710c8a704391d9; the archive includes a final source snapshot, not a claim
that every earlier runner binary was built from that final snapshot. Historical embedded fixture SQL
and recorded input hashes remain authoritative for their individual runs.

The raw evidence archive, evidence.zip, is kept out of git (binary, about 1.9 MB); [SHA256SUMS.txt](SHA256SUMS.txt)
records its hash alongside the matrix hashes, so a copy can be verified. The archive contains raw reports, TRX,
controls, environment/cleanup receipts and source/corpus files. It excludes connection credentials. The earlier
[strict baseline](../2026-10-27-strict-coverage.md) and its archive remain unchanged as historical evidence.
