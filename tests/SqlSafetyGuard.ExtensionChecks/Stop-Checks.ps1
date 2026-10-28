param([Parameter(Mandatory=$true)][string]$OwnershipFile)
$ErrorActionPreference='Stop'
$owned=Get-Content -LiteralPath $OwnershipFile -Raw | ConvertFrom-Json
if($owned.Cleaned){ return }
if($owned.Database -notmatch '^SgExt_[A-Za-z0-9]+$' -or $owned.Suffix -notmatch '^SgExt[A-Za-z0-9]+$'){ throw 'Invalid ownership manifest.' }
if($owned.ProcessId){
    $process=Get-Process -Id $owned.ProcessId -ErrorAction SilentlyContinue
    if($process){
        $command=Get-CimInstance Win32_Process -Filter "ProcessId=$($owned.ProcessId)"
        if($process.StartTime.ToUniversalTime().ToString('O') -ne $owned.StartTime -or $command.CommandLine -notmatch ('/rootSuffix\s+'+[regex]::Escape($owned.Suffix)+'(?:\s|$)')){ throw 'Process identity changed; refusing cleanup.' }
        . "$PSScriptRoot\Client.ps1" -ChannelDirectory $owned.Directory -SsmsProcessId $owned.ProcessId -Database $owned.Database -Server $owned.Server
        Send-Channel close -NoWait | Out-Null
        if(!$process.WaitForExit(30000)){ throw 'Own SSMS did not quit; database retained for recovery.' }
    }
}
$builder=New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source']=$owned.Server; $builder['Initial Catalog']='master'; $builder['Integrated Security']=$true; $builder['TrustServerCertificate']=$true; $builder['Application Name']='SgExtCleanup'
$connection=New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
try {
    $connection.Open(); $command=$connection.CreateCommand()
    if($owned.SessionCreated){ $command.CommandText="DROP EVENT SESSION [$($owned.Database)] ON SERVER;"; [void]$command.ExecuteNonQuery(); $owned.SessionCreated=$false; $owned | ConvertTo-Json | Set-Content -LiteralPath $OwnershipFile -Encoding UTF8 }
    if($owned.DatabaseCreated){ $command.CommandText="ALTER DATABASE [$($owned.Database)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$($owned.Database)];"; [void]$command.ExecuteNonQuery(); $owned.DatabaseCreated=$false }
    $owned.Cleaned=$true
    $owned | ConvertTo-Json | Set-Content -LiteralPath $OwnershipFile -Encoding UTF8
} finally { $connection.Dispose() }
