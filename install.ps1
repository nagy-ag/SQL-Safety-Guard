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
    $installer = Join-Path $ssms.InstallPath 'Common7\IDE\VSIXInstaller.exe'

    # Remove a previous install of the same extension, so rebuilding and reinstalling the same version works.
    $removeArgs = @('/quiet', "/instanceIds:$($ssms.InstanceId)", "/logFile:$env:TEMP\SqlSafetyGuard_VSIXInstaller.log", "/uninstall:$ExtensionId")
    $removal = Start-Process -FilePath $installer -ArgumentList $removeArgs -Wait -PassThru
    if ($removal.ExitCode -eq 0) { Write-Host 'Removed the previously installed version.' }

    $installerArgs = @('/quiet', "/instanceIds:$($ssms.InstanceId)", "/logFile:$env:TEMP\SqlSafetyGuard_VSIXInstaller.log", "`"$VsixPath`"")
    Write-Host "Running $installer $($installerArgs -join ' ')"
    $process = Start-Process -FilePath $installer -ArgumentList $installerArgs -Wait -PassThru
    if ($process.ExitCode -ne 0) {
        throw "VSIXInstaller failed with exit code $($process.ExitCode). Log: $env:TEMP\SqlSafetyGuard_VSIXInstaller.log"
    }
    Write-Host 'Installed with VSIXInstaller.' -ForegroundColor Green
}
else {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw '-Copy writes to the SSMS program folder; run PowerShell as Administrator.'
    }
    $extensionsRoot = Join-Path $ssms.InstallPath 'Common7\IDE\Extensions'
    $target = Join-Path $extensionsRoot $ExtensionFolderName
    Remove-ManualCopies -Ssms $ssms
    New-Item -ItemType Directory -Force -Path $target | Out-Null

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::OpenRead($VsixPath)
    try {
        foreach ($entry in $zip.Entries) {
            if ($entry.FullName -in '[Content_Types].xml', 'manifest.json', 'catalog.json' -or $entry.FullName.EndsWith('/')) { continue }
            $destination = Join-Path $target ($entry.FullName -replace '/', '\')
            New-Item -ItemType Directory -Force -Path (Split-Path $destination) | Out-Null
            [IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destination, $true)
        }
    }
    finally { $zip.Dispose() }

    Update-ExtensionsConfigurationChanged -Path $extensionsRoot
    Write-Host "Copied to $target" -ForegroundColor Green
}

Write-Host 'Start SSMS 22. Look for Tools > Analyze Query Safety, the SQL Editor toolbar button and Tools > Options > SQL Safety Guard.'
