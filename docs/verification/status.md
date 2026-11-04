# Verification status

Updated **2026-11-04** after publication of the **1.0.0 SSMS 22.x compatibility update**.
Published source: `7c036b8b26850d2b44fe7a034c1334327f5b0088`, identified by replacement tag `v1.0.0` and
[the publication record](../history/runs/2026-11-04-ssms-22-publication.md). The stable release is marked Latest and targets
`main`. Its tested SQLCMD adapter and outline-free badges support **SSMS 22.0 and later 22.x releases**, 64-bit,
with manifest range `[22.0,23.0)`. The owner explicitly requested keeping version **1.0.0** and replacing the old release.
The older published source `0cd57d0`, campaign base `5e8c7f2`, frozen candidate and manual visual package retain their own
identities below. Their runtime observations are not fresh execution tests of the newly rebuilt publication artifact.
The analyzer's classification policy is unchanged. The extension still opens no SQL connections, executes no SQL and
queries no server metadata. The new maintenance command only hands off to the local SSMS installer after confirmation.

## Qualified conclusion

The [release preparation](../history/runs/2026-11-04-ssms-22-release-preparation.md) passed the locked offline gate
(**1,627 .NET / 10 Python / 527 rules**), both rebuilds, **32 Release interaction checks** and local package validation.
That local preflight VSIX SHA-256 was `b3ae436bce2e1a21abe348a63256bad9189c638e44864f56512f979cf6a7f8eb`.
All 58 C#/XAML Release source files, excluding DEBUG branches, match the visual candidate the owner tried. Metadata now
states the supported SSMS 22.x range. The final committed-source package was built with the unchanged official release
script, again passing **1,627 .NET / 10 Python / 527 rules**, the Release rebuild, **32 interaction checks** and package
validation. Fresh GitHub downloads of all three assets match the validated local hashes. Published VSIX SHA-256:
`944732eba0ff020ed61b5836b939d3e86954b887c550f2926dbbd6b71c0d3c87`.
The old release/tag/assets were backed up before replacement. New source/tag GitHub CI was queued at publication,
not yet PASS; current run links are in the publication record. No fresh installation/live trace of this exact artifact
or new server campaign was added. Current `main` includes the product update; subsequent publication-record edits are documentation only.

The owner explicitly reports **all eleven manual checklist rows and the outline check PASS** on local
**22.11.2 / 22.11.12324.101**, using visual candidate SHA-256
`d59ff032b2940c70967c860a3944751efc9e423b4ab8824ca26d81705ba81b85`.
These are user-reported observations, not an independent eleven-case SQL trace. The final retained counters were **1,1**,
rather than the sequence's expected **4,3**, so that sample does not corroborate the complete sequence. Automated newest
F5/selection/two-window gaps remain; the manual report is additional evidence, not a relabeling of failed drivers.
Owned manual database/package cleanup passed and all twenty protected profile hashes matched. No cloud resources were added.

Retained server campaigns report no unresolved reproducible analyzer defect after their triage. Known green policy effects
remain, including the tempdb opt-in and RAISERROR WITH LOG. Observation gaps, unavailable features and unverified targets
are not passes. See [policy-questions.md](policy-questions.md) and [contract.md](contract.md); owner decisions remain pending.
The following campaign narrative preserves its historical published-package identity; the new release does not erase
its failures or convert candidate/manual observations into a fresh published-package campaign.
**Focused candidate evidence now spans representative builds from 22.0 through 22.11; exhaustive and published-package
qualification remains incomplete.** The [original cloud continuation](../history/runs/2026-11-03-ssms-cloud-compatibility.md)
sampled six minor lines. Published newest **22.11.3 / 22.11.12328.437** passed eleven focused Release execution cases using
guest-injected F5 and the actual Execute toolbar; these are not human physical-key claims. Published older 22.0–22.4 strict
target cases failed with SQLCMD-unavailable warnings. The selected-red submission qualifications and oldest Cancel oracle
gap remain in that report. In that original published-package run, **22.5.2 / 22.5.11723.231** installed but VSIX installation timed out; 22.6–22.10 were not installed.

The [SQLCMD follow-up](../history/runs/2026-11-03-ssms-sqlcmd-compatibility-pilot.md) demonstrated an editor property mismatch
on **22.4.1 / 22.4.11612.150**: its captured editor exposes inherited `IsSqlCmdMode`. The unreleased candidate supports this
and the existing `IsSqlCmdModeEnabled` path while keeping missing/error/conflict unknown and retaining all target/policy
assertions. The November 3 candidate Release run passed **10/11** focused execution cases: seven injected F5, two real
toolbar and one second-window case. Its return-to-window-A case was incomplete because the helper could not find the hidden tab.
The [November 4 focused retry](../history/runs/2026-11-04-ssms-224-return-window-retry.md) passed **2/2**: window B and
return to A, with exact SQL once in the correct sessions and unchanged strict assertions. All eleven distinct focused
candidate Release cases now have passes across the two runs; this is not a new eleven-case run or full requalification.
Debug observed A-off/A-on/B-off correctly, but its separate return-A channel gap remains incomplete.
Candidate 22.11 full keyboard/two-window execution acceptance remains incomplete; the partial local runtime results below do not establish blanket compatibility.
The candidate offline gate passed 1,627 .NET / 10 Python / 527 rules and both builds. No product SQL access was added.
A later [read-only 22.11.2 metadata supplement](../history/runs/2026-11-03-ssms-sqlcmd-2211-metadata.md) found both properties
and a mode setter that updates both values, supporting one adapter. This is static evidence, not candidate 22.11 runtime PASS.
The [22.5–22.10 continuation](../history/runs/2026-11-04-ssms-225-2210-startup.md) created a new VM but stopped during IAP/SSH startup: both early
backend connection checks failed, and the helper stopped before its full readiness window. **0/6 versions were installed
or tested.** This adds no compatibility result; all owned resources were deleted and billing was unlinked.
The [partial continuation](../history/runs/2026-11-04-ssms-225-226-candidate-continuation.md) established candidate
**22.5.2 / 22.5.11723.231: 11/11 strict execution cases PASS**, with three panes/basic settings. Its 22.6 run stopped after
nine passes because the helper reused discovery prefix `SQLQuery` as an exact filename instead of `SQLQuery2.sql`.
The external-only exact-name correction passed seven offline checks and retained every ownership and SQL assertion.
The [completed focused retry](../history/runs/2026-11-04-ssms-226-2210-candidate-retry.md) then established **11/11 strict
candidate execution cases PASS on each of 22.6.0, 22.7.2, 22.8.2, 22.9.2 and 22.10.2**: **55/55** independently reviewed
from raw SQL/XE, counts/counters, intended text, context and session binding. All five also passed three representative
panes and basic settings. Exact full builds and package/archive hashes are in the report. This supplies the formerly
missing 22.6 window B/return-A checks; the original partial evidence remains unchanged. The same frozen candidate was
reused, with no production/assertion change, rebuild or repeated full gate. Each Release uninstall attempt remained
incomplete after the external command was rejected. Billing propagation was handled by a bounded read-only readiness check;
its three offline checks passed, and all startup failures remain recorded. No company build, older-minor or newest retest
was added. These focused passes do not close the overall published-package matrix.


The [earlier-minor continuation](../history/runs/2026-11-04-ssms-220-223-candidate-continuation.md) established **11/11
strict candidate execution cases PASS on each of 22.0.0 / 22.0.11205.157, 22.1.0 / 22.1.11304.174,
22.2.1 / 22.2.11415.280 and 22.3.3 / 22.3.11527.330**: **44/44** independently reviewed from raw SQL/XE, counts,
counters, text, context and owned query sessions. All four passed three semantic panes; basic settings controls passed
on 22.0–22.2, while **22.3's integrated F5 warnings settings check is INCOMPLETE**. Release uninstall remains incomplete.
The frozen candidate and live assertions were unchanged. An offline checker had assumed only the newer query application
label; it now checks exact registered application/session/PID/database identities, with nine focused checks. The initial
incomplete review and raw failures remain preserved. This adds candidate evidence for the earlier minors without
relabeling the original published-package failures. Candidate 22.11 full execution acceptance remains incomplete; local partial results follow.


The [local 22.11.2 candidate run](../history/runs/2026-11-04-ssms-2211-local-candidate.md) added runtime evidence on
**22.11.2 / 22.11.12324.101**: frozen candidate Release install/load, three semantic panes and settings passed, together
with two strict actual-toolbar whole-window checks. Cancel submitted nothing; approved Run submitted the intended SQL once
in the correct database with independent counters and raw XE confirmation. Focused Debug A-off/A-on/B-off SQLCMD readings
agreed on both properties; Debug return-A remained incomplete. Six Debug selection/pane checks and a limited unverified-target
Cancel observation passed. Injected keyboard selection failed its unchanged scope precondition, including a paced-key
attempt; therefore F5/selected execution and Release two-window acceptance remain incomplete. Raw helper failures are
preserved. One initial no-focus Debug process lingered after acknowledged Quit and required exact owned-process termination;
later sessions closed normally. All owned SQL resources and both profile packages were cleaned; 18 original protected
files retained their hashes. No cloud billing was enabled and no product change or rebuild was made.

The [local retry and badge change](../history/runs/2026-11-04-ssms-2211-local-candidate-retry.md) added **no completed
execution cases**: editor binding, a rejected public COM call, duplicate menu observations, a normal-profile isolation
guard and a disconnected-file/connected-query mismatch stopped the attempts before case execution. A corrected startup
selector was not live-proven because an overlapping user window caused refusal. Computer Use failed before attaching
with a kernel-assets path error, including after reset. Original failures remain intact. The user-authorized temporary
normal-profile payload swap was restored: 25 original extension files, both settings backups and all 20 protected baseline
hashes verified; three owned SQL database/session pairs and the isolated package were cleaned. No cloud resources were added.
The requested outline removal changes only the four numbered red/yellow badge ellipses. Candidate Debug/Release builds and
1,627 unit tests passed; at that snapshot the separately hashed visual candidate was **uninstalled and visually unverified**.
The later owner-run manual check and release preparation above supersede that visual status without changing the old
snapshot or converting the earlier failed drivers to passes. Automated local F5/selected/two-window acceptance remains incomplete.

Unknown-target, broad Debug and current Release uninstall gaps remain. Newest Light/Dark acceptance remains earlier
Debug-only preview evidence. Prior host/server/publication evidence is preserved below. All owned cloud test runs are
finalized: resources deleted, project billing unlinked, exact owned tunnels closed and the monitor disabled. Raw evidence
remains external. This run's key-deletion command was blocked by automatic approval review, so current keys and older
approval-blocked key sets remain pending manual deletion; the VM is deleted. Earlier successful key removals remain
recorded in their snapshots. The cumulative budget/account remain for delayed billing, are not a spending cap, and
actual billed cost is pending.

Earlier live Debug uninstall acceptance remains evidence for its recorded build/profile: cancellation, correct installer target, clean close and removal, with protected settings/assembly hashes preserved. It is not a new candidate or published Release result.

## Evidence ledger

| Component | Evidence and identity | Establishes | Limits |
|---|---|---|---|
| Published SSMS 22.x compatibility update 1.0.0 | [Publication](../history/runs/2026-11-04-ssms-22-publication.md), source `7c036b8`, replacement `v1.0.0`; VSIX hash above | Clean committed-source official build, full locked offline gate, 32 interaction checks, package inspection, GitHub asset digests and three fresh-download hashes; old release replaced, new stable release Latest | Source/tag CI queued at publication; exact new artifact not freshly installed/live-traced; candidate/manual scenario limits unchanged; same-version uninstall/reinstall required |
| Local 1.0.0 compatibility preparation and owner manual acceptance | [Preparation](../history/runs/2026-11-04-ssms-22-release-preparation.md); base `5e8c7f2` plus promoted adapter/badges and bounded manifest; new package hash above | 1,627 .NET / 10 Python / 527 rules; both rebuilds; 32 Release interaction checks; package/Release-source checks; user reports eleven manual rows and outline PASS; owned cleanup and 20 protected hashes | Uncommitted/unpublished preflight; final artifact not freshly installed; manual final counters 1,1 do not corroborate cumulative 4,3; no new execution trace; existing automated/unknown/uninstall/22.3 settings gaps retained |
| Local retry and badge change | [Retry](../history/runs/2026-11-04-ssms-2211-local-candidate-retry.md); original frozen candidate plus separately hashed visual build | Both visual-candidate builds and 1,627 unit tests PASS; original normal-profile bytes/settings restored; owned SQL/profile cleanup verified | Zero new execution cases; Computer Use runtime blocked; revised startup helper not live-proven; visual candidate not installed or visually accepted |
| Local 22.11.2 frozen candidate | [Local run](../history/runs/2026-11-04-ssms-2211-local-candidate.md); exact same frozen candidate packages, no rebuild | Release three panes/settings and two strict whole-window actual-toolbar cases; focused Debug A-off/A-on/B-off; limited pane/Cancel observations | F5/selected execution, Release two-window and Debug return-A INCOMPLETE; initial Debug shutdown linger; all owned SQL/profile packages cleaned, 18 protected files unchanged |
| 22.0–22.3 candidate continuation | [Completed run](../history/runs/2026-11-04-ssms-220-223-candidate-continuation.md); same frozen candidate, no product change/rebuild | Each actual22.0.0/22.1.0/22.2.1/22.3.3 strict11/11 execution PASS (44/44), three panes; basic settings22.0–22.2; independent SQL/XE/session/archive review | Unreleased candidate;22.3 integrated F5 settings INCOMPLETE; uninstall/unknown-target/broad Debug gaps; candidate22.11 runtime untested. Resources empty/billing unlinked/tunnel closed/monitor disabled; key deletion approval-blocked, actual cost pending |
| 22.5–22.10 candidate continuation | [Prior partial run](../history/runs/2026-11-04-ssms-225-226-candidate-continuation.md) and [completed retry](../history/runs/2026-11-04-ssms-226-2210-candidate-retry.md); unchanged frozen candidate | 22.5.2 prior11/11;22.6.0–22.10.2 each11/11 strict execution PASS (55/55 new), representative panes/basic settings, independent raw SQL/XE/archive/session review | Unreleased candidate; Release uninstall, deliberate unknown-target and broad Debug incomplete; candidate newest runtime untested. Earlier failures preserved. All cloud resources removed/billing unlinked/tunnel closed/current keys removed/monitor disabled; older keys and actual cost pending |
| SQLCMD compatibility candidate | [Bounded pilot](../history/runs/2026-11-03-ssms-sqlcmd-compatibility-pilot.md) and [focused retry](../history/runs/2026-11-04-ssms-224-return-window-retry.md); isolated uncommitted base `5e8c7f2`, frozen packages; prior 1,627 .NET / 10 Python / 527 rules, both builds | 22.4.1 legacy SQLCMD property diagnosed; eleven distinct strict Release cases PASS across runs (10/11 then 2/2); three semantic panes; Debug A-off/A-on/B-off | Separate Debug return-A channel gap INCOMPLETE; candidate 22.11 runtime untested; later minor results in continuation row; not published. Cloud cleanup verified; older keys/manual deletion and billing cost pending |
| Focused SSMS 22.x compatibility | [Cloud continuation](../history/runs/2026-11-03-ssms-cloud-compatibility.md) and preserved [host assessment](../history/runs/2026-11-03-ssms-compatibility.md); source `5e8c7f2`, exact published VSIX; prior offline gate 1,621 .NET / 10 Python / 527 rules | Six selected minor lines sampled; newest 22.11.3 Release 11/11 strict focused execution cases, pane/settings; Debug-only Light/Dark previews; exact build 22.5.2 also verified | Older target assertions FAIL; oldest oracle qualification; later UI and VSIX-install timeout; five minor lines untested. Unknown/uninstall/broad Debug incomplete. Cloud cleanup verified; actual billing and manual key removal pending |
| Published replacement 1.0.0 | [Publication record](../history/runs/2026-11-03-replacement-publication.md), source `0cd57d0`: full offline gate, Release rebuild, 32 interaction checks, package validation and source/tag CI passed | Exact unsigned assets validated; GitHub digests and fresh downloads matched all three hashes; previous release removed and tag replaced with a lease | No new SQL/server/F5 routing campaign or full-theme rendering review; same-version installations need uninstall/reinstall |
| Replacement 1.0.0 preflight | [Preparation and live check](../history/runs/2026-11-03-replacement-preflight.md), working tree based on `5a35399`: 1,621 .NET tests, 10 Python tests, 527 generated rules and Debug rebuild passed | Actual registered Debug command, cancellation, exact instance/profile launch, interactive removal after clean close; 10 original profile hashes preserved | No physical-key/SQL routing test or screenshot-rendering acceptance; exact Release package evidence is in the publication row above |
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

1. The 12-minor published-package matrix remains incomplete. The isolated candidate has11/11 strict execution passes on
   each22.0–22.3 representative, eleven distinct focused22.4.1 passes across two runs, prior22.5.2 eleven passes, and
   eleven passes on each22.6–22.10 representative. Newest manual eleven-row/outline acceptance is user-reported with
   a final-counter discrepancy; automated newest execution, broader Debug routing, the
   separate return-A channel gap, deliberate unknown-target and current Release uninstall checks remain incomplete;
   22.3 integrated F5 settings acceptance is also incomplete. External exact-tab and offline oracle-review corrections
   retained strict ownership/SQL assertions. No further run is scheduled. Cloud resources are deleted and billing unlinked;
   current/older approval-blocked keys and actual billed cost remain pending. Earlier failures/missing evidence are preserved.

2. The latest server report retains unavailable-feature, observation-gap and unresolved-target cases. Do not promote them.
3. Trusted procedures, allow-list effects, permanent tempdb opt-in, logging and session state retain their current behavior
   until the owner decides. This work does not relabel or relax them. Typed confirmation stays disabled.
4. New freeze/launcher execution needs a clean reviewed checkout and an explicitly authorized disposable environment.
   Original published binaries were reused; the separate candidate was built and frozen once, with external helpers kept outside packages.
5. Earlier GitHub offline CI passed for source/tag `0cd57d0`. New source/tag CI at `7c036b8` was queued at publication;
   see the linked runs in the new publication record. The committed-source release build, full local offline gate,
   interaction/package checks and fresh downloads passed; these do not establish live SSMS execution behavior.
6. Same-version replacement requires uninstall/reinstall on machines with the earlier package. Source SHA and artifact
   checksum identify the build; the unchanged 1.0.0 version alone is insufficient. The published VSIX SHA-256 is
   `944732eba0ff020ed61b5836b939d3e86954b887c550f2926dbbd6b71c0d3c87`.

For offline commands and evidence levels see [how-to.md](how-to.md). For execution entry points and prerequisites see
[the tooling map](../../tools/verification/README.md). For all current documents see [docs/README.md](../README.md).
