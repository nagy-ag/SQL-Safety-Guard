> **Snapshot — not maintained.** Current state: [verification/status.md](../verification/status.md). All current docs: [docs/README.md](../README.md).

# Strict runtime verification — October 27, 2026

## Outcome

The systematic, non-UI verification foundation is implemented and its curated corpus passes. **Universal T-SQL runtime coverage is not complete.** Classification of all statement kinds is not proof that every clause, argument, execution path or server side effect has been tested.

- 456 automated tests passed, including 40 new corpus-validation and shared guard-contract tests.
- Release harness build passed with zero warnings and zero errors.
- 118/118 explicit fixtures executed successfully against local SQL Server instance .\SQLEXPRESS, reporting engine version 17.00.1000.
- All 334 concrete statement kinds in ScriptDom 180.140.0 are inventoried: 103 have passing linked runtime fixtures, 198 remain missing, and 33 are safety-excluded. Exclusions are not passes.
- No false green was observed by the current data/catalog oracle in these 118 cases. This is bounded evidence, not a proof of absence.
- A final read-only sys.databases query returned no SqlGuard_Coverage_* databases. No business databases, existing SqlGuard_Prod/SqlGuard_Dev databases, themes or SSMS settings were changed by this run.

## Reproducible evidence

See the [fixture and statement matrix](2026-10-27-strict-coverage-report.md), full raw JSON evidence archive (`2026-10-27-strict-coverage-results.zip`, kept out of git), [negative-control results](2026-10-27-negative-controls.json), and [runner instructions](../../tests/SqlSafetyGuard.ServerHarness/Coverage/README.md).

The archive contains coverage-results.json (approximately 10 MB uncompressed), including complete fixture SQL, analyzer findings, row/catalog observations, differences, per-case server versions and source/binary hashes. Archive SHA-256: 223d2231ef7e1c2f940f26e3a868d3c70021d22cb61cbf470b6c7abb55c64c47.

Authoritative run started 2026-10-27T15:28:49.7053021+00:00 and completed 2026-10-27T15:30:25.7246995+00:00. The inputs and binaries were unchanged during this run.

- Analyzer SHA-256: 06086DE72295F47DBEFEDF78B8A52054C26603E50B1596849BA2D95133DFF2D7
- Harness SHA-256: DBDF6A20051BA057B86812C3FBDFD097B0677AF6A43AB7222589BA9FE17EAF3F
- Manifest SHA-256: 1EBB5C1F98132865E96330C13AE3198F79F1BE3383E6737D147E0C7C1E67809A
- Fixtures SHA-256: 71C4DBC6315EF40B42706A73A6E4A6873AEAA7EBF16D7AE5D2F76B128A3D9B4F

## Method and safety boundary

Each fixture receives a newly generated disposable database. Administrative credentials provision and remove only that run-owned database; fixture SQL executes as a database-scoped user without a login, with backup denied. The harness analyzes only the tested SQL, not setup. After setup it captures sorted typed table rows and selected catalogs, runs the SQL, verifies a scalar and transaction count, captures visible state, closes the fixture session, then observes persisted state through a separate connection. Closing the session rolls back uncommitted work. Cleanup failures and execution errors are failures, never passes.

The observer covers selected logical data and metadata, not every catalog, physical page/log write, external file, remote server, network or nontransactional side effect. Temporary-table changes are intentionally not permanent-data changes. Red SQL can produce no net state change because it is unreachable, a no-op or rolled back; yellow opaque procedure/dynamic SQL can actually write. These cases are explicitly represented.

The current curated fixtures are trusted test inputs. Database-scoped impersonation is defense in depth, not permission to run arbitrary hostile SQL on this shared instance. Server-wide configuration, login/cluster operations, backup/restore and filesystem-affecting operations remain explicitly excluded. Use a separate disposable server/container and extend the observer before executing those families.

## Negative controls

Three sequential checks confirmed non-success reporting:

1. An intentionally incorrect scalar expectation produced status fail and exit 1.
2. A syntactically valid SELECT against a missing table produced execution-error and exit 1, not pass.
3. A passing selected fixture with --require-complete produced exit 1 because the overall inventory remains incomplete.

The first two filters also matched SELECT INTO fixtures; those passed independently. The completeness check used --limit 1. An initial PowerShell invocation incorrectly splatted a string as characters and failed argument parsing; the explicit-argument rerun above supplies the recorded completeness evidence. Canonical corpus files were never modified for negative controls.

## Findings and remaining work

No analyzer defect was demonstrated by the final curated corpus. During harness development, sequence first-use state required observing last_used_value, and an ALTER USER fixture needed a non-default starting schema to make its expected mutation meaningful. These were oracle/fixture corrections, not analyzer fixes.

Default DBCC CHECKTABLE failed under the restricted database-scoped execution context. The passing fixture explicitly uses WITH TABLOCK, NO_INFOMSGS; the default snapshot-based variant remains unverified and must not be inferred to pass.

Missing families include Service Broker, keys/certificates/encryption, full-text/spatial/XML index variants, external/cloud-specific objects and numerous ALTER variants. The manifest is the exact backlog. Even filling all 334 kinds would not exhaust all clause combinations or runtime paths. The next valuable step is expanding fixtures and observations by risk family, with a disposable server for privileged operations, rather than reporting untested kinds as covered.

## Extension integration boundary

The new headless guard tests exercise the production GuardDecision logic for guard modes, approve/cancel, failure behavior and typed database-name policy through a counted execution adapter. They do **not** execute the installed VSIX, SSMS DTE command dispatch, dialogs or editor routing; they do not independently establish exactly-once execution inside SSMS. Prior [release acceptance](../release-acceptance.md) remains separate. No desktop automation or theme checks were performed in this task.

Terra supplied the initial reflected inventory and 14 proposed cases. The parent expanded the corpus, implemented the strict runner/oracle and guard-contract tests, executed the server checks, corrected the measurement issues, and preserved this qualified evidence. No commit or push was created for this task.
