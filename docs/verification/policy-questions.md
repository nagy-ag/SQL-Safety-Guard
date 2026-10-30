# Policy questions (not decisions)

Living list; update it when a decision is made. Keep typed confirmation disabled regardless of these discussions.

## Short list for the company (2026-10-30)

None of these is a bug against the current design; each needs a decision. Evidence: the
[2026-10-30 full run](../history/runs/2026-10-30-full-run.md) and the detailed entries below.

1. **Allow-listed objects with side effects.** Allow-listing `sales.Orders`, a view or a synonym also authorizes its trigger writes
   (`audit.Changes`), cross-database writes (`Archive.dbo.Counter`), temporal history and sequence advances. Does authorizing the entry
   point authorize everything it reaches?
2. **Trusted procedures.** A trusted procedure is shown green even when it writes. Should trust mean "authorized", a different label,
   or a claim that it has no effects?
3. **`tempdb` opt-in (`TempdbIsTemp`).** With the setting on, permanent shared `tempdb.dbo.*` objects are green. Keep the opt-in,
   restrict it to session-local objects, or remove it?
4. **`RAISERROR … WITH LOG`.** Writes the server error log but is green. Treat log writes as writes?
5. **Session-state changes that outlive the script**: `EXECUTE AS`/`SETUSER`, `SET IDENTITY_INSERT`, `SET IMPLICIT_TRANSACTIONS`/
   `ANSI_DEFAULTS`, `sp_set_session_context`, session-owned `sp_getapplock`, global `##temp` tables. All green today.
6. **`ROLLBACK` of a caller's transaction.** Green today, but it can discard work the script did not start.
7. **Metadata maintenance**: `UPDATE STATISTICS` (yellow) and `DBCC CHECKDB` (green). Warn or not?
8. **Service Broker `SEND`.** Which object should the red finding name: the dialog, service, queue, or all of them?
9. **Red-only mode.** Unknown procedures produce no prompt in Red mode by design. Acceptable trade-off?

## Detailed entries

"Current" below means offline analyzer behavior at baseline edea579 unless an entry says it was observed on a server.

- `EXECUTE AS USER = 'u';` / `SETUSER 'u';`: ReadOnly; impersonation can survive the script and affect the next command.
- `SET IDENTITY_INSERT dbo.Items ON;`: ReadOnly; persistent session behavior changes even with no row modification.
- `ROLLBACK;`: ReadOnly; can discard a caller-owned transaction not created by this script.
- `WRITETEXT #t.Body @ptr 'x';`: RealWrite fallback; a genuinely local temporary target could justify TempOnly, but target resolution and text pointers need evidence.
- `UPDATE STATISTICS dbo.Items;`: Unknown; metadata changes are intentional, but whether this merits a red capability warning is a policy choice.
- `RAISERROR ('x', 10, 1) WITH LOG;`: ReadOnly; authorized execution can write an error/event log.
- `SET NOCOUNT ON;` / `SET IMPLICIT_TRANSACTIONS ON;`: ReadOnly when alone; session options remain altered even without an open transaction.
- `SET ANSI_DEFAULTS ON; SELECT * FROM dbo.Items;`: ReadOnly; ANSI_DEFAULTS can enable implicit transactions indirectly, while tracking recognizes the explicit IMPLICIT_TRANSACTIONS flag.
- `CREATE TABLE ##a (Qty int);`: TempOnly; global temporary objects are visible to other sessions and can outlive the creator's scope.
- `CREATE TABLE tempdb.dbo.Items (Qty int);` with TempdbIsTemp enabled: TempOnly; this is a permanent shared tempdb object, not session-local storage.
- `EXEC sys.sp_set_session_context @key=N'x', @value=1;`: ReadOnly; retained session context changes later behavior.
- `EXEC sys.sp_getapplock @Resource=N'x', @LockMode=N'Exclusive', @LockOwner=N'Session';`: ReadOnly; session-owned locks can survive the script and block other work.
- Administratively trusted procedures: ReadOnly; administrator authorization is not observational proof that the procedure has no side effects.
- `DBCC CHECKDB;`: ReadOnly without repair; internal snapshots/check bookkeeping complicate a literal zero-server-change definition.
- `UPDATE dbo.Orders SET Qty = Qty WHERE 1 = 0;`: RealWrite; zero observed row changes do not remove write capability, locks or possible statement-level trigger effects.
- Unknown procedure plus yellow suppression in Red mode: Unknown but no prompt; mode selection intentionally trades protection for noise reduction.

### Wave-2 attribution question

- Service Broker `SEND ON CONVERSATION @dialog`: red finding names the dialog variable, not the asynchronously written queue or conversation catalog. Should a target finding represent the dialog, service, queue, or all reachable objects? Until decided, the harness reports UNRESOLVED TARGET rather than WRONG TARGET or PASS. A runtime dialog variable is not treated as a statically missed base table.

### Wave-3 observed policy effects (October 28, 2026)

Evidence is retained outside the repository (run folders `wave3-host-final`, superseding `wave3-host-replay` receipts, and `wave3-isolated-final`). These are observations, not new authorization policy. Typed confirmation stays disabled.

- An allow-listed `UPDATE sales.Orders` also writes `audit.Changes` in Core and Audit through its trigger. `UPDATE app.OrderEntry` (INSTEAD OF view) and `UPDATE app.OrderAlias` (synonym) reach the same base table and audit destinations. Should authorization expand to every reachable effect, or does authorizing the entry point authorize its implementation? No analyzer behavior was changed.
- Allow-listed `DELETE app.RemoteCounter` reaches `Archive.dbo.Counter`; allow-listed `DELETE app.Accounts` also writes temporal `audit.AccountHistory`; inserting into `dbo.IndSequenceDefault` advances `app.OrderSequence`. Matching only the visible entry target does not describe the complete authorization boundary. Pre-execution effect edges preserve these destinations.
- An explicitly trusted writer is classified ReadOnly but mutates the owned world. This is reported as POLICY EFFECT, not observational proof of read-only behavior. Should trust mean authorized execution, a different green label, or an asserted absence of effects?
- With `TempdbIsTemp=true`, `CREATE TABLE tempdb.dbo.IndOptIn_T(Id int);` is TempOnly but creates a permanent shared tempdb object. The isolated oracle records FALSE GREEN against the contract. This is the existing explicit opt-in policy discrepancy, not a newly changed default.
- Authorized `RAISERROR(N'Wave3OwnedLogMarker',10,1) WITH LOG;` writes the SQL Server error log while classified ReadOnly. The dedicated log-observer feature probe detects the marker; an ordinary four-database snapshot does not. This confirms the previously listed log-side-effect policy question. Severity alone does not make log output incidental engine bookkeeping.
- Unknown remote OPENQUERY/OPENROWSET output cannot be canonically attributed from a static provider string. Unmatched observed destinations are UNRESOLVED TARGET unless a concrete conflicting target is actually claimed. This oracle correction is not an authorization exemption.

`EXEC master.dbo.sp_monitor;` is **not** deferred as a policy choice: its intentional persistent `master.dbo.spt_monitor` update contradicted the safe-procedure inventory, so wave 3 moves it to the writing-procedure inventory.
