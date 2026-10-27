# Real-server harness

Checks the analyzer against what SQL Server actually does. Each T-SQL case is analyzed, then **executed** on a
throwaway sandbox, and a fingerprint of the sandbox shows whether anything permanent changed. A case the analyzer
calls green that changed something is a **false green**, the one failure that matters most for this extension.

## What it runs

- Every string literal in the test project that looks like T-SQL (plus literal `+` literal concatenations), so the
  unit-test cases are re-checked against a real server.
- `samples/*.sql`: each whole file and each `-- #n` example on its own.
- Generated scripts shaped like real ones: 600 to 9,000 lines, 25 temp tables, CTE chains, nested derived tables,
  `UPDATE ... FROM` joins, `MERGE` into temp tables, TRY/CATCH, table variables, and a SELECT with 30 joins and a
  WHERE clause on a single 25,000-character line. `gen:green-*` must be green, write nothing and run with no errors.
  `gen:red-*` bury one real write in the middle and must be non-green and write.
- Semicolon-free scripts with 100, 330 and 700 temp-table processing steps (up to 9,102 lines), both without
  permanent writes and with a buried permanent UPDATE. Early-return cases check the continuing transaction path.

## Sandbox and safety

- Two databases stand in for a production and a dev server: `SqlGuard_Prod` (where cases run) and `SqlGuard_Dev`
  (for cross-database cases). The tables, columns and sequences the cases mention are created and seeded
  automatically, so a statement doesn't just fail on a missing table.
- Cases run as the login `sqlguard_harness` through `EXECUTE AS LOGIN ... WITH NO REVERT`. That login has no server
  permissions (so no SHUTDOWN, KILL, sp_configure, xp_cmdshell, CREATE DATABASE or bulk file access). It is
  `db_owner` only in the two sandbox databases and in tempdb, and it is denied BACKUP. Setup checks that the login
  can't reach any other database on the instance, and the server-level fingerprint (logins, configuration, databases,
  linked servers) is checked after every case. Any change stops the run.
- SQLCMD scripts are expanded the way SQLCMD mode does it: `:setvar` in order, `$(var)` substituted everywhere,
  `:exit(query)` runs the query. Client-side commands (`!!`, `:r`, `:out`, `:connect`, ...) are never run, and those
  cases are reported as not run.
- After a case that changed something, the databases are restored from database snapshots and tempdb is cleaned up.
  The run stops if the restore doesn't reproduce the baseline. Everything is dropped at the end (`--keep` keeps it).

"Wrote" means the fingerprint changed (data checksums, row counts, objects, columns, indexes, sequence values,
identity values, schemas, principals, permissions, extended properties, synonyms, database options), or the script
left uncommitted log records in a sandbox database (BEGIN TRAN without COMMIT).

## Run

Needs a local SQL Server where your Windows login is sysadmin (Developer or Express edition, 2016 SP1 or later).

```powershell
dotnet build src\SqlSafetyGuard.Analyzer -c Release
dotnet run --project tests\SqlSafetyGuard.ServerHarness -- `
    --analyzer src\SqlSafetyGuard.Analyzer\bin\Release\netstandard2.0\SqlSafetyGuard.Analyzer.dll `
    --tests tests\SqlSafetyGuard.Tests --samples samples --server .\SQLEXPRESS --out harness-out
```

The analyzer is loaded by path, so you can point `--analyzer` at any branch's build. Repeat `--tests <dir>` to
include independent review corpora alongside the main unit tests. Other options: `--sql <dir>`
for extra `.sql` files, `--filter <text>`, `--limit <n>`, `--timeout <seconds per batch>` (default 10).
The exit code is 0 when there are no false greens, analyzer crashes or failed expectations. `harness-out\report.md`
lists the problems, the generated-script table, non-green cases that ran cleanly and changed nothing (to review for
false reds), and the slowest analyses. `results.json` has every case.

Allow-list and per-server settings aren't applied (the analyzer runs with default options). Those are covered by the
unit tests.
