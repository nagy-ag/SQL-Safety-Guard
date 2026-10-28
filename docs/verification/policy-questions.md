# Policy questions (not decisions)

Current means offline analyzer behavior at baseline edea579, not an observed SQL Server outcome. Keep typed confirmation disabled regardless of these discussions.

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
