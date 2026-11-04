> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS SQLCMD compatibility pilot — November 3, 2026

The unreleased candidate removes the SQLCMD-unavailable warning on tested **SSMS 22.4.1 / 22.4.11612.150** without changing
classification or execution-target safety assertions. **Ten of eleven focused Release execution cases passed; the last case
is incomplete.** The candidate has not been run on 22.11 or the remaining minor lines. It is not a replacement release or a
claim of compatibility across SSMS 22.x. The original published-package failures remain unchanged in the
[earlier cloud report](2026-11-03-ssms-cloud-compatibility.md).

## Diagnosis and narrow candidate

The read-only Debug probe observed `IsSqlCmdMode` on `ScriptAndResultsEditorControl`, a base class of the captured
`SqlScriptEditorControl` in the 22.4 host (`SqlEditors` assembly version 22.200.0.0). It changed from false to true when
SQLCMD mode was enabled. The published guard attempted `IsSqlCmdModeEnabled`; absence of that property produced the
fail-closed warning. This establishes an internal editor API mismatch on this anchor, not absence of SQLCMD functionality.
It does not establish the private API of every older minor line.

Microsoft's [SQLCMD editor documentation](https://learn.microsoft.com/en-us/ssms/scripting/sqlcmd-scripts-query-editor)
and [SSMS 22 release notes](https://learn.microsoft.com/en-us/ssms/release-notes-22) provide public feature context; they do
not document this private property contract. A release-note change to publishing SQLCMD variables does not establish when
query-editor SQLCMD mode first existed. The concrete property diagnosis comes from the retained runtime probe.

The candidate reads only boolean properties `IsSqlCmdModeEnabled` and `IsSqlCmdMode` on the already captured editor and
its base types. Missing, malformed, throwing or conflicting properties return unknown. It preserves the existing warning
for unknown state. It does not use global active-window state, fields or command status as execution fallback; diagnostics
recorded global status only for comparison. Existing frame/view/owner/selection/connection-label checks and analyzer policy
remain. No product SQL connection, execution or metadata query was added.

## Recorded results

| Evidence level | Actual result | Qualification |
|---|---|---|
| Candidate Release installation/load | Fresh owned profiles displayed all three representative pane contents | Functional pane evidence; raw managed-module enumeration remained INCOMPLETE |
| Candidate Release pane semantics | Green SELECT, yellow unknown procedure, red permanent UPDATE observed | Settings button present; no new settings-edit, uninstall or theme acceptance claimed |
| Candidate Release injected F5 | 7/7 strict cases PASS | Selected red/yellow Cancel and Run, whole-window Cancel and Run, selected green; exact SQL/count/counter/context/target checks retained |
| Candidate Release real Execute toolbar | Red Cancel and Run PASS | UI Automation invoked the actual owned toolbar, not command dispatch |
| Candidate Release second query window | Window-B Run PASS | Exact intended SQL once, expected counter and verified target/context |
| Candidate Release return to window A | INCOMPLETE | External helper: `Owned query document tab is not unique.` No assertion was relaxed |
| Candidate Debug target-owned SQLCMD state | A-off=false, A-on=true, B-off=false; captured document and focused owner matched | Return-A channel unconsumed; subsequent switching and restore samples missing |
| Initial diagnostic Debug probe | Same first three mode observations; return-A incomplete | Original failure retained; one bounded tab-selection correction did not resolve it |
| Candidate newest 22.11 runtime regression | NOT TESTED | Pilot stopped before another installer after strict gate remained incomplete |
| Remaining minor campaign / 22.5 VSIX hang | NOT RESUMED / UNRESOLVED | No speculative installer or editor retries |

F5 input was Windows `SendInput` inside the owned guest, not human physical keypresses. There was no Debug channel in the
Release execution path. Cancel produced zero attributed SQL batches and unchanged counters; approved Run produced the
intended batch once. The external oracle bound the owned process, query-session ID and recognized SSMS application name.
No UIA text-provider crash retry was used. The two-view helper cached an editor identity only after tab ownership checks;
its return-A refusal remains incomplete, not product PASS or a demonstrated product routing failure.

Raw helper summaries contain inherited wording saying "Released VSIX". Their scenario package fields, payload hashes and
this report identify the actual **unreleased candidate Release build**. Raw files were preserved without correction.
Controller JSON array-shape aggregation also failed; local adjudication read the individual raw cases, retained ten passes
and the missing eleventh case. No later controller completion was treated as scenario success.

## Combined twelve-minor ledger

Published results below are historical and unchanged. "Not installed" does not mean unavailable. Candidate coverage exists
only on 22.4.1.

| Representative | Actual full build | Original published VSIX | Unreleased candidate |
|---|---|---|---|
| 22.0.0 | 22.0.11205.157 | Panes PASS; strict target FAIL; oldest oracle qualification | Not tested |
| 22.1.0 | 22.1.11304.174 | Panes PASS; strict target FAIL; later cases incomplete | Not tested |
| 22.2.1 | 22.2.11415.280 | Panes PASS; strict target FAIL; later cases incomplete | Not tested |
| 22.3.3 | 22.3.11527.330 | Panes PASS; strict target FAIL; later cases incomplete | Not tested |
| 22.4.1 | 22.4.11612.150 | Panes PASS; strict target FAIL; later cases incomplete | 10/11 execution PASS; return-A incomplete |
| 22.5.2 | 22.5.11723.231 | Installed; VSIX installation timed out | Not tested |
| 22.6.0 | Not installed | Not tested | Not tested |
| 22.7.2 | Not installed | Not tested | Not tested |
| 22.8.2 | Not installed | Not tested | Not tested |
| 22.9.2 | Not installed | Not tested | Not tested |
| 22.10.2 | Not installed | Not tested | Not tested |
| 22.11.3 | 22.11.12328.437 | 11/11 focused Release execution PASS | Not tested |

Unknown/ambiguous-target, broad Debug routing and current Release uninstall gaps in the earlier report remain. Light/Dark
acceptance remains the earlier Debug-only preview evidence; it was not repeated or promoted to Release evidence here.

## Source, packages and local checks

Candidate source is an isolated uncommitted change based on `5e8c7f20c33c611b93846465602f1cd15669c4f2`; no commit, push,
publication or replacement of host SSMS occurred. The source patch and separately frozen new adapter/test files identify
the candidate. Product changes remain in that isolated checkout. Main release code/assets were preserved.

| Artifact | SHA-256 |
|---|---|
| Candidate Release VSIX | `c09c435244c7daa923ed2e13a78f48e9ae52758645ef30abb9787e107a438ecc` |
| Candidate Debug VSIX | `1e1f7d2fa959b45888a98ce49a07fc6d7190c1e1553a930798562e09f88b5832` |
| Candidate Release extension DLL | `b2facfc2d1113194a1cd83d90676e767af1610ca36df92159181bf2424d01af0` |
| Candidate tracked-source patch | `eec3142b5a6b82c0a2f603523b439ecf5e3bfe7da42998501ad0ed05a0699612` |
| Added adapter source | `809c907b483118b68e129794ced65a8235cc252cb93a07097366bc8292cd63bc` |
| Added regression-test source | `61dbbc9dcc8f87d64107c7798ff3d27bdf0f2349b7a7e1fc7c505e7b65c9db82` |
| Candidate anchor raw archive | `96651bdb1f9ce7171fb68a974c0e497b0cb1f7bf8c393de3528fbb7fa5567d67` |
| Initial diagnostic raw archive | `9f1d3d3433dbd0c68269d0e9efcb9f904cd71a0efbbbd0936418d714571b890ba` |

The candidate offline gate passed once: **1,627 .NET tests, 10 Python tests, 527 generated rules**; Debug and Release builds
passed. Six added cases cover current/private inherited legacy properties, agreeing/conflicting aliases, missing/malformed/
throwing state, and independent opposite editor states with no cache. Release packaging inspection found no test helpers or
Debug probe method metadata. Frozen packages were reused; the analyzer/server campaign was not repeated. Documentation-only
validation after the report/status/index edits is recorded in the final external receipt.

## Timing, evidence and cleanup

The historical campaign starts remain 13:33:19 UTC and 16:07:26 UTC. This authorized follow-up bootstrap began
18:49:23.2944887 UTC; recovery did not reset it. Guest setup installed SQL Express 17.0.1000.7 and the exact 22.4.1 build.
The installer returned 3010 and a normal guest-only restart was completed. The reused VM name had a new resource ID;
the follow-up auto-delete deadline stayed 20:47:36 UTC. Tests stopped at 19:51 UTC on the bounded readiness gate. The
original approximate four-hour scope had already been exceeded by setup/recovery; remaining versions were not silently
expanded into another campaign.

Raw evidence, immutable failures, helper sources, logs, package/source hashes and cleanup receipts remain outside the
repository in the pilot workspace, including `candidate-anchor-22.4.1.zip`, its expanded copy,
`final-pilot-adjudication.json`, `candidate-local-verification.json` and the package/source manifests. Original host/cloud
evidence and SSD contents were preserved. The archive was copied and SHA-verified before deleting the VM.

Cloud cleanup was verified at **19:56:13 UTC**: no VM, disks, snapshots, reserved addresses, owned network, subnet or firewall
remain. Project billing is unlinked. The exact owned tunnel process/tree was closed and its listener verified absent;
the monitor is disabled. The billing account and existing cumulative $5 budget/$4.50 alert remain for delayed reporting;
the budget was not reset and is not a spending cap. Actual billed cost remains pending. No paid test resources remain.
The first cleanup invocation stopped on gcloud's informational stderr after successful VM deletion; the preserved helper
correction checked native exit codes and completed the remaining idempotent cleanup. No product code was involved.

Automatic approval review rejected the SSH-key deletion command with "blocked by policy." New pilot keys and the earlier
owned recovery/SSD key sets therefore remain for manual deletion; no bypass was attempted. Both test VMs have been deleted,
so their instance access is gone. Host input/default SSMS/Docker/default cloud configuration and other agents were preserved.

The smallest useful next scope is to repair and verify the external return-to-A ownership/channel readiness in a bounded
session, then validate this frozen candidate on newest 22.11 before resuming other minors. The 22.5 installer blocker also
needs resolution before a sequential remaining-minor campaign. No new infrastructure or automatic retries are scheduled.
