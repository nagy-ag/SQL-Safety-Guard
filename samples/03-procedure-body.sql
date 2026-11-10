-- Pane inspection example. Open this file in SSMS with nothing selected.
-- No setup or SQL execution is needed to see the body breakdown.
-- The tables below are illustrative; do not execute this procedure.
-- F5 on this whole script would create/alter the definition, not run its body.
CREATE OR ALTER PROCEDURE dbo.usp_SafetyGuard_BodyExample
    @OrderId int,
    @RuntimeSql nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Pending (OrderId int);
    INSERT INTO #Pending (OrderId) VALUES (@OrderId);

    UPDATE dbo.DemoOrders
    SET Processed = 1
    WHERE OrderId = @OrderId;

    -- Deliberately missing WHERE: the pane should call this out.
    DELETE FROM dbo.DemoOrderArchive;

    INSERT INTO dbo.DemoOrderAudit (OrderId) VALUES (@OrderId);

    -- A second operation on the same object is listed separately.
    UPDATE dbo.DemoOrders SET Reviewed = 1 WHERE OrderId = @OrderId;

    BEGIN TRY
        EXEC(N'UPDATE dbo.DemoInventory SET Checked = 1 WHERE ItemId = 42;');
    END TRY
    BEGIN CATCH
        PRINT 'Illustrative error handler';
    END CATCH;

    -- The definition of this other procedure is unavailable: yellow.
    EXEC dbo.usp_DemoNotify @OrderId;

    -- The runtime string is unavailable: yellow.
    IF @RuntimeSql IS NOT NULL
        EXEC(@RuntimeSql);

    SELECT OrderId FROM #Pending;
END;
GO
