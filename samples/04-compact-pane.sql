-- Open in SSMS to inspect the pane. No setup or execution is needed.
-- Illustrative objects only: inspect this script without executing it.
-- dbo.PaneOrders has four separate operation rows, including two UPDATEs.
UPDATE dbo.PaneOrders SET Reviewed = 1 WHERE OrderId = 42;
UPDATE dbo.PaneOrders SET Processed = 1 WHERE OrderId = 42;
INSERT INTO dbo.PaneOrders (OrderId) VALUES (43);
DELETE FROM dbo.PaneOrders;
GO

-- One procedure object has both a red definition and a yellow call.
-- Its visible body has a separate potential-effects breakdown.
CREATE OR ALTER PROCEDURE dbo.usp_PaneDemo
AS
BEGIN
    UPDATE dbo.PaneOrders SET Reviewed = 1 WHERE OrderId = 42;
    EXEC dbo.usp_UnavailableProcedure;
    SELECT 1;
END;
GO
EXEC dbo.usp_PaneDemo;
GO

-- Green operations stay separate from permanent writes.
CREATE TABLE #PaneScratch (Id int);
INSERT INTO #PaneScratch VALUES (1);
SELECT * FROM #PaneScratch;
