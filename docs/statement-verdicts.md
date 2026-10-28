# Statement verdicts

🟢 Green = read-only or temp-only, runs without asking; 🟡 Yellow = can't tell, asks; 🔴 Red = changes permanent data/schema/server (or is conservatively treated as doing so), asks.
Verdicts use default settings: no allow-list and no environment rules; explicitly configured exceptions are noted.
The verdict columns compare permanent and #temp / @table targets; statements without a table/object target use the first column, with — in the second.
One representative example per statement kind, not every clause combination; snippets are analyzed, not executed, without fixture setup.
See [live-server evidence](verification/2026-10-27-all-kinds/README.md); AST-only kinds have no standalone SQL verdict and appear last.

| Statement | On a permanent table | On a #temp table / @table variable | Other cases | Example | Kind |
|---|---|---|---|---|---|
| BEGIN ATOMIC … END | 🟢 Green | — | Contained statements are analyzed too; their effects can change the colour. | `BEGIN ATOMIC WITH (TRANSACTION ISOLATION LEVEL = SNAPSHOT, LANGUAGE = N'us_english') RETURN @a + 42; END` | `BeginEndAtomicBlockStatement` |
| BEGIN TRY … END TRY / BEGIN CATCH … END CATCH | 🟢 Green | — | Contained statements are analyzed too; their effects can change the colour. | `BEGIN TRY DECLARE @v1 AS INT; END TRY BEGIN CATCH END CATCH` | `TryCatchStatement` |
| BEGIN … END | 🟢 Green | — | Contained statements are analyzed too; their effects can change the colour. | `BEGIN RETURN 1; END` | `BeginEndBlockStatement` |
| BREAK | 🟢 Green | — |  | `BREAK` | `BreakStatement` |
| CLOSE | 🟢 Green | — |  | `CLOSE GLOBAL c1` | `CloseCursorStatement` |
| CLOSE MASTER KEY | 🟢 Green | — |  | `CLOSE MASTER KEY` | `CloseMasterKeyStatement` |
| CLOSE SYMMETRIC KEY | 🟢 Green | — |  | `CLOSE ALL SYMMETRIC KEYS` | `CloseSymmetricKeyStatement` |
| CONTINUE | 🟢 Green | — |  | `CONTINUE` | `ContinueStatement` |
| DBCC CHECKDB | 🟢 Green | — | 🔴 Red for FREEPROCCACHE. 🔴 Red with REPAIR_ALLOW_DATA_LOSS. 🔴 Red for SSG_UNKNOWN (parsed as DLL unload). 🟡 Yellow for MEMORYMAP (default-parser errors included). | `DBCC CHECKDB (N'Coverage');` | `DbccStatement` |
| DEALLOCATE | 🟢 Green | — |  | `DEALLOCATE @cursorVar` | `DeallocateCursorStatement` |
| DECLARE @table TABLE | 🟢 Green | — |  | `DECLARE @items TABLE (Id int);` | `DeclareTableVariableStatement` |
| DECLARE @variable | 🟢 Green | — |  | `DECLARE @value int;` | `DeclareVariableStatement` |
| DECLARE … CURSOR | 🟢 Green | — |  | `DECLARE t1_cursor CURSOR FOR SELECT * FROM t1` | `DeclareCursorStatement` |
| EXECUTE AS | 🟢 Green | — |  | `EXECUTE AS CALLER` | `ExecuteAsStatement` |
| FETCH | 🟢 Green | — |  | `FETCH LAST FROM ac` | `FetchCursorStatement` |
| GET CONVERSATION GROUP | 🟢 Green | — |  | `GET CONVERSATION GROUP @group FROM dbo.CoverageQueue;` | `GetConversationGroupStatement` |
| GOTO | 🟢 Green | — |  | `GOTO start` | `GoToStatement` |
| IF | 🟢 Green | — | Contained statements are analyzed too; their effects can change the colour. | `IF @counter=2 BREAK;` | `IfStatement` |
| LINENO | 🟢 Green | — |  | `LINENO 1;` | `LineNoStatement` |
| OPEN | 🟢 Green | — |  | `OPEN authorcursor` | `OpenCursorStatement` |
| OPEN MASTER KEY | 🟢 Green | — |  | `OPEN MASTER KEY DECRYPTION BY PASSWORD = 'password'` | `OpenMasterKeyStatement` |
| OPEN SYMMETRIC KEY | 🟢 Green | — |  | `OPEN SYMMETRIC KEY [key1] DECRYPTION BY CERTIFICATE c1` | `OpenSymmetricKeyStatement` |
| PRINT | 🟢 Green | — |  | `PRINT 3` | `PrintStatement` |
| RAISERROR | 🟢 Green | — |  | `RAISERROR (50009, 16, 10)` | `RaiseErrorStatement` |
| READTEXT | 🟢 Green | 🟢 Green (#temp) · — (@table) |  | `READTEXT dbo.Items.Body @ptrval 1 @size;` | `ReadTextStatement` |
| RETURN | 🟢 Green | — |  | `RETURN` | `ReturnStatement` |
| REVERT | 🟢 Green | — |  | `REVERT` | `RevertStatement` |
| ROLLBACK TRANSACTION | 🟢 Green | — |  | `ROLLBACK` | `RollbackTransactionStatement` |
| SAVE TRANSACTION | 🟢 Green | — |  | `SAVE TRAN marker;` | `SaveTransactionStatement` |
| SELECT | 🟢 Green | 🟢 Green | 🔴 Red for SELECT INTO a permanent table. 🔴 Red when advancing a sequence. | `SELECT Id FROM dbo.Items;` | `SelectStatement` |
| SET (session option) | 🟢 Green | — |  | `SET NOCOUNT ON` | `PredicateSetStatement` |
| SET (session value) | 🟢 Green | — |  | `SET LOCK_TIMEOUT -1` | `SetCommandStatement` |
| SET @variable | 🟢 Green | — |  | `SET @a ^= 1` | `SetVariableStatement` |
| SET ERRLVL | 🟢 Green | — |  | `SET ERRLVL 1000` | `SetErrorLevelStatement` |
| SET IDENTITY_INSERT | 🟢 Green | 🟢 Green (#temp) · — (@table) |  | `SET IDENTITY_INSERT dbo.Items OFF;` | `SetIdentityInsertStatement` |
| SET OFFSETS | 🟢 Green | — |  | `SET OFFSETS PARAM ON` | `SetOffsetsStatement` |
| SET ROWCOUNT | 🟢 Green | — |  | `SET ROWCOUNT 0;` | `SetRowCountStatement` |
| SET STATISTICS | 🟢 Green | — |  | `SET STATISTICS IO ON` | `SetStatisticsStatement` |
| SET TEXTSIZE | 🟢 Green | — |  | `SET TEXTSIZE 0` | `SetTextSizeStatement` |
| SET TRANSACTION ISOLATION LEVEL | 🟢 Green | — |  | `SET TRANSACTION ISOLATION LEVEL SNAPSHOT` | `SetTransactionIsolationLevelStatement` |
| SETUSER | 🟢 Green | — |  | `SETUSER` | `SetUserStatement` |
| THROW | 🟢 Green | — |  | `THROW` | `ThrowStatement` |
| WAITFOR | 🟢 Green | — | Contained statements are analyzed too. 🔴 Red when dequeuing from a queue. 🟢 Green when getting a conversation group. | `WAITFOR DELAY '00:00:01';` | `WaitForStatement` |
| label: | 🟢 Green | — |  | `gogo:` | `LabelStatement` |
| ALTER TABLE … WITH (CLUSTER BY) | 🟡 Yellow | — | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `ALTER TABLE Customers WITH (CLUSTER BY (CustomerID));` | `AlterTableAddClusterByStatement` |
| BACKUP DATABASE | 🟡 Yellow | — |  | `BACKUP DATABASE d1 TO someDevice` | `BackupDatabaseStatement` |
| BACKUP LOG | 🟡 Yellow | — |  | `BACKUP LOG d1 TO someDevice` | `BackupTransactionLogStatement` |
| BEGIN TRANSACTION | 🟡 Yellow | — | Unmatched transaction boundary in isolation. 🟢 Green for a balanced transaction. | `BEGIN TRAN;` | `BeginTransactionStatement` |
| CHECKPOINT | 🟡 Yellow | — |  | `CHECKPOINT` | `CheckpointStatement` |
| COMMIT TRANSACTION | 🟡 Yellow | — | Unmatched transaction boundary in isolation. 🟢 Green for a balanced transaction. | `COMMIT;` | `CommitTransactionStatement` |
| DISK | 🟡 Yellow | — | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `DISK INIT NAME='DEVICE1',PHYSNAME='c:\sql80\data\device1.dat',VDEVNO=1,SIZE=6144;` | `DiskStatement` |
| EXEC | 🟡 Yellow | — | 🟢 Green for a built-in read-only system procedure. 🟢 Green when this procedure is trusted in settings. 🟢 Green for literal dynamic SQL that reads. 🔴 Red for literal dynamic SQL that writes. 🔴 Red for a known writing system procedure. | `EXEC dbo.CoverageProc;` | `ExecuteStatement` |
| RAISERROR (legacy) | 🟡 Yellow | — | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `RAISERROR 50001 'legacy message';` | `RaiseErrorLegacyStatement` |
| UPDATE STATISTICS | 🟡 Yellow | 🟡 Yellow (#temp) · — (@table) |  | `UPDATE STATISTICS dbo.Items;` | `UpdateStatisticsStatement` |
| USE | 🟡 Yellow | — |  | `USE master` | `UseStatement` |
| USE FEDERATION | 🟡 Yellow | — |  | `USE FEDERATION ROOT WITH RESET` | `UseFederationStatement` |
| ADD SENSITIVITY CLASSIFICATION | 🔴 Red | — |  | `ADD SENSITIVITY CLASSIFICATION TO T1.C1, T1.C2, DBO.T1.C3 WITH (LABEL = 'Highly Confidential', INFORMATION_TYPE = 'Financial')` | `AddSensitivityClassificationStatement` |
| ADD SIGNATURE | 🔴 Red | — |  | `ADD SIGNATURE TO HumanResources.uspUpdateEmployeeLogin BY CERTIFICATE HumanResourcesDP` | `AddSignatureStatement` |
| ALTER APPLICATION ROLE | 🔴 Red | — |  | `ALTER APPLICATION ROLE CoverageApp WITH NAME = CoverageAppRenamed;` | `AlterApplicationRoleStatement` |
| ALTER ASSEMBLY | 🔴 Red | — |  | `ALTER ASSEMBLY a1 FROM 'zzz'` | `AlterAssemblyStatement` |
| ALTER ASYMMETRIC KEY | 🔴 Red | — |  | `ALTER ASYMMETRIC KEY a1 ATTESTED BY 'zzz'` | `AlterAsymmetricKeyStatement` |
| ALTER AUTHORIZATION | 🔴 Red | — |  | `ALTER AUTHORIZATION ON USER::Parts.Sprockets TO [c1]` | `AlterAuthorizationStatement` |
| ALTER AVAILABILITY GROUP | 🔴 Red | — |  | `ALTER AVAILABILITY GROUP group1 JOIN` | `AlterAvailabilityGroupStatement` |
| ALTER BROKER PRIORITY | 🔴 Red | — |  | `ALTER BROKER PRIORITY CoveragePriority FOR CONVERSATION SET (PRIORITY_LEVEL = 7);` | `AlterBrokerPriorityStatement` |
| ALTER CERTIFICATE | 🔴 Red | — |  | `ALTER CERTIFICATE c1 ATTESTED BY 'zzz'` | `AlterCertificateStatement` |
| ALTER COLUMN ENCRYPTION KEY | 🔴 Red | — |  | `ALTER COLUMN ENCRYPTION KEY OneValueCEK DROP VALUE ( COLUMN_MASTER_KEY = CMK2 )` | `AlterColumnEncryptionKeyStatement` |
| ALTER CREDENTIAL | 🔴 Red | — |  | `ALTER CREDENTIAL c1 WITH IDENTITY = 'id1'` | `AlterCredentialStatement` |
| ALTER CRYPTOGRAPHIC PROVIDER | 🔴 Red | — |  | `ALTER CRYPTOGRAPHIC PROVIDER cp1 ENABLE` | `AlterCryptographicProviderStatement` |
| ALTER DATABASE ADD FILE | 🔴 Red | — |  | `ALTER DATABASE [test] ADD FILE (NAME = 'MyFile') TO FILEGROUP [MY FILEGROUP]` | `AlterDatabaseAddFileStatement` |
| ALTER DATABASE ADD FILEGROUP | 🔴 Red | — |  | `ALTER DATABASE d1 ADD FILEGROUP fg1` | `AlterDatabaseAddFileGroupStatement` |
| ALTER DATABASE AUDIT SPECIFICATION | 🔴 Red | — |  | `ALTER DATABASE AUDIT SPECIFICATION AuditSpec1` | `AlterDatabaseAuditSpecificationStatement` |
| ALTER DATABASE COLLATE | 🔴 Red | — |  | `ALTER DATABASE db1 COLLATE Estonian_CS_AS` | `AlterDatabaseCollateStatement` |
| ALTER DATABASE ENCRYPTION KEY | 🔴 Red | — |  | `ALTER DATABASE ENCRYPTION KEY REGENERATE WITH ALGORITHM=AES_256;` | `AlterDatabaseEncryptionKeyStatement` |
| ALTER DATABASE MODIFY FILE | 🔴 Red | — |  | `ALTER DATABASE db1 MODIFY FILE (NAME = n1, NEWNAME = n2)` | `AlterDatabaseModifyFileStatement` |
| ALTER DATABASE MODIFY FILEGROUP | 🔴 Red | — |  | `ALTER DATABASE d1 MODIFY FILEGROUP fg1 DEFAULT` | `AlterDatabaseModifyFileGroupStatement` |
| ALTER DATABASE MODIFY NAME | 🔴 Red | — |  | `ALTER DATABASE db1 MODIFY NAME = db2` | `AlterDatabaseModifyNameStatement` |
| ALTER DATABASE REBUILD LOG | 🔴 Red | — |  | `ALTER DATABASE db1 REBUILD LOG` | `AlterDatabaseRebuildLogStatement` |
| ALTER DATABASE REMOVE FILE | 🔴 Red | — |  | `ALTER DATABASE db1 REMOVE FILE myfile` | `AlterDatabaseRemoveFileStatement` |
| ALTER DATABASE REMOVE FILEGROUP | 🔴 Red | — |  | `ALTER DATABASE d1 REMOVE FILEGROUP fg1` | `AlterDatabaseRemoveFileGroupStatement` |
| ALTER DATABASE SCOPED CONFIGURATION CLEAR | 🔴 Red | — |  | `ALTER DATABASE SCOPED CONFIGURATION CLEAR PROCEDURE_CACHE` | `AlterDatabaseScopedConfigurationClearStatement` |
| ALTER DATABASE SCOPED CONFIGURATION SET | 🔴 Red | — |  | `ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 1` | `AlterDatabaseScopedConfigurationSetStatement` |
| ALTER DATABASE SET | 🔴 Red | — |  | `ALTER DATABASE d1 SET HADR OFF` | `AlterDatabaseSetStatement` |
| ALTER DATABASE … PERFORM_CUTOVER | 🔴 Red | — |  | `ALTER DATABASE db PERFORM_CUTOVER` | `AlterDatabasePerformCutoverStatement` |
| ALTER ENDPOINT | 🔴 Red | — |  | `ALTER ENDPOINT CoverageEndpoint STATE=STARTED;` | `AlterEndpointStatement` |
| ALTER EVENT SESSION | 🔴 Red | — |  | `ALTER EVENT SESSION es1 ON SERVER STATE = STOP` | `AlterEventSessionStatement` |
| ALTER EXTERNAL DATA SOURCE | 🔴 Red | — |  | `ALTER EXTERNAL DATA SOURCE eds3 SET CREDENTIAL = cred1` | `AlterExternalDataSourceStatement` |
| ALTER EXTERNAL LANGUAGE | 🔴 Red | — |  | `ALTER EXTERNAL LANGUAGE language6 REMOVE PLATFORM LINUX` | `AlterExternalLanguageStatement` |
| ALTER EXTERNAL LIBRARY | 🔴 Red | — |  | `ALTER EXTERNAL LIBRARY xts SET (CONTENT = 'c:\Path\xts.zip') WITH (LANGUAGE = 'R')` | `AlterExternalLibraryStatement` |
| ALTER EXTERNAL MODEL | 🔴 Red | — |  | `ALTER EXTERNAL MODEL abc SET ( MODEL = 'new_model' )` | `AlterExternalModelStatement` |
| ALTER EXTERNAL RESOURCE POOL | 🔴 Red | — |  | `ALTER EXTERNAL RESOURCE POOL CoverageExternalPool WITH(MAX_CPU_PERCENT=80);` | `AlterExternalResourcePoolStatement` |
| ALTER FEDERATION | 🔴 Red | — |  | `ALTER FEDERATION f1 SPLIT AT (k1 = 10)` | `AlterFederationStatement` |
| ALTER FULLTEXT CATALOG | 🔴 Red | — |  | `ALTER FULLTEXT CATALOG c1 REBUILD` | `AlterFullTextCatalogStatement` |
| ALTER FULLTEXT INDEX | 🔴 Red | — |  | `ALTER FULLTEXT INDEX ON t1 DISABLE` | `AlterFullTextIndexStatement` |
| ALTER FULLTEXT STOPLIST | 🔴 Red | — |  | `ALTER FULLTEXT STOPLIST fs1 DROP ALL;` | `AlterFullTextStopListStatement` |
| ALTER FUNCTION | 🔴 Red | — |  | `ALTER FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 2; END;` | `AlterFunctionStatement` |
| ALTER FUNCTION … AS EXTERNAL FUNCTION | 🔴 Red | — | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `ALTER FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | `AlterExternalFunctionStatement` |
| ALTER INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER INDEX ALL ON t1 DISABLE` | `AlterIndexStatement` |
| ALTER LOGIN … ADD / DROP CREDENTIAL | 🔴 Red | — |  | `ALTER LOGIN l1 ADD CREDENTIAL cr1` | `AlterLoginAddDropCredentialStatement` |
| ALTER LOGIN … ENABLE / DISABLE | 🔴 Red | — |  | `ALTER LOGIN l1 ENABLE` | `AlterLoginEnableDisableStatement` |
| ALTER LOGIN … WITH | 🔴 Red | — |  | `ALTER LOGIN l1 WITH CREDENTIAL = cr1` | `AlterLoginOptionsStatement` |
| ALTER MASTER KEY | 🔴 Red | — |  | `ALTER MASTER KEY ADD ENCRYPTION BY PASSWORD = 'pw1'` | `AlterMasterKeyStatement` |
| ALTER MESSAGE TYPE | 🔴 Red | — |  | `ALTER MESSAGE TYPE m1 VALIDATION = EMPTY` | `AlterMessageTypeStatement` |
| ALTER PARTITION FUNCTION | 🔴 Red | — |  | `ALTER PARTITION FUNCTION pfItems() MERGE RANGE (100);` | `AlterPartitionFunctionStatement` |
| ALTER PARTITION SCHEME | 🔴 Red | — |  | `ALTER PARTITION SCHEME s1 NEXT USED` | `AlterPartitionSchemeStatement` |
| ALTER PROCEDURE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER PROCEDURE dbo.P AS SELECT 2;` | `AlterProcedureStatement` |
| ALTER QUEUE | 🔴 Red | — |  | `ALTER QUEUE dbo.q1 WITH ACTIVATION (DROP)` | `AlterQueueStatement` |
| ALTER REMOTE SERVICE BINDING | 🔴 Red | — |  | `ALTER REMOTE SERVICE BINDING b1 WITH USER = u1` | `AlterRemoteServiceBindingStatement` |
| ALTER RESOURCE GOVERNOR | 🔴 Red | — |  | `ALTER RESOURCE GOVERNOR DISABLE` | `AlterResourceGovernorStatement` |
| ALTER RESOURCE POOL | 🔴 Red | — |  | `ALTER RESOURCE POOL CoveragePool WITH(MAX_CPU_PERCENT=80);` | `AlterResourcePoolStatement` |
| ALTER ROLE | 🔴 Red | — |  | `ALTER ROLE r1 WITH NAME = newName` | `AlterRoleStatement` |
| ALTER ROUTE | 🔴 Red | — |  | `ALTER ROUTE CoverageRoute WITH SERVICE_NAME = N'CoverageServiceTwo', ADDRESS = N'LOCAL';` | `AlterRouteStatement` |
| ALTER SCHEMA | 🔴 Red | — |  | `ALTER SCHEMA sc1 TRANSFER a1.b1` | `AlterSchemaStatement` |
| ALTER SEARCH PROPERTY LIST | 🔴 Red | — |  | `ALTER SEARCH PROPERTY LIST foo DROP 'bax';` | `AlterSearchPropertyListStatement` |
| ALTER SECURITY POLICY | 🔴 Red | — |  | `ALTER SECURITY POLICY sec1 WITH (STATE = ON)` | `AlterSecurityPolicyStatement` |
| ALTER SEQUENCE | 🔴 Red | — |  | `ALTER SEQUENCE S1 NO CACHE` | `AlterSequenceStatement` |
| ALTER SERVER AUDIT | 🔴 Red | — |  | `ALTER SERVER AUDIT a1 REMOVE WHERE` | `AlterServerAuditStatement` |
| ALTER SERVER AUDIT SPECIFICATION | 🔴 Red | — |  | `ALTER SERVER AUDIT SPECIFICATION AuditSpec1` | `AlterServerAuditSpecificationStatement` |
| ALTER SERVER CONFIGURATION | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET PROCESS AFFINITY CPU=AUTO;` | `AlterServerConfigurationStatement` |
| ALTER SERVER CONFIGURATION SET BUFFER POOL EXTENSION | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET BUFFER POOL EXTENSION OFF` | `AlterServerConfigurationSetBufferPoolExtensionStatement` |
| ALTER SERVER CONFIGURATION SET DIAGNOSTICS LOG | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET DIAGNOSTICS LOG ON` | `AlterServerConfigurationSetDiagnosticsLogStatement` |
| ALTER SERVER CONFIGURATION SET EXTERNAL AUTHENTICATION | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET EXTERNAL AUTHENTICATION OFF` | `AlterServerConfigurationSetExternalAuthenticationStatement` |
| ALTER SERVER CONFIGURATION SET FAILOVER CLUSTER PROPERTY | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET FAILOVER CLUSTER PROPERTY VerboseLogging = 2` | `AlterServerConfigurationSetFailoverClusterPropertyStatement` |
| ALTER SERVER CONFIGURATION SET HADR CLUSTER | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET HADR CLUSTER CONTEXT = LOCAL` | `AlterServerConfigurationSetHadrClusterStatement` |
| ALTER SERVER CONFIGURATION SET SOFTNUMA | 🔴 Red | — |  | `ALTER SERVER CONFIGURATION SET SOFTNUMA ON` | `AlterServerConfigurationSetSoftNumaStatement` |
| ALTER SERVER ROLE | 🔴 Red | — |  | `ALTER SERVER ROLE r1 WITH NAME = newName` | `AlterServerRoleStatement` |
| ALTER SERVICE | 🔴 Red | — |  | `ALTER SERVICE s1 ON QUEUE dbo.q1` | `AlterServiceStatement` |
| ALTER SERVICE MASTER KEY | 🔴 Red | — |  | `ALTER SERVICE MASTER KEY REGENERATE` | `AlterServiceMasterKeyStatement` |
| ALTER SYMMETRIC KEY | 🔴 Red | — |  | `ALTER SYMMETRIC KEY k1 DROP ENCRYPTION BY PASSWORD = 'p1'` | `AlterSymmetricKeyStatement` |
| ALTER TABLE … ADD | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE T ADD jsonCol JSON` | `AlterTableAddTableElementStatement` |
| ALTER TABLE … ALTER COLUMN | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 ALTER COLUMN c1 INT` | `AlterTableAlterColumnStatement` |
| ALTER TABLE … ALTER INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 ALTER INDEX i1 REBUILD WITH (BUCKET_COUNT = 1)` | `AlterTableAlterIndexStatement` |
| ALTER TABLE … CHECK / NOCHECK CONSTRAINT | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 NOCHECK CONSTRAINT ALL` | `AlterTableConstraintModificationStatement` |
| ALTER TABLE … DROP | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 DROP INDEX i1` | `AlterTableDropTableElementStatement` |
| ALTER TABLE … ENABLE / DISABLE CHANGE_TRACKING | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 ENABLE CHANGE_TRACKING` | `AlterTableChangeTrackingModificationStatement` |
| ALTER TABLE … ENABLE / DISABLE FILETABLE_NAMESPACE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 ENABLE FILETABLE_NAMESPACE` | `AlterTableFileTableNamespaceStatement` |
| ALTER TABLE … ENABLE / DISABLE TRIGGER | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 ENABLE TRIGGER ALL` | `AlterTableTriggerModificationStatement` |
| ALTER TABLE … REBUILD | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE t1 REBUILD` | `AlterTableRebuildStatement` |
| ALTER TABLE … SET | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE T SET (SYSTEM_VERSIONING = ON)` | `AlterTableSetStatement` |
| ALTER TABLE … SPLIT / MERGE RANGE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `ALTER TABLE T MERGE RANGE (10)` | `AlterTableAlterPartitionStatement` |
| ALTER TABLE … SWITCH | 🔴 Red | 🟢 Green (#temp) · — (@table) | Temp column is temp-to-temp only; 🔴 Red with either side permanent. | `ALTER TABLE dbo.Items SWITCH TO dbo.Other;` | `AlterTableSwitchStatement` |
| ALTER TRIGGER | 🔴 Red | — |  | `ALTER TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'new';` | `AlterTriggerStatement` |
| ALTER USER | 🔴 Red | — |  | `ALTER USER u1 WITH NAME = newName` | `AlterUserStatement` |
| ALTER VIEW | 🔴 Red | — |  | `ALTER MATERIALIZED VIEW View1 DISABLE` | `AlterViewStatement` |
| ALTER WORKLOAD GROUP | 🔴 Red | — |  | `ALTER WORKLOAD GROUP g USING EXTERNAL p_ext` | `AlterWorkloadGroupStatement` |
| ALTER XML SCHEMA COLLECTION | 🔴 Red | — |  | `ALTER XML SCHEMA COLLECTION MyCollection ADD 'Some xml text'` | `AlterXmlSchemaCollectionStatement` |
| BACKUP CERTIFICATE | 🔴 Red | — |  | `BACKUP CERTIFICATE c1 TO FILE = 'f1'` | `BackupCertificateStatement` |
| BACKUP MASTER KEY | 🔴 Red | — |  | `BACKUP MASTER KEY TO FILE = 'f1' ENCRYPTION BY PASSWORD = 'p1'` | `BackupMasterKeyStatement` |
| BACKUP SERVICE MASTER KEY | 🔴 Red | — |  | `BACKUP SERVICE MASTER KEY TO FILE = 'f1' ENCRYPTION BY PASSWORD = 'pw1'` | `BackupServiceMasterKeyStatement` |
| BEGIN CONVERSATION TIMER | 🔴 Red | — |  | `BEGIN CONVERSATION TIMER (@ch) TIMEOUT = 10` | `BeginConversationTimerStatement` |
| BEGIN DIALOG | 🔴 Red | — |  | `BEGIN DIALOG @dh FROM SERVICE s1 TO SERVICE @s2` | `BeginDialogStatement` |
| BULK INSERT | 🔴 Red | 🟢 Green (#temp) · — (@table) | 🟢 Green with an allow-listed target. | `BULK INSERT dbo.Items FROM 'C:\data\items.csv';` | `BulkInsertStatement` |
| COPY INTO | 🔴 Red | — |  | `COPY INTO dbo.[FactOnlineSalesv1] FROM 'https://unsecureaccount.blob.core.windows.net/customerdatasets/linitem.csv'` | `CopyStatement` |
| CREATE AGGREGATE | 🔴 Red | — |  | `CREATE AGGREGATE a1(@input XML( zzz)) RETURNS XML( zzz) EXTERNAL NAME ag1` | `CreateAggregateStatement` |
| CREATE APPLICATION ROLE | 🔴 Red | — |  | `CREATE APPLICATION ROLE weekly_receipts WITH PASSWORD = '987'` | `CreateApplicationRoleStatement` |
| CREATE ASSEMBLY | 🔴 Red | — |  | `CREATE ASSEMBLY HelloWorld AUTHORIZATION dbo FROM @FullPath` | `CreateAssemblyStatement` |
| CREATE ASYMMETRIC KEY | 🔴 Red | — |  | `CREATE ASYMMETRIC KEY k1 WITH ALGORITHM = DES` | `CreateAsymmetricKeyStatement` |
| CREATE AVAILABILITY GROUP | 🔴 Red | — |  | `CREATE AVAILABILITY GROUP group1 WITH (REQUIRED_COPIES_TO_COMMIT = 10) FOR DATABASE db1, db2 REPLICA ON 'server1' WITH (AVAILABILITY_MODE = SYNCHRONOUS_COMMIT, FAILOVER_MODE = AUTOMATIC, ENDPOINT_URL = 'TCP://1234:80', SECONDARY_ROLE (ALLOW_CONNECTIONS = READ_ONLY), SESSION_TIMEOUT = 10, APPLY_DELAY = 1), 'server2' WITH (FAILOVER_MODE = MANUAL, APPLY_DELAY = 1, ENDPOINT_URL = 'TCP://1234:80', PRIMARY_ROLE (ALLOW_CONNECTIONS = ALL), AVAILABILITY_MODE = ASYNCHRONOUS_COMMIT, SESSION_TIMEOUT = 10)` | `CreateAvailabilityGroupStatement` |
| CREATE BROKER PRIORITY | 🔴 Red | — |  | `CREATE BROKER PRIORITY bp1 FOR CONVERSATION` | `CreateBrokerPriorityStatement` |
| CREATE CERTIFICATE | 🔴 Red | — |  | `CREATE CERTIFICATE s4 FROM FILE = 'f1'` | `CreateCertificateStatement` |
| CREATE COLUMN ENCRYPTION KEY | 🔴 Red | — |  | `CREATE COLUMN ENCRYPTION KEY CoverageCek WITH VALUES(COLUMN_MASTER_KEY=CoverageCmk,ALGORITHM='RSA_OAEP',ENCRYPTED_VALUE=0x0102030405);` | `CreateColumnEncryptionKeyStatement` |
| CREATE COLUMN MASTER KEY | 🔴 Red | — |  | `CREATE COLUMN MASTER KEY CoverageCmk WITH(KEY_STORE_PROVIDER_NAME=N'MSSQL_CERTIFICATE_STORE',KEY_PATH=N'CurrentUser/My/fixture');` | `CreateColumnMasterKeyStatement` |
| CREATE COLUMNSTORE INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE COLUMNSTORE INDEX cindx ON t(c1)` | `CreateColumnStoreIndexStatement` |
| CREATE CONTRACT | 🔴 Red | — |  | `CREATE CONTRACT c1 (m1 SENT BY INITIATOR)` | `CreateContractStatement` |
| CREATE CREDENTIAL | 🔴 Red | — |  | `CREATE CREDENTIAL c1 WITH IDENTITY = 'id1'` | `CreateCredentialStatement` |
| CREATE CRYPTOGRAPHIC PROVIDER | 🔴 Red | — |  | `CREATE CRYPTOGRAPHIC PROVIDER cp1 FROM FILE = 'c:\v1.dll'` | `CreateCryptographicProviderStatement` |
| CREATE DATABASE | 🔴 Red | — |  | `CREATE DATABASE d1` | `CreateDatabaseStatement` |
| CREATE DATABASE AUDIT SPECIFICATION | 🔴 Red | — |  | `CREATE DATABASE AUDIT SPECIFICATION AuditSpec1 FOR SERVER AUDIT a1` | `CreateDatabaseAuditSpecificationStatement` |
| CREATE DATABASE ENCRYPTION KEY | 🔴 Red | — |  | `CREATE DATABASE ENCRYPTION KEY WITH ALGORITHM = AES_128 ENCRYPTION BY SERVER CERTIFICATE enc1` | `CreateDatabaseEncryptionKeyStatement` |
| CREATE DEFAULT | 🔴 Red | — |  | `CREATE DEFAULT dbo.r1 AS (-10)` | `CreateDefaultStatement` |
| CREATE ENDPOINT | 🔴 Red | — |  | `CREATE ENDPOINT e1 AUTHORIZATION zzz AS TCP ( LISTENER_PORT = 4022 ) FOR TSQL()` | `CreateEndpointStatement` |
| CREATE EVENT NOTIFICATION | 🔴 Red | — |  | `CREATE EVENT NOTIFICATION log_ddl1 ON SERVER FOR OBJECT_CREATED TO SERVICE 'NotifyService', '8140'` | `CreateEventNotificationStatement` |
| CREATE EVENT SESSION | 🔴 Red | — |  | `CREATE EVENT SESSION es1 ON SERVER ADD EVENT b.c` | `CreateEventSessionStatement` |
| CREATE EXTERNAL DATA SOURCE | 🔴 Red | — |  | `CREATE EXTERNAL DATA SOURCE eds1 WITH ( LOCATION = 'protocol://ip_address:port' )` | `CreateExternalDataSourceStatement` |
| CREATE EXTERNAL FILE FORMAT | 🔴 Red | — |  | `CREATE EXTERNAL FILE FORMAT eff3 WITH ( FORMAT_TYPE = ORC )` | `CreateExternalFileFormatStatement` |
| CREATE EXTERNAL LANGUAGE | 🔴 Red | — |  | `CREATE EXTERNAL LANGUAGE language4 FROM (CONTENT = '\\machine-name\dir1\dir2\windowsextension.zip', FILE_NAME = 'WindowsExtension.dll', PLATFORM = WINDOWS)` | `CreateExternalLanguageStatement` |
| CREATE EXTERNAL LIBRARY | 🔴 Red | — |  | `CREATE EXTERNAL LIBRARY xts FROM (CONTENT = 'c:\Path\xts.zip') WITH (LANGUAGE = 'R')` | `CreateExternalLibraryStatement` |
| CREATE EXTERNAL MODEL | 🔴 Red | — |  | `CREATE EXTERNAL MODEL simple_model WITH ( LOCATION = '/models/simple', API_FORMAT = 'OpenAI', MODEL_TYPE = EMBEDDINGS, MODEL = 'gpt-3.5-turbo' )` | `CreateExternalModelStatement` |
| CREATE EXTERNAL RESOURCE POOL | 🔴 Red | — |  | `CREATE EXTERNAL RESOURCE POOL p` | `CreateExternalResourcePoolStatement` |
| CREATE EXTERNAL STREAM | 🔴 Red | — |  | `CREATE EXTERNAL STREAM CoverageStream WITH(LOCATION='fixture');` | `CreateExternalStreamStatement` |
| CREATE EXTERNAL TABLE | 🔴 Red | — |  | `CREATE EXTERNAL TABLE dbo.CoverageExternal(Id int) WITH(DATA_SOURCE=CoverageSource,LOCATION='fixture');` | `CreateExternalTableStatement` |
| CREATE FEDERATION | 🔴 Red | — |  | `CREATE FEDERATION f1 (k1 BIGINT RANGE)` | `CreateFederationStatement` |
| CREATE FULLTEXT CATALOG | 🔴 Red | — |  | `CREATE FULLTEXT CATALOG c1` | `CreateFullTextCatalogStatement` |
| CREATE FULLTEXT INDEX | 🔴 Red | — |  | `CREATE FULLTEXT INDEX ON dbo.Items (Description) KEY INDEX PK_Items;` | `CreateFullTextIndexStatement` |
| CREATE FULLTEXT STOPLIST | 🔴 Red | — |  | `CREATE FULLTEXT STOPLIST fs1;` | `CreateFullTextStopListStatement` |
| CREATE FUNCTION | 🔴 Red | — |  | `CREATE FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 1; END;` | `CreateFunctionStatement` |
| CREATE FUNCTION … AS EXTERNAL FUNCTION | 🔴 Red | — | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `CREATE FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | `CreateExternalFunctionStatement` |
| CREATE INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE INDEX ind1 ON t1(c1)` | `CreateIndexStatement` |
| CREATE JSON INDEX | 🔴 Red | — |  | `CREATE JSON INDEX IX_JSON_Basic ON dbo.Users (JsonData)` | `CreateJsonIndexStatement` |
| CREATE LOGIN | 🔴 Red | — |  | `CREATE LOGIN [l1] FROM WINDOWS` | `CreateLoginStatement` |
| CREATE MASTER KEY | 🔴 Red | — |  | `CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Example-only-password!';` | `CreateMasterKeyStatement` |
| CREATE MESSAGE TYPE | 🔴 Red | — |  | `CREATE MESSAGE TYPE m1 VALIDATION = EMPTY` | `CreateMessageTypeStatement` |
| CREATE OR ALTER FUNCTION | 🔴 Red | — |  | `CREATE OR ALTER FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 2; END;` | `CreateOrAlterFunctionStatement` |
| CREATE OR ALTER FUNCTION … AS EXTERNAL FUNCTION | 🔴 Red | — | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `CREATE OR ALTER FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | `CreateOrAlterExternalFunctionStatement` |
| CREATE OR ALTER PROCEDURE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE OR ALTER PROCEDURE dbo.P AS SELECT 2;` | `CreateOrAlterProcedureStatement` |
| CREATE OR ALTER TRIGGER | 🔴 Red | — |  | `CREATE OR ALTER TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'new';` | `CreateOrAlterTriggerStatement` |
| CREATE OR ALTER VIEW | 🔴 Red | — |  | `CREATE OR ALTER VIEW dbo.V AS SELECT Id FROM dbo.Items;` | `CreateOrAlterViewStatement` |
| CREATE PARTITION FUNCTION | 🔴 Red | — |  | `CREATE PARTITION FUNCTION PF(int) AS RANGE LEFT FOR VALUES(10);` | `CreatePartitionFunctionStatement` |
| CREATE PARTITION SCHEME | 🔴 Red | — |  | `CREATE PARTITION SCHEME PS AS PARTITION PF ALL TO ([PRIMARY]);` | `CreatePartitionSchemeStatement` |
| CREATE PROCEDURE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE PROCEDURE p1 AS RETURN 0;` | `CreateProcedureStatement` |
| CREATE QUEUE | 🔴 Red | — |  | `CREATE QUEUE dbo.q1` | `CreateQueueStatement` |
| CREATE REMOTE SERVICE BINDING | 🔴 Red | — |  | `CREATE REMOTE SERVICE BINDING CoverageBinding TO SERVICE N'CoverageService' WITH USER = CoverageBrokerUser, ANONYMOUS = ON;` | `CreateRemoteServiceBindingStatement` |
| CREATE RESOURCE POOL | 🔴 Red | — |  | `CREATE RESOURCE POOL CoveragePool;` | `CreateResourcePoolStatement` |
| CREATE ROLE | 🔴 Red | — |  | `CREATE ROLE r1` | `CreateRoleStatement` |
| CREATE ROUTE | 🔴 Red | — |  | `CREATE ROUTE r1 WITH SERVICE_NAME = 's1'` | `CreateRouteStatement` |
| CREATE RULE | 🔴 Red | — |  | `CREATE RULE dbo.r1 AS @a1 > 10` | `CreateRuleStatement` |
| CREATE SCHEMA | 🔴 Red | — |  | `CREATE SCHEMA [dev]` | `CreateSchemaStatement` |
| CREATE SEARCH PROPERTY LIST | 🔴 Red | — |  | `CREATE SEARCH PROPERTY LIST new_list;` | `CreateSearchPropertyListStatement` |
| CREATE SECURITY POLICY | 🔴 Red | — |  | `CREATE SECURITY POLICY dbo.sec1` | `CreateSecurityPolicyStatement` |
| CREATE SELECTIVE XML INDEX | 🔴 Red | — |  | `CREATE SELECTIVE XML INDEX sxi1 ON t1(c1) FOR( path1 = '/a/b/c' )` | `CreateSelectiveXmlIndexStatement` |
| CREATE SEMANTIC INDEX | 🔴 Red | — |  | `CREATE SEMANTIC INDEX SI_Basic ON dbo.Documents (content) WITH (EXTERNAL_MODEL = MyModel)` | `CreateSemanticIndexStatement` |
| CREATE SEQUENCE | 🔴 Red | — |  | `CREATE SEQUENCE S1 NO CACHE` | `CreateSequenceStatement` |
| CREATE SERVER AUDIT | 🔴 Red | — |  | `CREATE SERVER AUDIT a1 TO EXTERNAL_MONITOR` | `CreateServerAuditStatement` |
| CREATE SERVER AUDIT SPECIFICATION | 🔴 Red | — |  | `CREATE SERVER AUDIT SPECIFICATION AuditSpec1 FOR SERVER AUDIT a1` | `CreateServerAuditSpecificationStatement` |
| CREATE SERVER ROLE | 🔴 Red | — |  | `CREATE SERVER ROLE r1` | `CreateServerRoleStatement` |
| CREATE SERVICE | 🔴 Red | — |  | `CREATE SERVICE s1 ON QUEUE q1 (c1)` | `CreateServiceStatement` |
| CREATE SPATIAL INDEX | 🔴 Red | — |  | `CREATE SPATIAL INDEX sp1 ON a.c (d) ON ab` | `CreateSpatialIndexStatement` |
| CREATE STATISTICS | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE STATISTICS st1 ON t1(c1)` | `CreateStatisticsStatement` |
| CREATE SYMMETRIC KEY | 🔴 Red | — |  | `CREATE SYMMETRIC KEY k1 FROM PROVIDER p1` | `CreateSymmetricKeyStatement` |
| CREATE SYNONYM | 🔴 Red | — |  | `CREATE SYNONYM mysyn FOR t1` | `CreateSynonymStatement` |
| CREATE TABLE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `CREATE TABLE dbo.Items (Id int);` | `CreateTableStatement` |
| CREATE TABLE … INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) | Inline INDEX inside CREATE TABLE, not standalone SQL; the enclosing table is analyzed. | `CREATE TABLE dbo.Items (Id int, INDEX IX_Items (Id));` | `IndexDefinition` |
| CREATE TRIGGER | 🔴 Red | — |  | `CREATE TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'created';` | `CreateTriggerStatement` |
| CREATE TYPE … AS TABLE | 🔴 Red | — |  | `CREATE TYPE dbo.ItemList AS TABLE (Id int);` | `CreateTypeTableStatement` |
| CREATE TYPE … EXTERNAL NAME | 🔴 Red | — |  | `CREATE TYPE SSN EXTERNAL NAME [SomeAssembly]` | `CreateTypeUdtStatement` |
| CREATE TYPE … FROM | 🔴 Red | — |  | `CREATE TYPE [schema].SSN FROM INT NULL` | `CreateTypeUddtStatement` |
| CREATE USER | 🔴 Red | — |  | `CREATE USER u1` | `CreateUserStatement` |
| CREATE VECTOR INDEX | 🔴 Red | — |  | `CREATE VECTOR INDEX IX_Vector_Basic ON dbo.Documents (VectorData)` | `CreateVectorIndexStatement` |
| CREATE VIEW | 🔴 Red | — |  | `CREATE VIEW v1 AS SELECT * FROM t1` | `CreateViewStatement` |
| CREATE WORKLOAD CLASSIFIER | 🔴 Red | — |  | `CREATE WORKLOAD CLASSIFIER wcAllOptions WITH ( WORKLOAD_GROUP = 'wgDefaultParams', MEMBERNAME = 'ELTRole', WLM_CONTEXT = 'dim_load', START_TIME = '22:00', END_TIME = '02:00', WLM_LABEL = 'label', IMPORTANCE = HIGH )` | `CreateWorkloadClassifierStatement` |
| CREATE WORKLOAD GROUP | 🔴 Red | — |  | `CREATE WORKLOAD GROUP CoverageGroup;` | `CreateWorkloadGroupStatement` |
| CREATE XML INDEX | 🔴 Red | — |  | `CREATE PRIMARY XML INDEX c1 ON t1(c1)` | `CreateXmlIndexStatement` |
| CREATE XML SCHEMA COLLECTION | 🔴 Red | — |  | `CREATE XML SCHEMA COLLECTION MyCollection AS 'Some xml text'` | `CreateXmlSchemaCollectionStatement` |
| DELETE | 🔴 Red | 🟢 Green | 🟢 Green with an allow-listed target. | `DELETE FROM dbo.Items WHERE Id = 1;` | `DeleteStatement` |
| DENY | 🔴 Red | — |  | `DENY all TO PUBLIC CASCADE` | `DenyStatement` |
| DENY | 🔴 Red | — | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `DENY SELECT ON dbo.Coverage TO public;` | `DenyStatement80` |
| DROP AGGREGATE | 🔴 Red | — |  | `DROP AGGREGATE aggX` | `DropAggregateStatement` |
| DROP APPLICATION ROLE | 🔴 Red | — |  | `DROP APPLICATION ROLE a1` | `DropApplicationRoleStatement` |
| DROP ASSEMBLY | 🔴 Red | — |  | `DROP ASSEMBLY a` | `DropAssemblyStatement` |
| DROP ASYMMETRIC KEY | 🔴 Red | — |  | `DROP ASYMMETRIC KEY k1` | `DropAsymmetricKeyStatement` |
| DROP AVAILABILITY GROUP | 🔴 Red | — |  | `DROP AVAILABILITY GROUP group1` | `DropAvailabilityGroupStatement` |
| DROP BROKER PRIORITY | 🔴 Red | — |  | `DROP BROKER PRIORITY bp1` | `DropBrokerPriorityStatement` |
| DROP CERTIFICATE | 🔴 Red | — |  | `DROP CERTIFICATE c1` | `DropCertificateStatement` |
| DROP COLUMN ENCRYPTION KEY | 🔴 Red | — |  | `DROP COLUMN ENCRYPTION KEY OneValueCEK` | `DropColumnEncryptionKeyStatement` |
| DROP COLUMN MASTER KEY | 🔴 Red | — |  | `DROP COLUMN MASTER KEY CMK1` | `DropColumnMasterKeyStatement` |
| DROP CONTRACT | 🔴 Red | — |  | `DROP CONTRACT c1` | `DropContractStatement` |
| DROP CREDENTIAL | 🔴 Red | — |  | `DROP CREDENTIAL c1` | `DropCredentialStatement` |
| DROP CRYPTOGRAPHIC PROVIDER | 🔴 Red | — |  | `DROP CRYPTOGRAPHIC PROVIDER cp1` | `DropCryptographicProviderStatement` |
| DROP DATABASE | 🔴 Red | — |  | `DROP DATABASE dbX` | `DropDatabaseStatement` |
| DROP DATABASE AUDIT SPECIFICATION | 🔴 Red | — |  | `DROP DATABASE AUDIT SPECIFICATION AuditSpec1` | `DropDatabaseAuditSpecificationStatement` |
| DROP DATABASE ENCRYPTION KEY | 🔴 Red | — |  | `DROP DATABASE ENCRYPTION KEY` | `DropDatabaseEncryptionKeyStatement` |
| DROP DEFAULT | 🔴 Red | — |  | `DROP DEFAULT d1` | `DropDefaultStatement` |
| DROP ENDPOINT | 🔴 Red | — |  | `DROP ENDPOINT e1` | `DropEndpointStatement` |
| DROP EVENT NOTIFICATION | 🔴 Red | — |  | `DROP EVENT NOTIFICATION n1 ON QUEUE q1` | `DropEventNotificationStatement` |
| DROP EVENT SESSION | 🔴 Red | — |  | `DROP EVENT SESSION es1 ON SERVER` | `DropEventSessionStatement` |
| DROP EXTERNAL DATA SOURCE | 🔴 Red | — |  | `DROP EXTERNAL DATA SOURCE eds1` | `DropExternalDataSourceStatement` |
| DROP EXTERNAL FILE FORMAT | 🔴 Red | — |  | `DROP EXTERNAL FILE FORMAT eff1` | `DropExternalFileFormatStatement` |
| DROP EXTERNAL LANGUAGE | 🔴 Red | — |  | `DROP EXTERNAL LANGUAGE language1` | `DropExternalLanguageStatement` |
| DROP EXTERNAL LIBRARY | 🔴 Red | — |  | `DROP EXTERNAL LIBRARY eltest` | `DropExternalLibraryStatement` |
| DROP EXTERNAL MODEL | 🔴 Red | — |  | `DROP EXTERNAL MODEL my_model1` | `DropExternalModelStatement` |
| DROP EXTERNAL RESOURCE POOL | 🔴 Red | — |  | `DROP EXTERNAL RESOURCE POOL p` | `DropExternalResourcePoolStatement` |
| DROP EXTERNAL TABLE | 🔴 Red | — |  | `DROP EXTERNAL TABLE t1` | `DropExternalTableStatement` |
| DROP FEDERATION | 🔴 Red | — |  | `DROP FEDERATION fed1` | `DropFederationStatement` |
| DROP FULLTEXT CATALOG | 🔴 Red | — |  | `DROP FULLTEXT CATALOG c1` | `DropFullTextCatalogStatement` |
| DROP FULLTEXT INDEX | 🔴 Red | — |  | `DROP FULLTEXT INDEX ON t1` | `DropFullTextIndexStatement` |
| DROP FULLTEXT STOPLIST | 🔴 Red | — |  | `DROP FULLTEXT STOPLIST fs1;` | `DropFullTextStopListStatement` |
| DROP FUNCTION | 🔴 Red | — |  | `DROP FUNCTION f1` | `DropFunctionStatement` |
| DROP INDEX | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `DROP INDEX IX_Items ON dbo.Items;` | `DropIndexStatement` |
| DROP LOGIN | 🔴 Red | — |  | `DROP LOGIN l1` | `DropLoginStatement` |
| DROP MASTER KEY | 🔴 Red | — |  | `DROP MASTER KEY` | `DropMasterKeyStatement` |
| DROP MESSAGE TYPE | 🔴 Red | — |  | `DROP MESSAGE TYPE mt1` | `DropMessageTypeStatement` |
| DROP PARTITION FUNCTION | 🔴 Red | — |  | `DROP PARTITION FUNCTION f1` | `DropPartitionFunctionStatement` |
| DROP PARTITION SCHEME | 🔴 Red | — |  | `DROP PARTITION SCHEME s1` | `DropPartitionSchemeStatement` |
| DROP PROCEDURE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `DROP PROCEDURE p1` | `DropProcedureStatement` |
| DROP QUEUE | 🔴 Red | — |  | `DROP QUEUE q1` | `DropQueueStatement` |
| DROP REMOTE SERVICE BINDING | 🔴 Red | — |  | `DROP REMOTE SERVICE BINDING rsb1` | `DropRemoteServiceBindingStatement` |
| DROP RESOURCE POOL | 🔴 Red | — |  | `DROP RESOURCE POOL res_pool` | `DropResourcePoolStatement` |
| DROP ROLE | 🔴 Red | — |  | `DROP ROLE r1` | `DropRoleStatement` |
| DROP ROUTE | 🔴 Red | — |  | `DROP ROUTE r1` | `DropRouteStatement` |
| DROP RULE | 🔴 Red | — |  | `DROP RULE [r1]` | `DropRuleStatement` |
| DROP SCHEMA | 🔴 Red | — |  | `DROP SCHEMA s1` | `DropSchemaStatement` |
| DROP SEARCH PROPERTY LIST | 🔴 Red | — |  | `DROP SEARCH PROPERTY LIST spl1;` | `DropSearchPropertyListStatement` |
| DROP SECURITY POLICY | 🔴 Red | — |  | `DROP SECURITY POLICY secX` | `DropSecurityPolicyStatement` |
| DROP SENSITIVITY CLASSIFICATION | 🔴 Red | — |  | `DROP SENSITIVITY CLASSIFICATION FROM T1.C1, T1.C2` | `DropSensitivityClassificationStatement` |
| DROP SEQUENCE | 🔴 Red | — |  | `DROP SEQUENCE seqX` | `DropSequenceStatement` |
| DROP SERVER AUDIT | 🔴 Red | — |  | `DROP SERVER AUDIT a1` | `DropServerAuditStatement` |
| DROP SERVER AUDIT SPECIFICATION | 🔴 Red | — |  | `DROP SERVER AUDIT SPECIFICATION Spec1` | `DropServerAuditSpecificationStatement` |
| DROP SERVER ROLE | 🔴 Red | — |  | `DROP SERVER ROLE sr1` | `DropServerRoleStatement` |
| DROP SERVICE | 🔴 Red | — |  | `DROP SERVICE s1` | `DropServiceStatement` |
| DROP SIGNATURE | 🔴 Red | — |  | `DROP SIGNATURE FROM OBJECT::dbo.CoverageSigned BY CERTIFICATE CoverageCert;` | `DropSignatureStatement` |
| DROP STATISTICS | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `DROP STATISTICS dbo.Items.Stats;` | `DropStatisticsStatement` |
| DROP SYMMETRIC KEY | 🔴 Red | — |  | `DROP SYMMETRIC KEY k1` | `DropSymmetricKeyStatement` |
| DROP SYNONYM | 🔴 Red | — |  | `DROP SYNONYM synX` | `DropSynonymStatement` |
| DROP TABLE | 🔴 Red | 🟢 Green (#temp) · — (@table) |  | `DROP TABLE dbo.Items;` | `DropTableStatement` |
| DROP TRIGGER | 🔴 Red | — |  | `DROP TRIGGER t1` | `DropTriggerStatement` |
| DROP TYPE | 🔴 Red | — |  | `DROP TYPE t1` | `DropTypeStatement` |
| DROP USER | 🔴 Red | — |  | `DROP USER u1` | `DropUserStatement` |
| DROP VIEW | 🔴 Red | — |  | `DROP VIEW v1` | `DropViewStatement` |
| DROP WORKLOAD CLASSIFIER | 🔴 Red | — |  | `DROP WORKLOAD CLASSIFIER wcAllOptions` | `DropWorkloadClassifierStatement` |
| DROP WORKLOAD GROUP | 🔴 Red | — |  | `DROP WORKLOAD GROUP wg1` | `DropWorkloadGroupStatement` |
| DROP XML SCHEMA COLLECTION | 🔴 Red | — |  | `DROP XML SCHEMA COLLECTION a` | `DropXmlSchemaCollectionStatement` |
| ENABLE / DISABLE TRIGGER | 🔴 Red | — |  | `ENABLE TRIGGER t1 ON o1` | `EnableDisableTriggerStatement` |
| END CONVERSATION | 🔴 Red | — |  | `END CONVERSATION 10` | `EndConversationStatement` |
| EXEC sys.sp_create_streaming_job | 🔴 Red | — |  | `EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1'` | `CreateExternalStreamingJobStatement` |
| GRANT | 🔴 Red | — |  | `GRANT BACKUP LOG TO [guest]` | `GrantStatement` |
| GRANT | 🔴 Red | — | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `GRANT SELECT ON dbo.Coverage TO public;` | `GrantStatement80` |
| INSERT | 🔴 Red | 🟢 Green | 🟢 Green with an allow-listed target. | `INSERT dbo.Items VALUES (3,30);` | `InsertStatement` |
| INSERT BULK | 🔴 Red | 🟢 Green (#temp) · — (@table) | 🟢 Green with an allow-listed target. | `INSERT BULK db1.dbo.t1` | `InsertBulkStatement` |
| KILL | 🔴 Red | — |  | `KILL 12` | `KillStatement` |
| KILL QUERY NOTIFICATION SUBSCRIPTION | 🔴 Red | — |  | `KILL QUERY NOTIFICATION SUBSCRIPTION 12` | `KillQueryNotificationSubscriptionStatement` |
| KILL STATS JOB | 🔴 Red | — |  | `KILL STATS JOB 12` | `KillStatsJobStatement` |
| MERGE | 🔴 Red | 🟢 Green | 🟢 Green with an allow-listed target. | `MERGE dbo.Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;` | `MergeStatement` |
| MOVE CONVERSATION | 🔴 Red | — |  | `MOVE CONVERSATION @handle TO @destination;` | `MoveConversationStatement` |
| RECEIVE | 🔴 Red | — |  | `RECEIVE * FROM ExpenseQueue` | `ReceiveStatement` |
| RECONFIGURE | 🔴 Red | — |  | `RECONFIGURE` | `ReconfigureStatement` |
| RENAME OBJECT | 🔴 Red | — |  | `RENAME OBJECT T2 TO T1` | `RenameEntityStatement` |
| RESTORE DATABASE / LOG | 🔴 Red | — |  | `RESTORE LOG db1` | `RestoreStatement` |
| RESTORE MASTER KEY | 🔴 Red | — |  | `RESTORE MASTER KEY FROM FILE = 'f1' DECRYPTION BY PASSWORD = 'p1' ENCRYPTION BY PASSWORD = 'p2'` | `RestoreMasterKeyStatement` |
| RESTORE SERVICE MASTER KEY | 🔴 Red | — |  | `RESTORE SERVICE MASTER KEY FROM FILE = 'f1' DECRYPTION BY PASSWORD = 'pw1'` | `RestoreServiceMasterKeyStatement` |
| REVOKE | 🔴 Red | — |  | `REVOKE all TO PUBLIC CASCADE` | `RevokeStatement` |
| REVOKE | 🔴 Red | — | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `REVOKE SELECT ON dbo.Coverage FROM public;` | `RevokeStatement80` |
| SEND | 🔴 Red | — |  | `SEND ON CONVERSATION (@dialog_handle) MESSAGE TYPE m1` | `SendStatement` |
| SHUTDOWN | 🔴 Red | — |  | `SHUTDOWN` | `ShutdownStatement` |
| TRUNCATE TABLE | 🔴 Red | 🟢 Green (#temp) · — (@table) | 🟢 Green with an allow-listed target. | `TRUNCATE TABLE dbo.Items;` | `TruncateTableStatement` |
| UPDATE | 🔴 Red | 🟢 Green | 🟢 Green with an allow-listed target. | `UPDATE dbo.Items SET Qty = 20 WHERE Id = 1;` | `UpdateStatement` |
| UPDATETEXT | 🔴 Red | 🔴 Red (#temp) · — (@table) |  | `UPDATETEXT dbo.Items.Body @ptrval 0 3 'new';` | `UpdateTextStatement` |
| WHILE | 🔴 Red | — | Contained statements are analyzed too; their effects can change the colour. | `WHILE @a > 10 CREATE TABLE t1 ( c1 INT );` | `WhileStatement` |
| WRITETEXT | 🔴 Red | 🔴 Red (#temp) · — (@table) |  | `WRITETEXT dbo.Items.Body @ptrval 0xFF;` | `WriteTextStatement` |

## AST-only kinds

| Statement | On a permanent table | On a #temp table / @table variable | Other cases | Example | Kind |
|---|---|---|---|---|---|
| DROP EXTERNAL STREAM | — | — | No standalone SQL: Generator AST node; pinned parser has no DROP EXTERNAL STREAM production. | — | `DropExternalStreamStatement` |
| DROP EXTERNAL STREAMING JOB | — | — | No standalone SQL: Generator AST node; sp_drop_streaming_job parses as ExecuteStatement, not this node. | — | `DropExternalStreamingJobStatement` |
| EVENT SESSION | — | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter event-session subclasses. | — | `EventSessionStatement` |
| EXTERNAL RESOURCE POOL | — | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter external-resource-pool subclasses. | — | `ExternalResourcePoolStatement` |
| RESOURCE POOL | — | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter resource-pool subclasses. | — | `ResourcePoolStatement` |
| SELECT snippet (AST only) | — | — | No standalone SQL: Programmatically constructed SELECT snippet, not a standalone parser statement kind. | — | `SelectStatementSnippet` |
| SQL snippet (AST only) | — | — | No standalone SQL: Programmatically constructed raw statement snippet; not a parser production. | — | `TSqlStatementSnippet` |

Generated by StatementVerdictsTests from the coverage manifest and fixtures. To regenerate in PowerShell:

~~~powershell
$env:UPDATE_STATEMENT_VERDICTS='1'
dotnet test tests/SqlSafetyGuard.Tests --filter FullyQualifiedName~StatementVerdictsTests
Remove-Item Env:UPDATE_STATEMENT_VERDICTS
~~~
