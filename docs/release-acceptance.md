# Release 1.0.0 acceptance

Updated 2026-11-03 for the published replacement 1.0.0 with the uninstall command. Accepted for the qualified checks below.
Source: `0cd57d004988cb7eb8080c89b1ee2b3136a5c0c2`, replacement tag `v1.0.0`. The
[publication record](history/runs/2026-11-03-replacement-publication.md) identifies all assets, checksums and CI evidence.

## Current checks

- Locked offline gate: 1,621 .NET tests, 10 Python tests and 527 generated rules current; Debug and Release rebuilds passed.
- Live Debug command checked in a new owned isolated SSMS profile on a private Windows desktop: real confirmation text,
  cancellation without an installer, correct installation/profile arguments, clean SSMS close and interactive removal.
- All 10 protected original profile settings/assembly hashes matched after the test; the normal and Exp installs remain.
- The confirmation defaults to No by the offline command contract. No physical keyboard input was synthesized.
- [Preflight details](history/runs/2026-11-03-replacement-preflight.md) explain driver retries and independent post-checks.
- Committed-source Release validation passed ZIP integrity, version/target, runtime files, unsigned status and path privacy.
  Independent inspection found no SQL client, test/harness tooling or Debug command channel. Release interaction checks
  passed: 13 pane, 15 confirmation and 4 settings checks; these do not establish shell rendering.
- GitHub source/tag CI passed and fresh release downloads matched all three validated local hashes. The old release/tag
  were backed up before the owner-authorized replacement. VSIX SHA-256:
  `c4068832e698f6faa9998fa7042267d115c1ca3810feed7f69c1cafae64a4ec9`.

## Scope and retained evidence

No SQL execution, server campaign or new F5 routing acceptance was performed. Production still opens no SQL connection,
executes no SQL and queries no server metadata. Pending owner policy decisions remain explicit.
The private-desktop PrintWindow capture was blank; UI Automation verified dialog text/button behavior, not rendering.
No fresh full-theme or screenshot-rendering acceptance is claimed. The October 30
[169-image acceptance](history/runs/2026-10-30-release-acceptance-final.md) describes its own earlier build and remains
historical evidence for unchanged UI surfaces. It is not fresh evidence for this replacement package.

The owner explicitly authorized retaining 1.0.0. Earlier 1.0.0 installations need uninstall/reinstall; use source SHA and
published checksums to distinguish the packages. GitHub packages remain unsigned; company signing is a separate internal copy.
