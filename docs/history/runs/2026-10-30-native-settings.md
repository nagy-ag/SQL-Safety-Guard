> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Native settings verification - 2026-10-30

Scope: integrated SSMS settings, command organization, compact toolbar status, and multiline settings editors.
The analyzer and execution guard logic were not changed. No SQL was executed.

| Check | Result |
|---|---|
| Debug / Release solution builds | Pass |
| Full automated suite | 1,560 passed; 0 failed; 0 skipped |
| Pane / confirmation / settings editor interaction checks | 13 / 15 / 4 passed |
| Native registration manifest against installed SSMS schema | Pass |
| Integrated Options page in isolated SSMS 22.11 | Six settings render directly; Import/Export and list/rule editor actions visible; no legacy Options popup |
| Native batch updates, before and after opening Options | All six active values match the native store |
| Import and export | All six settings round-trip; native page and running options agree |
| Import cancellation | All six settings unchanged; Cancel remains the default |
| Import that broadens trust | Six preview warnings visible before confirmation |
| Multiline editor commands | Allow-list Save and reopen pass; invalid trusted procedure rejected; trusted/rule Cancel pass |
| Restart persistence | All six persisted values match active settings after restart |

Live verification used the Debug-only in-process test channel in a private directory and an isolated SSMS profile.
Rendered WPF output was inspected in one theme. No mouse, keyboard, clipboard, or foreground activation was used.
The desktop screenshot helper was unavailable, so this is not a physical-input or every-theme acceptance pass.
Raw events, exported test settings, activity logs, and rendered images remain outside the repository.

The live checks found and fixed two bridge issues: legacy auto-save could overwrite later values in a native batch,
and the legacy per-property persistence cache could miss an import when the native Options context first activated.
Incoming settings notifications no longer auto-save; explicit import/editor saves commit the complete native batch.
Legacy option-page Apply continues to persist explicitly, and the original legacy store mappings/defaults are retained.

The previous server campaign and open policy questions remain in force. Company-laptop installation and a published
GitHub release are outside this UI verification.
