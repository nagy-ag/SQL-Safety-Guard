# Signing the VSIX on a company laptop

Build and test the extension first, then sign the finished VSIX on the company signing laptop. The two computers can be
different. The signing laptop does not need the source code or Visual Studio build workloads; it needs Sign CLI, its
prerequisites, and access to the signing private key. SSMS 22 is needed wherever the final installation smoke test runs.

This guide describes an internal pilot with a self-signed certificate. If IT supplies a company-issued certificate or managed
signing service, use that identity and its key-access instructions instead of creating the pilot certificate below.

## Privacy and identity

- The example commands contain no passwords, tokens or private-key material. Running certificate creation generates a
  private key in the selected Windows user's key storage. Keep that key on the company signing laptop.
- The Windows account controls access to the key. The certificate's subject identifies the signer to recipients. Logging in
  with a company account does not turn a self-signed certificate into a verified company identity.
- The public `.cer`, its fingerprint and the signed VSIX can be shared with recipients. Certificate subject/organization
  fields are visible to recipients, so use the agreed company/product identity rather than a personal name or email.
- `NonExportable` prevents ordinary private-key export. Copying the `.cer` to another computer or logging in there with the
  same company account does not transfer the signing key. If the original key is lost, a new certificate/key is needed.
- Do not create the pilot key on a personal laptop expecting to move it later. Create it under the intended company Windows
  account on the signing laptop. A software non-exportable key is not the same as a hardware-protected signing service.
- Signing does not remove build metadata. This project includes PDB symbols in the VSIX; symbols can contain source paths.
  Review the packaged contents separately if excluding personal paths is a distribution requirement.

## One-time setup on the company signing laptop

Use PowerShell under the company Windows account that will sign future releases. Switching to a different administrator
account also switches `CurrentUser` and can make the key unavailable.

Install Sign CLI with the .NET 8 SDK or later and the Microsoft Visual C++ 14 runtime available. Install a specific version
selected and tested by the release owner from the [official package page](https://www.nuget.org/packages/sign).

```powershell
$ErrorActionPreference = 'Stop'
$signVersion = Read-Host 'Exact Sign CLI package version selected for this release process'
if ([string]::IsNullOrWhiteSpace($signVersion)) { throw 'A package version is required.' }
dotnet tool install --global sign --version $signVersion
if ($LASTEXITCODE -ne 0) { throw 'Sign CLI installation failed.' }
sign --version
if ($LASTEXITCODE -ne 0) { throw 'Sign CLI is not available; check the .NET tools PATH.' }
sign code certificate-store --help
if ($LASTEXITCODE -ne 0) { throw 'Certificate-store signing is not available.' }
```

If Sign CLI is already installed, check its version and help; use `dotnet tool update --global sign --version <version>`
when a version change is intended. Record the tested version with the release evidence.

For a pilot only, create the following certificate **once**, then reuse it for subsequent pilot packages. Replace the example
subject with the company-approved pilot name before running. A self-signed name is a claim, not independent identity proof.

```powershell
$ErrorActionPreference = 'Stop'
$signingFolder = Join-Path $env:USERPROFILE 'Documents\SqlSafetyGuardSigning'
$cerPath = Join-Path $signingFolder 'SqlSafetyGuard-Pilot.cer'
New-Item -ItemType Directory -Path $signingFolder -Force | Out-Null
if (Test-Path -LiteralPath $cerPath) {
    throw 'Pilot certificate file already exists. Reuse the existing key; do not recreate it for each release.'
}

$cert = New-SelfSignedCertificate `
    -Type CodeSigningCert `
    -Subject 'CN=SQL Safety Guard Company Pilot TEST' `
    -FriendlyName 'SQL Safety Guard Company Pilot TEST' `
    -CertStoreLocation 'Cert:\CurrentUser\My' `
    -KeyAlgorithm RSA `
    -KeyLength 3072 `
    -HashAlgorithm SHA256 `
    -KeyUsage DigitalSignature `
    -KeyExportPolicy NonExportable `
    -NotAfter (Get-Date).AddYears(1)

Export-Certificate -Cert $cert -FilePath $cerPath -NoClobber | Out-Null
$fingerprint = (Get-FileHash -LiteralPath $cerPath -Algorithm SHA256).Hash
Write-Host "Public certificate: $cerPath"
Write-Host "SHA256 certificate fingerprint: $fingerprint"
Write-Host "Windows thumbprint: $($cert.Thumbprint)"
Write-Host "Private key available: $($cert.HasPrivateKey)"
$cert.EnhancedKeyUsageList | Select-Object FriendlyName, ObjectId
```

The expected code-signing EKU is `1.3.6.1.5.5.7.3.3`. The exported certificate is DER-encoded public data; hashing that file
with SHA-256 produces the fingerprint accepted by `-cfp`. Do not substitute the usual Windows SHA-1 thumbprint for `-cfp`.
If certificate export fails after key creation, recover/export the existing certificate from `CurrentUser\My` rather than
blindly rerunning creation. Plan certificate renewal and key custody with IT before this pilot key expires or the laptop is
replaced.

## For each release

### 1. Build and test on the development computer

Follow [release-process.md](release-process.md) for versioning, tests, the Release build and acceptance checks. The build output
is `src\SqlSafetyGuard.Vsix\bin\Release\SqlSafetyGuard.vsix`.

Freeze that tested build. Copy it to an `unsigned` staging folder as `SqlSafetyGuard-<version>-unsigned.vsix`, using the actual manifest
version. Record its SHA-256 hash and source revision in the release evidence. Transfer the unsigned package through the
company-approved channel and compare its SHA-256 hash on the signing laptop with the value recorded on the build computer.
For an independently authenticated comparison, convey the expected hash through an authenticated channel as well.

Only the finished package needs to move to the signing laptop. No private key needs to move to the development computer.
Source code and build tools are needed on the company laptop only if it will also perform the build.

### 2. Sign on the company laptop

Place the received file in `Documents\SqlSafetyGuardSigning\unsigned`. Run as the company account that owns the private key.
Set `$version` to the version actually built; `1.0.0` below is an example. This does not change the manifest version.

```powershell
$ErrorActionPreference = 'Stop'
$version = '1.0.0' # Replace with the version in the built VSIX manifest.
$signingFolder = Join-Path $env:USERPROFILE 'Documents\SqlSafetyGuardSigning'
$cerPath = Join-Path $signingFolder 'SqlSafetyGuard-Pilot.cer'
$vsix = Join-Path $signingFolder "unsigned\SqlSafetyGuard-$version-unsigned.vsix"
$outputFolder = Join-Path $signingFolder 'signed'
$signedVsix = Join-Path $outputFolder "SqlSafetyGuard-$version.vsix"

if (-not (Test-Path -LiteralPath $vsix -PathType Leaf)) { throw "VSIX not found: $vsix" }
if (-not (Test-Path -LiteralPath $cerPath -PathType Leaf)) { throw "Certificate not found: $cerPath" }
if (Test-Path -LiteralPath $signedVsix) { throw "Output already exists: $signedVsix" }

$publicCert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($cerPath)
$storeCert = Get-Item -LiteralPath ("Cert:\CurrentUser\My\" + $publicCert.Thumbprint)
if (-not $storeCert.HasPrivateKey) { throw 'This account does not have the signing private key.' }
if ((Get-Date) -lt $storeCert.NotBefore -or (Get-Date) -ge $storeCert.NotAfter) {
    throw 'Signing certificate is outside its validity period.'
}
$fingerprint = (Get-FileHash -LiteralPath $cerPath -Algorithm SHA256).Hash
New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null

sign code certificate-store $vsix `
    -cfp $fingerprint `
    -d "SQL Safety Guard $version - Company Pilot" `
    -fd sha256 `
    -t 'http://timestamp.acs.microsoft.com/' `
    -o $signedVsix

if ($LASTEXITCODE -ne 0) { throw 'VSIX signing failed. Do not distribute the output.' }
if (-not (Test-Path -LiteralPath $signedVsix -PathType Leaf)) { throw 'Signed output was not created.' }
Get-FileHash -LiteralPath $signedVsix -Algorithm SHA256
```

The timestamp requires network access. A timestamp attests to signing time; it does not establish trust in our certificate.
If using an IT-issued certificate, use its public certificate file and the key location/provider specified by IT instead;
the pilot example assumes a key in `CurrentUser\My`.

### 3. Verify and distribute the signed output

1. Verify the VSIX signature, signer fingerprint, certificate chain and timestamp with a VSIX-capable verification tool agreed
   with IT. A zero exit code and the existence of an output file do not replace signature verification. Do not assume
   `Get-AuthenticodeSignature` is a VSIX package verifier.
2. Smoke-test the exact signed output in an isolated SSMS 22 profile using the
   [extension UI workflow](../.agents/skills/ssms-extension-ui/SKILL.md). Check installer certificate/signature details and the
   expected pilot trust state, then pane, green/yellow/red execution guards, settings and upgrade/uninstall behaviour.
   Use VSIXInstaller for this check; manual extraction does not exercise its signature checks.
3. Generate the release `SHA256SUMS.txt` from the final signed VSIX and the other final release assets. This VSIX hash differs
   from the unsigned input hash. Keep both hashes, the signer fingerprint and verification results in release evidence
   outside the repository.
4. Distribute the signed output and final checksums. Recipients do not need Sign CLI or the signing private key. IT can use
   the public `.cer` and an independently checked fingerprint to configure pilot trust under company policy.

Do not rebuild or edit package contents after signing. A rebuild produces a new artifact that must be signed and verified
again; editing signed contents invalidates the signature. Renaming/copying the finished file without changing its bytes is
fine. Our installer scripts are separate release assets: signing the VSIX does not sign `install.ps1` or `uninstall.ps1`.

## Trust and production use

The pilot scripts create the certificate in the Personal store; they do not configure trusted roots or trusted publishers.
Expect an untrusted signer until IT establishes the required trust on pilot machines. Have IT deploy the appropriate trust
configuration rather than adding root certificates indiscriminately. Windows and company policy can affect installation.

For wider internal deployment, prefer an IT-issued code-signing certificate with centrally managed trust and key custody.
For public distribution, use a publicly trusted signing identity appropriate to the distribution channel. Microsoft states
that Visual Studio Marketplace does not accept VSIX packages signed with self-issued certificates.

Signing provides evidence of origin and detects changes to signed contents. It does not certify SQL analysis correctness
or replace the project's safety tests and release acceptance.

## References

- [Microsoft: signing VSIX packages](https://learn.microsoft.com/en-us/visualstudio/extensibility/signing-vsix-packages?view=visualstudio)
- [Microsoft: Sign CLI reference for VSIX](https://learn.microsoft.com/en-us/visualstudio/extensibility/dotnet-sign-cli-reference-vsix?view=visualstudio)
- [Sign CLI package and prerequisites](https://www.nuget.org/packages/sign)
- [Microsoft: New-SelfSignedCertificate](https://learn.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2025-ps)
- [Microsoft: Export-Certificate](https://learn.microsoft.com/en-us/powershell/module/pki/export-certificate?view=windowsserver2025-ps)
