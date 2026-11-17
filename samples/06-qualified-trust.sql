-- Text-only examples: replace DemoServer / Sandbox with the exact connection labels before approving rules.
-- Write-target rule: DemoServer.Sandbox.dbo.ImportBuffer
-- Procedure rule: DemoServer.Sandbox.dbo.usp_Import
-- Function rule: FUNCTION DemoServer.Sandbox.dbo.GetTotal

USE [Sandbox]; -- green context operation, with a blue accent
GO
UPDATE dbo.ImportBuffer SET Reviewed = 1 WHERE Id = 42;
-- A short name needs approval in every possible database, including the initial database if USE fails.
UPDATE Sandbox.dbo.ImportBuffer SET Reviewed = 1 WHERE Id = 42;
-- This target explicitly names the database; its approval is independent of USE.
EXEC Sandbox.dbo.usp_Import;
SELECT Sandbox.dbo.GetTotal();
GO
EXEC(N'USE [Sandbox]; SELECT 1;'); -- scoped context; outer context is retained
GO
CREATE OR ALTER PROCEDURE dbo.usp_Import AS
BEGIN
    UPDATE dbo.ImportBuffer SET Reviewed = 1 WHERE Id = 42;
END;
-- Definition stays red; the full container keeps its purple outline and separate potential-effect counts.
