param([Parameter(Mandatory=$true)][string]$ChannelDirectory,[Parameter(Mandatory=$true)][int]$SsmsProcessId,[Parameter(Mandatory=$true)][string]$Database,[string]$Server='.\SQLEXPRESS',[switch]$FunctionsOnly)
. "$PSScriptRoot\Client.ps1" -ChannelDirectory $ChannelDirectory -SsmsProcessId $SsmsProcessId -Database $Database -Server $Server
$script:Results = @()
$script:RunId = [DateTime]::UtcNow.ToString('HHmmss')
$nl = [Environment]::NewLine
function Get-Batches {
    [xml]$xml = Invoke-Sql "SELECT CAST(t.target_data AS nvarchar(max)) FROM sys.dm_xe_session_targets t JOIN sys.dm_xe_sessions s ON s.address=t.event_session_address WHERE s.name='$Database';" -Scalar
    if ($xml.RingBufferTarget.truncated -ne '0' -or $xml.RingBufferTarget.droppedCount -ne '0') { throw 'Incomplete Extended Events evidence.' }
    foreach ($event in $xml.RingBufferTarget.event) {
        $pidValue = ($event.action | Where-Object name -eq client_pid).value
        $app = ($event.action | Where-Object name -eq client_app_name).value.InnerText
        if ([int]$pidValue -eq $script:SsmsProcessId -and $app -eq "Microsoft SQL Server Management Studio - Query") {
            [pscustomobject]@{ Time=$event.timestamp; Text=($event.data | Where-Object name -eq batch_text).value.InnerText; Result=[int]($event.data | Where-Object name -eq result).value.InnerText }
        }
    }
}
function Get-Counters {
    return @(foreach ($tag in @('red','yellow','outside')) { [int](Invoke-Sql "SELECT Executions FROM dbo.Counter WHERE Tag='$tag';" -Scalar) })
}
function Test-Execution {
    param([string]$Name,[string]$Text,[string]$Verdict='ReadOnly',[string]$Decision='None',[int]$ExpectedCount=1,
          [string]$Command='Query.Execute',[string]$Selection='', [string]$ExecutedText='', [switch]$KeepText,
          [switch]$Toolbar,[switch]$NoGuard,[switch]$Stale,[string]$ServerText='',[int]$ExpectedWrites=-1,[switch]$SaveCancel,
          [string]$TargetText='', [string]$TargetDocument='', [switch]$Ambiguous)
    $marker = "SgExt:$($script:RunId):$Name"
    $textToSet = "-- $marker$nl$Text"
    if (!$KeepText -and !$Stale) { Set-Query $textToSet }
    if ($Selection) { Send-Channel select $Selection }
    $captureText = if ($ExecutedText) { $ExecutedText } else { $textToSet }
    if ($TargetText) { $captureText = $TargetText }
    if (!$TargetDocument) { $TargetDocument = $script:Document }
    if (!$Stale) { Start-Sleep -Milliseconds 700 }
    $before = @(Get-Batches)
    $countersBefore = @(Get-Counters)
    $offset = @(Read-Events).Count
    $errorText = ''
    $dialogSeen = $false
    $targetWarning = ''
    try {
        if ($Toolbar) {
            $button = Find-OwnElement 'Execute'
            if (!$button) { throw 'Execute toolbar is not exposed through this process UIA tree.' }
            Invoke-OwnElement $button
        } elseif ($Stale) {
            $batchPath = Join-Path $ChannelDirectory 'stale.sql'
            [IO.File]::WriteAllText($batchPath, $textToSet, $script:Utf8)
            $packet = "$script:Document|replacefile|$batchPath$nl$script:Document|state|$nl$script:Document|execute|$Command"
            [IO.File]::WriteAllText((Join-Path $ChannelDirectory 'SqlSafetyGuard-pane-cmd.txt.stage'), $packet, $script:Utf8)
            Move-Item -LiteralPath (Join-Path $ChannelDirectory 'SqlSafetyGuard-pane-cmd.txt.stage') -Destination (Join-Path $ChannelDirectory 'SqlSafetyGuard-pane-cmd.txt')
        } else { Send-Channel execute $Command -NoWait | Out-Null }
        $deadline = [DateTime]::UtcNow.AddSeconds(20)
        do {
            $events = @(Read-Events | Select-Object -Skip $offset)
            $failure = $events | Where-Object { $_ -match '\terror\texecute\t' }
            if ($failure) { throw ($failure -join ' ') }
            $dialog = Find-OwnElement '' RunButton
            if ($dialog) {
                $dialogSeen = $true
                $warningElement = Find-OwnElement '' TargetWarningText
                if ($warningElement) { $targetWarning = $warningElement.Current.Name }
                if ($Decision -eq 'Run') { Invoke-OwnElement $dialog }
                else { Invoke-OwnElement (Find-OwnElement '' CancelButton) }
            }
            if ($SaveCancel) {
                $saveWindow = Get-OwnWindows | Where-Object { $_.Current.Name -eq 'Save Results' } | Select-Object -First 1
                if ($saveWindow) {
                    $cancel = $saveWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Cancel')))
                    [ExtensionCheckWindows]::ClickButton([IntPtr]$cancel.Current.NativeWindowHandle,$script:SsmsProcessId)
                }
            }
            if ($events | Where-Object { $_ -match '\tack\texecute$' }) { break }
            Start-Sleep -Milliseconds 150
        } while ([DateTime]::UtcNow -lt $deadline)
        if (!$Toolbar -and !($events | Where-Object { $_ -match '\tack\texecute$' })) { throw 'No execute acknowledgement.' }
        Start-Sleep -Milliseconds 1200
    } catch { $errorText = $_.Exception.Message }
    $after = @(Get-Batches)
    $allNewBatches = @($after | Select-Object -Skip $before.Count)
    $newBatches = @($allNewBatches | Where-Object { $_.Text -notmatch "^SET (PARSEONLY|SHOWPLAN_XML|STATISTICS XML) " })
    $countersAfter = @(Get-Counters)
    $delta = @(for($index=0; $index -lt 3; $index++){ $countersAfter[$index]-$countersBefore[$index] })
    $guards = @(Read-Events | Select-Object -Skip $offset | Where-Object { $_ -match '\tguard\t' })
    $guard = if ($guards.Count) { $guards[-1].Split([char]9) } else { @() }
    $hash = Get-TextHash $captureText
    $hashMatches = $NoGuard -or ($guard.Count -gt 4 -and $guard[3] -eq $hash)
    $verdictMatches = $NoGuard -or ($guard.Count -gt 4 -and $guard[4] -eq $Verdict)
    $serverExpected = if($ServerText){ "-- $marker$nl$ServerText" }else{ $captureText }
    $textMatches = $newBatches.Count -eq 0 -or @($newBatches | Where-Object { $_.Text.Replace([string][char]13,'').Trim() -ne $serverExpected.Replace([string][char]13,'').Trim() }).Count -eq 0
    $executionCount = if ($Command -in @('Query.Parse','Query.DisplayEstimatedExecutionPlan')) { ($delta | Measure-Object -Sum).Sum } else { @($newBatches | Where-Object Result -eq 0).Count }
    $writeCount = [int](($delta | Measure-Object -Sum).Sum)
    if ($ExpectedWrites -lt 0) { $ExpectedWrites = if ($Text -match 'UPDATE|EXEC dbo.Tick') { $ExpectedCount } else { 0 } }
    $guardMatches = if ($NoGuard) { $guards.Count -eq 0 } else { $guards.Count -eq 1 -and [IO.Path]::GetFileName($guard[2]) -eq $TargetDocument }
    $targetEvents = @(Read-Events | Select-Object -Skip $offset | Where-Object { $_ -match '\ttarget\t' })
    $targetEvent = if ($targetEvents.Count) { $targetEvents[-1].Split([char]9) } else { @() }
    $contextMatches = $NoGuard -or ($targetEvent.Count -gt 7 -and $targetEvent[7] -eq $Database -and $targetEvent[6] -eq $Server)
    $ambiguityMatches = $NoGuard -or $(if ($Ambiguous) { $targetWarning -like 'The guard could not verify which window will run.*' -and $targetEvent[4] -ne 'verified' } else { $targetWarning -eq '' -and $targetEvent.Count -gt 4 -and $targetEvent[4] -eq 'verified' })
    $pass = !$errorText -and $hashMatches -and $verdictMatches -and $textMatches -and $guardMatches -and $contextMatches -and $ambiguityMatches -and ($writeCount -eq $ExpectedWrites) -and ($executionCount -eq $ExpectedCount) -and ($dialogSeen -eq ($Decision -ne 'None'))
    $result = [pscustomobject]@{ Scenario=$Name; Entry=if($Toolbar){'Toolbar Invoke'}else{$Command}; ExpectedVerdict=$Verdict;
        Decision=$Decision; ExpectedCount=$ExpectedCount; ObservedCount=$executionCount; BatchSubmissionCount=$newBatches.Count; CounterDelta=$delta;
        Dialog=$dialogSeen; GuardCount=$guards.Count; GuardVerdict=if($guard.Count){$guard[4]}else{''};
        ExpectedHash=$hash; GuardHash=if($guard.Count){$guard[3]}else{''}; HashMatches=$hashMatches; TextMatches=$textMatches;
        TargetDocument=$TargetDocument; GuardDocument=if($guard.Count){$guard[2]}else{''}; TargetWarning=$targetWarning; ContextMatches=$contextMatches; AmbiguityMatches=$ambiguityMatches;
        Status=if($pass){'PASS'}elseif($errorText){'BLOCKED'}else{'FAIL'}; Error=$errorText; Batches=$newBatches }
    $script:Results += $result
    $script:Results | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $ChannelDirectory "results-$script:RunId.json") -Encoding UTF8
    $result | Select-Object Scenario,ObservedCount,Dialog,GuardVerdict,Status,Error | Format-Table -AutoSize | Out-Host
}
if ($FunctionsOnly) { return }
Send-Channel document SgExtA.sql
Send-Channel mode 2
$emptyRules = Join-Path $ChannelDirectory 'rules.txt'
[IO.File]::WriteAllText($emptyRules, '', $script:Utf8)
Send-Channel rulesfile $emptyRules
$green = "SELECT N'green';"
$red = "UPDATE dbo.Counter SET Executions=Executions+1 WHERE Tag='red';"
$yellow = 'EXEC dbo.Tick;'
Test-Execution green $green
Test-Execution red-cancel $red RealWrite Cancel 0
Test-Execution red-run $red RealWrite Run
Test-Execution yellow-cancel $yellow Unknown Cancel 0
Test-Execution yellow-run $yellow Unknown Run
Test-Execution select-green "$green$nl$red" -Selection '2 2' -ExecutedText $green -ExpectedWrites 0
Test-Execution select-red "$red$nl$green" RealWrite Run -Selection '2 2' -ExecutedText $red
Test-Execution select-red-cancel "$red$nl$green" RealWrite Cancel 0 -Selection '2 2' -ExecutedText $red
Send-Channel mode 0
Test-Execution off-red $red -NoGuard
Send-Channel mode 1
Test-Execution redonly-yellow $yellow Unknown
Test-Execution redonly-red $red RealWrite Cancel 0
Send-Channel mode 2
$rules = "* | $Database | SgExtEnvironment | Off | false | |"
[IO.File]::WriteAllText($emptyRules, $rules, $script:Utf8)
Send-Channel rulesfile $emptyRules
Test-Execution rule-off $red -NoGuard
[IO.File]::WriteAllText($emptyRules, '', $script:Utf8)
Send-Channel rulesfile $emptyRules
$large = ((1..5100 | ForEach-Object {'-- padding'}) -join $nl) + "$nl$red"
Test-Execution large-red $large RealWrite Cancel 0
Test-Execution large-red-run $large RealWrite Run
Set-Query $green
Start-Sleep 1
Send-Channel state
Test-Execution stale-green-to-red $red RealWrite Cancel 0 -Stale
Test-Execution toolbar-red $red RealWrite Cancel 0 -Toolbar
Test-Execution parse-red $red -ExpectedCount 0 -Command Query.Parse -NoGuard
Test-Execution estimated-red $red -ExpectedCount 0 -Command Query.DisplayEstimatedExecutionPlan -NoGuard
Send-Channel execute Query.IncludeActualExecutionPlan
Test-Execution actual-red-cancel $red RealWrite Cancel 0
Test-Execution actual-red-run $red RealWrite Run
Send-Channel execute Query.IncludeActualExecutionPlan
Send-Channel execute Query.ResultstoText
Test-Execution text-red-cancel $red RealWrite Cancel 0
Test-Execution text-red-run $red RealWrite Run
Send-Channel execute Query.ResultstoGrid
Send-Channel execute Query.IncludeLiveQueryStatistics
Test-Execution livestats-red-cancel $red RealWrite Cancel 0
Test-Execution livestats-red-run $red RealWrite Run
Send-Channel execute Query.IncludeLiveQueryStatistics
$script:Document = 'SgExtB.sql'
Send-Channel document SgExtB.sql
Set-Query $red
$script:Document = 'SgExtA.sql'
Send-Channel document SgExtA.sql
Test-Execution active-A-green $green
$script:Document = 'SgExtB.sql'
Send-Channel document SgExtB.sql
$targetGreen = "-- SgExt:$($script:RunId):active-A-green$nl$green"
Test-Execution active-B-red-cancel $red ReadOnly Cancel 0 -TargetText $targetGreen -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 0
Test-Execution active-B-red-run $red ReadOnly Run -TargetText $targetGreen -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 0
$script:Document = 'SgExtA.sql'
Send-Channel document SgExtA.sql
$targetRed = "UPDATE dbo.Counter SET Executions=Executions+1 WHERE Tag='outside';"
Set-Query $targetRed
$script:Document = 'SgExtB.sql'
Send-Channel document SgExtB.sql
Test-Execution mismatch-green-B-red-A $green RealWrite Cancel 0 -TargetText $targetRed -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 0
Test-Execution mismatch-green-B-red-A-run $green RealWrite Run -TargetText $targetRed -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 1
Send-Channel mode 0
Test-Execution mismatch-off $green RealWrite Cancel 0 -TargetText $targetRed -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 0
Send-Channel mode 1
$script:Document = 'SgExtA.sql'
Set-Query $green
$script:Document = 'SgExtB.sql'
Test-Execution mismatch-redonly-green $red ReadOnly Cancel 0 -TargetText $green -TargetDocument SgExtA.sql -Ambiguous -ExpectedWrites 0
Send-Channel mode 2
$script:Document = 'SgExtA.sql'
Send-Channel document SgExtA.sql
Send-Channel execute Query.SQLCMDMode
$sqlcmd = ":setvar Target dbo.Counter$nl" + 'UPDATE $(Target) SET Executions=Executions+1 WHERE Tag=''red'';'
Test-Execution sqlcmd-on $sqlcmd RealWrite Run -ServerText $red
Send-Channel execute Query.SQLCMDMode
Test-Execution sqlcmd-off-cancel $sqlcmd Unknown Cancel 0
Test-Execution sqlcmd-off-run $sqlcmd Unknown Run -ExpectedWrites 0
Send-Channel execute Query.ResultstoFile
Test-Execution file-red-cancel $red RealWrite Cancel 0
Test-Execution file-run-savecancel $red RealWrite Run 0 -SaveCancel
Send-Channel execute Query.ResultstoGrid
$script:Results | Group-Object Status | Select-Object Name,Count | Format-Table
