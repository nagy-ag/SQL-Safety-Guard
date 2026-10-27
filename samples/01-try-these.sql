/*
    SQL Safety Guard: example queries to play with. Run 00-setup-sandbox.sql first.

    How to use this file:
      - Select one example (from its "-- #" header to the next one) and press Ctrl+Shift+F5
        to analyze it without running it. The SQL Safety Guard window shows the verdict.
      - Or select an example and press F5. Green ones just run; yellow and red ones show
        the confirmation dialog first (Cancel is the default; "Run anyway" really runs it).
      - Press F5 with nothing selected and the whole file is checked at once: red.

    Every "expect:" line is checked by an automated test (SamplesTests.cs), so these verdicts are real.
    Writes only touch the SafetyGuardSandbox database; run 00-setup-sandbox.sql again to reset it.
*/
USE SafetyGuardSandbox;
GO

-------------------------------------------------------------------------------------------------
-- GREEN: these run without any prompt
-------------------------------------------------------------------------------------------------

-- #1 A normal read
-- expect: ReadOnly
SELECT o.Id, c.Name, o.Qty, o.Status
FROM dbo.Orders o
JOIN dbo.Customers c ON c.Id = o.CustomerId;

-- #2 Copy into a #temp table, change it, drop it: only temp objects are written
-- expect: TempOnly
SELECT * INTO #o FROM dbo.Orders;
UPDATE #o SET Qty = 0 WHERE Status = 'shipped';
SELECT * FROM #o;
DROP TABLE #o;

-- #3 Table variables are temp too
-- expect: TempOnly
DECLARE @ids TABLE (Id int);
INSERT @ids SELECT Id FROM dbo.Orders WHERE Status = 'open';
SELECT * FROM @ids;

-- #4 Session settings and an empty transaction are read-only
-- expect: ReadOnly
SET NOCOUNT ON;
BEGIN TRAN;
SELECT COUNT(*) AS OpenOrders FROM dbo.Orders WHERE Status = 'open';
COMMIT;

-- #5 Read-only system procedures are recognized
-- expect: ReadOnly
EXEC sp_help 'dbo.Orders';

-- #6 DELETE inside a comment or a string is not a delete
-- expect: ReadOnly
-- DELETE FROM dbo.Orders
SELECT 'DELETE FROM dbo.Orders' AS JustText;

-- #7 Dynamic SQL made only of string literals is parsed and checked
-- expect: ReadOnly
EXEC ('SELECT TOP (1) * FROM dbo.Orders');

-------------------------------------------------------------------------------------------------
-- YELLOW: the guard can't see what will run, so it asks
-------------------------------------------------------------------------------------------------

-- #8 A stored procedure (this one deletes rows, but the guard can't look inside)
-- expect: Unknown
EXEC dbo.usp_Cleanup;

-- #9 Dynamic SQL in a variable
-- expect: Unknown
DECLARE @sql nvarchar(200) = N'DELETE FROM dbo.Orders WHERE Id = 1';
EXEC sp_executesql @sql;

-- #10 INSERT ... EXEC: the target is temp, but the procedure might write
-- expect: Unknown
CREATE TABLE #r (Id int, Qty int);
INSERT INTO #r EXEC dbo.usp_GetOrders;
SELECT * FROM #r;
DROP TABLE #r;

-- #11 Maintenance commands whose effect the guard can't see
-- expect: Unknown
UPDATE STATISTICS dbo.Orders;

GO
-- #12 Text that doesn't parse (see the "PARSE ERROR" row). It sits in its own GO batch, so only this
--     batch fails and the rest of the file still runs.
-- expect: Unknown
SELECT * FROM dbo.Orders WHERE;
GO

-------------------------------------------------------------------------------------------------
-- RED: real changes to permanent data or objects
-------------------------------------------------------------------------------------------------

-- #13 A plain UPDATE
-- expect: RealWrite
UPDATE dbo.Orders SET Status = 'cancelled' WHERE Id = 4;

-- #14 UPDATE through an alias: the target "o" is resolved to dbo.Orders
-- expect: RealWrite
UPDATE o SET o.Qty = o.Qty + 1
FROM dbo.Orders o
JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE c.Name = N'Anna';

-- #15 DELETE through a CTE: resolved to the CTE's base table dbo.Orders
-- expect: RealWrite
WITH open_orders AS (SELECT * FROM dbo.Orders WHERE Status = 'open')
DELETE FROM open_orders WHERE Qty = 0;

-- #16 The INSERT goes to a table variable (green), but OUTPUT ... INTO writes to dbo.AuditLog (red)
-- expect: RealWrite
DECLARE @copied TABLE (Id int);
INSERT @copied (Id)
OUTPUT inserted.Id, N'copied' INTO dbo.AuditLog (OrderId, Note)
SELECT Id FROM dbo.Orders;

-- #17 Literal dynamic SQL that deletes: found "inside dynamic SQL"
-- expect: RealWrite
EXEC ('DELETE FROM dbo.Orders WHERE Id = 3');

-- #18 SELECT INTO a permanent table creates it
-- expect: RealWrite
SELECT * INTO dbo.Orders_Backup FROM dbo.Orders;

-- #19 Writes nested in IF / BEGIN ... END are found
-- expect: RealWrite
IF EXISTS (SELECT 1 FROM dbo.Orders WHERE Qty = 0)
BEGIN
    DELETE FROM dbo.Orders WHERE Qty = 0;
END

-- #20 TRUNCATE of a permanent table
-- expect: RealWrite
TRUNCATE TABLE dbo.AuditLog;

GO
-- #21 Changing a procedure definition is red (its body is not analyzed: it doesn't run now).
--     CREATE PROCEDURE must be the first statement of its batch, hence the GO before and after it.
-- expect: RealWrite
CREATE OR ALTER PROCEDURE dbo.usp_Report AS
    DELETE FROM dbo.Orders;
GO

-- #22 MERGE into a permanent table
-- expect: RealWrite
MERGE dbo.Orders AS t
USING (SELECT 2 AS Id, 9 AS Qty) AS s ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET t.Qty = s.Qty;

-------------------------------------------------------------------------------------------------
-- PLAY WITH THE SETTINGS
-------------------------------------------------------------------------------------------------

-- #23 Red by default. Add  dbo.Staging_*  to Tools > Options > SQL Safety Guard >
--     Safe tables allow-list, then check it again: it turns green (Allow-listed).
-- expect: RealWrite
UPDATE dbo.Staging_Import SET X = 1;

-- #24 Several batches: the DELETE is reported on its real line; double-click the row to jump there.
-- expect: RealWrite
SELECT 1 AS FirstBatch;
GO
SELECT 2 AS SecondBatch;
DELETE FROM dbo.AuditLog WHERE Id < 0;
GO

-------------------------------------------------------------------------------------------------
-- SEVERAL TEMP TABLES WORKING TOGETHER
-------------------------------------------------------------------------------------------------

-- #25 A typical temp-table workflow: create, fill, index, alter, join, update via alias, delete via CTE,
--     merge, global ##temp, table variable, dynamic SQL, truncate, drop, across two batches. All green.
-- expect: TempOnly
IF OBJECT_ID('tempdb..#orders') IS NOT NULL DROP TABLE #orders;
CREATE TABLE #orders (Id int PRIMARY KEY, CustomerId int, Qty int);
INSERT #orders SELECT Id, CustomerId, Qty FROM dbo.Orders WHERE Status = 'open';

SELECT c.Id, c.Name INTO #cust
FROM dbo.Customers c
WHERE EXISTS (SELECT 1 FROM #orders o WHERE o.CustomerId = c.Id);

CREATE INDEX ix_cust ON #cust (Id);
ALTER TABLE #orders ADD Flag bit NULL;
UPDATE o SET Flag = 1 FROM #orders o JOIN #cust c ON c.Id = o.CustomerId;

WITH zero AS (SELECT * FROM #orders WHERE Qty = 0) DELETE FROM zero;

MERGE #cust AS t
USING (SELECT DISTINCT CustomerId FROM #orders) AS s ON t.Id = s.CustomerId
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SELECT * INTO ##shared FROM #orders;
DECLARE @summary TABLE (CustomerId int, Total int);
INSERT @summary SELECT CustomerId, SUM(Qty) FROM #orders GROUP BY CustomerId;
SELECT * FROM @summary;

TRUNCATE TABLE #cust;
DROP TABLE #orders, #cust, ##shared;
GO
CREATE TABLE #batch2 (x int);
INSERT #batch2 VALUES (1);
EXEC ('UPDATE #batch2 SET x = 2');
SELECT * FROM #batch2;
DROP TABLE #batch2;
GO

-- #26 The same kind of temp work, but one statement copies the result back into a permanent table.
--     Only that UPDATE is red; everything else stays green in the results window.
-- expect: RealWrite
SELECT Id, Qty INTO #fix FROM dbo.Orders WHERE Status = 'open';
UPDATE #fix SET Qty = Qty + 10;
UPDATE o SET o.Qty = f.Qty
FROM dbo.Orders o
JOIN #fix f ON f.Id = o.Id;
DROP TABLE #fix;

-------------------------------------------------------------------------------------------------
-- LOOKS LIKE A READ, BUT ISN'T
-------------------------------------------------------------------------------------------------

-- #27 OPENQUERY sends text to a linked server, and that text can change data there.
--     Analyze only (Ctrl+Shift+F5): the sandbox has no linked server, so F5 would just fail.
-- expect: Unknown
SELECT * FROM OPENQUERY(ReportingServer, 'EXEC dbo.usp_Cleanup');

-- #28 NEXT VALUE FOR advances a sequence. It's a write, and ROLLBACK doesn't undo it.
-- expect: RealWrite
SELECT NEXT VALUE FOR dbo.OrderNumbers AS NewOrderNumber;

-- #29 Even when the value only goes into a #temp table, the sequence itself still moves.
-- expect: RealWrite
CREATE TABLE #new (OrderNumber int);
INSERT #new VALUES (NEXT VALUE FOR dbo.OrderNumbers);
SELECT * FROM #new;
DROP TABLE #new;

-------------------------------------------------------------------------------------------------
-- TRANSACTIONS, SYSTEM PROCEDURES, DBCC
-------------------------------------------------------------------------------------------------

-- #30 A COMMIT with no BEGIN TRAN in the text commits a transaction an EARLIER run left open:
--     its uncommitted changes become permanent. Analyze only (on its own it just errors).
-- expect: Unknown
COMMIT;

-- #31 BEGIN TRAN without COMMIT/ROLLBACK: the change is red, and the open transaction (holding its locks) is
--     reported too. If you run it, run ROLLBACK afterwards.
-- expect: RealWrite
BEGIN TRAN;
UPDATE dbo.Orders SET Qty = Qty WHERE Id = 1;

-- #32 The usual safe pattern: no warning about the transaction itself.
-- expect: RealWrite
BEGIN TRY
    BEGIN TRAN;
    UPDATE dbo.Orders SET Qty = Qty WHERE Id = 1;
    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH

-- #33 System procedures known to change things are red, with the reason. Analyze only: it renames a table.
-- expect: RealWrite
EXEC sp_rename 'dbo.Staging_Import', 'Staging_Import_Old';

-- #34 Read-only DBCC commands are green.
-- expect: ReadOnly
DBCC USEROPTIONS;
DBCC CHECKDB WITH NO_INFOMSGS;

-- #35 DBCC commands that change server state are red. Analyze only: it clears the plan cache.
-- expect: RealWrite
DBCC FREEPROCCACHE;

-- #36 Session-only statements are green (the key doesn't exist in the sandbox: analyze only).
-- expect: ReadOnly
OPEN SYMMETRIC KEY SandboxKey DECRYPTION BY PASSWORD = 'not-a-real-password';
CLOSE SYMMETRIC KEY SandboxKey;
