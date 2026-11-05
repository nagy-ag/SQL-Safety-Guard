> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Uninstall UI correction — November 5

The owner could not find an uninstall action in SSMS and requested removal from the SSMS UI. The normal profile's
installed Release DLL matched the November 4 published binary, SHA-256
`d0239723debc6cdb419c57c26c12a9cfaf1e27db536f1cc2a873fa5781752451`. This was not an older installed binary.
The supplied screenshots also showed that this SSMS build has no Extensions > Manage Extensions entry.

## Change and identity

Based on source `83a17fa`, the local uncommitted correction adds **Uninstall SQL Safety Guard...** beside Import and
Export in the native SQL Safety Guard settings category. It invokes the existing uninstall command, with its No-default
confirmation, installation/profile validation and local installer handoff. No analyzer, execution-guard, SQL connection,
server metadata, settings persistence, confirmation policy or installer-launch implementation changed.

The Tools submenu had been parented directly to the Tools menu. The correction inserts a command group between them,
as required by the shell menu model. A regression now rejects the original missing-group hierarchy. The native settings
action inventory regression includes the uninstall command. User instructions and an installer-script comment no longer
promise an Extensions > Manage Extensions entry on every SSMS build.

The package retains 1.0.0, unsigned, supporting `[22.0,23.0)`. Release VSIX SHA-256:
`6377bd1f67bdf915bf676e1d95bd060ba1f427ff68703f31f011555cb57c4085` (3,516,523 bytes, 28 ZIP members).
This is a local candidate built from the worktree, not a clean-source release-script acceptance or a GitHub publication.
The published tag, release, assets, normal installation and normal running SSMS process were not changed.

## Checks

- Locked-restore Debug and Release solution rebuilds: **PASS**.
- Full .NET suite: **1,628 PASS**, no failures or skips.
- Release interaction scripts: **4 settings, 13 pane, 15 confirmation PASS**. These exclude native host rendering.
- Package CRC, member safety/uniqueness, version/SSMS target, native settings command content and packaged/built DLL
  agreement: **PASS**. No checked test-channel/harness/SQL-client file was packaged.
- Live acceptance used Release on local SSMS **22.11.2 / 22.11.12324.101**, in two newly owned isolated profiles on an
  undisplayed private Windows desktop. No real mouse, keyboard, clipboard or shared foreground input was used.
- The settings page exposed a **visible, enabled Uninstall SQL Safety Guard... button** alongside Import and Export.
  Invoking that button through its actual UI Automation provider displayed the production confirmation naming the
  expected installation and test profile.
- Choosing **No** left the extension files present and started no installer. Choosing **Yes** on a second invocation
  started the adjacent VSIXInstaller with the expected instance ID, root suffix and fixed extension ID.
- After that handoff, the isolated extension DLL/manifest files disappeared. The installer log recorded removal from
  its registration caches. A separate profile-specific installer lookup then reported **NotInstalledException**, exit
  **1002**. That exit is not a successful fresh uninstall; the explicit diagnostic corroborates that the earlier UI
  operation removed registration. The existing scripts' handling of installer not-installed exit codes was not changed.
- All **22** pre-existing protected settings/extension DLL hashes matched after the first run, and all **23** protected
  hashes matched after the second (including the first run's now-empty isolated profile settings). Both owned SSMS
  processes closed cleanly; the user's normal process remained running. No SQL was executed or server resource created.

## Retained limitations and cleanup

Private-desktop UI Automation did not expose the Tools popup descendants in the first run. Source hierarchy/build
checks do not establish live visibility of that submenu. The primary verified entry point is the native settings button.

The second driver assumed the installer would expose a separate Uninstall button. It timed out waiting on the launcher's
PID, while VSIXInstaller's delegated process had already begun removal. Its report retains **Removal INCOMPLETE**; it
was not relabeled as a fully completed installer-UI pass. Supplemental file checks and an independent not-installed
lookup establish the resulting removal, but the completion page and a normal interactive installer exit remain unverified.
The hidden, owned installer chain was stopped only after the extension files were confirmed absent. No user SSMS process
was stopped, no user extension directory was manually deleted, and no other profile was uninstalled.

Raw activity/installer logs, UI Automation snapshots, ownership/protected-hash receipts, driver failures, package validation
and the local delivery files are retained outside the repository. No signing, commit, push or publication was performed.
