> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Native connection-confirmation upgrade retry — 2026-11-06

The requested same-installation **SSMS 22.9.2 → 22.11.3 upgrade remains INCOMPLETE**. The unchanged published extension
completed its baseline stages on exact **22.9.2 / 22.9.12120.119**, and a separate final raw-evidence audit corroborated
all **11 baseline execution cases**. The controller's independent review stopped while decoding the phase summary,
before it could accept the baseline and authorize the update. **22.11.3 was never installed.** Baseline evidence does
not prove survival across an upgrade.

## Scope and identity

- Published SQL Safety Guard **1.0.3**, source `30aec84e40758b28da85378e57b5dff3f6b0d7c9`, unchanged.
- VSIX SHA256: `0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC`.
- Release DLL SHA256: `97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2`.
- Sole controller original exact start **2026-11-06T14:14:41.5345224Z**, PID 36228, script SHA256
  `B1A06AED4965070BA67ED011A49A07BFAC3D86C6B3F417F923EAFE75B939F751`.
- Exact process start/path/command/hash were verified at startup checks. Rounded CIM timestamps never replaced the clock.
- One disposable Windows Server 2022 VM; guest-only automation. No host input, clipboard, desktop focus, local SSMS,
  Docker, default cloud configuration or unrelated resources were used or changed.
- One complete baseline, then an update of the same installation and one complete after phase only if the baseline
  review accepted it. No extra precheck, continuation, snapshot or restart-test phase was added.

All seven prior checkpoint hashes still match the preflight record. Their original clocks, failed outcomes, blocked
preparation and snapshots remain intact. Older approval-blocked key sets were untouched.

## External automation correction and live observations

The preceding attempt required the connection confirmation's Yes control to have the UIA Button type. SSMS exposed the
native control as a Pane. The external adapter now validates the expected server/disposable database, Windows
authentication, owned dialog and native Yes/No HWNDs, PID, root/parent, class, visibility and enabled state. It deduplicates
only identical observations of one HWND, rejects foreign/conflicting/stale controls, revalidates identity immediately
before a bounded native click, and retains native/UIA observations. A click alone never proves readiness or execution.

Before this launch, 21 connection checks, 17 PowerShell package/queue checks, 16 independent package checks and three
short owned-child fixtures passed. A real private-desktop control exposed as UIA Pane received the native click; foreign
ownership and a destroyed handle were rejected. The failed-phase reviewer also retained the prior original error instead
of failing on a success-only `Panes` field. These were harness checks, not live SSMS acceptance.

This VM retained **two verified-before-click / click-sent pairs** for the owned native confirmations. Subsequent
connected-editor, raw SQL/XE, scope, target and query-session evidence corroborates execution; the clicks were not used
as the acceptance oracle. Both input and multi-window stages exited 0, as did smoke and native settings stages.

## Why the upgrade stopped

`Test-Upgrade-Phase.ps1` saves the combined `summary.json` using `Set-Content` without an explicit encoding. In this
Windows PowerShell 5.1 guest the combined file contains Windows-1252 punctuation, including byte **0x96 at offset 2871**.
`Review-Upgrade.py` reads JSON as `utf-8-sig`, so the original before review raised `UnicodeDecodeError` while loading
that file. The controller correctly refused continuation and cleaned up.

The guest summary says PASS and contains all eleven rows, but the original independent review remains **0/11,
INCOMPLETE** because it stopped before the case loop. That record, its log and all source evidence bytes are unchanged.
No completed live phase review was rerun. A separate read-only final audit interpreted only the known non-UTF-8 summary
as Windows-1252 for diagnosis and independently checked the original UTF-8 case/oracle/UIA/XE files. It corroborated
11/11 baseline observations without changing the campaign outcome or enabling an update.

This is an evidence serialization/reviewer incompatibility, not a demonstrated extension execution failure. It also
means the requested upgrade question remains unanswered. Before any separately authorized retry, the external summary
writer needs explicit UTF-8 and a cross-language round-trip check containing real scope/connection punctuation and
Unicode settings. No further campaign or fixture was launched automatically.

## Independently checked baseline evidence

- Exact installed display version **22.9.2**, executable build **22.9.12120.119**; installer reported 3010. SQL setup
  completed with exit 0, and the installation wrapper completed before the baseline began.
- Five raw semantic pane states: green SELECT, unknown procedure, permanent write, allow-listed object and trusted procedure.
- Six native values on **both complete forms: 12 distinct controls**, with runtime identity/ancestry checks; six persisted
  settings matched the seeded baseline. Final capture was closed and read-only.
- All **27 frozen package members**, their original paths and hashes matched the VSIX and baseline. The same profile and
  installation identity were retained. Four named SSMS cache/metadata files were recorded separately, with **zero deltas**
  in this run. These are before-phase observations, not across-upgrade comparisons.
- Original before/after SQL oracle snapshots, counter values, exact batch text/hash, submission count and raw XE events
  were checked for every case. XE snapshots reported no truncation or dropped events. PID/application/database/session
  attribution, visible scope and dialog connection context agreed. Cancel cases made no guarded SQL submission.

| Baseline scenario | Guarded submissions | Counter delta (red, yellow, outside) |
|---|---:|---|
| Selected red Cancel | 0 | 0, 0, 0 |
| Selected red Run | 1 | 1, 0, 0 |
| Selected yellow Cancel | 0 | 0, 0, 0 |
| Selected yellow Run | 1 | 0, 1, 0 |
| Whole window Cancel | 0 | 0, 0, 0 |
| Whole window Run | 1 | 1, 1, 1 |
| Selected green | 1 | 0, 0, 0 |
| Toolbar red Cancel | 0 | 0, 0, 0 |
| Toolbar red Run | 1 | 1, 0, 0 |
| Window B Run | 1 | 0, 0, 1 |
| Return to window A Run | 1 | 1, 0, 0 |

The green case had no confirmation dialog; its editor connection/session title and raw oracle establish context.
Window B submitted once in its distinct query session; returning to A submitted once in A's session. Seven guarded
submissions were observed in total. Fixture database and XE-session cleanup were independently recorded.

## Export integrity, clocks and cleanup

Both exported archives passed SHA256, CRC, duplicate/case/path-containment, manifest length/hash, complete member and
extracted-byte checks. No unexpected files were present. The manifest itself is the sole member outside its own file list.

| Archive | Manifest files / archive members | SHA256 |
|---|---:|---|
| Baseline phase | 219 / 220 | `B3B3AEC0AB6639237E63CD2C34E4AD42FF9EB4DCE5BDDF5A0CE76FC2C7B32CCC` |
| Final export | 252 / 253 | `C7970442B80714B116505B477BC7EB2DDFC9D2A04749A6874D48B5CAC558970E` |

Absolute VM auto-delete remained **17:16:11 UTC**; execution/export reserve began **16:51:11 UTC** and final cleanup was
required by **17:01:11 UTC**. None was extended. Successful **pre-unlink inventories at 14:58:27 UTC** showed no remaining
VM, disk, snapshot, address, owned firewall, custom network or subnet. Project billing was then unlinked. No post-unlink
billing-disabled Compute query was used as new cleanup proof. The exact owned IAP process tree and listener 22433 were
verified absent at **15:05:11 UTC**. The same monitor is disabled with no next run.

Automatic approval review rejected deletion of this run's four generated local key files as **"blocked by policy"**,
without a specific reason. They remain; no bypass or older-key deletion was attempted. The existing billing account
and cumulative USD 5 budget retain their original start/end/alerts. Actual cost is unknown/pending; the budget is not a cap.

The final campaign checkpoint retains original identity, prior/latest samples, actual scheduler receipts, final audit,
cleanup evidence and qualification. Focused DocsIndexTests and `git diff --check` results are retained in its finalization
receipt. No product edits, commit, push or publication were made.

## Coverage limits

No after-upgrade settings, execution, package comparison or cache deltas exist. SQLCMD-on and deliberate ambiguous-target
cases are absent from the eleven-case core. Guest SendInput F5 is not human physical-key input; Server 2022 is not Windows
11 acceptance. The published Release package provides no internal guard text hash; exact external expected SQL, displayed
scope and paired server submissions were checked. No post-update repair, cache refresh, reinstall or settings rewrite
occurred because the update never ran.
