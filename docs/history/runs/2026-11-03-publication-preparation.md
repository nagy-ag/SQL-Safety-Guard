> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Stable 1.0.0 publication preparation, 2026-11-03

The owner explicitly requested pushing the completed maintenance and cleanup, deleting the previous prerelease, and
publishing this version as stable 1.0.0. The numeric product version remains 1.0.0; the changelog now records the publication
date and qualifies historical server evidence. Direct stable publication follows this explicit owner instruction.

The precommit offline gate passed: 1,592 .NET tests, 10 Python tooling tests and 527 generated rules current. Local-only
guidance remains ignored; build binaries, raw run output and release archives are excluded from the commit. The
[maintenance](2026-11-03-repository-maintenance.md) and [cleanup](2026-11-03-verification-cleanup.md) reports describe the
implementation and prior checks. Owner policy questions remain pending; no classifier verdict behavior was changed.

At this preflight, `origin/main` and the starting source were both `0dbe4b4`. The only published release was the
`signing-handoff-1.0.0-1` prerelease. Its metadata and all five assets were backed up outside the repository, and every
downloaded asset matched GitHub's recorded SHA-256 digest. No old release was deleted during preflight.

The subsequent release operation must commit the reviewed source, build the unsigned assets with
`scripts/New-ReleaseAssets.ps1` from the clean commit without skipping its UI interaction checks, and publish the source
under `v1.0.0`. The release notes and asset checksums record final artifact validation. This snapshot does not claim that
those later steps have already completed. Prior acceptance reports identify their own builds; no new live SSMS routing,
theme capture, SQL execution or server campaign is claimed by this preflight.

Publication retains the extension's text-only boundary: no SQL connection opening, SQL execution or server metadata
queries. Development harnesses are separate from the installed extension. GitHub artifacts remain unsigned, contain no
signing keys, and require their documented checksums when downloaded.
