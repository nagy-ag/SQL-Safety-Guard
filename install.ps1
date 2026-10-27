<#
.SYNOPSIS
    Installs SQL Safety Guard into SQL Server Management Studio 22.

.DESCRIPTION
    Default: runs SSMS's own VSIXInstaller.exe against the SSMS 22 setup instance (a per-user install under
    %LocalAppData%\Microsoft\SSMS\22.0_<instanceId>\Extensions). This is the supported VSIX mechanism; SSMS
    shows the extension in Extensions > Manage Extensions and can disable/uninstall it.

    -Copy: fallback that extracts the VSIX into <SSMS>\Common7\IDE\Extensions\SqlSafetyGuard (all users,
    requires an elevated PowerShell) and touches extensions.configurationchanged so SSMS rescans on next
    start. A manual copy into the per-user %LocalAppData% folder is NOT picked up by SSMS 22 (per-user
    extensions must be registered by VSIXInstaller), so that variant is not offered.

.EXAMPLE
    .\install.ps1
    .\install.ps1 -VsixPath .\src\SqlSafetyGuard.Vsix\bin\Debug\SqlSafetyGuard.vsix
    .\install.ps1 -Copy    # from an elevated PowerShell
#>
[CmdletBinding()]
param(
    [string] $VsixPath,
    [switch] $Copy
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'scripts\SsmsCommon.ps1')

$ssms = Get-Ssms22
Write-Host "SSMS $($ssms.Version) at $($ssms.InstallPath) (instance $($ssms.InstanceId))"
Assert-SsmsNotRunning

if (-not $VsixPath) {
    $VsixPath = Get-ChildItem (Join-Path $PSScriptRoot 'src\SqlSafetyGuard.Vsix\bin') -Recurse -Filter 'SqlSafetyGuard.vsix' -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1 -ExpandProperty FullName
    if (-not $VsixPath) { throw 'No SqlSafetyGuard.vsix found. Build the solution first (see README.md).' }
}
$VsixPath = (Resolve-Path $VsixPath).Path
Write-Host "VSIX: $VsixPath"

if (-not $Copy) {
    Install-WithVsixInstaller -VsixPath $VsixPath -Ssms $ssms
    Write-Host 'Installed with VSIXInstaller.' -ForegroundColor Green
}
else {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw '-Copy writes to the SSMS program folder; run PowerShell as Administrator.'
    }
    $extensionsRoot = Join-Path $ssms.InstallPath 'Common7\IDE\Extensions'
    $target = Install-ManualCopy -VsixPath $VsixPath -ExtensionsRoot $extensionsRoot
    Write-Host "Copied to $target" -ForegroundColor Green
}

Write-Host 'Refreshing the SSMS extension cache...'
if (Update-SsmsConfiguration -Ssms $ssms) { Write-Host 'Extension cache refreshed.' -ForegroundColor Green }

Write-Host 'Start SSMS 22. Query windows show the safety pane on the right; also see Tools > Analyze Query Safety (Ctrl+Shift+F5) and Tools > Options > SQL Safety Guard.'
