/*
    SQL Safety Guard: how broken and SQLCMD scripts are reported.

    Analyze the WHOLE file: press Ctrl+Shift+F5 with nothing selected. Don't run it: batch 2 has a syntax error
    on purpose, and batch 5 needs SQLCMD mode.

    What you'll see in the SQL Safety Guard window:
      - Batch 2 has a syntax error. SQL Server compiles a batch as a whole, so NONE of it would run. Its
        statements are still listed (dimmed, "won't run now"), because they will run once the error is fixed.
      - Batch 3 is fine: it runs normally.
      - Batch 4 has a CREATE PROCEDURE in the middle of a batch: the guard tells you to add GO before it.
      - Batch 5 uses SQLCMD lines. The guard checks Query > SQLCMD Mode for this window:
          off (the default): batch 5 is shown as what the server would get, syntax errors, with a hint to turn
                             the mode on.
          on:                :setvar and $(variables) are understood ($(TargetTable) -> dbo.Orders) and :r is yellow.
        Analyze once, switch Query > SQLCMD Mode on, and analyze again to see both.
*/
USE SafetyGuardSandbox;
GO

-- Batch 2: a syntax error in the middle. Nothing in this batch runs.
SELECT COUNT(*) AS Orders FROM dbo.Orders;
UPDATE dbo.Orders SET Qty = Qty + 1 WHERE Id = 1;
SELECT * FROM dbo.Orders WHERE;
DELETE FROM dbo.AuditLog;
GO

-- Batch 3: fine, this runs.
SELECT * FROM dbo.Customers;
GO

-- Batch 4: CREATE PROCEDURE must be the first statement of its batch.
UPDATE dbo.Staging_Import SET X = 2 WHERE Id = 1;
CREATE PROCEDURE dbo.usp_Archive AS
    DELETE FROM dbo.AuditLog WHERE Id < 0;
GO

-- Batch 5: SQLCMD lines. With SQLCMD Mode on, the variable is resolved and ":r" (a file the guard can't see) is
-- yellow; with it off, this batch is a syntax error and the guard says how to fix it.
:setvar TargetTable dbo.Orders
:on error exit
UPDATE $(TargetTable) SET Status = 'checked' WHERE Id = 2;
:r .\more-changes.sql
GO
