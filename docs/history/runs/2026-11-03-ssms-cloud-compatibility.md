> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Focused SSMS 22.x cloud compatibility assessment — November 3, 2026

## Qualified result

Compatibility across all twelve released minor lines remains **incomplete**. Six minor lines have exported test results;
a seventh has an exact installed build and an incomplete VSIX-installation attempt. Five selected lines were not installed.
The newest build passed the eleven focused Release execution cases. Older sampled builds warned because their SQLCMD state
could not be verified, failing the strict verified-target assertions. This was fail-closed behavior; the selected-red
submission observations below do not turn those failed cases into passes. No product code, behavior or assertion changed.

| Selected build | Actual installed full build | Released pane/load evidence | Released execution and limits |
|---|---|---|---|
| 22.0 | 22.0.0 / 22.0.11205.157 | Three static panes PASS | Four strict cases FAIL; old application-name filter gap; retrospective selected-red Run qualification only; Cancel total-zero-submission INCOMPLETE |
| 22.1 | 22.1.0 / 22.1.11304.174 | Three static panes PASS | Four strict cases FAIL; sampled F5/toolbar Cancel 0, Run 1 intended batch; remaining seven cases INCOMPLETE |
| 22.2.1 | 22.2.1 / 22.2.11415.280 | Three static panes PASS | Four strict cases FAIL; sampled F5/toolbar Cancel 0, Run 1 intended batch; remaining seven cases INCOMPLETE |
| 22.3.3 | 22.3.3 / 22.3.11527.330 | Three static panes PASS | Four strict cases FAIL; sampled F5/toolbar Cancel 0, Run 1 intended batch; remaining seven cases INCOMPLETE |
| 22.4.1 | 22.4.1 / 22.4.11612.150 | Three static panes PASS | Four strict cases FAIL; sampled F5/toolbar Cancel 0, Run 1 intended batch; remaining seven cases INCOMPLETE |
| 22.5.2 | 22.5.2 / 22.5.11723.231 | INCOMPLETE: Release VSIX installation timed out | No execution cases completed |
| 22.6.0 | Not installed | INCOMPLETE | Not tested |
| 22.7.2 | Not installed | INCOMPLETE | Not tested |
| 22.8.2 | Not installed | INCOMPLETE | Not tested |
| 22.9.2 | Not installed | INCOMPLETE | Not tested |
| 22.10.2 | Not installed | INCOMPLETE | Not tested |
| 22.11.3 | 22.11.3 / 22.11.12328.437 | Three static panes PASS | 11/11 focused Release execution cases PASS; unknown-target setup and uninstall handoff INCOMPLETE |

Selection used one available Microsoft-signed patched bootstrapper per released minor line from Microsoft's
[SSMS release history](https://learn.microsoft.com/en-us/ssms/release-history). The company-build requirement was dropped
by the owner. Runtime versions were read from installed catalog/file metadata; no newer minor was silently substituted.
The five untested lines were not demonstrated unavailable: the campaign stopped before attempting them.

## Scope, identity and environment

The tested source was `5e8c7f20c33c611b93846465602f1cd15669c4f2`; its product/testing trees match replacement release source
`0cd57d004988cb7eb8080c89b1ee2b3136a5c0c2`. Frozen Release and Debug packages were reused throughout.

| Artifact | SHA-256 |
|---|---|
| Published Release VSIX | `c4068832e698f6faa9998fa7042267d115c1ca3810feed7f69c1cafae64a4ec9` |
| Release extension DLL | `18ec1a84ff3450160a26207f952e274a6f9960ecd36514d30537d1c1e109a213` |
| Analyzer DLL | `1bfc25f15f1170b5f72ac39628edce396770890412af811de5659358e2194f4c` |
| Debug VSIX | `b168ec36e44325abf2e075dc6824dc5f69d04a13b98c11adaf2804e1a9224385` |
| Debug extension DLL | `ed9b86b8083d19010cb306ac3a8b9785adff786ebedd28ef2b44c69d1b7fde2e` |

The unchanged source's offline gate had already passed once: 1,621 .NET tests, 10 Python tests and 527 generated rules.
No per-version rebuild, analyzer/server campaign or repeated offline gate was run. Documentation changes receive only
the four DocsIndexTests; their result is retained in the external finalization receipt.

Host VM prerequisites were previously blocked as recorded in the preserved [host assessment](2026-11-03-ssms-compatibility.md).
The cloud continuation proved an owned Windows Server 2022 Desktop VM with 4 CPUs, 16 GB RAM, an SSD boot disk and a clean
snapshot. The newest run used the first VM lifetime; the clean snapshot was restored to the replacement VM for sequential
22.0 through 22.5 testing. This is Windows Server guest evidence, not Windows 11 laptop validation. The guest-only SQL
Express 2025 instance used Windows authentication, disabled TCP/named pipes and disposable owned database/XE objects.
No production extension SQL connection, server access or metadata query was added. The host desktop, default SSMS,
Docker workloads, default cloud configuration and other agents' environments were left untouched.

The SSD interruption ended the original local controller. Recovery to a separate local workspace preserved the original
remaining-sequence start of **16:07:26 UTC**. The SSD later returned healthy; newest/oldest archive hashes matched their
original receipts, and the newest archive was copied and verified in the recovery workspace. Controller retries did not
reset that clock. The cloud VM window began **13:33:19 UTC**; it exceeded the four-hour soft target, with installation and
automation limits reported during monitoring. No alternate virtualization campaign or open-ended UI debugging followed.

## What the evidence demonstrates

- On 22.11.3, seven Release cases cover selected red/yellow Cancel and Run, whole-window Cancel and Run, and selected green
  without a dialog. Four more cover the real Execute toolbar Cancel/Run, a second query window and return to the first.
  SQL text, exact batch count, counter deltas, context and verified-target requirements passed. F5 was Windows SendInput
  inside the owned guest; toolbar used actual UIA Invoke. Neither is a human physical keypress or Debug command-dispatch claim.
- On 22.1–22.4, selected-red Cancel submitted zero observed batches and changed no counters; Run submitted exactly one
  intended batch with the expected counter/context. The SQLCMD-unavailable warning still makes every recorded strict case
  FAIL. An eight-second readiness wait found two visible editor views after Run, blocking later cases. That single bounded
  helper correction did not solve the issue; no further editor experiments or assertion weakening followed.
- On 22.0, the initial oracle's application-name filter missed the older application's batches. A separate retrospective
  comparison establishes one intended selected-red Run batch and expected counter change. The correctly filtered immediate
  pre-Cancel baseline is missing, so total-zero-submission Cancel remains INCOMPLETE. Raw failures were retained.
- Release functional green/yellow/red pane controls establish loading in the successful owned profiles using frozen package
  hashes. Raw `Process.Modules` managed-DLL enumeration remains INCOMPLETE; it was not relabeled PASS. Static panes do not
  establish every live-edit update transition. Settings loaded on newest, 22.0, 22.1 and 22.2; settings discovery on 22.3/22.4
  and all 22.5 settings checks are INCOMPLETE.
- Release registered uninstall cancellation passed on 22.0–22.2 through the actual DTE command. Installer handoff/removal
  remains INCOMPLETE. Menu lookup failures and rejected second DTE calls are preserved. These calls prove no F5/toolbar path.
- Newest Light/Dark pane and confirmation previews were visually reviewed using the frozen **Debug-only** package, without
  SQL; theme settings were restored. Four newest Debug text-only selection checks passed. Broader Debug routing, deliberate
  unknown-target setup and later connection-identity checks were blocked. Newest Release ambiguity probes did not establish
  their intended ambiguous target. An earlier Debug Cancel observation had two unretained batches and remains unresolved.
  None of these gaps is a PASS or a confirmed false-green product defect.

## Stop condition and retained failures

The first 22.5.2 installation returned 8006. Its own setup log identified a running guest .NET optimization process; that
process exited naturally before inspection. One bounded retry installed the requested build. The subsequent Release VSIX
installation timed out, leaving its installer-created update-configuration process tree. The controller stopped before
another upgrade. No second campaign retry was launched. Owned windows were asked to close; subsequent inspection found
no SSMS/VSIXInstaller processes. Export then verified no owned SQL database/XE objects remained.

This separates an observed installation/automation timeout from a diagnosed product defect. It also leaves 22.6–22.10
explicitly untested. Further useful work would require a fresh, separately scoped assessment of the SQLCMD compatibility
path and installer timeout; this report makes no fix or compatibility promise for them.

## Evidence retention and cleanup

Raw archives, initial failed driver attempts, process identities, installer logs, oracles, cleanup receipts and the generated
`compatibility-matrix.json` remain outside the repository. Archive transfer hashes matched guest/original export receipts.
The newest archive and all later decisive archives are retained in the recovery workspace; the original SSD content is preserved.

| Archive | SHA-256 |
|---|---|
| newest-evidence-final.zip | `75ffd609b265ce54e5a7dea81a8851fcf3b65a93f298f3ac96232a2a833663ce` |
| evidence-22.0.zip | `ded61575da300271d74c193911bb202a3be06d32a59d4a258d048d32f4b4bd39` |
| evidence-22.1.zip | `b862fd35df94dbcd205eb62c6a85639f4c85264527c2de0108ae91d9725784dc` |
| evidence-22.2.1.zip | `ec865ee84bfdd78f8d8ae270cf71582b77f89f1a135e6941df35b226cd2b94d6` |
| evidence-22.3.3.zip | `5a53a3c59040088148923e623032ffde8a9d9e13854e6d326cc13535bed96b39` |
| evidence-22.4.1.zip | `90d68d1040ed6e403b5e7c71cc71aed30afdf249d54e4583d7939a796ba51db9` |
| evidence-22.5.2.zip | `e3b098005341b6fcf0d99b205785a4679e07f5ea629c636c4efb595c95f56e7e` |
| final-diagnostics.zip | `cf299a0b399a595b2140d1288267ab6f94fd400a97d1ec24b2f33bb3c10f747f` |
| installer-8006-original.zip | `ee8b7f0be79285b48b1e0147ea4129cc211dea1a79cbc402c238e097d11d7e24` |

Final cleanup verified empty VM, disk, snapshot and reserved-address inventories and removal of the owned custom network,
subnet and firewall. The test project's billing was unlinked. The new billing account and $5 budget/$4.50-spent alert remain
for delayed charges; a budget is not a spending cap, and actual billed cost and alert delivery are not yet established.
The exact owned IAP process tree was closed, and the monitor disabled. Automatic approval review blocked deletion of the
owned recovery and original SSD SSH key files with “blocked by policy”; those files remain for manual removal. The deleted
VM no longer accepts them. Public-key/host-key provenance and evidence are retained; no user SSD contents were removed.
No commit, push or publication was performed.
