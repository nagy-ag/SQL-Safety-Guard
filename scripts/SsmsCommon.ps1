# Shared helpers for install.ps1 / uninstall.ps1.

$ExtensionId = 'SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558'
$ExtensionFolderName = 'SqlSafetyGuard'

function Get-Ssms22 {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $instance = $null
    if (Test-Path $vswhere) {
        $json = & $vswhere -all -prerelease -products Microsoft.VisualStudio.Product.Ssms -version '[22.0,23.0)' -format json
        $instance = $json | ConvertFrom-Json | Sort-Object installationVersion -Descending | Select-Object -First 1
    }

    if ($instance) {
        $installPath = $instance.installationPath
        $instanceId = $instance.instanceId
        $version = $instance.installationVersion
    }
    else {
        # Fallback: default install folder; the instance id is in SSMS.isolation.ini.
        $installPath = 'C:\Program Files\Microsoft SQL Server Management Studio 22\Release'
        $ini = Join-Path $installPath 'Common7\IDE\SSMS.isolation.ini'
        if (-not (Test-Path $ini)) { throw 'SQL Server Management Studio 22 was not found.' }
        $instanceId = ((Select-String -Path $ini -Pattern '^InstallationID=(.+)$').Matches[0].Groups[1].Value).Trim()
        $version = ((Select-String -Path $ini -Pattern '^InstallationVersion=(.+)$').Matches[0].Groups[1].Value).Trim()
    }

    [pscustomobject]@{
        InstallPath        = $installPath
        InstanceId         = $instanceId
        Version            = $version
        UserExtensionsPath = Join-Path $env:LOCALAPPDATA "Microsoft\SSMS\22.0_$instanceId\Extensions"
    }
}

function Assert-SsmsNotRunning {
    if (Get-Process -Name 'Ssms' -ErrorAction SilentlyContinue) {
        throw 'SSMS is running. Close all SSMS windows and run the script again.'
    }
}

function Update-ExtensionsConfigurationChanged([string] $Path) {
    # SSMS (like Visual Studio) rescans extensions when this file's timestamp changes.
    $marker = Join-Path $Path 'extensions.configurationchanged'
    if (Test-Path $marker) { (Get-Item $marker).LastWriteTime = Get-Date }
    else { New-Item -ItemType File -Path $marker | Out-Null }
}

function Remove-ManualCopies($Ssms) {
    $removed = $false
    foreach ($root in @($Ssms.UserExtensionsPath, (Join-Path $Ssms.InstallPath 'Common7\IDE\Extensions'))) {
        $folder = Join-Path $root $ExtensionFolderName
        if (Test-Path $folder) {
            Remove-Item -Recurse -Force $folder
            Update-ExtensionsConfigurationChanged -Path $root
            $removed = $true
        }
    }
    return $removed
}
