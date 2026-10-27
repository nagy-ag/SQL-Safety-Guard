param([Parameter(Mandatory = $true)][string] $FixtureRoot)

$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$FixtureRoot = [IO.Path]::GetFullPath($FixtureRoot)
if (-not $FixtureRoot.StartsWith($repository + '\', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Fixtures must stay inside the worktree.'
}
. (Join-Path $PSScriptRoot 'SsmsCommon.ps1')
Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.IO.Compression
New-Item -ItemType Directory -Path $FixtureRoot | Out-Null
$env:TEMP = Join-Path $FixtureRoot 'temp with spaces'
New-Item -ItemType Directory -Path $env:TEMP | Out-Null
$extensions = Join-Path $FixtureRoot 'Extensions'
$target = Join-Path $extensions 'SqlSafetyGuard'
New-Item -ItemType Directory -Path $target | Out-Null
[IO.File]::WriteAllText((Join-Path $target 'previous.txt'), 'previous version')
$manifest = '<PackageManifest><Metadata><Identity Id="' + $ExtensionId + '" /></Metadata></PackageManifest>'
$script:caseCount = 0

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
    $script:caseCount++
}

function Assert-Throws([scriptblock] $Action, [string] $Message) {
    $caught = $null
    try { & $Action | Out-Null }
    catch { $caught = $_ }
    Assert-True ($null -ne $caught) $Message
    return $caught.ToString()
}

function New-FixtureVsix([string] $Name, [string[]] $Extra = @(), [string] $Identity = $manifest, [switch] $MissingDll) {
    $path = Join-Path $FixtureRoot ($Name + '.vsix')
    $archive = [IO.Compression.ZipFile]::Open($path, [IO.Compression.ZipArchiveMode]::Create)
    try {
        $files = @{ 'extension.vsixmanifest' = $Identity; 'SqlSafetyGuard.dll' = 'fixture'; 'SqlSafetyGuard.Analyzer.dll' = 'fixture' }
        if ($MissingDll) { $files.Remove('SqlSafetyGuard.dll') }
        foreach ($name in $files.Keys) {
            $writer = New-Object IO.StreamWriter($archive.CreateEntry($name).Open())
            try { $writer.Write($files[$name]) } finally { $writer.Dispose() }
        }
        foreach ($name in $Extra) {
            $writer = New-Object IO.StreamWriter($archive.CreateEntry($name).Open())
            try { $writer.Write('unexpected') } finally { $writer.Dispose() }
        }
    }
    finally { $archive.Dispose() }
    return $path
}

try {
    $unsafeEntries = @('../escape.txt', 'nested/../../escape.txt', '..\escape.txt',
        'C:/escape.txt', '/escape.txt', '//server/share/escape.txt', 'file:stream',
        'nested/./escape.txt', 'nested//escape.txt', 'trailing./escape.txt', 'trailing /escape.txt', 'SQLSAFETYGUARD.DLL')
    $index = 0
    foreach ($entry in $unsafeEntries) {
        $archive = New-FixtureVsix -Name ('unsafe' + $index++) -Extra @($entry)
        Assert-Throws { Install-ManualCopy $archive $extensions } "Accepted unsafe entry $entry" | Out-Null
        Assert-True (Test-Path -LiteralPath (Join-Path $target 'previous.txt')) "Old install lost for $entry"
    }
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $FixtureRoot 'escape.txt'))) 'Traversal escaped staging.'
    $missing = New-FixtureVsix -Name 'missing' -MissingDll
    Assert-Throws { Install-ManualCopy $missing $extensions } 'Missing DLL accepted.' | Out-Null
    $wrong = New-FixtureVsix -Name 'wrong' -Identity '<PackageManifest><Metadata><Identity Id="other" /></Metadata></PackageManifest>'
    Assert-Throws { Install-ManualCopy $wrong $extensions } 'Wrong identity accepted.' | Out-Null
    Assert-True (Test-Path (Join-Path $target 'previous.txt')) 'Validation removed previous install.'

    $valid = New-FixtureVsix -Name 'valid'
    $originalMarker = ${function:Update-ExtensionsConfigurationChanged}
    function Update-ExtensionsConfigurationChanged { throw 'fixture marker failure' }
    Assert-Throws { Install-ManualCopy $valid $extensions } 'Marker failure ignored.' | Out-Null
    Assert-True (Test-Path (Join-Path $target 'previous.txt')) 'Marker failure did not roll back.'
    Set-Item Function:Update-ExtensionsConfigurationChanged $originalMarker

    function Move-Item {
        param($LiteralPath, $Destination)
        if ([IO.Path]::GetFileName($LiteralPath).StartsWith('.SqlSafetyGuard-stage-')) { throw 'fixture move failure' }
        Microsoft.PowerShell.Management\Move-Item -LiteralPath $LiteralPath -Destination $Destination
    }
    Assert-Throws { Install-ManualCopy $valid $extensions } 'Move failure ignored.' | Out-Null
    Assert-True (Test-Path (Join-Path $target 'previous.txt')) 'Move failure did not roll back.'
    Remove-Item Function:Move-Item
    Install-ManualCopy $valid $extensions | Out-Null
    Assert-True (Test-Path (Join-Path $target 'SqlSafetyGuard.dll')) 'Valid install missing.'
    Assert-True (-not (Test-Path (Join-Path $target 'previous.txt'))) 'Old payload retained.'
    Assert-True (@(Get-ChildItem $extensions -Force | Where-Object Name -like '.SqlSafetyGuard-*').Count -eq 0) 'Staging/backup leaked.'

    $ssms = [pscustomobject]@{ InstallPath = $FixtureRoot; InstanceId = 'fixture'; Version = '22.0'; UserExtensionsPath = $extensions }
    $script:processCalls = @()
    $script:exitCodes = New-Object 'System.Collections.Generic.Queue[int]'
    function Start-Process {
        param($FilePath, $ArgumentList, $WindowStyle, [switch] $Wait, [switch] $PassThru)
        $script:processCalls += [pscustomobject]@{ Arguments = $ArgumentList; WindowStyle = $WindowStyle }
        return [pscustomobject]@{ ExitCode = $script:exitCodes.Dequeue() }
    }
    Assert-Throws { Install-WithVsixInstaller $wrong $ssms } 'Invalid upgrade accepted.' | Out-Null
    Assert-True ($processCalls.Count -eq 0) 'Validation called uninstall.'
    $exitCodes.Enqueue(5)
    Assert-Throws { Install-WithVsixInstaller $valid $ssms } 'Uninstall failure ignored.' | Out-Null
    Assert-True ($processCalls.Count -eq 1) 'Install followed failed uninstall.'
    $exitCodes.Enqueue(0); $exitCodes.Enqueue(1)
    $message = Assert-Throws { Install-WithVsixInstaller $valid $ssms } 'Install failure ignored.'
    Assert-True ($message -like '*Recovery:*known-working previous*Do not assume the guard is active*') 'Recovery instructions absent.'
    $exitCodes.Enqueue(2003); $exitCodes.Enqueue(0)
    Install-WithVsixInstaller $valid $ssms
    foreach ($call in $processCalls) {
        Assert-True ($call.WindowStyle -eq 'Hidden') 'Installer window was not hidden.'
        Assert-True (@($call.Arguments | Where-Object { $_ -like '/logFile:"*temp with spaces*"' }).Count -eq 1) 'Log path not quoted.'
    }
    $latest = Select-LatestSsmsInstance @([pscustomobject]@{ installationVersion = '22.9.0' }, [pscustomobject]@{ installationVersion = '22.10.0' })
    Assert-True ($latest.installationVersion -eq '22.10.0') 'Versions sorted lexically.'

    $fixtureScripts = Join-Path $FixtureRoot 'uninstall/scripts'
    New-Item -ItemType Directory -Path $fixtureScripts -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $repository 'uninstall.ps1') -Destination (Join-Path $FixtureRoot 'uninstall/uninstall.ps1')
    $wrapper = @'
. '__COMMON__'
function Get-Ssms22 { [pscustomobject]@{ InstallPath = '__ROOT__'; Version = '22.0'; InstanceId = 'fixture' } }
function Assert-SsmsNotRunning { }
function Start-Process { [pscustomobject]@{ ExitCode = 5 } }
function Remove-ManualCopies { throw 'Manual deletion after failure' }
'@
    $wrapper = $wrapper.Replace('__COMMON__', (Join-Path $PSScriptRoot 'SsmsCommon.ps1').Replace("'", "''")).Replace('__ROOT__', $FixtureRoot.Replace("'", "''"))
    [IO.File]::WriteAllText((Join-Path $fixtureScripts 'SsmsCommon.ps1'), $wrapper)
    $ErrorActionPreference = 'Continue'
    $uninstallOutput = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $FixtureRoot 'uninstall/uninstall.ps1') 2>&1 | Out-String
    $uninstallExit = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'
    Assert-True ($uninstallExit -ne 0) 'Uninstall exit code was zero.'
    Assert-True ($uninstallOutput -notlike '*Done. Restart SSMS*') 'Uninstall reported Done after failure.'
    Write-Host "Installer safety fixtures passed ($caseCount checks)."
}
finally { Remove-ExtensionDirectory -Path $FixtureRoot -Root $repository }
