> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 to 22.11.3 diagnostic retry — November 6

**INCOMPLETE: the baseline stopped on native settings observation, before the host upgrade.** Initial SSMS
**22.9.2 / build 22.9.12120.119** and unchanged published **SQL Safety Guard 1.0.3** were installed. Five pane states
and eleven baseline execution records have independently corroborated raw evidence. They do not qualify the baseline:
six native settings values and the final profile/file comparison were not established. The phase reviewer returned
**0/11, INCOMPLETE**; 22.11.3, first launch after update and restart survival were not tested. No extension regression
or false green was demonstrated. The [first November 6 retry](2026-11-06-ssms-229-2211-upgrade-retry.md),
[November 5 attempt](2026-11-05-ssms-229-2211-upgrade-attempt.md) and blocked preparation remain unchanged.

## Inputs and diagnostic scope

The owner requested another attempt with better logging. The controller's original start was **07:13:12.9674917 UTC**;
that clock was preserved throughout. No product source or binary was changed, rebuilt or republished. Release source is
`30aec84e40758b28da85378e57b5dff3f6b0d7c9`; existing build/offline gates retain their
[publication identity](2026-11-05-release-1.0.3-publication.md).

| Input | SHA-256 |
|---|---|
| Published 1.0.3 VSIX | `0ee37ae4847caf31544ee4a9f686e3c10a6f72a55d3970859c338e6e4f4f69dc` |
| Release DLL, also in initial installed inventory | `97b90388dc8d604d8f6b80c411727693432f1b229d06077964f3f0fa8c5b81f2` |
| Microsoft 22.9.2 bootstrapper | `3826637010859431029327f8cb8d63608166da459bd5ca3da5ad524c80892270` |
| Selected Microsoft 22.11.3 bootstrapper; not run | `2c1ae4f6bd3aeb7314282ef5c94abee6aa2d68a2cbfd4b8825438f8542f66a36` |

Both bootstrapper signatures and frozen hashes were checked before launch. The disposable Windows Server 2022 guest
used SQL Express 2025 **17.0.1000.7**, setup exit 0. SSMS installation took approximately 12.85 minutes, returned
**3010**, and its normal requested restart was honored before baseline testing. The released VSIX was installed once
into a new isolated profile. Its 27 member hashes match the frozen VSIX and the initial 31-file inventory; six seeded
settings were recorded. This is an initial inventory, not proof of later preservation or native settings readback.

External helpers gained process/parent/start/path/command/window snapshots, exception stacks, ten-second guest
heartbeats and verified CIM process disappearance during clean shutdown. Exact profile ownership was required;
unknown retained processes would stop the run. Initial installer descendant cleanup was permitted only after installer
exit 0, profile refresh and member-hash verification; this run recorded no such stop. Eight SSMS shutdowns were verified
absent by CIM, and nine quiet-stage checks completed. Seven transient UI Automation observations found already-closed
windows; subsequent verified process absence establishes closure, not a retained-process failure.

Before launch, external pane expectations were corrected for the released allowed-settings UI, and PowerShell 5 JSON
array wrapping was corrected. Offline positive/negative fixtures, shutdown identity simulations, shared-open-log
exports, same-request SSH retries and script parsing passed. SQL/XE/count/counter/context/target assertions were retained.
No prepare generator was rerun. Host desktop/input/clipboard, local SSMS, Docker, default cloud configuration and
unrelated resources were left alone; all execution and input occurred in the disposable guest.

## Exact failure and evidence

All four baseline helper processes exited 0, but their result evidence was inspected rather than treating exits as
success. The settings helper recorded **“Native setting control not unique: AllowList”**. Its raw native UI dump contains
two occurrences each of the `AllowList`, `TrustedProcedures` and `EnvironmentRules` edit controls. The helper searches
all owned windows and their descendants; repeated traversal is a plausible explanation. The dump lacks UIA RuntimeId
and ancestor identity, so it cannot distinguish repeated enumeration from distinct controls. It also lacks captured
values. The failure does not establish lost settings or an extension defect.

The wrapper retained the error **“Native settings values/loading not established”**, including script stack and source
line, at **07:52:12 UTC**. Execution helpers had run, but the later settings acceptance gate rejected the phase before
the end-of-phase profile capture. `native-values.json` and `profile-Capture.json` are absent. The independent reviewer
ran once at **07:52:55 UTC**, verified the archive/build, then stopped on the missing profile capture. It was not rerun
or overridden. The controller correctly refused the host update and exported evidence before cleanup.

Separate postmortem inspection of the existing raw files established the following qualified observations:

| Observation | Evidence |
|---|---|
| Five pane states | SELECT safe header; unknown procedure/body not analyzed; permanent UPDATE/no WHERE; explicit allowed-settings section and correct object/reason for allow-list and trusted procedure |
| Seven guest-injected F5 cases | Selected red Cancel/Run, selected yellow Cancel/Run, whole-window Cancel/Run, selected green |
| Four toolbar/two-window cases | Real Execute toolbar Cancel/Run, window B Run, return to window A Run |
| Cancel behavior | Zero new submissions and zero counter deltas for every Cancel |
| Run behavior | Exactly one intended submission per Run; red `[1,0,0]`, procedure `[0,1,0]`, whole window `[1,1,1]`, window B `[0,0,1]`; selected green `[0,0,0]` |
| Independent server oracle | Before/after batch prefixes, exact normalized SQL, successful result, PID/application/session/database attribution, complete non-dropped/non-truncated XE and all three counters agree for all eleven records |
| Displayed context/target | Captured dialog scope, server/database text, owned query context and expected SQL hashes agree with the server evidence; no target warning |

These eleven consistent execution observations are **baseline-only**, not eleven accepted upgrade cases. Formal phase
acceptance remains 0/11 because required settings/file evidence is missing. UIA could not read editor selection text;
paired Run SQL on the server provides external text evidence. No Release internal guard hash is available. Guest
SendInput F5 is not human physical-key coverage. SQLCMD-on and deliberate ambiguous-target execution remain explicit
gaps, and Windows Server 2022 does not establish Windows 11 acceptance. No repair, cache refresh, reinstall or settings
rewrite was performed after a host upgrade; no host upgrade occurred.

## Export integrity and cleanup

Both archive hashes, every extracted member and all manifest lengths/hashes were independently checked after completion.
The manifests report complete exports with no copy errors; active logs are bounded initial-length snapshots.

| Export | SHA-256 | Verified members / manifest files |
|---|---|---|
| Baseline | `9b1a32fc8a31a3fa73dc70f706779f72528d310b7847d851b8dba847296f54c9` | 222 / 221 |
| Final | `ecd884821b7207623a22b884778e83315cc2749b7c927b5391eff3255ae3d556` | 255 / 254 |

The VM's absolute auto-delete was **10:14:35 UTC**, with execution/export reserve from **09:49:35 UTC** and cleanup
required by **09:59:35 UTC**. None was extended. Successful pre-unlink inventories at **07:55:05 UTC** show no owned
VM, disk, snapshot, address, custom network, subnet or firewall. The controller finalized INCOMPLETE at **07:55:08 UTC**.
Fresh billing readback confirms the project is unlinked; post-unlink Compute queries were not used as fresh cleanup
proof. The exact owned IAP process tree was identity-checked and stopped, and its listener was verified absent.
The same monitor is disabled; there is no next check.

**Local cleanup limitation:** automatic approval review rejected removal of this attempt's four generated key files,
including a separate exact-path deletion request, with no specific reason supplied. They remain in the external run's
key directory. The cloud guest and its instance metadata have been deleted. No older keys or unrelated files were
removed, and no alternative deletion mechanism was used to bypass that block.

The existing billing account and cumulative USD5 budget remain, retaining the November 3 start, November 7 end,
scope and alerts. The budget is not a spending cap; actual cost remains unknown/pending. Before any future run, the
external settings observer needs unique UIA identity/scoping and captured six-value diagnostics without modifying
settings; failed phases also need a diagnostic end inventory. Neither an assertion waiver nor another cloud run was
started. Raw evidence and inspection receipts remain outside the repository. Focused documentation validation is
recorded in the external finalization receipt; no commit, push, publication or product edit was performed.
