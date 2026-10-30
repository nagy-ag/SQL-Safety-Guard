---
name: classifier-change
description: Use before changing how SQL Safety Guard judges any statement, procedure, DBCC/SET option, target resolution, row filter, allow-list or environment rule in src/SqlSafetyGuard.Analyzer. Enforces the false-green-first safety ladder and the regression checklist.
---

# Changing a verdict safely

The analyzer's only job is to never call a write green. Read the safety principle in `AGENTS.md` first.

## The ladder

Before moving anything **toward green** (red → yellow, yellow → green, or adding a procedure/DBCC/SET entry to a safe list):

1. Show it cannot change permanent data in **any** documented form: every parameter, option and edition-specific behaviour.
2. Back it with evidence from a real disposable server (see the `server-verification` skill), not documentation alone. A
   procedure that "usually" only reads is not ReadOnly.
3. If any form can write, or you cannot tell, it stays yellow or red.

Moving **toward red** (catching a missed write) needs no server evidence; do it, then add evidence if you can.

Never relax a verdict to reduce noise. If the noise is a real usability problem, record it in
`docs/verification/policy-questions.md` for the owner instead.

## Checklist for every change

- [ ] A failing test first, in the closest existing file (`SoundnessRegressionTests`, `RecoveryTests`, `TempTableTests`,
      `HiddenSideEffectTests`, `PhysicalTargetRegressionTests`, ...).
- [ ] The same write in every wrapper: plain, `BEGIN/END`, `TRY/CATCH`, `IF`, `WHILE`, cursor loop, transaction, `EXEC(N'...')`,
      `sp_executesql`, after `GO`. The verdict must not change.
- [ ] Every spelling of the target: bracketed, schema-less (default schema), three-part, cross-database, synonym, view,
      CTE/derived table over the target, alias that looks like a temp name (`[#a]`).
- [ ] Permanent versus `#temp`, `##global`, `@table` and permanent `tempdb.dbo.*`.
- [ ] Side channels: triggers, cascading foreign keys, temporal history, sequences (`NEXT VALUE FOR`), `OUTPUT INTO`,
      composable DML, `SELECT INTO`, `ALTER TABLE ... SWITCH`.
- [ ] The finding names the object that physically changes (table, not index/statistic/schema name).
- [ ] Row-scope notes stay honest: "affects every row" only when nothing limits the rows.
- [ ] Resource limits still fail closed (`AnalysisLimitException` → warning, never green).
- [ ] Regenerate `docs/statement-verdicts.md` (`UPDATE_STATEMENT_VERDICTS=1`, see `AGENTS.md`) and, when rule tables change,
      the inventory (`python tools/verification/generate_inventory.py`); review the diff.
- [ ] Full `dotnet test tests\SqlSafetyGuard.Tests` passes.
- [ ] If a false green was fixed, add it to the "Defects found and fixed" table in `docs/verification/status.md`.

## Product documentation

See `docs/development.md` for build/debug and implementation details, `docs/user-guide.md` for installation, use and
settings examples, and `docs/verification/status.md` for current evidence. The root README is a short front page.
