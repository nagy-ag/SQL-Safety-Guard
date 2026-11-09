> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# SSMS same-installation upgrade — simple package-integrity attempt

**2026-11-06: INCOMPLETE.** Exact **SSMS22.9.2 /22.9.12120.119** installed with unchanged published SQL Safety Guard1.0.3.
Independent raw inspection confirmed five semantic pane states, all six native values in two complete forms/12distinct
controls, six persisted settings, and all27 frozen package members. The baseline execution helper then stopped at the
startup connection confirmation before any core case. Formal acceptance is **0/11**; the controller refused to update
to22.11.3. This run does not establish that the extension survives an in-place update, and does not prove a lost setting,
extension regression or false green.

## Scope and provenance

The owner requested a simpler sequence: install22.9.2 and published1.0.3, verify settings/execution, update that same
installation to22.11.3, verify again, compare immutable package files and record SSMS cache changes separately.
The campaign was `upgrade-229-2211-simple-20261106-1300`; all six earlier attempts and the blocked preparation were retained.
The sole controller's original exact start was **13:03:28.9115563UTC**, PID36392, script SHA256
`6F8946A9429F719AEB5031EC3D02B0E39CA69349037DE21D3BAB27432BF7FCB3`. Exact process/start/script/hash checks passed while
it ran. Its final absence was checked against the result, exports and cleanup receipts, not treated as success alone.

Published source: `30aec84e40758b28da85378e57b5dff3f6b0d7c9`.

| Frozen artifact | SHA256 |
|---|---|
| Published1.0.3 VSIX | `0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC` |
| Release DLL | `97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2` |

Only external harness code changed before launch. No product edits, rebuild, commit, push or publication occurred.
The host desktop, input, clipboard, local SSMS, Docker, default cloud configuration and unrelated resources were preserved.
The VM ran Windows Server2022; it was not Windows11 acceptance.

## Harness changes and local evidence

The profile inventory and independent package reviewer distinguish all27 frozen VSIX payload members from four named
SSMS root metadata/cache files. Missing, changed, extra, duplicate, conflicting or unowned package paths reject acceptance;
all six persisted settings remain mandatory. Cache bytes are observed without repair or refresh and their deltas are
reported separately. The strict SQL/XE/submission-count/counter/context/target review block remained unchanged.

The queue worker captures child identity once, catches observational failures separately, writes a terminal marker in
`finally`, and never appends errors to a stdout file held open by its child. Local validation passed **17PowerShell checks,
16independent Python checks and three actual short owned-child fixtures** covering success, exit7 and an observation
exception. These checks executed no SSMS UI or SQL and were not repeated after launch.

The planned VM flow had one complete before phase and one after phase, each reviewed once; no extra precheck,
continuation, snapshot or restart-test phase. No post-update repair, cache refresh, reinstall or settings rewrite was
permitted. No update took place in this run. Initial VSIX installation/profile initialization happened before baseline.

## Observed result and cause

| Event, UTC | Result |
|---|---|
|13:19:14 | Disposable SQL Express setup completed, child exit0 |
|13:38:18 | SSMS readback22.9.2 /22.9.12120.119; installer exit3010 retained |
|13:39:42–13:44:10 | Smoke and native-settings stages exited0; raw evidence retained |
|13:44:24 | Input stage startup observer raised `Startup connection confirmation not unique` |
|13:44:29 | Owned input SSMS exited0; process handle exit and CIM absence verified |
|13:44:36 | Phase/queue terminal INCOMPLETE with child exit1; final read-only profile capture succeeded |
|13:45:28 | Controller ran the before review once;0/11INCOMPLETE |
|13:48:26–13:48:31 | Successful empty cloud inventories, then billing unlink; controller finished |

The input helper launched its owned query with a server/database argument. SSMS displayed “Connect to the following
server?”; the startup observer required exactly one enabled UIA `Button` named Yes. Its saved stack identifies that
filter/uniqueness check as the failure. The later shutdown UI snapshot exposes Yes as **`ControlType.Pane`**, with native
handle524824; two flattened observations refer to the **same handle**, not proof of two separate buttons. This supports
a UI representation mismatch in the automation. The exact matching-control count at the failure instant was not logged,
so that detail remains unproven. The helper stopped safely instead of choosing an unverified control.

The fixture database, counter table/procedure and XE session were created by development setup SQL, then reported
cleaned after owned SSMS closure. **No extension execution case completed.** There are no input case results,
query-oracle identities, before/after counters or raw XE case files; the multi-window stage never started. Therefore
actual guarded SQL text, submission counts, counter deltas, execution context and target acceptance remain unestablished.
The saved SQL input file and intended server/database are preparatory evidence, not execution proof.

The whole-phase reviewer additionally raised **`KeyError('Panes')`**. The phase writes its pane summary only after all
stages; the earlier input failure left that key absent. This is a secondary reporting failure, not the initial startup
failure. The original review was preserved and never rerun. A separate final audit inspected the raw pane evidence:
green SELECT, yellow unknown procedure, red permanent UPDATE, allow-listed UPDATE and trusted procedure were all present,
with clean closure receipts. Legacy smoke `ModuleLoaded:false`/INCOMPLETE flags remain as recorded; the raw semantic pane
observations do not turn the incomplete phase into PASS.

## Settings, payload and separate cache evidence

Raw native observation verified the selected SQL Safety Guard category, owned Options root/page, runtime identities,
ancestry, enabled/visible typed controls, both complete Preview forms and exact values for all six settings. The closed,
read-only final capture matched the seeded persisted settings and original profile/installation identity.

All27 original installed package paths/hashes match the frozen VSIX in both baseline and final inventories; no extra,
missing or changed package member was accepted. Four SSMS root cache/metadata files were recorded separately. Only
`Extensions/ExtensionMetadataCache.mpack` changed:

| Before SHA256 | After SHA256 |
|---|---|
| `F874DD58B63E86372CA1F476C1B57DB29219473900D42824D15C09DAB29A48AE` | `9506A9D01072EAC97FA85A08049EB28190621065D4A6898BB2E70F8ED8D6E7F5` |

That cache delta did not fail package integrity. Its contents/cause were not established. These observations are entirely
on22.9.2 and do not prove payload/settings preservation across22.11.3.

## Exports, cleanup and limits

The separate final audit passed archive SHA256, CRC, duplicate/path containment, every manifest member length/hash,
every extracted member and absence of extra extracted files. It did not invoke or overwrite the completed phase review.

| Archive | SHA256 | Manifest files / archive members |
|---|---|---|
| `phase-before.zip` | `1A0DFB8711724CA909D0102AEEDCC4BDFF255EC3DAC615E5CADEEEB259F62DB4` |135/136 |
| `upgrade-final.zip` | `23AB0761AC9E0CE50029CC6C70C9A8CFC9A3173438584D052FC266101C6985D2` |165/166 |

Raw outputs remain outside the repository, including `final-independent-audit.json`, the original review, lifecycle
events and terminal queue records. Successful **pre-unlink** inventories were empty for VMs, disks, snapshots, addresses,
owned custom network/subnet and firewall; project billing was then unlinked. No post-unlink billing-disabled Compute query
was used as fresh cleanup proof. The exact owned IAP tree and listener22433 were closed and rechecked; an asynchronous
console-host exit initially appeared pending, then absence was verified without further termination.

VM immutable ID113705306520841826 had absolute auto-delete **16:05:13UTC**; export reserve15:40:13UTC and final cleanup
cutoff15:50:13UTC were never extended. Cleanup completed well before those limits. The existing account and cumulative
USD5 budget, original November3 start, November7 end and90/100alerts were retained. Actual cost remains unknown/pending;
the budget is not a spending cap.

Automatic approval review rejected deletion of this campaign's four generated local key files as **“blocked by policy”**
without a specific reason. They remain; no bypass or older-key deletion was attempted. The same monitor is disabled,
actual next run is null, and no automatic campaign retry follows.

SQLCMD-on and deliberate ambiguous-target cases remain outside the11core cases. F5 would have used guest SendInput,
not a human physical keyboard. No Release internal guard text hash,22.11.3 acceptance or extra restart acceptance is
claimed. The next technical work, if separately authorized, is to validate an owned native connection-confirmation
adapter and make failed-stage reviews retain raw diagnostics without requiring a success-only pane summary.
