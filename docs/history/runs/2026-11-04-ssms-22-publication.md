> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.x compatibility update 1.0.0 publication — November 4

The owner explicitly authorized updating GitHub `main`, publishing this compatibility update as **1.0.0**, making it
Latest and deleting the older release. The tested SQLCMD adapter and four outline-free numbered badges were committed
as **`7c036b8b26850d2b44fe7a034c1334327f5b0088`** and pushed to `main` without a force push. The replacement annotated
tag `v1.0.0` points to that commit. Later `main` publication-record changes are documentation only.

The new stable release is [SQL Safety Guard 1.0.0](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.0),
release ID **434155448**, published **2026-11-04T16:17:13Z**, target branch `main`, draft false, prerelease false, Latest.
Only one release remains. Supported hosts: **SSMS 22.0 and later 22.x releases**, 64-bit, manifest `[22.0,23.0)`.
This is representative-minor compatibility evidence, not every-patch or future-major acceptance.

## Source and checks

The [preparation record](2026-11-04-ssms-22-release-preparation.md) explains the source promotion, all representative
builds, frozen candidate hashes, user-reported manual acceptance and remaining runtime limitations. The final source
retains the captured-editor SQLCMD aliases with missing/error/conflict unknown, the unchanged strict target checks and
analyzer policy, and no production SQL connection, execution or metadata access. Diagnostic pilot probes were not promoted.
All 58 C#/XAML Release source files, excluding DEBUG branches, match the accepted visual candidate. No live SSMS, SQL,
server campaign, full-theme check or cloud resource was added during publication.

From the clean committed checkout, the unmodified clean-tree guards of `scripts/New-ReleaseAssets.ps1` passed:

- Locked offline gate: **1,627 .NET tests, 10 Python tests PASS; 527 generated rules current**.
- Release solution rebuild with locked restore: **PASS**. Both configurations had also passed during preparation.
- Release interaction checks: **13 pane, 15 confirmation, 4 settings PASS**; these exclude shell rendering/live execution.
- VSIX ZIP CRC/entry safety, required files, identity/version/SSMS target, unsigned status and local-path privacy: **PASS**.
- Independent binary inspection: 28 ZIP members, target adapter present, no SQL client reference or checked Debug
  probe/channel/capture methods or test SQL/scripts. No test tooling or raw output was included in the package.
- GitHub asset digests and **fresh downloads of all three assets** matched the validated local hashes below.

| Published asset | Bytes | SHA-256 |
|---|---:|---|
| `SqlSafetyGuard-1.0.0.vsix` | 3516477 | `944732eba0ff020ed61b5836b939d3e86954b887c550f2926dbbd6b71c0d3c87` |
| `SqlSafetyGuard-1.0.0-scripts.zip` | 5762 | `421ff4ce86bc998aed6d729964dc444869c785e0c66359abfab93f3d63731596` |
| `SHA256SUMS.txt` | 193 | `a741287de5250f34bab795980c65f2c1599a4124d5a26b9371f7d25fa07c871c` |

Release `SqlSafetyGuard.dll`: `d0239723debc6cdb419c57c26c12a9cfaf1e27db536f1cc2a873fa5781752451`.
The earlier uncommitted preflight VSIX has its own hash; it was not uploaded as the committed-source artifact.

At publication, the new GitHub offline CI jobs were **queued, not PASS**:
[main source](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41865251895) and
[replacement tag](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41865501017).
These run links show subsequent status; later outcomes belong in the maintained verification status. Local build/gate
success and the earlier publication's CI success do not substitute for results from these new jobs.

## Replacement and retained evidence

Before replacement, the old release body/metadata, tag object, all four old assets and a verified source-history Git
bundle were saved outside the repository. Each old asset matched its GitHub SHA-256 digest. The old release ID was
**432619670**, source `0cd57d004988cb7eb8080c89b1ee2b3136a5c0c2`, annotated tag object
`b380c687de59992e33f7514cac66cffd1a121fb7`; old VSIX SHA-256
`c4068832e698f6faa9998fa7042267d115c1ca3810feed7f69c1cafae64a4ec9`.

Only `v1.0.0` was moved, using a force-with-lease against that exact old tag object. The new annotated tag object is
`82d43ebc88f637b32ca6f51a8967166c625928b9`. The exact old release ID was deleted. The new release was staged as a draft,
and all three uploaded digests were checked before publication as Latest. A by-tag REST lookup did not return the draft;
the existing draft was found in the release inventory and read/published by its exact ID. No duplicate release was created.
The old extra all-in-one ZIP asset was not retained on the new release; the VSIX and optional scripts ZIP are explicit assets.

External publication evidence retains the old-release backup, official build log, source/tag/asset receipts, release body,
fresh downloads/hashes, binary/source inspection and documentation/preservation receipts. Existing history snapshots were
not rewritten. The working source, raw evidence and user SSD contents remain intact. Earlier cloud cleanup and delayed
billing/key-removal limits are unchanged.

The release remains unsigned. Same-version users must uninstall/reinstall and compare the package checksum. The new
artifact was not freshly installed or live-traced. The candidate's scenario, settings, unknown-target, broader Debug and
Release in-extension uninstall-handoff gaps remain documented; publication does not turn them into passes.
