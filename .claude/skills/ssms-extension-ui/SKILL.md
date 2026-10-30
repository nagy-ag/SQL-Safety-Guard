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

## Theme capture without executing SQL

Install the **Debug** VSIX in **Exp**, with SSMS closed: uninstall the previous extension, install the VSIX, then
run `SSMS.exe /rootSuffix Exp /updateconfiguration` hidden. Leave other profiles and other agents' processes alone.
Run Windows PowerShell 5.1 from the repository root (use an absolute output directory outside the repository):

```powershell
.\scripts\ThemeAcceptanceCapture.ps1 -OutputDirectory C:\capture-out\trial -Themes Light,Dark
# Omit -Themes for all 13, only when the owner authorizes the complete theme pass.
```

`-SettingsPath` defaults to `%LocalAppData%\Microsoft\SSMS\22.0_e8efe2d3Exp\settings.json`. The script refuses a running
Exp session and launches only Exp minimized on a private Windows desktop that is never displayed or switched to.
This isolates SSMS's own startup activation from the shared desktop. The channel also vetoes activation on its UI thread.
It restores its own window without activation on that private desktop for PrintWindow.
It uses process-scoped test/fault-injection variables, never dispatches Execute, and closes its own SSMS with WM_CLOSE
(BM_CLICK only for a shutdown prompt). It backs up settings bytes/hash and restores them in `finally`, including on failure.
Inspect `restoration.json` for identical hashes **and** `SsmsExited: true`; a process that failed to close could later rewrite
settings, so close that run cleanly and restore `settings.original.json` again before continuing.

The Debug file channel accepts `document.sql|verb|argument` in its private test directory. `showconfirm`, `showfailure`,
`showlisteditor allowList|trustedProcedures|environmentRules`, and `showimportpreview <absolute .ssgsettings.json path>` show
the real dialogs modeless with ShowActivated=false, owned by SSMS. Run buttons are inert; Save/Import are disabled.
`showoptions` opens the real integrated options page. `capture <surface> <absolute PNG path>` waits through dispatcher
ContextIdle/ApplicationIdle and uses RenderTargetBitmap at actual layout size/DPI for `pane`, `confirm`, `failure`,
`allowList`, `trustedProcedures`, `environmentRules`, `importpreview` (dialogs use their client root, excluding native chrome).
`main` uses PrintWindow(PW_RENDERFULLCONTENT), supplemented with RenderTargetBitmap of the live WPF HWND roots at their
actual positions: an undisplayed desktop has no DWM composition, and PrintWindow alone omits WPF content there.
Options readiness requires the real visible "F5 warnings" control; an idle queue alone cannot prove asynchronous navigation loaded.
`closedialogs` closes only channel previews. `preparecaptureclose` marks this run's scratch documents clean before WM_CLOSE.
Wait for `shown`/`captured` receipts in `events.tsv`, not just `ack` (which means the request was queued).
`manifest.csv` records theme, surface, PNG path, bytes, dimensions and pixel variance/non-blank status. Open several PNGs
to verify content as well: pixel variance alone cannot prove the correct surface was captured. All capture code/verbs are
compiled out of Release; this is a visual check, not evidence of F5 execution behavior.

The stored `environment.visualExperience.colorTheme` mapping is:

| Display name | Stored value |
|---|---|
| Light | `light` |
| Cool Breeze | `cool-breeze` |
| Icy Mint | `icy-mint` |
| Sunny Day | `sunny-day` |
| Juicy Plum | `juicy-plum` |
| Mango Paradise | `mango-paradise` |
| Bubblegum | `bubblegum` |
| Silky Pink | `silky-pink` |
| Dark | `dark` |
| Cool Slate | `cool-slate` |
| Moonlight Glow | `moonlight-glow` |
| Mystical Forest | `mystical-forest` |
| Spicy Red | `spicy-red` |

Checked against SSMS 22's `CommonExtensions/Platform/Shell/Shell.registration.json` (light/dark),
`CommonExtensions/Platform/Themes.Registration.pkgdef` (display names/GUIDs), and the shipped
`Microsoft.VisualStudio.Shell.UI.Internal.dll`: `EnvironmentOptions.GeneralOptionsPage.KnownThemeInvariantNameMap`,
`ColorThemeProvider.ComputeMoniker` and `ColorThemeDefinitionResolver.GetThemeInfo`. The tinted values are invariant
hyphenated names, not their theme GUIDs. Windows contrast themes are outside this 13-theme pass.

## Product documentation

See `docs/development.md` for build/debug and implementation details, `docs/user-guide.md` for installation, use and
settings examples, and `docs/verification/status.md` for current evidence. The root README is a short front page.

A guard failure before classification asks Run anyway or Cancel (default/Escape). Only known Off settings with a verified
target bypass the question. A broken failure dialog cancels. Debug `guardfailurecheck` exercises this handler without SQL.
