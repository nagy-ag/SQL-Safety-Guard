> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 → 22.11.3 all-forms observer retry

**Outcome: INCOMPLETE.** This fifth attempt installed actual SSMS **22.9.2 / 22.9.12120.119** and the unchanged
published SQL Safety Guard **1.0.3**. Startup-window automation failed before the corrected native settings observer
was reached. The early acceptance gate prevented execution stages. The once-only phase review recorded **0/11**;
there were **zero core execution cases**, no 22.11.3 installation, and no upgrade or restart acceptance. This does
not demonstrate an extension regression, lost settings or a false green.

## Identity and local preparation

The sole controller started **2026-11-06 10:28:22.9476784 UTC**, PID19012, script SHA256
`46D47EF033E3E48BB1F552E578227FF859A5F802357D8A012242E54FBE9C6514`. Its ownership receipt, original clock and
all four prior campaign checkpoints were preserved; their recorded checkpoint hashes were verified at finalization.
The controller finished **11:07:58.4452979 UTC** with `Independent before evidence review failed; no continuation`.
Its absence was checked alongside the result and export receipts, not treated as success by itself.

Published source remains `30aec84e40758b28da85378e57b5dff3f6b0d7c9`. Frozen artifact hashes:

| Artifact | SHA256 |
|---|---|
| Published VSIX 1.0.3 | `0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC` |
| Release extension DLL | `97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2` |
| Microsoft 22.9.2 bootstrapper | `3826637010859431029327F8CB8D63608166DA459BD5CA3DA5AD524C80892270` |
| Microsoft 22.11.3 bootstrapper, not run | `2C1AE4F6BD3AEB7314282EF5C94ABEE6AA2D68A2CBFD4B8825438F8542F66A36` |

Only external harness helpers changed. The local correction uses `SelectionPattern.Current.GetSelection()` and
requires every complete rendered Preview form under the owned Options root and selected SQL Safety Guard page
to show all six exact values. One or two forms are permitted; each must have six unique typed/named controls,
verified runtime identities, ancestry, process ownership and visibility. Incomplete, conflicting, extra or unknown
forms reject acceptance. An independent Python reviewer derives the groups again from raw controls. The strict
SQL/XE/counter/context/target review block was preserved byte-for-byte.

Before launch, 22 original observer checks, 31 all-form checks and 27 independent-review checks passed. An actual
WPF/UIA fixture on a never-switched private desktop exposed two distinct Preview forms and 12 controls; the corrected
API read both complete forms and the fixture closed cleanly. An initial missing System.Xaml compile reference was
corrected and its error retained. These checks validate that observer fixture, not SSMS startup handling or live
SSMS acceptance. No host input, foreground focus, clipboard, local SSMS or SQL was used for this fixture.

## Actual failure and expected flow

After initial installation and five smoke launches, the settings helper launched its owned SSMS process and called
`Dismiss-OwnedStartup` before opening Options. At `Common.ps1:38`, the unguarded
`AutomationElement.FromHandle(window)` call threw `ElementNotAvailableException`, wrapped in a PowerShell
`MethodInvocationException` with “Unrecognized error.” The stack identifies `Candidate-Settings.ps1:56` as the caller.
Unlike the nearby general UI enumeration helper, this startup path did not catch this unavailable-element condition.

A window disappearing between enumeration and UIA lookup is a plausible explanation; the exact failing handle was
not captured, so a handle race or SSMS crash is not established. Lifecycle evidence confirms the owned settings
process subsequently closed with exit0 and CIM absence. There are no guest native-observation/native-values files;
the successful local fixture capture is separate and cannot substitute for them.

The intended flow was startup dismissal, native readback of every rendered form, baseline execution with actual
server evidence, then the same-installation 22.11.3 update, first launch and restart without repair or settings writes.
The settings gate rejected the failed readback and stopped before the input and multi-window stages. Both child
scripts returning exit0 did not imply acceptance: the structured settings error and phase summary correctly rejected it.

Before another campaign, the startup-window adapter needs bounded handling of the specific unavailable-element
condition, fresh owned PID/start/window checks and per-handle lifecycle logging. Required UI observation must still
fail closed when it cannot be established. The local fixture should cover disappearing windows before another live
attempt. That correction was not implemented or live-tested during this finalization; no replacement run was launched.

## Independently inspected evidence

| Evidence | Result and limit |
|---|---|
| SQL setup | SQL Server Express 2025 17.0.1000.7, installer exit0, disposable guest |
| SSMS installation | Display22.9.2, executable22.9.12120.119; installer3010 and guest restart honored |
| Initial extension | Installer exit0 and initial refresh completed; all27 frozen package-member hashes verified |
| Five panes | Green SELECT; yellow unknown procedure; red permanent UPDATE; allow-listed UPDATE; trusted procedure; all show Whole window and expected named rows/counts |
| Persisted settings | GuardMode YellowRed, ShowConnection true, AllowList dbo.UpgradeAllowed, TempdbIsTemp true, TrustedProcedures dbo.UpgradeReader and the exact seeded environment rule unchanged |
| Closed profile capture | Same installation/profile; read-only capture, six settings and31 extension-file hashes unchanged, no capture errors |
| Native live settings | Not reached; no acceptance |
| Lifecycle | Six owned SSMS launches closed with exit0/CIM absence; initial installer and refresh exited0; transient close-dialog UIA errors retained separately |
| Execution | No input/multi-window stages, SQL/XE/case counts/submission counters/context/target evidence produced; no case PASS |
| Upgrade | Not attempted; no 22.11.3 installed-build receipt |

The baseline reviewer ran once at **11:04:48 UTC**. It was not repeated. Independent final inspection checked its
errors against raw settings, summary, pane, profile and lifecycle evidence, and verified every archive member and
manifest length/hash against the extracted files, including duplicate-path and containment checks.

| Export | Members / manifest files | SHA256 |
|---|---|---|
| Baseline | 115 / 114 | `D018AADDE1643718A4EECF274173C9B11AEA08845E7089E081214183EADC0295` |
| Final | 145 / 144 | `384CE5348585E11086D452E154148783E73DE5DC50415EFA4807BC401F49C20B` |

Raw evidence and the independent final audit remain outside the repository in this campaign's evidence directory.
Earlier attempts retain their original identities and qualifications; their observations are not new cases for this run.

## Cleanup and limits

The VM's absolute auto-delete stayed **13:30:12 UTC**, with export reserve **13:05:12 UTC** and cleanup cutoff
**13:15:12 UTC**. No deadline was extended. Successful pre-unlink inventories at **11:07:51 UTC** show no owned
VM, disk, snapshot, address, custom network, subnet or firewall remaining. The cleanup helper checks native command
exit codes and immutable VM ownership. Billing was then unlinked and a fresh billing readback confirmed disabled;
post-unlink Compute failures were not used as cleanup proof. The existing account and cumulative USD5 budget,
November3 start/November7 end and alerts were retained. Budget is not a spending cap; actual cost remains unknown/pending.

At **11:19:13 UTC**, the exact owned IAP root/start/command and descendants were checked, closed and verified absent,
including listener22433. The same monitor was disabled, with returned nextRunAt null. Automatic approval review
rejected the separate contained deletion of this run's four generated local key files with “blocked by policy” and
no specific reason. They remain; older blocked key sets were untouched and no alternative deletion was attempted.

Host desktop/input/clipboard, local SSMS, Docker, default cloud configuration and unrelated resources were preserved.
No post-upgrade repair, reinstall, cache refresh or settings rewrite occurred; the upgrade was never reached.
The intended eleven core cases omit SQLCMD-on and deliberate ambiguous-target execution. Guest SendInput F5 would
not prove physical human keyboard input, and Windows Server2022 would not establish Windows11 acceptance. No internal
Release guard text hash is available. Missing evidence remains INCOMPLETE.

Documentation-only finalization passed the focused **4 DocsIndexTests** and `git diff --check`. No product edits,
commit, push or publication were performed.
