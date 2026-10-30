# Release 1.0.0 acceptance

Date: 2026-10-30. Tested commit: `2848b4f82faffaed087df29a9e3a156c209a1f59`. SSMS: 22.11.12324.101.

**Overall verdict: accepted** for the checks described here. No visual issues were found in the captured states. This is not a test of real query execution.

## Builds, tests and installation

- Visual Studio MSBuild Debug and Release solution builds each succeeded with 0 warnings and 0 errors.
- `dotnet test tests\SqlSafetyGuard.Tests` passed 1,574 tests, with 0 failed and 0 skipped, before and after the documentation update.
- Release interaction scripts passed 13 pane, 15 confirmation and 4 settings checks. These are in-process checks; they do not test physical input or execute SQL.
- Debug VSIX was uninstalled, installed and refreshed in the isolated Exp profile. Installed DLL hashes matched the Debug build: `SqlSafetyGuard.dll` SHA-256 `48C77AA0F61E01DD452DEC706358D5F07AA05B47EEF95B556D52DECB5FF273C5`; `SqlSafetyGuard.Analyzer.dll` SHA-256 `92F92452AC4C25B4B484FB333CAF6EFF3A552C0B540B06DB81F73E130F531815`.

## Visual review

All 169 images were opened and checked individually for readability, risk colours, clipping, buttons and default Cancel, themed backgrounds and consistent labels. Each cell below is the verdict for the named captured surface. No issues or issue screenshot paths were recorded. The images and per-image CSV are stored outside the repository in the acceptance run output; surface names correspond to PNG filenames in each theme folder. Dialog images show client areas; the main and options images show the SSMS window. The previews use the Debug test channel and do not execute SQL.

| Theme | pane-green | pane-yellow | pane-red | confirm-yellow | confirm-red | failure | list-allowList | list-trustedProcedures | list-environmentRules | import-preview | pane-settings | main-toolbar | options |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Light | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Cool Breeze | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Icy Mint | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Sunny Day | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Juicy Plum | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Mango Paradise | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Bubblegum | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Silky Pink | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Dark | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Cool Slate | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Moonlight Glow | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Mystical Forest | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| Spicy Red | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS | PASS |

## Restoration and limits

The Exp settings file SHA-256 before and after capture was `27307919E64630C81FEB920F868A37D7307B4C4FBFC3EF66FC30A1DD985E9BE5`. The restoration record says restored and SSMS exited; a separate post-run hash matched. The capture-owned SSMS instances were closed, and Exp retains the tested Debug build.

Not verified: physical keyboard F5, Ctrl+E or Alt+X; real SQL execution against a server; the double-click VSIX installer UI; Windows high-contrast themes; and SSMS versions other than 22.11.12324.101. The screenshots do not establish execution guard behavior.