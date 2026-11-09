> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 to 22.11.3 upgrade attempt — November 5

**INCOMPLETE: the attempt stopped during initial SSMS installation, before the extension baseline.**
No in-place upgrade, extension installation, settings-survival check or F5/Execute case completed.
This is an infrastructure/observation failure, not evidence that SQL Safety Guard failed an SSMS upgrade.
Earlier compatibility and publication results retain their original identities and limits.

## Intended qualification and frozen inputs

The owner requested the actual same-installation upgrade from SSMS 22.9 to 22.11 with the extension already installed.
The selected representatives were **22.9.2 → 22.11.3**, with the unchanged published **SQL Safety Guard 1.0.3 Release VSIX**.
The plan required a disposable Windows Server 2022 guest, an isolated SSMS profile, a pre-upgrade snapshot, exact installed
build readback, six nondefault/explicit native settings, unchanged extension files, and independent SQL/XE evidence for
eleven focused execution cases before the upgrade, on first launch afterward, and after a restart. No post-update repair,
cache refresh, reinstallation or settings rewrite was permitted. SQLCMD-on execution and deliberate ambiguous-target
coverage were outside those eleven core cases; guest-injected F5 would not establish human physical-key coverage.

| Frozen input | SHA-256 | Established |
|---|---|---|
| Published `SqlSafetyGuard-1.0.3.vsix` | `0ee37ae4847caf31544ee4a9f686e3c10a6f72a55d3970859c338e6e4f4f69dc` | Fresh download matches published checksum; manifest identity/version checked; not installed in this attempt |
| Embedded Release extension DLL | `97b90388dc8d604d8f6b80c411727693432f1b229d06077964f3f0fa8c5b81f2` | Hash read from that VSIX; no runtime load claim |
| Microsoft 22.9.2 bootstrapper | `3826637010859431029327f8cb8d63608166da459bd5ca3da5ad524c80892270` | Hash and Microsoft Authenticode signature valid; actual installed build not obtained |
| Microsoft 22.11.3 bootstrapper | `2c1ae4f6bd3aeb7314282ef5c94abee6aa2d68a2cbfd4b8825438f8542f66a36` | Hash and Microsoft Authenticode signature valid; installation/update not reached |

Release source remains `30aec84e40758b28da85378e57b5dff3f6b0d7c9`; its existing build/offline gates are recorded in the
[publication snapshot](2026-11-05-release-1.0.3-publication.md). No product source, assertion or release binary changed,
and no build or full offline gate was repeated for this host-upgrade attempt.

## Observed execution and failure

Initial preflight found local Hyper-V access denied and both accessible cloud billing accounts closed. After the owner
reopened the existing test billing account, the controller started at **20:29:38 UTC** on November 5. A fresh disposable
guest was created with an absolute **23:31:49 UTC** auto-delete deadline and boot-disk auto-delete; neither was extended.
The pinned SSH connection and guest console became ready. Guest SQL Express setup reported **READY / installer exit 0**
at **20:45:19 UTC**, and initial SSMS 22.9.2 installation started at approximately **20:45:35 UTC**.

A subsequent controller SSH request timed out with exit **255** while that installation was underway. The controller
stopped before obtaining its installed-version receipt or running the baseline. It did not establish whether SSMS setup
itself succeeded or failed. No second controller, VM, queue request or speculative installer retry was launched.

Final export then failed because `ssms-22.9.2/installation.log` was still held open by the installation helper.
The full evidence archive was **not** exported. The VM was subsequently deleted by the controller's cleanup path, so the
guest installation log and any later setup result cannot be recovered from this attempt. Host controller/error logs,
provisioning receipts, package checksums, checkpoints and cleanup receipts remain outside the repository. The independent
review reports all three planned phases **INCOMPLETE**, with no available execution cases; missing archives are not passes.

One final-export host variable-expansion typo was corrected in the transport wrapper before dispatch, with a correction
receipt. That correction did not change a test/assertion and did not resolve the separate locked-file export failure.
Earlier transient SSH/console-startup messages and one unsuccessful read-only observation are retained without being
relabeled as extension failures.

## Cleanup and remaining work

Before unlinking project billing, cleanup receipts confirmed empty inventories for the owned VM, disks, snapshots,
reserved addresses, network, subnet and firewall at **20:53:28 UTC**. The controller finalized **INCOMPLETE** at
**20:53:33 UTC**. A later billing readback confirmed the test project remained unlinked. Post-unlink Compute inventory
requests were rejected because billing was disabled; their empty-looking output is not independent cleanup proof.
The exact owned IAP process tree and listener were subsequently confirmed absent, this attempt's generated SSH key files
were removed separately, and the monitor was disabled. The billing account and existing cumulative budget/alert remain
for delayed charges; actual billed cost is unknown. Host SSMS and other resources were not modified.

The requested upgrade remains unverified. A future separately scoped attempt needs transient SSH failures to remain
observations of the same running queue job, and evidence export to wait for or safely snapshot open logs before VM deletion.
This report adds no extension defect, false-green finding, settings-migration pass or supported-version expansion.
Documentation validation is recorded in the external finalization receipt. No commit, push or publication was performed.
