# Release 1.0.0 acceptance

Updated 2026-11-03 for the owner-authorized replacement 1.0.0 with the uninstall command. Preflight is based on
`5a35399`; committed-source package and publication identities are recorded in [verification/status.md](verification/status.md).

## Current checks

- Locked offline gate: 1,621 .NET tests, 10 Python tests and 527 generated rules current; Debug rebuild passed.
- Live Debug command checked in a new owned isolated SSMS profile on a private Windows desktop: real confirmation text,
  cancellation without an installer, correct installation/profile arguments, clean SSMS close and interactive removal.
- All 10 protected original profile settings/assembly hashes matched after the test; the normal and Exp installs remain.
- The confirmation defaults to No by the offline command contract. No physical keyboard input was synthesized.
- [Preflight details](history/runs/2026-11-03-replacement-preflight.md) explain driver retries and independent post-checks.
- Release build, packaged contents, interaction checks and fresh GitHub download validation follow the committed build.

## Scope and retained evidence

No SQL execution, server campaign or new F5 routing acceptance was performed. Production still opens no SQL connection,
executes no SQL and queries no server metadata. Pending owner policy decisions remain explicit.
The private-desktop PrintWindow capture was blank; UI Automation verified dialog text/button behavior, not rendering.
No fresh full-theme or screenshot-rendering acceptance is claimed. The October 30
[169-image acceptance](history/runs/2026-10-30-release-acceptance-final.md) describes its own earlier build and remains
historical evidence for unchanged UI surfaces. It is not fresh evidence for this replacement package.

The owner explicitly authorized retaining 1.0.0. Earlier 1.0.0 installations need uninstall/reinstall; use source SHA and
published checksums to distinguish the packages. GitHub packages remain unsigned; company signing is a separate internal copy.
