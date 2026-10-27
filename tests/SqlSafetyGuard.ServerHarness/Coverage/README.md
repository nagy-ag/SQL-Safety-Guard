# Strict statement execution coverage

This is a curated executable corpus, not SQL discovery from arbitrary strings. The manifest inventories all 334 concrete TSqlStatement types in ScriptDom 180.140.0. A fixture link means a kind is intentionally exercised, not that it passed. Missing means not covered; safety-excluded means deliberately not executed, with a per-kind reason. Neither status earns coverage. Clause combinations and execution paths are not exhaustive.

## Run

From the repository root, with a local SQL Server and a sysadmin Windows login for provisioning:

```powershell
dotnet build src/SqlSafetyGuard.Analyzer -c Release
dotnet run --project tests/SqlSafetyGuard.ServerHarness -c Release -- `
  --analyzer src/SqlSafetyGuard.Analyzer/bin/Release/netstandard2.0/SqlSafetyGuard.Analyzer.dll `
  --coverage tests/SqlSafetyGuard.ServerHarness/Coverage `
  --server .\SQLEXPRESS --out harness-out/strict
```

Use --validate-coverage to validate without connecting. --filter and --limit select subsets. --require-complete makes untested, excluded or missing kinds fail the completion gate, even when all selected fixtures pass. A normal zero exit means only that selected fixtures passed. Do not combine strict mode with discovery inputs or --keep. Run only one database harness at a time.

## Fixture contract

Schema version 1 stores stable IDs, explicit statementTypes, setupSql, sql, expectedVerdict under default analyzer settings, expectedPermanentEffect, scalar verifySql, typed expectedVerifyScalar and prerequisites. Optional expectedVisibleChange / expectedPersistedChange distinguish uncommitted and rolled-back writes; expectedTransactionCount defaults to zero. A red verdict can correctly accompany no net mutation (unreachable or rolled-back writes). An Unknown verdict may accompany an actual procedure/runtime-dynamic write.

The loader validates inventory/version, unique IDs, bidirectional links and parsing of all three SQL phases. Only sql is analyzed. Setup-only types and definition bodies do not automatically earn kind coverage. The corpus is reviewed, trusted executable test code: this runner is not a sandbox for hostile SQL. Review new fixtures and their observer coverage before executing them.

## Isolation and observations

Each fixture creates a new randomly named SqlGuard_Coverage_<guid> database. Only the provisioning/independent observation connections use the Windows login. Fixture setup and execution use EXECUTE AS USER for a database user without a login, with db_owner only inside that new database. Backup permissions are denied, auto statistics disabled and server-control permission checked. Cleanup closes the session and drops only the database this fixture created. Existing SqlGuard_Prod / SqlGuard_Dev, SSMS rules/themes, and permanent tempdb objects are not reset or cleaned.

After setup, the runner captures exact sorted rows (including duplicates, typed values and explicit nulls) plus selected catalogs. It checks the verification scalar and transaction count, captures the in-session state, closes the session (rolling back outstanding work), and captures persisted state independently. Observation resets ROWCOUNT and IMPLICIT_TRANSACTIONS so fixture settings do not truncate reads or start observation transactions. Session-only behavior is checked by explicit scalar assertions, not the database fingerprint. First sequence use is observed through last_used_value as well as current_value.

Catalog coverage includes objects/modules, columns/indexes/statistics, sequence/identity state, schemas/users/roles/permissions, types/synonyms/constraints, trigger enablement, XML schema collection membership, partition functions/ranges/schemes and selected database flags. It is not a complete oracle for every catalog, XML schema component, physical write, transaction log, external file, remote server or network effect. A no-op or rolled-back statement can perform physical work without changing these snapshots. Extend the observer before adding fixtures relying on uncaptured state.

Syntax/setup/execution/verification/observation/cleanup errors never count as passes. Unexpected state, scalar, transaction count or verdict fails the fixture. Every completed case checkpoints coverage-results.json and coverage-report.md. JSON records the full executed fixture, observations/diffs, server version and analyzer/harness/input hashes. Keep these evidence files with any published results; do not edit input files or binaries during a run.

Default DBCC CHECKTABLE failed under the restricted execution context during development; the curated fixture uses WITH TABLOCK, NO_INFOMSGS instead. This tests that variant only, not the default snapshot-based operation. Server-wide, shutdown/login/cluster/configuration and filesystem operations are deliberately excluded rather than executed against the shared developer instance.

## Guard integration boundary

GuardExecutionContractTests calls the same GuardDecision used by the VSIX and tests Off/Red/YellowRed, approve/cancel, failure handling and exact typed-name policy. The counted execution callback is a test adapter, not SSMS command dispatch. This fast suite does not prove DTE interception, dialog rendering, active-tab selection or exactly-once delivery in SSMS. Existing release acceptance evidence remains separate.

See [combined dated evidence](../../../../docs/verification/2026-10-27-all-kinds/README.md) for current results and remaining gaps. The earlier [strict baseline](../../../../docs/verification/2026-10-27-strict-coverage.md) is preserved as historical evidence.

## Static coverage of all 334 kinds

The passing unit suite checks 318 kinds using real SQL parsed by TSql180Parser, nine using the exact legacy/Fabric parser plus the production analyzer's response, and seven through constructed AST fallback tests. The last seven are shared base/snippet/generator nodes, not independently executable SQL. Sources are parser-fixtures.json, dialect-fixtures.json and ast-only-fixtures.json. Parser samples adapted from Microsoft's MIT-licensed ScriptDom tests retain their source filenames and ScriptDom-LICENSE.txt. None of these static tests establishes a live database effect.

## Privileged probes: disposable Docker only

The additional isolated corpora run only with SQLGUARD_COVERAGE_CONTAINER and SQLGUARD_COVERAGE_CONNECTION supplied to the process. The container name must match sqlguard-cov-[16 lowercase hexadecimal characters], its sqlguard.coverage label must be disposable, it must have no mounts, and the SQL MachineName must match. Provision that hostname explicitly when creating the container. Bind the SQL port to loopback only. Do not put the password in checked-in scripts, reports, or command-line arguments. Never point privileged probes at the shared development instance.

Run exactly one SQL harness at a time. Each isolated fixture uses the fixed SqlGuard_IsolatedCase database inside this exclusively owned container; database rename/restore probes also use SqlGuard_IsolatedRenamed. The corpora can create server configuration, credentials, certificates, audit/event sessions, backups and files. Review all fixture SQL and dispose of the whole verified container afterward. Before rerunning, delete only prior fixture-owned export files and backup history in that same container; otherwise expected-before assertions deliberately fail.

With the two environment variables set:


    dotnet run --project tests/SqlSafetyGuard.ServerHarness -c Release --no-build -- --analyzer src/SqlSafetyGuard.Analyzer/bin/Release/netstandard2.0/SqlSafetyGuard.Analyzer.dll --isolated-coverage tests/SqlSafetyGuard.ServerHarness/Coverage/isolated-fixtures.json --out harness-out/isolated

Repeat with additional-isolated-fixtures.json and remaining-isolated-fixtures.json. These are distinct from expanded-fixtures.json, which preserves a proposed contributor corpus; fixtures.json is the authoritative restricted-runtime corpus. The BULK INSERT fixture requires the run-owned /var/opt/mssql/data/sqlguard-coverage-bulk.csv containing two lines, 1 and 2. Full-text fixtures require the optional full-text package. Feature absence and unsupported external connectors remain failures, not skipped passes.

Run --isolated-lifecycle as the LAST SQL command. It opens its own victim session, verifies KILL removes that session and rolls back its uncommitted counter update, then executes SHUTDOWN WITH NOWAIT and observes the same Docker container stop plus its matching NOWAIT shutdown log out of process. It records the actual exit code without treating zero as a requirement; the observed Linux run exited 255, with no Docker OOM/dead/error flag. It intentionally stops the database server. --validate-coverage validates without opening SQL; --require-complete cannot succeed for these partial corpora. A report's Complete=false means not all 334 kinds are live verified; it does not mean a selected run was interrupted. Check planned/executed counts and individual statuses.

## Combined evidence and honest limits

runtime-limitations.json records a concrete prerequisite or observation limitation for each kind lacking a runtime fixture. CoverageMatrixTests prevents unexplained inventory gaps; it does not turn a proposed fixture into a pass. The report generator requires a passing TRX test artifact and runtime reports with binary hashes. It rejects duplicate static categories, unknown kinds, mixed analyzer binaries and unexplained gaps.

Example, passing archived runtime result paths explicitly:

    node tests/SqlSafetyGuard.ServerHarness/Coverage/generate-matrix.mjs <output-directory> <passing-tests.trx> <strict-results.json> <isolated-results.json> <additional-results.json> <remaining-results.json> <lifecycle-results.json>

The generated statement-matrix.json is the combined source of truth, not the restricted manifest's missing/excluded counts. A live pass proves representative execution and declared assertions, not every possible side effect. In particular unchanged key counts do not prove cryptographic material was restored, a metadata-only external table probe does not exercise remote I/O, and successful control-flow samples do not cover all branches. Failed attempts and absent engines/prerequisites remain explicit. No headless result proves VSIX installation, DTE interception or SSMS command delivery.
