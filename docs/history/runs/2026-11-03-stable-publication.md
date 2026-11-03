> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Stable 1.0.0 publication, 2026-11-03

The owner requested pushing the completed repository work and replacing the previous prerelease with stable 1.0.0.
Source commit `98aebf619e1eab0530f6ca645b43f3cebbc2c9c6` was pushed to main and tagged `v1.0.0` (annotated tag).
The [stable release](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.0) was published and verified as latest,
with prerelease=false and draft=false. This documentation record follows that exact release source without changing it.

## Validation and assets

`scripts/New-ReleaseAssets.ps1` completed from the clean source commit without skipping UI checks:

- 1,592 .NET tests and 10 Python tooling tests passed; all 527 generated rules were current.
- Locked Release solution rebuild succeeded. All 13 pane, 15 confirmation and 4 settings interaction checks passed.
- ZIP CRCs, manifest identity/version/SSMS target, required files, unsigned status and local-profile-path checks passed.
- Independent content inspection found only expected extension/runtime/resource/legal files and parser localization
  assemblies. No test assemblies, SQL client, campaign tooling or Debug command channel was present. The optional
  scripts ZIP contained only install.ps1, uninstall.ps1 and scripts/SsmsCommon.ps1.
- Uploaded asset digests matched local hashes. Fresh downloads from the published release matched all three assets.
- GitHub offline CI passed for the [main push](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41617921192)
  and the [tag push](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41618110577), both testing the release source.

| Asset | SHA-256 |
|---|---|
| SqlSafetyGuard-1.0.0.vsix | `2b3c848cbf13b3a32c61f282c0bb4e02358a3ca2dbfb5f363a657f54787e868a` |
| SqlSafetyGuard-1.0.0-scripts.zip | `421ff4ce86bc998aed6d729964dc444869c785e0c66359abfab93f3d63731596` |
| SHA256SUMS.txt | `9a12bffe9a0d89f92f14c19cfbc7be9bb55ed7aad9cd6342986aa0a43e5776dd` |

Automatic MSBuild discovery failed on this machine. The release script was rerun successfully with its supported
`-MSBuildPath` argument pointing to the installed Visual Studio MSBuild. Earlier staged whitespace issues in new
documents/checksum manifests were normalized in the release source; all retained evidence matched committed Git blobs.

## Replacement and limits

The former `signing-handoff-1.0.0-1` prerelease was deleted after stable publication. Its metadata and all five assets were
backed up outside the repository and matched GitHub's digests before deletion. Its historical Git tag was retained.
The final release list contained stable v1.0.0 only. No signing key was used or distributed; GitHub assets remain unsigned.

Local release validation, backups and fresh downloads are retained outside the repository under
`../release-out/publication-1.0.0-20261103-121149/`. No build archive or raw execution log was committed.
No SQL, new live SSMS session, installation, full-theme capture or server campaign was performed for this publication.
Historical acceptance/server reports still describe their own builds. Pending owner policy decisions are unchanged.
The extension opens no SQL connection, executes no SQL and queries no server metadata.
