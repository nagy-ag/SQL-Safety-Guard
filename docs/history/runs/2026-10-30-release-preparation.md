> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# 1.0.0 preparation verification — 2026-10-30

The extension remains version 1.0.0 and the changelog remains unreleased. This work changes guard-failure decisions,
installer handling, release tooling and documentation; it changes no analyzer verdicts. Implementation revisions:
`e173364`, `f4bfb91`, `5bc2050`. No tag, push, GitHub release, signature or publishing assets were produced.

## Checks

| Gate | Result |
|---|---|
| Full unit suite | 1,571 passed; zero failed/skipped |
| Visual Studio MSBuild, Debug solution | Zero warnings, zero errors |
| Visual Studio MSBuild, Release solution | Zero warnings, zero errors |
| PaneInteractionTests, Release | 13 passed |
| ConfirmationInteractionTests, Release | 15 passed |
| SettingsInteractionTests, Release | 4 passed |
| InstallerSafetyTests, Windows PowerShell 5.1 | 60 passed, including uninstall exit 1002 followed by successful install |
| Generated inventory check | 527 rules current; generated source/test references refreshed, no verdict changes |
| Release-asset script, Windows PowerShell 5.1 | Passed with SkipUiChecks in an external temporary directory |

The three Release UI scripts were run separately without skipping any checks. The asset trial wrote the VSIX,
scripts ZIP and SHA256SUMS.txt. Both hashes were independently checked; the ZIP contained exactly install.ps1,
uninstall.ps1 and scripts/SsmsCommon.ps1 in that relative layout. Its output folder was then deleted.
Dirty-tree refusal was checked. The first artifact validation caught the local profile path in the analyzer PDB's
JSON-escaped SourceLink map; Release now normalizes that map along with compiler document paths. The final validation
passed ZIP CRCs, manifest identity/version/SSMS target, required files, license inclusion, unsigned/private-key checks,
and UTF-8/UTF-16LE profile-path checks (including JSON-escaped paths).

## Live failure confirmation

Installed Debug into the existing isolated SSMS Exp profile, refreshed its configuration, and launched an owned SSMS
process hidden. A private Debug channel and SQLSAFETYGUARD_FAULT_INJECTION=1 invoked the production OnBeforeExecute
handler. The channel recorded the continuation decision instead of dispatching Query.Execute or sending SQL.
The current theme was left unchanged; no mouse, keyboard, clipboard or foreground-focus automation was used.

- With no verdict, YellowRed settings and an unverified target, the injected InvalidOperationException showed the
  actual themed failure dialog and its short reason. Cancel was default and IsCancel was true (Escape cancellation).
- Invoking Cancel on that dialog returned CancelDefault=true: the continuation was blocked.
- Invoking Run anyway on a second injected failure returned CancelDefault=false: the continuation was allowed.
- The same handler retains failure logging, degraded health and the once-per-session InfoBar attempt. A computed
  red/yellow warning cannot be bypassed by a later presentation/logging failure.

Unit cases cover known Off plus verified target (run without a question), Off plus unverified target (ask), both
Red and YellowRed modes (ask), unknown settings (ask), both button answers, a throwing failure box (cancel, even if
reporting also throws), and computed-warning preservation. UI contracts require buttons only and safe defaults.

No physical F5/Escape key input, actual server submission or independent InfoBar rendering check was performed.
The all-themes pass is reserved for the owner's separate review. This check does not renew earlier server evidence.
The owned SSMS process was closed with WM_CLOSE. No temporary SSMS profiles were created. Exp retains the prepared
Debug build; the normal SSMS profile was not installed into or changed.
