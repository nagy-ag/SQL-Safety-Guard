# Release 1.0.2 acceptance

Updated **2026-11-05** for the owner-requested **1.0.2 pane interaction update**. The owner authorized updating
GitHub `main`, publishing 1.0.2 and updating the changelog. Earlier releases are retained. GitHub packages remain
unsigned and support **SSMS 22.0 and later 22.x releases**, 64-bit, bounded to `[22.0,23.0)`.

## Changes

- Highlights appear only for the finding group explicitly chosen in the pane. Red/yellow statements have no idle tint,
  and cursor movement never activates a highlight.
- Editor clicks, keys, caret/SQL-selection changes, buffer edits, pane-background deselection and pane collapse clear
  the chosen highlight. A refresh cannot restore a choice that was cleared.
- Double-click/Enter navigation keeps the requested highlight without selecting SQL text or changing the execution scope.
- The **No permanent changes detected** arrow points **up when collapsed** and **down when expanded**. The toggle action
  and automatic opening of the safe section are retained.
- Updates [CHANGELOG.md](../CHANGELOG.md), the user guide and all product version numbers. Analyzer verdicts and F5
  confirmation policy are unchanged.

## Local validation

| Check | Result |
|---|---|
| Locked offline suite | PASS: 1,635 .NET tests, 10 Python tests, 527 current rules |
| Debug and Release solution builds | PASS |
| Release WPF interaction scripts | PASS: 26 pane, 15 confirmation, 4 settings checks |
| Arrow template and interaction | PASS: both directions, toggle state, content visibility and expand/collapse actions |
| Isolated SSMS highlight checks | PASS: 17 checks plus fresh-analysis clearing; actual buffer and adornment layer |
| Light-theme visual check | PASS: chosen/cleared SQL and collapsed-up/expanded-down arrows; 18 captures |
| Clean committed-source packaging gate | Pending source commit |

An initial packaging attempt and one fixture failed because the system disk was full. Temporary files were redirected
to another volume and the full offline suite and builds then passed. No analyzer limit or test assertion was relaxed.
The arrow checks instantiate the actual WPF templates. The isolated Debug SSMS run also produced 18 Light-theme captures,
including both arrow states and the highlighted/cleared editor. Its settings were restored byte-for-byte and SSMS exited
cleanly. The original isolated installation is being restored. See the [focused UI record](history/runs/2026-11-05-pane-interaction-1.0.2.md).
This is UI evidence, not actual F5 execution.

## Scope and retained limits

This UI update does not repeat the SQL/server campaign, exact Release-artifact execution acceptance, every SSMS version,
the full theme matrix, company signing, or signed-copy verification. Prior evidence and unresolved observation gaps remain
in [verification/status.md](verification/status.md). No SQL execution or new cloud resource is needed for these changes.
Publication details and checksums will be recorded after the clean-source package is validated and uploaded.
