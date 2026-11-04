> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Read-only SSMS 22.11 SQLCMD metadata supplement — November 3, 2026

After cloud cleanup, the owner requested that the same candidate work on both 22.4 and 22.11 if possible, while leaving no
cloud resource consuming money. A read-only assembly inspection supplied additional compatibility evidence without launching
SSMS, installing anything, using desktop input, loading application types for execution or creating more cloud resources.
It does **not** close the runtime gap in the [candidate pilot](2026-11-03-ssms-sqlcmd-compatibility-pilot.md).

The host contains **SSMS 22.11.2 / 22.11.12324.101**. This is explicitly a different servicing build from the cloud's earlier
22.11.3 / 22.11.12328.437; no substitution of its runtime results is claimed. Its editor assembly has assembly version
22.200.0.0, file version 22.11.171.48639 and SHA-256
`b1b21015e160bb6e5b5815323e946a3bc87a6b1fa9121a4e20b9267fa23aadb9`.

PE metadata and getter/setter IL show:

- `SqlScriptEditorControl.IsSqlCmdModeEnabled` returns `IsWithOleSQLScripting`, which reads the current query execution
  options' `WithOleSqlScripting` value.
- Its base `ScriptAndResultsEditorControl.IsSqlCmdMode` reads the `isSqlCmdMode` boolean field.
- `SqlScriptEditorControl.SetSqlCmdMode(bool)` compares the execution-option value with the requested mode. When changing
  it, it sets both `IsWithOleSQLScripting` and the inherited `IsSqlCmdMode` to that requested value.

This supports using the **same candidate adapter** for the two host lines. No version-specific analyzer, routing policy or
separate package is currently justified. The candidate accepts the existing property and the legacy property only when
available readings agree; missing, throwing, malformed or conflicting readings remain unknown and warn. A disagreement
between the two storage paths is not permission to guess or substitute global active-tab state.

The concrete difference demonstrated on the earlier 22.4 runtime is the usable inherited `IsSqlCmdMode` property while the
published reader's `IsSqlCmdModeEnabled` lookup was unavailable. On this 22.11.2 assembly both properties exist, and the
normal mode-changing method updates both. Static metadata does not prove their values in every live editor/transition.

**Still required before claiming both versions supported:** run the frozen candidate Release package on an owned 22.11
profile, verify actual installed build, mode off/on and opposite-tab ownership, and the focused SQL/count/counter/context/
target execution checks. Finish the external return-A ownership/channel helper gap separately; do not alter product routing
or weaken assertions to accommodate it. Original published 22.11.3's eleven passes remain useful historical evidence for
that original package only. Candidate 22.4 still has ten strict passes and one incomplete return-A case.

The external receipt `host2211-readonly-metadata.json` retains the exact assembly hash, signatures, IL bytes and resolved
method/field references; the metadata helper remains outside release packages and the repository. Candidate source and
frozen package hashes are unchanged from the pilot. No new build or full offline gate was needed. The final external receipt
records the four documentation-index checks following this supplement.

Cloud cleanup remains verified: no test VM/disks/snapshots/reserved addresses or owned network resources, project billing
unlinked, owned IAP tunnel closed and monitor disabled. The existing account/budget remain for delayed billing and actual
cost is pending. Automatic approval review blocked SSH-key deletion; keys remain for manual cleanup, with both VMs deleted.
No additional cloud work is scheduled; no commit, push or publication occurred.
