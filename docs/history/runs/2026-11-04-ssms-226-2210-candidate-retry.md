> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.6–22.10 frozen candidate retry — November 4

All five requested minor representatives completed with the **same frozen, unreleased SQLCMD compatibility candidate**.
Independent review established **55/55 strict execution cases PASS**, three representative semantic panes and basic settings
on each installed build. Release uninstall, deliberately ambiguous execution targets and broader Debug coverage remain
INCOMPLETE. This is focused candidate evidence, not a blanket 22.x compatibility or published-VSIX qualification.

| Requested / actual display version | Actual full build | Strict execution | Pane/settings evidence | Release uninstall |
|---|---|---|---|---|
| 22.6.0 | 22.6.11806.211 | 11/11 PASS | 3/3 PASS; basic settings PASS | INCOMPLETE |
| 22.7.2 | 22.7.11919.86 | 11/11 PASS | 3/3 PASS; basic settings PASS | INCOMPLETE |
| 22.8.2 | 22.8.12023.21 | 11/11 PASS | 3/3 PASS; basic settings PASS | INCOMPLETE |
| 22.9.2 | 22.9.12120.119 | 11/11 PASS | 3/3 PASS; basic settings PASS | INCOMPLETE |
| 22.10.2 | 22.10.12217.157 | 11/11 PASS | 3/3 PASS; basic settings PASS | INCOMPLETE |

The separate [prior continuation](2026-11-04-ssms-225-226-candidate-continuation.md) retains **22.5.2 / 22.5.11723.231:
11/11 PASS** with this same candidate. It was not repeated here. That run's original 22.6 partial results and failures
remain unchanged; this retry supplies the new eleven-case result, including both formerly incomplete window cases.
Every actual installed display version/full build was read back. No servicing patch or minor substitution occurred.

## Scenarios and proof

Each eleven-case set contained seven primary guest-injected F5 cases: selected green, selected red Cancel/Run,
selected unknown-procedure Cancel/Run and whole-window Cancel/Run; two actual Execute toolbar red Cancel/Run cases;
and two additional injected-F5 window B/return-to-A cases. Guest Windows SendInput is not human physical-key coverage.
The actual toolbar was invoked through UI Automation, not replaced by command dispatch.

Before/after SQL-oracle batches, intended SQL hashes, counters, target/context flags and raw Extended Events were reviewed
for every case. The external oracle was bound to the owned SSMS PID, SQL session IDs and actual recognized query application.
Cancel produced zero SQL submissions; each approved Run submitted the intended SQL exactly once, with only its expected
counter changes. Window B used a different session; return to A used the original session:

| Build representative | A session | B session | Return-A session |
|---|---|---|---|
| 22.6.0 | 54 | 55 | 54 |
| 22.7.2 | 53 | 80 | 53 |
| 22.8.2 | 70 | 75 | 70 |
| 22.9.2 | 54 | 58 | 54 |
| 22.10.2 | 53 | 76 | 53 |

The raw green, yellow and red pane controls and clean SSMS exits were also checked. Basic settings evidence means the
settings controls and integrated F5 warnings loaded; setting values were not changed or persistence-tested.
All execution helpers dropped their owned disposable database and XE session. Product code still opens no SQL connection,
executes no SQL and queries no server metadata.

UIA selection text and the internal Release guard hash were unavailable. Displayed scope and paired Run/server SQL evidence
support the recorded selection checks; no UIA TextPattern crash retries were added. SQL execution was confined to the owned
throwaway guest databases. No full analyzer/server campaign, themes, previews or performance work was repeated.

## What was corrected, and what remains incomplete

The prior 22.6 blocker was external: the helper discovered `SQLQuery2.sql` under the prefix `SQLQuery`, then demanded that
the prefix be followed immediately by ` - `. The correction captures the exact generated filename from the unique connected
tab before the existing process/database/focus/runtime-ID binding. Seven focused offline checks passed, and this run proved
window B and return A on all five hosts. No product behavior or strict SQL/count/counter/context/target assertion changed.

The retained IAP startup correction waited through the complete fifteen-minute Windows SSH readiness window. A subsequent
pre-VM attempt hit billing propagation: the billing API said linked, while Compute still rejected billing-disabled requests.
That attempt created no VM and was cleaned. A read-only, five-minute Compute readiness check now retries only that explicit
propagation error; three focused checks passed. This run's first billing probe succeeded. Historical failed attempts and
their evidence remain intact. The initial connection resets/console-not-ready observations were transient startup records.
One separate read-only status query hung across a normal guest restart; only that uniquely identified SSH query was
cancelled, with no test-controller or guest-job restart. That observation remains incomplete.

On every host, the external ROT/DTE uninstall command received `RPC_E_CALL_REJECTED`, and its actual confirmation did not
appear. Cancel, installer handoff and removal remain **INCOMPLETE**, despite normal helper exit. Earlier Debug uninstall
acceptance is a separate build/profile result. No deliberate ambiguous-target test was established; broad Debug routing,
the earlier separate Debug return-A channel gap, candidate 22.11 runtime regression and earlier candidate minors remain
untested/incomplete. Previous published-package target failures are not relabelled by this candidate's passes.

## Frozen source, packages and archives

Base source: `5e8c7f20c33c611b93846465602f1cd15669c4f2`, plus the prior isolated, uncommitted SQLCMD candidate.
Source-patch SHA-256: `EEC3142B5A6B82C0A2F603523B439ECF5E3BFE7DA42998501AD0ED05A0699612`.
Candidate Release VSIX SHA-256: `C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC`.
Embedded Release assembly SHA-256: `B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0`.
The prior candidate gate remains **1,627 .NET / 10 Python / 527 rules, Debug and Release builds PASS**; no rebuild or full
gate was repeated per host. Testing helpers stayed external to the release package. Final documentation checks are recorded
in the external docs-test receipt.

Raw outputs remain outside the repository in the owned `ssms226-2210-r4-20261104-1000` workspace: installed-version receipts,
per-version archives/expanded evidence, raw SQL/XE/UI controls, settings/uninstall errors, controller/queue logs,
`independent-export-review-*.json`, `compatibility-matrix-final.json` and cleanup receipts. Cached reviewed results were
reused; only newly exported versions were reviewed at each check. Per-version ZIP members were compared byte-for-byte
with their expanded evidence. All 785 members of the final archive were also byte-verified.

| Representative | Evidence ZIP SHA-256 | Microsoft bootstrapper SHA-256 |
|---|---|---|
| 22.6.0 | `02F425EFB0486687B280E6ECE675014E6C38EDC1263651EF06317E38C8D1CC5A` | `7B7E3D2C029A74AF8BA86A2E6FE1CDABECB39732AD041F6D25D15BF89F2913A4` |
| 22.7.2 | `18A9F7B448873248A34EE418DBE44C6CBEF32EF025714D61CB47008F9C4C8189` | `398CC5C7919BF1BE33B9BFB876831F2BD324D376136A902912B1FE45998FF3CA` |
| 22.8.2 | `10A6A9637EACCCBB3072B5ED0C5D3731F25B5F51481409A6C73B4FCE1C9C67C1` | `48E041E0C0733E22D9035A085CFEA82370C9CA1DC0DCC8F8D2717453F8B5E35F` |
| 22.9.2 | `DBF48142D15CAF24E9C1C0018AF175E0E47F1FAC89E96332B89201469487D232` | `3826637010859431029327F8CB8D63608166DA459BD5CA3DA5AD524C80892270` |
| 22.10.2 | `C8366B437264ACF580737D2BB97C6D3CCCC571EF456A340C6F481A4A86EB1316` | `E7B3885D3A0FEBB83B7A5CB155EFD8410A9A651A1BE16EAAA35D594C4D1D75D2` |

Final `minors-final.zip` SHA-256: `3046754E81718CE24373559A6CD20DF7F5AC568C44FE2E175311B95389158F06`.
Each archive hash matched its guest export receipt. No raw archive or run output was added to the repository.

## Timing, cleanup and retained limits

Original continuation start **2026-11-04T08:12:23.1308041Z** was preserved. This attempt started **10:00:30.9747016Z**;
final candidate checks ended **12:08:21Z** and the controller completed **12:12:44.8708500Z**. Including earlier attempts,
the continuation reached cleanup in approximately four hours. Historical November 3 and focused November 4 clocks were retained.

The VM's fixed **13:02:13Z** auto-delete was never extended. Final export finished before cleanup, and receipts verified
empty VM/disks/snapshots/addresses/owned network/subnet/firewall at **12:12:40Z**. Project billing was unlinked. The exact
owned IAP tree was checked by PID/start/command line/VM identity and closed; process and listener absence were confirmed
at **12:29:39Z**. Its private/public SSH keys were removed separately at **12:29:46Z**. The monitor is disabled with no next run.
Older key sets previously blocked by automatic approval review remain untouched for manual cleanup. Host desktop/input,
default SSMS, Docker, persistent default gcloud configuration/project, other agents and SSD contents were preserved.

The existing cumulative **$5 budget / $4.50 alert** was not reset and is not a spending cap. The billing account/budget
remain for delayed charges; actual billed cost is pending. No owned paid test resource remains. Previous reports/server
evidence were preserved. No commit, push or publication was performed, and no earlier-minor/newest follow-up was started.
