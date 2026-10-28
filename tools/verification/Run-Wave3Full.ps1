[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$Frozen,
    [string]$Output,
    [string]$Server = '.\SQLEXPRESS',
    [int]$Workers = 4,
    [int]$Limit = 0,
    [string]$Filter,
    [switch]$ValidateOnly
)
$ErrorActionPreference = 'Stop'
$Frozen = [IO.Path]::GetFullPath($Frozen)
$manifest = Get-Content -LiteralPath (Join-Path $Frozen 'manifest.json') -Raw | ConvertFrom-Json
foreach ($entry in $manifest.Files) {
    $actual = (Get-FileHash -LiteralPath (Join-Path $Frozen $entry.Path) -Algorithm SHA256).Hash
    if ($actual -ne $entry.Sha256) { throw "Frozen file changed: $($entry.Path)" }
}
if (-not $Output) { $Output = Join-Path (Split-Path (Split-Path $Frozen)) ('full-' + (Get-Date -Format 'yyyy-MM-dd')) }
$Output = [IO.Path]::GetFullPath($Output)
[IO.Directory]::CreateDirectory($Output) | Out-Null
$lease = [Threading.Mutex]::new($false, 'Local\SqlSafetyGuard.Wave3Full')
if (-not $lease.WaitOne(0)) { $lease.Dispose(); throw 'Another full launcher is active.' }
$harness = Join-Path $Frozen 'harness\SqlSafetyGuard.ServerHarness.dll'
$analyzer = Join-Path $Frozen 'analyzer\SqlSafetyGuard.Analyzer.dll'
$results = [Collections.Generic.List[object]]::new()
try {
    foreach ($lane in @('generated-host','host','offline','isolated')) {
        $cases = Join-Path $Frozen ('cases\' + $lane)
        if (-not (Test-Path -LiteralPath $cases)) { continue }
        & dotnet $harness --analyzer $analyzer --campaign $cases --campaign-validate --out (Join-Path $Output ('validation-' + $lane))
        if ($LASTEXITCODE -ne 0) { throw "No usable cases in $lane; inspect upfront validation." }
    }
    $features = Join-Path $Frozen 'cases\features.json'
    & dotnet $harness --analyzer $analyzer --isolated-coverage $features --validate-coverage --out (Join-Path $Output 'validation-features')
    if ($LASTEXITCODE -ne 0) { throw 'No usable feature fixtures; inspect upfront validation.' }
    if ($ValidateOnly) { return }
    foreach ($lane in @('offline','generated-host','host','isolated','features')) {
        $cases = Join-Path $Frozen ('cases\' + $lane)
        if ($lane -eq 'features') { $cases = $features }
        if (-not (Test-Path -LiteralPath $cases)) { continue }
        $destination = Join-Path $Output $lane
        $arguments = @($harness,'--analyzer',$analyzer,'--campaign',$cases,'--out',$destination,'--budget-minutes','0','--timeout','20','--shrink-attempts','0')
        if ($Filter) { $arguments += @('--filter',$Filter) }
        if ($Limit) { $arguments += @('--limit',"$Limit") }
        if ($lane -eq 'offline') {
            & dotnet @arguments --campaign-offline
        } elseif ($lane -eq 'generated-host') {
            & dotnet @arguments --server $Server --workers $Workers --campaign-principal admin
        } elseif ($lane -eq 'host') {
            & dotnet @arguments --server $Server --workers $Workers
        } else {
            $isolatedArguments = @('-NoProfile','-File',(Join-Path $Frozen 'Run-Isolated.ps1'),'-Cases',$cases,'-BudgetMinutes','0','-TimeoutSeconds','20','-Output',$destination,'-HarnessPath',$harness,'-AnalyzerPath',$analyzer,'-ShrinkAttempts','0')
            if ($Filter) { $isolatedArguments += @('-Filter',$Filter) }
            if ($Limit) { $isolatedArguments += @('-Limit',"$Limit") }
            if ($lane -eq 'features') { $isolatedArguments += '-CoverageFixtures' }
            $previousCompleted = -1
            do {
                & (Get-Process -Id $PID).Path @isolatedArguments
                $laneExitCode = $LASTEXITCODE
                $retry = $false
                if ($lane -eq 'isolated') {
                    $reportPath = Join-Path $destination 'report.md'
                    if (Test-Path -LiteralPath $reportPath) {
                        $report = Get-Content -LiteralPath $reportPath -Raw
                        if ($report -match 'Completed: (\d+)/(\d+);') {
                            $completed = [int]$Matches[1]
                            $planned = [int]$Matches[2]
                            $retry = $completed -lt $planned -and $completed -gt $previousCompleted
                            $previousCompleted = $completed
                        }
                    }
                }
                if ($retry) { Write-Warning 'Isolated lane stopped after durable progress; resuming unattempted cases in a fresh disposable server.' }
            } while ($retry)
            $global:LASTEXITCODE = $laneExitCode
        }
        $results.Add([pscustomobject]@{ Lane=$lane; ExitCode=$LASTEXITCODE; CompletedUtc=[DateTime]::UtcNow.ToString('o') })
        $results | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $Output 'launcher-results.json') -Encoding UTF8
    }
    if (@($results | Where-Object ExitCode -ne 0).Count) { throw 'Finished all lanes with findings; inspect launcher-results.json and lane reports.' }
}
finally { $lease.ReleaseMutex(); $lease.Dispose() }
