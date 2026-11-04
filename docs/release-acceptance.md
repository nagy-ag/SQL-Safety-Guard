# Release 1.0.0 acceptance

Updated **2026-11-04** for the published **1.0.0 SQLCMD compatibility update**, source
`7c036b8b26850d2b44fe7a034c1334327f5b0088`, replacement tag `v1.0.0`. It supports **SSMS 22.0 and later 22.x releases**,
64-bit, bounded to `[22.0,23.0)`. The owner authorized updating `main` and replacing the previous release. The new
unsigned stable release is Latest. See the [publication record](history/runs/2026-11-04-ssms-22-publication.md).
The [November 3 publication](history/runs/2026-11-03-replacement-publication.md) is preserved as historical evidence.

## Current checks

- Promoted the tested target-owned SQLCMD adapter and six regressions into the main source. Both host property names
  are accepted; unavailable, malformed, throwing or conflicting state still fails closed. Analyzer policy is unchanged.
- Removed black outlines only from the four numbered red/yellow pane badges. The owner reports all eleven manual
  checklist rows and the outline check passed on **22.11.2 / 22.11.12324.101** with the separate visual candidate.
  This is user-reported acceptance, not an independently traced eleven-case result; retained counters qualify it below.
- Locked offline gate: **1,627 .NET tests, 10 Python tests, 527 generated rules current**; Debug and Release rebuilds PASS.
- Release interaction checks: **13 pane, 15 confirmation, 4 settings PASS**; these do not establish live shell rendering.
- Local preflight reused the exact release-script ZIP/CRC, required-file, identity/version/target, unsigned and path-privacy
  validations. Independent binary inspection found the adapter and no SQL client references or checked Debug probe/channel
  methods. All **58 C#/XAML Release source files**, excluding DEBUG branches, match the manually tested visual candidate.
- Published VSIX SHA-256: `944732eba0ff020ed61b5836b939d3e86954b887c550f2926dbbd6b71c0d3c87`.
  Assembly SHA-256: `d0239723debc6cdb419c57c26c12a9cfaf1e27db536f1cc2a873fa5781752451`.
  The [preparation snapshot](history/runs/2026-11-04-ssms-22-release-preparation.md) identifies the earlier local preflight;
  its different package hash is not the publication artifact. The official script then passed all checks from clean
  committed source. GitHub digests and fresh downloads of all three new assets matched the validated local hashes.

## Scope and retained evidence

The frozen compatibility candidate has focused execution evidence on one representative of every 22.x minor through
22.11. Earlier 22.0–22.10 strict SQL/XE results, the two-run 22.4 qualification, local 22.11 toolbar/Debug evidence and
the later user manual report are distinct evidence levels; see [verification/status.md](verification/status.md).
The final local artifact was rebuilt with narrower manifest metadata and was not freshly installed or live-traced.
Manual counters sampled before cleanup were **1,1**, rather than the checklist's cumulative **4,3**; the retained sample
does not independently corroborate all eleven rows. It does not invalidate or replace the owner's reported observations.
Deliberate ambiguous-target acceptance, broader Debug routing, current Release in-extension uninstall handoff and 22.3
integrated settings gaps remain. Standalone VSIXInstaller removed the manual profile package; this is separate from that handoff.

No server campaign, full-theme pass or live execution run was repeated during packaging. Production still opens no SQL
connection, executes no SQL and queries no server metadata. All twenty protected profile hashes were unchanged after
removing the owned manual database/package. Earlier cloud cleanup evidence remains unchanged; no cloud resource was created.

`New-ReleaseAssets.ps1` still requires a clean committed checkout. Its guards were preserved and the official script
passed from committed source before publication. New source/tag CI was queued at publication; it is not yet a PASS.
The source/tag/release/assets and all earlier snapshots were preserved externally before the authorized replacement.

The owner explicitly requested retaining **1.0.0**. Earlier 1.0.0 installations need uninstall/reinstall; use the exact
package checksum to distinguish them. GitHub packages remain unsigned; company signing is a separate internal copy.
