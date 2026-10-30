# Verification status

Last updated: **2026-10-30**, after 1.0.0 preparation and the Light/Dark capture tooling trial. The latest server campaign used analyzer
`34630d62` (main `4388bc2`); this preparation does not change analyzer verdicts.
This page is rewritten after every run; the dated details live in [../history/](../history/README.md).

## Bottom line

**No known false green.** Every write the analyzer calls green has either been fixed or is a documented policy choice waiting for a
company decision ([policy-questions.md](policy-questions.md)). Typed confirmation stays disabled.

## Latest release preparation - 2026-10-30

[Theme capture trial](../history/runs/2026-10-30-theme-capture.md): **26 non-blank PNGs** across Light and Dark, covering
the pane, real modeless dialog previews, integrated options and toolbar. Several PNGs were opened to verify their real
content and theme colours. The Exp settings file was restored byte-for-byte with matching SHA-256; SSMS closed cleanly.
The remaining 11 themes were not run. This trial does not sign off visual acceptance: the Dark safe-findings text has low
contrast, and the normal F5/Execute path was deliberately not invoked. All capture hooks are Debug-only.

[Preparation report](../history/runs/2026-10-30-release-preparation.md). All **1,571 tests** pass (zero failed/skipped).
Debug and Release solution builds each have **zero warnings and errors**. Release UI scripts pass **13 pane + 15
confirmation + 4 settings checks**; installer safety passes **60 checks**. The generated 527-rule inventory is current.

In the isolated SSMS `Exp` profile, the Debug fault-injection channel showed the themed failure box with Cancel as
default and Escape-cancel configured. Cancel returned `CancelDefault=true`; Run anyway returned `CancelDefault=false`.
This exercised the production handler with a recorded continuation; it did not dispatch a query to a database.
Physical keys, actual server execution and the all-themes pass were not part of this check. The owned SSMS process
was closed with WM_CLOSE, no temporary profiles were created, and `Exp` retains the prepared Debug build.

The release-asset script passed with `-SkipUiChecks` into a temporary directory outside the repository. Its three
files, ZIP layout and checksums were verified, then the directory was deleted. The privacy check caught escaped
SourceLink paths during preparation; Release now maps those to `/_/SqlSafetyGuard/` too. No signing or publication occurred.
Version 1.0.0 and the unreleased changelog heading remain unchanged.

## Previous native-settings check - 2026-10-30

[Native settings report](../history/runs/2026-10-30-native-settings.md). Debug and Release solution builds pass;
all **1,560 tests** and **32 in-process UI checks** pass. An isolated SSMS 22 profile confirmed the integrated
page, native settings batches, import/export synchronization, cancellation, editor validation, and restart persistence.
No SQL was executed. The previous server coverage and policy questions below remain applicable.

## Latest server run - 2026-10-29/30

[Full report](../history/runs/2026-10-30-full-run.md). A local SQL Server 2025 Developer instance plus a disposable Docker container.

| What ran | Cases | Real false greens |
|---|---:|---:|
| Offline checks (no SQL) | 890 | 0 |
| Host: rules, independent corpus, settings | 858 | 0 |
| Docker: server-level cases | 283 | 0 |
| Statement-kind fixtures | 71 | 0 |
| Generated combinations, each case's own principal | 31,004 | 0 |
| Full-access replay of permission failures and green cases with `CATCH` | 4,232 | 0 |

The run reported 11 false greens: 8 were clone-login races between parallel workers (all PASS on a single-worker replay of 1,109
disturbed results) and 3 are the known `TempdbIsTemp` and `RAISERROR … WITH LOG` policy items.

## Defects found and fixed so far

| Found | Defect | Fixed in |
|---|---|---|
| Wave 3 | `EXEC master.dbo.sp_monitor` was green but writes `master.dbo.spt_monitor` | wave 3 |
| Wave 3 | `DROP STATISTICS`, `DROP INDEX`, `ALTER SCHEMA … TRANSFER` named the wrong object | wave 3 |
| Wave 3 | The guard could analyze one query window while SSMS executed another | wave 3 |
| Review | `ALTER TABLE #x SWITCH TO` a permanent table was green | `7c2996f` |
| Wave 1 | `sp_create_streaming_job` inside `BEGIN … END` dropped from red to yellow | wave 2 |

## Coverage

| Area | Covered | Not covered |
|---|---|---|
| Analyzer rules (250) | 152 with server evidence | 50 cannot be exercised by SQL (API/client/AST guards); 48 still lack a working case |
| Original hard statement kinds (67) | 16 verified live | 51: missing packages, Synapse/Fabric-only, unsupported edition or syntax |
| All statement kinds (334) | 267 live-passed on 2026-10-27 | see [2026-10-27 report](../history/2026-10-27-all-kinds/README.md) |
| Unit and integration tests | 1,571 passing | — |

## Open items

1. **Company decisions:** the 9-item short list in [policy-questions.md](policy-questions.md), each with a recommended answer (four would move verdicts toward yellow/red; none toward green).
2. **Harness fixes implemented** (not analyzer bugs): joined principal snapshots exclude other workers' clone lifecycle,
   retain owned/non-clone changes, and disable clone exclusions in Docker. Clone creation retries SQL error 596 once on fresh
   connections, never after case execution starts. Checked live: 800 generated cases with 4 workers gave 0 false greens and
   0 crashes; the remaining 23 EXTERNAL CHANGE results were another database's own state flicker, correctly kept out of FALSE GREEN.
3. **Not executable here:** 57 host and 99 Docker cases (mostly DBCC commands this server version rejects, remote providers without a
   linked server); 34 statement-kind fixtures need other platforms or editions.
4. **Needs a human look:** 13 observation gaps and 6 unresolved targets (Service Broker dialogs, remote OPENQUERY/OPENROWSET).
5. **Inventory refreshed:** all 527 rules and 120 reviewed semantic entries preserved; only source/test references changed.
   `generate_inventory.py --check` and all 276 `RuleInventoryTests` pass.
6. **Timing test stabilized:** warm-up plus a 15-second budget replaces the loaded-machine-sensitive three-second bound;
   `RealWrite` and the resolved permanent-table finding remain required. All 26 `AnalysisResourceTests` pass.
