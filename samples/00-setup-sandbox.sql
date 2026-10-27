/*
    SQL Safety Guard playground: creates (or re-creates) a throwaway database called SafetyGuardSandbox.
    Run it again any time to reset the data.

    Run it on a local or dev server (e.g. .\SQLEXPRESS), never on production.

    Pressing F5 on this script is itself a good first test: it drops and creates a database, tables and
    procedures, so the guard shows a red dialog. Click "Run anyway".
*/
USE master;
GO

IF DB_ID('SafetyGuardSandbox') IS NOT NULL
BEGIN
    ALTER DATABASE SafetyGuardSandbox SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SafetyGuardSandbox;
END
GO

CREATE DATABASE SafetyGuardSandbox;
GO

USE SafetyGuardSandbox;
GO

CREATE TABLE dbo.Customers (Id int PRIMARY KEY, Name nvarchar(50) NOT NULL);
CREATE TABLE dbo.Orders (Id int PRIMARY KEY, CustomerId int NOT NULL, Qty int NOT NULL, Status varchar(20) NOT NULL);
CREATE TABLE dbo.AuditLog (Id int IDENTITY PRIMARY KEY, OrderId int, Note nvarchar(100));
CREATE TABLE dbo.Staging_Import (Id int, X int);

INSERT dbo.Customers VALUES (1, N'Anna'), (2, N'Bence'), (3, N'Csilla');
INSERT dbo.Orders VALUES (1, 1, 5, 'open'), (2, 1, 0, 'open'), (3, 2, 7, 'shipped'), (4, 3, 2, 'open');
INSERT dbo.Staging_Import VALUES (1, 0), (2, 0);

-- A sequence: every NEXT VALUE FOR changes its state, even inside a transaction that is rolled back.
CREATE SEQUENCE dbo.OrderNumbers AS int START WITH 1000;
GO

-- A procedure that writes (the guard can't see inside it when you EXEC it).
CREATE PROCEDURE dbo.usp_Cleanup AS
    DELETE FROM dbo.Orders WHERE Qty = 0;
GO

-- A procedure that only reads (the guard still can't know that, so EXEC is yellow).
CREATE PROCEDURE dbo.usp_GetOrders AS
    SELECT Id, Qty FROM dbo.Orders;
GO

SELECT 'Sandbox ready' AS Result, (SELECT COUNT(*) FROM dbo.Orders) AS Orders;
