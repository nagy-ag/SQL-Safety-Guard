> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Replacement 1.0.0 publication, 2026-11-03

The owner explicitly requested publishing the uninstall-capable build to main, deleting the previous release and retaining
version **1.0.0**, with no changelog file. Published source is `0cd57d004988cb7eb8080c89b1ee2b3136a5c0c2`.
The annotated `v1.0.0` tag now identifies that source; its tag object is `b380c687de59992e33f7514cac66cffd1a121fb7`.
This documentation record follows publication without changing product code or the released artifacts.

## Validation

- The clean-source release script passed the full locked offline gate: 1,621 .NET tests, 10 Python tests and 527 generated
  rules current. Its Release rebuild, 13 pane, 15 confirmation and 4 settings interaction checks all passed.
- The preceding Debug rebuild and [live isolated uninstall acceptance](2026-11-03-replacement-preflight.md) passed with
  the implementation committed in this source. Actual cancellation, correct installer arguments, clean close and completed
  removal were observed; all 10 original protected profile hashes matched independently.
- Release validation checked ZIP CRCs, version/identity/SSMS target, required runtime/legal files, unsigned status and local
  user-path privacy. Independent inspection found no SQL client, test/harness tooling or Debug command channel in the VSIX.
  The optional scripts ZIP contains exactly install.ps1, uninstall.ps1 and scripts/SsmsCommon.ps1.
- GitHub offline CI passed for the [main source push](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41629415810)
  and the [replacement tag push](https://github.com/nagy-ag/SQL-Safety-Guard/actions/runs/41629511428), both at the source above.
- The new release was verified as stable, non-draft and latest. GitHub's asset digests and fresh downloads matched all three
  local validated files. The release list contained only [SQL Safety Guard 1.0.0](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.0).

| Asset | SHA-256 |
|---|---|
| SqlSafetyGuard-1.0.0.vsix | `c4068832e698f6faa9998fa7042267d115c1ca3810feed7f69c1cafae64a4ec9` |
| SqlSafetyGuard-1.0.0-scripts.zip | `421ff4ce86bc998aed6d729964dc444869c785e0c66359abfab93f3d63731596` |
| SHA256SUMS.txt | `1414841d8536e03b2de61dcaac5d3b03f1f55c3b39d9f9d3eaf6b2eb363079a0` |

## Replacement and recovery

Before deletion, the original release metadata and all three assets were saved outside the repository and their SHA-256
digests matched GitHub. A verified Git bundle preserves the original tag object `ba6d30715a527fcdea13c37222ff0a8e4f9c34c0`
and its complete history, including original release source `98aebf619e1eab0530f6ca645b43f3cebbc2c9c6`.
The old release object (432539884) was deleted and replacement release 432619670 created. The tag update used a lease for
only the old v1.0.0 tag object; main was pushed normally. No other branch, tag, worktree or historical evidence was removed.

Assets, fresh downloads, metadata, CI receipts, the old tag bundle and the uninstall driver/post-check remain outside the
repository under `../release-out/replacement-1.0.0-20261103-131829/`. No raw output or binary archive was committed.
Existing installations of the earlier 1.0.0 need uninstall/reinstall; source SHA and package checksum distinguish this build.

The changelog file and its current references were removed as requested. User/deployment/signing guidance, agent navigation
and mirrored extension skills were updated; version consistency now checks package/assembly metadata directly.
Production still opens no SQL connection, executes no SQL and queries no server metadata. Analyzer verdict policy and
pending owner decisions are unchanged. No new SQL/server/F5 routing campaign, signing or full-theme rendering review is
claimed. The private-desktop confirmation capture was blank; live text/button behavior was checked with UI Automation.
