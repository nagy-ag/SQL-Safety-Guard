> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Grouped objects within visible code scopes — 2026-11-17

Local unreleased UI change on top of `d7685268d966f39c602e5b6451a56b55a4566917` (published 1.1.1 documentation).
No commit, version bump, publication or normal-profile installation was performed for this change.

## Result

Visible procedure, function, trigger and view previews now group identified targets beneath Inside, followed by
their operations. Decoded EXEC scopes use the same projection; remote and stored Agent command previews retain
their separate scopes. Targetless operations and remote query expressions remain direct operation rows.
Object groups begin collapsed, appear in first-occurrence order and retain source-ordered statements.

Existing risk colors, preview counts, shared line gutter, filtering, single-click navigation and persistent
highlights, double-click expansion and pixel scrolling are preserved. Group expansion and selected highlights
survive passive reanalysis. Groups reuse the original findings and add no analysis results. Short names are
not merged with qualified names by assuming the editor's database in a body or remote scope.

The [inspection sample](../../../samples/06-grouped-code-scopes.sql) includes repeated permanent, temporary and
table-variable targets, unknown calls, separate dynamic executions, remote/job commands and module kinds.
Analyzer verdicts, F5 policy, resource limits and settings format are unchanged.

## Verification

| Check | Result |
|---|---|
| Locked offline gate | 1,913 .NET tests and 10 Python tests PASS; generated references and the 548-rule inventory fresh. |
| Build | Debug and Release rebuilds PASS; final capture-driven refinements built successfully in both configurations. Existing DialogChrome threading warnings remain. |
| WPF pane | 82 checks PASS in both Debug and Release, including grouped scope filtering, row identity/order, independent execution scopes and oversized grouped-object wheel scrolling. |
| Other Release WPF checks | 15 confirmation and 14 settings checks PASS, for 111 Release WPF checks total. |
| Focused native SSMS | 48 grouping checks PASS: compact defaults, actual row gestures, source navigation, counts/filtering, nested scopes, passive restoration and click-elsewhere clearing. |
| Existing native regression | Procedure body 24, decoded dynamic SQL 15, automatic pane 19, code bodies 175, nested code bodies 21, pane polish 15 and collapsed summary 43 PASS; compact layout, minimum width and highlight persistence also PASS. |
| Captures | 2 focused and 34 existing-surface Dark captures retained; final grouped-pane capture visually checked. |
| Cleanup | Both capture runs report settings restored byte-for-byte and owned SSMS exited; no shared desktop input, SQL execution or connection creation. |

Native checks used an owned isolated SSMS 22.11 profile on an undisplayed private desktop. Initial Debug installation
was verified from the native installer completion log despite its parent CLI timeout. Final Debug DLL/PDB updates
were copied only with owned SSMS closed and verified by hash before accepted native runs. This establishes final
Debug loading and behavior, not an exact final Release VSIX installation or upgrade.

Earlier offline attempts during concurrent builds/initial profile setup hit analysis time limits; the final
sequential locked gate passed without changing those limits. A cold-start capture timed out and closed its owned
SSMS cleanly; the warmed retry passed. Intermediate captures exposed a redundant dynamic wrapper for embedded
previews and a remote query expression presented as an object; both were corrected before final accepted checks.

Raw logs, hash receipts, restoration receipts and captures remain outside the repository under the evidence
label `grouped-code-scopes-20261117`; final native lanes are `accepted-scope` and `accepted-regression`.
Mixed-risk union checks use a synthetic UI fixture, not new server proof. No server campaign, full-theme matrix,
F5 routing campaign, normal-profile upgrade or release publication is claimed. Published 1.1.1 remains unchanged.
