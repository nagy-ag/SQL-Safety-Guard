# Verification status

Updated **2026-11-03** for the owner-authorized replacement **1.0.0**, including the in-extension uninstall command.
Preflight work is based on `5a35399`; committed-source package/publication evidence follows the release build.
The analyzer's classification policy is unchanged. The extension still opens no SQL connections, executes no SQL and
queries no server metadata. The new maintenance command only hands off to the local SSMS installer after confirmation.

## Qualified conclusion

Retained server campaigns report no unresolved reproducible analyzer defect after their triage. Known green policy effects
remain, including the tempdb opt-in and RAISERROR WITH LOG. Observation gaps, unavailable features and unverified targets
are not passes. See [policy-questions.md](policy-questions.md) and [contract.md](contract.md); owner decisions remain pending.
This uninstall-command verification executes no SQL and supplies no fresh evidence of live SSMS routing or server effects.
Live Debug uninstall acceptance verified cancellation, the correct installer target, clean SSMS close and completed removal
in an owned isolated profile. Other profiles' protected settings/assembly hashes matched their pre-run baseline.

## Evidence ledger

| Component | Evidence and identity | Establishes | Limits |
|---|---|---|---|
| Replacement 1.0.0 preflight | [Preparation and live check](../history/runs/2026-11-03-replacement-preflight.md), working tree based on `5a35399`: 1,621 .NET tests, 10 Python tests, 527 generated rules and Debug rebuild passed | Actual registered Debug command, cancellation, exact instance/profile launch, interactive removal after clean close; 10 original profile hashes preserved | No physical-key/SQL routing test or screenshot-rendering acceptance; Release package/publication checks follow committed build |
| Initial uninstall implementation | [Provisional-version record](../history/runs/2026-11-03-in-extension-uninstall.md): offline targeting/cancellation and prior local builds | Initial no-server-access and package checks | Snapshot predates the owner's decision to retain 1.0.0 and the live acceptance above |
| Original published 1.0.0 | [Original publication record](../history/runs/2026-11-03-stable-publication.md), source `98aebf6`: full offline gate, Release rebuild, 32 UI checks, package validation and both GitHub CI runs passed | Original unsigned assets and downloads validated | Historical identity; the owner authorized replacement, so do not identify those old assets by tag name alone |
| 1.0.0 publication preflight | [November 3 preparation](../history/runs/2026-11-03-publication-preparation.md): 1,592 .NET tests, 10 Python tests and 527 generated rules current | Offline source validation before committing and building release assets | Final package validation is recorded with GitHub release assets; no new live SSMS/server campaign |
| Current verification cleanup | [November 3 cleanup](../history/runs/2026-11-03-verification-cleanup.md): 1,592 .NET tests, 10 Python tests and 527 generated rules current | Maintained launcher callers and retained safety/interaction contracts after removing cosmetic constraints | Uncommitted working tree; no fresh VSIX build, server or live SSMS execution |
| Repository maintenance | [November 3 maintenance](../history/runs/2026-11-03-repository-maintenance.md): 1,592 .NET tests, 10 Python tests, both builds and 32 UI interaction checks | Source organization, tool contracts, generated references and production access-boundary checks | Earlier working tree; production source unchanged by subsequent verification cleanup |
| Release UI/build acceptance | [October 30 acceptance](../history/runs/2026-10-30-release-acceptance-final.md), commit `2848b4f` | Reported 1,574 tests, both builds, 32 Release interaction checks and 169 reviewed theme images | Historical build; no SQL executed |
| Analyzer runtime campaign | [October 30 full run](../history/runs/2026-10-30-full-run.md), recorded analyzer `34630d62` / main `4388bc2` | Recorded execution/observation results with single-worker triage and explicit policy effects | Oracle limits, permissions/platform gaps and external interference remain |
| Guard command routing | [October 28 original evidence](../history/2026-10-28-extension/README.md) | Reported 35 PASS, zero FAIL, one BLOCKED in 36 scenarios for that Debug build | Physical keys were not synthesized; toolbar and other paths remain limited |

The [previous status narrative](../history/runs/2026-10-30-status-before-maintenance.md) preserves prior counts, fixed-defect
history, campaign exclusions and observation gaps. Reports describe their recorded binaries, not the current working tree.

## Maintenance scope

- Added a confirmed per-user uninstall handoff bound to the running SSMS installation/profile; no guard/settings changes.
- Explicit harness modes; legacy discovery requires an explicit disposable server.
- Rejected campaign/isolated inputs stop before execution; extension results distinguish failure from incomplete evidence.
- Debugger launch uses an isolated profile. Visual-capture settings resolve the installed instance rather than a machine ID.
- Test associations use Roslyn declarations and qualified identities. Stable rule IDs and verdicts remain unchanged.
- Campaign freezing rebuilds from clean source with SDK/build provenance; generated cases keep their own principals.
- Measured reports require new external outputs and explicit measurement metadata. Historical evidence is hash-protected.
- Pure guard types and existing test classes have discoverable files. Current architecture and task guidance are separate.
- SDK feature-band policy, package locks and an offline-only workflow make ordinary checks reproducible.
- Removed obsolete Wave3 launcher aliases and the retired Wave2 registration stub. Tests call maintained launchers directly;
  UI contracts retain safety and interaction assertions without fixing decorative glyphs, fonts or exact pixel sizes.

## Open limits and decisions

1. No new live guard/server campaign has been run for this maintenance. Offline checks cannot prove host behavior.
2. The latest server report retains unavailable-feature, observation-gap and unresolved-target cases. Do not promote them.
3. Trusted procedures, allow-list effects, permanent tempdb opt-in, logging and session state retain their current behavior
   until the owner decides. This work does not relabel or relax them. Typed confirmation stays disabled.
4. New freeze/launcher execution needs a clean reviewed checkout and an explicitly authorized disposable environment.
   No existing scheduled task or external frozen bundle was changed.
5. GitHub offline CI passed for both the source push and `v1.0.0` tag at `98aebf6`. Release build/interaction and package
   validation passed locally; these do not establish live SSMS execution behavior.
6. Same-version replacement requires uninstall/reinstall on machines with the earlier package. Source SHA and artifact
   checksum identify the build; the unchanged 1.0.0 version alone is insufficient. Release validation/publication is pending.

For offline commands and evidence levels see [how-to.md](how-to.md). For execution entry points and prerequisites see
[the tooling map](../../tools/verification/README.md). For all current documents see [docs/README.md](../README.md).
