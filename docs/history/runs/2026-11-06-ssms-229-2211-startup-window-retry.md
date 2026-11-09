> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.9.2 → 22.11.3 startup-window retry

**Outcome: INCOMPLETE.** The sixth attempt installed actual SSMS **22.9.2 / 22.9.12120.119** with unchanged published
SQL Safety Guard **1.0.3**. Five pane observations and all six native settings succeeded. Independent final inspection
verified both rendered settings forms and their 12 distinct controls. The closed-profile gate then rejected a change
to `Extensions/ExtensionMetadataCache.mpack`. All 27 frozen package members and six persisted settings remained unchanged;
30 of 31 inventoried files matched. The gate prevented execution: **zero core cases, formal 0/11 INCOMPLETE**. SSMS22.11.3
was not installed; same-installation upgrade, first-launch and restart behavior remain unverified. This is not evidence
of an extension regression, lost settings or a false green.

## Identity and local preparation

The sole controller started **2026-11-06 11:44:27.9381496 UTC**, PID5664, script SHA256
`9B830EE407E4B3C5F40509FE9E46B052AF1ED7ADAB8629E721854406B96BF4C3`. Monitoring verified the exact process start,
script and hash; CIM's microsecond-rounded timestamp did not replace the original clock. All five prior checkpoint
hashes were verified without changing their records. The controller finished **12:40:24.3995694 UTC** with
`Independent before evidence review failed; no continuation`. Its absence and unchanged script hash were checked
alongside the result and export receipts.

Published source remains `30aec84e40758b28da85378e57b5dff3f6b0d7c9`.

| Artifact | SHA256 |
|---|---|
| Unchanged published VSIX1.0.3 | `0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC` |
| Release extension DLL | `97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2` |
| Microsoft22.9.2 bootstrapper | `3826637010859431029327F8CB8D63608166DA459BD5CA3DA5AD524C80892270` |
| Microsoft22.11.3 bootstrapper, not run | `2C1AE4F6BD3AEB7314282EF5C94ABEE6AA2D68A2CBFD4B8825438F8542F66A36` |

Only external harness helpers changed. The startup adapter validates the owned PID/start/path/command and window
ownership before observing and before acting. It catches only the specific unavailable-element exception, logs the
handle/identity/exception, revalidates ownership and lets the bounded caller observe again. Other errors still reject.
The settings caller separately requires a unique ready settings button. Existing all-form observation and strict
SQL/XE/count/counter/context/target helpers were retained.

Before launch, **21 startup, 22 original observer, 31 all-form, 27 independent native-review and 7 phase-control checks**
passed. A real destroyed-window fixture reproduced the UIA exception on a never-switched private desktop and then
recovered into both complete forms. The first cross-desktop fixture failure was retained; the corrected observer and
fixture shared one private desktop. Phase-control fixtures used synthetic child results, not live SQL or SSMS acceptance.
No host input, clipboard, foreground focus or local SSMS was used.

## Actual precheck and failure

The initial installer returned3010; the guest restart was honored. SQL Express setup completed with exit0, reporting
SQL Server2025 Express17.0.1000.7 on Windows Server2022. The short precheck installed the frozen VSIX and seeded the owned
profile once, then ran only smoke/settings stages. Both child stages exited0; structured acceptance remained mandatory.

The settings lifecycle recorded readiness at **12:22:14 UTC**. Raw native observations at **12:22:39 UTC** identify the
owned Options root, selected SQL Safety Guard category/page, two complete Preview forms and six exact typed/named
controls per form. The independent final audit checked every runtime identity, value, ancestry, visibility and PID
against the owned profile/process. Six SSMS launches closed with exit0 and CIM absence. A transient close-dialog UIA
error was retained separately; it did not prevent verified closure.

At **12:22:55 UTC**, the closed read-only profile capture existed and showed all six settings unchanged. Its file
comparison rejected exactly one inventory item:

| File | Baseline SHA256 | Closed-capture SHA256 |
|---|---|---|
| `Extensions/ExtensionMetadataCache.mpack` | `3EFE0FE837E63666BF218F424769E7CBCBF99B934909550464B50E96E7F0C73A` | `88A12BD2617087288C94EDC8D4EDA3BEA5E66CE7CA6907D0EAA451723E95C36E` |

The baseline was taken before the first smoke launches and included shell metadata/cache files alongside package
payload. The actual package members all matched the frozen VSIX before and after. A shell cache update is a plausible
explanation for this difference, but its contents/cause were not established. The existing gate was not weakened or
reinterpreted as PASS. `FinalProfileCaptured=false` denotes failed comparison acceptance, not an absent capture file.

The guest queue heartbeat stopped at **12:19:02 UTC**. At the later read-only check, the recorded queue runner and
precheck child were absent, and no completion marker existed despite the retained finished phase summary/stderr.
The queue runner's exit cause is unresolved. No kill, restart, queue resubmission or replacement controller was used.
The existing host deadline expired at **12:36:14 UTC** and led to failed-phase export and its normal once-only review.

The intended short precheck would export and receive independent precheck acceptance before an atomically claimed
continuation ran input/multi-window tests on the same profile. That acceptance was never reached: there is no accepted
precheck export/review or continuation claim. The normal baseline review ran once at **12:36:53 UTC**, recording0/11
INCOMPLETE. It was not repeated. No execution SQL, XE, case counts, submission counters, execution context or target
evidence exists for this attempt. SQL setup is not execution-case acceptance.

Before a separately authorized retry, the profile inventory needs a tested distinction between immutable package
payload and shell-maintained cache state, with settings and all frozen payload hashes still mandatory. Queue-runner
termination/completion diagnostics also need investigation. Neither issue was fixed or retried during finalization.

## Independently verified evidence

| Evidence | Result and limit |
|---|---|
| Exact installed host | Display22.9.2, executable22.9.12120.119; no22.11.3 installed receipt |
| Initial frozen extension | Installer root exit0; all27 package-member hashes verified against the unchanged VSIX |
| Panes | Green SELECT; yellow unknown procedure; red permanent UPDATE; allow-listed UPDATE; trusted procedure; expected named rows and Whole window scope independently checked |
| Native settings | Two forms /12distinct controls, all six exact values, owned root/category/page/PID/runtime identities verified |
| Persisted settings | YellowRed, ShowConnection true, dbo.UpgradeAllowed, TempdbIsTemp true, dbo.UpgradeReader and exact seeded environment rule unchanged |
| Closed read-only inventory | Six settings and30/31files unchanged; one metadata cache mismatch rejects final acceptance |
| Lifecycle | Six SSMS closures verified by process handle and CIM; startup readiness established |
| Core execution / upgrade | None; formal0/11 INCOMPLETE, no upgrade or restart acceptance |

The final audit separately verified archive receipts, duplicate-path/containment rules, every member's CRC, manifest
length/SHA256 and corresponding extracted file. It inspected raw evidence independently of the failed phase review.

| Export | Members / manifest files | SHA256 |
|---|---|---|
| Baseline |117 /116| `8A583C7F2ECFD5FDC19DE043699F90C50475AFE4F798D316B0DBC6638568EE72` |
| Final |147 /146| `B1C0BE8C086D1B4C3B31A9735694DE9900E64151FD2986BE0A79ABBFD09AD6F2` |

Raw output, archives, the final audit and cleanup receipts remain outside the repository. All prior snapshots retain
their original qualifications and are not new execution evidence for this run.

## Cleanup and qualifications

The absolute VM auto-delete stayed **14:47:05 UTC**, export reserve **14:22:05 UTC**, cleanup cutoff **14:32:05 UTC**.
No deadline was extended. Successful pre-unlink inventories at **12:40:18 UTC** show no owned VM, disk, snapshot,
address, custom network, subnet or firewall remaining. The cleanup helper checks native exit codes and immutable VM
ownership. Billing was then unlinked with `billingEnabled=false`; post-unlink Compute failures were not cleanup proof.
The existing account and cumulative USD5 budget, November3 start/November7 end and90/100alerts were retained.
The budget is not a spending cap; actual cost remains unknown/pending.

At **12:40:07 UTC**, the exact owned IAP root/start/command and descendants were closed and verified absent, including
listener22433. The same monitor was disabled with returned nextRunAt null. Automatic approval review rejected the
separate contained deletion of this attempt's four generated keys with “blocked by policy” and no specific reason.
They remain; no alternative deletion was attempted and older blocked key sets were untouched.

Host desktop/input/clipboard, local SSMS, Docker, default cloud configuration and unrelated resources were preserved.
No post-upgrade repair/cache refresh/reinstall/settings rewrite occurred; the upgrade was never reached. The intended
eleven cases omit SQLCMD-on and deliberate ambiguous-target execution. Guest SendInput F5 would not establish physical
keyboard input, and Windows Server2022 does not establish Windows11 acceptance. No internal Release guard text hash is
available. Missing evidence remains INCOMPLETE. No further automatic campaign retry was launched.

Documentation-only finalization passed the focused **4 DocsIndexTests** and `git diff --check`. No product edits,
commit, push or publication were performed.
