# Server campaign validation — 2026-10-28

This is infrastructure/smoke evidence, not proof of every analyzer rule or combination. The analyzer was not changed. Outputs are outside the checkout under ..\harness-out\campaign\2026-10-28; generated SQL names and timings remain in results.jsonl and report.md. No scheduled task was registered.

## Tested build and environment

* Analyzer Release SHA-256: 7E4C3C8944136E9699E3AF1D2E8D5E4A5A5313858B69D14967AF64302D0DF652.
* Harness Release SHA-256: 4C607A2D4C407AFBF11BF259859C4E0A967D1A2A4A1B784D775513E828962B75.
* Both Release builds succeeded with zero warnings/errors. dotnet test tests\SqlSafetyGuard.Tests -c Release --nologo: 824 passed, zero failed/skipped, 8 seconds on the final run.
* Local .\SQLEXPRESS reports Enterprise Developer Edition (64-bit), SQL Server 17.0.1000.7; its instance name does not imply Express edition. Docker probes used mcr.microsoft.com/mssql/server:2025-latest in verified disposable, loopback-only, unmounted containers.
* All three campaign PowerShell scripts parsed successfully under Windows PowerShell 5.1. The isolated wrapper also executed successfully under that shell, including its finally cleanup.

## Final shared-instance smoke

Output: smoke-release. Campaign identity: 5711286031FD1E98B7B2F1CA287E9E2044B0B4D5E4B25226B248113C8C2E1303.

50/50 cases completed with two workers, shrink-attempts=8 and SQL timeout=10 seconds. UTC execution window: 14:18:43.5276–14:22:42.0185, approximately 238.49 seconds including provisioning, observations and shrink trials.

| Outcome | Real cases | Negative controls |
|---|---:|---:|
| PASS | 39 | 0 |
| FALSE GREEN | 0 | 1 |
| WRONG TARGET | 6 | 0 |
| NOISE | 4 | 0 |
| CRASH | 0 | 0 |
| TIMEOUT | 0 | 0 |
| NOT EXECUTABLE | 0 | 0 |

Per-case total elapsed milliseconds: min 5159.1, median 6087.0, p95 11692.2, max 12926.4. These include four-database restore/cleanup and observation, not only SQL time. The report includes individual SQL/analyzer/total times. Thirteen shrink trials consumed 87342.5 worker-ms in aggregate.

The six target diagnostics are write-alias, write-view, write-instead-of, write-synonym, write-cross-synonym and broker-send. Their physical destinations appear in the evidence; these are attribution diagnostics, not false-green classifications. Noise cases are read-procedure, session-context, session-options and write-zero-rows. Session context/options/impersonation/open-transaction differences are independently retained.

The deliberately forced-green control retained the actual analyzer verdict and produced the expected FALSE GREEN (CONTROL). Five fresh-database shrink trials reduced its three-statement original to:

~~~sql
UPDATE dbo.Counter SET Value=Value+1 ;
~~~

This is a locally minimized, rerun-proven control, not an actual analyzer defect. Repeating the identical final smoke command loaded the same 50 persisted results, executed zero cases/trials, and regenerated the report. Reports identify the current invocation start, so that resume replaces the original invocation window; the original window is preserved here. Exit 1 is expected because real target diagnostics remain.

## Final Docker feature probes

Output: isolated-sqlguard-cov-1ec666ad027f471c. Campaign identity: 936894C721A3984D1BE62A93C777DE81435FD166CAD758E4FEE58B8FC3170362.

8/8 completed, one worker. UTC 14:23:47.4642–14:25:58.2182, approximately 130.75 seconds. Five PASS, one WRONG TARGET (OPENQUERY write), one NOISE (OPENQUERY read), one intentional TIMEOUT (15-second WAITFOR with a 10-second deadline); zero FALSE GREEN/CRASH/NOT EXECUTABLE. Total case milliseconds: min 10720.5, median 15193.6, p95/max 26048.4. One target shrink trial consumed 10701.4 worker-ms.

Observed evidence: backup changes container-file inventory; permanent tempdb table changes tempdb metadata/typed rows; server role/configuration changes server state; EXEC AT and OPENQUERY writes name the restored Core.dbo.Counter base table. The intentional timeout's SQL execution took approximately 10020.9 ms; its total includes cleanup/observation. Wrapper exit 1 reflects the diagnostic/intentional timeout, not infrastructure failure. Its container and password file were removed.

## Interruption and ownership checks

* Hard-kill recovery: a sole owned harness executing a two-minute WAITFOR was terminated after all four databases appeared. The durable reservation 93968a98fd4445e6b5fa8f5eb9fae3bb was recovered by --prepare-world, removing exactly its four databases/login and registry row.
* Budget cancellation: a two-minute WAITFOR with a one-minute budget exited cleanly after approximately 63.68 seconds, completed 0/1, left results.jsonl empty and returned exit 1. Cancellation is not saved as a completed execution failure, so the case can resume. This test exercised the final cancellation implementation before the unrelated typed-tempdb-row helper extraction.
* Incompatible binary/input/settings identity was explicitly refused when reusing an older output directory.
* Final local inventory contains only SgPlay_Core/Sales/Audit/Archive and SgRun_Results among campaign databases; pending owned-set reservations=0 and SgRun logins=0. Read-only inventory confirms 9 user tables and 406 seeded rows per template: 36 tables and 1624 rows total. Per database: two views, two triggers, two synonyms, two procedures, three functions, one sequence and one user queue/service.
* Final Docker label inventory (including stopped containers) is empty; no campaign .env password files remain. Four checked template backups intentionally remain in the instance default backup directory. Unrelated databases were not altered or dropped.

## Remaining scope

After the full smokes, final review hardened fixture identity against Git's Windows CRLF conversion. Only script-hash newline normalization changed; execution/oracle/shrinking semantics did not. The post-hardening harness Release SHA-256 is E10E2631C651FD13BDA9B2D9DBC500842E81961DB921D0EBA7AB555633B25467. A deliberately CRLF-converted build-output rebind.sql successfully reused the existing LF-pinned templates via --prepare-world. The focused line-ending-regression completed its forced-green control (7084.6 ms total case time), five shrink trials (26256.0 worker-ms), the same minimized UPDATE and then a zero-execution resume, all with exit 0 because the false green was explicitly a control. The full 50/8-case counts above refer to the explicitly identified earlier binary, not a claimed rerun of all cases after this portability-only change.

Read ORACLE.md for observation limits: volatile DMV evidence, transient/rolled-back metadata, asynchronous Broker, incomplete server catalogs/SET options and bounded file inventory. Permanent tempdb changes currently use a scope-level target rather than individual object attribution. SQL Agent, CLR, full-text, SHUTDOWN and arbitrary external effects were not exercised by these corpora. No SQL generator or exhaustive rule-to-corpus mapping is claimed; rule inventory/contract/no-server tests are separate work. Cross-container resume is not implemented because the disposable results database and container identity change. Shrinking is bounded and greedy, not a globally minimal proof.
