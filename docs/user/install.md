# Install guide

How to download, install, update and remove SQL Safety Guard for yourself. To install it for every user of a computer,
see the [IT deployment guide](../it/deployment.md). Once installed, continue with the [user guide](user-guide.md).

## Requirements

- 64-bit **SQL Server Management Studio 22.0 or a later 22.x release**. The package is limited to SSMS 22; a future
  major version needs separate verification. Focused checks covered representative builds from 22.0 to 22.11; the
  evidence and remaining gaps are in [status.md](../status.md).
- No administrator rights. The normal installation is per user.
- SSMS must be closed while installing, updating or uninstalling.

## Download

All files are on [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases):

| File | What it is |
|---|---|
| `SqlSafetyGuard-<version>.vsix` | The extension. This is all most people need. |
| `SqlSafetyGuard-<version>.zip` | The identical VSIX plus install/uninstall scripts, `README.txt`, legal notices and the optional company signing kit |
| `SqlSafetyGuard-<version>-scripts.zip` | Only `install.ps1`, `uninstall.ps1` and their helper |
| `SqlSafetyGuard-<version>-signing.zip` | Only the company signing kit; see [company signing](../it/signing.md) |
| `SHA256SUMS.txt` | Checksums of the files above |

GitHub packages are **unsigned**. To check a download, compare `Get-FileHash -Algorithm SHA256 <file>` with the line in
`SHA256SUMS.txt`. Extract a ZIP before installing from it.

## Install

SQL Safety Guard installs for one Windows account: the account you're signed in to when you install it.

1. Save your work and close SSMS.
2. Sign in to the Windows account you use SSMS with.
3. Install the downloaded `.vsix`:
   - If Windows blocks the file, right-click it, choose **Properties**, tick **Unblock**, then choose **OK**.
   - Double-click the `.vsix` and follow the installer.
   - Optionally, restart the computer.
4. Start SSMS and open a query window. The safety pane appears on the right of the editor.

If IT has already installed an all-users copy on your computer, do not add a second copy.

### Install with the script

Download and extract the scripts ZIP (or the full ZIP) from the same release, open PowerShell in that folder and run:

```powershell
.\install.ps1 -VsixPath .\SqlSafetyGuard-<version>.vsix
```

The script finds SSMS 22, validates the package, removes a previous version, installs with SSMS's own installer and
refreshes SSMS's extension cache. From a source checkout, `.\install.ps1` without arguments installs the newest local
build. Details for administrators are in [how the install scripts work](../it/deployment.md#how-the-install-scripts-work).

## Update

Close SSMS and install the newer VSIX the same way; it replaces the old version. Keep the previous `.vsix` until the
new one works, so you can reinstall it (`.\install.ps1 -VsixPath <previous.vsix>`) if something goes wrong.
The [changelog](../../CHANGELOG.md) lists what changed in each version.

## Uninstall

For a normal per-user installation:

1. Open **Tools > Options > SQL Safety Guard** and choose **Uninstall SQL Safety Guard...** (below the settings).
   The same command is under **Tools > SQL Safety Guard**.
2. Choose **Yes** to open SSMS's uninstaller. **No** is the default and changes nothing.
3. Save your work and close SSMS when the installer asks, then finish the removal there.

Opening the installer alone does not remove anything, and the command does not switch off the guard. It removes the
extension from the SSMS installation and profile that is running, including an isolated `/rootSuffix` profile.
An all-users copy installed by IT shows a message to contact IT instead.

If SSMS cannot load the extension, close SSMS and run the script from the scripts download:

```powershell
.\uninstall.ps1
```

It removes the per-user installation from SSMS's default profile. Run it elevated to remove an all-users copy as well.
It does not target isolated `/rootSuffix` profiles; remove those with
`VSIXInstaller /instanceIds:<id> /rootSuffix:<suffix> /uninstall:SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558`.

## If the safety pane does not appear

Close SSMS, install the `.vsix` again and restart SSMS. If the pane is still missing, close SSMS and run
`SSMS.exe /updateconfiguration` from `<SSMS>\Common7\IDE` (it rebuilds SSMS's extension cache and takes about
10 seconds), or ask IT to do it. The install scripts do this step automatically.

## Older versions

- The first published 1.0.0 package (November 4) can lack the **Tools > SQL Safety Guard** submenu and the uninstall
  button. Install 1.0.1 or later, or remove it with `uninstall.ps1`.
- The 1.0.0 package was later replaced without changing its version number. Two packages with the same version are not
  a normal upgrade: uninstall and reinstall, and identify the package by its published checksum.
