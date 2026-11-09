> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# In-extension uninstall command, 2026-11-03

The owner requested uninstalling from inside the extension. This is local, uncommitted work based on `5a35399`, marked
**1.1.0 Unreleased**. The published v1.0.0 release and its assets were not changed. No commit or push was performed.

## Change and boundaries

- Added **Tools > SQL Safety Guard > Uninstall SQL Safety Guard...**, separated from the regular query/settings commands.
- The confirmation defaults to No. A confirmed request starts the adjacent interactive SSMS VSIXInstaller with a fixed
  extension ID and the running installation/profile. It does not wait on the UI thread or report removal as completed.
- The loaded assembly must belong to the expected per-user profile. Mismatched profiles, ambiguous/unsafe identifiers,
  all-users manual-copy paths and unrecognized locations are refused with guidance. No force-close, elevation, file deletion,
  guard-mode change or settings mutation was added.
- Analyzer implementation, verdict policy and pending owner decisions are unchanged. Only analyzer version metadata changed.
  Production still opens no SQL connection, executes no SQL and queries no server metadata. Tests remain outside the VSIX.

## Verification

- Offline gate passed: 1,621 .NET tests, 10 Python tooling tests and 527 generated rules current. The new class contributes
  29 checks for instance/profile targeting, cancellation, launch failures, invalid inputs, manual-copy refusal and command wiring.
  The full .NET suite was rerun successfully after the final confirmation-text edit and rebuilds.
- Debug and Release solution rebuilds with locked restore succeeded. Release in-process interaction checks passed:
  13 pane, 15 confirmation and 4 settings checks. These do not exercise the new native SSMS message box.
- A read-only .NET Framework probe used the installed SSMS isolation INI and an existing Exp-profile assembly path as input
  to the newly built helper. It resolved the exact instance/profile and adjacent installer; declining started no process.
  The probe did not invoke the command in SSMS or modify the existing installation.
- Both rebuilt VSIX files passed CRC, identity/version, required runtime/legal files and packaged-versus-built DLL checks.
  Neither contains SQL client, test/harness tooling or a package signature. Final Release VSIX SHA-256:
  `7f1081034f88af5da97d3e731435b9f61b5a40e50aaeb73409f24a4994454e24`.
- An initial build caught an incorrect SDK enum reference; it was corrected. Independent package inspection then caught
  stale 1.0.0 manifest metadata from incremental VSSDK builds. Clean rebuilds produced verified 1.1.0 packages;
  development guidance now explains this. The existing release script already rebuilds and validates package versions.

## Limits before publication

No live SSMS menu/dialog/installer-completion check, installation change, server campaign, SQL execution, desktop input,
theme capture, signing or publication occurred. The new native dialog's host rendering and actual removal after closing
SSMS still need an isolated-profile acceptance check before publishing 1.1.0. Existing interaction tests do not prove those
behaviors. Administrator-managed all-users removal continues to use IT/the elevated uninstall script.
