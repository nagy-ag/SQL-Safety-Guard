# SQL Safety Guard — agent guide

If `AGENTS.local.md` exists next to this file, read it first: it describes this machine (servers, databases, desktop rules) and
takes precedence over generic defaults here. It is never committed.

## What this is

An extension for SQL Server Management Studio 22 that analyzes T-SQL **before** it runs and tells the user whether it changes
real data. A pane beside the editor shows the verdict as you type; the F5 guard analyzes the exact text SSMS is about to execute
and asks for confirmation before anything that is not proven safe. It never executes SQL itself.

It is for people working on databases where a mistaken `UPDATE` without `WHERE`, a write in the wrong window, or a write hidden
inside a procedure or dynamic SQL is expensive.

## The safety principle (non-negotiable)

1. **A false green is the worst possible bug.** Green means "proven not to change permanent data". Saying green for something
   that writes defeats the whole product.
2. **Green needs proof. If we don't know, it is yellow, never green.** Unknown procedures, runtime dynamic SQL, parse errors,
   unresolvable targets and anything the analyzer cannot see are yellow.
3. **Noise is acceptable; a missed write is not.** An unnecessary warning costs a click. Never turn something green just to
   reduce noise without evidence from a real server.
4. **Fail closed.** On a parse failure, a resource limit, an analysis exception or doubt about which window SSMS will execute,
   show the warning dialog rather than letting the query run silently.
5. **The verdict must not depend on how a write is spelled or wrapped**: brackets, schema-less names, three-part names,
   `BEGIN/END`, `TRY/CATCH`, `IF`, loops, cursors, transactions, `EXEC(...)`, `sp_executesql`, after `GO`, CTEs, derived tables,
   `OUTPUT INTO`, composable DML.
6. **Permanent versus temporary is the key distinction.** `#temp` tables, table variables and temp procedures are green
   (TempOnly); permanent objects, including permanent `tempdb` objects unless explicitly opted in, are red.

| Verdict | Colour | Meaning |
|---|---|---|
| ReadOnly | green | Reads, variables, session/control statements, balanced transactions, known read-only or trusted procedures |
| TempOnly | green | Writes only temporary tables/procedures or table variables |
| AllowListed | green | Writes only objects matching the user's allow-list |
| Unknown | yellow | Cannot see what runs (procedures, dynamic SQL, `INSERT ... EXEC`, DBCC/BACKUP, parse errors) |
| RealWrite | red | Any change to a permanent object: DML, DDL, security, server state, remote writes |

The precise promises are in [docs/verification/contract.md](docs/verification/contract.md); the current evidence is in
[docs/verification/status.md](docs/verification/status.md).

## Product decisions (do not reverse without the owner)

- Confirmation uses buttons only. F5 must never require typing anything (no typed database-name confirmation).
- The safety UI lives in an editor pane beside the query, not in a docked tool window.
- Behaviours listed in [docs/verification/policy-questions.md](docs/verification/policy-questions.md) are undecided company
  policy. Report them; do not silently change them.

## Code map

| Path | What it is |
|---|---|
| `src/SqlSafetyGuard.Analyzer` | netstandard2.0 analyzer on Microsoft ScriptDom. `SqlSafetyAnalyzer` entry, `StatementWalker` (per-statement verdicts, row filters), `Classifier` (procedure/DBCC/SET tables), `TargetResolver`, `Scope` (temp/CTE/alias tracking), `SideEffectScanner`, `AllowList`, `EnvironmentRules`, `AnalysisWork` (resource limits) |
| `src/SqlSafetyGuard.Vsix` | The SSMS extension (net472): `Guard` (F5 interception, execution-target binding), `UI` (pane, dialog), `Services`, `Options` |
| `tests/SqlSafetyGuard.Tests` | Unit, regression, generated-doc and docs-index tests |
| `tests/SqlSafetyGuard.ServerHarness` | Executes cases on a disposable server and compares the verdict with what really changed |
| `tests/SqlSafetyGuard.ExtensionChecks` | Checks against a running SSMS (which window executes, which commands are intercepted) |
| `tools/verification` | Generators for the rule inventory and campaign cases |
| `docs/development.md` | Build, debug, solution layout and implementation details |
| `docs/user-guide.md` | Install, use, settings examples and user limitations |
| `docs/` | Start at [docs/README.md](docs/README.md) |

## Build and test

```powershell
dotnet test tests\SqlSafetyGuard.Tests                     # analyzer + tests (must pass before any merge or push)
# The VSIX needs Visual Studio's MSBuild (see docs/development.md):  MSBuild.exe SqlSafetyGuard.sln /restore /p:Configuration=Release
$env:UPDATE_STATEMENT_VERDICTS='1'; dotnet test tests\SqlSafetyGuard.Tests --filter FullyQualifiedName~StatementVerdictsTests; Remove-Item Env:UPDATE_STATEMENT_VERDICTS
python tools/verification/generate_inventory.py            # regenerate docs/verification/rules/
```

## Repository rules

- Never commit zip archives or raw run output; evidence stays outside the repository.
- `docs/README.md` lists every current document. Dated reports go to `docs/history/` with the snapshot header; after a
  verification run rewrite `docs/verification/status.md`. Generated files are regenerated, never hand-edited. `DocsIndexTests`
  enforces the index.
- Do not commit or push unless asked. Run the full test suite before pushing.

## Skills

Task workflows follow the open Agent Skills format and live in `.agents/skills/<name>/SKILL.md`, the shared location that
Codex, GitHub Copilot and other compliant agents scan. Claude Code only scans `.claude/skills/`, so that folder holds an
identical copy. **Edit `.agents/skills/` and copy the change to `.claude/skills/`**; `AgentSkillsSyncTests` fails if they differ.
Agents that do not scan skill folders should open the file before starting that kind of task.

| Skill | Read it before |
|---|---|
| [classifier-change](.agents/skills/classifier-change/SKILL.md) | Changing how any statement, procedure or setting is judged |
| [server-verification](.agents/skills/server-verification/SKILL.md) | Running or interpreting the server harness or a verification campaign |
| [ssms-extension-ui](.agents/skills/ssms-extension-ui/SKILL.md) | Changing or testing the VSIX, the pane, the dialog or the F5 guard |
| [docs-upkeep](.agents/skills/docs-upkeep/SKILL.md) | Adding, moving or updating anything in `docs/` |
