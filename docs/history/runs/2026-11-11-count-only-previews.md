> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Count-only preview badges — 2026-11-11

Local unpublished presentation update. Preview badges on object and containing-operation rows show only coloured dots
and red/yellow/green counts. The visible Body/SQL labels are removed. Hover text identifies Body or Remote, replacing
the previous SQL term for remote context. A small gap separates submitted counts from preview counts.

Actual risk markers, preview count scope, purple definition context outlines, navigation, expansion and persistent
highlights are preserved. Analyzer verdicts, execution totals and guard behavior are unchanged. No SQL was executed.

## Verification

- Debug and Release builds PASS; two existing DialogChrome threading warnings remain.
- Locked offline gate: 1,872 .NET tests and 10 Python tests PASS; generated references and 540-rule inventory are fresh.
- Release WPF: 42 pane interaction checks PASS.
- Isolated SSMS 22.11, Light: 175 typed-body, 21 nested-body, 24 procedure and 15 dynamic checks PASS. Rendered headers
  retain counts without Body/Remote/SQL labels, while tooltips retain the correct context. Existing definition outlines,
  source navigation and persistent highlights pass their checks.
- Ten focused captures retained outside the repository. The compact procedure and remote-operation panes were visually
  inspected at the normal compact width. Owned SSMS exited cleanly; isolated settings restored byte-for-byte.
- Normal-profile VSIXInstaller installation and configuration refresh completed. All 25 installed payloads match the
  frozen Release package; normal settings are unchanged. SSMS is closed for the owner to reopen.

Frozen Release VSIX SHA256: `0092121C7E2998AA6248B1954F64D0D2644103F70072267EF6BC80A5EEA67A63`.
Logs, captures, event ledger, settings restoration, installed payload hashes and package remain outside the repository.
No commit, push or publication was made. This is one-theme presentation evidence using in-process events, not a new
server campaign, physical-input/F5 verification or full theme matrix.
