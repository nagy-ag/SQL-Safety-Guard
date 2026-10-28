[CmdletBinding()]
param([string]$Server = '.\SQLEXPRESS')
& (Join-Path $PSScriptRoot 'Run-Overnight.ps1') -Server $Server -Workers 1 -PrepareOnly
exit $LASTEXITCODE

