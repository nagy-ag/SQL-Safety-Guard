SQL Safety Guard company signing kit

GitHub downloads stay unsigned. Run this kit on the company signing computer.
No keys, certificates or third-party tool binaries are included.

1. Read SIGNING.md, including identity, certificate setup and trust instructions.
   In a source checkout the maintained guide is docs/it/signing.md.
2. Download the release's original SqlSafetyGuard-<version>.vsix and SHA256SUMS.txt.
   Keep their original filenames. The full release ZIP already contains the VSIX.
3. Check the input without accessing keys or installing tools:
   .\Sign-CompanyVsix.ps1 -VsixPath ..\SqlSafetyGuard-1.0.1.vsix -ChecksumsPath ..\SHA256SUMS.txt -CheckOnly
4. On the company computer, install the IT-approved Sign CLI version and arrange
   access to a code-signing key in the company Windows account's CurrentUser\My.
   Export only its public DER .cer. SIGNING.md explains the optional pilot setup.
5. Sign a separate copy, substituting your certificate and output paths:
   .\Sign-CompanyVsix.ps1 -VsixPath ..\SqlSafetyGuard-1.0.1.vsix -ChecksumsPath ..\SHA256SUMS.txt -PublicCertificatePath C:\CompanySigning\Public.cer -OutputPath C:\CompanySigning\signed\SqlSafetyGuard-1.0.1.vsix
6. IT verifies the exact signed VSIX's signature, signer fingerprint, trust chain
   and timestamp, then tests installation in an isolated SSMS 22 profile.
   The generated .signing.json records hashes and marks verification PENDING.
   A successful signing command is not proof of trust or completed verification.
7. Distribute the signed copy and new internal checksums through your company
   channel. Keep the original GitHub file/checksums and the signing receipt.

For a signing-only ZIP, place it in a signing subfolder beside the downloaded
VSIX and SHA256SUMS.txt, or adjust the command paths. Never upload the signed
copy, signing receipts containing internal details, or private keys to GitHub.
The kit never creates a key automatically, changes trust, installs software,
uploads files, or modifies the original VSIX. The timestamp service needs network
access when signing. Managed/HSM signing identities use IT's provider workflow;
this helper covers a local CurrentUser certificate-store key only.
