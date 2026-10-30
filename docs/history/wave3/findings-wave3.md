> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Wave-3 findings

## Confirmed analyzer defects

1. **FALSE GREEN:** `EXEC master.dbo.sp_monitor;` was ReadOnly. In the disposable Developer server, `master.dbo.spt_monitor` changed (before checksum `-1283799775`, after `1399735909`; see `wave3-top-suspects-before/isolated-results.json`). The safe procedure inventory was wrong. `sp_monitor` is now a writing procedure (RealWrite). Eleven unit wrapper regressions and nine server wrapper probes cover direct, qualified, bracketed, dynamic, block and control-flow forms. Host master was not modified for this verification.
2. **WRONG TARGET:** `DROP STATISTICS dbo.IndTarget.IndValueStats` and `DROP INDEX dbo.IndTarget.IndValueIndex` named the leaf statistic/index rather than its physical table. Statement handling now resolves the table portion. Regressions distinguish the physical object from its child metadata name.
3. **WRONG TARGET:** `ALTER SCHEMA app TRANSFER dbo.IndSink;` named the destination schema but omitted the source object. The finding preserves the destination while exposing `dbo.IndSink` as its resolved target. A transfer effect edge accounts for the new schema identity. This does not authorize the write.

## Runner/oracle repairs

- Restricted SQL logins are mapped in all four owned databases; LOGIN impersonation with NO REVERT preserves the security boundary while permitting legitimate cross-database access. The explicit `admin` campaign principal uses the provisioning sysadmin identity for privileged cases. No host TRUSTWORTHY or cross-database ownership chaining change was required.
- Per-case analyzer settings now reach the loaded analyzer, with expanded database/default-schema context and same-connection execution alignment. Off/Red/YellowRed warning policy remains distinct from verdict and never enables typed confirmation.
- Tolerant JSONL validation retains accepted cases and emits every rejected line. Empty SQL is a valid no-op, fixing the two independent loader rejections. Validation precedes server work.
- Durable atomic per-case receipts supplement SQL results, survive Docker destruction, and bind resumption to input/world/binary/options/image identity. Changed input or build requires a new output folder.
- An opaque OPENQUERY/OPENROWSET destination without a contradictory concrete finding is UNRESOLVED TARGET, not a fabricated wrong-target defect. Observable indirect writes and green authorization violations remain visible.
- Permanent tempdb changes and dedicated system-state/log observations supplement the ordinary world snapshot. Missing observers are OBSERVATION GAP, never a safety pass.

## Deliberate non-fixes

Allow-list trigger/view/synonym/temporal/sequence effects, trusted writers, session/global-temp effects, TempdbIsTemp and RAISERROR WITH LOG are recorded in `policy-questions.md`. Existing policy was not silently redefined. SQL syntax/setup failures and unsupported features remain non-executable; no generic negative SELECT is counted as positive rule coverage.

Read `validation-wave3.md`, the generated `validation-wave3.json`, `rule-coverage-wave3.md` and `gaps-wave3.md` together. A case tagged with a rule ID is server-associated evidence, not internal branch instrumentation or a universal safety proof.
