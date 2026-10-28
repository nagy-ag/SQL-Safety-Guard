SET NOCOUNT ON;
GO
CREATE SCHEMA sales;
GO
CREATE SCHEMA app;
GO
CREATE SCHEMA audit;
GO
CREATE SCHEMA staging;
GO
CREATE TABLE dbo.Customers(Id int IDENTITY PRIMARY KEY, Name nvarchar(100) NOT NULL, Region varchar(12) NOT NULL);
CREATE TABLE sales.Orders(Id int IDENTITY PRIMARY KEY, CustomerId int NOT NULL REFERENCES dbo.Customers(Id), Qty int NOT NULL, Price decimal(12,2) NOT NULL, Total AS Qty*Price PERSISTED, Status varchar(20) NOT NULL DEFAULT 'open');
CREATE TABLE sales.OrderLines(Id int IDENTITY PRIMARY KEY, OrderId int REFERENCES sales.Orders(Id), Product nvarchar(60), Qty int);
CREATE TABLE audit.Changes(Id bigint IDENTITY PRIMARY KEY, OrderId int, Note nvarchar(200));
CREATE TABLE dbo.Counter(Id int PRIMARY KEY, Value int NOT NULL);
CREATE TABLE app.Counter(Id int PRIMARY KEY, Value int NOT NULL);
CREATE TABLE app.Accounts(Id int PRIMARY KEY, Balance decimal(12,2), ValidFrom datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(), ValidTo datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2,'9999-12-31 23:59:59.9999999'), PERIOD FOR SYSTEM_TIME(ValidFrom,ValidTo)) WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE = audit.AccountHistory));
CREATE PARTITION FUNCTION EventPartition(int) AS RANGE RIGHT FOR VALUES(25,50,75);
CREATE PARTITION SCHEME EventScheme AS PARTITION EventPartition ALL TO([PRIMARY]);
CREATE TABLE staging.Events(Id int NOT NULL, Payload nvarchar(80)) ON EventScheme(Id);
CREATE SEQUENCE app.OrderSequence AS bigint START WITH 1000 INCREMENT BY 1 NO CACHE;
WITH numbers AS(SELECT TOP (100) ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) number FROM sys.all_objects)
INSERT dbo.Customers(Name,Region) SELECT CONCAT('Company ',number), CASE WHEN number%2=0 THEN 'East' ELSE 'West' END FROM numbers;
INSERT sales.Orders(CustomerId,Qty,Price) SELECT Id,1+Id%5,10+Id FROM dbo.Customers;
INSERT sales.OrderLines(OrderId,Product,Qty) SELECT Id,'Widget',Qty FROM sales.Orders;
INSERT staging.Events SELECT Id,Name FROM dbo.Customers;
INSERT dbo.Counter VALUES(1,0),(2,0);
INSERT app.Counter VALUES(1,100),(2,100);
INSERT app.Accounts(Id,Balance) VALUES(1,1000),(2,2000);
CREATE USER app_user WITHOUT LOGIN WITH DEFAULT_SCHEMA=app;
ALTER ROLE db_owner ADD MEMBER app_user;
CREATE USER low_user WITHOUT LOGIN WITH DEFAULT_SCHEMA=sales;
GRANT SELECT TO low_user;
CREATE QUEUE app.WorkQueue;
CREATE SERVICE [SgWork] ON QUEUE app.WorkQueue([DEFAULT]);
GO
CREATE VIEW sales.OpenOrders AS SELECT Id,CustomerId,Qty,Price,Status FROM sales.Orders WHERE Status='open';
GO
CREATE VIEW app.OrderEntry AS SELECT Id,CustomerId,Qty,Price,Status FROM sales.Orders;
GO
CREATE TRIGGER app.OrderEntryUpdate ON app.OrderEntry INSTEAD OF UPDATE AS UPDATE orders SET Qty=inserted.Qty FROM sales.Orders orders JOIN inserted ON orders.Id=inserted.Id;
GO
CREATE PROCEDURE app.ReadOrders AS SELECT TOP(10) * FROM sales.Orders ORDER BY Id;
GO
CREATE PROCEDURE app.WriteOrder AS UPDATE sales.Orders SET Qty=Qty+1 WHERE Id=1;
GO
CREATE FUNCTION app.OrderTotal(@id int) RETURNS decimal(12,2) AS BEGIN RETURN(SELECT Total FROM sales.Orders WHERE Id=@id); END;
GO
CREATE FUNCTION app.OrderRows() RETURNS TABLE AS RETURN SELECT Id,Qty FROM sales.Orders;
GO
CREATE FUNCTION app.LocalWork(@value int) RETURNS @result TABLE(Value int) AS BEGIN INSERT @result VALUES(@value+1); RETURN; END;
