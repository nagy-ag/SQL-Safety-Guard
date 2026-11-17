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

`scripts/PaneInteractionTests.ps1 -Configuration Release` also runs without SSMS. Its oversized-object regression
routes mouse-wheel input through an operation in an 80-operation object, checks movement smaller than a page,
preserved expansion/selection, access to the final operation and return to the start. The shared list uses
`VirtualizingPanel.ScrollUnit="Pixel"` so expanded objects remain scrollable without disabling virtualization.

### Setup and run

Install the **Debug** VSIX in **Exp**, with SSMS closed: uninstall the previous extension, install the VSIX, then
run `SSMS.exe /rootSuffix Exp /updateconfiguration` hidden. Leave other profiles and other agents' processes alone.
Run Windows PowerShell 5.1 from the repository root (use an absolute output directory outside the repository):

```powershell
.\scripts\ThemeAcceptanceCapture.ps1 -OutputDirectory C:\capture-out\trial -Themes Light,Dark
# Add -CheckProcedureBody for definition/body separation and statement navigation checks with no SQL.
# Add -CheckPaneHighlights for click-to-highlight and display-filter captures.
# Add -CheckBalancedPane for mixed-verdict filtering, compact operations and persistent highlights.
# Add -CheckCompactSettings for focus routing, native persistence and page scrolling of the compact controls.
# Use -CheckTrustedRoutineStyles for a focused closed/open trusted-routine dropdown check and captures.
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

`-CheckProcedureBody` checks the sample procedure's flat inline operations, deferred counts, three verdicts, repeated
writes, whole-target warnings in expanded reasons, absence of another body expander/object tree/filter bar, and one rendered row per
operation. Definition risk fill covers only its header while a separate purple outline covers the complete parsed
definition, excluding GO and the separate EXEC. Body clicks retain that context and navigate/highlight without changing execution
selection; double-clicks open independent reasons, and matching expansion survives reanalysis. The driver also loads
`04-compact-pane.sql` to check CREATE/ALTER and EXEC separation and three preview rows, then checks repeated decoded operations,
distinct EXEC containers, unknown runtime SQL, scoped filtering, source/EXEC links, neutral supporting outlines and
persistent dynamic-SQL highlights. A shared EXEC
anchor uses the worst visible verdict; a green child must not paint over a red sibling in All view. The suites raise
control events inside the owned test process; they do not synthesize physical clicks or key presses.

`-CheckCodeBodies` loads all 13 examples in `05-other-code-bodies.sql`, checks typed previews, context tooltips,
actual object/operation risk markers, purple Inside/gray Remote totals, complete descendant badges,
and a scoped preview row with a circular marker and actual risk counts on the right,
execution/preview count separation, yellow function-call rows, opaque CLR bodies and the existing click/double-click
paths on every body type. A second fixture nests a stored job command and a remote query inside a procedure; it checks
literal-source navigation, recursive expansion restoration and persistent risk fills and full-definition context
outlines through passive reanalysis, including clearing both on a click elsewhere.
The driver captures both panes and editor surfaces at the normal compact width. It runs on the same private desktop
without connections or SQL execution.

It also runs `automaticpanecheck`: 19 checks for circular preview markers, submitted/preview count separation,
all function forms, independent risk/scope toggles, reachable ancestors, body click/double-click behavior and label-mode
state. Additional captures show the open trigger body at compact/minimum widths and the text-label presentation.
The Release WPF suite tests automatic resizing, forced labels and independent toggle clicks with large counts.
The same lane runs `panepolishcheck` with context-only filtering, a mixed decoded USE/SELECT block, a body preview,
and one- through four-digit lines. It verifies semantic toggle tooltips, context counts independent of Safe, and the
actual shared gutter edge across object, operation and preview headers. The Release WPF suite also verifies that
filtering keeps the gutter stable and that a new analysis releases unused width.

The same code-body lane runs `collapsedsummarycheck` on a six-color fixture. It verifies operation totals and hover meanings against the filter controls, independence from All/Writes/None display masks, and removal of stale circles while pending. Captures show the scope-only expanded header and the real collapsed strip. Release WPF checks additionally cover zero-category omission, numbered safe-only results, five-digit counts and the single expand target.

`-CheckBalancedPane` checks a synthetic object containing two red, one yellow and one green operation. It verifies
one object row, full counts under every filter, first-visible-operation navigation, matching highlights, preserved SQL
selection, independent reason expansion, empty-filter recovery and state after reanalysis. This fixture tests presentation;
it does not establish that the analyzer associates arbitrary reads with an object. Dispatcher receipts check object and
operation highlights at outer, procedure-body and dynamic-SQL levels: they persist beyond the former timeout and through passive
analysis, remain on the chosen row and clear on a click elsewhere. Captures then use actual analyzed SQL with each display filter.
Filters never change the execution verdict or the SQL submitted by F5.
The driver also shrinks the real pane, captures its minimum width, and checks the filter row after host layout settles,
allowing one DIP for pixel rounding. The Release WPF script separately checks minimum width with five-digit counts,
and verifies that temporary-only operations have no permanent-write risk label.

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
The `settingsappearancecheck` steps expand the six-row legend and open the warning ComboBox. They require supported
shell styles and theme foregrounds, then capture both states and the separate popup for visual inspection.
After startup, `setcapturetheme` applies the requested theme through the native settings manager in the isolated profile;
Dark/Light captures require the corresponding live shell background brightness. Startup JSON alone is insufficient
because the shell can overwrite it during settings migration. The driver still restores the original settings file.

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

Adjacent-row outline coverage uses both the offline WPF geometry checks (including a gap) and `-CheckBalancedPane`
in the real editor. Primary fill adornments carry no stroke; one union geometry carries the outer outline.
