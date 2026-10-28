[CmdletBinding()]
param(
    [string]$Cases,
    [string]$Server = '.\SQLEXPRESS',
    [ValidateRange(1,16)][int]$Workers = [Math]::Max(1,[Math]::Min(4,[Math]::Floor([Environment]::ProcessorCount / 2))),
    [ValidateRange(1,10080)][int]$BudgetMinutes = 480,
    [ValidateRange(1,3600)][int]$TimeoutSeconds = 10,
    [string]$Output,
    [switch]$PrepareOnly,
    [switch]$Isolated
)
$ErrorActionPreference = 'Stop'
if (-not $Cases) { $Cases = Join-Path $PSScriptRoot 'cases\smoke.jsonl' }
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
if (-not $Output) { $Output = Join-Path $root ('..\harness-out\campaign\' + (Get-Date -Format 'yyyy-MM-dd')) }
Push-Location $root
try {
    & dotnet build src\SqlSafetyGuard.Analyzer -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Analyzer build failed.' }
    & dotnet build tests\SqlSafetyGuard.ServerHarness -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Harness build failed.' }
    $arguments = @('tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0\SqlSafetyGuard.ServerHarness.dll',
        '--analyzer','src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0\SqlSafetyGuard.Analyzer.dll',
        '--campaign',$Cases,'--server',$Server,'--workers',"$Workers",'--budget-minutes',"$BudgetMinutes",
        '--timeout',"$TimeoutSeconds",'--out',$Output)
    if ($PrepareOnly) { $arguments += '--prepare-world' }
    if ($Isolated) { $arguments += '--campaign-isolated' }
    & dotnet @arguments
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) { Write-Warning "Campaign completed with findings/incomplete execution (exit $exitCode). See $Output\report.md" }
    exit $exitCode
}
finally { Pop-Location }
