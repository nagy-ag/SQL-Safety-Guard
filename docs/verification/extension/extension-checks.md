# Extension execution checks

The maintained execution-check driver is documented in the [ExtensionChecks README](../../../tests/SqlSafetyGuard.ExtensionChecks/README.md).
It launches an owned isolated SSMS profile, uses Debug-only hooks, and executes SQL against an owned disposable database.
It is separate from offline UI interaction tests and visual captures. Read local machine rules before running it.

## Evidence and limits

The latest retained execution-target campaign is the [October 28 snapshot](../../history/2026-10-28-extension/README.md):
35 PASS, zero FAIL, one toolbar BLOCKED across 36 scenarios. Those counts describe that historical Debug build, not the
current working tree. Original ledgers and events are preserved with checksums. No new live execution evidence is implied
by later builds, unit tests or documentation changes.

Physical keyboard shortcuts are not synthesized. Their command bindings and common Query.Execute dispatch are inspected.
Toolbar UIA availability, successful results-file Save, custom stored-procedure shortcuts and future SSMS host versions
remain separate coverage limits. The guard reads the focused editor's execution candidate and cross-checks host identities;
an ambiguous target requires a warning even in Off or Red-only mode.

## Result contract

The driver retains all scenario diagnostics and writes a machine-readable summary. FAIL makes the run fail. BLOCKED is
incomplete unless the caller names the exact expected blocked scenarios; that exception remains visible in the summary
and is never counted as PASS. Missing expected scenarios, an empty result set and duplicate scenario names cannot pass.
Setup/runtime failures abort, and the owning wrapper still performs cleanup in finally.

The result-aggregation helper is exercised offline without loading the UI client or opening a SQL connection. Its tests
establish result handling only. Real command routing, captured-text identity, submission count and side effects require
the separate integration observations described in the driver README.

For visual acceptance without SQL, use [theme-capture.md](theme-capture.md). For current qualified evidence, use
[status.md](../status.md); for component responsibilities, use [architecture.md](../../architecture.md).
