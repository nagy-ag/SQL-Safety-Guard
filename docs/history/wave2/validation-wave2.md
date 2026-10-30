> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Wave-2 validation - October 28, 2026

## Scope and reproducibility

Branch: `verify/wave2`, based on `3fff9b0`. Typed confirmation remains disabled. Only owned SgPlay/SgRun databases and principals were provisioned on `.\SQLEXPRESS`. Server-level feature changes ran in disposable Docker Developer containers, removed by the ownership-verifying wrapper. No attack-directory files were created and no subagents were used.

Artifacts are outside the worktree at `C:\Users\nagya\dev\work_exim\ssmsextensions\harness-out\campaign\2026-10-28`. Each report records binary SHA-256, world/corpus identity, exact executed SQL, errors and timings. See `findings-wave2.md`, `gaps-wave2.md`, and campaign `ORACLE.md` for limitations.

## Rule inventory and generators

- 1,501 old entries consolidated to **527 reviewed semantic/table rules**; **17** have no reviewed named-test association. `rule-id-migrations.json` retains retired checkpoint-to-semantic mappings; IDs were preserved wherever possible. Test associations are not branch/live coverage. RuleInventoryTests remains report-only and prints the uncovered count.
- Seed **20261028**. Generated host corpus: **31,004** cases: rule-wrapper 44, world-feature 8, pairwise 132, three-wise 796, seeded random 30,000, company-shaped 24. Offline rule candidates: **831** (277 rules x 3). Validation selection: **316** (all 76 fixed host cases plus 80 from each covering/random family).
- Company shapes: 500, 1,500 and 5,001+ lines, temporary tables, CTEs, derived tables, long single-line joins/predicates, transactions, TRY/CATCH, cursors and literal dynamic SQL. Twelve green and twelve one-buried-permanent-write variants record ground truth and write line.
- Cover arrays verify every pair/triple interaction in their declared dimensions. Dimensions include targets, shapes, wrappers, filters and principals. JSONL `family`, `seed`, `ruleIds` and manifest hashes make selection reproducible. SQLCMD client execution is explicitly unsupported; GO is supported.
- **Not exhaustive:** 250 rules need focused option/AST fixtures; the generic positive/SELECT-1/block offline candidates are not valid proof of every semantic rule's positive/negative/boundary. Allow-list/trusted-procedure configuration injection and a generated Docker loopback family are not implemented. Existing isolated loopback smoke evidence remains separate. Do not claim this meets the requested every-rule live attack coverage.

Regenerate with `python tools/verification/generate_campaign.py`; validate generator invariants with `python tools/verification/test_generate_campaign.py`. CLR feature fixtures are generated from `WaveClr.cs` compiled using the local .NET Framework C# compiler, then passed to `Run-Isolated.ps1 -CoverageFixtures <json>`.

## Outcome triage and feature validation

The ten original WRONG TARGET/NOISE cases are individually explained in `findings-wave2.md`. Wave2-smoke: 50/50, 35 real PASS, 9 INDIRECT EFFECT, 1 UNRESOLVED TARGET, 4 NOISE; one intentional forced-green control is separate. No real FALSE GREEN / WRONG TARGET. The alias harness now honors the analyzer's existing resolved base target; no extra analyzer policy was silently changed.

`wave2-features-final`: **15/15 PASS**, complete, verified owned Docker container removed. Twelve original missing/failed kinds now have executable assertions. All 67 original gaps have individual dispositions in `gaps-wave2.md`; **55 remain open/not reattempted**, including topology/platform/external-runtime prerequisites and feasible but unimplemented subscription/restart observers. This is incomplete relative to the request to attempt every feasible kind.

## Throughput and isolation

| Workload | Workers | Completed / wall time | Cases/hour | Notes |
| --- | ---: | ---: | ---: | --- |
| Historical original 50-case smoke | 2 | 50 / 238.49 s | 754.75 | Median 6,087 ms; includes 13 shrink trials. Historical campaign VALIDATION.md. |
| Wave2 same smoke corpus | 4 | 50 / 144.073 s | 1,249.37 | No shrinking; attribution/capture improvements. Not an isolated single-variable benchmark. |
| Final seeded mixed isolation benchmark | 4 | 400 / 282.096 s | 5,104.64 | Seed 42, approximately 90% read / 10% permanent UPDATE. Different workload. |

Final benchmark outcomes: 386 PASS, 14 EXTERNAL CHANGE; zero crashes, timeouts, not-executable, wrong targets or false greens. 340 retained-set executions passed exact pristine checks. Every read asserts `dbo.Counter.Value=0` before selecting; preceding writes increment that counter and must force restoration. No dirty-state control failed. External changes were unrelated existing database metadata, never reset by this work.

Worker reuse avoids restoring four databases after successful exact no-change cases. Dirty/error/setup cases and every shrink trial restore fresh sets. Catalog roundtrips are batched without replacing exact typed snapshots with checksums. Admin master/results pooling is separated from nonpooled execution connections. Case SQL is never retried. All SQL result records remain per-case durable; report/JSONL flush every 100/end.

The 3,750/hour target (30,000 in eight hours) is demonstrated **only for the read-heavy benchmark**, not the mixed principal/error-heavy generated corpus. Do not extrapolate 40,837/night to the full corpus. Write/error-triggered restore cost remains a bottleneck; the timed overnight run may finish fewer than 30,000 cases. Database snapshots/page-level restore were not implemented.

## Stratified validation

Final corrected run: `wave2-validation-verified`, campaign `D98A1D40BF2E18784532882E66A124A3201EAA6E4CF184F023B64E14A41B85A3`, October 28, 2026 16:10:05-16:19:38 UTC. **316/316**, four workers, 20-second SQL deadline, 30-minute ceiling; **572.586 seconds**, **1,986.78 cases/hour**. The approximately one-hour validation allocation included earlier runs, triage and corrections; this final pass took 9.54 minutes.

| Outcome | Count |
| --- | ---: |
| PASS | 186 |
| INDIRECT EFFECT | 17 |
| UNRESOLVED TARGET | 1 |
| EXTERNAL CHANGE | 9 |
| NOISE | 20 |
| NOT EXECUTABLE | 83 |
| FALSE GREEN / WRONG TARGET / CRASH / TIMEOUT | 0 each |

All 83 error-bearing cases are NOT EXECUTABLE: low-user permission denials and EXECUTE AS USER cross-database restrictions, including audit-trigger access. No generator syntax failure or expectation mismatch was observed. All nine EXTERNAL CHANGE cases are error-free. 129 reused sets passed 129 before-case pristine checks.

Validation exposed two runner problems: unrelated existing database-state drift invalidated pristine reuse, and external drift classification could mask execution errors (seven rows in the preceding run). The runner now narrowly distinguishes unrelated metadata drift and refuses EXTERNAL CHANGE for error-bearing executions. The corrected run above supersedes those earlier outcomes. A final strict-key comparison regression additionally rejects a missing/replaced zero-valued counter key; this last fix was not present in the 316-case run. Its rebuilt Release harness subsequently completed `wave2-head-isolation`: 32/32, 30 PASS, two EXTERNAL CHANGE, all other outcomes zero; 24 reused sets passed 24 pristine checks. The counter-zero read controls remained clean after permanent writes forced restores.

Run binary SHA-256: analyzer `5D595DDA5C988385EC4C6AFAF1EB1F54916B18419112E129E8C84743E36D37FF`; harness `3EEA4B3A17538068BC7E1E4EDF1BF170E4FCF1BFE498998D6F417213F68A2584`. The scheduled frozen-head manifest records the later final build separately.

No observed genuine FALSE GREEN SQL exists to list. The intentional smoke negative control is not an analyzer false green. Zero observed false greens is bounded evidence, not proof of safety; NOT EXECUTABLE, unresolved attribution and external interference remain inconclusive.

## Final checks

- Full unit suite: **1,400 passed, zero failed**, including strict dictionary-key isolation regression (original baseline 1,110).
- Visual Studio MSBuild Release solution and separate Release server harness: **zero warnings / zero errors**; final clean-head build is required immediately before freezing.
- Generator invariants: **4/4 passed**. Inventory regeneration check passes; PowerShell AST parse checks and `git diff --check` pass.
- Owned Docker feature-probe containers removed. No concurrent campaign remains after validation.
- Remaining limitations: 17 unassociated rules, 250 rules without focused generated fixtures, 55 original statement gaps still open, and no demonstrated full-corpus 30,000-case/night throughput.

## One-time overnight task

Task: **SQL Safety Guard Wave2 2026-10-28**, October 28, 2026 **22:00 Europe/Bucharest (UTC+02:00)**, once only. Eight-hour harness budget; nine-hour scheduler limit for draining cleanup; IgnoreNew. Interactive current-user, non-elevated logon requires this user to remain logged on; Windows power conditions still apply. The initial elevated registration was denied, so the script uses the existing user token (which already executes the validated SQL campaigns); it never requests elevation. The failed attempt left an immutable earlier-head frozen folder, not a scheduled task.

`tools/verification/Register-Wave2Overnight.ps1` refuses dirty source, past times, an existing task or an existing frozen-head folder. Run only after final Release builds from the clean branch head. It copies the entire harness/analyzer Release outputs, SQL world resources, overnight script and host inputs into:

`C:\Users\nagya\dev\work_exim\ssmsextensions\harness-out\campaign\bin\<full-head-sha>\`

It saves a SHA-256 manifest, registers the task, reads back NextRunTime/action/state, and exports `wave2-task-registration.json` plus `wave2-task.xml` beside the dated outputs. Those receipts identify the exact head without a circular documentation commit. No later build changes those copies. The frozen script skips all builds and uses its own stable working directory.

Command shape (all paths are absolute in the actual task receipt):

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "<frozen>\Run-Overnight.ps1" -HarnessPath "<frozen>\harness\SqlSafetyGuard.ServerHarness.dll" -AnalyzerPath "<frozen>\analyzer\SqlSafetyGuard.Analyzer.dll" -Cases "<frozen>\cases" -Server ".\SQLEXPRESS" -Workers 4 -BudgetMinutes 480 -TimeoutSeconds 20 -Output "<dated-root>\overnight-<head12>"
```

The task consumes only `generated/host`, not offline candidates, validation duplicates or attacker files. Default bounded shrinking remains enabled for detected false greens/wrong targets. A single campaign instance owns the SQL application lease; do not launch another harness while it runs. Budget exhaustion leaves unfinished cases resumable under the same frozen identity.

Remove before it starts (does not delete artifacts/templates/results):

```powershell
Unregister-ScheduledTask -TaskName 'SQL Safety Guard Wave2 2026-10-28' -Confirm:$false
```
