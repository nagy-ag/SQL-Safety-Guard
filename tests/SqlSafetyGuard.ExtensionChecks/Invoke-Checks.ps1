param([string]$Server='.\SQLEXPRESS',[string]$MSBuild='C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe')
$ErrorActionPreference='Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
. "$repo\scripts\SsmsCommon.ps1"
$ssms = Get-Ssms22
$suffix = 'SgExt' + [guid]::NewGuid().ToString('N').Substring(0,12)
$database = 'SgExt_' + $suffix.Substring(5)
$directory = Join-Path $env:TEMP $suffix
New-Item -ItemType Directory -Path $directory | Out-Null
$utf8 = New-Object Text.UTF8Encoding($false)
$manifest = [ordered]@{Database=$database;Server=$Server;Suffix=$suffix;Directory=$directory;DatabaseCreated=$false;SessionCreated=$false;ProcessId=0;StartTime='';Cleaned=$false}
$manifestPath = Join-Path $directory 'ownership.json'
function Save-Ownership { $manifest | ConvertTo-Json | Set-Content -LiteralPath $manifestPath -Encoding UTF8 }
function Invoke-OwnedSql([string]$Text,[string]$Catalog='master',[switch]$Scalar) {
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
    $builder['Data Source']=$Server; $builder['Initial Catalog']=$Catalog; $builder['Integrated Security']=$true; $builder['TrustServerCertificate']=$true; $builder['Application Name']='SgExtEvidence'
    $connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    try { $connection.Open(); $command=$connection.CreateCommand(); $command.CommandText=$Text
        if($Scalar){ return $command.ExecuteScalar() }; [void]$command.ExecuteNonQuery()
    } finally { $connection.Dispose() }
}
Save-Ownership
try {
    & $MSBuild "$repo\SqlSafetyGuard.sln" /restore /p:Configuration=Debug /v:minimal /nologo
    if($LASTEXITCODE -ne 0){ throw 'Debug build failed.' }
    $vsix = Join-Path $repo 'src\SqlSafetyGuard.Vsix\bin\Debug\SqlSafetyGuard.vsix'
    $installer = Join-Path $ssms.InstallPath 'Common7\IDE\VSIXInstaller.exe'
    $install = Start-Process -FilePath $installer -ArgumentList @('/quiet',"/instanceIds:$($ssms.InstanceId)","/rootSuffix:$suffix",("/logFile:" + $directory + '\install.log'),('"'+$vsix+'"')) -WindowStyle Hidden -PassThru
    if(!$install.WaitForExit(180000) -or $install.ExitCode -ne 0){ throw 'Isolated VSIX install failed; inspect install.log.' }
    Invoke-OwnedSql "CREATE DATABASE [$database];"
    $manifest.DatabaseCreated=$true; Save-Ownership
    Invoke-OwnedSql "CREATE TABLE dbo.Counter(Tag nvarchar(80) NOT NULL PRIMARY KEY, Executions int NOT NULL DEFAULT 0); INSERT dbo.Counter(Tag) VALUES ('red'),('yellow'),('outside');" $database
    Invoke-OwnedSql "CREATE PROCEDURE dbo.Tick AS UPDATE dbo.Counter SET Executions=Executions+1 WHERE Tag='yellow';" $database
    $databaseId = Invoke-OwnedSql "SELECT DB_ID(N'$database');" -Scalar
    Invoke-OwnedSql "CREATE EVENT SESSION [$database] ON SERVER ADD EVENT sqlserver.sql_batch_completed(ACTION(sqlserver.database_id,sqlserver.client_pid,sqlserver.client_app_name) WHERE ([sqlserver].[database_id]=($databaseId))) ADD TARGET package0.ring_buffer; ALTER EVENT SESSION [$database] ON SERVER STATE=START;"
    $manifest.SessionCreated=$true; Save-Ownership
    foreach($name in @('SgExtA.sql','SgExtB.sql')) { [IO.File]::WriteAllText((Join-Path $directory $name),'SELECT 1;', $utf8) }
    $previousDirectory=$env:SQLSAFETYGUARD_TEST_DIRECTORY
    try {
        $env:SQLSAFETYGUARD_TEST_DIRECTORY=$directory
        $process = Start-Process -FilePath (Join-Path $ssms.InstallPath 'Common7\IDE\SSMS.exe') -ArgumentList @(('"'+(Join-Path $directory 'SgExtA.sql')+'"'),'/rootSuffix',$suffix,'-nosplash','-S',$Server,'-d',$database,'-C','-log',('"'+(Join-Path $directory 'activity.xml')+'"')) -WindowStyle Hidden -PassThru
    } finally { $env:SQLSAFETYGUARD_TEST_DIRECTORY=$previousDirectory }
    $manifest.ProcessId=$process.Id; $manifest.StartTime=$process.StartTime.ToUniversalTime().ToString('O'); Save-Ownership
    . "$PSScriptRoot\Client.ps1" -ChannelDirectory $directory -SsmsProcessId $process.Id -Database $database -Server $Server
    $deadline=[DateTime]::UtcNow.AddMinutes(3)
    do {
        foreach($window in Get-OwnWindows) {
            if($window.Current.Name -like '*Microsoft SQL Server Management Studio'){ [ExtensionCheckWindows]::ShowWithoutActivation([IntPtr]$window.Current.NativeWindowHandle,$process.Id) }
            $elements=@($window.FindAll([System.Windows.Automation.TreeScope]::Descendants,[System.Windows.Automation.Condition]::TrueCondition))
            $text=($elements | ForEach-Object {$_.Current.Name}) -join ' '
            if($text -match 'connect' -and $text -match 'Yes') {
                $yes=$elements | Where-Object {$_.Current.Name -eq 'Yes' -or $_.Current.Name -eq '&Yes'} | Select-Object -First 1
                if($yes -and $yes.Current.NativeWindowHandle){ [ExtensionCheckWindows]::ClickButton([IntPtr]$yes.Current.NativeWindowHandle,$process.Id) }
            }
        }
        if(Test-Path (Join-Path $directory 'events.tsv')){ break }
        $request=Join-Path $directory 'SqlSafetyGuard-pane-cmd.txt'
        if(!(Test-Path $request)){ [IO.File]::WriteAllText($request,'SgExtA.sql|state|',$utf8) }
        Start-Sleep -Milliseconds 500
    } while([DateTime]::UtcNow -lt $deadline)
    Wait-Channel state 0
    Send-Channel commands (Join-Path $directory 'commands.tsv')
    Send-Channel open (Join-Path $directory 'SgExtB.sql')
    Start-Sleep -Seconds 10
    Send-Channel dismisswelcome
    Send-Channel document SgExtA.sql
    Send-Channel execute Query.Execute
    & "$PSScriptRoot\Run-Checks.ps1" -ChannelDirectory $directory -SsmsProcessId $process.Id -Database $database -Server $Server
    Write-Host "Evidence: $directory"
} catch {
    if (Get-Command Get-OwnWindows -ErrorAction SilentlyContinue) {
        Get-OwnWindows | ForEach-Object { $_.FindAll([System.Windows.Automation.TreeScope]::Descendants,[System.Windows.Automation.Condition]::TrueCondition) } | ForEach-Object { $_.Current | Select-Object Name,AutomationId,ControlType,IsEnabled } | ConvertTo-Json -Depth 3 | Set-Content (Join-Path $directory "failure-ui.json")
    }
    throw
} finally {
    & "$PSScriptRoot\Stop-Checks.ps1" -OwnershipFile $manifestPath
}
