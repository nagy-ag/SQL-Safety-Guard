> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Verification cleanup, 2026-11-03

This cleanup follows the [repository maintenance](2026-11-03-repository-maintenance.md) in the same uncommitted working
tree based on `0dbe4b4`. Existing work was preserved. No SQL, SSMS session, installation, scheduled-task operation, commit
or push was performed.

## Removed and retained

- Removed `tools/verification/Freeze-Wave3.ps1` and `Run-Wave3Full.ps1`, which only forwarded to maintained launchers.
  The orchestration fixtures now invoke `Freeze-Campaign.ps1` and `Run-Campaign.ps1` directly with their existing parameters.
- Removed `tools/verification/Register-Wave2Overnight.ps1`, which was already a refusal stub. Existing external frozen
  launchers and scheduled tasks were not inspected or changed. Historical reports retain their original command names;
  the [current tooling map](../../../tools/verification/README.md) records the replacements and removal.
- Simplified `ExpandedPaneContractTests` and `GuardUiContractTests`: removed exact badge/header dimensions, decorative
  glyphs/shapes, font choices, border widths and presentation ordering/alignment constraints. Header discovery now uses
  named content. Navigation handler checks remain explicit. Ledger height must still be finite and positive, without
  requiring a specific pixel value.
- Retained checks for Cancel as the default/Escape action, button-only confirmation, target warnings, passive ledgers,
  scrolling, theme resources, named status/count fields, row bindings/expansion, settings provenance and navigation.
  Analyzer safety regressions, resource limits, installer checks, verification-oracle checks and the production
  no-server-access checks remain. No existing .NET test case was removed; the header test was renamed for its actual scope.
- Retained Wave3 fixtures, generators, `WaveClr.cs` and historical reporting recipes. The maintained freezer still consumes
  Wave3 fixtures and the CLR helper. Historical names alone are not evidence that a dependency is obsolete. Historical
  reports are not part of the offline gate. A complete redundancy audit or deletion of the legacy discovery runner is
  not claimed by this cleanup.

## Validation

| Check | Observed result |
|---|---|
| Focused UI/guard/access-boundary contracts | 72 passed |
| Maintained launcher orchestration fixtures | 3 Python tests passed; git/build/validation stubbed, SQL execution forbidden |
| Full `scripts/Verify-Offline.ps1` | Exit 0: 1,592 .NET tests, 527 generated rules current, 10 Python tests passed |

Raw offline-gate output remains outside the repository under `../harness-out/verification-cleanup-*/offline.log`.
Source hashes were compared with the beginning of this cleanup: no production source or existing historical snapshot
was changed. The extension therefore retains its text-only boundary; these changes add no connection or server access.
No new VSIX build, runtime rendering/theme review, live guard routing, server campaign or remote CI result is claimed.

The current tooling map distinguishes routine offline checks from targeted UI/server acceptance and retained historical
recipes. Removing unused command aliases reduces supported entry points; it does not replace behavior tests with build success.
