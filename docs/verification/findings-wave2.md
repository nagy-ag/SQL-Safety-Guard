# Verification wave 2

## W1-001: streaming procedure wrapper discrepancy (fixed)

Minimal SQL: `EXEC sys.sp_create_streaming_job @name=N'x', @statement=N'';` versus the same SQL inside `BEGIN ... END`.

ScriptDom 180 performs `HotswapCreateExternalStreamingJobStatements` at batch level, producing `CreateExternalStreamingJobStatement`. The generic statement fallback correctly classifies that DDL node as RealWrite. Nested calls remain `ExecuteStatement` and use the procedure table, which omitted `sp_create_streaming_job`, producing Unknown. This was a missing semantic table entry, not incorrect block traversal.

The writing-procedure table now contains this procedure. Regressions cover BEGIN/END, IF/ELSE, WHILE, both TRY/CATCH arms, EXEC literal and sp_executesql literal. A parser-shape regression documents the upstream rewrite. The old metamorphic exemption is removed. Focused metamorphic validation: 286 passed on 2026-10-28.

Audit: searched specialized EXEC fixtures and statement dispatch, including literal dynamic recursion. The streaming hotswap is the only specialized EXEC-to-DDL conversion found in the committed statement fixtures. All inventoried safe/writing procedure entries are now compared standalone and inside every wrapper. No additional classification-position discrepancy was found by those tests. This is bounded evidence, not a claim about every possible parser rewrite.
