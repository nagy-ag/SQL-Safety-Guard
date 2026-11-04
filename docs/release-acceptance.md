# Release 1.0.0 acceptance

Updated **2026-11-04** for the locally prepared **1.0.0 SQLCMD compatibility update**. It supports **SSMS 22.0 and later
22.x releases**, 64-bit; the manifest is bounded to `[22.0,23.0)`. This package has not been committed, signed, tagged,
pushed or published. The existing GitHub 1.0.0 remains the older package recorded in the
[November 3 publication record](history/runs/2026-11-03-replacement-publication.md).

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
- Local VSIX SHA-256: `b3ae436bce2e1a21abe348a63256bad9189c638e44864f56512f979cf6a7f8eb`.
  Assembly SHA-256: `d0239723debc6cdb419c57c26c12a9cfaf1e27db536f1cc2a873fa5781752451`.
  The [preparation snapshot](history/runs/2026-11-04-ssms-22-release-preparation.md) identifies source, scripts and packages.

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

`New-ReleaseAssets.ps1` still requires a clean committed checkout. Its guard was preserved; the uncommitted local preflight
is not clean-source release-script acceptance. Publication, source/tag CI and fresh-download hash checks remain pending.

The owner explicitly requested retaining **1.0.0**. Earlier 1.0.0 installations need uninstall/reinstall; use the exact
package checksum to distinguish them. GitHub packages remain unsigned; company signing is a separate internal copy.
