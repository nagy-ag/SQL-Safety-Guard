> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Verification wave 2

## W1-001: streaming procedure wrapper discrepancy (fixed)

Minimal SQL: `EXEC sys.sp_create_streaming_job @name=N'x', @statement=N'';` versus the same SQL inside `BEGIN ... END`.

ScriptDom 180 performs `HotswapCreateExternalStreamingJobStatements` at batch level, producing `CreateExternalStreamingJobStatement`. The generic statement fallback correctly classifies that DDL node as RealWrite. Nested calls remain `ExecuteStatement` and use the procedure table, which omitted `sp_create_streaming_job`, producing Unknown. This was a missing semantic table entry, not incorrect block traversal.

The writing-procedure table now contains this procedure. Regressions cover BEGIN/END, IF/ELSE, WHILE, both TRY/CATCH arms, EXEC literal and sp_executesql literal. A parser-shape regression documents the upstream rewrite. The old metamorphic exemption is removed. Focused metamorphic validation: 286 passed on 2026-10-28.

Audit: searched specialized EXEC fixtures and statement dispatch, including literal dynamic recursion. The streaming hotswap is the only specialized EXEC-to-DDL conversion found in the committed statement fixtures. All inventoried safe/writing procedure entries are now compared standalone and inside every wrapper. No additional classification-position discrepancy was found by those tests. This is bounded evidence, not a claim about every possible parser rewrite.

## Original host smoke: six WRONG TARGET and four NOISE

| Case | Wave-2 disposition | Reason / minimal operation |
| --- | --- | --- |
| write-alias | INDIRECT EFFECT; harness fix | `UPDATE orders SET Qty=orders.Qty+1 FROM sales.Orders orders WHERE Id=1;` Analyzer already supplied `ResolvedTarget=sales.Orders`; AnalyzerHost incorrectly discarded it. Orders triggers also write local/cross-database audit rows. |
| write-view | INDIRECT EFFECT | `UPDATE sales.OpenOrders SET Qty=Qty+1 WHERE Id=1;` Single-base view resolves to sales.Orders and its audit triggers. |
| write-instead-of | INDIRECT EFFECT | `UPDATE app.OrderEntry SET Qty=Qty+1 WHERE Id=1;` INSTEAD OF trigger writes sales.Orders, then audit triggers. |
| write-synonym | INDIRECT EFFECT | `UPDATE app.OrderAlias SET Qty=Qty+1 WHERE Id=1;` Synonym base is sales.Orders, with audit effects. |
| write-cross-synonym | INDIRECT EFFECT | `UPDATE app.RemoteCounter SET Value=Value+1 WHERE Id=1;` Base is the owned Archive.dbo.Counter. |
| broker-send | UNRESOLVED TARGET; policy question | `SEND ON CONVERSATION @dialog MESSAGE TYPE [DEFAULT](0x01);` Variable is not a statically identified queue/table. Not evidence of a different named base target. |
| read-procedure | Expected NOISE | `EXEC app.ReadOrders;` Opaque procedure remains Unknown; no trusted-procedure assertion is inferred from one execution. |
| session-context | Expected NOISE | `USE {{Sales}}; SELECT 1;` Context trust is conservative; session context is separately recorded. |
| session-options | Expected NOISE | `SET NOCOUNT ON; SET IMPLICIT_TRANSACTIONS ON; SELECT * FROM dbo.Counter;` Session-flow warning; no permanent row mutation. |
| write-zero-rows | Expected capability NOISE | `UPDATE dbo.Counter SET Value=1 WHERE Id=-1;` RealWrite describes capability, not affected row count. |

The complete 50-case wave2-smoke rerun reports 35 PASS, 9 INDIRECT EFFECT, 1 UNRESOLVED TARGET and 4 NOISE, plus one deliberate forced-green negative control. No genuine FALSE GREEN or WRONG TARGET. No additional clear-cut analyzer defect was established: the alias defect was in harness presentation/attribution. The negative control is never an analyzer finding.

Attribution uses pre-execution catalog edges for synonyms, single-base views, enabled trigger named DML targets, cascading foreign keys and temporal history. Every concrete modified object must be covered, not merely one. Edges are conservative and operation-nonspecific; dynamic trigger SQL/aliases, multipart names containing dots and arbitrary remote catalogs are not fully resolved. UNRESOLVED TARGET is an open diagnostic, not a clean target-verification pass.
