> **Snapshot â€” not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Visible code bodies - 2026-11-11


Local unpublished changes extend the existing compact pane on top of 1.0.5. No commit, push or public release was made.
No SQL was executed and no database connection or metadata lookup was added.

## Result

- Typed previews cover procedures, DML/DDL/logon triggers, scalar functions, both table-valued function forms, views,
  visible remote queries and stored SQL Agent T-SQL commands. Preview effects never enter execution counts or F5 warnings.
- Existing compact row templates, colors, spacing, filters, single-click navigation and double-click details remain.
  Context labels describe When fired, Body when called, Query when read, Remote SQL or Stored command. Source links,
  persistent highlights, joined outlines and warnings only in expanded reasons are retained. Nested bodies have
  distinct row identities and recursive expansion/selection restoration.
- Uninspected qualified scalar/object-method and table-valued function calls now add Unknown findings, including
  predicates, RETURN, temporary writes and dynamic wrappers. Recognized built-ins retain existing behavior; a visible
  definition never proves a separate deployed call. Qualified built-ins and object methods can conservatively warn.
- Remote query sources/write targets keep their original Unknown/RealWrite outer verdict. Named proven Agent commands
  use TSQL by default for add; update requires a proven subsystem. Positional parameters, runtime values, Agent tokens,
  other subsystems and external/CLR implementations stay explicitly opaque. Remote/job previews clear local allow-list,
  trusted-procedure and permanent-tempdb opt-ins. Preview nesting is bounded.
- Broken RETURN/SELECT statements now retain module ownership during recovery. The Fabric external CREATE OR ALTER
  FUNCTION fixture consequently has one module diagnostic rather than three cascading errors; its verdict stays red.
  All 13 examples in the code-body sample are updated and checked offline.

## Verification

| Check | Result |
|---|---|
| Locked offline suite | 1,872 .NET tests and 10 Python tests PASS; generated statement reference and 540-rule inventory fresh |
| Builds | Debug and Release PASS; existing two DialogChrome threading warnings remain |
| Release WPF | 42 pane and 15 confirmation checks PASS |
| Existing isolated Light checks | 21 editor, 38 mixed-verdict, 23 procedure and 15 dynamic checks PASS, plus compact sample, minimum width and persistent highlights |
| New isolated Light checks | 85 typed-body and 12 nested-body checks PASS; literal navigation, recursive expansion and passive-analysis highlight persistence verified |
| Local installation | Native normal-profile installer completed; all 25 installed payloads match the frozen Release package; settings unchanged |

The initial new-body check compared header object counts with operation counts. That Debug-only test assumption was
corrected, and the dedicated new-body acceptance run then completed. Existing behavior receipts came from the earlier
Light run before that test stopped it. Product behavior was unchanged by the test correction. Both runs used an owned
isolated SSMS 22.11 profile on a private desktop, in-process events and no physical input. Settings were restored
byte-for-byte and the owned process closed cleanly. Captures were inspected for layout and source/outline behavior.

Frozen Release VSIX SHA-256: `D3B72844FF6915DA930B0EBCF376A2DB60AF51AC9855F7A82F975F4B32C8F0AF`.
Normal settings before and after: `72D73AAAA41F90564125BE81AAC4EE9F99000E5FA40203375366F1BC70FFBA8C`.
Raw logs, receipts, screenshots and frozen binaries remain outside the repository in the local evidence store.

## Limits

This is offline classification and UI evidence, not a new server campaign. No verdict was moved toward green.
Function warnings close a visibility gap conservatively; this does not establish which functions actually write.
Remote dialect/provider behavior, deployed bodies/dependencies, fired triggers/cascades and CLR implementations remain
unavailable. Stored commands require proven named inputs; opaque cases stay explicit warnings. Physical F5 routing,
every theme, version matrices and exhaustive server semantics are not claimed.

Agent defaults were checked against Microsoft's
[sp_add_jobstep documentation](https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-add-jobstep-transact-sql?view=sql-server-ver17)
and [sp_update_jobstep documentation](https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-update-jobstep-transact-sql?view=sql-server-ver17).
