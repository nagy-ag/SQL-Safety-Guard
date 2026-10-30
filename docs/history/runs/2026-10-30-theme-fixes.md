> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Theme readability and settings consistency — 2026-10-30

All 13 SSMS themes passed visual review of all 13 captured surfaces after the fixes below. The final run produced
169 non-blank PNGs. Every PNG was personally inspected using four full-size contact sheets per theme, preserving
the original pixels. No unreadable text, unthemed extension backgrounds, clipped text, missing default indication
or incorrectly coloured verdict badges remained in those captures.

## Product changes

| Defect | Fix and files |
|---|---|
| Dark finding rows inherited near-black text | Explicit tool-window text resources on row containers/expanders, themed grey secondary text, and themed validation-error text in `SafetyPaneControl.xaml`. Section headings use readable themed text beside verdict markers. |
| Pane settings button used the Windows button appearance and an obsolete label | `Settings...` uses the dialog button style in `SafetyPaneControl.xaml`; `SafetyPaneMargin.cs` and `SettingsCommands.cs` share the menu's settings action. |
| Environment rules description referred to a repository path | `registration.json` and `GeneralOptionsPage.cs` refer to the User guide on the SQL Safety Guard GitHub project page. Other setting descriptions contain no repository paths. |
| Import preview used a different trusted-procedures label | `SettingsTransfer.cs` now displays `Trusted procedures`, matching the settings page and pane. JSON keys and transfer format are unchanged. |
| Capture previews forced enabled actions grey | `SafetyPaneMargin.Capture.cs` retains production Save/Import enabled state. Preview handlers remain inert. |

The first full run additionally exposed low-contrast amber execution-target warning text in Light. The confirmation
now uses themed panel text with an amber edge (`ConfirmExecutionDialog.xaml` and its code-behind). List-editor Cancel
already was the default action but lacked an explicit default accent outline; `SettingsListDialog.xaml` now supplies it.
Pane and confirmation verdict shapes have themed outlines to preserve their edges against both backgrounds. Existing
semantic red/yellow/green fills remain unchanged; their text contrast is approximately 5.62:1, 10.66:1 and 5.13:1.

## Final capture matrix

Each row covers green/yellow/red pane, pane settings, yellow/red confirmation, guard failure, three list editors,
import preview, integrated settings and main-window toolbar.

| Theme | Surfaces reviewed | Result |
|---|---:|---|
| Light | 13 | Clean |
| Cool Breeze | 13 | Clean |
| Icy Mint | 13 | Clean |
| Sunny Day | 13 | Clean |
| Juicy Plum | 13 | Clean |
| Mango Paradise | 13 | Clean |
| Bubblegum | 13 | Clean |
| Silky Pink | 13 | Clean |
| Dark | 13 | Clean |
| Cool Slate | 13 | Clean |
| Moonlight Glow | 13 | Clean |
| Mystical Forest | 13 | Clean |
| Spicy Red | 13 | Clean |

External evidence is retained in `theme-fix-1` (discovery) and `theme-fix-2` (final), including PNGs, `manifest.csv`,
`restoration.json`, capture receipts and full-size review sheets. The final folder also contains `visual-review.json`.
No raw capture output or images are committed.

The script ran only owned SSMS processes in Exp on private, undisplayed desktops. No real mouse, keyboard, clipboard
or foreground-focus manipulation was used. No SQL or Execute command was dispatched. Processes closed with WM_CLOSE.
Both full runs restored settings byte-for-byte and recorded `SsmsExited=true`, `Restored=true` and an empty error.
The final settings SHA-256 before and after is:

```text
27307919E64630C81FEB920F868A37D7307B4C4FBFC3EF66FC30A1DD985E9BE5
```

The disabled native SSMS execution controls in disconnected captures are expected (`dark/main-toolbar.png`).
The integrated settings page scrolls normally (`dark/options.png`). These are acceptable; no extension theme defects
remain in the reviewed matrix. Windows contrast themes and actual F5/SQL execution are outside this visual check.

## Validation

- All 1,574 tests pass, including DocsIndexTests, AgentSkillsSyncTests and updated UI contracts.
- Visual Studio MSBuild Debug and Release solution builds each finish with zero warnings and zero errors.
- Release interaction scripts pass: 13 pane, 15 confirmation and 4 settings checks.
- Analyzer and guard logic are unchanged. This run adds no server-verification evidence.
- Exp retains the final Debug build; its installed extension and analyzer DLL hashes match the Debug output.

The user guide and development notes describe the current labels and theme behavior; the 1.0.0 changelog records
the dark-theme readability fix. No push, tag or release publication was performed.
