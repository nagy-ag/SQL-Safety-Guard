#requires -Version 5.1
<#
.SYNOPSIS
Captures real SQL Safety Guard surfaces in an isolated SSMS 22 Exp profile. Requires the Debug VSIX.
.DESCRIPTION
No keyboard, mouse, clipboard, screen capture, Execute command, or SQL connection is used. SSMS runs on
a private Windows desktop that is never displayed or switched to, preventing startup focus theft.
Main-window captures combine PrintWindow(PW_RENDERFULLCONTENT) with real WPF HWND renders because
an undisplayed desktop has no DWM composition for PrintWindow to copy.
Settings are backed up and restored byte-for-byte, including when capture fails. Do not run another
Exp session concurrently. Output must be a new/empty directory outside the repository.

Theme storage was checked in SSMS 22's CommonExtensions/Platform/Shell/Shell.registration.json,
CommonExtensions/Platform/Themes.Registration.pkgdef, and the shipped Microsoft.VisualStudio.Shell.UI.Internal.dll:
EnvironmentOptions.GeneralOptionsPage.KnownThemeInvariantNameMap and ColorThemeDefinitionResolver.
The stored values are invariant lowercase hyphenated names, NOT the registration GUIDs.
#>
[CmdletBinding()]
param(
    [string] $SettingsPath = "$env:LOCALAPPDATA\Microsoft\SSMS\22.0_e8efe2d3Exp\settings.json",
    [Parameter(Mandatory = $true)][string] $OutputDirectory,
    [string[]] $Themes = @('Light', 'Cool Breeze', 'Icy Mint', 'Sunny Day', 'Juicy Plum', 'Mango Paradise',
        'Bubblegum', 'Silky Pink', 'Dark', 'Cool Slate', 'Moonlight Glow', 'Mystical Forest', 'Spicy Red')
)

$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..')).TrimEnd('\')
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory).TrimEnd('\')
$SettingsPath = [IO.Path]::GetFullPath($SettingsPath)
if ($OutputDirectory -eq $repository -or $OutputDirectory.StartsWith($repository + '\', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Capture output must be outside the repository.'
}
if ($SettingsPath -notmatch '\\22\.0_[^\\]+Exp\\settings\.json$') { throw 'Only an isolated SSMS Exp settings.json is supported.' }
if (-not (Test-Path -LiteralPath $SettingsPath -PathType Leaf)) { throw "Settings file not found: $SettingsPath" }
if ((Test-Path -LiteralPath $OutputDirectory) -and @(Get-ChildItem -LiteralPath $OutputDirectory -Force).Count) {
    throw 'Choose a new or empty output directory so backups and evidence cannot be overwritten.'
}
$themeValues = [ordered]@{
    'Light' = 'light'; 'Cool Breeze' = 'cool-breeze'; 'Icy Mint' = 'icy-mint'; 'Sunny Day' = 'sunny-day'
    'Juicy Plum' = 'juicy-plum'; 'Mango Paradise' = 'mango-paradise'; 'Bubblegum' = 'bubblegum'; 'Silky Pink' = 'silky-pink'
    'Dark' = 'dark'; 'Cool Slate' = 'cool-slate'; 'Moonlight Glow' = 'moonlight-glow'
    'Mystical Forest' = 'mystical-forest'; 'Spicy Red' = 'spicy-red'
}
if (-not $Themes.Count) { throw 'Select at least one theme.' }
foreach ($theme in $Themes) { if (-not $themeValues.Contains($theme)) { throw "Unknown theme: $theme" } }
if (@($Themes | Select-Object -Unique).Count -ne $Themes.Count) { throw 'Duplicate themes are not supported.' }
. (Join-Path $PSScriptRoot 'SsmsCommon.ps1')
$ssmsInfo = Get-Ssms22
$ssmsExe = Join-Path $ssmsInfo.InstallPath 'Common7\IDE\SSMS.exe'
if ((Split-Path (Split-Path $SettingsPath -Parent) -Leaf) -ne ('22.0_' + $ssmsInfo.InstanceId + 'Exp')) {
    throw 'SettingsPath must belong to the installed SSMS 22 instance that will be launched.'
}

function Assert-ExpClosed {
    $existing = @(Get-CimInstance Win32_Process -Filter "Name = 'SSMS.exe'" | Where-Object {
        $_.CommandLine -match '(?i)/rootSuffix\s+"?Exp(?:"|\s|$)'
    })
    if ($existing.Count) { throw "An Exp process is already running (PID $($existing.ProcessId -join ', ')). It belongs to another run; leave it alone." }
}
Assert-ExpClosed

Add-Type -AssemblyName System.Drawing
if (-not ('ThemeCaptureNative' -as [type])) {
Add-Type -TypeDefinition @'
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
public static class ThemeCaptureNative {
    static readonly Dictionary<int,IntPtr> desktops = new Dictionary<int,IntPtr>();
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] struct Startup {
        public int cb; public string reserved, desktop, title; public int x,y,xSize,ySize,xChars,yChars,fill,flags;
        public short show; public short reservedSize; public IntPtr reserved2, input, output, error;
    }
    [StructLayout(LayoutKind.Sequential)] struct ProcessInfo { public IntPtr process, thread; public int pid, tid; }
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern bool CreateProcess(
        string app, StringBuilder command, IntPtr pa, IntPtr ta, bool inherit, uint flags, IntPtr env, string cwd, ref Startup startup, out ProcessInfo info);
    [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr handle);
    public delegate bool EnumProc(IntPtr h, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc callback, IntPtr p);
    [DllImport("user32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern IntPtr CreateDesktop(string name, IntPtr device, IntPtr mode, uint flags, uint access, IntPtr security);
    [DllImport("user32.dll")] static extern bool CloseDesktop(IntPtr desktop);
    [DllImport("user32.dll")] static extern bool EnumDesktopWindows(IntPtr desktop, EnumProc callback, IntPtr p);
    [DllImport("user32.dll")] static extern IntPtr GetWindow(IntPtr h, uint command);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr h, EnumProc callback, IntPtr p);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder text, int size);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder text, int size);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint message, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, uint message, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr h, int show);
    [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    public static uint ForegroundProcessId() { uint pid; GetWindowThreadProcessId(GetForegroundWindow(),out pid); return pid; }
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    public static int Launch(string exe, string sql, string directory) {
        // Never switch to this desktop. Even SSMS startup code cannot take the shared desktop's focus.
        string desktopName = "SqlSafetyGuardCapture-"+Guid.NewGuid().ToString("N");
        IntPtr desktop = CreateDesktop(desktopName,IntPtr.Zero,IntPtr.Zero,0,0x10000000,IntPtr.Zero);
        if(desktop==IntPtr.Zero)throw new Win32Exception();
        var env = new SortedDictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        foreach (DictionaryEntry item in Environment.GetEnvironmentVariables()) env[(string)item.Key] = (string)item.Value;
        env["SQLSAFETYGUARD_TEST_DIRECTORY"] = directory;
        env["SQLSAFETYGUARD_FAULT_INJECTION"] = "1";
        var block = new StringBuilder();
        foreach (var item in env) block.Append(item.Key).Append('=').Append(item.Value).Append('\0');
        block.Append('\0');
        var ptr = Marshal.StringToHGlobalUni(block.ToString());
        try {
            var startup = new Startup { cb=Marshal.SizeOf(typeof(Startup)), desktop=desktopName, flags=1, show=7 /* SW_SHOWMINNOACTIVE */ };
            ProcessInfo info;
            if (!CreateProcess(exe, new StringBuilder("\""+exe+"\" /rootSuffix Exp -nosplash \""+sql+"\""),
                IntPtr.Zero,IntPtr.Zero,false,0x400,ptr,System.IO.Path.GetDirectoryName(exe),ref startup,out info)) throw new Win32Exception();
            CloseHandle(info.thread); CloseHandle(info.process); desktops.Add(info.pid,desktop); return info.pid;
        } catch { CloseDesktop(desktop);throw; } finally { Marshal.FreeHGlobal(ptr); }
    }
    public static void ReleaseDesktop(int pid) { IntPtr desktop; if(desktops.TryGetValue(pid,out desktop)) {CloseDesktop(desktop);desktops.Remove(pid);} }
    public static string Text(IntPtr h) { var s=new StringBuilder(4096); GetWindowText(h,s,s.Capacity); return s.ToString(); }
    static string Class(IntPtr h) { var s=new StringBuilder(256); GetClassName(h,s,s.Capacity); return s.ToString(); }
    public static IntPtr[] Windows(int pid) {
        var found=new List<IntPtr>();
        EnumProc collect=(h,p)=>{uint owner; GetWindowThreadProcessId(h,out owner); if(owner==pid)found.Add(h);return true;};
        IntPtr desktop;
        if(desktops.TryGetValue(pid,out desktop))EnumDesktopWindows(desktop,collect,IntPtr.Zero);else EnumWindows(collect,IntPtr.Zero);
        return found.ToArray();
    }
    public static IntPtr MainWindow(int pid) {
        foreach(var h in Windows(pid)) if(IsWindowVisible(h) && GetWindow(h,4 /* GW_OWNER */)==IntPtr.Zero &&
            Text(h).IndexOf("Microsoft SQL Server Management Studio",StringComparison.OrdinalIgnoreCase)>=0)return h;
        return IntPtr.Zero;
    }
    public static void ShowMain(IntPtr h) {
        ShowWindow(h,4 /* SW_SHOWNOACTIVATE */);
        SetWindowPos(h,IntPtr.Zero,20,20,1400,960,0x14 /* NOZORDER | NOACTIVATE */);
    }
    public static void Close(IntPtr h) { PostMessage(h,0x10,IntPtr.Zero,IntPtr.Zero); }
    public static void DismissShutdown(int pid) {
        foreach(var h in Windows(pid)) {
            if(!IsWindowVisible(h))continue;
            var children=new List<IntPtr>(); EnumChildWindows(h,(c,p)=>{children.Add(c);return true;},IntPtr.Zero);
            var text=Text(h)+" "+string.Join(" ",children.ConvertAll(Text));
            if(text.IndexOf("shut down anyway",StringComparison.OrdinalIgnoreCase)<0)continue;
            foreach(var child in children) {
                string label=Text(child).Replace("&","");
                if(Class(child)=="Button" && (label=="Yes" || label=="Shut Down" || label=="Shut down anyway"))
                    SendMessage(child,0xF5 /* BM_CLICK */,IntPtr.Zero,IntPtr.Zero);
            }
        }
    }
    // Preserve strings, offsets and newlines while removing JSONC comments. PS 5.1 ConvertFrom-Json has no JSONC support.
    public static string Json(string source) {
        var s=new StringBuilder(); bool quote=false, escape=false, line=false, block=false;
        for(int i=0;i<source.Length;i++) {
            char c=source[i], next=i+1<source.Length?source[i+1]:'\0';
            if(line) { if(c=='\r'||c=='\n')line=false; else {s.Append(' ');continue;} }
            if(block) { if(c=='*'&&next=='/') {s.Append("  ");i++;block=false;} else s.Append(c=='\r'||c=='\n'?c:' ');continue; }
            if(!quote && c=='/' && (next=='/'||next=='*')) {line=next=='/';block=next=='*';s.Append("  ");i++;continue;}
            s.Append(c); if(escape)escape=false;else if(quote&&c=='\\')escape=true;else if(c=='"')quote=!quote;
        }
        if(block)throw new FormatException("Unclosed JSON comment."); return s.ToString();
    }
}
'@
}

$utf8 = New-Object Text.UTF8Encoding($false)
New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
$original = [IO.File]::ReadAllBytes($SettingsPath)
$beforeHash = (Get-FileHash -LiteralPath $SettingsPath -Algorithm SHA256).Hash
[IO.File]::WriteAllBytes((Join-Path $OutputDirectory 'settings.original.json'), $original)
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'settings.original.sha256'), $beforeHash, $utf8)
$themeValues | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'theme-values.json') -Encoding UTF8
$manifest = New-Object 'System.Collections.Generic.List[object]'
$process = $null
$script:channelDirectory = $null
$script:documentName = 'ThemeAcceptance.sql'
$script:eventOffset = 0
$failure = $null

function Read-Events {
    $path = Join-Path $script:channelDirectory 'events.tsv'
    if (Test-Path -LiteralPath $path) {
        # ReadAllText uses FileShare.Read and can make the in-process trace drop a receipt.
        $reader = $null
        try {
            $stream = [IO.File]::Open($path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
            $reader = New-Object IO.StreamReader($stream)
            return $reader.ReadToEnd()
        }
        catch [IO.IOException] { return '' }
        finally { if ($null -ne $reader) { $reader.Dispose() } }
    }
    return ''
}
function Wait-Receipt([string] $Pattern, [int] $Seconds = 60) {
    $deadline = [DateTime]::UtcNow.AddSeconds($Seconds)
    while ([DateTime]::UtcNow -lt $deadline) {
        if ($process.HasExited) { throw "SSMS exited before receipt: $Pattern" }
        $events = Read-Events
        $recent = if ($events.Length -ge $script:eventOffset) { $events.Substring($script:eventOffset) } else { '' }
        if ($recent -match '(?m)\t(?:error|capture-error)\t') { throw "Channel error: $recent" }
        if ($recent -match ('(?m)' + $Pattern)) { return }
        Start-Sleep -Milliseconds 200
    }
    throw "Timed out waiting for $Pattern. Recent trace: $recent"
}
function Send-Command([string] $Verb, [string] $Argument = '') {
    $script:eventOffset = (Read-Events).Length
    $command = Join-Path $script:channelDirectory 'SqlSafetyGuard-pane-cmd.txt'
    $staging = $command + '.tmp'
    [IO.File]::WriteAllText($staging, "$script:documentName|$Verb|$Argument`r`n", $utf8)
    Move-Item -LiteralPath $staging -Destination $command
    Wait-Receipt ('\tack\t' + [regex]::Escape($Verb) + '(?:\r?$)')
    if ([ThemeCaptureNative]::ForegroundProcessId() -eq $process.Id) { throw 'The capture process unexpectedly took foreground focus.' }
}
function Capture-Surface([string] $Surface, [string] $Name) {
    $path = Join-Path $script:channelDirectory ($Name + '.png')
    Send-Command 'capture' "$Surface $path"
    Wait-Receipt ('\tcaptured\t' + [regex]::Escape($Surface) + '\t' + [regex]::Escape($path) + '\t')
    $bitmap = [Drawing.Bitmap]::FromFile($path)
    try {
        [double]$sum = 0; [double]$squares = 0; [int]$count = 0; [int]$visible = 0
        $step = [Math]::Max(1, [int]([Math]::Min($bitmap.Width, $bitmap.Height) / 180))
        for ($y = 0; $y -lt $bitmap.Height; $y += $step) {
            for ($x = 0; $x -lt $bitmap.Width; $x += $step) {
                $pixel = $bitmap.GetPixel($x, $y)
                $value = ($pixel.R + $pixel.G + $pixel.B) / 3.0
                $sum += $value; $squares += $value * $value; $count++
                if ($pixel.A -gt 0) { $visible++ }
            }
        }
        $variance = $squares / $count - [Math]::Pow($sum / $count, 2)
        $nonBlank = $variance -gt 2 -and $visible -gt ($count / 2)
        $manifest.Add([pscustomobject]@{ Theme = $theme; Value = $themeValues[$theme]; Surface = $Name; File = $path
            Size = (Get-Item -LiteralPath $path).Length; Width = $bitmap.Width; Height = $bitmap.Height
            PixelVariance = [Math]::Round($variance, 3); NonBlank = $nonBlank })
        $manifest | Export-Csv -LiteralPath (Join-Path $OutputDirectory 'manifest.csv') -NoTypeInformation -Encoding UTF8
        if (-not $nonBlank) { throw "Blank capture: $path" }
        Write-Output "$theme / $Name : $($bitmap.Width)x$($bitmap.Height), variance $([Math]::Round($variance, 1))"
    }
    finally { $bitmap.Dispose() }
}
function Close-OwnedSsms {
    if ($null -eq $process) { return }
    if ($process.HasExited) { [ThemeCaptureNative]::ReleaseDesktop($process.Id); return }
    # Mark only this run's scratch editor clean, then close the main window with WM_CLOSE.
    # The catch also covers startup failure before a pane/channel exists.
    try { Send-Command 'preparecaptureclose' } catch { Write-Warning "Could not prepare scratch document for closing: $_" }
    $main = [ThemeCaptureNative]::MainWindow($process.Id)
    if ($main -ne [IntPtr]::Zero) { [ThemeCaptureNative]::Close($main) }
    $deadline = [DateTime]::UtcNow.AddSeconds(60)
    while (-not $process.HasExited -and [DateTime]::UtcNow -lt $deadline) {
        [ThemeCaptureNative]::DismissShutdown($process.Id)
        Start-Sleep -Milliseconds 250
    }
    if (-not $process.HasExited) { throw "SSMS PID $($process.Id) did not close cleanly. It was not killed. Backup: $OutputDirectory\settings.original.json" }
    [ThemeCaptureNative]::ReleaseDesktop($process.Id)
}

try {
    foreach ($theme in $Themes) {
        Assert-ExpClosed
        $settings = [ThemeCaptureNative]::Json([IO.File]::ReadAllText($SettingsPath)) | ConvertFrom-Json
        $settings | Add-Member NoteProperty 'environment.visualExperience.colorTheme' $themeValues[$theme] -Force
        [IO.File]::WriteAllText($SettingsPath, ($settings | ConvertTo-Json -Depth 100), $utf8)
        $script:channelDirectory = Join-Path $OutputDirectory $themeValues[$theme]
        New-Item -ItemType Directory -Path $script:channelDirectory | Out-Null
        $sql = Join-Path $script:channelDirectory $script:documentName
        [IO.File]::WriteAllText($sql, 'SELECT 1;', $utf8)
        $startedPid = [ThemeCaptureNative]::Launch($ssmsExe, $sql, $script:channelDirectory)
        $process = [Diagnostics.Process]::GetProcessById($startedPid)
        [IO.File]::WriteAllText((Join-Path $script:channelDirectory 'ssms.pid'), [string]$startedPid, $utf8)
        $script:eventOffset = 0
        Wait-Receipt '\tanalysis\tThemeAcceptance\.sql\t' 120
        $main = [ThemeCaptureNative]::MainWindow($process.Id)
        if ($main -eq [IntPtr]::Zero) { throw "Cannot find this run's SSMS main window." }
        [ThemeCaptureNative]::ShowMain($main)
        Send-Command 'dismisswelcome'
        Send-Command 'widen' '160'
        foreach ($query in @(
            @{ Name = 'green'; Text = 'SELECT 1;'; Verdict = 'ReadOnly' },
            @{ Name = 'yellow'; Text = 'EXEC dbo.SomeProc;'; Verdict = 'Unknown' },
            @{ Name = 'red'; Text = 'UPDATE dbo.T SET x = 1;'; Verdict = 'RealWrite' }
        )) {
            $queryFile = Join-Path $script:channelDirectory ($query.Name + '.sql')
            [IO.File]::WriteAllText($queryFile, $query.Text, $utf8)
            # A comment changes the buffer even for the first green query; every wait has a fresh analysis receipt.
            [IO.File]::AppendAllText($queryFile, "`r`n-- theme acceptance $($query.Name)", $utf8)
            Send-Command 'replacefile' $queryFile
            Wait-Receipt ('\tanalysis\tThemeAcceptance\.sql\t[^\t]+\t' + $query.Verdict + '\r?$')
            Capture-Surface 'pane' ('pane-' + $query.Name)
            if ($query.Name -ne 'green') {
                Send-Command 'showconfirm'
                Wait-Receipt '\tshown\tconfirm\t'
                Capture-Surface 'confirm' ('confirm-' + $query.Name)
                Send-Command 'closedialogs'
            }
        }
        Send-Command 'showfailure'
        Wait-Receipt '\tshown\tfailure\t'
        Capture-Surface 'failure' 'failure'
        Send-Command 'closedialogs'
        foreach ($list in @('allowList', 'trustedProcedures', 'environmentRules')) {
            Send-Command 'showlisteditor' $list
            Wait-Receipt ('\tshown\t' + $list + '\t')
            Capture-Surface $list ('list-' + $list)
            Send-Command 'closedialogs'
        }
        $sample = Join-Path $script:channelDirectory 'sample.ssgsettings.json'
        Send-Command 'settingsexport' $sample
        $transfer = Get-Content -LiteralPath $sample -Raw | ConvertFrom-Json
        $unique = [guid]::NewGuid().ToString('N')
        $transfer.settings.allowList = @($transfer.settings.allowList) + "dbo.ThemeCapture_$unique"
        $transfer.settings.trustedProcedures = @($transfer.settings.trustedProcedures) + "dbo.ThemeCaptureProc_$unique"
        [IO.File]::WriteAllText($sample, ($transfer | ConvertTo-Json -Depth 30), $utf8)
        Send-Command 'showimportpreview' $sample
        Wait-Receipt '\tshown\timportpreview\t'
        Capture-Surface 'importpreview' 'import-preview'
        Send-Command 'closedialogs'
        Send-Command 'page' 'settings'
        Capture-Surface 'pane' 'pane-settings'
        Send-Command 'page' 'analysis'
        Capture-Surface 'main' 'main-toolbar'
        Send-Command 'showoptions'
        Wait-Receipt '\tshown\toptions\tF5 warnings\r?$'
        Capture-Surface 'main' 'options'
        Close-OwnedSsms
        $process.Dispose(); $process = $null
    }
}
catch { $failure = $_; throw }
finally {
    try { Close-OwnedSsms }
    finally {
        # Restoration is unconditional, even if shutdown/capture failed. Never change the backup.
        [IO.File]::WriteAllBytes($SettingsPath, $original)
        $afterHash = (Get-FileHash -LiteralPath $SettingsPath -Algorithm SHA256).Hash
        [pscustomobject]@{ SettingsPath = $SettingsPath; BeforeSHA256 = $beforeHash; AfterSHA256 = $afterHash
            Restored = ($beforeHash -eq $afterHash); Error = [string]$failure; Captures = $manifest.Count
            SsmsExited = ($null -eq $process -or $process.HasExited) } |
            ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'restoration.json') -Encoding UTF8
        if ($beforeHash -ne $afterHash) { throw "Settings restoration hash mismatch. Backup: $OutputDirectory\settings.original.json" }
        Write-Output "Settings restored byte-for-byte: $afterHash"
    }
}
