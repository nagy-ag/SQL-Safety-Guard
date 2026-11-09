> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.5–22.10 candidate continuation — November 4, retry2

This bounded run used the frozen **unreleased SQLCMD compatibility candidate**, not the published VSIX. It completed
eleven strict execution cases on 22.5.2 and nine on 22.6.0. An external query-tab helper blocked the two remaining 22.6
cases, so the controller exported evidence and cleaned up. Four later minor representatives were not installed or tested.
No production code, analyzer policy, target assertions or package changed during this run.

| Requested representative | Actual installed full build | Strict candidate execution | Other evidence and gaps |
|---|---|---|---|
| 22.5.2 | 22.5.11723.231 | **11/11 PASS**: seven guest-injected F5, two actual Execute toolbar, two-window B/return-A | Three semantic panes and basic settings PASS; Release uninstall command INCOMPLETE |
| 22.6.0 | 22.6.11806.211 | **9/11 PASS**, two window cases INCOMPLETE: seven guest-injected F5 and two actual toolbar cases passed | Three semantic panes PASS; settings/uninstall not reached |
| 22.7.2 | Not installed | INCOMPLETE | Not tested; not an unavailable-version finding |
| 22.8.2 | Not installed | INCOMPLETE | Not tested; not an unavailable-version finding |
| 22.9.2 | Not installed | INCOMPLETE | Not tested; not an unavailable-version finding |
| 22.10.2 | Not installed | INCOMPLETE | Not tested; not an unavailable-version finding |

The actual catalog display versions were 22.5.2 and 22.6.0; neither bootstrapper silently substituted another minor line.
Both SSMS installers returned 3010, followed by a normal guest restart before VSIX installation. The earlier 22.5 VSIX
timeout did not reproduce in this fresh-guest sequence. This establishes a successful installation, not the timeout's sole cause.

## What the strict passes demonstrate

The seven F5 scenarios covered selected green SQL, selected red Cancel/Run, selected unknown-procedure Cancel/Run and
whole-window Cancel/Run. The toolbar scenarios covered red Cancel/Run. The two additional 22.5 scenarios executed window B
and returned to A. These inputs used Windows SendInput only inside the owned guest; they are not human physical-key tests
or command-dispatch substitutes for the actual toolbar.

Independent offline review compared each available result with before/after SQL-oracle batches, exact intended SQL/hash,
zero-or-one submission count, counter changes, verified target/context and raw Extended Events matched to the owned process,
session and actual SSMS query application. Cancel cases submitted zero SQL; approved cases submitted the intended SQL once.
All twenty available cases passed this review. Missing two-window evidence on 22.6 remains INCOMPLETE.
UIA selection text and the internal Release guard hash were unavailable; the paired Run/server SQL evidence and displayed
scope establish the recorded selection checks. No UIA TextPattern crash retries were added.

Green, yellow and red pane controls were recorded for both installed builds. On 22.5, the settings pane and integrated F5
warning setting loaded; values were not changed. Its external ROT/DTE uninstall command was rejected with
`RPC_E_CALL_REJECTED`; the actual confirmation did not appear. Cancellation, handoff and removal are therefore INCOMPLETE,
even though that helper exited normally. On 22.6 these settings/uninstall stages were not reached.
Deliberately ambiguous/unknown execution-target coverage, broader Debug routing and candidate 22.11 runtime regression remain
incomplete. No new themes, full analyzer/server campaign or published-package requalification was performed.

## Exact blocker and bounded follow-up

The 22.6 helper error was `Previously bound target tab unavailable after two bounded navigation steps`.
The preserved log shows selected `SQLQuery2.sql` alternating with the original document. The helper had discovered the new
tab using the generic prefix `SQLQuery`, then its stricter selector required `SQLQuery` followed immediately by ` - `.
That cannot match `SQLQuery2.sql - …`. This is a concrete external helper filename mismatch, not evidence of a product
SQLCMD/target-resolution defect. The second-window and return-A execution checks never ran.

After the user explicitly requested another attempt, an external-only correction captured the exact generated filename
from the unique connected query tab before the existing process/database/focus/runtime-ID ownership proof. Seven focused
offline checks passed, including the saved mismatch and malformed-name refusal. The frozen candidate and all strict SQL,
count, counter, text, context and target assertions remained unchanged. Live acceptance belongs to the separate subsequent
22.6–22.10 run; these preparation tests do not fill the missing two cases here.

That subsequent startup first encountered delayed Compute API billing propagation before any VM was created. Its empty
resource/billing-unlink receipts were retained. A read-only, five-minute propagation check was added; only the explicit
billing-propagation error retries, and unrelated errors stop immediately. Three focused offline checks passed. A separately
monitored attempt was then started under the same user authorization. Neither retry resets the original continuation clock.

## Provenance and retained evidence

Base source: `5e8c7f20c33c611b93846465602f1cd15669c4f2`, plus the prior isolated, uncommitted SQLCMD candidate.
Candidate source-patch SHA-256: `EEC3142B5A6B82C0A2F603523B439ECF5E3BFE7DA42998501AD0ED05A0699612`.
Frozen candidate Release VSIX SHA-256: `C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC`.
Embedded Release assembly SHA-256: `B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0`.
The prior candidate gate remains 1,627 .NET / 10 Python / 527 rules and Debug/Release builds PASS; it was not rerun here.
Testing helpers stayed outside release packages. Production still opens no SQL connection, executes no SQL and queries no metadata.

Raw evidence remains outside the repository in the owned `ssms225-2210-r2-20261104-0845` workspace. It includes exact installed
version receipts, installer logs, each case's oracles/raw XE/UI controls, original errors, cleanup receipts and
`independent-minor-review.json`. Archive hashes match the export receipts; extracted per-version files were also compared
byte-for-byte with their ZIP members. The initial offline review incorrectly stopped at an incomplete row count; that output
was preserved before correcting the external reviewer to validate available rows while keeping missing rows incomplete.

| Artifact | SHA-256 |
|---|---|
| `evidence-22.5.2.zip` | `07DC0174A70439DB8789EFCFE8F495D5609C05971C12746D6E9945662BD17CD8` |
| `evidence-22.6.0.zip` | `5EC16A9DB4AF0AC693A955F9B3CB37AE57122B1C6D6CC9D405FDABC085464812` |
| `minors-final.zip` | `9CCD90A57899537A47DB32D520C2BCA049DC2D9D5764F3BC9239C96A1A134A4B` |
| 22.5.2 bootstrapper | `48044F0377D8387A50442C6D7093D3B2A55735B2B1FF0B680C452A64FADA2636` |
| 22.6.0 bootstrapper | `7B7E3D2C029A74AF8BA86A2E6FE1CDABECB39732AD041F6D25D15BF89F2913A4` |

Original continuation start: **2026-11-04T08:12:23.1308041Z**. Retry2 started **08:46:17.3470521Z**; its controller finished
**09:46:40.6647756Z**. Historical November 3 campaign/sequence and November 4 focused-retry clocks were preserved.

## Cleanup for this recorded run

Each execution helper dropped its owned SQL database and XE session and exited its SSMS process. Final cloud receipts
verified no VM, disks, snapshots, addresses or owned network/subnet/firewall at **09:46:37Z**; project billing was unlinked.
The exact owned tunnel tree was identified by PID/start/command line and closed at **09:52:43Z**, with listener absence
verified. This run's private/public SSH keys were removed separately at **09:52:50Z**. Older key sets blocked by automatic
approval review were left untouched for manual cleanup. The monitor was disabled, then reused for the user's separately
authorized follow-up; no old controller was restarted. SSD contents and all evidence were preserved.

The cumulative $5 budget/$4.50 alert was not reset and is not a spending cap. The billing account/budget remain for delayed
charges; actual billed cost is pending. Subsequent live resources are outside this finalized run's cleanup claim and are
tracked in current status. No commit, push or publication was performed.
