[CmdletBinding()]
param(
    [datetime]$RunAt = '2026-10-28T22:00:00',
    [string]$OutputRoot = 'C:\Users\nagya\dev\work_exim\ssmsextensions\harness-out\campaign',
    [string]$TaskName = 'SQL Safety Guard Wave2 2026-10-28'
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
Push-Location $root
try {
    $head = (& git rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0 -or $head -notmatch '^[0-9a-f]{40}$') { throw 'Cannot resolve branch head.' }
    if (& git status --porcelain) { throw 'Commit all source changes before freezing.' }
    if ($RunAt -le (Get-Date)) { throw 'Scheduled time must be in the future.' }
    if (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue) { throw 'Task already exists; inspect it before replacement.' }
    $harness = Join-Path $root 'tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0'
    $analyzer = Join-Path $root 'src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0'
    foreach ($binary in @((Join-Path $harness 'SqlSafetyGuard.ServerHarness.dll'), (Join-Path $analyzer 'SqlSafetyGuard.Analyzer.dll'))) {
        if (-not (Test-Path -LiteralPath $binary)) { throw "Build Release before freezing: $binary" }
    }
    $frozen = Join-Path $OutputRoot ('bin\' + $head)
    if (Test-Path -LiteralPath $frozen) { throw 'Frozen head folder already exists; inspect its manifest, never overwrite it.' }
    New-Item -ItemType Directory -Path $frozen | Out-Null
    Copy-Item -LiteralPath $harness -Destination (Join-Path $frozen 'harness') -Recurse
    Copy-Item -LiteralPath $analyzer -Destination (Join-Path $frozen 'analyzer') -Recurse
    Copy-Item -LiteralPath (Join-Path $root 'tests\SqlSafetyGuard.ServerHarness\Campaign\cases\generated\host') -Destination (Join-Path $frozen 'cases') -Recurse
    Copy-Item -LiteralPath (Join-Path $root 'tests\SqlSafetyGuard.ServerHarness\Campaign\Run-Overnight.ps1') -Destination $frozen
    $manifest = Get-ChildItem -LiteralPath $frozen -Recurse -File | Sort-Object FullName | ForEach-Object {
        [pscustomobject]@{ Path = $_.FullName.Substring($frozen.Length + 1); Sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    }
    [pscustomobject]@{ Head = $head; FrozenUtc = [DateTime]::UtcNow.ToString('o'); Files = @($manifest) } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $frozen 'manifest.json') -Encoding UTF8
    $output = Join-Path $OutputRoot ($RunAt.ToString('yyyy-MM-dd') + '\overnight-' + $head.Substring(0, 12))
    $arguments = '-NoProfile -ExecutionPolicy Bypass -File "' + (Join-Path $frozen 'Run-Overnight.ps1') + '" -HarnessPath "' + (Join-Path $frozen 'harness\SqlSafetyGuard.ServerHarness.dll') + '" -AnalyzerPath "' + (Join-Path $frozen 'analyzer\SqlSafetyGuard.Analyzer.dll') + '" -Cases "' + (Join-Path $frozen 'cases') + '" -Server ".\SQLEXPRESS" -Workers 4 -BudgetMinutes 480 -TimeoutSeconds 20 -Output "' + $output + '"'
    $action = New-ScheduledTaskAction -Execute "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" -Argument $arguments -WorkingDirectory $frozen
    $trigger = New-ScheduledTaskTrigger -Once -At $RunAt
    $principal = New-ScheduledTaskPrincipal -UserId ([Security.Principal.WindowsIdentity]::GetCurrent().Name) -LogonType Interactive -RunLevel Highest
    $settings = New-ScheduledTaskSettingsSet -MultipleInstances IgnoreNew -ExecutionTimeLimit (New-TimeSpan -Hours 9)
    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Description ('Wave2 head ' + $head + '; 8-hour owned SQL campaign; interactive logon required.') | Out-Null
    $task = Get-ScheduledTask -TaskName $TaskName
    $info = Get-ScheduledTaskInfo -TaskName $TaskName
    $receipt = [pscustomobject]@{ TaskName = $task.TaskName; State = [string]$task.State; NextRunTime = $info.NextRunTime.ToString('o'); Head = $head; Frozen = $frozen; Output = $output; Execute = $action.Execute; Arguments = $arguments }
    New-Item -ItemType Directory -Path (Split-Path $output) -Force | Out-Null
    $receipt | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path (Split-Path $output) 'wave2-task-registration.json') -Encoding UTF8
    Export-ScheduledTask -TaskName $TaskName | Set-Content -LiteralPath (Join-Path (Split-Path $output) 'wave2-task.xml') -Encoding UTF8
    $receipt | Format-List
}
finally { Pop-Location }
