> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Purple code containers and inline counts — 2026-11-11

Local unpublished presentation changes on top of the existing typed-body candidate. No commit, push or release was made.
Analyzer verdicts, execution counts and F5 policy are unchanged. No SQL was executed or database connection opened.

## Result

- Procedure, function, trigger and view definitions, and stored SQL Agent commands, have a purple object marker.
  Purple identifies deferred code; each operation and editor highlight keeps its actual red/yellow/green verdict.
  Current dynamic SQL and remote execution retain their existing risk markers.
- Object badges on the right include submitted findings and every preview descendant exactly once. Main header and
  filter counts still use submitted findings only. Counts remain complete when display filters hide operations.
- Preview badges appear on the containing Definition/operation row, including nested embedded operations. The shared
  template supports any row with an attached body. No additional context-heading row is rendered; execution context
  remains in tooltips, and incomplete-body diagnostics remain explicit. Separate unknown calls do not acquire a body
  merely because a definition appears in the editor.
- Existing row spacing, arrows, single-click navigation, double-click details, source links, persistent highlights,
  continuous editor outlines, minimum filter width and warning placement are preserved.

## Verification

- Debug and Release solution builds PASS. The two existing DialogChrome threading warnings remain.
- Locked offline suite: 1,872 .NET tests and 10 Python tests PASS; generated references and the 540-rule inventory are fresh.
- Release WPF checks: 42 pane interaction and 15 confirmation interaction checks PASS.
- Owned isolated SSMS 22.11 profile, Light: 135 typed-body and 17 nested-body checks PASS. Checks include the container
  versus operation markers, complete descendant badges, rendered counts on the containing row, no preview-heading row,
  navigation, independent expansion and restoration. Existing 38 mixed-verdict, 23 procedure and 15 dynamic checks PASS;
  compact sample, minimum width and persistent-highlight receipts also PASS.
- 33 real SSMS captures retained outside the repository. The compact sample was visually inspected: the SP shows
  purple with red 2/yellow 2/green 1; Definition shows red 1/yellow 1/green 1 on the same line, followed directly by
  UPDATE, EXEC and SELECT rows. The separate Call remains yellow.
- Owned test SSMS exited cleanly; isolated settings were restored byte-for-byte.

Frozen Release VSIX SHA256: `409F156C4AFD50E9036B09C557F64C93840C1C718D484ABA0D12C673546A87BA`.
Build logs, offline logs, event ledger, captures, restoration receipt and frozen package are retained outside the repository.

## Limits

This is a presentation check in one theme, using in-process events on a private desktop. It is not a new server campaign,
physical-input/F5 verification or full theme matrix. Deployed bodies, remote runtimes and transitive dependencies remain
unproved; existing analyzer limits and conservative warnings still apply.
