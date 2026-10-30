# SQL Safety Guard — IT deployment guide

For administrators installing SQL Safety Guard on company laptops. Users can also install it for themselves without
administrator rights (see [user-guide.md](user-guide.md)); pick one of the two per laptop.

## What it is (for security review)

- An extension for SQL Server Management Studio (SSMS) 22 that analyzes T-SQL text before execution and shows a verdict.
- **It never executes SQL**, opens no connections of its own, makes **no network calls** and has **no telemetry**. Query text
  never leaves the machine. Settings are stored in the user's SSMS settings.
- It is a safety net against mistakes, **not a security boundary**; database permissions remain the real protection.
- The package is **not code-signed** yet. If application control requires signed packages, it must be allowed explicitly
  until a signed release is available.

## Requirements

- Windows with SQL Server Management Studio 22 (any 22.x). .NET Framework 4.7.2 or later (already required by SSMS 22).
- SSMS must be closed during install, upgrade and uninstall.

## Release files (GitHub Releases)

| File | Use |
|---|---|
| `SqlSafetyGuard-<version>.vsix` | The extension |
| `SqlSafetyGuard-<version>-scripts.zip` | `install.ps1`, `uninstall.ps1`, `scripts\SsmsCommon.ps1` |
| `SHA256SUMS.txt` | Checksums; verify with `Get-FileHash -Algorithm SHA256` |

Extract the scripts zip into one folder, put the `.vsix` next to `install.ps1`, and unblock the files if they were downloaded
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

## Rules

- **Do not combine** an all-users install with a per-user install on the same laptop. If a user already installed it for
  themselves, have them uninstall it first (SSMS > Extensions > Manage Extensions).
- Keep the previous `.vsix` until the new version is confirmed working, so you can reinstall it.

## Checks after install

1. Start SSMS 22 as a normal user: a query window shows the safety pane on its right.
2. Type `SELECT 1` (green) and `UPDATE dbo.SomeTable SET x = 1` (red); press F5 on the red one against a test database and
   confirm the dialog appears, then cancel.
3. **Tools > Options > SQL Safety Guard** opens.

The all-users method was verified on SSMS 22.11; validate it once on a test laptop with your SSMS build before a wide rollout.
