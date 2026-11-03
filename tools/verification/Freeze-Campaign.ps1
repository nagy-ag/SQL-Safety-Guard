<# Builds a clean source checkout into an immutable external bundle. Does not execute SQL. #>
[CmdletBinding()]
param([string]$OutputRoot)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
if (-not $OutputRoot) { $OutputRoot = Join-Path $root '..\harness-out\campaign' }
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
if ($OutputRoot -eq $root -or $OutputRoot.StartsWith($root.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Frozen output must be outside the repository.'
}
$ancestor = $OutputRoot
while ($ancestor) {
    if ((Test-Path -LiteralPath $ancestor) -and ((Get-Item -LiteralPath $ancestor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw 'Frozen output cannot traverse a reparse point.'
    }
    $ancestor = [IO.Path]::GetDirectoryName($ancestor)
}
function Assert-NativeSuccess([string]$Step) {
    if ($LASTEXITCODE -ne 0) { throw "$Step failed (exit $LASTEXITCODE)." }
}
Push-Location $root
try {
    $head = (& git rev-parse HEAD).Trim()
    Assert-NativeSuccess 'Resolve source'
    $dirty = & git status --porcelain --untracked-files=all
    Assert-NativeSuccess 'Read source status'
    if ($dirty) { throw 'Freeze requires a clean checkout. Do not commit changes without owner instruction.' }
    $frozen = Join-Path $OutputRoot ('bin\' + $head)
    $bundleAncestor = $frozen
    while ($bundleAncestor) {
        if ((Test-Path -LiteralPath $bundleAncestor) -and ((Get-Item -LiteralPath $bundleAncestor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)) {
            throw 'Frozen bundle cannot traverse a reparse point.'
        }
        $bundleAncestor = [IO.Path]::GetDirectoryName($bundleAncestor)
    }
    if (Test-Path -LiteralPath $frozen) { throw 'Frozen folder exists; never overwrite it.' }
    [IO.Directory]::CreateDirectory($frozen) | Out-Null
    $sdk = (& dotnet --version).Trim()
    Assert-NativeSuccess 'Read SDK'
    $commands = @()
    foreach ($project in @(
        @{Path='tests/SqlSafetyGuard.ServerHarness/SqlSafetyGuard.ServerHarness.csproj'; Destination='harness'},
        @{Path='src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj'; Destination='analyzer'})) {
        $arguments = @('build', $project.Path, '--configuration', 'Release', '--no-incremental',
            '--output', (Join-Path $frozen $project.Destination), '/p:RestoreLockedMode=true', "/p:SourceRevisionId=$head")
        & dotnet @arguments | Set-Content -LiteralPath (Join-Path $frozen ($project.Destination + '-build.log')) -Encoding UTF8
        Assert-NativeSuccess ($project.Destination + ' build')
        $commands += ,$arguments
    }
    $campaign = Join-Path $root 'tests\SqlSafetyGuard.ServerHarness\Campaign'
    foreach ($lane in @('generated-host','host','offline','isolated')) {
        [IO.Directory]::CreateDirectory((Join-Path $frozen ('cases\' + $lane))) | Out-Null
    }
    foreach ($lane in @('host','offline')) {
        $destination = if ($lane -eq 'host') { 'generated-host' } else { $lane }
        Copy-Item -LiteralPath (Join-Path $campaign ('cases\generated\' + $lane)) -Destination (Join-Path $frozen ('cases\' + $destination + '\generated')) -Recurse
    }
    Copy-Item -LiteralPath (Join-Path $campaign 'cases\wave3\features.json') -Destination (Join-Path $frozen 'cases\features.json')
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'WaveClr.cs') -Destination $frozen
    foreach ($mapping in @(@('shared','host'),@('isolated','isolated'),@('offline','offline'))) {
        Copy-Item -LiteralPath (Join-Path $campaign ('cases\independent\' + $mapping[0])) -Destination (Join-Path $frozen ('cases\' + $mapping[1] + '\independent')) -Recurse
    }
    foreach ($lane in @('host','offline','isolated')) {
        $focused = Join-Path $campaign ('cases\wave3\' + $lane)
        if (Test-Path -LiteralPath $focused) { Copy-Item -LiteralPath $focused -Destination (Join-Path $frozen ('cases\' + $lane + '\wave3')) -Recurse }
    }
    foreach ($script in @('Run-Isolated.ps1','Run-Overnight.ps1')) {
        Copy-Item -LiteralPath (Join-Path $campaign $script) -Destination $frozen
    }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Run-Campaign.ps1') -Destination $frozen
    $dirty = & git status --porcelain --untracked-files=all
    Assert-NativeSuccess 'Recheck source status'
    $currentHead = (& git rev-parse HEAD).Trim()
    Assert-NativeSuccess 'Recheck source revision'
    if ($dirty -or $currentHead -ne $head) { throw 'Source changed during build; bundle is incomplete and has no manifest.' }
    $files = @(Get-ChildItem -LiteralPath $frozen -Recurse -File | Sort-Object FullName | ForEach-Object {
        [pscustomobject]@{ Path=$_.FullName.Substring($frozen.Length+1); Sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
    [pscustomobject]@{
        SchemaVersion=1; Head=$head; FrozenUtc=[DateTime]::UtcNow.ToString('o')
        Build=[pscustomobject]@{Sdk=$sdk; Configuration='Release'; Commands=$commands}
        Files=$files
    } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $frozen 'manifest.json') -Encoding UTF8
    Write-Output $frozen
}
finally { Pop-Location }
