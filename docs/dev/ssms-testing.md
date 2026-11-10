# Testing in SSMS

Two kinds of checks run against a real SSMS 22:

- [Visual and settings checks](#visual-and-settings-checks-no-sql) capture the pane, dialogs, Options page and editors,
  and drive the settings controls. They never execute SQL.
- [Execution routing checks](#execution-routing-checks-sql-on-a-disposable-database) prove which window SSMS executes and
  which commands the guard intercepts. They execute SQL against an owned disposable database.

Both run in an isolated SSMS profile that you own, never in anyone's normal profile, and use Debug-only in-process hooks
instead of the real mouse, keyboard, clipboard or focus. Read the local machine rules (`AGENTS.local.md`) and the
[extension UI skill](../../.agents/skills/ssms-extension-ui/SKILL.md) before running either.

## Visual and settings checks (no SQL)

### Setup and run

Install the **Debug** VSIX in **Exp**, with SSMS closed: uninstall the previous extension, install the VSIX, then
run `SSMS.exe /rootSuffix Exp /updateconfiguration` hidden. Leave other profiles and other agents' processes alone.
Run Windows PowerShell 5.1 from the repository root (use an absolute output directory outside the repository):

```powershell
.\scripts\ThemeAcceptanceCapture.ps1 -OutputDirectory C:\capture-out\trial -Themes Light,Dark
# Add -CheckProcedureBody for definition/body separation and statement navigation checks with no SQL.
# Add -CheckPaneHighlights for the click-to-highlight checks and both safe-section arrow states.
# Add -CheckCompactSettings for focus routing, native persistence and page scrolling of the compact controls.
# Omit -Themes for all 13, only when the owner authorizes the complete theme pass.
```

`-SettingsPath` defaults to the discovered SSMS 22 instance's isolated Exp settings.json. The script refuses a running
Exp session and launches only Exp minimized on a private Windows desktop that is never displayed or switched to.
This isolates SSMS's own startup activation from the shared desktop. The channel also vetoes activation on its UI thread.
Use `-RootSuffix <owned-name>` for a separate isolated profile when Exp already belongs to another run; its settings
file must already exist. The suffix is validated and the script checks only that exact profile and installed SSMS instance.
It restores its own window without activation on that private desktop for PrintWindow.
It uses process-scoped test/fault-injection variables, never dispatches Execute, and closes its own SSMS with WM_CLOSE
(BM_CLICK only for a shutdown prompt). It backs up settings bytes/hash and restores them in `finally`, including on failure.
Inspect `restoration.json` for identical hashes **and** `SsmsExited: true`; a process that failed to close could later rewrite
settings, so close that run cleanly and restore `settings.original.json` again before continuing.

### Channel verbs and receipts

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

### Pane highlight checks

`-CheckPaneHighlights` also runs 21 in-process checks on the real editor buffer and adornment layer, followed by a
fresh-analysis check that cleared highlights stay cleared. It covers initial red/yellow statements without tint,
choosing and switching groups, editor mouse/key/caret interaction, pane activation with empty and nonempty SQL
selections, deselection, collapse/expand and a real buffer edit. These checks never dispatch Execute or open a SQL
connection. The normal Release pane interaction script separately checks the rendered arrow geometry, section
content visibility and toggle actions. These are UI checks, not a new execution/server campaign.

`-CheckProcedureBody` runs 25 checks against the sample procedure: deferred counts, shared single-line rows,
whole-target warnings, group and individual highlights, exact execution-selection preservation, nested keyboard
routing and expansion state after reanalysis. Both suites raise control events inside the owned test process;
they do not synthesize physical clicks or key presses. Single clicks highlight object groups, double clicks toggle
details, and the individual statement buttons replace the group highlight with one statement at either level.

### Settings checks

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

### Theme values

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

## Execution routing checks (SQL on a disposable database)

The maintained execution-check driver is documented in the [ExtensionChecks README](../../tests/SqlSafetyGuard.ExtensionChecks/README.md).
It launches an owned isolated SSMS profile, uses Debug-only hooks, and executes SQL against an owned disposable database.
It is separate from offline UI interaction tests and visual captures. Read local machine rules before running it.

### Evidence and limits

The latest retained execution-target campaign is the [October 28 snapshot](../history/2026-10-28-extension/README.md):
35 PASS, zero FAIL, one toolbar BLOCKED across 36 scenarios. Those counts describe that historical Debug build, not the
current working tree. Original ledgers and events are preserved with checksums. No new live execution evidence is implied
by later builds, unit tests or documentation changes.

Physical keyboard shortcuts are not synthesized. Their command bindings and common Query.Execute dispatch are inspected.
Toolbar UIA availability, successful results-file Save, custom stored-procedure shortcuts and future SSMS host versions
remain separate coverage limits. The guard reads the focused editor's execution candidate and cross-checks host identities;
an ambiguous target requires a warning even in Off or Red-only mode.

### Result contract

The driver retains all scenario diagnostics and writes a machine-readable summary. FAIL makes the run fail. BLOCKED is
incomplete unless the caller names the exact expected blocked scenarios; that exception remains visible in the summary
and is never counted as PASS. Missing expected scenarios, an empty result set and duplicate scenario names cannot pass.
Setup/runtime failures abort, and the owning wrapper still performs cleanup in finally.

The result-aggregation helper is exercised offline without loading the UI client or opening a SQL connection. Its tests
establish result handling only. Real command routing, captured-text identity, submission count and side effects require
the separate integration observations described in the driver README.

For current qualified evidence, use [status.md](../status.md); for component responsibilities, use
[architecture.md](architecture.md).
