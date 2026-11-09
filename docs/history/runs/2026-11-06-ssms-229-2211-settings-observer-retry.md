> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 to 22.11.3 settings-observer retry — November 6

**INCOMPLETE: native settings observation rejected the baseline before execution or upgrade.** Actual SSMS
**22.9.2 / build 22.9.12120.119** ran unchanged published **SQL Safety Guard 1.0.3**. Five pane states were observed;
the final closed-profile capture establishes six persisted settings and 31 extension-file hashes unchanged from
the initial inventory. Native six-value acceptance remains unavailable: there are two distinct UIA identities for
each setting, and reading the warning-mode dropdown also raised an API error. The once-only phase reviewer returned
**0/11, INCOMPLETE**. No core execution cases, 22.11.3 update, post-update first launch or restart test ran.
This demonstrates a test-observer blocker, not lost settings, an extension regression or a false green.

## Frozen inputs and bounded run

The owner approved the settings-observer plan and this retry. Original controller start **09:10:36.6380255 UTC**,
ownership and prior clocks were preserved. Controller script SHA-256 was
`ab4a9677d8397fdfcea76fa42a9cfc0f444bbb9a76955716ca3f799771ff85fd` and matched at final inspection;
the process was absent and its terminal result was inspected. No duplicate controller, guest, queue request or SSMS
was launched during monitoring. No product source or release package was changed, rebuilt or republished.
Published source remains `30aec84e40758b28da85378e57b5dff3f6b0d7c9`.

| Input | SHA-256 |
|---|---|
| Published 1.0.3 VSIX | `0ee37ae4847caf31544ee4a9f686e3c10a6f72a55d3970859c338e6e4f4f69dc` |
| Release DLL | `97b90388dc8d604d8f6b80c411727693432f1b229d06077964f3f0fa8c5b81f2` |
| Microsoft 22.9.2 bootstrapper | `3826637010859431029327f8cb8d63608166da459bd5ca3da5ad524c80892270` |
| Selected 22.11.3 bootstrapper, not run | `2c1ae4f6bd3aeb7314282ef5c94abee6aa2d68a2cbfd4b8825438f8542f66a36` |

Preflight verified frozen package identity and Microsoft bootstrapper signatures. SQL Express 2025 **17.0.1000.7**
setup returned 0 in the disposable Windows Server 2022 guest. SSMS installation took about 13 minutes, returned
**3010**, and the requested guest restart was honored. The VSIX was installed once in the new isolated profile;
all **27 installed package-member hashes** match the frozen VSIX. The final inventory retains those members within
the same **31-file** set, same instance and same profile.

External harness changes scoped the native observer to the owned active Options root, enumerated once and deduplicated
only equal UIA RuntimeIds. Diagnostics retain parent identities, visibility, type, bounds, values and read errors.
Twenty-two offline checks passed for selection/deduplication/refusal fixtures, six-value assertions, script parsing,
the early execution gate and final capture. They did **not** cover a live dropdown SelectionPattern call or the
two distinct Unified Settings subtrees subsequently observed. Strict SQL/XE/count/counter/context/target assertions
were unchanged. Existing lifecycle diagnostics, same-request read-only SSH retries and bounded shared-log exports
were retained. Prepare/patch generators and preflight were not rerun after launch.

## Exact blocker and independent observations

Baseline began **09:40:12 UTC**. Smoke and settings helpers exited 0, but their result files gated acceptance.
At **09:44:30 UTC**, the observer recorded the original error:

`Native setting identity not unique: AllowList; observations=2, distinct=2`

The owned Options root has runtime identity `42:655738`, process 2652. Its twelve captured controls contain two
different runtime identities per setting. In particular, AllowList identities **7:2652:537** and **7:2652:599** have
different bounds and different Preview ancestors, **7:2652:568** and **7:2652:630**, beneath a common Unified Settings
scope. Both report enabled and on-screen; both show `dbo.UpgradeAllowed`. The other edit/checkbox pairs also agree
on their captured values. This establishes distinct provider identities rather than repeated observation of the
same RuntimeId. It does not establish which subtree is the active authoritative settings page. Equal values do not
justify selecting the first control or waiving native identity acceptance.

Both WarningMode controls additionally record:

`[System.Windows.Automation.SelectionPattern] does not contain a method named 'GetCurrentSelection'`

Independent local .NET UIA type reflection confirms that method is absent; the selection-information type exposes
`GetSelection`, accessed through `SelectionPattern.Current`. This is a separate concrete harness API bug. No live
settings change, assertion relaxation or rerun was used to work around it. Before another campaign, the observer
needs a tested read-only selection adapter and proof of the active page/preview ownership; genuinely distinct or
unknown targets must still stop. The existing offline fixtures were insufficient for these live behaviors.

The early gate blocked Candidate-Input and Candidate-Multi. The finally-path read-only capture succeeded with SSMS
closed and no capture errors at **09:44:48 UTC**. The six persisted values exactly match the initial seed: YellowRed,
show connection on, allow-list `dbo.UpgradeAllowed`, tempdb opt-in on, trusted procedure `dbo.UpgradeReader`, and
the exact seeded environment rule. All 31 file hashes and the profile/instance match. This closes the diagnostic
capture gap from the previous attempt, but it does not replace native UI readback or post-upgrade preservation.

The phase reviewer ran **once**, at **09:45:21 UTC**, retained the native-observer error and returned INCOMPLETE.
It was not rerun. Separate inspection verified:

| Observation | Qualified result |
|---|---|
| Five pane states | SELECT safe section; unknown procedure/body not analyzed; permanent UPDATE/no WHERE; allow-list and trusted-procedure sections with correct objects and reasons; all show Whole window |
| Module probe | Reports false/INCOMPLETE despite captured extension UI; not used as proof of absent extension or a successful phase |
| Settings/profile/files | Six persisted values unchanged; 31 unchanged file hashes; same profile/instance; closed read-only final capture |
| Native settings | Two distinct RuntimeIds per setting; dropdown read errors; no complete native-values receipt; rejected |
| SQL/XE/submissions/counters/context/target | No core execution stages started, so no case evidence produced and no execution acceptance claimed |
| Lifecycle | Six clean SSMS exits independently verified by CIM, five quiet-stage events; six transient observations of already-closed UI do not override subsequent process-absence checks |

No forced installer-descendant stop was recorded. The controller finalized **INCOMPLETE at 09:48:14 UTC**, refusing
the upgrade after failed baseline review. Missing evidence never counts as PASS. The eleven-case design still omits
SQLCMD-on and deliberate ambiguous-target execution; guest SendInput F5 would not establish physical human keyboard
coverage. Windows Server 2022 is not Windows 11 acceptance, and there is no Release internal guard text hash.
No post-upgrade repair, refresh, reinstall or settings rewrite occurred; no upgrade occurred.

## Exports, cleanup and retained limits

Both archive SHA-256 values, every extracted member and every manifest length/hash were independently verified.
Both manifests report complete bounded shared-read snapshots without copy errors.

| Export | SHA-256 | Members / manifest files |
|---|---|---|
| Baseline | `b40bb042b562ecd705969d911ca984375b755ee2d5ad86ae44dd79442d5c900d` | 119 / 118 |
| Final | `2b268b27f31b9ca6b1483d9909744d1fd179ddb2a8321c2e1f2906597893c2ff` | 148 / 147 |

Absolute auto-delete was **12:12:13 UTC**, execution/export reserve began **11:47:13 UTC**, and final cleanup was
required by **11:57:13 UTC**. None was extended. Successful pre-unlink inventories at **09:48:06 UTC** establish
no owned VM, disk, snapshot, address, custom network, subnet or firewall remaining. Fresh billing readback confirms
the test project is unlinked; post-unlink Compute queries were not used as fresh cleanup proof. Only the owned
IAP tree was stopped after identity checks. Stopping its verified Python child caused the remaining tree to exit;
the loop refused a subsequent changed identity, and separate readback verified all three saved PIDs and the listener
absent. The same monitor is disabled with returned `nextRunAt: null`.

**Local cleanup limitation:** automatic approval review rejected the separate, contained removal request for this
run's four generated key files, reporting only “blocked by policy.” They remain in the external evidence directory;
no alternative deletion mechanism was attempted. Earlier blocked key sets remain untouched. Cloud guest metadata
was deleted with the VM. The existing billing account and cumulative USD5 budget retain the original November 3
start, November 7 end, scope and alerts. The budget is not a spending cap; actual cost is unknown/pending.

The [previous diagnostic attempt](2026-11-06-ssms-229-2211-diagnostic-retry.md),
[first November 6 retry](2026-11-06-ssms-229-2211-upgrade-retry.md),
[November 5 attempt](2026-11-05-ssms-229-2211-upgrade-attempt.md) and blocked preparation are preserved.
Host desktop/input/clipboard, local SSMS, Docker, default cloud configuration and unrelated resources were untouched.
Raw evidence and final inspection receipts remain outside the repository. Documentation validation is recorded in
the external finalization receipt. No commit, push, publication or product edit was performed.
