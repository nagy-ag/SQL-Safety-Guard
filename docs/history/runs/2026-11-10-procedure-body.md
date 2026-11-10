> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Procedure-body inspection in the editor pane

Unpublished local development change, 2026-11-10. The published release is unchanged.

## Behavior

`CREATE`, `ALTER` and `CREATE OR ALTER PROCEDURE` retain their definition verdict. A separate
`AnalysisResult.ProcedureBodies` collection describes potential effects when the visible procedure is called.
Those findings never enter the execution verdict, warning counts or F5 confirmation. Each body uses an independent
walker and shares the outer work budget/cancellation. Connection default database/schema are not used as proof for
context-dependent allow-list or trust matches in a deferred body; explicitly qualified names can still match opt-ins.

The pane automatically opens a single procedure's body, groups internal operations by target, preserves per-statement
positions and whole-target warnings, and keeps safe/allowed operations collapsed. Statement buttons navigate and highlight
without selecting SQL. Editor interaction clears the highlight while leaving the body available. Invalid/recovered bodies,
unavailable T-SQL bodies, dynamic parse failures and resource limits cannot appear as complete safe inspection.
Calls to other procedures and runtime SQL keep their existing unknown behavior. No server lookup or SQL dependency was added.

## Verification

- Full locked offline gate: **1,693 .NET tests**, **10 Python tooling tests**, and freshness of **527 inventory rules** pass.
  The statement-verdict reference was regenerated; execution classifications are unchanged.
- Debug and Release builds succeed. The existing DialogChrome threading warnings remain outside this change.
- **26 Release WPF pane interaction checks** pass.
- Owned isolated SSMS 22.11 profile, Light theme, final Debug candidate: **12 procedure-body checks** and
  **17 existing editor-highlight checks plus refresh** pass. Real rendered statement-button navigation, unchanged
  execution header counts, exact caret position, empty/existing SQL-selection preservation and highlight clearing are checked.
- **20 captured surfaces** include the actual editor and procedure pane at its normal 300-DIP width. Captures were visually
  inspected. The sample shows five permanent writes, two unknown operations and five safe operations beneath one red definition.
- The isolated process exits cleanly and its settings are restored byte-for-byte. The existing Exp profile is untouched.
  Evidence is retained outside the repository under `procedure-body-20261110/capture4/`.

Earlier local attempts exposed and corrected recovered-body completeness, automatic WPF list selection, and deferred
allow-list context problems. The latter affected only the new body inspection under development, not existing execution
verdicts. No known false green remains from this work.

## Limits

No SQL was executed and no new server campaign or execution-routing claim is made. Effects behind called procedures,
triggers, runtime strings, and function/view/trigger definitions remain outside this feature. Resource limits can prevent
inspection entirely; a complete walk is not proof that runtime-dependent effects are known. Only one SSMS theme was checked.

`samples/03-procedure-body.sql` is an inspection-only example requiring no setup or connection.
