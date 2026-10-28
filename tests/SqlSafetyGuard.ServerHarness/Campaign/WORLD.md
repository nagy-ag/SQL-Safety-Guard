# Owned company playground

Four reusable template databases live on .\SQLEXPRESS: **SgPlay_Core, SgPlay_Sales, SgPlay_Audit, SgPlay_Archive**. Only databases carrying the harness template marker are reusable. Their COPY_ONLY/CHECKSUM backups are written to the SQL instance's default backup directory. Every case restores all four into fresh SgRun_<random>_<role> databases with fresh physical files and NEW_BROKER, then rebinds all cross-database definitions to that case's set. Cases never execute against the templates.

The schema is repeated in each role so a case can choose its starting database without special fixture setup. Cross-database destinations still distinguish Audit and Archive. Defaults are database=Core, principal=runner, seed=1. JSON tokens {{Core}}, {{Sales}}, {{Audit}}, {{Archive}} and {{seed}} are expanded before both execution and analysis. Output preserves original templated input and exact executed SQL.

## Inventory per database

| Object | Purpose / assumption stressed |
|---|---|
| dbo.Customers (100 companies; identity PK) | Realistic typed text/region, identity consumption |
| sales.Orders (100; customer FK, decimal price, computed persisted Total) | Alias target resolution, predicates, computed metadata, parent constraints |
| sales.OrderLines (100; order FK) | Referential constraints and safe delete targets |
| audit.Changes (identity, initially empty) | Trigger side-effect attribution |
| dbo.Counter (2 rows), app.Counter (2 rows) | Same unqualified name with non-dbo default schema; app values start at 100 |
| app.Accounts (2 rows), audit.AccountHistory (temporal history) | SYSTEM_VERSIONING effects on a second physical table |
| staging.Events (100 rows) | Partitioned data, TRUNCATE/index/schema changes |
| EventPartition / EventScheme (25/50/75; PRIMARY destinations) | Partition catalog and range state; no host filegroup changes |
| sales.OpenOrders | Updatable filtered view |
| app.OrderEntry / app.OrderEntryUpdate | INSTEAD OF UPDATE trigger redirecting a view write |
| sales.OrderAudit | Local audit plus cross-database insert into the case's Audit.audit.Changes |
| app.OrderAlias | Same-database synonym to sales.Orders |
| app.RemoteCounter | Cross-database synonym to the case's Archive.dbo.Counter |
| app.OrderSequence | NO CACHE bigint sequence, first-use last_used_value |
| app.ReadOrders / app.WriteOrder | Same EXEC syntax, read versus write behavior |
| app.OrderTotal / app.OrderRows | Scalar and inline table functions reading permanent tables |
| app.LocalWork | Multi-statement TVF writes a return table variable, not permanent data |
| app_user (default app, db_owner) | Non-dbo name resolution and execution-context change |
| low_user (default sales, SELECT only) | Read-only principal / permission failures |
| app.WorkQueue / SgWork ([DEFAULT] contract) | Broker send/queue/conversation state |

Each database contains **9 user tables** (including history), **2 views**, **2 triggers**, **2 synonyms**, **2 procedures**, **3 functions**, **1 sequence**, **1 user Broker queue/service**, and five application schemas (dbo, sales, app, audit, staging). Seeded table rows total **406 per database** (1,624 across the four templates). System-created constraint objects are not included in these category counts.

## Security and server features

Every case gets a random SQL login with db_owner only in its four restored databases; BACKUP DATABASE/LOG are denied. It is impersonated WITH NO REVERT on a nonpooled connection. Setup verifies no CONTROL SERVER and no access to any nonsystem database outside that exact set. This deliberately fails rather than weakening isolation when public/guest permissions expose another database. Template users have no login. A principal override can further EXECUTE AS USER low_user or app_user.

Shared-instance setup does not alter existing databases, tempdb users, server configuration, or linked servers. Loopback linked-server provisioning belongs only in the owned Docker developer container; direct loopback access on the shared instance could inherit an ambient privileged login and is deliberately avoided. BACKUP, server-state probes and permanent tempdb mutations require privileged=true with --campaign-isolated --workers 1, ownership label/no mounts/loopback/SQL MachineName verification, and final container removal.

The template scripts are world.sql and rebind.sql. Provisioning is idempotent through the harness; do not execute world.sql directly against an existing database. The results database SgRun_Results is separately marked and retained. Restored databases/logins are removed in finally blocks. Hard-kill recovery uses the campaign's owned-set registry; never wildcard-drop all SgRun databases on a shared instance.
