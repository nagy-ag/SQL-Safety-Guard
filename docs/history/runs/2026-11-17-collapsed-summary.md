> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Scope header and collapsed six-color summary

Local, unpublished UI update on 2026-11-17. Published release 1.0.5 is unchanged.

The expanded header now shows the analysis scope without verdict badges. Existing environment indicators and
Analysis/Settings/collapse controls are preserved. The collapsed strip shows a numbered circle for each present
category, ordered Writes, Unknown, Safe, Inside, Context, Remote. Green is a numbered circle; zero-count categories
are omitted. The strip uses exactly the six display controls' unfiltered operation totals and their semantic hover
text. Filters do not lower those totals. Pending and placeholder transitions clear stale badges and show a neutral
marker. All passive circles retain one strip-level expand action, and large numbers shrink inside their circles.

The former header used object-group totals; the replacement shares operation totals with the controls. Purple
Inside and gray Remote still count preview operations separately from submitted red/yellow/green/context operations.
Analyzer classifications, the F5 guard, approved-object policy and SQL source mapping are unchanged.

## Verification

| Check | Result |
|---|---|
| Locked offline gate | 1,913 .NET and 10 Python tests PASS; 548-rule inventory and generated references fresh |
| Builds | Debug and Release PASS; existing DialogChrome threading warnings remain |
| Release WPF | 64 pane checks PASS: present-color omission, safe-only numbered circle, five-digit counts, filter independence, pending clearing and the strip expand action |
| Isolated SSMS 22.11, verified Dark | 28 nonblank captures; 43 collapsed-summary, 19 automatic-pane, 175 typed-body, 21 nested-body and 15 context/gutter/hover checks PASS |
| Visual inspection | Scope-only expanded header and all six numbered circles in the real collapsed strip inspected |
| Restoration | Isolated settings restored byte-for-byte; owned SSMS exited cleanly |
| Local installation | All 25 payload files match the frozen Release VSIX; normal-profile settings unchanged; extension cache refreshed |

Six additional text-only examples were inspected against the built analyzer: procedure calls to another procedure
and a function, the same calls in dynamic SQL, dynamic SQL inside a procedure, three versus four dynamic expansions,
and a remote query inside a procedure. Calls stay unproved unless separately approved; visible definitions do not
establish the implementation a call will execute. Three local dynamic expansions expose the final DELETE; a fourth
produces Unknown with an explicit depth warning. Existing body-preview nesting is bounded at eight inspected levels,
with an incomplete/Unknown boundary, and shared resource budgets can stop earlier. These observations inform a
future nesting discussion; no call graph or new nesting behavior was implemented.

No SQL was executed. No new server campaign, physical input simulation, all-theme matrix, commit, push or publication
was performed. Raw receipts, screenshots, illustrative analyzer output and frozen packages remain outside the repository.

Installed package SHA-256: `CF0F1C06EC9C5AF5ADF80CD08E2A6BEBE3E92A6DB6CFF7D7C8298C36CBFA40B6`.
