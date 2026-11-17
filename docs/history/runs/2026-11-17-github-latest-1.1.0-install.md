> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# GitHub Latest 1.1.0 download and local installation

On 2026-11-17 the owner requested downloading the published extension from GitHub, testing that exact package locally,
and making it the extension in the normal SSMS profile. GitHub's Latest-release API resolved to stable, non-draft
[`v1.1.0`](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.1.0) before and after installation.

## Verified result

- Fresh downloads of the standalone VSIX and `SHA256SUMS.txt` match GitHub asset digests; the VSIX also matches the
  checksum file. VSIX SHA-256: `872b5cf22ae53ca26c941394eb9b6cede069c922414e898fca7d389b35c92978`.
- The downloaded analyzer passes eight offline cases: read, temporary write, permanent write, unknown procedure,
  variable-backed dynamic write, deferred procedure body, deferred function body and USE context. No SQL is executed.
- The exact VSIX installs into a fresh owned SSMS 22.11 profile. All 24 payload files match the extracted download;
  all six extension setting values survive first-profile initialization. SSMS adds shell initialization data separately.
- Live UI Automation on an undisplayed private desktop observes `Whole window` and the six controls with expected
  counts: Writes 1, Unknown 2, Safe 1, Inside 2, Context 1, Remote 1. The script includes a definition with two inner
  operations, an unknown call, a read, USE and a remote command. The test never connects or executes it.
- The owned test session closes cleanly; its pre-launch settings are restored byte for byte. No shared desktop input
  or foreground focus is used.
- Native installation upgrades the owner's normal-profile 1.0.5 build to published 1.1.0, then refreshes the extension
  configuration. Its manifest is 1.1.0 and all 24 payload files match the GitHub download. The previous payload and
  settings are backed up outside the repository; normal-profile settings remain byte-identical. SSMS remains closed.

## Scope and retained evidence

This is a download/install and focused live-load check, not a new F5 routing, SQL execution, full theme or server campaign.
The [release verification](../releases/1.1.0.md) and [stable promotion](2026-11-17-release-1.1.0-stable.md) remain frozen.
No production code or published asset is changed. Raw downloads, hashes, installer receipts, UI Automation names,
settings backups and representative-case results remain outside the repository.
