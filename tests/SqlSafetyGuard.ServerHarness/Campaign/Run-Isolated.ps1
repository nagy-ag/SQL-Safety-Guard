[CmdletBinding()]
param(
    [string]$Cases,
    [string]$Image = 'mcr.microsoft.com/mssql/server:2025-latest',
    [int]$BudgetMinutes = 60,
    [ValidateRange(1,3600)][int]$TimeoutSeconds = 10,
    [string]$Output,
    [switch]$CoverageFixtures,
    [string]$HarnessPath,
    [string]$AnalyzerPath,
    [string]$Filter,
    [int]$Limit = 0,
    [int]$ShrinkAttempts = 0
)
$ErrorActionPreference = 'Stop'
if (-not $Cases) { $Cases = Join-Path $PSScriptRoot 'cases\isolated.jsonl' }
$Cases = [IO.Path]::GetFullPath($Cases)
if ($HarnessPath) { $HarnessPath = [IO.Path]::GetFullPath($HarnessPath) }
if ($AnalyzerPath) { $AnalyzerPath = [IO.Path]::GetFullPath($AnalyzerPath) }
if ($Output) { $Output = [IO.Path]::GetFullPath($Output) }
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$local = Join-Path ([IO.Path]::GetTempPath()) 'SqlSafetyGuard-Campaign'
[IO.Directory]::CreateDirectory($local) | Out-Null
$name = 'sqlguard-cov-' + [Guid]::NewGuid().ToString('N').Substring(0,16)
$envFile = Join-Path $local ($name + '.env')
$password = [Guid]::NewGuid().ToString('N') + 'Aa1!'
[IO.File]::WriteAllText($envFile, (@('ACCEPT_EULA=Y','MSSQL_PID=Developer','MSSQL_AGENT_ENABLED=true','MSSQL_ENABLE_HADR=1',"MSSQL_SA_PASSWORD=$password") -join [Environment]::NewLine) + [Environment]::NewLine)
$previousConnection = $env:SQLGUARD_COVERAGE_CONNECTION
$previousContainer = $env:SQLGUARD_COVERAGE_CONTAINER
try {
    & docker run --detach --name $name --hostname $name --label sqlguard.coverage=disposable --publish '127.0.0.1::1433' --env-file $envFile $Image | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Container creation failed.' }
    $ready = $false
    for ($attempt=0; $attempt -lt 120; $attempt++) {
        $ErrorActionPreference = 'SilentlyContinue'
        & docker exec $name sh -c 'export SQLCMDPASSWORD=$MSSQL_SA_PASSWORD; /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -b -l 1 -t 2 -Q ''SELECT 1''' *> $null
        $probeExit = $LASTEXITCODE
        $ErrorActionPreference = 'Stop'
        if ($probeExit -eq 0) { $ready=$true; break }
        Start-Sleep -Seconds 2
    }
    if (-not $ready) { throw 'SQL container did not become ready.' }
    & docker exec $name sh -c 'printf ''1\n2\n'' > /var/opt/mssql/data/sqlguard-coverage-bulk.csv'
    if ($LASTEXITCODE -ne 0) { throw 'Owned bulk-input preparation failed.' }
    & docker exec $name sh -c 'printf ''1,10\n2,20\n'' > /var/opt/mssql/data/sqlguard-independent.csv'
    if ($LASTEXITCODE -ne 0) { throw 'Independent bulk-input preparation failed.' }
    $binding = (& docker port $name 1433/tcp).Trim()
    if ($binding -notmatch '^127\.0\.0\.1:(\d+)$') { throw 'Unexpected SQL port binding.' }
    $env:SQLGUARD_COVERAGE_CONTAINER = $name
    $env:SQLGUARD_COVERAGE_CONNECTION = "Server=127.0.0.1,$($Matches[1]);Database=master;User ID=sa;Password=$password;TrustServerCertificate=true;Pooling=false"
    if (-not $Output) { $Output=Join-Path $root ('..\harness-out\campaign\' + (Get-Date -Format 'yyyy-MM-dd') + '\isolated-' + $name) }
    $shell = (Get-Process -Id $PID).Path
    if ($CoverageFixtures) {
        if (-not $HarnessPath) {
            & dotnet build tests\SqlSafetyGuard.ServerHarness -c Release --nologo
            if ($LASTEXITCODE -ne 0) { throw 'Harness build failed.' }
            $HarnessPath = 'tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0\SqlSafetyGuard.ServerHarness.dll'
            $AnalyzerPath = 'src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0\SqlSafetyGuard.Analyzer.dll'
        }
        $arguments = @($HarnessPath,'--analyzer',$AnalyzerPath,'--isolated-coverage',$Cases,'--timeout',"$TimeoutSeconds",'--out',$Output)
        if ($Filter) { $arguments += @('--filter',$Filter) }
        if ($Limit) { $arguments += @('--limit',"$Limit") }
        & dotnet @arguments
    } else {
        $arguments = @('-NoProfile','-File',(Join-Path $PSScriptRoot 'Run-Overnight.ps1'),'-Cases',$Cases,'-Workers','1','-BudgetMinutes',"$BudgetMinutes",'-TimeoutSeconds',"$TimeoutSeconds",'-Output',$Output,'-Isolated','-ShrinkAttempts',"$ShrinkAttempts")
        if ($HarnessPath) { $arguments += @('-HarnessPath',$HarnessPath,'-AnalyzerPath',$AnalyzerPath) }
        if ($Filter) { $arguments += @('-Filter',$Filter) }
        if ($Limit) { $arguments += @('-Limit',"$Limit") }
        & $shell @arguments
    }
    $exitCode = $LASTEXITCODE
}
finally {
    try {
        $ErrorActionPreference = 'SilentlyContinue'
        $inspectionJson = & docker inspect $name 2>$null
        $inspectionExit = $LASTEXITCODE
        $ErrorActionPreference = 'Stop'
        if ($inspectionExit -eq 0) {
            $inspection = $inspectionJson | ConvertFrom-Json
            if ($inspection[0].Config.Labels.'sqlguard.coverage' -ne 'disposable' -or $inspection[0].Mounts.Count -ne 0) { throw 'Refusing cleanup of an unowned container.' }
            & docker rm --force $name | Out-Null
            if ($LASTEXITCODE -ne 0) { throw "Owned container cleanup failed: $name" }
        }
    }
    finally {
        $env:SQLGUARD_COVERAGE_CONNECTION=$previousConnection
        $env:SQLGUARD_COVERAGE_CONTAINER=$previousContainer
        if (Test-Path -LiteralPath $envFile) { Remove-Item -LiteralPath $envFile }
        $password=$null
    }
}
exit $exitCode
