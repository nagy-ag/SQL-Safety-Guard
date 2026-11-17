> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Dynamic SQL held in variables

Local, unpublished follow-up to 1.0.5. The owner requested variable-backed dynamic SQL analysis and installation,
then explicitly retained the existing UI. No release was published and no commit or push was requested.

## Result and boundaries

- Resolve exact `varchar`/`nvarchar` strings from straight-line `DECLARE`, `SET`, copies, `+` and `+=` before
  `EXEC(...)` or qualified system `sp_executesql`. Ordinary `BEGIN/END` blocks are supported.
- Reuse the existing dynamic parser, verdict rules, target/row-scope annotations and EXEC source positions.
  Nested dynamic batches and visible procedure bodies have independent variable maps.
- Do not carry values across `GO`, into another dynamic invocation, or from outside the executed selection.
- Branches, loops, jumps and TRY/CATCH disable variable proof for their batch. Unmodeled statements clear known
  values, including SELECT/UPDATE assignments, cursor FETCH and procedure outputs. Unsupported types/conversions,
  ambiguous variable casing, non-ASCII varchar input, truncation and expansion limits retain yellow warnings.
- The pane, confirmation dialog, settings and guard policy are unchanged. No production SQL connection,
  server lookup or SQL execution capability was added.

## Offline and build evidence

| Check | Result |
|---|---|
| Focused variable regression suite | 45 PASS; assignments, concatenation, writes/targets, scope, loops/branches, output values, truncation, nesting and expansion limits |
| Full locked offline gate | 1,774 .NET tests and 10 Python tests PASS |
| Generated references | Statement verdicts regenerated; inventory includes six new semantic rules, 533 rules total |
| Visual Studio MSBuild | Debug and Release PASS; existing DialogChrome threading warnings remain |
| Existing Release WPF controls | 29 pane interaction checks and 15 confirmation checks PASS |

The existing sample's variable-backed DELETE now correctly expects RealWrite instead of Unknown. Shared guard
contract cases include resolved reads, resolved writes and unavailable variables across all warning modes and choices.

## Real SQL Server evidence

The checked-in [focused corpus](../../../tests/SqlSafetyGuard.ServerHarness/Campaign/cases/variable-dynamic.jsonl)
ran serially on fresh owned campaign database sets, using the frozen Release analyzer. All 17 cases executed:

- Eight ReadOnly cases and one TempOnly case: no observed permanent changes.
- Three resolved RealWrite cases: actual counter updates observed and attributed.
- Four Unknown cases: actual writes through SELECT assignment, procedure OUTPUT, branching and loop reassignment;
  none were incorrectly promoted to green.
- One Unknown truncation case: the executable truncated text only selected a constant, so the outcome is expected
  NOISE rather than proof that arbitrary truncation is safe.

Final outcomes: **16 PASS, one NOISE, zero false greens, zero expectation mismatches, zero execution failures**.
This focused run does not replace the broad campaign or establish exhaustive string/control-flow coverage.

Analyzer SHA-256: `C87A4DFB4CC9C79447C9468BA9252262BC84FFFE08988E5FFD451D721AE36E3D`.
Campaign identity: `706ECE6A77E6CF5D079376AFDD2CB15A86245BCAC1C6D08C5F56C001819B8304`.
Server version: SQL Server 2025, `17.00.1000`.

## SSMS and installation evidence

- The Debug candidate was installed and configuration refreshed in the owned Exp profile.
- One Light-theme run produced 18 captures, including variable-backed green, yellow and red panes and the existing
  yellow/red confirmation dialogs. The green/red panes and red dialog were visually inspected.
- SSMS ran on an undisplayed private desktop through the existing Debug channel; no real mouse, keyboard,
  clipboard, foreground focus, SQL connection or Execute dispatch was used. The isolated process exited cleanly;
  its settings were restored byte-for-byte with equal SHA-256 hashes.
- The Release VSIX was installed through the supported installer in the normal per-user SSMS 22.11 profile,
  followed by configuration refresh. The installed analyzer hash equals the frozen, server-tested binary above.
  Normal-profile settings hashes were identical before and after installation. Previous installed files and a
  known released VSIX were retained externally for recovery.
- This is a local development build retaining the 1.0.5 package identity. Its VSIX SHA-256 is
  `E1D31650406169F7F769C7FCBA5C341A0CDC6BFB3AD86365E33094C22AAEBE5E`.

Raw run output, frozen input/binary, screenshots, restoration and installation receipts stay outside the repository
under the task evidence directory `variable-dynamic-2026-11-11`. No physical F5 delivery, new execution-routing
campaign, full theme matrix or new published release is claimed.
