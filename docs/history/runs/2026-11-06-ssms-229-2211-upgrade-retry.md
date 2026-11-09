> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 to 22.11.3 upgrade retry — November 6

**INCOMPLETE: initial 22.9.2 and the released extension were installed, but the baseline stopped before execution checks.**
The pane captures contain read-only, unknown-procedure, permanent-write, allow-list and trusted-procedure states.
The same-installation update to 22.11.3 was not attempted. There is no upgrade, settings-survival or F5/Execute pass,
and no demonstrated extension regression. The [November 5 attempt](2026-11-05-ssms-229-2211-upgrade-attempt.md) and its
earlier blocked preparation remain unchanged.

## Identity and intended acceptance

The owner explicitly requested another attempt with the unchanged published **SQL Safety Guard 1.0.3 Release VSIX**.
Release source is `30aec84e40758b28da85378e57b5dff3f6b0d7c9`; the existing gates remain in the
[publication snapshot](2026-11-05-release-1.0.3-publication.md). No product source, assertion or binary was changed.

| Input or installed artifact | Verified identity |
|---|---|
| Published VSIX SHA-256 | `0ee37ae4847caf31544ee4a9f686e3c10a6f72a55d3970859c338e6e4f4f69dc` |
| Release extension DLL SHA-256, also recorded in the installed file inventory | `97b90388dc8d604d8f6b80c411727693432f1b229d06077964f3f0fa8c5b81f2` |
| Initial SSMS display version / executable build | **22.9.2 / 22.9.12120.119** |
| 22.9.2 bootstrapper SHA-256 | `3826637010859431029327f8cb8d63608166da459bd5ca3da5ad524c80892270` |
| Selected 22.11.3 bootstrapper SHA-256; not run | `2c1ae4f6bd3aeb7314282ef5c94abee6aa2d68a2cbfd4b8825438f8542f66a36` |

Both bootstrappers retained valid Microsoft Authenticode signatures. The frozen upload payload and embedded VSIX/DLL
were checked before launch. A fresh disposable Windows Server 2022 guest and a unique isolated SSMS profile were used.
Guest SQL Express 2025 reported READY, build **17.0.1000.7**, installer exit 0. Host SSMS, input, clipboard, Docker,
the default cloud configuration and unrelated resources were not used or modified.

The intended baseline, first launch after the update and restart phases each required eleven execution cases, actual
SQL/XE submission and counter evidence, verified target/context, five semantic panes, six native settings and unchanged
extension files. No repair, cache refresh, reinstall or settings rewrite was permitted after the upgrade. SQLCMD-on and
deliberate ambiguous-target execution remain explicit gaps of those eleven core cases. Guest SendInput F5 would not
establish human physical-key coverage. This Windows Server guest would not establish Windows 11 acceptance.

## Infrastructure changes and observed stop

Only the external test helpers were changed before this retry. Queue status SSH failures now retry read-only observations
of the same request; uncertain dispatch does not submit another request. Receipt reads and same-file transfers have
bounded retries. Evidence export copies a bounded initial-length snapshot using shared reads, then archives closed copies
with per-file lengths and hashes. The prior final-export quoting error was corrected in the new controller.
Synthetic checks established read retry without duplicate dispatch and exact export of a file held open by a writer;
the helper scripts also passed parser checks. The old preparation generators were not rerun.

The retry controller started **06:00:30 UTC** on November 6. Initial SSMS installation completed in approximately sixteen
minutes with exit **3010**, and the normal installer-requested guest restart was honored. The actual display version and
full executable build above were read back before the baseline began at **06:33:36 UTC**.

The released VSIX was installed once. The seed receipt records six explicit settings and a 31-file extension inventory;
27 entries match members of the frozen VSIX, including the expected Release DLL. This is an initial file/settings record,
not native settings UI readback or proof of settings persistence after a host upgrade.

The pane smoke helper returned exit 0, but its results were explicitly marked INCOMPLETE pending evidence inspection.
The next stage refused to start at **06:37:23 UTC** with **“Owned UI retained before stage”**. That check detects an
SSMS or VSIXInstaller process, but did not record the retained process's PID, command line or start time. All five tracked
SSMS launch cleanup receipts say Exited=true. Therefore the evidence does not identify the retained process or explain
the discrepancy; it does not establish an extension shutdown defect.

Independent inspection found the following pane text in the captured UI: “No permanent changes detected” for SELECT;
an unknown procedure with its body not analyzed; a permanent UPDATE without WHERE; and “ALLOWED BY YOUR SETTINGS” for
the allow-listed write and trusted procedure. These are qualified pane observations, not a formal baseline pass.
The legacy process-module probe reported ModuleLoaded=false for all five launches despite those visible extension
controls; it does not establish the runtime module identity. Activity logs retain shell MEF dependency/settings errors;
no extension-specific exception was identified in those error entries, and their causal role is unknown.

The wrapper stopped before native Options readback, the seven F5 cases and the four toolbar/two-window cases.
There are no execution SQL/XE/count/counter/context/target records, no end-of-baseline profile comparison, and no
post-update or restart archives. The independent before-phase reviewer ran once, verified the archive and actual build,
then reported **0/11, INCOMPLETE** because the required profile capture was absent. Its missing-file error was separately
checked against the phase directory and the earlier wrapper error. The controller correctly refused the update.

## Retained evidence and cleanup

The new exporter retained both archives successfully. Their SHA-256 values, extracted members and every manifest file
length/hash were independently verified; neither manifest reports an export error. Active log captures are snapshots,
not claims about any later bytes appended by a writer.

| Export | SHA-256 | Verified evidence |
|---|---|---|
| Baseline phase archive | `99814f7b8178c7610a9f33b9da39525c78eb86811f738dd6cd2edae7994b7117` | 50 file members, including manifest; 49 manifest file entries |
| Final archive | `93c46146c3a0a2352b70be6365aba8fbf9d054e140a11cca1b72b6febe0df824` | 61 file members, including manifest; 60 manifest file entries |

The VM had an absolute **09:02:16 UTC** auto-delete time; final cleanup was required by **08:47:16 UTC**. Neither was
extended. Successful pre-unlink inventory receipts at **06:39:56 UTC** show no owned VM, disks, snapshots, reserved
addresses, custom network, subnet or firewall. The controller finalized INCOMPLETE at **06:39:59 UTC**, with final
evidence exported. A later billing readback confirms the test project is unlinked. No post-unlink Compute inventory
was presented as fresh cleanup proof. The exact owned IAP tree/listener and only this retry's generated keys were
removed and verified at approximately **06:50 UTC**. The same monitor is disabled; there is no next check.

The existing billing account and budget remain. The expired budget end date was extended from November 5 to November 7
for this retry, retaining its original November 3 start, cumulative USD5 amount, project scope and alerts. It was not reset
and is not a spending cap. Actual billed cost remains unknown/pending.

The requested upgrade remains unverified. Before any future attempt, the external lifecycle helper needs exact retained
process diagnostics and verified closure. Its unreached allow-list/trusted-pane assertions also expect a SafeHeader,
whereas the captured released UI uses the allowed-settings section; that mismatch needs semantic review rather than
being reported as a product regression. No speculative retry, assertion change or new cloud run was started after this
failure. Raw evidence and validation receipts remain outside the repository. Documentation validation is recorded in the
external finalization receipt; no commit, push or publication was performed.
