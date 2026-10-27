# Shared helpers for install.ps1 / uninstall.ps1.

$ExtensionId = 'SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558'
$ExtensionFolderName = 'SqlSafetyGuard'

function Select-LatestSsmsInstance($Instances) {
    $Instances | Sort-Object { [version]$_.installationVersion } -Descending | Select-Object -First 1
}

function Get-Ssms22 {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $instance = $null
    if (Test-Path $vswhere) {
        $json = & $vswhere -all -prerelease -products Microsoft.VisualStudio.Product.Ssms -version '[22.0,23.0)' -format json
        $instance = Select-LatestSsmsInstance -Instances ($json | ConvertFrom-Json)
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

function Update-SsmsConfiguration($Ssms) {
    # The safety pane is an editor (MEF) component. After an install, upgrade or removal, SSMS can keep using its
    # cached component catalog: the pane then silently doesn't load (or SSMS reports a missing file) until the
    # cache is rebuilt. SSMS.exe /updateconfiguration rebuilds it without opening a window (about 10 seconds).
    $ssmsExe = Join-Path $Ssms.InstallPath 'Common7\IDE\SSMS.exe'
    try {
        $process = Start-Process -FilePath $ssmsExe -ArgumentList '/updateconfiguration' -WindowStyle Hidden -PassThru
        if (-not $process.WaitForExit(180000)) { $process.Kill(); throw 'timed out after 3 minutes' }
        if ($process.ExitCode -ne 0) { throw "exit code $($process.ExitCode)" }
        return $true
    }
    catch {
        Write-Warning "Could not refresh the SSMS extension cache ($_). If query windows show no safety pane, close SSMS and run: & `"$ssmsExe`" /updateconfiguration"
        return $false
    }
}

function Remove-ManualCopies($Ssms) {
    $removed = $false
    foreach ($root in @($Ssms.UserExtensionsPath, (Join-Path $Ssms.InstallPath 'Common7\IDE\Extensions'))) {
        $folder = Join-Path $root $ExtensionFolderName
        if (Test-Path $folder) {
            Remove-ExtensionDirectory -Path $folder -Root $root
            Update-ExtensionsConfigurationChanged -Path $root
            $removed = $true
        }
    }
    return $removed
}

function Remove-ExtensionDirectory([string] $Path, [string] $Root) {
    $fullRoot = [IO.Path]::GetFullPath($Root).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath($Path)
    if (-not $fullPath.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to delete outside extension root: $fullPath"
    }
    if (Test-Path -LiteralPath $fullPath) {
        if ((Get-Item -LiteralPath $fullPath).Attributes -band [IO.FileAttributes]::ReparsePoint) {
            throw "Refusing to delete a reparse-point extension directory: $fullPath"
        }
        Remove-Item -LiteralPath $fullPath -Recurse -Force
    }
}

function Expand-ValidatedVsix([string] $VsixPath, [string] $Stage) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $root = [IO.Path]::GetFullPath($Stage).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    $zip = [IO.Compression.ZipFile]::OpenRead($VsixPath)
    try {
        $paths = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
        $entries = @()
        [long]$total = 0
        foreach ($entry in $zip.Entries) {
            $name = $entry.FullName.Replace('\', '/')
            $parts = $name.TrimEnd('/').Split('/')
            if ([string]::IsNullOrWhiteSpace($name) -or [IO.Path]::IsPathRooted($name) -or
                $name.Contains(':') -or $parts -contains '..' -or $parts -contains '.' -or $parts -contains '' -or
                @($parts | Where-Object { $_.EndsWith('.') -or $_.EndsWith(' ') }).Count -gt 0) {
                throw "Unsafe VSIX entry: $($entry.FullName)"
            }
            $destination = [IO.Path]::GetFullPath((Join-Path $Stage $name.Replace('/', [IO.Path]::DirectorySeparatorChar)))
            if (-not $destination.StartsWith($root, [StringComparison]::OrdinalIgnoreCase) -or -not $paths.Add($destination)) {
                throw "Unsafe or duplicate VSIX entry: $($entry.FullName)"
            }
            $total += $entry.Length
            if ($total -gt 256MB -or $paths.Count -gt 10000) { throw 'VSIX exceeds extraction limits.' }
            $entries += [pscustomobject]@{ Entry = $entry; Destination = $destination; Name = $name }
        }
        New-Item -ItemType Directory -Path $Stage -ErrorAction Stop | Out-Null
        foreach ($item in $entries) {
            if ($item.Name.EndsWith('/') -or $item.Name -in '[Content_Types].xml', 'manifest.json', 'catalog.json') { continue }
            New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($item.Destination)) | Out-Null
            [IO.Compression.ZipFileExtensions]::ExtractToFile($item.Entry, $item.Destination, $false)
        }
    }
    finally { $zip.Dispose() }
    foreach ($required in @('extension.vsixmanifest', 'SqlSafetyGuard.dll', 'SqlSafetyGuard.Analyzer.dll')) {
        if (-not (Test-Path -LiteralPath (Join-Path $Stage $required) -PathType Leaf)) { throw "VSIX is missing $required" }
    }
    $settings = New-Object Xml.XmlReaderSettings
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $settings.XmlResolver = $null
    $reader = [Xml.XmlReader]::Create((Join-Path $Stage 'extension.vsixmanifest'), $settings)
    try {
        $manifest = New-Object Xml.XmlDocument
        $manifest.XmlResolver = $null
        $manifest.Load($reader)
        $identity = $manifest.SelectSingleNode("/*[local-name()='PackageManifest']/*[local-name()='Metadata']/*[local-name()='Identity']")
        if ($null -eq $identity -or $identity.GetAttribute('Id') -ne $ExtensionId) { throw 'VSIX identity is not SQL Safety Guard.' }
    }
    finally { $reader.Dispose() }
}

function Install-ManualCopy([string] $VsixPath, [string] $ExtensionsRoot) {
    $root = [IO.Path]::GetFullPath($ExtensionsRoot)
    $target = Join-Path $root $ExtensionFolderName
    $stage = Join-Path $root ('.SqlSafetyGuard-stage-' + [guid]::NewGuid().ToString('N'))
    $backup = Join-Path $root ('.SqlSafetyGuard-backup-' + [guid]::NewGuid().ToString('N'))
    $movedPrevious = $false
    $installed = $false
    New-Item -ItemType Directory -Force -Path $root | Out-Null
    try {
        Expand-ValidatedVsix -VsixPath $VsixPath -Stage $stage
        if (Test-Path -LiteralPath $target) {
            if ((Get-Item -LiteralPath $target).Attributes -band [IO.FileAttributes]::ReparsePoint) { throw 'Extension target is a reparse point.' }
            Move-Item -LiteralPath $target -Destination $backup
            $movedPrevious = $true
        }
        Move-Item -LiteralPath $stage -Destination $target
        $installed = $true
        Update-ExtensionsConfigurationChanged -Path $root
    }
    catch {
        $failure = $_
        if ($installed) { Remove-ExtensionDirectory -Path $target -Root $root }
        if ($movedPrevious) { Move-Item -LiteralPath $backup -Destination $target }
        throw $failure
    }
    finally {
        Remove-ExtensionDirectory -Path $stage -Root $root
    }
    if ($movedPrevious) { Remove-ExtensionDirectory -Path $backup -Root $root }
    return $target
}

function Install-WithVsixInstaller([string] $VsixPath, $Ssms) {
    $installer = Join-Path $Ssms.InstallPath 'Common7\IDE\VSIXInstaller.exe'
    $log = Join-Path $env:TEMP 'SqlSafetyGuard_VSIXInstaller.log'
    $stage = Join-Path $env:TEMP ('.SqlSafetyGuard-validate-' + [guid]::NewGuid().ToString('N'))
    try { Expand-ValidatedVsix -VsixPath $VsixPath -Stage $stage }
    finally { Remove-ExtensionDirectory -Path $stage -Root $env:TEMP }
    $common = @('/quiet', "/instanceIds:$($Ssms.InstanceId)", "/logFile:`"$log`"")
    $removeArgs = $common + "/uninstall:$ExtensionId"
    $removal = Start-Process -FilePath $installer -ArgumentList $removeArgs -WindowStyle Hidden -Wait -PassThru
    if ($removal.ExitCode -notin 0, 2003) { throw "Uninstall failed ($($removal.ExitCode)); upgrade aborted. Existing installation was not replaced. Log: $log" }
    try {
        $process = Start-Process -FilePath $installer -ArgumentList ($common + "`"$VsixPath`"") -WindowStyle Hidden -Wait -PassThru
        if ($process.ExitCode -ne 0) { throw "VSIXInstaller exit code $($process.ExitCode)." }
    }
    catch {
        throw "Install failed after the uninstall step: $_ The previous version may have been removed. Keep SSMS closed. Recovery: run & `"$installer`" /instanceIds:$($Ssms.InstanceId) `"<path to a known-working previous SqlSafetyGuard.vsix>`" (or rerun install.ps1 -VsixPath with that file). Do not assume the guard is active. Log: $log"
    }
}
