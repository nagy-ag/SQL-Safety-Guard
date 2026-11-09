# Theme capture without executing SQL

Install the **Debug** VSIX in **Exp**, with SSMS closed: uninstall the previous extension, install the VSIX, then
run `SSMS.exe /rootSuffix Exp /updateconfiguration` hidden. Leave other profiles and other agents' processes alone.
Run Windows PowerShell 5.1 from the repository root (use an absolute output directory outside the repository):

```powershell
.\scripts\ThemeAcceptanceCapture.ps1 -OutputDirectory C:\capture-out\trial -Themes Light,Dark
# Add -CheckPaneHighlights for the click-to-highlight checks and both safe-section arrow states.
# Add -CheckCompactSettings for focus routing, native persistence and page scrolling of the compact controls.
# Omit -Themes for all 13, only when the owner authorizes the complete theme pass.
```

`-SettingsPath` defaults to the discovered SSMS 22 instance's isolated Exp settings.json. The script refuses a running
Exp session and launches only Exp minimized on a private Windows desktop that is never displayed or switched to.
This isolates SSMS's own startup activation from the shared desktop. The channel also vetoes activation on its UI thread.
It restores its own window without activation on that private desktop for PrintWindow.
It uses process-scoped test/fault-injection variables, never dispatches Execute, and closes its own SSMS with WM_CLOSE
(BM_CLICK only for a shutdown prompt). It backs up settings bytes/hash and restores them in `finally`, including on failure.
Inspect `restoration.json` for identical hashes **and** `SsmsExited: true`; a process that failed to close could later rewrite
settings, so close that run cleanly and restore `settings.original.json` again before continuing.

The Debug file channel accepts `document.sql|verb|argument` in its private test directory. `showconfirm`, `showfailure`,
`showlisteditor allowList|trustedProcedures|environmentRules` (the structured list and rule editors), and `showimportpreview <absolute .ssgsettings.json path>` show
the real dialogs modeless with ShowActivated=false, owned by SSMS. Run, Save and Import handlers are inert in previews; buttons retain their production enabled state.
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

`-CheckPaneHighlights` also runs 17 in-process checks on the real editor buffer and adornment layer, followed by a
fresh-analysis check that cleared highlights stay cleared. It covers initial red/yellow statements without tint,
choosing and switching groups, editor mouse/key/caret interaction, pane activation with empty and nonempty SQL
selections, deselection, collapse/expand and a real buffer edit. These checks never dispatch Execute or open a SQL
connection. The normal Release pane interaction script separately checks the rendered arrow geometry, section
content visibility and toggle actions. These are UI checks, not a new execution/server campaign.

`-CheckCompactSettings` routes focus events through the six real controls, verifies that the native content host
does not block keyboard entry, and reads all six edited values back from the native settings manager. It restores
the original option values in a finally block. The outer capture run closes its owned process and restores the
original settings file byte-for-byte. A second step shrinks the page viewport so it overflows, raises in-process wheel
input over each control, waits for SSMS's sticky category header, and requires that no copy of the form covers the page
and that hit-tests at three heights reach the scrolling content (`compact-settings-scroll 11 PASS`). It then opens
each list editor through its real command and fills its form as a user would (`settings-lists 4 PASS`), and captures the
allow-list and rule editors with sample entries. Dialog previews also record a `chrome` receipt with the title-bar
colours applied; the native title bar itself is not part of the WPF captures. SSMS supplies the Preview caption around the custom GEL content area; the
compact settings themselves are live, with immediate persistence. No SQL or Execute command is used.

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
