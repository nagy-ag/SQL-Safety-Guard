# Verification tooling

The extension and analyzer have no SQL access. Only explicitly selected development harness modes execute SQL.
Read the project server-verification skill and any local machine guidance before an execution run. This directory
does not authorize a server campaign or a scheduled task.

## Maintained entry points

| Entry point | Inputs and output | SQL access |
|---|---|---|
| `scripts/Verify-Offline.ps1` (repository root) | Locked restore, offline tests, current-reference check, Python tool tests | None; package restore may use the network |
| `generate_inventory.py [--check]` | Analyzer source, semantic TSV, attributed test declarations and restored ScriptDom; current generated references | None |
| `Freeze-Campaign.ps1 -OutputRoot <external directory>` | Clean source checkout; rebuilds Release harness/analyzer with locked restore, then freezes binaries/cases/scripts and provenance | None |
| `Run-Campaign.ps1 -Frozen <bundle> -ValidateOnly` | New-schema hash-checked bundle; validates all supplied rows and fixtures | None |
| `Run-Campaign.ps1 -Frozen <bundle>` | Same bundle; external durable outputs, isolated/feature lanes first and generated host cases last | Yes; explicit authorized disposable servers only |
| `report_coverage.py` | Cobertura inputs, measurement date/revision/labels; new external report directory | None |

Restore offline projects before generating the inventory. The generator reuses the harness's existing Roslyn dependency
through `--test-index`; this mode exits before analyzer loading, mutex acquisition or database work. C# sources are
discovered recursively, excluding bin/obj. Qualified test identity and declaration location are validated separately from
coverage. Adding a file does not establish a new semantic rule: reviewed TSV anchors and stable rule IDs remain required.

```powershell
dotnet restore tests/SqlSafetyGuard.Tests --locked-mode
python -B tools/verification/generate_inventory.py --check
python -B -m unittest discover -s tools/verification -p 'test_*.py'
python -B tools/verification/report_coverage.py BASELINE.xml AUGMENTED.xml --output NEW_EXTERNAL_DIRECTORY --measured-at YYYY-MM-DD --source-revision BUILD_REVISION --baseline-label Baseline --augmented-label Augmented
```

The supplied coverage revision is recorded as caller attribution, not verified source/binary provenance. Report files
include input hashes. Reports never overwrite an existing run or write into repository history. Current `uncovered.md`
reports missing named-test associations only; measured line/branch gaps belong to a dated report.

## Campaign discipline

Freezing requires a clean checkout and builds directly into the external bundle. The manifest records the SDK, build
arguments, source revision and file hashes; a source change during freezing invalidates it. A failed freeze leaves an
incomplete directory without a runnable manifest. Choose a new output location rather than overwriting it.

The launcher preserves case principals. Permission failures and green cases whose CATCH may hide a permission error need
review and a separate explicit full-access replay; they are not automatically promoted to passes. Use a new receipt identity
and the harness's explicit `--campaign-principal admin` only for that reviewed subset. Never rerun privileged/server-level
cases on the host. See the [campaign contract](../../tests/SqlSafetyGuard.ServerHarness/Campaign/README.md) and
[oracle limits](../../tests/SqlSafetyGuard.ServerHarness/Campaign/ORACLE.md).

## Removed entry points and historical recipes

- The former `Freeze-Wave3.ps1` and `Run-Wave3Full.ps1` aliases have been removed. Use `Freeze-Campaign.ps1` and
  `Run-Campaign.ps1` directly with the same parameters. Existing external frozen copies are untouched.
- The retired `Register-Wave2Overnight.ps1` refusal stub has been removed. No scheduler registration is provided;
  launch the maintained runner explicitly. Existing scheduled tasks are untouched.
- `report_wave3.py --artifacts <root> --output <new external directory>` reconciles the historical named Wave3 runs. It is not
  a general current-run reporter and cannot overwrite the retained snapshots.
- `generate_wave3_attempts.py`, `generate_wave3_rules.py`, `prepare_wave3_replays.py` and `WaveClr.cs` support the retained Wave3
  corpus. Some consume historical gap ledgers as explicit inputs. Keep those dependencies until the corpus is superseded;
  do not run them as a generic maintenance step.

Archive writes use ordinary Python atomic file replacement. No Codex installation or apply_patch wrapper is required.

## Which checks to run

- Ordinary changes: `scripts/Verify-Offline.ps1` keeps analyzer regressions, harness integrity, installer safety,
  documentation and production access-boundary checks together. Fixing a bug does not retire its regression test.
- UI/guard or release changes: add the Release pane/confirmation/settings interaction checks and the relevant isolated
  SSMS checks. Static XAML checks protect bindings, keyboard defaults and warning structure; they do not prove rendering.
- Analyzer semantics or campaign/oracle changes: select the relevant disposable-server cases under the project skills.
  Full campaigns and full-theme capture remain targeted acceptance work, not routine per-edit checks.

Historical report recipes are retained for reproducibility, not run by the offline gate. The maintained freezer still
packages Wave3 fixtures and `WaveClr.cs`; their historical names do not make them unused. Retire those inputs only after
replacing their current campaign dependencies and checking that the same safety cases remain covered.
