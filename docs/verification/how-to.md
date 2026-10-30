# Verification how-to

Current state: [status.md](status.md). Promises: [contract.md](contract.md). Undecided policy: [policy-questions.md](policy-questions.md). This page only explains the offline tooling and how to regenerate its files; dated results live in [../history](../history/README.md). Typed database-name confirmation remains disabled. This work never executes SQL or changes the server harness.

## Inventory and rule references

`rules/rules.json` is the machine register, `rules/rules.md` its generated readable rendering. Reviewed table members and semantic entries are separate from explicitly **needs-review** `decision.*` source checkpoints. The latter intentionally overlap semantic entries and include traversal/display/guard paths; their broad downstream verdicts are not exact direct outputs. They are a conservative source-review checklist, not a claim that 1,501 distinct semantic rules have been audited. Review uncertainty before using the count as a denominator for correctness.

`rules/semantic-rules.tsv` supplies reviewed English conditions, source anchors and named existing tests. The generator extracts classifier tables, SET flags from pinned ScriptDom package XML and DBCC/SET enum families via assembly reflection, recovery keywords, msdb namespace procedures, SQLCMD directives and line-level decision candidates. It scans all top-level analyzer C# files. Conditional logic split over multiple lines needs manual review. `existingTests` is conservative association, not measured assertion/branch coverage; missing associations remain explicit.

`rules/rule-test-map.json` is the lightweight mapping file consumed by `RuleInventoryTests.ReportUncoveredRuleIds`. Invalid/stale IDs or nonexistent test references fail; missing mappings only REPORT. Table-driven added tests reference each procedure/DBCC/SET ID separately. DBCC enum helper tests are constructed-AST classification checks, not evidence of executable DBCC syntax/feature availability. Legacy DISABLE_DEF_CNST_CHK is AST-only and explicitly not executable in this parser.

```powershell
python tools/verification/generate_inventory.py
python tools/verification/generate_inventory.py --check
dotnet test tests\SqlSafetyGuard.Tests --filter FullyQualifiedName~RuleInventoryTests --logger "console;verbosity=detailed"
```

Generation requires Python 3, Windows PowerShell, apply_patch on PATH, and the restored pinned ScriptDom NuGet package. No new package is required: coverlet.collector 6.0.4 was already present.

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
python tools/verification/report_coverage.py <baseline-coverage.cobertura.xml> <augmented-coverage.cobertura.xml>
dotnet test tests\SqlSafetyGuard.Tests
```

`rules/uncovered.md` separates original 824-test coverage from augmented coverage and enumerates zero-hit lines, incomplete branch locations and unmapped IDs. Paths in the commands are placeholders for collector-returned artifact paths, not shell redirects. No raw test artifacts are committed. Full-suite wall time includes restore/build/coverage startup and varies by machine; seeded expansion is opt-in.
