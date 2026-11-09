> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Local published 1.0.3 upgrade baseline — 2026-11-06

**The 22.9.2 baseline is accepted; the same-installation upgrade remains INCOMPLETE.**
The owner authorized a local downgrade, installation of unchanged published SQL Safety Guard 1.0.3,
settings/execution checks, then an in-place update to 22.11.3 with the same checks. The downgrade
completed successfully, without a reboot. The later upgrade elevation request ended with Windows
reporting “The operation was canceled by the user.” This can also reflect an expired consent prompt;
it is not evidence that the owner cancelled the task. No elevated upgrade wrapper or installer started.
Installed SSMS remains **22.9.2 / 22.9.12120.119**. No after-upgrade result is available.

## Accepted baseline

| Check | Actual evidence and result |
|---|---|
| Installed identity | Exact 22.9.2 build read back at the original installation path; downgrade created a new installation ID, which was used for the isolated test profile |
| Published package | Frozen VSIX SHA-256 `0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC`; all 27 installed payload members matched the ZIP and initial capture by path/hash |
| Release assembly | SHA-256 `97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2` |
| Settings | All six seeded custom values matched persisted settings and both complete native Preview forms, each with six unique typed controls |
| Green read | Actual results grid showed the owned database/server and counter 0; SQL Server's owned-session input buffer recorded the exact SELECT |
| Red Cancel | Owned confirmation identified the expected server/database and whole-window execution; Cancel left counter/events at 0 and the server input buffer still held the earlier SELECT |
| Red Run | A separate Execute and one Run anyway action produced exactly one audit event, counter 0 to 1, exact UPDATE text and the expected SSMS session/application/database |
| Closure | Owned SSMS closed cleanly; only its captured, unsaved test buffer was discarded. Package and persisted settings were captured after closure |

The native review read `SelectionPattern.Current.GetSelection()` for the warning mode and checked
every rendered form. It retained control names/types, unique RuntimeIds, values, process identity,
and raw ancestry through the owned Options page. Both forms agreed on warning mode, connection
display, table allow-list, trusted procedures, environment rules and the permanent-tempdb opt-in.
No native setting was changed during observation. Settings were seeded once before the baseline.

An independent local acceptance script re-read the native/execution review, all frozen ZIP and
installed package member hashes, and the six persisted typed values before writing the accepted
baseline receipt. The four named SSMS root cache/metadata files were recorded separately from
the extension payload. A before/after cache delta requires the still-pending update.

## Input and automation qualifications

The green read was submitted with the human-operated toolbar Execute button. The remaining
Execute, Cancel and Run actions used UI Automation directed at the exact owned SSMS process,
with query text and database checked before execution. No agent global keyboard, mouse,
clipboard or foreground-focus calls were used. **Physical F5 interception was not tested.**
These are three focused local cases, not acceptance of the eleven-case VM suite. SQLCMD-on,
deliberate ambiguity, multiple windows and post-update behavior are not covered here.

The first prepared read query included `USE`, correctly causing a target-switch warning. The
original input was retained; the actual accepted SELECT omitted `USE`. External automation
selector/type and observation errors occurred before their attempted actions and were corrected
without changing the product. Action receipts were written before submission to prevent duplicate
execution. An unrelated Copilot telemetry activation error remains in the SSMS activity log;
the tested guard and SQL execution still completed. A loaded managed-module path was not
enumerated, so installed payload integrity is not reported as separate module-path proof.

## Retained state and continuation

The original 1,345-file/profile/registry/workload backups remain available. The original normal
profiles were retained under the former installation ID; they were not used for testing. Visual
Studio Community's registered version remained unchanged. No cloud resources were used.
The owned test database, counter 1, isolated profile and evidence remain for the same-installation
continuation. They were not removed as though the campaign had finished.

The single monitor is disabled with no next run while fresh Windows administrator consent is
needed. The failed consent receipt and original clocks are preserved. Do not rerun the baseline,
reinstall the extension, refresh configuration or seed settings again. After a successful update,
verify the same installation ID/path and exact 22.11.3 build, repeat the focused checks, require all
27 package files unchanged, and report SSMS cache changes separately. No product edits, build,
repackage, commit, push or publication were made.
