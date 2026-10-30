<#
.SYNOPSIS
    Tests, builds and validates unsigned release files. Never publishes or signs anything.
#>
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [string] $MSBuildPath,
    [switch] $SkipUiChecks
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))

function Assert-NativeSuccess([string] $Step) {
    if ($LASTEXITCODE -ne 0) { throw "$Step failed (exit $LASTEXITCODE)." }
}

Push-Location $repository
try {
    $dirty = & git status --porcelain --untracked-files=all
    Assert-NativeSuccess 'Git status'
    if ($dirty) { throw 'The working tree is dirty. Commit or remove changes before making release assets.' }
    [xml] $sourceManifest = Get-Content -Raw -LiteralPath 'src\SqlSafetyGuard.Vsix\source.extension.vsixmanifest'
    $identity = $sourceManifest.PackageManifest.Metadata.Identity
    $version = [string] $identity.Version
    if ($version -notmatch '^\d+\.\d+\.\d+$') { throw "Invalid release version: $version" }
    if (-not $OutputDirectory) { $OutputDirectory = Join-Path $repository "..\release-out\$version" }
    $OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
    $repoPrefix = $repository.TrimEnd('\', '/') + '\'
    if ($OutputDirectory -eq $repository -or $OutputDirectory.StartsWith($repoPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Release output must be outside the repository.'
    }
    # Reject junction/symlink ancestors too: the textual path must not conceal an in-repo destination.
    $ancestor = $OutputDirectory
    while ($ancestor) {
        if (Test-Path -LiteralPath $ancestor) {
            if ((Get-Item -LiteralPath $ancestor).Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Release output cannot traverse a reparse point: $ancestor"
            }
        }
        $ancestor = [IO.Path]::GetDirectoryName($ancestor)
    }
    if ((Test-Path -LiteralPath $OutputDirectory) -and @(Get-ChildItem -LiteralPath $OutputDirectory -Force).Count) {
        throw 'Release output directory must be empty; existing files will not be replaced.'
    }
    if (-not $MSBuildPath) {
        $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
        if (-not (Test-Path -LiteralPath $vswhere)) { throw 'vswhere was not found. Supply -MSBuildPath.' }
        $MSBuildPath = & $vswhere -latest -prerelease -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
            Select-Object -First 1
        Assert-NativeSuccess 'MSBuild discovery'
    }
    if (-not $MSBuildPath -or -not (Test-Path -LiteralPath $MSBuildPath -PathType Leaf)) { throw 'Visual Studio MSBuild was not found.' }

    & dotnet test tests\SqlSafetyGuard.Tests
    Assert-NativeSuccess 'Unit tests'
    & $MSBuildPath SqlSafetyGuard.sln /restore /t:Rebuild /p:Configuration=Release /nologo /verbosity:minimal
    Assert-NativeSuccess 'Release build'
    if (-not $SkipUiChecks) {
        foreach ($scriptName in @('PaneInteractionTests.ps1', 'ConfirmationInteractionTests.ps1', 'SettingsInteractionTests.ps1')) {
            & powershell.exe -NoProfile -Sta -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot $scriptName) -Configuration Release
            Assert-NativeSuccess $scriptName
        }
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    Add-Type -AssemblyName System.IO.Compression
    if (-not ('ReleaseAssetBytes' -as [type])) {
        Add-Type -TypeDefinition @'
using System;
public static class ReleaseAssetBytes {
    public static uint Crc32(byte[] bytes) {
        uint crc = 0xffffffff;
        foreach (byte b in bytes) {
            crc ^= b;
            for (int i = 0; i < 8; i++) crc = (crc >> 1) ^ ((crc & 1) == 0 ? 0u : 0xedb88320u);
        }
        return ~crc;
    }
    public static bool Contains(byte[] bytes, byte[] pattern) {
        for (int i = 0; i <= bytes.Length - pattern.Length; i++) {
            int j = 0;
            while (j < pattern.Length && bytes[i+j] == pattern[j]) j++;
            if (j == pattern.Length) return true;
        }
        return false;
    }
}
'@
    }
    $vsix = Join-Path $repository 'src\SqlSafetyGuard.Vsix\bin\Release\SqlSafetyGuard.vsix'
    # Read central-directory CRCs independently; ZipArchive decompression alone does not validate CRC32.
    $raw = [IO.File]::ReadAllBytes($vsix)
    $end = $raw.Length - 22
    while ($end -ge [Math]::Max(0, $raw.Length - 65557) -and [BitConverter]::ToUInt32($raw, $end) -ne 0x06054b50) { $end-- }
    if ($end -lt 0 -or [BitConverter]::ToUInt32($raw, $end) -ne 0x06054b50) { throw 'ZIP end record is missing.' }
    if ([BitConverter]::ToUInt16($raw, $end + 4) -ne 0 -or [BitConverter]::ToUInt16($raw, $end + 6) -ne 0) { throw 'Multi-disk ZIP is unsupported.' }
    $count = [BitConverter]::ToUInt16($raw, $end + 10)
    $offset = [long][BitConverter]::ToUInt32($raw, $end + 16)
    $crcs = @{}
    for ($i = 0; $i -lt $count; $i++) {
        if ([BitConverter]::ToUInt32($raw, $offset) -ne 0x02014b50) { throw 'Invalid ZIP central directory.' }
        $nameLength = [BitConverter]::ToUInt16($raw, $offset + 28)
        $extraLength = [BitConverter]::ToUInt16($raw, $offset + 30)
        $commentLength = [BitConverter]::ToUInt16($raw, $offset + 32)
        $name = [Text.Encoding]::UTF8.GetString($raw, $offset + 46, $nameLength)
        if ($crcs.ContainsKey($name)) { throw "Duplicate ZIP entry: $name" }
        $crcs[$name] = [BitConverter]::ToUInt32($raw, $offset + 16)
        $offset += 46 + $nameLength + $extraLength + $commentLength
    }
    if ([string]::IsNullOrWhiteSpace($env:USERPROFILE)) { throw 'USERPROFILE is required for the path privacy check.' }
    $profilePaths = @($env:USERPROFILE, $env:USERPROFILE.Replace('\', '/'), $env:USERPROFILE.Replace('\', '\\'))
    $archive = [IO.Compression.ZipFile]::OpenRead($vsix)
    try {
        if ($archive.Entries.Count -ne $count) { throw 'ZIP entry counts disagree.' }
        foreach ($entry in $archive.Entries) {
            $name = $entry.FullName.Replace('\', '/')
            if ($name -match '(^|/)_xmlsignatures(/|$)' -or $name -match '\.(pfx|p12|key)$') { throw "Forbidden release entry: $name" }
            if ($name -match '(^/|:|(^|/)\.\.?(/|$))') { throw "Unsafe ZIP path: $name" }
            $stream = $entry.Open()
            $memory = New-Object IO.MemoryStream
            try { $stream.CopyTo($memory); $bytes = $memory.ToArray() }
            finally { $stream.Dispose(); $memory.Dispose() }
            if ($bytes.LongLength -ne $entry.Length -or [ReleaseAssetBytes]::Crc32($bytes) -ne $crcs[$entry.FullName]) {
                throw "ZIP integrity check failed: $name"
            }
            foreach ($profilePath in $profilePaths) {
                # Case-insensitive string checks supplement exact byte searches (including odd UTF-16 offsets).
                foreach ($encoding in @([Text.Encoding]::UTF8, [Text.Encoding]::Unicode)) {
                    if ([ReleaseAssetBytes]::Contains($bytes, $encoding.GetBytes($profilePath)) -or
                        $encoding.GetString($bytes).IndexOf($profilePath, [StringComparison]::OrdinalIgnoreCase) -ge 0 -or
                        $name.IndexOf($profilePath, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
                        throw "Local user profile path found in release entry: $name"
                    }
                }
            }
        }
        foreach ($required in @('extension.vsixmanifest', 'SqlSafetyGuard.dll', 'SqlSafetyGuard.pdb', 'SqlSafetyGuard.pkgdef',
            'SqlSafetyGuard.Analyzer.dll', 'SqlSafetyGuard.Analyzer.pdb', 'Microsoft.SqlServer.TransactSql.ScriptDom.dll',
            'Options/registration.json', 'LICENSE', 'THIRD-PARTY-NOTICES.md')) {
            if (-not $archive.GetEntry($required)) { throw "Missing VSIX file: $required" }
        }
        $reader = New-Object IO.StreamReader($archive.GetEntry('extension.vsixmanifest').Open())
        try { [xml] $manifest = $reader.ReadToEnd() } finally { $reader.Dispose() }
        if ($manifest.PackageManifest.Metadata.Identity.Id -ne 'SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558' -or
            $manifest.PackageManifest.Metadata.Identity.Version -ne $version) { throw 'VSIX identity/version mismatch.' }
        $targets = @($manifest.PackageManifest.Installation.InstallationTarget)
        if ($targets.Count -ne 1 -or $targets[0].Id -ne 'Microsoft.VisualStudio.Ssms' -or
            $targets[0].Version -ne '[22.0,)' -or $targets[0].ProductArchitecture -ne 'amd64') { throw 'Unexpected SSMS installation target.' }
        if ($manifest.SelectSingleNode('//*[local-name()="License"]')) { throw 'The installer must not require license acceptance.' }
    }
    finally { $archive.Dispose() }

    # Recheck after tests/build before writing the distribution files.
    $dirty = & git status --porcelain --untracked-files=all
    Assert-NativeSuccess 'Final git status'
    if ($dirty) { throw 'The working tree changed during validation.' }
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
    $vsixName = "SqlSafetyGuard-$version.vsix"
    $scriptsName = "SqlSafetyGuard-$version-scripts.zip"
    Copy-Item -LiteralPath $vsix -Destination (Join-Path $OutputDirectory $vsixName)
    $scriptsZip = [IO.Compression.ZipFile]::Open((Join-Path $OutputDirectory $scriptsName), [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($file in @('install.ps1', 'uninstall.ps1', 'scripts/SsmsCommon.ps1')) {
            [void][IO.Compression.ZipFileExtensions]::CreateEntryFromFile($scriptsZip, (Join-Path $repository $file), $file)
        }
    }
    finally { $scriptsZip.Dispose() }
    $sums = foreach ($name in @($vsixName, $scriptsName)) {
        $hash = (Get-FileHash -LiteralPath (Join-Path $OutputDirectory $name) -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $name"
    }
    [IO.File]::WriteAllLines((Join-Path $OutputDirectory 'SHA256SUMS.txt'), [string[]] $sums, [Text.Encoding]::ASCII)
    Write-Host "Validated unsigned release assets: $OutputDirectory"
}
finally { Pop-Location }
