/*
    Inspection only: open in SSMS; do not execute. No connection or setup is needed.
    Expand a definition, then Inside, then an object to see its individual operations.
    Each body / decoded EXEC / remote preview keeps its own object groups.
    SELECT/PRINT/RETURN without an attributed target remain direct statement rows.
*/

CREATE OR ALTER PROCEDURE dbo.GroupedScopeDemo
AS
BEGIN
    CREATE TABLE #Stage (Id int);
    INSERT INTO #Stage VALUES (1);
    UPDATE #Stage SET Id = 2;
    DECLARE @Rows TABLE (Id int);
    INSERT INTO @Rows VALUES (1);
    DELETE FROM @Rows;

    UPDATE dbo.Orders SET Reviewed = 1 WHERE Id = 1;
    DELETE FROM dbo.Orders WHERE Id = 2;
    EXEC dbo.MissingRoutine;
    EXEC dbo.MissingRoutine;

    -- Same target name, separate execution scopes; these must not merge above.
    EXEC(N'UPDATE dbo.Orders SET Reviewed = 1 WHERE Id = 3;
           DELETE FROM dbo.Orders WHERE Id = 4; PRINT N''decoded'';');
    EXEC(N'DELETE FROM dbo.Orders WHERE Id = 5;');

    EXEC msdb.dbo.sp_add_jobstep
        @job_name = N'InspectionOnly', @subsystem = N'TSQL',
        @command = N'UPDATE dbo.Orders SET Reviewed = 1 WHERE Id = 6; DELETE dbo.Orders;';
    SELECT * FROM OPENQUERY([ExampleServer],
        'UPDATE dbo.Orders SET Reviewed = 1 WHERE Id = 7; DELETE dbo.Orders;');

    PRINT N'Body preview only';
    RETURN;
END;
GO

-- A separate top-level decoded scope, not part of the definition above.
EXEC(N'UPDATE dbo.Orders SET Reviewed = 1 WHERE Id = 8;
       DELETE dbo.Orders WHERE Id = 9; SELECT 1;');
GO

CREATE OR ALTER FUNCTION dbo.GroupedRows()
RETURNS @Rows TABLE (Id int)
AS
BEGIN
    INSERT INTO @Rows VALUES (1);
    INSERT INTO @Rows VALUES (2);
    RETURN;
END;
GO

CREATE OR ALTER TRIGGER dbo.GroupedAudit ON dbo.Orders AFTER DELETE
AS
BEGIN
    INSERT INTO dbo.AuditRows (Id) SELECT Id FROM deleted;
    DELETE FROM dbo.AuditRows WHERE Id = 0;
END;
GO

CREATE OR ALTER VIEW dbo.GroupedView AS SELECT Id FROM dbo.Orders;
GO
