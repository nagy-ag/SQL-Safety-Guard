/*
    Open in SSMS to inspect the CURRENT pane. No setup or connection is needed.
    Inspection only: do not execute this file. It contains real writes and definitions.
    Object, assembly, linked-server and job names are illustrative.

    Select one numbered example to isolate its results; include its complete definition.
    Single-click a row to navigate/highlight. Double-click to see current details.
    Previews describe visible text only; no server definitions or dependencies are retrieved.
    Selecting only a call does not make a definition elsewhere part of the analysis.
*/

-- #1 Trigger definition
-- Red definition; expand CREATE/ALTER for Inside, then objects and their INSERT/DELETE rows.
-- The definition changes an object; the trigger body does not run when defined.
CREATE OR ALTER TRIGGER dbo.tr_PaneOrders_Audit
ON dbo.PaneOrders
AFTER UPDATE, DELETE
AS
BEGIN
    INSERT INTO dbo.PaneAudit (OrderId)
        SELECT OrderId FROM deleted;
    DELETE FROM dbo.PaneArchive;
END;
GO

-- #2 Scalar T-SQL function
-- Red definition; expand CREATE/ALTER for Inside and the direct DECLARE/SELECT/RETURN rows.
CREATE OR ALTER FUNCTION dbo.fn_PaneOrderCount()
RETURNS int
AS
BEGIN
    DECLARE @count int;
    SELECT @count = COUNT(*) FROM dbo.PaneOrders;
    RETURN @count;
END;
GO

-- #3 Inline table-valued function
-- Red definition; its returned SELECT appears directly under Inside.
CREATE OR ALTER FUNCTION dbo.fn_PaneOrdersForCustomer(@CustomerId int)
RETURNS TABLE
AS
RETURN
(
    SELECT OrderId, CustomerId
    FROM dbo.PaneOrders
    WHERE CustomerId = @CustomerId
);
GO

-- #4 Multi-statement table-valued function
-- Red definition; Inside groups the local INSERT under @Rows (green), with a direct RETURN.
-- The @Rows insert is temporary; preview counts stay separate from execution counts.
CREATE OR ALTER FUNCTION dbo.fn_PaneOrderRows(@CustomerId int)
RETURNS @Rows TABLE (OrderId int)
AS
BEGIN
    INSERT INTO @Rows (OrderId)
        SELECT OrderId FROM dbo.PaneOrders WHERE CustomerId = @CustomerId;
    RETURN;
END;
GO

-- #5 View definition
-- Red definition; Inside exposes the visible SELECT. Hover for its query-when-read context.
CREATE OR ALTER VIEW dbo.v_PaneOrders
AS
SELECT OrderId, CustomerId
FROM dbo.PaneOrders
WHERE Reviewed = 0;
GO

-- #6 Function calls in expressions and FROM, plus a view read
-- NOW: green SELECT rows plus yellow FUNCTION rows for unavailable implementations.
-- A view read still has no transitive dependency lookup; local definitions do not prove deployed calls.
-- Select this example alone to see that behavior without the red definitions above.
SELECT dbo.fn_PaneOrderCount() AS OrderCount;
SELECT * FROM dbo.fn_PaneOrdersForCustomer(42);
SELECT * FROM dbo.fn_PaneOrderRows(42);
SELECT * FROM dbo.v_PaneOrders;
SELECT OrderId FROM dbo.PaneOrders WHERE dbo.fn_PaneOrderCount() > 0;
GO

-- #7 OPENQUERY: visible remote SQL text
-- NOW: green outer SELECT plus yellow Remote query operation with a separate T-SQL preview.
-- The remote operation stays yellow: the remote dialect/runtime are unverified.
SELECT * FROM OPENQUERY([PaneDemoServer],
    'DELETE FROM dbo.RemoteOrders; SELECT 1 AS Done;');
GO

-- #8 OPENROWSET: query through a provider
-- NOW: green outer SELECT plus yellow Remote query with DELETE/SELECT preview rows.
SELECT * FROM OPENROWSET(
    'MSOLEDBSQL',
    'Server=PaneDemoServer;Trusted_Connection=yes;',
    'DELETE FROM dbo.RemoteOrders; SELECT 1 AS Done;');
GO

-- #9 EXEC AT: remote dynamic SQL
-- NOW: yellow Remote query; its DELETE preview navigates to the SQL string, with Sent at linking EXEC.
EXEC(N'DELETE FROM dbo.RemoteOrders;') AT [PaneDemoServer];
GO

-- #10 SQL Agent job-step command
-- NOW: red Stored command operation (job metadata); expand for the separate command preview.
-- Adding a step stores its command; it does not execute that command immediately.
EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'PaneDemo_InspectionOnly',
    @step_name = N'Cleanup',
    @subsystem = N'TSQL',
    @database_name = N'PaneDemoDatabase',
    @command = N'DELETE FROM dbo.PaneArchive; UPDATE dbo.PaneOrders SET Reviewed = 1;';
GO

-- #11 CLR function: implementation is outside this SQL text
-- NOW: red definition; expand for Body unavailable (yellow), without an invented T-SQL body.
-- The assembly's implementation is unavailable to this text-only analyzer.
CREATE FUNCTION dbo.fn_PaneExternalScore(@Value int)
RETURNS int
AS EXTERNAL NAME [PaneDemoAssembly].[PaneDemo.Functions].[Score];
GO

-- #12 Procedure whose definition is unavailable
-- NOW: yellow procedure call; no invented body or server-definition lookup.
EXEC dbo.usp_PaneBodyNotShown;
GO

-- #13 Existing support: control flow already exposes nested operations
-- NOW: the UPDATE, DELETE and SELECT are analyzed without another block hierarchy.
-- Both branches contribute potential effects; this is not a prediction of which runs.
IF 1 = 1
BEGIN
    UPDATE dbo.PaneOrders SET Reviewed = 1 WHERE OrderId = 42;
END
ELSE
BEGIN
    DELETE FROM dbo.PaneArchive;
END;
BEGIN TRY
    SELECT 1 AS Done;
END TRY
BEGIN CATCH
    PRINT N'Example only';
END CATCH;
GO
