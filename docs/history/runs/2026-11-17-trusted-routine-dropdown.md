> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Trusted-routine dropdown dark-theme fix

On 2026-11-17 the owner paused the requested 1.1.1 release after noticing a white Procedure/Function dropdown in
Trusted routines. Publication remains on hold: the local source versions are prepared as 1.1.1, but nothing is
committed, tagged or published. GitHub's published release remains 1.1.0.

The RoutineType selector now resolves the same supported SSMS themed ComboBox style already used by the F5 warning
selector. This covers both its closed surface and popup. No routine matching, authorization or persisted settings format changes.

## Verification

- Clean Debug and Release rebuilds PASS. Both VSIX archives have manifest 1.1.1 and pass ZIP CRC checks.
  Existing DialogChrome threading warnings remain. The initial incremental outputs had stale generated manifest
  versions; the installation version gate rejected that attempt before any live acceptance. Only the clean rebuilt
  package is accepted as evidence below. No release package was published from either attempt.
- All 1,913 .NET tests and all 14 Release settings checks PASS.
- The rebuilt Debug package installs in a fresh owned isolated SSMS 22.11 profile; all 24 installed payload files match.
- The focused `ThemeAcceptanceCapture.ps1 -CheckTrustedRoutineStyles -Themes Dark` lane verifies that native Dark
  reaches the shell resources, the real selector resolves the supported style, switching Procedure/Function updates
  the form, and the dropdown popup opens. Two nonblank captures of the closed selector and popup are visually
  inspected: both have dark backgrounds and readable light text.
- The owned session closes cleanly and its original settings are restored byte for byte. No shared mouse, keyboard,
  clipboard, foreground focus, SQL connection or execution is used. The normal-profile runtime still matches all
  24 files from the published GitHub 1.1.0 package.

The changelog, README, upgrade guide and current status explicitly retain the publication hold. The testing guide
lists the focused capture switch. Raw logs, rejected incremental-output evidence, installed-file comparisons, images,
trace receipts and restoration hashes remain outside the repository. This focused Dark check is not full-theme,
F5-routing, SQL/server or release-publication evidence.
