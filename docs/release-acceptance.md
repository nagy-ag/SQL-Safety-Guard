# Release acceptance — 2026-10-27

Status: accepted for the user-approved Light and Dark theme scope. Automated/delegated observations and user-reported checks are distinguished below.

## Completed

- Release solution build succeeded and all 416 unit tests passed.
- Server harness Release build succeeded with zero warnings and zero errors.
- Pinned the harness dependency System.Security.Cryptography.Pkcs to 9.0.21, the first patched 9.0 version listed for GHSA-2fmv-rv2c-59x7 and GHSA-846f-f764-76mx. This overrides the vulnerable 9.0.13 transitive dependency without suppressing audit warnings.
- NuGet vulnerability audit including transitive harness dependencies reported no vulnerable packages against the configured sources.
- Reinstalled the Release VSIX using install.ps1 and the supported VSIXInstaller on SSMS 22.11.12324.101 (instance e8efe2d3). Installer exited successfully.
- Installed extension and analyzer DLL SHA-256 hashes match their Release build outputs:
  - SqlSafetyGuard.dll: 12BD66AA82988A91AB0BF779A627B232C054184A5B0D298870BFD3DAD682011D
  - SqlSafetyGuard.Analyzer.dll: F38F68A077126DE148659C3076A5262144147BABF443F4B40A787BE80E441EA7

## SSMS runtime acceptance

- Delegated UI checks confirmed the installed extension loads, with its toolbar, status indicator, and results pane visible. Manual Analyze follows SQLQuery1/SQLQuery2, reporting red UPDATE versus green SELECT and marking analysis as not executed.
- Production exact-name confirmation rejected blank and lowercase input and accepted SqlGuard_Prod. Enter defaulted to Cancel; cancellation preserved the sandbox counter at 0. One Run anyway click affected exactly one row and advanced the counter to 1.
- Development/Off passed both Execute-toolbar and F5 checks without a guard dialog. Manual Analyze remained available. The focused F5 check independently confirmed the sandbox counter advanced from 1 to 2.
- Delegated Light and Dark checks confirmed readable results, red/green risk indicators, Production labels/warnings, typed-confirmation instructions, disabled Run anyway, and accented default Cancel. Warning dialogs were canceled.
- Yellow/unknown-risk warning rendering was observed by a delegate in Cool Breeze. The final Light and Dark yellow-warning checks were completed manually by the user, who reported that they work. These two checks are user-reported, not agent-observed.

## Scope and restoration

- The user explicitly reduced the final theme scope from all 13 themes to one Light and one Dark theme. The other 11 themes and Windows-controlled contrast variants were not reaccepted in this final pass.
- Delegates restored the original Cool Breeze theme after the earlier successful theme checks. Final theme state after the user's manual checks was not independently verified.
- The user reported deleting both temporary sandbox environment rules, restoring the original empty rule list. This cleanup is user-reported, not independently verified.
- Testing used only the dedicated SqlGuard_Prod and SqlGuard_Dev databases. Sandbox tables remain; the last independently observed counters were Production 1 and Development 2.
- Intermittent Computer Use native-pipe failures prevented the final delegate from checking yellow warnings in Light/Dark. It made no UI or settings changes; the user's manual verification closes that remaining visual gate. An earlier inconclusive F5 capture was superseded by the successful focused F5 check, not classified as a confirmed product defect.
