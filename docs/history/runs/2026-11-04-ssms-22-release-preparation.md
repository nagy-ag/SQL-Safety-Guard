> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Release 1.0.0 SSMS 22.x compatibility preparation — November 4

The owner requested **version 1.0.0** containing the compatibility code from the completed SSMS minor-version tests,
plus removal of the black outlines from the numbered red/yellow badges. This preparation promotes that adapter into the
main working source and produces local unsigned assets. No commit, push, tag replacement or publication was performed.
The existing published 1.0.0 is unchanged and remains identified by its [publication record](2026-11-03-replacement-publication.md).

Public compatibility wording: **Supports SQL Server Management Studio 22.0 and later 22.x releases**, 64-bit.
The VSIX target is `[22.0,23.0)`; future major versions are outside this support claim. One representative build from
each released minor through 22.11 has focused evidence, not every historical patch or every host interaction.

## Source and packages

Base commit: `5e8c7f20c33c611b93846465602f1cd15669c4f2`, with an uncommitted source overlay retained externally.
The target capture now calls `TargetSqlCmdModeReader.TryRead(editor)`. It accepts the captured editor's current
`IsSqlCmdModeEnabled` or inherited legacy `IsSqlCmdMode` boolean, rejects inconsistent/malformed/throwing state,
and never substitutes global command state. Six existing candidate regressions were promoted. No analyzer, SQL
connection, execution, server-metadata or target-identity policy was added or relaxed. Diagnostic pilot probes were
not promoted. Four numbered badge ellipses explicitly set `StrokeThickness="0"`; other outlines are unchanged.

All **58 C#/XAML Release source files** match the manually tested visual candidate after DEBUG branches and line endings
are excluded. This is a source comparison, not an assertion of binary identity or a new live run. Manifest description,
the SSMS-major upper bound and its matching release-validator expectation are the additional packaging changes.

| Artifact | SHA-256 |
|---|---|
| Frozen minor-campaign candidate VSIX | `c09c435244c7daa923ed2e13a78f48e9ae52758645ef30abb9787e107a438ecc` |
| Outline-free manual candidate VSIX | `d59ff032b2940c70967c860a3944751efc9e423b4ab8824ca26d81705ba81b85` |
| Prepared `SqlSafetyGuard-1.0.0.vsix` | `b3ae436bce2e1a21abe348a63256bad9189c638e44864f56512f979cf6a7f8eb` |
| Prepared Release `SqlSafetyGuard.dll` | `d0239723debc6cdb419c57c26c12a9cfaf1e27db536f1cc2a873fa5781752451` |
| Prepared `SqlSafetyGuard-1.0.0-scripts.zip` | `421ff4ce86bc998aed6d729964dc444869c785e0c66359abfab93f3d63731596` |
| Promoted adapter source | `809c907b483118b68e129794ced65a8235cc252cb93a07097366bc8292cd63bc` |

The scripts ZIP contains only `install.ps1`, `uninstall.ps1` and `scripts/SsmsCommon.ps1` with that layout.
`SHA256SUMS.txt` identifies both distribution files. Raw logs, manual ownership, source overlay, checks and provenance
remain outside the repository; the source overlay includes the two new files as well as the tracked diff.

## Retained host coverage

These rows summarize earlier immutable runs; packaging did not rerun them. The strict cloud cases independently checked
intended SQL, submission count, counter change, database/context and owned query-session binding. Guest-injected F5,
actual toolbar clicks, Debug-only observations and later human observations remain separate evidence levels.

| SSMS display version | Full build | Compatibility-candidate evidence |
|---|---|---|
| 22.0.0 | 22.0.11205.157 | 11/11 strict Release cases PASS |
| 22.1.0 | 22.1.11304.174 | 11/11 strict Release cases PASS |
| 22.2.1 | 22.2.11415.280 | 11/11 strict Release cases PASS |
| 22.3.3 | 22.3.11527.330 | 11/11 strict Release cases PASS; integrated settings INCOMPLETE |
| 22.4.1 | 22.4.11612.150 | All 11 distinct cases have passes across two runs: 10/11 then 2/2 |
| 22.5.2 | 22.5.11723.231 | 11/11 strict Release cases PASS |
| 22.6.0 | 22.6.11806.211 | 11/11 strict Release cases PASS |
| 22.7.2 | 22.7.11919.86 | 11/11 strict Release cases PASS |
| 22.8.2 | 22.8.12023.21 | 11/11 strict Release cases PASS |
| 22.9.2 | 22.9.12120.119 | 11/11 strict Release cases PASS |
| 22.10.2 | 22.10.12217.157 | 11/11 strict Release cases PASS |
| 22.11.2 | 22.11.12324.101 | Frozen candidate: 2 strict toolbar cases and separate Debug observations; visual candidate: owner reports 11 manual rows and outline PASS |

Details: [early minors](2026-11-04-ssms-220-223-candidate-continuation.md),
[22.4 pilot](2026-11-03-ssms-sqlcmd-compatibility-pilot.md), [22.4 return-A retry](2026-11-04-ssms-224-return-window-retry.md),
[22.5 and partial 22.6](2026-11-04-ssms-225-226-candidate-continuation.md),
[22.6–22.10](2026-11-04-ssms-226-2210-candidate-retry.md), [local 22.11](2026-11-04-ssms-2211-local-candidate.md)
and [outline change/local retry](2026-11-04-ssms-2211-local-candidate-retry.md).
The original published-package 22.11.3 strict passes belong to another package and are not relabeled here.

The owner explicitly confirmed all eleven manual checklist rows and the outline check passed. That statement is retained
as user-reported acceptance. The later database sample was 1,1, rather than the checklist's cumulative 4,3; there was no
independent execution trace for this manual run. The owner chose to proceed with the completed-campaign code and badge
change without further investigation. Automated local keyboard/selection/two-window gaps and earlier raw failures remain.
Deliberate unknown/ambiguous-target, broader Debug routing and current Release in-extension uninstall-handoff coverage
are incomplete. This is focused compatibility evidence, not a blanket safety guarantee.

## Preparation checks and cleanup

- Locked offline gate: **1,627 .NET tests / 10 Python tests PASS; 527 generated rules current**.
- Debug and Release solution rebuilds with locked restore: **PASS**.
- Release interaction checks: **13 pane + 15 confirmation + 4 settings PASS**; no desktop input or SQL execution.
- Exact release-script package-validation block: **PASS** for ZIP CRCs and entry safety, identity/version/target,
  required runtime/registration/license files, unsigned status and local-path privacy.
- Independent binary inspection: 28 ZIP members; adapter present; no SQL client reference or checked Debug probe/channel
  methods, test scripts or SQL helper payloads. This is artifact inspection, not a live load/execution check.
- Official `New-ReleaseAssets.ps1` clean-tree guards are unchanged. A separate external helper reused its validation and
  writing blocks after the recorded builds. The assets are an **uncommitted local preflight**; clean-commit acceptance,
  publication, source/tag CI and fresh public-download verification remain pending.
- Manual database ownership token and installed payload hash were verified before cleanup. The first drop correctly
  refused an active pooled observer session. The external cleanup helper then used a nonpooled connection; no guard
  assertion was relaxed. The exact owned database and isolated package were removed; standalone VSIXInstaller exit 0.
- All **20 protected original profile hashes matched**. Owned profile/settings evidence was retained; the normal profile
  was not replaced during this preparation. No cloud VM or billing link was created. Earlier cloud cleanup records and
  pending delayed charges remain unchanged.

Prior history snapshots were retained byte-for-byte. Final documentation-index and preservation receipts are kept with
the external preparation evidence. No commit, push or publication was performed. Existing 1.0.0 installations require
uninstall/reinstall and checksum comparison to distinguish the update.
