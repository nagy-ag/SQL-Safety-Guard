# Statement verdicts

🟢 Green = read-only or temp-only, runs without asking; 🟡 Yellow = can't tell, asks; 🔴 Red = changes permanent data/schema/server (or is conservatively treated as doing so), asks.
Verdicts use default settings: no allow-list and no environment rules; explicitly configured exceptions are noted.
One representative example per statement kind, not every clause combination; snippets are analyzed, not executed, without fixture setup.
See [live-server evidence](verification/2026-10-27-all-kinds/README.md); AST-only kinds have no standalone SQL verdict and appear last.

| Verdict | Statement | Example | When it differs | Kind |
|---|---|---|---|---|
| 🟢 Green | BEGIN ATOMIC … END | `BEGIN ATOMIC WITH (TRANSACTION ISOLATION LEVEL = SNAPSHOT, LANGUAGE = N'us_english') RETURN @a + 42; END` | Contained statements are analyzed too; their effects can change the colour. | `BeginEndAtomicBlockStatement` |
| 🟢 Green | BEGIN TRY … END TRY / BEGIN CATCH … END CATCH | `BEGIN TRY DECLARE @v1 AS INT; END TRY BEGIN CATCH END CATCH` | Contained statements are analyzed too; their effects can change the colour. | `TryCatchStatement` |
| 🟢 Green | BEGIN … END | `BEGIN RETURN 1; END` | Contained statements are analyzed too; their effects can change the colour. | `BeginEndBlockStatement` |
| 🟢 Green | BREAK | `BREAK` |  | `BreakStatement` |
| 🟢 Green | CLOSE | `CLOSE GLOBAL c1` |  | `CloseCursorStatement` |
| 🟢 Green | CLOSE MASTER KEY | `CLOSE MASTER KEY` |  | `CloseMasterKeyStatement` |
| 🟢 Green | CLOSE SYMMETRIC KEY | `CLOSE ALL SYMMETRIC KEYS` |  | `CloseSymmetricKeyStatement` |
| 🟢 Green | CONTINUE | `CONTINUE` |  | `ContinueStatement` |
| 🟢 Green | DBCC CHECKDB | `DBCC CHECKDB (N'Coverage');` | 🔴 Red for a changing command (`DBCC FREEPROCCACHE;`). 🔴 Red with repair (`DBCC CHECKDB (N'Coverage', REPAIR_ALLOW_DATA_LOSS);`). 🔴 Red for an unrecognized command (`DBCC SSG_UNKNOWN;`). | `DbccStatement` |
| 🟢 Green | DEALLOCATE | `DEALLOCATE @cursorVar` |  | `DeallocateCursorStatement` |
| 🟢 Green | DECLARE @table TABLE | `DECLARE @items TABLE (Id int);` |  | `DeclareTableVariableStatement` |
| 🟢 Green | DECLARE @variable | `DECLARE @value int;` |  | `DeclareVariableStatement` |
| 🟢 Green | DECLARE … CURSOR | `DECLARE t1_cursor CURSOR FOR SELECT * FROM t1` |  | `DeclareCursorStatement` |
| 🟢 Green | EXECUTE AS | `EXECUTE AS CALLER` |  | `ExecuteAsStatement` |
| 🟢 Green | FETCH | `FETCH LAST FROM ac` |  | `FetchCursorStatement` |
| 🟢 Green | GET CONVERSATION GROUP | `GET CONVERSATION GROUP @group FROM dbo.CoverageQueue;` |  | `GetConversationGroupStatement` |
| 🟢 Green | GOTO | `GOTO start` |  | `GoToStatement` |
| 🟢 Green | IF | `IF @counter=2 BREAK;` | Contained statements are analyzed too; their effects can change the colour. | `IfStatement` |
| 🟢 Green | LINENO | `LINENO 1;` |  | `LineNoStatement` |
| 🟢 Green | OPEN | `OPEN authorcursor` |  | `OpenCursorStatement` |
| 🟢 Green | OPEN MASTER KEY | `OPEN MASTER KEY DECRYPTION BY PASSWORD = 'password'` |  | `OpenMasterKeyStatement` |
| 🟢 Green | OPEN SYMMETRIC KEY | `OPEN SYMMETRIC KEY [key1] DECRYPTION BY CERTIFICATE c1` |  | `OpenSymmetricKeyStatement` |
| 🟢 Green | PRINT | `PRINT 3` |  | `PrintStatement` |
| 🟢 Green | RAISERROR | `RAISERROR (50009, 16, 10)` |  | `RaiseErrorStatement` |
| 🟢 Green | READTEXT | `READTEXT t1.c1 @ptrval 1 @size` |  | `ReadTextStatement` |
| 🟢 Green | RETURN | `RETURN` |  | `ReturnStatement` |
| 🟢 Green | REVERT | `REVERT` |  | `RevertStatement` |
| 🟢 Green | ROLLBACK TRANSACTION | `ROLLBACK` |  | `RollbackTransactionStatement` |
| 🟢 Green | SAVE TRANSACTION | `SAVE TRAN marker;` |  | `SaveTransactionStatement` |
| 🟢 Green | SELECT | `SELECT 1 AS Value;` | 🔴 Red for SELECT INTO a permanent table (`SELECT 1 AS Id INTO dbo.Items;`). 🟢 Green for SELECT INTO a #temp table (`SELECT 1 AS Id INTO #Items;`). 🔴 Red when advancing a sequence (`SELECT NEXT VALUE FOR dbo.Seq;`). | `SelectStatement` |
| 🟢 Green | SET (session option) | `SET NOCOUNT ON` |  | `PredicateSetStatement` |
| 🟢 Green | SET (session value) | `SET LOCK_TIMEOUT -1` |  | `SetCommandStatement` |
| 🟢 Green | SET @variable | `SET @a ^= 1` |  | `SetVariableStatement` |
| 🟢 Green | SET ERRLVL | `SET ERRLVL 1000` |  | `SetErrorLevelStatement` |
| 🟢 Green | SET IDENTITY_INSERT | `SET IDENTITY_INSERT t1 OFF` |  | `SetIdentityInsertStatement` |
| 🟢 Green | SET OFFSETS | `SET OFFSETS PARAM ON` |  | `SetOffsetsStatement` |
| 🟢 Green | SET ROWCOUNT | `SET ROWCOUNT 0;` |  | `SetRowCountStatement` |
| 🟢 Green | SET STATISTICS | `SET STATISTICS IO ON` |  | `SetStatisticsStatement` |
| 🟢 Green | SET TEXTSIZE | `SET TEXTSIZE 0` |  | `SetTextSizeStatement` |
| 🟢 Green | SET TRANSACTION ISOLATION LEVEL | `SET TRANSACTION ISOLATION LEVEL SNAPSHOT` |  | `SetTransactionIsolationLevelStatement` |
| 🟢 Green | SETUSER | `SETUSER` |  | `SetUserStatement` |
| 🟢 Green | THROW | `THROW` |  | `ThrowStatement` |
| 🟢 Green | WAITFOR | `WAITFOR DELAY '00:00:01';` |  | `WaitForStatement` |
| 🟢 Green | label: | `gogo:` |  | `LabelStatement` |
| 🟡 Yellow | ALTER TABLE … WITH (CLUSTER BY) | `ALTER TABLE Customers WITH (CLUSTER BY (CustomerID));` | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `AlterTableAddClusterByStatement` |
| 🟡 Yellow | BACKUP DATABASE | `BACKUP DATABASE d1 TO someDevice` |  | `BackupDatabaseStatement` |
| 🟡 Yellow | BACKUP LOG | `BACKUP LOG d1 TO someDevice` |  | `BackupTransactionLogStatement` |
| 🟡 Yellow | BEGIN TRANSACTION | `BEGIN TRAN;` | Unmatched transaction boundary in isolation. 🟢 Green for a balanced transaction (`BEGIN TRAN; COMMIT;`). | `BeginTransactionStatement` |
| 🟡 Yellow | CHECKPOINT | `CHECKPOINT` |  | `CheckpointStatement` |
| 🟡 Yellow | COMMIT TRANSACTION | `COMMIT;` | Unmatched transaction boundary in isolation. 🟢 Green for a balanced transaction (`BEGIN TRAN; COMMIT;`). | `CommitTransactionStatement` |
| 🟡 Yellow | DISK | `DISK INIT NAME='DEVICE1',PHYSNAME='c:\sql80\data\device1.dat',VDEVNO=1,SIZE=6144;` | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `DiskStatement` |
| 🟡 Yellow | EXEC | `EXEC dbo.CoverageProc;` | 🟢 Green for a built-in read-only system procedure (`EXEC sys.sp_help;`). 🟢 Green when this procedure is trusted in settings (`EXEC dbo.CoverageProc;`). 🟢 Green for literal dynamic SQL that reads (`EXEC(N'SELECT 1');`). 🔴 Red for literal dynamic SQL that writes (`EXEC(N'DELETE dbo.Items');`). 🔴 Red for a known writing system procedure (`EXEC sys.sp_configure N'maxdop', 1;`). | `ExecuteStatement` |
| 🟡 Yellow | RAISERROR (legacy) | `RAISERROR 50001 'legacy message';` | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `RaiseErrorLegacyStatement` |
| 🟡 Yellow | UPDATE STATISTICS | `UPDATE STATISTICS t1` |  | `UpdateStatisticsStatement` |
| 🟡 Yellow | USE | `USE master` |  | `UseStatement` |
| 🟡 Yellow | USE FEDERATION | `USE FEDERATION ROOT WITH RESET` |  | `UseFederationStatement` |
| 🔴 Red | ADD SENSITIVITY CLASSIFICATION | `ADD SENSITIVITY CLASSIFICATION TO T1.C1, T1.C2, DBO.T1.C3 WITH (LABEL = 'Highly Confidential', INFORMATION_TYPE = 'Financial')` |  | `AddSensitivityClassificationStatement` |
| 🔴 Red | ADD SIGNATURE | `ADD SIGNATURE TO HumanResources.uspUpdateEmployeeLogin BY CERTIFICATE HumanResourcesDP` |  | `AddSignatureStatement` |
| 🔴 Red | ALTER APPLICATION ROLE | `ALTER APPLICATION ROLE CoverageApp WITH NAME = CoverageAppRenamed;` |  | `AlterApplicationRoleStatement` |
| 🔴 Red | ALTER ASSEMBLY | `ALTER ASSEMBLY a1 FROM 'zzz'` |  | `AlterAssemblyStatement` |
| 🔴 Red | ALTER ASYMMETRIC KEY | `ALTER ASYMMETRIC KEY a1 ATTESTED BY 'zzz'` |  | `AlterAsymmetricKeyStatement` |
| 🔴 Red | ALTER AUTHORIZATION | `ALTER AUTHORIZATION ON USER::Parts.Sprockets TO [c1]` |  | `AlterAuthorizationStatement` |
| 🔴 Red | ALTER AVAILABILITY GROUP | `ALTER AVAILABILITY GROUP group1 JOIN` |  | `AlterAvailabilityGroupStatement` |
| 🔴 Red | ALTER BROKER PRIORITY | `ALTER BROKER PRIORITY CoveragePriority FOR CONVERSATION SET (PRIORITY_LEVEL = 7);` |  | `AlterBrokerPriorityStatement` |
| 🔴 Red | ALTER CERTIFICATE | `ALTER CERTIFICATE c1 ATTESTED BY 'zzz'` |  | `AlterCertificateStatement` |
| 🔴 Red | ALTER COLUMN ENCRYPTION KEY | `ALTER COLUMN ENCRYPTION KEY OneValueCEK DROP VALUE ( COLUMN_MASTER_KEY = CMK2 )` |  | `AlterColumnEncryptionKeyStatement` |
| 🔴 Red | ALTER CREDENTIAL | `ALTER CREDENTIAL c1 WITH IDENTITY = 'id1'` |  | `AlterCredentialStatement` |
| 🔴 Red | ALTER CRYPTOGRAPHIC PROVIDER | `ALTER CRYPTOGRAPHIC PROVIDER cp1 ENABLE` |  | `AlterCryptographicProviderStatement` |
| 🔴 Red | ALTER DATABASE ADD FILE | `ALTER DATABASE [test] ADD FILE (NAME = 'MyFile') TO FILEGROUP [MY FILEGROUP]` |  | `AlterDatabaseAddFileStatement` |
| 🔴 Red | ALTER DATABASE ADD FILEGROUP | `ALTER DATABASE d1 ADD FILEGROUP fg1` |  | `AlterDatabaseAddFileGroupStatement` |
| 🔴 Red | ALTER DATABASE AUDIT SPECIFICATION | `ALTER DATABASE AUDIT SPECIFICATION AuditSpec1` |  | `AlterDatabaseAuditSpecificationStatement` |
| 🔴 Red | ALTER DATABASE COLLATE | `ALTER DATABASE db1 COLLATE Estonian_CS_AS` |  | `AlterDatabaseCollateStatement` |
| 🔴 Red | ALTER DATABASE ENCRYPTION KEY | `ALTER DATABASE ENCRYPTION KEY REGENERATE WITH ALGORITHM=AES_256;` |  | `AlterDatabaseEncryptionKeyStatement` |
| 🔴 Red | ALTER DATABASE MODIFY FILE | `ALTER DATABASE db1 MODIFY FILE (NAME = n1, NEWNAME = n2)` |  | `AlterDatabaseModifyFileStatement` |
| 🔴 Red | ALTER DATABASE MODIFY FILEGROUP | `ALTER DATABASE d1 MODIFY FILEGROUP fg1 DEFAULT` |  | `AlterDatabaseModifyFileGroupStatement` |
| 🔴 Red | ALTER DATABASE MODIFY NAME | `ALTER DATABASE db1 MODIFY NAME = db2` |  | `AlterDatabaseModifyNameStatement` |
| 🔴 Red | ALTER DATABASE REBUILD LOG | `ALTER DATABASE db1 REBUILD LOG` |  | `AlterDatabaseRebuildLogStatement` |
| 🔴 Red | ALTER DATABASE REMOVE FILE | `ALTER DATABASE db1 REMOVE FILE myfile` |  | `AlterDatabaseRemoveFileStatement` |
| 🔴 Red | ALTER DATABASE REMOVE FILEGROUP | `ALTER DATABASE d1 REMOVE FILEGROUP fg1` |  | `AlterDatabaseRemoveFileGroupStatement` |
| 🔴 Red | ALTER DATABASE SCOPED CONFIGURATION CLEAR | `ALTER DATABASE SCOPED CONFIGURATION CLEAR PROCEDURE_CACHE` |  | `AlterDatabaseScopedConfigurationClearStatement` |
| 🔴 Red | ALTER DATABASE SCOPED CONFIGURATION SET | `ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 1` |  | `AlterDatabaseScopedConfigurationSetStatement` |
| 🔴 Red | ALTER DATABASE SET | `ALTER DATABASE d1 SET HADR OFF` |  | `AlterDatabaseSetStatement` |
| 🔴 Red | ALTER DATABASE … PERFORM_CUTOVER | `ALTER DATABASE db PERFORM_CUTOVER` |  | `AlterDatabasePerformCutoverStatement` |
| 🔴 Red | ALTER ENDPOINT | `ALTER ENDPOINT CoverageEndpoint STATE=STARTED;` |  | `AlterEndpointStatement` |
| 🔴 Red | ALTER EVENT SESSION | `ALTER EVENT SESSION es1 ON SERVER STATE = STOP` |  | `AlterEventSessionStatement` |
| 🔴 Red | ALTER EXTERNAL DATA SOURCE | `ALTER EXTERNAL DATA SOURCE eds3 SET CREDENTIAL = cred1` |  | `AlterExternalDataSourceStatement` |
| 🔴 Red | ALTER EXTERNAL LANGUAGE | `ALTER EXTERNAL LANGUAGE language6 REMOVE PLATFORM LINUX` |  | `AlterExternalLanguageStatement` |
| 🔴 Red | ALTER EXTERNAL LIBRARY | `ALTER EXTERNAL LIBRARY xts SET (CONTENT = 'c:\Path\xts.zip') WITH (LANGUAGE = 'R')` |  | `AlterExternalLibraryStatement` |
| 🔴 Red | ALTER EXTERNAL MODEL | `ALTER EXTERNAL MODEL abc SET ( MODEL = 'new_model' )` |  | `AlterExternalModelStatement` |
| 🔴 Red | ALTER EXTERNAL RESOURCE POOL | `ALTER EXTERNAL RESOURCE POOL CoverageExternalPool WITH(MAX_CPU_PERCENT=80);` |  | `AlterExternalResourcePoolStatement` |
| 🔴 Red | ALTER FEDERATION | `ALTER FEDERATION f1 SPLIT AT (k1 = 10)` |  | `AlterFederationStatement` |
| 🔴 Red | ALTER FULLTEXT CATALOG | `ALTER FULLTEXT CATALOG c1 REBUILD` |  | `AlterFullTextCatalogStatement` |
| 🔴 Red | ALTER FULLTEXT INDEX | `ALTER FULLTEXT INDEX ON t1 DISABLE` |  | `AlterFullTextIndexStatement` |
| 🔴 Red | ALTER FULLTEXT STOPLIST | `ALTER FULLTEXT STOPLIST fs1 DROP ALL;` |  | `AlterFullTextStopListStatement` |
| 🔴 Red | ALTER FUNCTION | `ALTER FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 2; END;` |  | `AlterFunctionStatement` |
| 🔴 Red | ALTER FUNCTION … AS EXTERNAL FUNCTION | `ALTER FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `AlterExternalFunctionStatement` |
| 🔴 Red | ALTER INDEX | `ALTER INDEX ALL ON t1 DISABLE` | 🟢 Green on a #temp table (`ALTER INDEX ALL ON #Items DISABLE`). | `AlterIndexStatement` |
| 🔴 Red | ALTER LOGIN … ADD / DROP CREDENTIAL | `ALTER LOGIN l1 ADD CREDENTIAL cr1` |  | `AlterLoginAddDropCredentialStatement` |
| 🔴 Red | ALTER LOGIN … ENABLE / DISABLE | `ALTER LOGIN l1 ENABLE` |  | `AlterLoginEnableDisableStatement` |
| 🔴 Red | ALTER LOGIN … WITH | `ALTER LOGIN l1 WITH CREDENTIAL = cr1` |  | `AlterLoginOptionsStatement` |
| 🔴 Red | ALTER MASTER KEY | `ALTER MASTER KEY ADD ENCRYPTION BY PASSWORD = 'pw1'` |  | `AlterMasterKeyStatement` |
| 🔴 Red | ALTER MESSAGE TYPE | `ALTER MESSAGE TYPE m1 VALIDATION = EMPTY` |  | `AlterMessageTypeStatement` |
| 🔴 Red | ALTER PARTITION FUNCTION | `ALTER PARTITION FUNCTION pfItems() MERGE RANGE (100);` |  | `AlterPartitionFunctionStatement` |
| 🔴 Red | ALTER PARTITION SCHEME | `ALTER PARTITION SCHEME s1 NEXT USED` |  | `AlterPartitionSchemeStatement` |
| 🔴 Red | ALTER PROCEDURE | `ALTER PROCEDURE dbo.P AS SELECT 2;` | 🟢 Green for a #temp procedure (`ALTER PROCEDURE #TempProc AS SELECT 2;`). | `AlterProcedureStatement` |
| 🔴 Red | ALTER QUEUE | `ALTER QUEUE dbo.q1 WITH ACTIVATION (DROP)` |  | `AlterQueueStatement` |
| 🔴 Red | ALTER REMOTE SERVICE BINDING | `ALTER REMOTE SERVICE BINDING b1 WITH USER = u1` |  | `AlterRemoteServiceBindingStatement` |
| 🔴 Red | ALTER RESOURCE GOVERNOR | `ALTER RESOURCE GOVERNOR DISABLE` |  | `AlterResourceGovernorStatement` |
| 🔴 Red | ALTER RESOURCE POOL | `ALTER RESOURCE POOL CoveragePool WITH(MAX_CPU_PERCENT=80);` |  | `AlterResourcePoolStatement` |
| 🔴 Red | ALTER ROLE | `ALTER ROLE r1 WITH NAME = newName` |  | `AlterRoleStatement` |
| 🔴 Red | ALTER ROUTE | `ALTER ROUTE CoverageRoute WITH SERVICE_NAME = N'CoverageServiceTwo', ADDRESS = N'LOCAL';` |  | `AlterRouteStatement` |
| 🔴 Red | ALTER SCHEMA | `ALTER SCHEMA sc1 TRANSFER a1.b1` |  | `AlterSchemaStatement` |
| 🔴 Red | ALTER SEARCH PROPERTY LIST | `ALTER SEARCH PROPERTY LIST foo DROP 'bax';` |  | `AlterSearchPropertyListStatement` |
| 🔴 Red | ALTER SECURITY POLICY | `ALTER SECURITY POLICY sec1 WITH (STATE = ON)` |  | `AlterSecurityPolicyStatement` |
| 🔴 Red | ALTER SEQUENCE | `ALTER SEQUENCE S1 NO CACHE` |  | `AlterSequenceStatement` |
| 🔴 Red | ALTER SERVER AUDIT | `ALTER SERVER AUDIT a1 REMOVE WHERE` |  | `AlterServerAuditStatement` |
| 🔴 Red | ALTER SERVER AUDIT SPECIFICATION | `ALTER SERVER AUDIT SPECIFICATION AuditSpec1` |  | `AlterServerAuditSpecificationStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION | `ALTER SERVER CONFIGURATION SET PROCESS AFFINITY CPU=AUTO;` |  | `AlterServerConfigurationStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET BUFFER POOL EXTENSION | `ALTER SERVER CONFIGURATION SET BUFFER POOL EXTENSION OFF` |  | `AlterServerConfigurationSetBufferPoolExtensionStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET DIAGNOSTICS LOG | `ALTER SERVER CONFIGURATION SET DIAGNOSTICS LOG ON` |  | `AlterServerConfigurationSetDiagnosticsLogStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET EXTERNAL AUTHENTICATION | `ALTER SERVER CONFIGURATION SET EXTERNAL AUTHENTICATION OFF` |  | `AlterServerConfigurationSetExternalAuthenticationStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET FAILOVER CLUSTER PROPERTY | `ALTER SERVER CONFIGURATION SET FAILOVER CLUSTER PROPERTY VerboseLogging = 2` |  | `AlterServerConfigurationSetFailoverClusterPropertyStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET HADR CLUSTER | `ALTER SERVER CONFIGURATION SET HADR CLUSTER CONTEXT = LOCAL` |  | `AlterServerConfigurationSetHadrClusterStatement` |
| 🔴 Red | ALTER SERVER CONFIGURATION SET SOFTNUMA | `ALTER SERVER CONFIGURATION SET SOFTNUMA ON` |  | `AlterServerConfigurationSetSoftNumaStatement` |
| 🔴 Red | ALTER SERVER ROLE | `ALTER SERVER ROLE r1 WITH NAME = newName` |  | `AlterServerRoleStatement` |
| 🔴 Red | ALTER SERVICE | `ALTER SERVICE s1 ON QUEUE dbo.q1` |  | `AlterServiceStatement` |
| 🔴 Red | ALTER SERVICE MASTER KEY | `ALTER SERVICE MASTER KEY REGENERATE` |  | `AlterServiceMasterKeyStatement` |
| 🔴 Red | ALTER SYMMETRIC KEY | `ALTER SYMMETRIC KEY k1 DROP ENCRYPTION BY PASSWORD = 'p1'` |  | `AlterSymmetricKeyStatement` |
| 🔴 Red | ALTER TABLE … ADD | `ALTER TABLE T ADD jsonCol JSON` | 🟢 Green on a #temp table (`ALTER TABLE #Items ADD jsonCol JSON`). | `AlterTableAddTableElementStatement` |
| 🔴 Red | ALTER TABLE … ALTER COLUMN | `ALTER TABLE t1 ALTER COLUMN c1 INT` | 🟢 Green on a #temp table (`ALTER TABLE #Items ALTER COLUMN c1 INT`). | `AlterTableAlterColumnStatement` |
| 🔴 Red | ALTER TABLE … ALTER INDEX | `ALTER TABLE t1 ALTER INDEX i1 REBUILD WITH (BUCKET_COUNT = 1)` | 🟢 Green on a #temp table (`ALTER TABLE #Items ALTER INDEX i1 REBUILD WITH (BUCKET_COUNT = 1)`). | `AlterTableAlterIndexStatement` |
| 🔴 Red | ALTER TABLE … CHECK / NOCHECK CONSTRAINT | `ALTER TABLE t1 NOCHECK CONSTRAINT ALL` | 🟢 Green on a #temp table (`ALTER TABLE #Items NOCHECK CONSTRAINT ALL`). | `AlterTableConstraintModificationStatement` |
| 🔴 Red | ALTER TABLE … DROP | `ALTER TABLE t1 DROP INDEX i1` | 🟢 Green on a #temp table (`ALTER TABLE #Items DROP INDEX i1`). | `AlterTableDropTableElementStatement` |
| 🔴 Red | ALTER TABLE … ENABLE / DISABLE CHANGE_TRACKING | `ALTER TABLE t1 ENABLE CHANGE_TRACKING` | 🟢 Green on a #temp table (`ALTER TABLE #Items ENABLE CHANGE_TRACKING`). | `AlterTableChangeTrackingModificationStatement` |
| 🔴 Red | ALTER TABLE … ENABLE / DISABLE FILETABLE_NAMESPACE | `ALTER TABLE t1 ENABLE FILETABLE_NAMESPACE` | 🟢 Green on a #temp table (`ALTER TABLE #Items ENABLE FILETABLE_NAMESPACE`). | `AlterTableFileTableNamespaceStatement` |
| 🔴 Red | ALTER TABLE … ENABLE / DISABLE TRIGGER | `ALTER TABLE t1 ENABLE TRIGGER ALL` | 🟢 Green on a #temp table (`ALTER TABLE #Items ENABLE TRIGGER ALL`). | `AlterTableTriggerModificationStatement` |
| 🔴 Red | ALTER TABLE … REBUILD | `ALTER TABLE t1 REBUILD` | 🟢 Green on a #temp table (`ALTER TABLE #Items REBUILD`). | `AlterTableRebuildStatement` |
| 🔴 Red | ALTER TABLE … SET | `ALTER TABLE T SET (SYSTEM_VERSIONING = ON)` | 🟢 Green on a #temp table (`ALTER TABLE #Items SET (SYSTEM_VERSIONING = ON)`). | `AlterTableSetStatement` |
| 🔴 Red | ALTER TABLE … SPLIT / MERGE RANGE | `ALTER TABLE T MERGE RANGE (10)` | 🟢 Green on a #temp table (`ALTER TABLE #Items MERGE RANGE (10)`). | `AlterTableAlterPartitionStatement` |
| 🔴 Red | ALTER TABLE … SWITCH | `ALTER TABLE t1 SWITCH TO t2` | 🟢 Green on a #temp table (`ALTER TABLE #Items SWITCH TO t2`). The analyzer checks the source target here, even with a permanent destination; this is not a safety guarantee. | `AlterTableSwitchStatement` |
| 🔴 Red | ALTER TRIGGER | `ALTER TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'new';` |  | `AlterTriggerStatement` |
| 🔴 Red | ALTER USER | `ALTER USER u1 WITH NAME = newName` |  | `AlterUserStatement` |
| 🔴 Red | ALTER VIEW | `ALTER MATERIALIZED VIEW View1 DISABLE` |  | `AlterViewStatement` |
| 🔴 Red | ALTER WORKLOAD GROUP | `ALTER WORKLOAD GROUP g USING EXTERNAL p_ext` |  | `AlterWorkloadGroupStatement` |
| 🔴 Red | ALTER XML SCHEMA COLLECTION | `ALTER XML SCHEMA COLLECTION MyCollection ADD 'Some xml text'` |  | `AlterXmlSchemaCollectionStatement` |
| 🔴 Red | BACKUP CERTIFICATE | `BACKUP CERTIFICATE c1 TO FILE = 'f1'` |  | `BackupCertificateStatement` |
| 🔴 Red | BACKUP MASTER KEY | `BACKUP MASTER KEY TO FILE = 'f1' ENCRYPTION BY PASSWORD = 'p1'` |  | `BackupMasterKeyStatement` |
| 🔴 Red | BACKUP SERVICE MASTER KEY | `BACKUP SERVICE MASTER KEY TO FILE = 'f1' ENCRYPTION BY PASSWORD = 'pw1'` |  | `BackupServiceMasterKeyStatement` |
| 🔴 Red | BEGIN CONVERSATION TIMER | `BEGIN CONVERSATION TIMER (@ch) TIMEOUT = 10` |  | `BeginConversationTimerStatement` |
| 🔴 Red | BEGIN DIALOG | `BEGIN DIALOG @dh FROM SERVICE s1 TO SERVICE @s2` |  | `BeginDialogStatement` |
| 🔴 Red | BULK INSERT | `BULK INSERT dbo.Items FROM 'C:\data\items.csv';` | 🟢 Green on a #temp table (`BULK INSERT #Items FROM 'C:\data\items.csv';`). 🟢 Green with the target allow-listed in settings (`BULK INSERT dbo.Items FROM 'C:\data\items.csv';`). | `BulkInsertStatement` |
| 🔴 Red | COPY INTO | `COPY INTO dbo.[FactOnlineSalesv1] FROM 'https://unsecureaccount.blob.core.windows.net/customerdatasets/linitem.csv'` |  | `CopyStatement` |
| 🔴 Red | CREATE AGGREGATE | `CREATE AGGREGATE a1(@input XML( zzz)) RETURNS XML( zzz) EXTERNAL NAME ag1` |  | `CreateAggregateStatement` |
| 🔴 Red | CREATE APPLICATION ROLE | `CREATE APPLICATION ROLE weekly_receipts WITH PASSWORD = '987'` |  | `CreateApplicationRoleStatement` |
| 🔴 Red | CREATE ASSEMBLY | `CREATE ASSEMBLY HelloWorld AUTHORIZATION dbo FROM @FullPath` |  | `CreateAssemblyStatement` |
| 🔴 Red | CREATE ASYMMETRIC KEY | `CREATE ASYMMETRIC KEY k1 WITH ALGORITHM = DES` |  | `CreateAsymmetricKeyStatement` |
| 🔴 Red | CREATE AVAILABILITY GROUP | `CREATE AVAILABILITY GROUP group1 WITH (REQUIRED_COPIES_TO_COMMIT = 10) FOR DATABASE db1, db2 REPLICA ON 'server1' WITH (AVAILABILITY_MODE = SYNCHRONOUS_COMMIT, FAILOVER_MODE = AUTOMATIC, ENDPOINT_URL = 'TCP://1234:80', SECONDARY_ROLE (ALLOW_CONNECTIONS = READ_ONLY), SESSION_TIMEOUT = 10, APPLY_DELAY = 1), 'server2' WITH (FAILOVER_MODE = MANUAL, APPLY_DELAY = 1, ENDPOINT_URL = 'TCP://1234:80', PRIMARY_ROLE (ALLOW_CONNECTIONS = ALL), AVAILABILITY_MODE = ASYNCHRONOUS_COMMIT, SESSION_TIMEOUT = 10)` |  | `CreateAvailabilityGroupStatement` |
| 🔴 Red | CREATE BROKER PRIORITY | `CREATE BROKER PRIORITY bp1 FOR CONVERSATION` |  | `CreateBrokerPriorityStatement` |
| 🔴 Red | CREATE CERTIFICATE | `CREATE CERTIFICATE s4 FROM FILE = 'f1'` |  | `CreateCertificateStatement` |
| 🔴 Red | CREATE COLUMN ENCRYPTION KEY | `CREATE COLUMN ENCRYPTION KEY CoverageCek WITH VALUES(COLUMN_MASTER_KEY=CoverageCmk,ALGORITHM='RSA_OAEP',ENCRYPTED_VALUE=0x0102030405);` |  | `CreateColumnEncryptionKeyStatement` |
| 🔴 Red | CREATE COLUMN MASTER KEY | `CREATE COLUMN MASTER KEY CoverageCmk WITH(KEY_STORE_PROVIDER_NAME=N'MSSQL_CERTIFICATE_STORE',KEY_PATH=N'CurrentUser/My/fixture');` |  | `CreateColumnMasterKeyStatement` |
| 🔴 Red | CREATE COLUMNSTORE INDEX | `CREATE COLUMNSTORE INDEX cindx ON t(c1)` | 🟢 Green on a #temp table (`CREATE COLUMNSTORE INDEX cindx ON #Items(c1)`). | `CreateColumnStoreIndexStatement` |
| 🔴 Red | CREATE CONTRACT | `CREATE CONTRACT c1 (m1 SENT BY INITIATOR)` |  | `CreateContractStatement` |
| 🔴 Red | CREATE CREDENTIAL | `CREATE CREDENTIAL c1 WITH IDENTITY = 'id1'` |  | `CreateCredentialStatement` |
| 🔴 Red | CREATE CRYPTOGRAPHIC PROVIDER | `CREATE CRYPTOGRAPHIC PROVIDER cp1 FROM FILE = 'c:\v1.dll'` |  | `CreateCryptographicProviderStatement` |
| 🔴 Red | CREATE DATABASE | `CREATE DATABASE d1` |  | `CreateDatabaseStatement` |
| 🔴 Red | CREATE DATABASE AUDIT SPECIFICATION | `CREATE DATABASE AUDIT SPECIFICATION AuditSpec1 FOR SERVER AUDIT a1` |  | `CreateDatabaseAuditSpecificationStatement` |
| 🔴 Red | CREATE DATABASE ENCRYPTION KEY | `CREATE DATABASE ENCRYPTION KEY WITH ALGORITHM = AES_128 ENCRYPTION BY SERVER CERTIFICATE enc1` |  | `CreateDatabaseEncryptionKeyStatement` |
| 🔴 Red | CREATE DEFAULT | `CREATE DEFAULT dbo.r1 AS (-10)` |  | `CreateDefaultStatement` |
| 🔴 Red | CREATE ENDPOINT | `CREATE ENDPOINT e1 AUTHORIZATION zzz AS TCP ( LISTENER_PORT = 4022 ) FOR TSQL()` |  | `CreateEndpointStatement` |
| 🔴 Red | CREATE EVENT NOTIFICATION | `CREATE EVENT NOTIFICATION log_ddl1 ON SERVER FOR OBJECT_CREATED TO SERVICE 'NotifyService', '8140'` |  | `CreateEventNotificationStatement` |
| 🔴 Red | CREATE EVENT SESSION | `CREATE EVENT SESSION es1 ON SERVER ADD EVENT b.c` |  | `CreateEventSessionStatement` |
| 🔴 Red | CREATE EXTERNAL DATA SOURCE | `CREATE EXTERNAL DATA SOURCE eds1 WITH ( LOCATION = 'protocol://ip_address:port' )` |  | `CreateExternalDataSourceStatement` |
| 🔴 Red | CREATE EXTERNAL FILE FORMAT | `CREATE EXTERNAL FILE FORMAT eff3 WITH ( FORMAT_TYPE = ORC )` |  | `CreateExternalFileFormatStatement` |
| 🔴 Red | CREATE EXTERNAL LANGUAGE | `CREATE EXTERNAL LANGUAGE language4 FROM (CONTENT = '\\machine-name\dir1\dir2\windowsextension.zip', FILE_NAME = 'WindowsExtension.dll', PLATFORM = WINDOWS)` |  | `CreateExternalLanguageStatement` |
| 🔴 Red | CREATE EXTERNAL LIBRARY | `CREATE EXTERNAL LIBRARY xts FROM (CONTENT = 'c:\Path\xts.zip') WITH (LANGUAGE = 'R')` |  | `CreateExternalLibraryStatement` |
| 🔴 Red | CREATE EXTERNAL MODEL | `CREATE EXTERNAL MODEL simple_model WITH ( LOCATION = '/models/simple', API_FORMAT = 'OpenAI', MODEL_TYPE = EMBEDDINGS, MODEL = 'gpt-3.5-turbo' )` |  | `CreateExternalModelStatement` |
| 🔴 Red | CREATE EXTERNAL RESOURCE POOL | `CREATE EXTERNAL RESOURCE POOL p` |  | `CreateExternalResourcePoolStatement` |
| 🔴 Red | CREATE EXTERNAL STREAM | `CREATE EXTERNAL STREAM CoverageStream WITH(LOCATION='fixture');` |  | `CreateExternalStreamStatement` |
| 🔴 Red | CREATE EXTERNAL TABLE | `CREATE EXTERNAL TABLE dbo.CoverageExternal(Id int) WITH(DATA_SOURCE=CoverageSource,LOCATION='fixture');` |  | `CreateExternalTableStatement` |
| 🔴 Red | CREATE FEDERATION | `CREATE FEDERATION f1 (k1 BIGINT RANGE)` |  | `CreateFederationStatement` |
| 🔴 Red | CREATE FULLTEXT CATALOG | `CREATE FULLTEXT CATALOG c1` |  | `CreateFullTextCatalogStatement` |
| 🔴 Red | CREATE FULLTEXT INDEX | `CREATE FULLTEXT INDEX ON dbo.Items (Description) KEY INDEX PK_Items;` |  | `CreateFullTextIndexStatement` |
| 🔴 Red | CREATE FULLTEXT STOPLIST | `CREATE FULLTEXT STOPLIST fs1;` |  | `CreateFullTextStopListStatement` |
| 🔴 Red | CREATE FUNCTION | `CREATE FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 1; END;` |  | `CreateFunctionStatement` |
| 🔴 Red | CREATE FUNCTION … AS EXTERNAL FUNCTION | `CREATE FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `CreateExternalFunctionStatement` |
| 🔴 Red | CREATE INDEX | `CREATE INDEX ind1 ON t1(c1)` | 🟢 Green on a #temp table (`CREATE INDEX ind1 ON #Items(c1)`). | `CreateIndexStatement` |
| 🔴 Red | CREATE JSON INDEX | `CREATE JSON INDEX IX_JSON_Basic ON dbo.Users (JsonData)` |  | `CreateJsonIndexStatement` |
| 🔴 Red | CREATE LOGIN | `CREATE LOGIN [l1] FROM WINDOWS` |  | `CreateLoginStatement` |
| 🔴 Red | CREATE MASTER KEY | `CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Example-only-password!';` |  | `CreateMasterKeyStatement` |
| 🔴 Red | CREATE MESSAGE TYPE | `CREATE MESSAGE TYPE m1 VALIDATION = EMPTY` |  | `CreateMessageTypeStatement` |
| 🔴 Red | CREATE OR ALTER FUNCTION | `CREATE OR ALTER FUNCTION dbo.F() RETURNS int AS BEGIN RETURN 2; END;` |  | `CreateOrAlterFunctionStatement` |
| 🔴 Red | CREATE OR ALTER FUNCTION … AS EXTERNAL FUNCTION | `CREATE OR ALTER FUNCTION dbo.MyExternalFn AS EXTERNAL FUNCTION mySet.myFn;` | Alternate-dialect fixture (FabricDW); colour is the default analyzer response, including parse errors. | `CreateOrAlterExternalFunctionStatement` |
| 🔴 Red | CREATE OR ALTER PROCEDURE | `CREATE OR ALTER PROCEDURE dbo.P AS SELECT 2;` | 🟢 Green for a #temp procedure (`CREATE OR ALTER PROCEDURE #TempProc AS SELECT 2;`). | `CreateOrAlterProcedureStatement` |
| 🔴 Red | CREATE OR ALTER TRIGGER | `CREATE OR ALTER TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'new';` |  | `CreateOrAlterTriggerStatement` |
| 🔴 Red | CREATE OR ALTER VIEW | `CREATE OR ALTER VIEW dbo.V AS SELECT Id FROM dbo.Items;` |  | `CreateOrAlterViewStatement` |
| 🔴 Red | CREATE PARTITION FUNCTION | `CREATE PARTITION FUNCTION PF(int) AS RANGE LEFT FOR VALUES(10);` |  | `CreatePartitionFunctionStatement` |
| 🔴 Red | CREATE PARTITION SCHEME | `CREATE PARTITION SCHEME PS AS PARTITION PF ALL TO ([PRIMARY]);` |  | `CreatePartitionSchemeStatement` |
| 🔴 Red | CREATE PROCEDURE | `CREATE PROCEDURE p1 AS RETURN 0;` | 🟢 Green for a #temp procedure (`CREATE PROCEDURE #TempProc AS RETURN 0;`). | `CreateProcedureStatement` |
| 🔴 Red | CREATE QUEUE | `CREATE QUEUE dbo.q1` |  | `CreateQueueStatement` |
| 🔴 Red | CREATE REMOTE SERVICE BINDING | `CREATE REMOTE SERVICE BINDING CoverageBinding TO SERVICE N'CoverageService' WITH USER = CoverageBrokerUser, ANONYMOUS = ON;` |  | `CreateRemoteServiceBindingStatement` |
| 🔴 Red | CREATE RESOURCE POOL | `CREATE RESOURCE POOL CoveragePool;` |  | `CreateResourcePoolStatement` |
| 🔴 Red | CREATE ROLE | `CREATE ROLE r1` |  | `CreateRoleStatement` |
| 🔴 Red | CREATE ROUTE | `CREATE ROUTE r1 WITH SERVICE_NAME = 's1'` |  | `CreateRouteStatement` |
| 🔴 Red | CREATE RULE | `CREATE RULE dbo.r1 AS @a1 > 10` |  | `CreateRuleStatement` |
| 🔴 Red | CREATE SCHEMA | `CREATE SCHEMA [dev]` |  | `CreateSchemaStatement` |
| 🔴 Red | CREATE SEARCH PROPERTY LIST | `CREATE SEARCH PROPERTY LIST new_list;` |  | `CreateSearchPropertyListStatement` |
| 🔴 Red | CREATE SECURITY POLICY | `CREATE SECURITY POLICY dbo.sec1` |  | `CreateSecurityPolicyStatement` |
| 🔴 Red | CREATE SELECTIVE XML INDEX | `CREATE SELECTIVE XML INDEX sxi1 ON t1(c1) FOR( path1 = '/a/b/c' )` |  | `CreateSelectiveXmlIndexStatement` |
| 🔴 Red | CREATE SEMANTIC INDEX | `CREATE SEMANTIC INDEX SI_Basic ON dbo.Documents (content) WITH (EXTERNAL_MODEL = MyModel)` |  | `CreateSemanticIndexStatement` |
| 🔴 Red | CREATE SEQUENCE | `CREATE SEQUENCE S1 NO CACHE` |  | `CreateSequenceStatement` |
| 🔴 Red | CREATE SERVER AUDIT | `CREATE SERVER AUDIT a1 TO EXTERNAL_MONITOR` |  | `CreateServerAuditStatement` |
| 🔴 Red | CREATE SERVER AUDIT SPECIFICATION | `CREATE SERVER AUDIT SPECIFICATION AuditSpec1 FOR SERVER AUDIT a1` |  | `CreateServerAuditSpecificationStatement` |
| 🔴 Red | CREATE SERVER ROLE | `CREATE SERVER ROLE r1` |  | `CreateServerRoleStatement` |
| 🔴 Red | CREATE SERVICE | `CREATE SERVICE s1 ON QUEUE q1 (c1)` |  | `CreateServiceStatement` |
| 🔴 Red | CREATE SPATIAL INDEX | `CREATE SPATIAL INDEX sp1 ON a.c (d) ON ab` |  | `CreateSpatialIndexStatement` |
| 🔴 Red | CREATE STATISTICS | `CREATE STATISTICS st1 ON t1(c1)` | 🟢 Green on a #temp table (`CREATE STATISTICS st1 ON #Items(c1)`). | `CreateStatisticsStatement` |
| 🔴 Red | CREATE SYMMETRIC KEY | `CREATE SYMMETRIC KEY k1 FROM PROVIDER p1` |  | `CreateSymmetricKeyStatement` |
| 🔴 Red | CREATE SYNONYM | `CREATE SYNONYM mysyn FOR t1` |  | `CreateSynonymStatement` |
| 🔴 Red | CREATE TABLE | `CREATE TABLE dbo.Items (Id int);` | 🟢 Green on a #temp table (`CREATE TABLE #Items (Id int);`). | `CreateTableStatement` |
| 🔴 Red | CREATE TABLE … INDEX | `CREATE TABLE dbo.CoverageInline(Id int, INDEX CoverageInlineIndex(Id));` | Inline INDEX inside CREATE TABLE, not standalone SQL; the enclosing table is analyzed. 🟢 Green inside a #temp table definition (`CREATE TABLE #Items (Id int, INDEX IX_Items (Id));`). | `IndexDefinition` |
| 🔴 Red | CREATE TRIGGER | `CREATE TRIGGER dbo.T ON dbo.Items AFTER UPDATE AS PRINT N'created';` |  | `CreateTriggerStatement` |
| 🔴 Red | CREATE TYPE … AS TABLE | `CREATE TYPE dbo.ItemList AS TABLE (Id int);` |  | `CreateTypeTableStatement` |
| 🔴 Red | CREATE TYPE … EXTERNAL NAME | `CREATE TYPE SSN EXTERNAL NAME [SomeAssembly]` |  | `CreateTypeUdtStatement` |
| 🔴 Red | CREATE TYPE … FROM | `CREATE TYPE [schema].SSN FROM INT NULL` |  | `CreateTypeUddtStatement` |
| 🔴 Red | CREATE USER | `CREATE USER u1` |  | `CreateUserStatement` |
| 🔴 Red | CREATE VECTOR INDEX | `CREATE VECTOR INDEX IX_Vector_Basic ON dbo.Documents (VectorData)` |  | `CreateVectorIndexStatement` |
| 🔴 Red | CREATE VIEW | `CREATE VIEW v1 AS SELECT * FROM t1` |  | `CreateViewStatement` |
| 🔴 Red | CREATE WORKLOAD CLASSIFIER | `CREATE WORKLOAD CLASSIFIER wcAllOptions WITH ( WORKLOAD_GROUP = 'wgDefaultParams', MEMBERNAME = 'ELTRole', WLM_CONTEXT = 'dim_load', START_TIME = '22:00', END_TIME = '02:00', WLM_LABEL = 'label', IMPORTANCE = HIGH )` |  | `CreateWorkloadClassifierStatement` |
| 🔴 Red | CREATE WORKLOAD GROUP | `CREATE WORKLOAD GROUP CoverageGroup;` |  | `CreateWorkloadGroupStatement` |
| 🔴 Red | CREATE XML INDEX | `CREATE PRIMARY XML INDEX c1 ON t1(c1)` |  | `CreateXmlIndexStatement` |
| 🔴 Red | CREATE XML SCHEMA COLLECTION | `CREATE XML SCHEMA COLLECTION MyCollection AS 'Some xml text'` |  | `CreateXmlSchemaCollectionStatement` |
| 🔴 Red | DELETE | `DELETE FROM dbo.Items WHERE Id = 1;` | 🟢 Green on a #temp table (`DELETE FROM #Items WHERE Id = 1;`). 🟢 Green on a @table variable (`DELETE FROM @Items WHERE Id = 1;`). 🟢 Green with the target allow-listed in settings (`DELETE FROM dbo.Items WHERE Id = 1;`). | `DeleteStatement` |
| 🔴 Red | DENY | `DENY all TO PUBLIC CASCADE` |  | `DenyStatement` |
| 🔴 Red | DENY | `DENY SELECT ON dbo.Coverage TO public;` | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `DenyStatement80` |
| 🔴 Red | DROP AGGREGATE | `DROP AGGREGATE aggX` |  | `DropAggregateStatement` |
| 🔴 Red | DROP APPLICATION ROLE | `DROP APPLICATION ROLE a1` |  | `DropApplicationRoleStatement` |
| 🔴 Red | DROP ASSEMBLY | `DROP ASSEMBLY a` |  | `DropAssemblyStatement` |
| 🔴 Red | DROP ASYMMETRIC KEY | `DROP ASYMMETRIC KEY k1` |  | `DropAsymmetricKeyStatement` |
| 🔴 Red | DROP AVAILABILITY GROUP | `DROP AVAILABILITY GROUP group1` |  | `DropAvailabilityGroupStatement` |
| 🔴 Red | DROP BROKER PRIORITY | `DROP BROKER PRIORITY bp1` |  | `DropBrokerPriorityStatement` |
| 🔴 Red | DROP CERTIFICATE | `DROP CERTIFICATE c1` |  | `DropCertificateStatement` |
| 🔴 Red | DROP COLUMN ENCRYPTION KEY | `DROP COLUMN ENCRYPTION KEY OneValueCEK` |  | `DropColumnEncryptionKeyStatement` |
| 🔴 Red | DROP COLUMN MASTER KEY | `DROP COLUMN MASTER KEY CMK1` |  | `DropColumnMasterKeyStatement` |
| 🔴 Red | DROP CONTRACT | `DROP CONTRACT c1` |  | `DropContractStatement` |
| 🔴 Red | DROP CREDENTIAL | `DROP CREDENTIAL c1` |  | `DropCredentialStatement` |
| 🔴 Red | DROP CRYPTOGRAPHIC PROVIDER | `DROP CRYPTOGRAPHIC PROVIDER cp1` |  | `DropCryptographicProviderStatement` |
| 🔴 Red | DROP DATABASE | `DROP DATABASE dbX` |  | `DropDatabaseStatement` |
| 🔴 Red | DROP DATABASE AUDIT SPECIFICATION | `DROP DATABASE AUDIT SPECIFICATION AuditSpec1` |  | `DropDatabaseAuditSpecificationStatement` |
| 🔴 Red | DROP DATABASE ENCRYPTION KEY | `DROP DATABASE ENCRYPTION KEY` |  | `DropDatabaseEncryptionKeyStatement` |
| 🔴 Red | DROP DEFAULT | `DROP DEFAULT d1` |  | `DropDefaultStatement` |
| 🔴 Red | DROP ENDPOINT | `DROP ENDPOINT e1` |  | `DropEndpointStatement` |
| 🔴 Red | DROP EVENT NOTIFICATION | `DROP EVENT NOTIFICATION n1 ON QUEUE q1` |  | `DropEventNotificationStatement` |
| 🔴 Red | DROP EVENT SESSION | `DROP EVENT SESSION es1 ON SERVER` |  | `DropEventSessionStatement` |
| 🔴 Red | DROP EXTERNAL DATA SOURCE | `DROP EXTERNAL DATA SOURCE eds1` |  | `DropExternalDataSourceStatement` |
| 🔴 Red | DROP EXTERNAL FILE FORMAT | `DROP EXTERNAL FILE FORMAT eff1` |  | `DropExternalFileFormatStatement` |
| 🔴 Red | DROP EXTERNAL LANGUAGE | `DROP EXTERNAL LANGUAGE language1` |  | `DropExternalLanguageStatement` |
| 🔴 Red | DROP EXTERNAL LIBRARY | `DROP EXTERNAL LIBRARY eltest` |  | `DropExternalLibraryStatement` |
| 🔴 Red | DROP EXTERNAL MODEL | `DROP EXTERNAL MODEL my_model1` |  | `DropExternalModelStatement` |
| 🔴 Red | DROP EXTERNAL RESOURCE POOL | `DROP EXTERNAL RESOURCE POOL p` |  | `DropExternalResourcePoolStatement` |
| 🔴 Red | DROP EXTERNAL TABLE | `DROP EXTERNAL TABLE t1` |  | `DropExternalTableStatement` |
| 🔴 Red | DROP FEDERATION | `DROP FEDERATION fed1` |  | `DropFederationStatement` |
| 🔴 Red | DROP FULLTEXT CATALOG | `DROP FULLTEXT CATALOG c1` |  | `DropFullTextCatalogStatement` |
| 🔴 Red | DROP FULLTEXT INDEX | `DROP FULLTEXT INDEX ON t1` |  | `DropFullTextIndexStatement` |
| 🔴 Red | DROP FULLTEXT STOPLIST | `DROP FULLTEXT STOPLIST fs1;` |  | `DropFullTextStopListStatement` |
| 🔴 Red | DROP FUNCTION | `DROP FUNCTION f1` |  | `DropFunctionStatement` |
| 🔴 Red | DROP INDEX | `DROP INDEX authors.au_id_ind` | 🟢 Green on a #temp table (`DROP INDEX IX_Items ON #Items;`). | `DropIndexStatement` |
| 🔴 Red | DROP LOGIN | `DROP LOGIN l1` |  | `DropLoginStatement` |
| 🔴 Red | DROP MASTER KEY | `DROP MASTER KEY` |  | `DropMasterKeyStatement` |
| 🔴 Red | DROP MESSAGE TYPE | `DROP MESSAGE TYPE mt1` |  | `DropMessageTypeStatement` |
| 🔴 Red | DROP PARTITION FUNCTION | `DROP PARTITION FUNCTION f1` |  | `DropPartitionFunctionStatement` |
| 🔴 Red | DROP PARTITION SCHEME | `DROP PARTITION SCHEME s1` |  | `DropPartitionSchemeStatement` |
| 🔴 Red | DROP PROCEDURE | `DROP PROCEDURE p1` | 🟢 Green for a #temp procedure (`DROP PROCEDURE #TempProc`). | `DropProcedureStatement` |
| 🔴 Red | DROP QUEUE | `DROP QUEUE q1` |  | `DropQueueStatement` |
| 🔴 Red | DROP REMOTE SERVICE BINDING | `DROP REMOTE SERVICE BINDING rsb1` |  | `DropRemoteServiceBindingStatement` |
| 🔴 Red | DROP RESOURCE POOL | `DROP RESOURCE POOL res_pool` |  | `DropResourcePoolStatement` |
| 🔴 Red | DROP ROLE | `DROP ROLE r1` |  | `DropRoleStatement` |
| 🔴 Red | DROP ROUTE | `DROP ROUTE r1` |  | `DropRouteStatement` |
| 🔴 Red | DROP RULE | `DROP RULE [r1]` |  | `DropRuleStatement` |
| 🔴 Red | DROP SCHEMA | `DROP SCHEMA s1` |  | `DropSchemaStatement` |
| 🔴 Red | DROP SEARCH PROPERTY LIST | `DROP SEARCH PROPERTY LIST spl1;` |  | `DropSearchPropertyListStatement` |
| 🔴 Red | DROP SECURITY POLICY | `DROP SECURITY POLICY secX` |  | `DropSecurityPolicyStatement` |
| 🔴 Red | DROP SENSITIVITY CLASSIFICATION | `DROP SENSITIVITY CLASSIFICATION FROM T1.C1, T1.C2` |  | `DropSensitivityClassificationStatement` |
| 🔴 Red | DROP SEQUENCE | `DROP SEQUENCE seqX` |  | `DropSequenceStatement` |
| 🔴 Red | DROP SERVER AUDIT | `DROP SERVER AUDIT a1` |  | `DropServerAuditStatement` |
| 🔴 Red | DROP SERVER AUDIT SPECIFICATION | `DROP SERVER AUDIT SPECIFICATION Spec1` |  | `DropServerAuditSpecificationStatement` |
| 🔴 Red | DROP SERVER ROLE | `DROP SERVER ROLE sr1` |  | `DropServerRoleStatement` |
| 🔴 Red | DROP SERVICE | `DROP SERVICE s1` |  | `DropServiceStatement` |
| 🔴 Red | DROP SIGNATURE | `DROP SIGNATURE FROM OBJECT::dbo.CoverageSigned BY CERTIFICATE CoverageCert;` |  | `DropSignatureStatement` |
| 🔴 Red | DROP STATISTICS | `DROP STATISTICS dbo.Items.Stats;` | 🟢 Green on a #temp table (`DROP STATISTICS #Items.Stats;`). | `DropStatisticsStatement` |
| 🔴 Red | DROP SYMMETRIC KEY | `DROP SYMMETRIC KEY k1` |  | `DropSymmetricKeyStatement` |
| 🔴 Red | DROP SYNONYM | `DROP SYNONYM synX` |  | `DropSynonymStatement` |
| 🔴 Red | DROP TABLE | `DROP TABLE dbo.Items;` | 🟢 Green on a #temp table (`DROP TABLE #Items;`). | `DropTableStatement` |
| 🔴 Red | DROP TRIGGER | `DROP TRIGGER t1` |  | `DropTriggerStatement` |
| 🔴 Red | DROP TYPE | `DROP TYPE t1` |  | `DropTypeStatement` |
| 🔴 Red | DROP USER | `DROP USER u1` |  | `DropUserStatement` |
| 🔴 Red | DROP VIEW | `DROP VIEW v1` |  | `DropViewStatement` |
| 🔴 Red | DROP WORKLOAD CLASSIFIER | `DROP WORKLOAD CLASSIFIER wcAllOptions` |  | `DropWorkloadClassifierStatement` |
| 🔴 Red | DROP WORKLOAD GROUP | `DROP WORKLOAD GROUP wg1` |  | `DropWorkloadGroupStatement` |
| 🔴 Red | DROP XML SCHEMA COLLECTION | `DROP XML SCHEMA COLLECTION a` |  | `DropXmlSchemaCollectionStatement` |
| 🔴 Red | ENABLE / DISABLE TRIGGER | `ENABLE TRIGGER t1 ON o1` |  | `EnableDisableTriggerStatement` |
| 🔴 Red | END CONVERSATION | `END CONVERSATION 10` |  | `EndConversationStatement` |
| 🔴 Red | EXEC sys.sp_create_streaming_job | `EXEC sys.sp_create_streaming_job @name=N'CoverageJob', @statement=N'SELECT 1'` |  | `CreateExternalStreamingJobStatement` |
| 🔴 Red | GRANT | `GRANT BACKUP LOG TO [guest]` |  | `GrantStatement` |
| 🔴 Red | GRANT | `GRANT SELECT ON dbo.Coverage TO public;` | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `GrantStatement80` |
| 🔴 Red | INSERT | `INSERT dbo.Items VALUES (3,30);` | 🟢 Green on a #temp table (`INSERT #Items VALUES (3,30);`). 🟢 Green on a @table variable (`INSERT @Items VALUES (3,30);`). 🟢 Green with the target allow-listed in settings (`INSERT dbo.Items VALUES (3,30);`). | `InsertStatement` |
| 🔴 Red | INSERT BULK | `INSERT BULK db1.dbo.t1` | 🟢 Green on a #temp table (`INSERT BULK #Items`). 🟢 Green with the target allow-listed in settings (`INSERT BULK db1.dbo.t1`). | `InsertBulkStatement` |
| 🔴 Red | KILL | `KILL 12` |  | `KillStatement` |
| 🔴 Red | KILL QUERY NOTIFICATION SUBSCRIPTION | `KILL QUERY NOTIFICATION SUBSCRIPTION 12` |  | `KillQueryNotificationSubscriptionStatement` |
| 🔴 Red | KILL STATS JOB | `KILL STATS JOB 12` |  | `KillStatsJobStatement` |
| 🔴 Red | MERGE | `MERGE dbo.Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;` | 🟢 Green on a #temp table (`MERGE #Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;`). 🟢 Green on a @table variable (`MERGE @Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;`). 🟢 Green with the target allow-listed in settings (`MERGE dbo.Items AS t USING (SELECT 1 AS Id) AS s ON t.Id = s.Id WHEN MATCHED THEN DELETE;`). | `MergeStatement` |
| 🔴 Red | MOVE CONVERSATION | `MOVE CONVERSATION @handle TO @destination;` |  | `MoveConversationStatement` |
| 🔴 Red | RECEIVE | `RECEIVE * FROM ExpenseQueue` |  | `ReceiveStatement` |
| 🔴 Red | RECONFIGURE | `RECONFIGURE` |  | `ReconfigureStatement` |
| 🔴 Red | RENAME OBJECT | `RENAME OBJECT T2 TO T1` |  | `RenameEntityStatement` |
| 🔴 Red | RESTORE DATABASE / LOG | `RESTORE LOG db1` |  | `RestoreStatement` |
| 🔴 Red | RESTORE MASTER KEY | `RESTORE MASTER KEY FROM FILE = 'f1' DECRYPTION BY PASSWORD = 'p1' ENCRYPTION BY PASSWORD = 'p2'` |  | `RestoreMasterKeyStatement` |
| 🔴 Red | RESTORE SERVICE MASTER KEY | `RESTORE SERVICE MASTER KEY FROM FILE = 'f1' DECRYPTION BY PASSWORD = 'pw1'` |  | `RestoreServiceMasterKeyStatement` |
| 🔴 Red | REVOKE | `REVOKE all TO PUBLIC CASCADE` |  | `RevokeStatement` |
| 🔴 Red | REVOKE | `REVOKE SELECT ON dbo.Coverage FROM public;` | Alternate-dialect fixture (80); colour is the default analyzer response, including parse errors. | `RevokeStatement80` |
| 🔴 Red | SEND | `SEND ON CONVERSATION (@dialog_handle) MESSAGE TYPE m1` |  | `SendStatement` |
| 🔴 Red | SHUTDOWN | `SHUTDOWN` |  | `ShutdownStatement` |
| 🔴 Red | TRUNCATE TABLE | `TRUNCATE TABLE dbo.Items;` | 🟢 Green on a #temp table (`TRUNCATE TABLE #Items;`). 🟢 Green with the target allow-listed in settings (`TRUNCATE TABLE dbo.Items;`). | `TruncateTableStatement` |
| 🔴 Red | UPDATE | `UPDATE dbo.Items SET Qty = 20 WHERE Id = 1;` | 🟢 Green on a #temp table (`UPDATE #Items SET Qty = 20 WHERE Id = 1;`). 🟢 Green on a @table variable (`UPDATE @Items SET Qty = 20 WHERE Id = 1;`). 🟢 Green with the target allow-listed in settings (`UPDATE dbo.Items SET Qty = 20 WHERE Id = 1;`). | `UpdateStatement` |
| 🔴 Red | UPDATETEXT | `UPDATETEXT dbo.TextItems.Body @ptr 0 3 'new';` |  | `UpdateTextStatement` |
| 🔴 Red | WHILE | `WHILE @a > 10 CREATE TABLE t1 ( c1 INT );` | Contained statements are analyzed too; their effects can change the colour. | `WhileStatement` |
| 🔴 Red | WRITETEXT | `WRITETEXT t1.c1 @var 0xFF` |  | `WriteTextStatement` |
| — | DROP EXTERNAL STREAM | — | No standalone SQL: Generator AST node; pinned parser has no DROP EXTERNAL STREAM production. | `DropExternalStreamStatement` |
| — | DROP EXTERNAL STREAMING JOB | — | No standalone SQL: Generator AST node; sp_drop_streaming_job parses as ExecuteStatement, not this node. | `DropExternalStreamingJobStatement` |
| — | EVENT SESSION | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter event-session subclasses. | `EventSessionStatement` |
| — | EXTERNAL RESOURCE POOL | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter external-resource-pool subclasses. | `ExternalResourcePoolStatement` |
| — | RESOURCE POOL | — | No standalone SQL: Concrete shared AST base; actual SQL produces create/alter resource-pool subclasses. | `ResourcePoolStatement` |
| — | SELECT snippet (AST only) | — | No standalone SQL: Programmatically constructed SELECT snippet, not a standalone parser statement kind. | `SelectStatementSnippet` |
| — | SQL snippet (AST only) | — | No standalone SQL: Programmatically constructed raw statement snippet; not a parser production. | `TSqlStatementSnippet` |

Generated by StatementVerdictsTests from the coverage manifest and fixtures. To regenerate in PowerShell:

~~~powershell
$env:UPDATE_STATEMENT_VERDICTS='1'
dotnet test tests/SqlSafetyGuard.Tests --filter FullyQualifiedName~StatementVerdictsTests
Remove-Item Env:UPDATE_STATEMENT_VERDICTS
~~~
