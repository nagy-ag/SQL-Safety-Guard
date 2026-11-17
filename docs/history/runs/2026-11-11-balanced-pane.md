> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Compact object and operation rows

Local, unpublished UI follow-up to 1.0.5, including the existing local variable-backed dynamic SQL changes.
The owner requested implementation and installation for a local trial. No commit, push or release publication.

## Result

- One compact object row with its worst overall verdict and complete red/yellow/green operation counts.
- All/Writes/Unknown/Safe are display filters. Hidden operations retain their contribution to counts and overall risk.
- Single-click navigates to the first visible operation and briefly highlights matching SQL; double-click expands the
  operation list. Each operation has its own click/navigation and double-click short reason. Arrows and left/right keys
  also control expansion. Existing execution selections are preserved; highlights expire after 1.8 seconds.
- Separate procedure-body potential effects use the same rows with an independent filter. They do not enter submission
  counts. New objects and bodies start collapsed; matching expansion/filter state survives reanalysis.
- No classifier, F5 policy, connection handling or SQL execution capability was changed for this pane work.
- [Sample](../../../samples/04-compact-pane.sql) parses without errors and produces a real red/yellow procedure object,
  repeated writes to one table, and temporary-only operations. It was analyzed without execution.

## Verification

| Check | Result |
|---|---|
| Full .NET suite | 1,783 PASS |
| Locked offline gate | 1,783 .NET and 10 Python PASS; generated references and 533-rule inventory fresh |
| Builds | Debug and Release PASS; two existing DialogChrome threading analyzer warnings remain |
| Release WPF scripts | 33 pane interaction checks and 15 confirmation ledger/dropdown checks PASS |
| Isolated SSMS, Light | 36 mixed-verdict checks plus actual highlight-expiry receipt; 21 editor checks plus refresh; 25 procedure-body checks PASS |
| Layout | 29 captures in the full run, 21 in the expanded-body follow-up; inspected normal-width expanded rows |
| Cleanup | Both owned sessions closed cleanly; isolated settings restored byte-for-byte |
| Normal-profile installation | Supported VSIXInstaller install and configuration refresh succeeded; five installed payload/manifest hashes match the Release outputs; settings unchanged |

The first locked-gate attempt hit the existing active-cancellation test's three-second timeout. The focused test then
passed in isolation, and the complete locked gate passed on rerun. No analyzer or cancellation-test change was made.

The three-colour same-object fixture is synthetic presentation evidence. Ordinary reads are not currently assigned to
every referenced object, unknown targets are not guessed, and multiple actions mapped to one source span remain separate
operations. These UI checks raise in-process events; they do not establish physical input, live F5 execution, a new server
campaign or an all-theme pass. Normal-profile launch and personal acceptance remain for the owner.

## Package identity

Manifest version remains 1.0.5 for this local trial; distinguish this candidate by SHA-256:

`b1401a74fd1db51807e3d80150b0e4ac6ddf92205ad6279c4c630338b8d85924`

The previous local package and installed extension were backed up outside the repository. Raw logs, captures, settings
backups, installation receipts and the frozen candidate also remain outside the repository.
