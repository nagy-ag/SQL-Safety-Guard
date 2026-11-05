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
| Clean committed-source packaging gate | PASS: full offline gate, Release rebuild, all 45 WPF checks and unsigned asset validation |
| Published assets and fresh downloads | PASS: all five API hashes and downloaded bytes match local assets |
| GitHub CI at release source | PASS: main and v1.0.2 |

An initial packaging attempt and one fixture failed because the system disk was full. Temporary files were redirected
to another volume and the full offline suite and builds then passed. No analyzer limit or test assertion was relaxed.
The arrow checks instantiate the actual WPF templates. The isolated Debug SSMS run also produced 18 Light-theme captures,
including both arrow states and the highlighted/cleared editor. Its settings were restored byte-for-byte and SSMS exited
cleanly. The original isolated extension files were restored and match the backup; normal-profile settings were unchanged. See the [focused UI record](history/runs/2026-11-05-pane-interaction-1.0.2.md).
This is UI evidence, not actual F5 execution.

## Scope and retained limits

This UI update does not repeat the SQL/server campaign, exact Release-artifact execution acceptance, every SSMS version,
the full theme matrix, company signing, or signed-copy verification. Prior evidence and unresolved observation gaps remain
in [verification/status.md](verification/status.md). No SQL execution or new cloud resource is needed for these changes.

## Published result

[SQL Safety Guard 1.0.2](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.2) is stable and Latest, published
**2026-11-05 at 15:27:33 UTC**, from source `5ceeeb0ed1a619cccd50d332d074efb205bf43ac`, annotated tag `v1.0.2`.
Main and the tag were pushed without force; earlier releases were retained. All five GitHub API asset hashes and fresh
downloads match local validated assets. See the [publication record](history/runs/2026-11-05-release-1.0.2-publication.md)
for exact checksums, the two successful source/tag CI runs and the unchanged-gate retry after a tooling fixture timeout.
This record update is documentation only; the fixed tag identifies the published product source.
