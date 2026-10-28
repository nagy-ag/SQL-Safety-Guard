DROP SYNONYM IF EXISTS app.OrderAlias;
DROP SYNONYM IF EXISTS app.RemoteCounter;
CREATE SYNONYM app.OrderAlias FOR sales.Orders;
CREATE SYNONYM app.RemoteCounter FOR [{{Archive}}].dbo.Counter;
GO
CREATE OR ALTER TRIGGER sales.OrderAudit ON sales.Orders AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT audit.Changes(OrderId,Note) SELECT Id,'order changed' FROM inserted;
INSERT [{{Audit}}].audit.Changes(OrderId,Note) SELECT Id,'cross database change' FROM inserted;
END;

