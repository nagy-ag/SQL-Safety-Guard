# SSMS 22 extension execution verification

## Outcome

**30 PASS, 2 FAIL, 1 BLOCKED across 33 scenarios. The exact-executed-text requirement is NOT satisfied for the tested no-focus multi-editor command-dispatch path.** A green guard decision for B allowed an UPDATE from A without a confirmation. The opposite direction also showed a red confirmation for B followed by execution of A's SELECT. No production fix was made; these remain explicit failing regressions.

This campaign uses real SSMS and SQL Server, not analyzer-only mocks. Physical key presses, real mouse, clipboard and foreground/focus manipulation were not used. The evidence establishes in-process DTE command-dispatch behavior, not physical shortcut or user-tab-click behavior.

## Run identity and evidence

- Date: October 28, 2026; final campaign 17:27–17:30 UTC (Europe/Bucharest UTC+2).
- Baseline: main 29615860a6c20d43f9cf6d96087845b891dc1e79; branch verify/extension.
- Host: SSMS 22.11.2, setup instance e8efe2d3; Windows authentication to .\SQLEXPRESS.
- Isolated profile: /rootSuffix SgExt8a6c7b6ac999; owned SSMS PID 10392, started 2026-10-28T17:27:32.5252344Z.
- Owned database and server XE session: SgExt_8a6c7b6ac999. No other databases were used for test writes.
- Tested Debug VSIX SHA-256: 870734117fc44140e42dc9254a2134fbfe842ed8955175fd838ceae0267a9074.
- Local raw evidence: C:\Users
agya\AppData\Local\Temp\SgExt8a6c7b6ac999 (ownership.json, results-172756.json, events.tsv, commands.tsv, installer/activity logs).
- Retained repository evidence: [scenario ledger](extension-evidence.json), [Debug journal](extension-events.tsv), [installed command inventory](extension-commands.tsv). The ledger abbreviates the large SQL payload while retaining its length and SHA-256; raw local JSON retains it in full. Repository TSV copies omit trailing empty fields; raw local copies retain them.

The driver samples database counters and sql_batch_completed XE before/after every scenario. XE is scoped to the owned database and filtered to the owned client PID and exact SSMS Query application, excluding IntelliSense. A unique SQL comment identifies ordinary batches. Guard tracing records the analyzed document, exact UTF-8 text hash, verdict, selection flag, database and mode. It occurs before confirmation. Counter order is [red, yellow, outside]. dbo.Tick increments yellow; direct UPDATE increments the named row.

In the table, **B** is the number of observed target batch completions (BatchSubmissionCount), including syntax-rejected submissions. It is not a successful-write count. **Δ** is the independent counter delta. Parse/estimated plan submit the target batch under server options but produce zero writes. SET PARSEONLY/SHOWPLAN_XML/STATISTICS XML wrapper batches are excluded from target counts. Every retained ordinary completion has XE result=0, including the SQLCMD-off syntax-error submission; result=0 must not be treated as proof of SQL success. The driver's ObservedCount counts result=0 target completions for ordinary commands and counter writes for Parse/estimated-plan checks.

Server text equality removes CR characters and trims outer whitespace; internal text must match. Guard hashes do not normalize text. SQLCMD-on compares the original guard source hash but expects directive removal and variable expansion on the server. No claim of byte-identical transport whitespace is made. Counts are bounded observations after command completion plus a 1.2-second settling interval; no claim about arbitrarily delayed external work is made.

## Results

PASS means the stated assertions passed, not that all physical entry points are covered. “No guard” for Off/rule Off is intentional. “No guard” for Parse/estimated plan records an unguarded path, with zero tested writes, rather than a successful interception.

| Scenario | Entry point | Expected | Observed (server counts) | Result |
|---|---|---|---|---|
| green | Query.Execute | Green; no dialog; one SELECT | ReadOnly; no dialog; B=1; Δ=[0,0,0] | PASS |
| red-cancel | Query.Execute | Red; Cancel; no submission/write | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| red-run | Query.Execute | Red; Run anyway; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| yellow-cancel | Query.Execute | Yellow; Cancel; no submission/write | Unknown; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| yellow-run | Query.Execute | Yellow; Run anyway; one yellow write | Unknown; dialog Run; B=1; Δ=[0,1,0] | PASS |
| select-green | Query.Execute | Green selection only; one SELECT; unselected red never runs | ReadOnly; no dialog; B=1; Δ=[0,0,0] | PASS |
| select-red | Query.Execute | Red selection only; Run; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| select-red-cancel | Query.Execute | Red selection only; Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| off-red | Query.Execute | Off intentionally skips guard; one red write | No guard; no dialog; B=1; Δ=[1,0,0] | PASS |
| redonly-yellow | Query.Execute | Red only skips yellow dialog; one yellow write | Unknown; no dialog; B=1; Δ=[0,1,0] | PASS |
| redonly-red | Query.Execute | Red only; red dialog Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| rule-off | Query.Execute | Owned-database environment rule Off; one red write | No guard; no dialog; B=1; Δ=[1,0,0] | PASS |
| large-red | Query.Execute | 5,100 padding lines + red; Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| large-red-run | Query.Execute | 5,100 padding lines + red; Run; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| stale-green-to-red | Query.Execute | Cached pane green; execute-time red; Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| toolbar-red | Toolbar Invoke | Actual Execute button; red dialog Cancel; zero | No UIA Execute element; dispatch not attempted; B=0; Δ=[0,0,0] | BLOCKED |
| parse-red | Query.Parse | Determine interception; parse must not write | No guard; no dialog; B=1; Δ=[0,0,0] | PASS |
| estimated-red | Query.DisplayEstimatedExecutionPlan | Determine interception; estimated plan must not write | No guard; no dialog; B=1; Δ=[0,0,0] | PASS |
| actual-red-cancel | Query.Execute + IncludeActualExecutionPlan | Actual plan enabled; red Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| actual-red-run | Query.Execute + IncludeActualExecutionPlan | Actual plan enabled; red Run; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| text-red-cancel | Query.Execute + ResultstoText | Results to Text; red Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| text-red-run | Query.Execute + ResultstoText | Results to Text; red Run; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| livestats-red-cancel | Query.Execute + IncludeLiveQueryStatistics | Live statistics enabled; red Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| livestats-red-run | Query.Execute + IncludeLiveQueryStatistics | Live statistics enabled; red Run; one red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| active-A-green | Query.Execute | A green/B red; activate A; one A SELECT | ReadOnly; no dialog; B=1; Δ=[0,0,0] | PASS |
| active-B-red-cancel | Query.Execute | A green/B red; activate B; red Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| active-B-red-run | Query.Execute | A green/B red; activate B; red Run; one B write | RealWrite; dialog Run; B=1; Δ=[0,0,0]; A SELECT executed, not B | FAIL |
| mismatch-green-B-red-A | Query.Execute | A red/B green; activate B; one B SELECT, no write | ReadOnly; no dialog; B=1; Δ=[0,0,1]; A UPDATE executed, not B | FAIL |
| sqlcmd-on | Query.Execute | SQLCMD on; red Run; one expanded red write | RealWrite; dialog Run; B=1; Δ=[1,0,0] | PASS |
| sqlcmd-off-cancel | Query.Execute | SQLCMD off; yellow Cancel; zero | Unknown; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| sqlcmd-off-run | Query.Execute | SQLCMD off; yellow Run; one invalid submission, zero writes | Unknown; dialog Run; B=1; Δ=[0,0,0]; unexpanded syntax rejected | PASS |
| file-red-cancel | Query.Execute + ResultstoFile | Results to File; red Cancel; zero | RealWrite; dialog Cancel; B=0; Δ=[0,0,0] | PASS |
| file-run-savecancel | Query.Execute + ResultstoFile | Results to File; red Run, then cancel Save Results; zero | RealWrite; dialog Run; B=0; Δ=[0,0,0] | PASS |

The stale-text scenario replaces the editor buffer, records state, and executes in one Debug-channel UI callback. The journal shows the pane's cached green summary while the newly captured text hash is red; the execution guard recomputed red and Cancel suppressed submission. Both large cases contain 5,100 padding comment lines plus the UPDATE and marker.

## Finding F1: analyzed document differs from execution target

**Severity: high functional safety failure; unfixed.** This is a write-capable bypass of confirmation in the tested no-focus automation route. The guard correctly hashes DTE.ActiveDocument B, but the real Query.Execute command can remain routed to editor A. Merely checking a fresh active-document snapshot does not establish that it belongs to the SSMS command target.

### Repeatable reproduction

From the repository root, run the complete owned-profile campaign:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\SqlSafetyGuard.ExtensionChecks\Invoke-Checks.ps1
~~~

Inspect active-B-red-run and mismatch-green-B-red-A in the generated results JSON. The same mismatch was seen during preliminary investigation in a separate experimental profile and again in the final fresh-profile installation. The final retained campaign is the auditable evidence.

The exact failing sequence in Run-Checks.ps1 is:

1. Keep connected SgExtA.sql and SgExtB.sql open in the owned process, with guard mode Yellow+Red and no environment override. The shell is shown with SW_SHOWNOACTIVATE; never change foreground/focus.
2. Put red UPDATE text into B using its addressed replacefile command. Activate A using the channel document command (DTE Document.Activate), replace A with a uniquely marked SELECT N'green', and invoke real Query.Execute. This runs A normally.
3. Address B, call document SgExtB.sql, replace B with uniquely marked red UPDATE text. Invoke Query.Execute, Cancel once, then invoke again and choose Run anyway. The guard captures B's red source both times. Cancel submits nothing; Run submits A's earlier SELECT, not B's UPDATE.
4. Address/activate A and replace its buffer with the following text:

~~~sql
UPDATE dbo.Counter SET Executions=Executions+1 WHERE Tag='outside';
~~~

5. Address/activate B using document SgExtB.sql. Replace B with a unique scenario marker followed by SELECT N'green'; invoke Query.Execute through DTE.ExecuteCommand.
6. Observe guard B=ReadOnly, no confirmation, but A's UPDATE reaches the server exactly once. Counter outside increases by 1, although the selected DTE document's script has no write.

Final exact observations:

- active-B-red-run: B guard hash 2f47862b7286a2907ce6e3a821c678e4860ff010f07409f00fb3051e5aaea851; RealWrite dialog accepted; XE at 2026-10-28T17:29:24.066Z contains A's -- SgExt:172756:active-A-green / SELECT N'green'; B's red counter delta is zero, not one.
- mismatch-green-B-red-A: B guard hash 900b1dad69f8872e37cb7f5885d7dd2d0c4060b379b6e9c2f184e433419f1838; ReadOnly and no dialog; XE at 2026-10-28T17:29:27.878Z contains exactly the UPDATE in step 4; Δ=[0,0,1].

Scope matters: this proves a mismatch for permitted DTE activation plus real SSMS command dispatch without OS focus changes. It does **not** prove the same failure occurs after physical tab clicks, F5 input or normal foreground transitions. The production capture uses DTE.ActiveDocument; binding capture to the host's actual editor/connection command target, or safely rejecting a mismatch, needs host-specific investigation and regression coverage. Changing routing/focus in the driver to conceal this outcome would not validate the guard. A production fix is not small and clearly correct, so this campaign reports rather than patches it.

## Other paths and coverage limits

- **F5, Ctrl+E, Alt+X:** the installed inventory maps all three to Query.Execute, GUID {52692960-56BC-4989-B5D3-94C47A513E8D}, ID 1. All Execute rows invoke this real command; no physical keyboard-message/keybinding dispatch was attempted. These are shared-handler tests, not three independently exercised shortcuts.
- **Execute toolbar:** actual Invoke was attempted, but this process's main shell exposed no UIA descendants and no Execute element. toolbar-red is BLOCKED, not a zero-execution pass. The underlying Query.Execute command was exercised separately.
- **Parse (Ctrl+F5, Query.Parse, ID 2):** no guard event/dialog; one target submission under PARSEONLY; zero counter writes. Unguarded, but no write-capable bypass demonstrated by the tested UPDATE.
- **Estimated plan (Ctrl+L, Query.DisplayEstimatedExecutionPlan, ID 11):** no guard event/dialog; one target submission under SHOWPLAN_XML; zero counter writes. Unguarded, but no write-capable bypass demonstrated by the tested UPDATE.
- **Actual plan, live statistics, Results to Text/Grid:** these are mode toggles followed by Query.Execute, not independent executors. Actual-plan, live-statistics and Text red Cancel/Run pairs passed; Grid was the ordinary default path. No promise is made that every plan-producing statement or mode combination was tested.
- **Results to File (Ctrl+Shift+F):** red guard Cancel passed. Red Run then Cancel in the native Save Results dialog caused zero target submissions and zero writes. Successful filename entry/save could not be covered: the native filename edit exposed no ValuePattern. No keyboard fallback, unsafe default-path overwrite or clipboard was used.
- **Stored-procedure shortcuts:** Query.sp_help, Query.sp_who, Query.sp_lock and CustomSP1–9 appear in the inventory but arbitrary configured shortcut execution was not tested/configured. This is a coverage gap, not a claim that these routes are protected or harmless. Client-statistics and other listed non-executing commands were not exhaustively combined.
- **SQLCMD:** tested :setvar Target dbo.Counter with UPDATE $(Target). On: red warning, Run expands and writes once. Off: yellow warning, Cancel submits zero; Run submits original invalid syntax once and writes zero. Includes no :r files, :connect, shell escapes or external scripts.
- **Modes/environment:** Off, Red only and Yellow+Red were set in process memory only. An exact owned-database first-match rule with environment label SgExtEnvironment and mode Off intentionally bypassed warning and wrote once. No normal-profile options were persisted.
- **Selections:** whole-line green-only/red-only selections and red cancellation passed in a single execution-target editor. Partial-line selections, selected SQLCMD directives, selection changes while the modal dialog is open, cross-database switches and every shortcut/selection cross-product remain untested.
- **Safety pane:** cached-green/live-edit evidence and execution-time refresh were checked. This campaign is not a full visual/layout or accessibility audit of the right-hand pane.

## Isolation and cleanup

Invoke-Checks.ps1 builds Debug, installs only into a fresh SgExt suffix, launches that process with a private test-channel directory, and owns one randomly named SgExt_ database plus XE session. In-process automation and PID-filtered UIA Invoke/validated BM_CLICK drive it. SW_SHOWNOACTIVATE makes command routing available without activating the shell; a fully hidden fresh shell initially reported Query.Execute unavailable. Failed startup probes abort instead of fabricating coverage.

Cleanup is in finally and validates PID, process start time and exact suffix. It closes only the owned process, then drops only manifest-recorded owned resources. Independent post-run master queries confirmed no owned database/XE remained for SgExt_1028 (preliminary), SgExt_b9bd660be18d (aborted hidden-shell attempt), or SgExt_8a6c7b6ac999 (final). Owned SSMS PIDs 1200, 31912 and 10392 were absent. The final manifest has Cleaned=true, DatabaseCreated=false and SessionCreated=false. Experimental profile installations and local evidence remain intentionally; the user's normal profile and other SSMS processes were not targeted.

## Validation and changes

- dotnet test tests\SqlSafetyGuard.Tests --no-restore: **1,400 passed, 0 failed, 0 skipped**, final idle rerun, 8 seconds. An earlier overlapping startup/build run had one existing 5,000-line resource-test timing failure (4.2063681 seconds); it was not modified. The complete clean rerun passed.
- VS 18 MSBuild SqlSafetyGuard.sln /restore /p:Configuration=Release /v:minimal /nologo /clp:Summary: **build succeeded, 0 warnings, 0 errors**. Local logs: %TEMP%\SgExt-unit-tests-final.log and %TEMP%\SgExt-release-build.log.
- PowerShell parser validation passed for Client.ps1, Invoke-Checks.ps1, Run-Checks.ps1 and Stop-Checks.ps1.
- Only production-source additions are #if DEBUG trace instrumentation and extensions to the already DEBUG-only test channel. Release guard behavior is unchanged. No bypass was fixed without a proven target-binding solution.
- Driver usage, prerequisites, recovery protocol and evidence limitations are documented in [the driver README](../../tests/SqlSafetyGuard.ExtensionChecks/README.md).
