<#
.SYNOPSIS
    Removes SQL Safety Guard from SQL Server Management Studio 22 (both VSIXInstaller and manual-copy installs).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'scripts\SsmsCommon.ps1')

$ssms = Get-Ssms22
Write-Host "SSMS $($ssms.Version) at $($ssms.InstallPath) (instance $($ssms.InstanceId))"
Assert-SsmsNotRunning

$installer = Join-Path $ssms.InstallPath 'Common7\IDE\VSIXInstaller.exe'
$installerArgs = @('/quiet', "/instanceIds:$($ssms.InstanceId)", "/logFile:`"$env:TEMP\SqlSafetyGuard_VSIXInstaller.log`"", "/uninstall:$ExtensionId")
$process = Start-Process -FilePath $installer -ArgumentList $installerArgs -WindowStyle Hidden -Wait -PassThru
switch ($process.ExitCode) {
    0 { Write-Host 'Uninstalled with VSIXInstaller.' -ForegroundColor Green }
    2003 { Write-Host 'Not installed through VSIXInstaller.' }
    default { throw "VSIXInstaller exit code $($process.ExitCode) (log: $env:TEMP\SqlSafetyGuard_VSIXInstaller.log). Uninstall failed." }
}

if (Remove-ManualCopies -Ssms $ssms) {
    Write-Host 'Removed manually copied extension files.' -ForegroundColor Green
}

Write-Host 'Done. Restart SSMS.'
