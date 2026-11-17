> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Source navigation and continuous highlights

Local, unpublished changes on top of the inline procedure/dynamic-SQL pane. Manifest version remains 1.0.5.

## Result

- Object and operation headers have no **All rows** or **Drops object** badge. Whole-target explanations appear only
  when the individual operation is expanded, including operations in procedure previews and dynamic SQL.
- A Dynamic SQL container navigates to EXEC and highlights its visible execution effects, with neutral dashed outlines
  at contributing SQL strings. A decoded operation navigates to and colours its own string fragments, with a neutral
  outline at EXEC. Expanded details provide **SQL source** and **Executed at** navigation links.
- The declaration's own operation retains its normal verdict. Source metadata follows proven variable copies,
  concatenation, +=, reassignment, escaped quotes, GO offsets and nested dynamic SQL. Finding execution offsets remain
  anchored to EXEC. Missing source metadata falls back to EXEC; runtime SQL stays unknown. Value proof and guard policy
  are unchanged.
- Primary highlight fills have no individual stroke. The outline is drawn around the union of rendered rectangles,
  removing internal borders between adjacent highlighted rows while preserving separate ranges and their verdict fills.
  Highlights still persist until another interaction and navigation preserves the actual execution selection.

## Verification

| Check | Result |
|---|---|
| Locked offline gate | 1,793 .NET tests, 10 Python tests, generated references and 533-rule inventory freshness PASS |
| Debug and Release builds | PASS; existing DialogChrome threading warnings only |
| Release WPF checks | 42 pane checks, including joined-outline and gap geometry; 15 confirmation checks PASS |
| Isolated SSMS 22.11, Light | 21 editor, 38 mixed-verdict, 23 procedure-body and 15 dynamic checks PASS; compact sample and measured minimum width PASS |
| Persistent highlights | Mixed object, procedure body and decoded operation PASS through passive reanalysis |
| Real editor outline | Adjacent rendered row boundary has no internal stroke PASS |
| Visual inspection | 34 captures; operation warning placement and source/execution details inspected |
| Settings and processes | Test settings restored byte-for-byte; owned test process closed; no physical input or SQL execution |
| Normal-profile installation | All 25 installed payload files match the frozen Release VSIX; settings unchanged |

VSIX SHA256: `0d8b58f28a41fde68ed3aaf66935d5ad76d468fa516d8b88a09e6f1a20b7392a`.

Evidence stays outside the repository in the local `harness-out/source-highlights-20261111` run directory. It contains
build/test logs, isolated receipts and captures, the frozen VSIX, installed payload hashes and the settings comparison.
The first offline attempt passed all .NET tests but found stale generated source references; regeneration and the full
offline rerun passed. No server-verdict campaign, physical F5-key coverage or additional theme matrix is claimed.
