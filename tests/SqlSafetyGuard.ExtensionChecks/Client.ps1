param(
    [Parameter(Mandatory=$true)][string]$ChannelDirectory,
    [Parameter(Mandatory=$true)][int]$SsmsProcessId,
    [string]$Database = 'SgExt_1028',
    [string]$Server = '.\SQLEXPRESS'
)
$ErrorActionPreference = 'Stop'
if ($Database -notmatch '^SgExt_[A-Za-z0-9_]+$') { throw 'Only explicitly owned SgExt_* databases are allowed.' }
$ownedProcess = Get-CimInstance Win32_Process -Filter "ProcessId=$SsmsProcessId"
if (!$ownedProcess -or $ownedProcess.Name -ne 'Ssms.exe' -or $ownedProcess.CommandLine -notmatch '/rootSuffix\s+SgExt') {
    throw 'Refusing to target anything except a separately launched SgExt experimental SSMS process.'
}
Add-Type -AssemblyName UIAutomationClient,UIAutomationTypes,System.Data
if (!('ExtensionCheckWindows' -as [type])) { Add-Type -Path "$PSScriptRoot\NativeWindows.cs" }
$script:ChannelDirectory = [IO.Path]::GetFullPath($ChannelDirectory)
$script:SsmsProcessId = $SsmsProcessId
$script:Database = $Database
$script:Server = $Server
$script:Document = 'SgExtA.sql'
$script:Utf8 = New-Object Text.UTF8Encoding($false)
function Read-Events {
    $path = Join-Path $script:ChannelDirectory 'events.tsv'
    if (Test-Path $path) { return @(Get-Content -LiteralPath $path) }
    return @()
}
function Send-Channel([string]$Verb, [string]$Argument = '', [switch]$NoWait) {
    $path = Join-Path $script:ChannelDirectory 'SqlSafetyGuard-pane-cmd.txt'
    if (Test-Path $path) { throw 'A previous channel request is still pending.' }
    $offset = @(Read-Events).Count
    $stage = "$path.stage"
    [IO.File]::WriteAllText($stage, "$script:Document|$Verb|$Argument", $script:Utf8)
    Move-Item -LiteralPath $stage -Destination $path
    if ($NoWait) { return $offset }
    Wait-Channel $Verb $offset
}
function Wait-Channel([string]$Verb, [int]$Offset, [int]$Timeout = 30) {
    $deadline = [DateTime]::UtcNow.AddSeconds($Timeout)
    do {
        $events = @(Read-Events | Select-Object -Skip $Offset)
        $failure = $events | Where-Object { $_ -match "\terror\t$Verb\t" }
        if ($failure) { throw ($failure -join "\n") }
        if ($events | Where-Object { $_ -match "\tack\t$Verb$" }) { return }
        Start-Sleep -Milliseconds 100
    } while ([DateTime]::UtcNow -lt $deadline)
    throw "Timed out waiting for channel $Verb (offset $Offset)."
}
function Set-Query([string]$Text) {
    $path = Join-Path $script:ChannelDirectory 'batch.sql'
    [IO.File]::WriteAllText($path, $Text, $script:Utf8)
    Send-Channel replacefile $path
}
function Get-OwnWindows {
    foreach ($handle in [ExtensionCheckWindows]::ForProcess($script:SsmsProcessId)) {
        try { [System.Windows.Automation.AutomationElement]::FromHandle($handle) } catch { }
    }
}
function Get-OwnElements {
    foreach ($window in Get-OwnWindows) {
        $window
        try { $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) } catch { }
    }
}
function Find-OwnElement([string]$Name, [string]$AutomationId = '') {
    foreach ($element in Get-OwnElements) {
        if ($element.Current.ProcessId -ne $script:SsmsProcessId) { continue }
        if (($Name -eq '' -or $element.Current.Name -eq $Name) -and
            ($AutomationId -eq '' -or $element.Current.AutomationId -eq $AutomationId)) { return $element }
    }
    return $null
}
function Invoke-OwnElement($Element) {
    if (!$Element -or $Element.Current.ProcessId -ne $script:SsmsProcessId) { throw 'Invalid UIA process target.' }
    $pattern = $Element.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
    $pattern.Invoke()
}
function Invoke-Sql([string]$Text, [switch]$Scalar) {
    $connection = New-Object System.Data.SqlClient.SqlConnection("Server=$script:Server;Database=$script:Database;Integrated Security=True;TrustServerCertificate=True;Application Name=SgExtEvidence")
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $Text
        if ($Scalar) { return $command.ExecuteScalar() }
        [void]$command.ExecuteNonQuery()
    } finally { $connection.Dispose() }
}
function Get-TextHash([string]$Text) {
    $sha = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($Text)))).Replace('-','').ToLowerInvariant() }
    finally { $sha.Dispose() }
}
