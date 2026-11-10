> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Nested procedure pane

Unpublished working-tree UI follow-up to [procedure-body inspection](2026-11-10-procedure-body.md).
The owner selected the normal safety pane nested inside the expanded stored-procedure row.

## Result

- Procedure definitions and nested objects share the normal single-line object/action/first-line template.
- Body objects use the same section-heading template as the main pane: permanent changes, unknown effects and
  operations allowed by settings. The nested green section starts collapsed. Counts describe objects in that scope.
- A thin indentation guide separates the deferred body from the definition. Whole-target warnings remain red;
  internal statement buttons still navigate without selecting SQL or changing the execution header counts.
- Nested keyboard and double-click handling stays inside the body. Reanalysis preserves body expansion, open
  object rows and the nested safe section. The initial diagnostic run caught collapsed body state being overwritten
  when reusing a model; snapshotting the previous state fixed it, and the final rerun passed.
- Analyzer verdicts, F5 policy, settings storage and the production text-only boundary are unchanged by this follow-up.

## Verification

| Check | Result |
|---|---|
| Locked offline suite | PASS: 1,693 .NET tests, 10 Python tests, 527 inventory rules checked for freshness |
| Visual Studio MSBuild | Debug and Release PASS; the two existing `DialogChrome` threading warnings remain |
| Release WPF pane interactions | 26 PASS |
| Isolated SSMS procedure checks | 17 PASS, including a shared row template, one-line definition geometry, deferred counts, exact statement navigation, preserved SQL selection, nested keyboard routing and fresh-model expansion state |
| Isolated editor highlights | 17 PASS plus fresh-analysis clearing PASS |
| Light visual capture | 20 captures; the procedure pane was inspected at the normal 300-DIP width |
| Cleanup | Owned isolated SSMS exited cleanly; settings restored byte-for-byte with identical SHA-256 |

The retained final run is `nested-body-20261110/capture2` in the external development evidence directory;
`capture1` retains the diagnostic failure and its successful cleanup. Raw events, captures, restoration records and
installation helpers remain outside the repository.

Release VSIX SHA-256: `B7FBECCD7900F7797D2ED6BBD1CA0CFDD513B67414D14E598813DE7488C5760F`.

These are UI and offline checks. No SQL or Execute command was dispatched; no new server campaign, physical keyboard
test, all-theme pass or public release is implied. The published release remains 1.0.4.
