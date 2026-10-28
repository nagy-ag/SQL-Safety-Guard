[CmdletBinding()]
param(
    [string]$Cases,
    [string]$Server = '.\SQLEXPRESS',
    [ValidateRange(1,16)][int]$Workers = [Math]::Max(1,[Math]::Min(4,[Math]::Floor([Environment]::ProcessorCount / 2))),
    [ValidateRange(0,10080)][int]$BudgetMinutes = 480,
    [ValidateRange(1,3600)][int]$TimeoutSeconds = 10,
    [string]$Output,
    [switch]$PrepareOnly,
    [switch]$Isolated,
    [string]$HarnessPath,
    [string]$AnalyzerPath,
    [string]$Filter,
    [ValidateRange(0,2147483647)][int]$Limit = 0,
    [ValidateRange(0,1000)][int]$ShrinkAttempts = 24,
    [ValidateSet('admin')][string]$Principal
)
$ErrorActionPreference = 'Stop'
if (-not $Cases) { $Cases = Join-Path $PSScriptRoot 'cases\smoke.jsonl' }
$root = if ($HarnessPath -and $AnalyzerPath) { $PSScriptRoot } else { [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..')) }
if (-not $Output) { $Output = Join-Path $root ('..\harness-out\campaign\' + (Get-Date -Format 'yyyy-MM-dd')) }
Push-Location $root
try {
    if ([bool]$HarnessPath -ne [bool]$AnalyzerPath) { throw 'Supply both frozen binary paths or neither.' }
    if (-not $HarnessPath) {
        & dotnet build src\SqlSafetyGuard.Analyzer -c Release --nologo
        if ($LASTEXITCODE -ne 0) { throw 'Analyzer build failed.' }
        & dotnet build tests\SqlSafetyGuard.ServerHarness -c Release --nologo
        if ($LASTEXITCODE -ne 0) { throw 'Harness build failed.' }
        $HarnessPath = 'tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0\SqlSafetyGuard.ServerHarness.dll'
        $AnalyzerPath = 'src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0\SqlSafetyGuard.Analyzer.dll'
    }
    if (-not (Test-Path -LiteralPath $HarnessPath) -or -not (Test-Path -LiteralPath $AnalyzerPath)) { throw 'Frozen binaries are missing.' }
    $arguments = @($HarnessPath,
        '--analyzer',$AnalyzerPath,
        '--campaign',$Cases,'--server',$Server,'--workers',"$Workers",'--budget-minutes',"$BudgetMinutes",
        '--timeout',"$TimeoutSeconds",'--out',$Output)
    if ($PrepareOnly) { $arguments += '--prepare-world' }
    if ($Isolated) { $arguments += '--campaign-isolated' }
    if ($Filter) { $arguments += @('--filter',$Filter) }
    if ($Limit) { $arguments += @('--limit',"$Limit") }
    $arguments += @('--shrink-attempts',"$ShrinkAttempts")
    if ($Principal) { $arguments += @('--campaign-principal',$Principal) }
    & dotnet @arguments
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) { Write-Warning "Campaign completed with findings/incomplete execution (exit $exitCode). See $Output\report.md" }
    exit $exitCode
}
finally { Pop-Location }
