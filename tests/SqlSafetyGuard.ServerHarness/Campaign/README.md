# Real-server campaign

See ../../../docs/history/wave3/validation-wave3.md for wave-3 evidence and the frozen full-run command. VALIDATION.md and validation-wave2.md preserve historical results. The one-time task SQL Safety Guard Wave2 2026-10-28 is DISABLED and must remain disabled.

This is a separate mode, not legacy schema discovery and not a replacement for strict coverage. It executes trusted test SQL against worker-owned, pristine-verified four-database sets and persists evidence. Start with ORACLE.md and WORLD.md. The supplied 50-case smoke corpus is representative infrastructure validation, **not every analyzer rule/combination proven safe**. Per-case analyzer settings and an offline analyzer-only lane are supported; server execution still requires reviewed SQL and setup.

## Prepare / run

From the repository root in PowerShell 5.1 or 7 with .NET 10 and SQL sysadmin provisioning access:

~~~powershell
.\tests\SqlSafetyGuard.ServerHarness\Campaign\Initialize-World.ps1
.\tests\SqlSafetyGuard.ServerHarness\Campaign\Run-Overnight.ps1 -Workers 2 -BudgetMinutes 480
~~~

Initialize-World builds Release and invokes --prepare-world: create missing owned SgPlay_Core/Sales/Audit/Archive templates, bind cross-database objects, take checked COPY_ONLY backups. It does not run cases. Repeated preparation reuses complete marked templates. Script hashes normalize CRLF/LF before pinning in extended properties; semantic script changes require an explicit reviewed rebuild of those owned templates. Legacy complete campaign templates missing a hash are adopted once; foreign/unmarked or incomplete templates are refused, not dropped automatically. Backup files persist with the four templates; there are no snapshots of unrelated databases.

Direct invocation after builds:

~~~powershell
dotnet tests\SqlSafetyGuard.ServerHarness\bin\Release\net10.0\SqlSafetyGuard.ServerHarness.dll --analyzer src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0\SqlSafetyGuard.Analyzer.dll --campaign tests\SqlSafetyGuard.ServerHarness\Campaign\cases\smoke.jsonl --workers 2 --shrink-attempts 8 --out ..\harness-out\campaign\2026-10-28\smoke
~~~

--campaign accepts one JSONL file or a directory recursively containing JSONL files. Do not point it at mixed host/isolated corpora: privileged cases are rejected on the shared instance. --server defaults to .\SQLEXPRESS; --filter matches case IDs; --limit selects the first N after deterministic seed/id ordering. Workers default to cores/2 clamped 1..4, configurable 1..16. Each worker executes its cases and shrink trials sequentially. SQL execution has a linked per-case deadline (--timeout seconds, default 10); connection/provisioning/observation/cleanup have separate finite command timeouts, so the total case time includes restoring four databases and can exceed the SQL deadline.

Never run two harness processes at once. A Windows-session mutex protects all normal harness modes. Campaign additionally holds a session-owned SQL application lock in master, preventing campaigns from other Windows sessions against the same instance. Other harness modes/remote tooling do not take that SQL lock; operators must still serialize them. No persistent system setting enables unsafe concurrency.

## Case contract

One JSON object per nonblank line; property names are case-insensitive:

~~~json
{"id":"write-order","sql":"UPDATE sales.Orders SET Qty=Qty+1 WHERE Id=1;","setup":"","database":"Core","principal":"runner","expectedClassification":"RealWrite","ruleIds":["inventory-rule-id"],"seed":42}
~~~

Required: unique nonempty id (max400 characters), non-null sql (empty/comment-only SQL is valid). Optional defaults: setup empty, database Core (Core/Sales/Audit/Archive), principal runner (runner/app_user/low_user/admin), seed1. The admin principal uses the explicitly authorized ambient sysadmin provisioning identity; it is not a persistent new sysadmin login. expectedClassification is an exact analyzer classification assertion, not a replacement verdict. ruleIds are external inventory keys, not proof of internal branch coverage. Tolerant upfront validation writes validation.json and rejected.jsonl and continues with valid cases; one malformed case cannot abort a corpus. Setup runs as the provisioning account in the disposable context: **review setup SQL**, since this is a trusted corpus, not a hostile-code sandbox.

localProbeOptions accepts allowListPatterns, trustedProcedures, environmentRules, mode (Off/Red/YellowRed), server, sqlCmdMode, tempdbIsTemp, maxDynamicSqlDepth, currentDatabase and defaultSchema. Tokens expand before analysis. Execution uses the same owned database/schema context; currentDatabase cannot escape the four-database set. Typed confirmation is always disabled. SQLCMD mode supports bounded in-memory variable/directive preprocessing, never shell/include/connect/output-file commands. --campaign-offline runs analyzer assertions only; it executes no SQL. --campaign-validate performs loader checks only.

{{Core}}, {{Sales}}, {{Audit}}, {{Archive}} expand to the current case's restored names and {{seed}} to the integer seed. Avoid hardcoded generated names. Seed determines ordering and explicit SQL substitution, not SQL Server RNG/time/GUID behavior. Exact executed SQL and the original template are saved. All restored cross-database trigger/synonym references are rebound before setup. Case database context and impersonation are applied before measuring session baseline.

privileged=true requires verified Docker isolation and workers1. controlVerdict=ReadOnly deliberately forces a negative-control green while preserving the true analyzer verdict; controls are separately counted and excluded from real false-green rates and defect exit gates. Never use a control to claim an analyzer defect.

## Results, resume, recovery

SgRun_Results is a separately marked retained database. dbo.CampaignResults stores the full JSON payload keyed by SHA-256 campaign identity and case ID. An atomic external per-case receipt journal also permits cross-container resume for matching binaries, inputs, settings, world and Docker image digest. New containers are independently isolation-verified; container names are not reused as identity. Changes require a new output directory. Feature fixtures have their own atomic checkpoint with the same binary/input/image constraints. Resume never invents an unfinished result.

Default output is <repository>\..\harness-out\campaign\<local-date>\report.md plus results.jsonl and campaign-identity.json. Every 100 completions and clean termination rewrites JSONL and report atomically. The external journal is saved before SQL persistence and supersedes a stale SQL result on resume; SQL-only historical results remain usable. Failure evidence is saved before shrinking. A hard interruption during shrinking can retain the original failure without a minimized repro; rerun its SQL as a new case/identity to shrink again. Resume writes a current invocation report with historical per-case timings.

An owned-set registry in dbo.CampaignOwnedSets reserves a cryptographically random GUID before creating databases/login and records no password. Startup under the instance lease drops only the exact four derived names and login in outstanding registered sets, then removes the registry row. Names are validated and checked for preexistence before reservation. A failed cleanup keeps its reservation for recovery. **Never wildcard-drop SgRun_*:** preserve Results and other independently owned data. Templates and backups remain for later runs. Incomplete template provisioning requires manual inspection; it is never silently rebuilt.

--budget-minutes defaults480; Ctrl+C requests a clean stop. No further cases/trials start, active commands receive cancellation, and workers drain cleanup. Budget-cancelled TIMEOUT/CRASH cases are left uncompleted for resume. Other completed failures remain evidence. Hard process termination relies on registered recovery next time; leave cleanup margin in scheduler limits.

Outcomes: FALSE GREEN = green plus logical or physical mutation evidence; WRONG TARGET = an observed concrete write is not attributable; INDIRECT EFFECT = catalog-backed secondary writes; UNRESOLVED TARGET = opaque target; EXTERNAL CHANGE = unrelated database metadata interference only; NOISE = conservative non-green without mutation; CRASH = analyzer/provision/observer/cleanup failure; TIMEOUT / NOT EXECUTABLE retain SQL errors; PASS = executable, no contradiction under this oracle. Session leftovers are independently reported. Exit1 signals defects, execution failures, expectation mismatches or incomplete budget; exit0 allows NOISE and expected negative controls. A changed green followed by a SQL error still reports FALSE GREEN.

Shrinking greedily removes top-level statements and WHERE/ORDER BY/TOP clauses using ScriptDom offsets. Every trial restores a fresh four-database set with original setup/principal/seed and must reproduce the same outcome without execution errors. Trials are bounded (--shrink-attempts default24) and obey the run budget. This produces a local minimum among attempted reductions, **not a globally smallest proof**. Repro SQL retains template tokens and needs the original JSON setup/context; report and repro-*.sql save it.

## Isolated server probes

~~~powershell
.\tests\SqlSafetyGuard.ServerHarness\Campaign\Run-Isolated.ps1
~~~

The wrapper creates one sqlguard-cov-<16hex> SQL2025 Developer container, label sqlguard.coverage=disposable, random loopback-only published port, no mounts. SA password exists only in the ignored .campaign-local env file, process environment, and container configuration; it is not an argument, report field or checked-in setting. finally removes the verified owned container and secret file. C# verifies label/mounts/port bindings and SQL MachineName before using sysadmin case execution. Do not use privileged mode against a manually supplied ordinary SQL instance. Container diagnostics may contain secrets; do not archive docker inspect output.

Docker-only SgLoop uses MSOLEDBSQL at explicit IPv4 127.0.0.1,1433, encrypt=optional, trusted container certificate and explicit SA mapping. The explicit IPv4 endpoint avoids the observed Linux localhost provider timeouts. It exists only inside the disposable container and permits OPENQUERY/EXEC AT against that case's own restored databases. It is not installed on .\SQLEXPRESS. The isolated sample includes a backup file, server role/configuration changes, permanent tempdb table, loopback read/writes, and an intentional 15second WAITFOR timeout against the default10second limit. That intentional timeout gives exit1. Run-Isolated also accepts -TimeoutSeconds when a feature needs a longer execution deadline. Developer image availability does not prove SQL Agent/CLR/full-text functionality; unsupported probes need their own executable fixtures. Container destruction, not SQL reversal, resets server/tempdb/files between separate isolated runs; individual cases observe current state and mutate it serially.

## Overnight Task Scheduler

Wave 2 registers the user-approved one-time task **SQL Safety Guard Wave2 2026-10-28** for October 28, 2026 at **22:00 Europe/Bucharest (UTC+02:00)**, with an eight-hour campaign budget and nine-hour scheduler cleanup limit. See `../../../docs/history/wave2/validation-wave2.md` for evidence, limitations and receipt locations.

Historical registration is not an instruction to enable or replace the task. Wave 3 prepares an unlimited manual full launcher using tools/verification/Freeze-Wave3.ps1 and Run-Wave3Full.ps1; it neither registers nor starts an overnight task. The full run must not overlap another campaign or extension verification operation requiring the campaign servers.

Interactive logon requires the user to remain logged on; Windows power conditions still apply. Never overlap this task with another host or Docker harness. A completed corpus does not repeat for eight hours: the 31,004 generated host cases supply work, while identity-matched unfinished runs can resume. Permission-denied and unresolved cases are evidence gaps, not safety passes.

Leave the historical task registered and disabled. Do not enable, replace, unregister or start it as part of wave 3.
