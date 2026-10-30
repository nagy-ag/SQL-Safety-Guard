> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Offline findings - wave 1

No SQL was executed. These results concern classification only; none proves a SQL Server side effect or absence of one. Policy hypotheses are separately listed in policy-questions.md.

## W1-001 - BEGIN/END changes a known writer from red to yellow

**Severity: other (classification inconsistency; potential warning-mode bypass), not a demonstrated false green or server NOISE.** Default yellow-and-red policy still prompts. Red-only policy can suppress the rewritten yellow result, so this is operationally important despite both results being non-green.

Reduced parser-clean repro (default options):

```sql
EXEC sys.sp_create_streaming_job @name=N'x', @statement=N'';
-- Overall: RealWrite

BEGIN
    EXEC sys.sp_create_streaming_job @name=N'x', @statement=N'';
END;
-- Overall: Unknown
```

The original fixture is `EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1';` and exhibits the same change when wrapped. Both parameter names are needed for this reduced repro: removing either made both forms Unknown during reduction. This is a local reduction, not a proof of globally shortest input.

The production analyzer is unchanged. `KnownWaveOneStreamingWrapperDiscrepancy` pins both examples; the seeded runner exempts only this exact rewrite/direction and full fixture (with optional trailing semicolon). Unexpected differences still fail. Executability depends on server feature/version/setup; missing feature must be NOT EXECUTABLE, never pass.

## Rewrite-generator exclusions

Contextual `THROW` cannot safely become `[THROW]`: that changes syntax/meaning rather than an identifier. Rewrites therefore use AST Identifier offsets, not every lexer Identifier token. Temp renaming excludes EXEC scripts to avoid missing references inside opaque dynamic string literals. Alias renaming is conservative; names colliding with object or terminal column identifiers are excluded. Baseline parse errors and parse-invalid rewrites are counted and excluded, never called passing rewrites. GO insertion excludes session/context/control-flow-sensitive scripts. Nested existing EXEC is not wrapped, to avoid deliberately exceeding dynamic-analysis depth limits. These exclusions limit coverage and must not be mistaken for analyzer safety findings.

## Outcomes

No unexpected false-green monotonicity failure was observed in the sampled offline corpus. This does not discharge the strict green contract (especially session-state/global-temp policy questions), nor establish the absence of false greens, wrong targets, crashes or runtime effects. No offline discrepancy is labeled a server pass.

## Measured run (October 28, 2026)

- Existing baseline: 824/824 tests pass; augmented suite: 1,110/1,110 pass (about 15 seconds with coverage).
- Expanded fixed-seed run (`SSG_REWRITE_COUNT=2000`): 1,113 selected distinct candidates; 109 parse-invalid baselines excluded; 5,185 attempted rewrites; 5,152 parser-valid comparisons; 33 parse-invalid rewrites excluded; 181 green-to-permanent-write monotonic checks; two occurrences of W1-001; no unexpected mismatches. Eleven xUnit cases pass in about ten seconds.
- Default fixed-seed sample: 137 selected; 13 parse-invalid baselines excluded; 629 attempted rewrites; 623 parser-valid comparisons; six parse-invalid rewrites excluded; 32 monotonic checks; no sampled known discrepancies or unexpected mismatches. The two explicit W1-001 regression cases run even when the shuffled sample does not select that fixture.
