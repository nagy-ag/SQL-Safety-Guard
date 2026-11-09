# SQL Safety Guard — IT deployment guide

For administrators installing SQL Safety Guard on company laptops. Users can also install it for themselves without
administrator rights (see the [install guide](../user/install.md)); pick one of the two per laptop.

## What it is (for security review)

- An extension for SQL Server Management Studio (SSMS) 22 that analyzes T-SQL text before execution and shows a verdict.
- **It never executes SQL**, opens no connections of its own, makes **no network calls** and has **no telemetry**. Query text
  never leaves the machine. Settings are stored in the user's SSMS settings.
- It is a safety net against mistakes, **not a security boundary**; database permissions remain the real protection.
- GitHub packages are **unsigned**. If application control requires a signature, IT can sign and validate an internal copy
  on the company computer; see [vsix-signing.md](signing.md). Keep its checksum separate from the GitHub package.

## Requirements

- Windows with SQL Server Management Studio 22 (any 22.x). .NET Framework 4.7.2 or later (already required by SSMS 22).
- SSMS must be closed during install, upgrade and uninstall.

## Release files (GitHub Releases)

| File | Use |
|---|---|
| `SqlSafetyGuard-<version>.vsix` | The extension |
| `SqlSafetyGuard-<version>-scripts.zip` | `install.ps1`, `uninstall.ps1`, `scripts\SsmsCommon.ps1` |
| `SqlSafetyGuard-<version>.zip` | Identical VSIX, install/uninstall scripts, instructions, legal notices and signing kit |
| `SqlSafetyGuard-<version>-signing.zip` | Company signing helper and guide; no keys, certificates or tool binaries |
| `SHA256SUMS.txt` | Checksums; verify with `Get-FileHash -Algorithm SHA256` |

Extract the full ZIP, or extract the scripts ZIP and put the `.vsix` next to `install.ps1`. Download `SHA256SUMS.txt`
beside that folder's VSIX for the signing helper; see [vsix-signing.md](signing.md). Unblock the files if they were downloaded
(`Get-ChildItem -Recurse | Unblock-File`).

## Install for all users of a laptop (administrator)

From an **elevated** PowerShell in that folder:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\install.ps1 -Copy -VsixPath .\SqlSafetyGuard-<version>.vsix
```

The script finds SSMS 22, validates the package, extracts it into `<SSMS>\Common7\IDE\Extensions\SqlSafetyGuard`, and refreshes
SSMS's extension cache (about 10 seconds). Running it again with a newer `.vsix` upgrades in place; a failed replacement
restores the previous files.

**Uninstall** (elevated):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\uninstall.ps1
```

This removes the all-users copy and, for the account running it, any per-user install.

Per-user VSIX installations offer **Tools > Options > SQL Safety Guard > Uninstall SQL Safety Guard...** beside Import
and Export. The Tools > SQL Safety Guard submenu also registers this command, although its live popup remains unverified.
The command confirms first, then opens the adjacent SSMS VSIXInstaller for the running instance/profile. Users save work
and close SSMS when prompted; it does not elevate, delete loaded files or disable the query guard. It refuses all-users
manual copies and unrecognized paths with administrator guidance. Keep using the elevated script for all-users removal.

## Rules

- **Do not combine** an all-users install with a per-user install on the same laptop. If a user already installed it for
  themselves, have them uninstall it first (the in-extension command, or the uninstall script with SSMS closed).
- Keep the previous `.vsix` until the new version is confirmed working, so you can reinstall it.
- The owner replaced the original 1.0.0 package while retaining its version. For a laptop with that older package,
  uninstall/reinstall rather than relying on same-version upgrade detection; identify the replacement by its checksum.

## How the install scripts work

`install.ps1` finds the newest SSMS 22 with `vswhere` (product `Microsoft.VisualStudio.Product.Ssms`, versions compared
numerically), validates the package, removes any previous version, and runs **SSMS's own `VSIXInstaller.exe`**
against that SSMS setup instance:

```
"<SSMS>\Common7\IDE\VSIXInstaller.exe" /quiet /instanceIds:<instanceId> SqlSafetyGuard.vsix
```

This is a per-user install: the files land in `%LocalAppData%\Microsoft\SSMS\22.0_<instanceId>\Extensions\<random>\` and
are registered with that SSMS installation. Double-clicking the `.vsix` does the same (Explorer opens it with
VSLauncher, which hands it to VSIXInstaller for SSMS 22 when that is the only matching product). SSMS 22 builds can omit
**Extensions > Manage Extensions**; use the uninstall command or script instead.

`-VsixPath <path>` installs a specific build. `-Copy` (elevated PowerShell only) extracts the VSIX into
`<SSMS>\Common7\IDE\Extensions\SqlSafetyGuard` and touches `extensions.configurationchanged`. The copy path rejects unsafe
archive paths (traversal, rooted/alternate-stream paths and duplicate entries), validates the manifest and required
assemblies, and extracts into a sibling staging directory **before** moving the current install; a failed replacement
restores the backup. The VSIXInstaller path aborts on a failed uninstall and reports failed upgrades with recovery
instructions. Installer log paths are quoted even when TEMP contains spaces, and uninstall failures return a nonzero
exit code instead of printing success.

After installing or uninstalling, both scripts run `SSMS.exe /updateconfiguration` (hidden, about 10 seconds) so SSMS
rebuilds its editor component cache. The safety pane is an editor (MEF) component; without this step SSMS can keep a
stale cache after an upgrade and the pane silently doesn't appear (or SSMS reports a missing file). If the step fails,
the script prints the command to run by hand with SSMS closed.

`uninstall.ps1` runs `VSIXInstaller /uninstall:SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558` for the discovered
SSMS instance's default profile and removes any `-Copy` install folder (elevated). Remove an isolated profile with
`VSIXInstaller /instanceIds:<id> /rootSuffix:<suffix> /uninstall:<extension-id>`.

### Installation methods checked on SSMS 22.11.2 (VS shell 18.11)

- ✅ VSIXInstaller per-user install (`install.ps1`): loads, all features work.
- ✅ Double-clicking the `.vsix`: same per-user install.
- ✅ Manual copy into `<SSMS>\Common7\IDE\Extensions` (`install.ps1 -Copy`, elevated): on the next start SSMS
  regenerates its pkgdef cache, imports `SqlSafetyGuard.pkgdef`, and the package, command, shortcut and F5 guard all
  work. `uninstall.ps1` (elevated) removes it. Don't combine it with a per-user install of the same extension.
- ❌ Manual copy into `%LocalAppData%\Microsoft\SSMS\22.0_<id>\Extensions\<folder>` plus touching
  `extensions.configurationchanged`: **not picked up**. Per-user extensions must be registered by VSIXInstaller, so
  `install.ps1` doesn't offer this.

## Checks after install

1. Start SSMS 22 as a normal user: a query window shows the safety pane on its right.
2. Type `SELECT 1` (green) and `UPDATE dbo.SomeTable SET x = 1` (red); press F5 on the red one against a test database and
   confirm the dialog appears, then cancel.
3. **Tools > Options > SQL Safety Guard** opens.

The all-users method was verified on SSMS 22.11; validate it once on a test laptop with your SSMS build before a wide rollout.
