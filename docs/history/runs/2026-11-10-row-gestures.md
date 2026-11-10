> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Pane row gestures

Unpublished UI follow-up to the [nested procedure pane](2026-11-10-nested-procedure-pane.md).

## Result

- Outer and nested object rows use the same gestures: single click highlights all statements for that object;
  double click toggles its details; an individual statement button highlights only that statement.
- Highlighting leaves the SQL selection intact. Several rows can remain expanded, including across reanalysis.
- Body and safe-section headings retain their single-click toggle. Their chevrons point down when collapsed and
  up when expanded. The paragraph beginning “Based on this window's text” is removed from the body.
- Replacing the query with a new single procedure still opens its body automatically even when previously expanded
  objects no longer appear in the result.
- Analyzer verdicts, execution guard policy, settings storage and the production text-only boundary are unchanged
  by this follow-up.

## Verification

| Check | Result |
|---|---|
| Locked offline suite | PASS: 1,693 .NET tests, 10 Python tests, 527 inventory rules checked for freshness |
| Final .NET rerun | 1,693 PASS |
| Visual Studio MSBuild | Final forced Debug and Release rebuilds PASS; existing `DialogChrome` threading warnings remain |
| Release WPF pane interactions | 29 PASS |
| Isolated SSMS editor highlights | 21 PASS plus fresh-analysis clearing PASS |
| Isolated SSMS procedure checks | 25 PASS, including outer and nested row gestures, individual highlights, preserved SQL selection and expansion state |
| Light visual capture | 20 captures; procedure pane inspected at the normal 300-DIP width |
| Cleanup | Owned isolated SSMS exited cleanly; settings restored byte-for-byte with identical SHA-256 |

The final evidence is `row-gestures-20261110/capture3` in the external development evidence directory. The first
diagnostic run loaded an older test payload; the second exposed the new-procedure automatic-expansion edge case.
Both diagnostic sessions cleaned up, and neither is counted as the final acceptance run. Raw captures, events,
restoration records and installation helpers stay outside the repository.

Release VSIX SHA-256: `28070ADE72C300DB346772E861FD61EE17A02BE2FBB8577D0E37700B89EC38BF`.

Live gestures were dispatched through the Debug-only in-process event channel against rendered controls. These
checks do not claim physical mouse or keyboard coverage. No SQL or Execute command was dispatched; no new server
campaign, all-theme pass or public release is implied. The published release remains 1.0.4.
