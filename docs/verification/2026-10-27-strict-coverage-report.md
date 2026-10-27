# Strict SQL Server coverage evidence

- Run (UTC): 2026-10-27T15:30:25.7246995+00:00
- Analyzer SHA-256: 06086DE72295F47DBEFEDF78B8A52054C26603E50B1596849BA2D95133DFF2D7
- ScriptDom package: 180.140.0; inventory: 334 concrete kinds.
- Fixtures: 118/118; pass=118
- Kind statuses: missing=198, safety-excluded=33, pass=103
- Every fixture uses a fresh uniquely named database and a database-scoped user without a login. Cleanup drops only that database. Existing SqlGuard_Prod/SqlGuard_Dev and SSMS settings are untouched.
- Only fixture SQL is analyzed. Setup is excluded. Exact table rows and selected catalog state are compared after setup, before closing the session, and after session-close rollback. Errors are not passes.
- This is representative kind/fixture coverage, NOT every clause combination, execution path, external side effect, SSMS event dispatch, or physical storage change. Body AST nodes alone do not earn coverage. A zero exit code means selected fixtures passed, not complete coverage; use --require-complete to enforce all kinds.

## Fixtures

| ID | Verdict | Visible change | Persisted change | Status |
|---|---|---|---|---|
| read-select | ReadOnly | False | False | pass |
| session-nocount | ReadOnly | False | False | pass |
| transaction-balanced | ReadOnly | False | False | pass |
| transaction-rollback | ReadOnly | False | False | pass |
| temp-insert | TempOnly | False | False | pass |
| alias-update | RealWrite | True | True | pass |
| cte-delete | RealWrite | True | True | pass |
| composable-delete | RealWrite | True | True | pass |
| merge-real | RealWrite | True | True | pass |
| dbcc-checkident | ReadOnly | False | False | pass |
| create-temp-procedure | TempOnly | False | False | pass |
| execute-procedure | Unknown | False | False | pass |
| create-table | RealWrite | True | True | pass |
| drop-table | RealWrite | True | True | pass |
| read-join-window | ReadOnly | False | False | pass |
| read-select-into-real | RealWrite | True | True | pass |
| read-select-into-temp | TempOnly | False | False | pass |
| insert-real | RealWrite | True | True | pass |
| update-no-where | RealWrite | True | True | pass |
| delete-no-where | RealWrite | True | True | pass |
| update-temp | TempOnly | False | False | pass |
| delete-temp | TempOnly | False | False | pass |
| alias-temp-looking-real | RealWrite | True | True | pass |
| output-into-real | RealWrite | True | True | pass |
| output-into-temp | RealWrite | True | True | pass |
| merge-temp | TempOnly | False | False | pass |
| dynamic-literal-write | RealWrite | True | True | pass |
| dynamic-literal-read | ReadOnly | False | False | pass |
| dynamic-runtime-write | Unknown | True | True | pass |
| procedure-hidden-write | Unknown | True | True | pass |
| trigger-hidden-write | RealWrite | True | True | pass |
| view-target-update | RealWrite | True | True | pass |
| sequence-select | RealWrite | True | True | pass |
| write-rollback | RealWrite | False | False | pass |
| write-uncommitted | RealWrite | True | False | pass |
| savepoint-rollback | RealWrite | False | False | pass |
| if-unreachable-write | RealWrite | False | False | pass |
| if-reachable-write | RealWrite | True | True | pass |
| while-control-flow | ReadOnly | False | False | pass |
| try-catch-read | ReadOnly | False | False | pass |
| throw-caught | ReadOnly | False | False | pass |
| raiseerror-caught | ReadOnly | False | False | pass |
| goto-label | ReadOnly | False | False | pass |
| return-read | ReadOnly | False | False | pass |
| print-read | ReadOnly | False | False | pass |
| waitfor-zero | ReadOnly | False | False | pass |
| table-variable | TempOnly | False | False | pass |
| cursor-lifecycle | ReadOnly | False | False | pass |
| session-isolation | ReadOnly | False | False | pass |
| session-statistics | ReadOnly | False | False | pass |
| session-textsize | ReadOnly | False | False | pass |
| session-rowcount | ReadOnly | False | False | pass |
| truncate-table | RealWrite | True | True | pass |
| alter-table-add | RealWrite | True | True | pass |
| alter-table-column | RealWrite | True | True | pass |
| alter-table-drop | RealWrite | True | True | pass |
| create-index | RealWrite | True | True | pass |
| alter-index | RealWrite | True | True | pass |
| drop-index | RealWrite | True | True | pass |
| create-statistics | RealWrite | True | True | pass |
| drop-statistics | RealWrite | True | True | pass |
| create-procedure | RealWrite | True | True | pass |
| alter-procedure | RealWrite | True | True | pass |
| create-or-alter-procedure | RealWrite | True | True | pass |
| drop-procedure | RealWrite | True | True | pass |
| create-view | RealWrite | True | True | pass |
| alter-view | RealWrite | True | True | pass |
| create-or-alter-view | RealWrite | True | True | pass |
| drop-view | RealWrite | True | True | pass |
| create-function | RealWrite | True | True | pass |
| alter-function | RealWrite | True | True | pass |
| create-or-alter-function | RealWrite | True | True | pass |
| drop-function | RealWrite | True | True | pass |
| create-trigger | RealWrite | True | True | pass |
| alter-trigger | RealWrite | True | True | pass |
| create-or-alter-trigger | RealWrite | True | True | pass |
| disable-trigger | RealWrite | True | True | pass |
| drop-trigger | RealWrite | True | True | pass |
| create-sequence | RealWrite | True | True | pass |
| alter-sequence | RealWrite | True | True | pass |
| drop-sequence | RealWrite | True | True | pass |
| create-schema | RealWrite | True | True | pass |
| alter-schema | RealWrite | True | True | pass |
| drop-schema | RealWrite | True | True | pass |
| create-type-alias | RealWrite | True | True | pass |
| create-type-table | RealWrite | True | True | pass |
| drop-type | RealWrite | True | True | pass |
| create-synonym | RealWrite | True | True | pass |
| drop-synonym | RealWrite | True | True | pass |
| create-user | RealWrite | True | True | pass |
| alter-user | RealWrite | True | True | pass |
| drop-user | RealWrite | True | True | pass |
| create-role | RealWrite | True | True | pass |
| alter-role | RealWrite | True | True | pass |
| drop-role | RealWrite | True | True | pass |
| grant-select | RealWrite | True | True | pass |
| deny-select | RealWrite | True | True | pass |
| revoke-select | RealWrite | True | True | pass |
| execute-as-revert | ReadOnly | False | False | pass |
| dbcc-reseed | RealWrite | True | True | pass |
| dbcc-checktable | ReadOnly | False | False | pass |
| dbcc-useroptions | ReadOnly | False | False | pass |
| create-default | RealWrite | True | True | pass |
| drop-default | RealWrite | True | True | pass |
| create-rule | RealWrite | True | True | pass |
| drop-rule | RealWrite | True | True | pass |
| create-xml-schema | RealWrite | True | True | pass |
| drop-xml-schema | RealWrite | True | True | pass |
| create-partition-function | RealWrite | True | True | pass |
| alter-partition-function | RealWrite | True | True | pass |
| drop-partition-function | RealWrite | True | True | pass |
| create-partition-scheme | RealWrite | True | True | pass |
| drop-partition-scheme | RealWrite | True | True | pass |
| create-application-role | RealWrite | True | True | pass |
| drop-application-role | RealWrite | True | True | pass |
| write-text | RealWrite | True | True | pass |
| update-text | RealWrite | True | True | pass |
| read-text | ReadOnly | False | False | pass |

## Complete inventory

| Kind | Status | Fixtures / reason |
|---|---|---|
| AddSensitivityClassificationStatement | missing |   |
| AddSignatureStatement | missing |   |
| AlterApplicationRoleStatement | missing |   |
| AlterAssemblyStatement | missing |   |
| AlterAsymmetricKeyStatement | missing |   |
| AlterAuthorizationStatement | missing |   |
| AlterAvailabilityGroupStatement | safety-excluded |  Changes server/cluster availability configuration. |
| AlterBrokerPriorityStatement | missing |   |
| AlterCertificateStatement | missing |   |
| AlterColumnEncryptionKeyStatement | missing |   |
| AlterCredentialStatement | safety-excluded |  Changes server credential state outside the disposable database. |
| AlterCryptographicProviderStatement | missing |   |
| AlterDatabaseAddFileGroupStatement | missing |   |
| AlterDatabaseAddFileStatement | missing |   |
| AlterDatabaseAuditSpecificationStatement | missing |   |
| AlterDatabaseCollateStatement | missing |   |
| AlterDatabaseEncryptionKeyStatement | missing |   |
| AlterDatabaseModifyFileGroupStatement | missing |   |
| AlterDatabaseModifyFileStatement | missing |   |
| AlterDatabaseModifyNameStatement | missing |   |
| AlterDatabasePerformCutoverStatement | missing |   |
| AlterDatabaseRebuildLogStatement | missing |   |
| AlterDatabaseRemoveFileGroupStatement | missing |   |
| AlterDatabaseRemoveFileStatement | missing |   |
| AlterDatabaseScopedConfigurationClearStatement | missing |   |
| AlterDatabaseScopedConfigurationSetStatement | missing |   |
| AlterDatabaseSetStatement | missing |   |
| AlterEndpointStatement | safety-excluded |  Changes an instance endpoint beyond fixture isolation. |
| AlterEventSessionStatement | missing |   |
| AlterExternalDataSourceStatement | missing |   |
| AlterExternalFunctionStatement | missing |   |
| AlterExternalLanguageStatement | missing |   |
| AlterExternalLibraryStatement | missing |   |
| AlterExternalModelStatement | missing |   |
| AlterExternalResourcePoolStatement | missing |   |
| AlterFederationStatement | missing |   |
| AlterFullTextCatalogStatement | missing |   |
| AlterFullTextIndexStatement | missing |   |
| AlterFullTextStopListStatement | missing |   |
| AlterFunctionStatement | pass | alter-function  |
| AlterIndexStatement | pass | alter-index  |
| AlterLoginAddDropCredentialStatement | safety-excluded |  Changes server-scoped credentials. |
| AlterLoginEnableDisableStatement | safety-excluded |  Changes server-scoped authentication availability. |
| AlterLoginOptionsStatement | safety-excluded |  Changes server-scoped authentication settings. |
| AlterMasterKeyStatement | missing |   |
| AlterMessageTypeStatement | missing |   |
| AlterPartitionFunctionStatement | pass | alter-partition-function  |
| AlterPartitionSchemeStatement | missing |   |
| AlterProcedureStatement | pass | alter-procedure  |
| AlterQueueStatement | missing |   |
| AlterRemoteServiceBindingStatement | missing |   |
| AlterResourceGovernorStatement | safety-excluded |  Changes instance-wide resource governance. |
| AlterResourcePoolStatement | safety-excluded |  Changes instance-wide resource governance. |
| AlterRoleStatement | pass | alter-role  |
| AlterRouteStatement | missing |   |
| AlterSchemaStatement | pass | alter-schema  |
| AlterSearchPropertyListStatement | missing |   |
| AlterSecurityPolicyStatement | missing |   |
| AlterSequenceStatement | pass | alter-sequence  |
| AlterServerAuditSpecificationStatement | missing |   |
| AlterServerAuditStatement | missing |   |
| AlterServerConfigurationSetBufferPoolExtensionStatement | missing |   |
| AlterServerConfigurationSetDiagnosticsLogStatement | missing |   |
| AlterServerConfigurationSetExternalAuthenticationStatement | missing |   |
| AlterServerConfigurationSetFailoverClusterPropertyStatement | missing |   |
| AlterServerConfigurationSetHadrClusterStatement | missing |   |
| AlterServerConfigurationSetSoftNumaStatement | missing |   |
| AlterServerConfigurationStatement | safety-excluded |  Changes instance-wide settings. |
| AlterServerRoleStatement | safety-excluded |  Changes instance-level security. |
| AlterServiceMasterKeyStatement | missing |   |
| AlterServiceStatement | missing |   |
| AlterSymmetricKeyStatement | missing |   |
| AlterTableAddClusterByStatement | missing |   |
| AlterTableAddTableElementStatement | pass | alter-table-add  |
| AlterTableAlterColumnStatement | pass | alter-table-column  |
| AlterTableAlterIndexStatement | missing |   |
| AlterTableAlterPartitionStatement | missing |   |
| AlterTableChangeTrackingModificationStatement | missing |   |
| AlterTableConstraintModificationStatement | missing |   |
| AlterTableDropTableElementStatement | pass | alter-table-drop  |
| AlterTableFileTableNamespaceStatement | missing |   |
| AlterTableRebuildStatement | missing |   |
| AlterTableSetStatement | missing |   |
| AlterTableSwitchStatement | missing |   |
| AlterTableTriggerModificationStatement | missing |   |
| AlterTriggerStatement | pass | alter-trigger  |
| AlterUserStatement | pass | alter-user  |
| AlterViewStatement | pass | alter-view  |
| AlterWorkloadGroupStatement | missing |   |
| AlterXmlSchemaCollectionStatement | missing |   |
| BackupCertificateStatement | missing |   |
| BackupDatabaseStatement | safety-excluded |  Writes external backup files; filesystem effects are outside this database-only oracle. |
| BackupMasterKeyStatement | missing |   |
| BackupServiceMasterKeyStatement | missing |   |
| BackupTransactionLogStatement | safety-excluded |  Writes external backup files; filesystem effects are outside this database-only oracle. |
| BeginConversationTimerStatement | missing |   |
| BeginDialogStatement | missing |   |
| BeginEndAtomicBlockStatement | missing |   |
| BeginEndBlockStatement | pass | while-control-flow  |
| BeginTransactionStatement | pass | transaction-balanced, transaction-rollback, write-rollback, write-uncommitted, savepoint-rollback  |
| BreakStatement | pass | while-control-flow  |
| BulkInsertStatement | safety-excluded |  Requires external filesystem input; not provisioned or authorized by this corpus. |
| CheckpointStatement | missing |   |
| CloseCursorStatement | pass | cursor-lifecycle  |
| CloseMasterKeyStatement | missing |   |
| CloseSymmetricKeyStatement | missing |   |
| CommitTransactionStatement | pass | transaction-balanced, savepoint-rollback  |
| ContinueStatement | pass | while-control-flow  |
| CopyStatement | missing |   |
| CreateAggregateStatement | missing |   |
| CreateApplicationRoleStatement | pass | create-application-role  |
| CreateAssemblyStatement | missing |   |
| CreateAsymmetricKeyStatement | missing |   |
| CreateAvailabilityGroupStatement | safety-excluded |  Changes server/cluster availability configuration. |
| CreateBrokerPriorityStatement | missing |   |
| CreateCertificateStatement | missing |   |
| CreateColumnEncryptionKeyStatement | missing |   |
| CreateColumnMasterKeyStatement | missing |   |
| CreateColumnStoreIndexStatement | missing |   |
| CreateContractStatement | missing |   |
| CreateCredentialStatement | safety-excluded |  Changes server credential state outside the disposable database. |
| CreateCryptographicProviderStatement | missing |   |
| CreateDatabaseAuditSpecificationStatement | missing |   |
| CreateDatabaseEncryptionKeyStatement | missing |   |
| CreateDatabaseStatement | safety-excluded |  Creates databases outside the runner-owned fixture database; only runner provisioning is authorized. |
| CreateDefaultStatement | pass | create-default  |
| CreateEndpointStatement | safety-excluded |  Creates an instance endpoint beyond fixture isolation. |
| CreateEventNotificationStatement | missing |   |
| CreateEventSessionStatement | missing |   |
| CreateExternalDataSourceStatement | missing |   |
| CreateExternalFileFormatStatement | missing |   |
| CreateExternalFunctionStatement | missing |   |
| CreateExternalLanguageStatement | missing |   |
| CreateExternalLibraryStatement | missing |   |
| CreateExternalModelStatement | missing |   |
| CreateExternalResourcePoolStatement | missing |   |
| CreateExternalStreamingJobStatement | missing |   |
| CreateExternalStreamStatement | missing |   |
| CreateExternalTableStatement | missing |   |
| CreateFederationStatement | missing |   |
| CreateFullTextCatalogStatement | missing |   |
| CreateFullTextIndexStatement | missing |   |
| CreateFullTextStopListStatement | missing |   |
| CreateFunctionStatement | pass | create-function  |
| CreateIndexStatement | pass | create-index  |
| CreateJsonIndexStatement | missing |   |
| CreateLoginStatement | safety-excluded |  Changes server-scoped security; the runner user has no login/server privileges. |
| CreateMasterKeyStatement | missing |   |
| CreateMessageTypeStatement | missing |   |
| CreateOrAlterExternalFunctionStatement | missing |   |
| CreateOrAlterFunctionStatement | pass | create-or-alter-function  |
| CreateOrAlterProcedureStatement | pass | create-or-alter-procedure  |
| CreateOrAlterTriggerStatement | pass | create-or-alter-trigger  |
| CreateOrAlterViewStatement | pass | create-or-alter-view  |
| CreatePartitionFunctionStatement | pass | create-partition-function  |
| CreatePartitionSchemeStatement | pass | create-partition-scheme  |
| CreateProcedureStatement | pass | create-temp-procedure, create-procedure  |
| CreateQueueStatement | missing |   |
| CreateRemoteServiceBindingStatement | missing |   |
| CreateResourcePoolStatement | safety-excluded |  Changes instance-wide resource governance. |
| CreateRoleStatement | pass | create-role  |
| CreateRouteStatement | missing |   |
| CreateRuleStatement | pass | create-rule  |
| CreateSchemaStatement | pass | create-schema  |
| CreateSearchPropertyListStatement | missing |   |
| CreateSecurityPolicyStatement | missing |   |
| CreateSelectiveXmlIndexStatement | missing |   |
| CreateSemanticIndexStatement | missing |   |
| CreateSequenceStatement | pass | create-sequence  |
| CreateServerAuditSpecificationStatement | missing |   |
| CreateServerAuditStatement | missing |   |
| CreateServerRoleStatement | safety-excluded |  Changes instance-level security. |
| CreateServiceStatement | missing |   |
| CreateSpatialIndexStatement | missing |   |
| CreateStatisticsStatement | pass | create-statistics  |
| CreateSymmetricKeyStatement | missing |   |
| CreateSynonymStatement | pass | create-synonym  |
| CreateTableStatement | pass | create-table  |
| CreateTriggerStatement | pass | create-trigger  |
| CreateTypeTableStatement | pass | create-type-table  |
| CreateTypeUddtStatement | pass | create-type-alias  |
| CreateTypeUdtStatement | missing |   |
| CreateUserStatement | pass | create-user  |
| CreateVectorIndexStatement | missing |   |
| CreateViewStatement | pass | create-view  |
| CreateWorkloadClassifierStatement | missing |   |
| CreateWorkloadGroupStatement | missing |   |
| CreateXmlIndexStatement | missing |   |
| CreateXmlSchemaCollectionStatement | pass | create-xml-schema  |
| DbccStatement | pass | dbcc-checkident, dbcc-reseed, dbcc-checktable, dbcc-useroptions  |
| DeallocateCursorStatement | pass | cursor-lifecycle  |
| DeclareCursorStatement | pass | cursor-lifecycle  |
| DeclareTableVariableStatement | pass | table-variable  |
| DeclareVariableStatement | pass | dynamic-runtime-write, while-control-flow, cursor-lifecycle  |
| DeleteStatement | pass | cte-delete, delete-no-where, delete-temp  |
| DenyStatement | pass | deny-select  |
| DenyStatement80 | missing |   |
| DiskStatement | safety-excluded |  Legacy disk/server operation outside disposable database isolation. |
| DropAggregateStatement | missing |   |
| DropApplicationRoleStatement | pass | drop-application-role  |
| DropAssemblyStatement | missing |   |
| DropAsymmetricKeyStatement | missing |   |
| DropAvailabilityGroupStatement | safety-excluded |  Changes server/cluster availability configuration. |
| DropBrokerPriorityStatement | missing |   |
| DropCertificateStatement | missing |   |
| DropColumnEncryptionKeyStatement | missing |   |
| DropColumnMasterKeyStatement | missing |   |
| DropContractStatement | missing |   |
| DropCredentialStatement | safety-excluded |  Changes server credential state outside the disposable database. |
| DropCryptographicProviderStatement | missing |   |
| DropDatabaseAuditSpecificationStatement | missing |   |
| DropDatabaseEncryptionKeyStatement | missing |   |
| DropDatabaseStatement | safety-excluded |  Could remove databases outside the runner-owned fixture database. |
| DropDefaultStatement | pass | drop-default  |
| DropEndpointStatement | safety-excluded |  Removes an instance endpoint beyond fixture isolation. |
| DropEventNotificationStatement | missing |   |
| DropEventSessionStatement | missing |   |
| DropExternalDataSourceStatement | missing |   |
| DropExternalFileFormatStatement | missing |   |
| DropExternalLanguageStatement | missing |   |
| DropExternalLibraryStatement | missing |   |
| DropExternalModelStatement | missing |   |
| DropExternalResourcePoolStatement | missing |   |
| DropExternalStreamingJobStatement | missing |   |
| DropExternalStreamStatement | missing |   |
| DropExternalTableStatement | missing |   |
| DropFederationStatement | missing |   |
| DropFullTextCatalogStatement | missing |   |
| DropFullTextIndexStatement | missing |   |
| DropFullTextStopListStatement | missing |   |
| DropFunctionStatement | pass | drop-function  |
| DropIndexStatement | pass | drop-index  |
| DropLoginStatement | safety-excluded |  Changes server-scoped security outside disposable database ownership. |
| DropMasterKeyStatement | missing |   |
| DropMessageTypeStatement | missing |   |
| DropPartitionFunctionStatement | pass | drop-partition-function  |
| DropPartitionSchemeStatement | pass | drop-partition-scheme  |
| DropProcedureStatement | pass | drop-procedure  |
| DropQueueStatement | missing |   |
| DropRemoteServiceBindingStatement | missing |   |
| DropResourcePoolStatement | safety-excluded |  Changes instance-wide resource governance. |
| DropRoleStatement | pass | drop-role  |
| DropRouteStatement | missing |   |
| DropRuleStatement | pass | drop-rule  |
| DropSchemaStatement | pass | drop-schema  |
| DropSearchPropertyListStatement | missing |   |
| DropSecurityPolicyStatement | missing |   |
| DropSensitivityClassificationStatement | missing |   |
| DropSequenceStatement | pass | drop-sequence  |
| DropServerAuditSpecificationStatement | missing |   |
| DropServerAuditStatement | missing |   |
| DropServerRoleStatement | safety-excluded |  Changes instance-level security. |
| DropServiceStatement | missing |   |
| DropSignatureStatement | missing |   |
| DropStatisticsStatement | pass | drop-statistics  |
| DropSymmetricKeyStatement | missing |   |
| DropSynonymStatement | pass | drop-synonym  |
| DropTableStatement | pass | drop-table  |
| DropTriggerStatement | pass | drop-trigger  |
| DropTypeStatement | pass | drop-type  |
| DropUserStatement | pass | drop-user  |
| DropViewStatement | pass | drop-view  |
| DropWorkloadClassifierStatement | missing |   |
| DropWorkloadGroupStatement | missing |   |
| DropXmlSchemaCollectionStatement | pass | drop-xml-schema  |
| EnableDisableTriggerStatement | pass | disable-trigger  |
| EndConversationStatement | missing |   |
| EventSessionStatement | missing |   |
| ExecuteAsStatement | pass | execute-as-revert  |
| ExecuteStatement | pass | execute-procedure, dynamic-literal-write, dynamic-literal-read, dynamic-runtime-write, procedure-hidden-write  |
| ExternalResourcePoolStatement | missing |   |
| FetchCursorStatement | pass | cursor-lifecycle  |
| GetConversationGroupStatement | missing |   |
| GoToStatement | pass | goto-label  |
| GrantStatement | pass | grant-select  |
| GrantStatement80 | missing |   |
| IfStatement | pass | if-unreachable-write, if-reachable-write  |
| IndexDefinition | missing |   |
| InsertBulkStatement | safety-excluded |  Requires a bulk-transfer protocol/input rather than an ordinary fixture batch. |
| InsertStatement | pass | temp-insert, composable-delete, insert-real, table-variable  |
| KillQueryNotificationSubscriptionStatement | missing |   |
| KillStatement | safety-excluded |  Terminates sessions outside a disposable database; excluded on the shared instance. |
| KillStatsJobStatement | missing |   |
| LabelStatement | pass | goto-label  |
| LineNoStatement | missing |   |
| MergeStatement | pass | merge-real, merge-temp  |
| MoveConversationStatement | missing |   |
| OpenCursorStatement | pass | cursor-lifecycle  |
| OpenMasterKeyStatement | missing |   |
| OpenSymmetricKeyStatement | missing |   |
| PredicateSetStatement | pass | session-nocount  |
| PrintStatement | pass | print-read  |
| RaiseErrorLegacyStatement | missing |   |
| RaiseErrorStatement | pass | raiseerror-caught  |
| ReadTextStatement | pass | read-text  |
| ReceiveStatement | missing |   |
| ReconfigureStatement | safety-excluded |  Changes server configuration outside the disposable database. |
| RenameEntityStatement | missing |   |
| ResourcePoolStatement | missing |   |
| RestoreMasterKeyStatement | missing |   |
| RestoreServiceMasterKeyStatement | missing |   |
| RestoreStatement | safety-excluded |  Restores external backup media and can replace databases/files outside the fixture. |
| ReturnStatement | pass | return-read  |
| RevertStatement | pass | execute-as-revert  |
| RevokeStatement | pass | revoke-select  |
| RevokeStatement80 | missing |   |
| RollbackTransactionStatement | pass | transaction-rollback, write-rollback, savepoint-rollback  |
| SaveTransactionStatement | pass | savepoint-rollback  |
| SelectStatement | pass | read-select, read-join-window, read-select-into-real, read-select-into-temp, sequence-select, try-catch-read  |
| SelectStatementSnippet | missing |   |
| SendStatement | missing |   |
| SetCommandStatement | missing |   |
| SetErrorLevelStatement | missing |   |
| SetIdentityInsertStatement | missing |   |
| SetOffsetsStatement | missing |   |
| SetRowCountStatement | pass | session-rowcount  |
| SetStatisticsStatement | pass | session-statistics  |
| SetTextSizeStatement | pass | session-textsize  |
| SetTransactionIsolationLevelStatement | pass | session-isolation  |
| SetUserStatement | missing |   |
| SetVariableStatement | pass | while-control-flow  |
| ShutdownStatement | safety-excluded |  Stops the shared SQL Server instance; excluded from execution on the developer host. |
| ThrowStatement | pass | throw-caught  |
| TruncateTableStatement | pass | truncate-table  |
| TryCatchStatement | pass | try-catch-read, throw-caught, raiseerror-caught  |
| TSqlStatementSnippet | missing |   |
| UpdateStatement | pass | alias-update, update-no-where, update-temp, alias-temp-looking-real, output-into-real, output-into-temp, trigger-hidden-write, view-target-update, write-rollback, write-uncommitted, savepoint-rollback, if-unreachable-write, if-reachable-write  |
| UpdateStatisticsStatement | missing |   |
| UpdateTextStatement | pass | update-text  |
| UseFederationStatement | missing |   |
| UseStatement | missing |   |
| WaitForStatement | pass | waitfor-zero  |
| WhileStatement | pass | while-control-flow  |
| WriteTextStatement | pass | write-text  |
