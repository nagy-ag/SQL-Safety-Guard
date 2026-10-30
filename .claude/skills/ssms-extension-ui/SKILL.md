---
name: ssms-extension-ui
description: Use before changing or testing the SQL Safety Guard SSMS 22 extension (src/SqlSafetyGuard.Vsix) - the editor pane, the confirmation dialog, the F5/Execute guard, installers or extension checks. Covers fail-closed guard rules, isolated test instances and non-intrusive UI testing.
---

# Working on the SSMS extension

## Guard rules

- The F5 guard analyzes the exact text SSMS is about to execute: the selection if there is one, otherwise the whole window.
- **Bind to the window SSMS actually executes.** SSMS routes `Query.Execute` through the focused control's command chain; the
  guard must analyze that editor, not whatever document looks active. If the target is ambiguous or unverified, show the
  yellow warning (fail closed), even in Off or Red-only mode.
- Green runs without interruption only when the target is verified. Yellow and red show the confirmation dialog by default.
- Confirmation is buttons only; never require typing. Analysis errors and limits show a warning, never a silent run.
- The safety UI is an editor pane beside the query, not a docked tool window.

## Testing without disturbing anyone

- Install and run test builds in an **isolated SSMS profile** (`/rootSuffix <name>`), never the user's normal profile.
  Identify your own SSMS process by command line, never by window title, and never close processes you did not start.
- Do not use the real mouse, keyboard, clipboard or foreground focus: the desktop may be shared. Prefer in-process test hooks
  (Debug-only command channels), UI Automation on your own process, and window messages to your own windows.
- Close SSMS cleanly (`WM_CLOSE`); killing it leaves a recovery prompt on the next start.
- After reinstalling a VSIX into the same profile, editor (MEF) parts may not load until the profile's configuration is
  refreshed (`/updateconfiguration`); uninstall, install, refresh, then launch.
- Verify UI changes in one theme. The full pass over every SSMS theme happens only when the owner says the extension is
  finished; restore theme settings afterwards.

## Before finishing

- Debug and Release builds succeed (Visual Studio MSBuild for the VSIX) and `dotnet test tests\SqlSafetyGuard.Tests` passes.
- Guard behaviour changes get a test in `GuardExecutionContractTests` / `GuardUiContractTests` or an extension check.

## Product documentation

See `docs/development.md` for build/debug and implementation details, `docs/user-guide.md` for installation, use and
settings examples, and `docs/verification/status.md` for current evidence. The root README is a short front page.

A guard failure before classification asks Run anyway or Cancel (default/Escape). Only known Off settings with a verified
target bypass the question. A broken failure dialog cancels. Debug `guardfailurecheck` exercises this handler without SQL.
