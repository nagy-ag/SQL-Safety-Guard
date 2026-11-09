# Contributing

SQL Safety Guard checks T-SQL in SSMS 22 before it runs. What it does for users is in the [README](README.md); this page is
the starting point for changing it. [AGENTS.md](AGENTS.md) holds the same rules for AI coding agents, with more detail.

## The one rule that matters

**A false green is the worst possible bug.** Green means "proven not to change permanent data". If the analyzer cannot
see what a query does, the answer is yellow, never green. Noise is acceptable; a missed write is not. Parse errors,
limits, exceptions and doubt about which window SSMS will execute must ask the user, never run silently.

The extension never executes SQL, opens a connection or queries server metadata. Only the explicit test harness under
`tests/` executes SQL, and only on a disposable server. Keep that boundary when you refactor or add dependencies.

What green, yellow and red promise is defined in [docs/safety/contract.md](docs/safety/contract.md).
Behaviours listed in [policy-questions.md](docs/safety/policy-questions.md) await a company decision: report them,
don't change them.

## Where things are

| Path | What it is |
|---|---|
| `src/SqlSafetyGuard.Analyzer` | The analyzer (netstandard2.0, Microsoft ScriptDom). Everything that decides a colour lives here. |
| `src/SqlSafetyGuard.Vsix` | The SSMS extension (net472): F5 guard, safety pane, dialogs, settings, commands. |
| `tests/SqlSafetyGuard.Tests` | Unit, regression, generated-document and docs-index tests. |
| `tests/SqlSafetyGuard.ServerHarness` | Runs cases on a disposable server and compares the verdict with what really changed. |
| `tests/SqlSafetyGuard.ExtensionChecks` | Checks against a running SSMS: which window executes, which commands are intercepted. |
| `scripts/` | Release packaging, offline verification, UI interaction checks and isolated SSMS capture. |
| `docs/` | Start at [docs/README.md](docs/README.md): `user/`, `it/`, `dev/`, `safety/`, `status.md` and the frozen `history/`. |

[docs/dev/architecture.md](docs/dev/architecture.md) explains responsibilities and traced flows.

## Build and test

```powershell
dotnet test tests\SqlSafetyGuard.Tests                       # analyzer and tests; must pass before any push
powershell.exe -NoProfile -File scripts\Verify-Offline.ps1   # full offline gate, generated-file checks; no SQL
```

The VSIX needs Visual Studio's MSBuild; see [docs/dev/development.md](docs/dev/development.md) for requirements,
isolated debugging in an experimental SSMS profile, and regenerating generated files.

## Before you change something

| Change | Read first |
|---|---|
| How a statement, procedure or setting is judged | [classifier-change skill](.agents/skills/classifier-change/SKILL.md) |
| The extension UI, the pane, dialogs or the F5 guard | [extension UI skill](.agents/skills/ssms-extension-ui/SKILL.md) |
| Anything that runs SQL for verification | [server verification skill](.agents/skills/server-verification/SKILL.md) |
| Anything under `docs/` | [docs upkeep skill](.agents/skills/docs-upkeep/SKILL.md) |
| A release | [docs/dev/release-process.md](docs/dev/release-process.md) |

The skills are written for agents but are short checklists that work for people too.

## Repository rules

- Never commit zip archives or raw run output; evidence stays outside the repository.
- Generated files (`docs/user/statement-verdicts.md`, `docs/verification/rules/`) are regenerated, never hand-edited.
- Current documents are listed in [docs/README.md](docs/README.md); dated reports go to `docs/history/` and are never
  edited afterwards. `DocsIndexTests` enforces both.
- Run the full test suite before pushing.
