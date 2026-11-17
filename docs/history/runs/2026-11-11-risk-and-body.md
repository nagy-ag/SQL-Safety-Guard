> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Risk markers and body context — 2026-11-11

Local unpublished presentation update to the typed-body candidate. No commit, push or release was made.
Analyzer verdicts, submitted-operation totals and F5 policy are unchanged. No SQL was executed or database connection opened.

## Result

- Object and operation markers keep their actual red/yellow/green verdict. Creating or altering a permanent module
  remains red; purple no longer replaces that risk marker.
- A purple Body label on the right introduces separate red/yellow/green preview counts for module definitions and
  stored job commands. Remote previews use a neutral SQL label. Preview badges count nested descendants exactly once;
  ordinary object counts and main filters remain submission-only. No extra preview-heading row is added.
- Containing operations use their actual SQL verbs, such as CREATE/ALTER PROCEDURE and EXEC, instead of Definition/Call.
- Selecting a definition outlines its complete parsed span in purple, including AS and BEGIN/END where present,
  excluding GO and separate calls. The header retains its actual risk fill. Selecting a child retains the enclosing
  module outline and fills only that child's operation. Embedded remote/job previews retain their enclosing module context.
- Existing single-click navigation, double-click expansion, persistent selection, joined risk outlines, source links,
  minimum filter width and warning placement remain intact. Actual execution selections are preserved.

## Verification and installation

- Debug and Release solution builds PASS. The two existing DialogChrome threading warnings remain.
- Locked offline suite: 1,872 .NET tests and 10 Python tests PASS; generated references and the 540-rule inventory are fresh.
- Release WPF checks: 42 pane interaction and 15 confirmation interaction checks PASS.
- Owned isolated SSMS 22.11 profile, Light: 175 typed-body, 21 nested-body, 38 mixed-verdict, 24 procedure and 15 dynamic
  checks PASS. Coverage includes all 13 examples in `05-other-code-bodies.sql`, nested bodies, separate count scopes,
  actual risk markers, labelled preview badges, full module contexts, source navigation, expansion and persistent highlights.
- Compact sample and minimum-width receipts PASS. Context adornments survive passive reanalysis and clear with the
  selected operation when clicking elsewhere. A top-level compact-sample selection outlines lines 12–18, leaving GO
  and the separate line-20 EXEC outside the purple rectangle.
- 33 acceptance captures and five focused definition-context captures retained outside the repository. Compact pane,
  remote/nested badges and the full definition outline were visually inspected. Both owned runs exited cleanly and
  restored isolated settings byte-for-byte.
- Release package installed through the normal-profile VSIXInstaller and configuration refresh. All 25 installed
  payload files match the frozen package; normal SSMS settings are unchanged. SSMS is closed for the owner to reopen.

Frozen Release VSIX SHA256: `49C6B2DAECA54842DE5F496E787911CE2467D8FCDB54D13A9EC464ED4D045FF6`.
Build/test logs, event ledgers, captures, restoration receipts, installed payload hashes and frozen package remain outside the repository.

## Limits

This verifies presentation in one theme with in-process events on a private desktop. It is not a new server campaign,
physical-input/F5 check or full theme matrix. Calls do not resolve deployed implementations; remote runtimes, runtime
text and transitive dependencies retain their existing conservative treatment.
