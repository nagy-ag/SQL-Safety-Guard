> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.0–22.3 frozen candidate continuation — November 4

All four requested minor representatives completed with the **same frozen, unreleased SQLCMD compatibility candidate**.
Independent review established **44/44 strict execution cases PASS** and three representative semantic panes on every build.
Basic settings controls passed on 22.0–22.2; the 22.3 integrated settings check is **INCOMPLETE**. Release uninstall,
deliberately ambiguous execution targets and broader Debug coverage remain incomplete. These are focused candidate results,
not published-package acceptance or a blanket SSMS 22.x qualification.

| Actual display version | Actual full build | Strict execution | Panes | Basic settings | Release uninstall |
|---|---|---|---|---|---|
| 22.0.0 | 22.0.11205.157 | 11/11 PASS | 3/3 PASS | PASS: controls loaded | INCOMPLETE |
| 22.1.0 | 22.1.11304.174 | 11/11 PASS | 3/3 PASS | PASS: controls loaded | INCOMPLETE |
| 22.2.1 | 22.2.11415.280 | 11/11 PASS | 3/3 PASS | PASS: controls loaded | INCOMPLETE |
| 22.3.3 | 22.3.11527.330 | 11/11 PASS | 3/3 PASS | INCOMPLETE: F5 warnings controls | INCOMPLETE |

Requested bootstrappers were 22.0, 22.1, 22.2.1 and 22.3.3, one representative for each released minor before 22.4.
The [Microsoft release history](https://learn.microsoft.com/en-us/ssms/release-history) was consulted again; existing
frozen bootstrappers were reused, hash/signature-checked in the guest, and installed display/full builds read back.
The 22.0/22.1 display strings were 22.0.0/22.1.0. No minor or servicing patch was silently substituted.
The [prior 22.6–22.10 retry](2026-11-04-ssms-226-2210-candidate-retry.md),
[22.5 result](2026-11-04-ssms-225-226-candidate-continuation.md) and
[focused 22.4 retry](2026-11-04-ssms-224-return-window-retry.md) remain separate immutable reports.
No 22.4-or-later test was repeated. Candidate 22.11 runtime regression remains untested; its earlier published-package
passes are for a different package.

## Scenarios and proof

Each eleven-case set contained seven primary guest-injected F5 cases: selected green, selected red Cancel/Run,
selected unknown-procedure Cancel/Run and whole-window Cancel/Run; two actual Execute toolbar red Cancel/Run cases;
and two further injected-F5 cases for window B and return to A. Windows SendInput inside the owned guest is not human
physical-key coverage. The actual toolbar used UI Automation Invoke, not command dispatch.

Independent review checked raw Extended Events, before/after oracle batches, exact intended SQL hashes, submission
counts, counter changes, verified-target/context flags and query sessions. Cancel submitted zero SQL. Approved Run
submitted the intended SQL exactly once and changed only its expected counters. Window B used a distinct session;
return A used the original session:

| SSMS | A session | B session | Return-A session |
|---|---|---|---|
| 22.0.0 | 82 | 51 | 82 |
| 22.1.0 | 77 | 51 | 77 |
| 22.2.1 | 69 | 73 | 69 |
| 22.3.3 | 69 | 54 | 69 |

The external oracle binds the actual recognized application name to the owned SSMS PID, database and query-tab session.
All owned SQL databases/XE sessions were cleaned. SQL access is confined to external development helpers in the disposable
guest; production still opens no SQL connections, executes no SQL and queries no server metadata.
UIA selection text and the internal Release guard hash were unavailable. Displayed scope and paired Run/server SQL
evidence support the selection checks; no UIA TextPattern crash retries were made.

All green/yellow/red pane controls and clean SSMS exits passed. Basic settings acceptance means controls loaded,
without changing settings or proving persistence. On **22.3.3**, the helper recorded `Integrated native settings did not
expose F5 warnings.` Its settings acceptance remains incomplete; normal driver completion does not override that result.
On every version, actual uninstall confirmation was absent, so confirmation cancellation, installer handoff and removal
remain incomplete. Some raw summaries set an uninstall Cancel flag to PASS; without actual confirmation evidence that
flag is not accepted as cancellation proof. Earlier Debug uninstall acceptance remains a different build/profile result.
No deliberate unknown/ambiguous execution-target scenario or broader Debug campaign was added. Unknown-procedure
execution cases do not establish ambiguous-target behavior. No theme, preview, performance or analyzer/server campaign was repeated.

## External verification correction and preserved limitations

The live helpers and frozen candidate were unchanged. The existing exact-filename navigation correction retained
process/database/focus/runtime-ID binding and all SQL/count/counter/context/target assertions; it had seven offline
checks and prior live proof. No new product behavior, dependency or fallback was added in this continuation.

The initial independent 22.0 review marked five of eleven cases complete because its offline checker hardcoded the
newer application label `Microsoft SQL Server Management Studio - Query`. Older windows also used the already recognized
`SQL Server Management Studio`; the same 22.0 process used different recognized labels on its two registered sessions.
The frozen live oracle had already captured exact registered identities. Only the offline reviewer was corrected to
require each batch's exact registered application/session, owned PID and database. It does not accept a generic global
application match. Nine focused checks passed, covering both recognized labels, mixed sessions, and rejection of wrong
PID/database/application, unregistered/conflicting sessions and SQL errors. Raw SQL equality, counts, counters, context,
target and exact raw-XE matching assertions remained unchanged.

The original scripts and initial incomplete review were retained externally. Corrected review independently established
11/11 for 22.0; each later export was reviewed once with that checker. The initial final-matrix verification also stopped
when it assumed every settings check had passed; the final matrix instead retains 22.3's actual incomplete result.
No missing or failed evidence was promoted to PASS. Startup console-not-ready/old-console-identity messages cleared
within the existing bounded readiness waits; no controller, VM or queue was restarted by the monitor.

## Frozen source, packages and evidence

Base source: `5e8c7f20c33c611b93846465602f1cd15669c4f2`, plus the prior isolated, uncommitted SQLCMD candidate.
Source-patch SHA-256: `EEC3142B5A6B82C0A2F603523B439ECF5E3BFE7DA42998501AD0ED05A0699612`.
Candidate Release VSIX SHA-256: `C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC`.
Embedded Release assembly SHA-256: `B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0`.
The prior candidate gate remains **1,627 .NET / 10 Python / 527 rules and Debug/Release builds PASS**; no rebuild or
full gate was repeated. Helpers stayed outside release packages. The final four DocsIndexTests are recorded in the
external docs-test receipt; no full suite was repeated for documentation-only edits.

Raw output remains outside the repository in `ssms220-223-20261104-1240`: installed-version receipts, per-version ZIPs,
expanded raw SQL/XE/UI evidence, queue/controller logs, settings/uninstall gaps, initial failed offline review,
`offline-review-correction.json`, nine-check receipt, independent export-review receipts, final matrix and cleanup receipts.
Previously reviewed versions used cached receipts. Per-version ZIP members and all **650**
final-archive members were compared byte-for-byte with their expanded evidence.

| SSMS | Evidence ZIP SHA-256 | Microsoft bootstrapper SHA-256 |
|---|---|---|
| 22.0.0 | `AFF93138894B4A51DBD8052805C7317003215FF3AADE2C901F25435623119CC7` | `B50E52759454345AB17BDAFCB40774F9BC635CF8FD933DACC4097909F3DBCCF7` |
| 22.1.0 | `D47BC7CFCBA21D18BC5DD36A4F0E3280EA482A4090300AF30300428F59DFF6A0` | `4A7EDA53A20FECF4D012D8B845BF4B7E2C94A4BCED41F979B85CDEE1B517021F` |
| 22.2.1 | `4F5B21BCD39E10176E1B001D906A7EB289906294D50A29575F94039EF1DB12C5` | `75159ABCE36103C35F173E03B7B3EC0B004094013968F5980D77A4C0436E36C7` |
| 22.3.3 | `9BA6786CBEB7DAAEA89A6E29E58565B0A549A7FAAF12E8AAA035EEC4C49C1DD2` | `58EFA2590BF79FCEA84F35CB6D0C5F6B852F7AA21A8E40939D1FDFE9CBA80650` |

Final `minors-final.zip` SHA-256: `CD0EC819DD08C852045ED38B0D49108E2CE7ABCFA1F0E327D4B7E5F7ADC5FA25`.
Archive hashes matched guest export receipts. No raw run output or archives were added to the repository.

## Timing and cleanup

This earlier-minor phase began **2026-11-04T12:40:03.354152Z**; its controller attempt began **12:40:52.9321713Z**.
Final checks ended **14:16:07Z**, export completed **14:16:42Z**, and the controller completed **14:19:21Z**,
approximately one hour forty minutes after this phase began. Historical November 3 and November 4 clocks were preserved.
The fixed **15:44:48Z** VM auto-delete was not extended; final export and cleanup finished well before the reserve deadlines.

Cleanup receipts verified no VM, disks, snapshots, addresses or owned network/subnet/firewall at **14:19:16Z**.
Project billing was unlinked. The owned IAP tree was checked by exact PID/start/command line/VM ID and closed;
process/listener absence was verified at **14:29:23Z**. The monitor is disabled with no next run. No owned paid test resources remain.

Automatic approval review rejected the command containing this run's private/public SSH key deletion, with only
`blocked by policy` and no detailed reason. It was not retried or bypassed. Current keys and older approval-blocked key
sets remain pending manual deletion; the VM has been deleted. All evidence and SSD contents were preserved. The host
desktop/input, default SSMS, Docker, persistent default gcloud configuration/project and other agents were untouched.

The existing cumulative **$5 budget / $4.50 alert** was not reset and is not a spending cap. The account/budget are retained
for delayed charges; actual billing cost remains pending. Prior host/server/publication evidence and reports are preserved.
No commit, push or publication was performed, and no further test run is scheduled.
