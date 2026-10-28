[CmdletBinding()]
param([string]$OutputRoot)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
if (-not $OutputRoot) { $OutputRoot = [IO.Path]::GetFullPath((Join-Path $root '..\harness-out\campaign')) }
Push-Location $root
try {
    $head = (& git rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0 -or $head -notmatch '^[0-9a-f]{40}$') { throw 'Cannot resolve head.' }
    if (& git status --porcelain) { throw 'Commit source before freezing.' }
    $frozen = Join-Path $OutputRoot ('bin\' + $head)
    if (Test-Path -LiteralPath $frozen) { throw 'Frozen folder exists; never overwrite it.' }
    [IO.Directory]::CreateDirectory($frozen) | Out-Null
    Copy-Item -LiteralPath 'tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0' -Destination (Join-Path $frozen 'harness') -Recurse
    Copy-Item -LiteralPath 'src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0' -Destination (Join-Path $frozen 'analyzer') -Recurse
    foreach ($lane in @('generated-host','host','offline','isolated')) { [IO.Directory]::CreateDirectory((Join-Path $frozen ('cases\' + $lane))) | Out-Null }
    $campaign = Join-Path $root 'tests\SqlSafetyGuard.ServerHarness\Campaign'
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
    foreach ($script in @('Run-Isolated.ps1','Run-Overnight.ps1')) { Copy-Item -LiteralPath (Join-Path $campaign $script) -Destination $frozen }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Run-Wave3Full.ps1') -Destination $frozen
    $files = @(Get-ChildItem -LiteralPath $frozen -Recurse -File | Sort-Object FullName | ForEach-Object {
        [pscustomobject]@{ Path=$_.FullName.Substring($frozen.Length+1); Sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
    [pscustomobject]@{ Head=$head; FrozenUtc=[DateTime]::UtcNow.ToString('o'); Files=$files } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $frozen 'manifest.json') -Encoding UTF8
    Write-Output $frozen
}
finally { Pop-Location }
