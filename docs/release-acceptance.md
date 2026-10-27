# Release acceptance — 2026-10-27

Status: pending SSMS runtime acceptance. Do not treat this record as final release approval.

## Completed

- Release solution build succeeded and all 416 unit tests passed.
- Server harness Release build succeeded with zero warnings and zero errors.
- Pinned the harness dependency System.Security.Cryptography.Pkcs to 9.0.21, the first patched 9.0 version listed for GHSA-2fmv-rv2c-59x7 and GHSA-846f-f764-76mx. This overrides the vulnerable 9.0.13 transitive dependency without suppressing audit warnings.
- NuGet vulnerability audit including transitive harness dependencies reported no vulnerable packages against the configured sources.
- Reinstalled the Release VSIX using install.ps1 and the supported VSIXInstaller on SSMS 22.11.12324.101 (instance e8efe2d3). Installer exited successfully.
- Installed extension and analyzer DLL SHA-256 hashes match their Release build outputs:
  - SqlSafetyGuard.dll: 12BD66AA82988A91AB0BF779A627B232C054184A5B0D298870BFD3DAD682011D
  - SqlSafetyGuard.Analyzer.dll: F38F68A077126DE148659C3076A5262144147BABF443F4B40A787BE80E441EA7

## Remaining acceptance gates

- Launch SSMS and verify the installed build loads and Analyze follows the active editor tab.
- Verify Production warning/typed confirmation, cancellation without execution, and Run anyway executing exactly once in the disposable sandbox.
- Verify the SqlGuard_Dev Development rule with guard mode Off executes without a guard dialog while manual Analyze remains available.
- Repeat results-window and confirmation-dialog readability checks in all 13 selectable themes: Light, Cool Breeze, Icy Mint, Sunny Day, Juicy Plum, Mango Paradise, Bubblegum, Silky Pink, Dark, Cool Slate, Moonlight Glow, Mystical Forest, Spicy Red. Include environment labels, risk colours, and the default Cancel accent; restore the original theme.
- Windows-controlled contrast variants remain outside this 13-theme check.
- Publish accepted main only after the runtime gates pass.

## Blocker

Windows Computer Use could not connect: "Computer Use native pipe is unavailable: failed to connect native pipe: The system cannot find the file specified. (os error 2)". Retrying and resetting the JavaScript session did not restore access. No SSMS UI actions or new theme checks were performed in this acceptance attempt. Earlier UI/theme results do not establish acceptance of this installed build.
