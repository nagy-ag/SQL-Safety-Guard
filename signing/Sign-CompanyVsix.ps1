<#
.SYNOPSIS
    Checks an unsigned GitHub VSIX, then optionally signs a separate company copy.
.DESCRIPTION
    Run only on the company signing computer. Never creates keys, configures trust,
    installs tools or uploads files. -CheckOnly needs no certificate or Sign CLI.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string] $VsixPath,
    [Parameter(Mandatory = $true)][string] $ChecksumsPath,
    [string] $PublicCertificatePath,
    [string] $OutputPath,
    [string] $SignToolPath = 'sign',
    [string] $TimestampUrl = 'http://timestamp.acs.microsoft.com/',
    [switch] $CheckOnly
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$vsix = (Resolve-Path -LiteralPath $VsixPath).ProviderPath
$checksums = (Resolve-Path -LiteralPath $ChecksumsPath).ProviderPath
$name = [IO.Path]::GetFileName($vsix)
if ($name -notmatch '^SqlSafetyGuard-(\d+\.\d+\.\d+)\.vsix$') {
    throw 'Keep the original GitHub filename: SqlSafetyGuard-<version>.vsix.'
}
$version = $Matches[1]
$expected = @()
foreach ($line in [IO.File]::ReadAllLines($checksums)) {
    if ($line -match '^([0-9a-fA-F]{64})  (.+)$' -and $Matches[2] -ceq $name) { $expected += $Matches[1] }
}
if ($expected.Count -ne 1) { throw 'Checksum manifest must contain exactly one entry for this VSIX.' }
$inputHash = (Get-FileHash -LiteralPath $vsix -Algorithm SHA256).Hash
if ($inputHash -ine $expected[0]) { throw 'VSIX checksum mismatch. Download the release again.' }

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead($vsix)
try {
    if (@($archive.Entries | Where-Object {
        $_.FullName.Replace('\', '/') -match '(?i)(^|/)(_xmlsignatures|package/services/digital-signature)(/|$)|\.(psdsxs|psdor|psdsor)$'
    }).Count) {
        throw 'Input already has package signatures. Use the original unsigned GitHub VSIX.'
    }
    $manifests = @($archive.Entries | Where-Object { $_.FullName -ceq 'extension.vsixmanifest' })
    if ($manifests.Count -ne 1) { throw 'VSIX must contain exactly one extension manifest.' }
    $reader = [IO.StreamReader]::new($manifests[0].Open())
    try { [xml] $manifest = $reader.ReadToEnd() } finally { $reader.Dispose() }
    $identity = $manifest.PackageManifest.Metadata.Identity
    if ($identity.Id -ne 'SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558' -or $identity.Version -ne $version) {
        throw 'VSIX identity/version does not match the release filename.'
    }
}
finally { $archive.Dispose() }
Write-Host "Verified unsigned input: $name SHA256 $inputHash"
if ($CheckOnly) { return }

if (-not $PublicCertificatePath -or -not $OutputPath) { throw 'Signing requires -PublicCertificatePath and -OutputPath.' }
$output = [IO.Path]::GetFullPath($OutputPath)
$receiptPath = $output + '.signing.json'
if ([IO.Path]::GetExtension($output) -ine '.vsix') { throw 'Output must have the .vsix extension.' }
if ((Test-Path -LiteralPath $output) -or (Test-Path -LiteralPath $receiptPath)) { throw 'Output or receipt already exists; nothing will be overwritten.' }
$cer = (Resolve-Path -LiteralPath $PublicCertificatePath).ProviderPath
if ([IO.Path]::GetExtension($cer) -ine '.cer') { throw 'Supply the DER public .cer, never a private-key file.' }
$publicCert = [Security.Cryptography.X509Certificates.X509Certificate2]::new($cer)
try {
    if ([Convert]::ToBase64String($publicCert.RawData) -cne [Convert]::ToBase64String([IO.File]::ReadAllBytes($cer))) {
        throw 'Public certificate must be a DER-encoded X.509 .cer.'
    }
    $storeCert = Get-Item -LiteralPath ("Cert:\CurrentUser\My\" + $publicCert.Thumbprint)
    if (-not $storeCert.HasPrivateKey) { throw 'This company account does not have the signing private key.' }
    if ((Get-Date) -lt $storeCert.NotBefore -or (Get-Date) -ge $storeCert.NotAfter) { throw 'Signing certificate is outside its validity period.' }
    $eku = @($storeCert.Extensions | Where-Object { $_.Oid.Value -eq '2.5.29.37' } |
        ForEach-Object { $_.EnhancedKeyUsages } | Where-Object { $_.Value -eq '1.3.6.1.5.5.7.3.3' })
    if (-not $eku.Count) {
        throw 'Signing certificate must explicitly allow code signing.'
    }
    $fingerprint = (Get-FileHash -LiteralPath $cer -Algorithm SHA256).Hash
}
finally { $publicCert.Dispose() }
$tool = (Get-Command $SignToolPath -CommandType Application -ErrorAction Stop).Source
$toolVersion = & $tool --version
if ($LASTEXITCODE -ne 0) { throw 'Sign CLI version check failed.' }
$outputFolder = [IO.Path]::GetDirectoryName($output)
New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null
# Sign a disposable copy so even an unexpected tool failure cannot modify the download.
$staging = Join-Path $outputFolder (([Guid]::NewGuid().ToString('N')) + '.vsix')
Copy-Item -LiteralPath $vsix -Destination $staging
try {
    & $tool code certificate-store $staging -cfp $fingerprint `
        -d "SQL Safety Guard $version - Company copy" -fd sha256 -t $TimestampUrl -o $output
    if ($LASTEXITCODE -ne 0) { throw 'Signing failed. Do not distribute any partial output.' }
    if (-not (Test-Path -LiteralPath $output -PathType Leaf)) { throw 'Sign CLI did not create the output.' }
    $signedHash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
    if ($signedHash -eq $inputHash) { throw 'Output is unchanged; do not distribute it as signed.' }
    $signed = [IO.Compression.ZipFile]::OpenRead($output)
    try {
        if (-not @($signed.Entries | Where-Object {
            $_.FullName.Replace('\', '/') -match '(?i)^(_xmlsignatures|package/services/digital-signature)/.*\.psdsxs$'
        }).Count) {
            throw 'Output has no VSIX package signature; do not distribute it.'
        }
    }
    finally { $signed.Dispose() }
    [ordered]@{
        version = $version; unsignedSha256 = $inputHash; signedSha256 = $signedHash
        certificateSha256 = $fingerprint; signCliVersion = ($toolVersion -join ' ')
        timestampUrl = $TimestampUrl; createdUtc = [DateTime]::UtcNow.ToString('o')
        signatureChainAndTimestampVerification = 'PENDING - IT must verify before distribution'
    } | ConvertTo-Json | Set-Content -LiteralPath $receiptPath -Encoding UTF8
    Write-Host "Created company copy: $output"
    Write-Host "Receipt: $receiptPath. Verify signature, chain and timestamp with IT before distribution."
}
finally { Remove-Item -LiteralPath $staging -Force }
