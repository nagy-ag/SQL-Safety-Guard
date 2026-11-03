# Verification how-to

Current state: [status.md](status.md). Promises: [contract.md](contract.md). Undecided policy: [policy-questions.md](policy-questions.md). This page describes offline checks; dated results live in [../history](../history/README.md). Typed database-name confirmation remains disabled. The commands below execute no SQL. For maintained launchers and mode safety, use the [tooling entry-point map](../../tools/verification/README.md).

## Inventory and rule references

`rules/rules.json` is the machine register; `rules/rules.md` is its generated readable rendering. The register contains
reviewed semantic entries and explicit dispatch-table members. Retired overlapping `decision.*` checkpoints are recorded
in `rule-id-migrations.json`; they are not extra semantic rules or a correctness denominator.

`semantic-rules.tsv` provides reviewed conditions, stable rule IDs, source anchors and named tests. The generator scans C#
recursively (excluding bin/obj), reads pinned ScriptDom metadata, and uses Roslyn to identify attributed test declarations.
References include qualified method identity and exact declaration location. `RuleInventoryTests` checks both against the
source and compiled test methods. None of this measures asserted branches or SQL Server behavior.

`rule-test-map.json` records associations; missing mappings are report-only and appear in generated `uncovered.md`.
Constructed-AST DBCC tests do not establish executable syntax or feature availability. Legacy DISABLE_DEF_CNST_CHK remains
AST-only in this parser. Source/file movement requires regeneration and review while rule IDs and verdicts remain stable.

```powershell
dotnet restore tests/SqlSafetyGuard.Tests --locked-mode
python -B tools/verification/generate_inventory.py
python tools/verification/generate_inventory.py --check
dotnet test tests\SqlSafetyGuard.Tests --filter FullyQualifiedName~RuleInventoryTests --logger "console;verbosity=detailed"
```

Generation requires Python 3.10+, Windows PowerShell, the SDK selected by global.json, and restored locked packages. It reuses the harness's existing Roslyn dependency in --test-index mode, which performs no database work. Ordinary atomic UTF-8 writes replace generated files; no agent-specific installation is required.

## Deterministic rewrites

`MetamorphicVerificationTests` extracts candidate SQL from test C# literals, statement-verdicts inline examples, Coverage JSON fixtures and explicit seeds. It is deliberately a SQL corpus extractor, not a full C# interpreter: interpolated/escaped/invalid snippets may be excluded by the parser. Fixed seed: 20261028. Default: 17 seed inputs plus up to 120 shuffled corpus inputs; `SSG_REWRITE_COUNT` scales additional inputs (1..20000), not rewrite multiplicity. Inputs are deduplicated and bounded. The full current corpus fits with 2000.

AST/token-aware identifier case and brackets, keyword-filled comments/whitespace, conservative semicolons, local-temp/alias rename, BEGIN/END, literal EXEC and safe GO splitting must preserve Overall. Appending a permanent zero-row UPDATE must turn each applicable green baseline non-green. Parse-invalid baselines/rewrites are excluded and counted, not passes. Known failures are exact allowlisted cases; unrelated changes fail. This proves rewrite relationships only; execution equivalence and runtime side effects require the separate server oracle.

```powershell
$env:SSG_REWRITE_COUNT = '2000'
dotnet test tests\SqlSafetyGuard.Tests --filter FullyQualifiedName~MetamorphicVerificationTests --logger "console;verbosity=detailed"
Remove-Item Env:SSG_REWRITE_COUNT
```

## Coverage collection

```powershell
dotnet test tests\SqlSafetyGuard.Tests --filter 'FullyQualifiedName!~RuleInventoryTests&FullyQualifiedName!~MetamorphicVerificationTests' --collect "XPlat Code Coverage" --results-directory "$env:TEMP\ssg-wave1-baseline-coverage"
dotnet test tests\SqlSafetyGuard.Tests --collect "XPlat Code Coverage" --results-directory "$env:TEMP\ssg-wave1-augmented-coverage"
python -B tools/verification/report_coverage.py BASELINE.xml AUGMENTED.xml --output NEW_EXTERNAL_DIRECTORY --measured-at YYYY-MM-DD --source-revision BUILD_REVISION
dotnet test tests\SqlSafetyGuard.Tests
```

`rules/uncovered.md` lists missing named-test associations in current source. The [original measured coverage report](../history/2026-10-28-offline-coverage/README.md)
is immutable historical evidence. New measured reports require actual input paths, measurement date, build attribution and
a new external output directory. Their input hashes are recorded; the caller-supplied revision is not independently verified
by the report. No raw test artifacts are committed. Full-suite time varies by machine; seeded expansion is opt-in.

## What each layer establishes

| Layer | Establishes | Does not establish |
|---|---|---|
| Analyzer unit/regression tests | Expected verdicts, targets, positions and bounded failures for supplied text/options | SQL execution effects |
| Metamorphic tests | Specified rewrite relationships | General execution equivalence |
| Guard policy / XAML tests | Decision rules, warning/default-button structure | SSMS routing or actual submission count |
| In-process UI checks | Loaded control interaction/layout | Live host routing, server behavior or complete theme acceptance |
| Visual captures | Observed surfaces in the recorded host/theme | Guard execution correctness |
| Server/extension campaigns | Recorded execution observations under their oracle and host limits | Universal safety or unobserved effects |

The production boundary is checked offline by ServerAccessBoundaryTests: production dependencies are allow-listed and
SQL/network client APIs are rejected in source. This is a regression guardrail, not a hostile-code security sandbox.
