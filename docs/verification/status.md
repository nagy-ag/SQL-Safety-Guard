# Verification status

Last updated: **2026-10-30**, after the full run on analyzer `34630d62` (main `4388bc2`). This page is rewritten after every run; the
dated details live in [../history/](../history/README.md).

## Bottom line

**No known false green.** Every write the analyzer calls green has either been fixed or is a documented policy choice waiting for a
company decision ([policy-questions.md](policy-questions.md)). Typed confirmation stays disabled.

## Latest run — 2026-10-29/30

[Full report](../history/runs/2026-10-30-full-run.md). SQL Server 2025 Developer on `.\SQLEXPRESS` plus a disposable Docker container.

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
| Unit and integration tests | 1,474 passing | — |

## Open items

1. **Company decisions:** the 9-item short list in [policy-questions.md](policy-questions.md).
2. **Harness fixes** (not analyzer bugs): ignore temporary clone logins reliably in the server-permission snapshot; label
   server-level changes from other workers consistently; retry a clone once on "session is in the kill state".
3. **Not executable here:** 57 host and 99 Docker cases (mostly DBCC commands this server version rejects, remote providers without a
   linked server); 34 statement-kind fixtures need other platforms or editions.
4. **Needs a human look:** 13 observation gaps and 6 unresolved targets (Service Broker dialogs, remote OPENQUERY/OPENROWSET).
5. `generate_inventory.py --check` reports `rules.json` as stale (already the case before the docs move); regenerate and review.
6. Flaky timing test `AnalysisResourceTests.RealisticScriptsRemainFastAndFindTheRealWrite(lines: 5000)` fails under heavy CPU load.
