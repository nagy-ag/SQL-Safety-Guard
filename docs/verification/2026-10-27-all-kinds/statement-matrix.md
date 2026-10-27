# Combined 334-kind verification matrix

- Static: 334/334 (318 default parser, 9 alternate dialect, 7 AST-only).
- Live passed: 267; failed-only: 3; not live tested: 64.
- A live pass means the representative fixture executed and its declared observer/verdict assertions passed. Unchanged scalar counts do not prove key restoration or physical/external effects.
- No claim of all-clause coverage, all-engine compatibility, complete side-effect observation, or SSMS/VSIX dispatch testing.

| Kind | Static evidence | Runtime | Fixture IDs / limitation |
|---|---|---|---|
| AddSensitivityClassificationStatement | default-parser-sql | live-pass | expanded-add-sensitivity-classification: pass |
| AddSignatureStatement | default-parser-sql | live-pass | expanded-add-signature: pass |
| AlterApplicationRoleStatement | default-parser-sql | live-pass | expanded-alter-application-role: pass |
| AlterAssemblyStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| AlterAsymmetricKeyStatement | default-parser-sql | live-pass | expanded-alter-asymmetric-key: pass |
| AlterAuthorizationStatement | default-parser-sql | live-pass | expanded-alter-authorization: pass |
| AlterAvailabilityGroupStatement | default-parser-sql | not-live-tested | ; Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterBrokerPriorityStatement | default-parser-sql | live-pass | expanded-alter-broker-priority: pass |
| AlterCertificateStatement | default-parser-sql | live-pass | expanded-alter-certificate: pass |
| AlterColumnEncryptionKeyStatement | default-parser-sql | live-pass | cek-alter: pass |
| AlterCredentialStatement | default-parser-sql | live-pass | server-alter-credential: pass |
| AlterCryptographicProviderStatement | default-parser-sql | not-live-tested | ; Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| AlterDatabaseAddFileGroupStatement | default-parser-sql | live-pass | expanded-database-add-empty-filegroup: pass, database-add-filegroup: pass |
| AlterDatabaseAddFileStatement | default-parser-sql | live-pass | database-add-file: pass |
| AlterDatabaseAuditSpecificationStatement | default-parser-sql | live-pass | db-audit-alter: pass |
| AlterDatabaseCollateStatement | default-parser-sql | live-pass | expanded-database-collate-current: pass, database-collate: pass |
| AlterDatabaseEncryptionKeyStatement | default-parser-sql | live-pass | tde-alter: pass |
| AlterDatabaseModifyFileGroupStatement | default-parser-sql | live-pass | expanded-database-modify-empty-filegroup: pass, database-modify-filegroup: pass |
| AlterDatabaseModifyFileStatement | default-parser-sql | live-pass | database-modify-file: pass |
| AlterDatabaseModifyNameStatement | default-parser-sql | live-pass | database-rename: pass |
| AlterDatabasePerformCutoverStatement | default-parser-sql | not-live-tested | ; Requires a supported Azure database copy/cutover topology, absent from the disposable standalone SQL Server. |
| AlterDatabaseRebuildLogStatement | default-parser-sql | not-live-tested | ; Requires deliberately damaged/offline database/log files and a recovery-specific runner; normal disposable database observations do not prove log reconstruction. |
| AlterDatabaseRemoveFileGroupStatement | default-parser-sql | live-pass | expanded-database-remove-empty-filegroup: pass, database-remove-filegroup: pass |
| AlterDatabaseRemoveFileStatement | default-parser-sql | live-pass | database-remove-file: pass |
| AlterDatabaseScopedConfigurationClearStatement | default-parser-sql | live-pass | expanded-database-scoped-cache-clear: pass |
| AlterDatabaseScopedConfigurationSetStatement | default-parser-sql | live-pass | expanded-database-scoped-configuration-set: pass |
| AlterDatabaseSetStatement | default-parser-sql | live-pass | expanded-database-set-current: pass, database-set-options: pass |
| AlterEndpointStatement | default-parser-sql | live-pass | server-alter-endpoint: pass |
| AlterEventSessionStatement | default-parser-sql | live-pass | server-alter-events: pass |
| AlterExternalDataSourceStatement | default-parser-sql | live-pass | external-source-alter: pass |
| AlterExternalFunctionStatement | alternate-dialect-sql | not-live-tested | ; SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| AlterExternalLanguageStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| AlterExternalLibraryStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| AlterExternalModelStatement | default-parser-sql | not-live-tested | ; Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| AlterExternalResourcePoolStatement | default-parser-sql | live-pass | server-alter-external-pool: pass |
| AlterFederationStatement | default-parser-sql | not-live-tested | ; Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| AlterFullTextCatalogStatement | default-parser-sql | live-pass | expanded-alter-fulltext-catalog: pass |
| AlterFullTextIndexStatement | default-parser-sql | live-pass | expanded-alter-fulltext-index: pass |
| AlterFullTextStopListStatement | default-parser-sql | live-pass | expanded-alter-fulltext-stoplist: pass |
| AlterFunctionStatement | default-parser-sql | live-pass | alter-function: pass |
| AlterIndexStatement | default-parser-sql | live-pass | alter-index: pass |
| AlterLoginAddDropCredentialStatement | default-parser-sql | live-pass | server-login-credential: pass |
| AlterLoginEnableDisableStatement | default-parser-sql | live-pass | server-alter-login-disable: pass |
| AlterLoginOptionsStatement | default-parser-sql | live-pass | server-alter-login-options: pass |
| AlterMasterKeyStatement | default-parser-sql | live-pass | expanded-alter-master-key: pass |
| AlterMessageTypeStatement | default-parser-sql | live-pass | expanded-alter-message-type: pass |
| AlterPartitionFunctionStatement | default-parser-sql | live-pass | alter-partition-function: pass |
| AlterPartitionSchemeStatement | default-parser-sql | live-pass | expanded-alter-partition-scheme: pass |
| AlterProcedureStatement | default-parser-sql | live-pass | alter-procedure: pass |
| AlterQueueStatement | default-parser-sql | live-pass | expanded-alter-queue: pass |
| AlterRemoteServiceBindingStatement | default-parser-sql | live-pass | expanded-alter-remote-service-binding: pass |
| AlterResourceGovernorStatement | default-parser-sql | live-pass | server-governor-reconfigure: execution-error-or-assertion-failure, server-governor-reconfigure: pass |
| AlterResourcePoolStatement | default-parser-sql | live-pass | server-alter-pool: pass |
| AlterRoleStatement | default-parser-sql | live-pass | alter-role: pass |
| AlterRouteStatement | default-parser-sql | live-pass | expanded-alter-local-route: pass |
| AlterSchemaStatement | default-parser-sql | live-pass | alter-schema: pass |
| AlterSearchPropertyListStatement | default-parser-sql | live-pass | expanded-alter-search-property-list: pass |
| AlterSecurityPolicyStatement | default-parser-sql | live-pass | expanded-alter-security-policy: pass |
| AlterSequenceStatement | default-parser-sql | live-pass | alter-sequence: pass |
| AlterServerAuditSpecificationStatement | default-parser-sql | live-pass | server-alter-audit-spec: pass |
| AlterServerAuditStatement | default-parser-sql | live-pass | server-alter-audit: pass |
| AlterServerConfigurationSetBufferPoolExtensionStatement | default-parser-sql | not-live-tested | ; Requires supported buffer-pool-extension storage/platform plus file-size and memory prerequisites, not provisioned in the Linux test container. |
| AlterServerConfigurationSetDiagnosticsLogStatement | default-parser-sql | not-live-tested | ; Requires supported diagnostic-log platform/configuration; no Windows SQL Server diagnostic-file environment was provisioned. |
| AlterServerConfigurationSetExternalAuthenticationStatement | default-parser-sql | not-live-tested | ; Requires configured external identity provider/authentication environment; intentionally not pointed at user identities or shared server authentication. |
| AlterServerConfigurationSetFailoverClusterPropertyStatement | default-parser-sql | not-live-tested | ; Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterServerConfigurationSetHadrClusterStatement | default-parser-sql | not-live-tested | ; Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterServerConfigurationSetSoftNumaStatement | default-parser-sql | live-attempt-failed | server-softnuma: execution-error-or-assertion-failure; InvalidDataException: Expected OFF -> ON, got "OFF" -> "OFF"; verdict RealWrite, expected RealWrite. |
| AlterServerConfigurationStatement | default-parser-sql | live-pass | server-affinity: pass |
| AlterServerRoleStatement | default-parser-sql | live-pass | server-alter-role: pass |
| AlterServiceMasterKeyStatement | default-parser-sql | not-live-tested | ; Regeneration requires an observer of key material/re-encryption rather than unchanged key count; the current scalar runner cannot establish that effect. Not claimed as a live pass. |
| AlterServiceStatement | default-parser-sql | live-pass | expanded-alter-service: pass |
| AlterSymmetricKeyStatement | default-parser-sql | live-pass | expanded-alter-symmetric-key: pass |
| AlterTableAddClusterByStatement | alternate-dialect-sql | not-live-tested | ; SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| AlterTableAddTableElementStatement | default-parser-sql | live-pass | alter-table-add: pass |
| AlterTableAlterColumnStatement | default-parser-sql | live-pass | alter-table-column: pass |
| AlterTableAlterIndexStatement | default-parser-sql | not-live-tested | ; Requires memory-optimized/native-compilation fixtures and storage. Atomic body must actually execute, not merely appear in a CREATE definition; no such runtime evidence recorded. |
| AlterTableAlterPartitionStatement | default-parser-sql | not-live-tested | ; Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| AlterTableChangeTrackingModificationStatement | default-parser-sql | live-pass | expanded-table-change-tracking: pass |
| AlterTableConstraintModificationStatement | default-parser-sql | live-pass | expanded-table-constraint-modification: pass |
| AlterTableDropTableElementStatement | default-parser-sql | live-pass | alter-table-drop: pass |
| AlterTableFileTableNamespaceStatement | default-parser-sql | not-live-tested | ; Requires a real FileTable and FILESTREAM-enabled supported server/storage; not available in the disposable Linux fixture environment. |
| AlterTableRebuildStatement | default-parser-sql | live-pass | expanded-table-rebuild-rollback: pass |
| AlterTableSetStatement | default-parser-sql | live-pass | expanded-table-set-lock-escalation: pass |
| AlterTableSwitchStatement | default-parser-sql | live-pass | expanded-table-switch: pass |
| AlterTableTriggerModificationStatement | default-parser-sql | live-pass | expanded-table-trigger-modification: pass |
| AlterTriggerStatement | default-parser-sql | live-pass | alter-trigger: pass |
| AlterUserStatement | default-parser-sql | live-pass | alter-user: pass |
| AlterViewStatement | default-parser-sql | live-pass | alter-view: pass |
| AlterWorkloadGroupStatement | default-parser-sql | live-pass | workload-alter: pass |
| AlterXmlSchemaCollectionStatement | default-parser-sql | live-pass | expanded-alter-xml-schema-collection: pass |
| BackupCertificateStatement | default-parser-sql | live-pass | key-backup-certificate: pass |
| BackupDatabaseStatement | default-parser-sql | live-pass | database-backup: pass |
| BackupMasterKeyStatement | default-parser-sql | live-pass | key-backup-master: pass |
| BackupServiceMasterKeyStatement | default-parser-sql | live-pass | key-backup-service-master: pass |
| BackupTransactionLogStatement | default-parser-sql | live-pass | database-log-backup: pass |
| BeginConversationTimerStatement | default-parser-sql | live-pass | expanded-begin-conversation-timer: pass |
| BeginDialogStatement | default-parser-sql | live-pass | expanded-begin-dialog: pass |
| BeginEndAtomicBlockStatement | default-parser-sql | not-live-tested | ; Requires memory-optimized/native-compilation fixtures and storage. Atomic body must actually execute, not merely appear in a CREATE definition; no such runtime evidence recorded. |
| BeginEndBlockStatement | default-parser-sql | live-pass | while-control-flow: pass |
| BeginTransactionStatement | default-parser-sql | live-pass | transaction-balanced: pass, transaction-rollback: pass, write-rollback: pass, write-uncommitted: pass, savepoint-rollback: pass, expanded-table-rebuild-rollback: pass |
| BreakStatement | default-parser-sql | live-pass | while-control-flow: pass |
| BulkInsertStatement | default-parser-sql | live-pass | bulk-insert-owned-file: pass |
| CheckpointStatement | default-parser-sql | live-pass | expanded-database-checkpoint: pass |
| CloseCursorStatement | default-parser-sql | live-pass | cursor-lifecycle: pass |
| CloseMasterKeyStatement | default-parser-sql | live-pass | expanded-alter-master-key: pass, expanded-close-master-key: pass |
| CloseSymmetricKeyStatement | default-parser-sql | live-pass | expanded-alter-symmetric-key: pass, expanded-close-symmetric-key: pass |
| CommitTransactionStatement | default-parser-sql | live-pass | transaction-balanced: pass, savepoint-rollback: pass |
| ContinueStatement | default-parser-sql | live-pass | while-control-flow: pass |
| CopyStatement | default-parser-sql | not-live-tested | ; Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| CreateAggregateStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| CreateApplicationRoleStatement | default-parser-sql | live-pass | create-application-role: pass |
| CreateAssemblyStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| CreateAsymmetricKeyStatement | default-parser-sql | live-pass | expanded-create-asymmetric-key: pass |
| CreateAvailabilityGroupStatement | default-parser-sql | not-live-tested | ; Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| CreateBrokerPriorityStatement | default-parser-sql | live-pass | expanded-create-broker-priority: pass |
| CreateCertificateStatement | default-parser-sql | live-pass | expanded-create-certificate: pass |
| CreateColumnEncryptionKeyStatement | default-parser-sql | live-pass | cek-create: pass |
| CreateColumnMasterKeyStatement | default-parser-sql | live-pass | cmk-create: pass |
| CreateColumnStoreIndexStatement | default-parser-sql | live-pass | expanded-create-columnstore-index: pass, expanded-create-clustered-columnstore-index: pass |
| CreateContractStatement | default-parser-sql | live-pass | expanded-create-contract: pass |
| CreateCredentialStatement | default-parser-sql | live-pass | server-create-credential: pass |
| CreateCryptographicProviderStatement | default-parser-sql | not-live-tested | ; Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| CreateDatabaseAuditSpecificationStatement | default-parser-sql | live-pass | db-audit-create: pass |
| CreateDatabaseEncryptionKeyStatement | default-parser-sql | live-pass | tde-create: pass |
| CreateDatabaseStatement | default-parser-sql | live-pass | database-create: pass |
| CreateDefaultStatement | default-parser-sql | live-pass | create-default: pass |
| CreateEndpointStatement | default-parser-sql | live-pass | server-create-endpoint: pass |
| CreateEventNotificationStatement | default-parser-sql | live-pass | expanded-create-event-notification: pass |
| CreateEventSessionStatement | default-parser-sql | live-pass | server-create-events: pass |
| CreateExternalDataSourceStatement | default-parser-sql | live-pass | external-source-create: pass |
| CreateExternalFileFormatStatement | default-parser-sql | live-pass | expanded-create-external-file-format: pass |
| CreateExternalFunctionStatement | alternate-dialect-sql | not-live-tested | ; SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| CreateExternalLanguageStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| CreateExternalLibraryStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| CreateExternalModelStatement | default-parser-sql | not-live-tested | ; Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| CreateExternalResourcePoolStatement | default-parser-sql | live-pass | server-create-external-pool: pass |
| CreateExternalStreamingJobStatement | default-parser-sql | not-live-tested | ; Requires a compatible streaming engine and actual stream/job resource prerequisites. The grammar conversion is statically tested; no running streaming engine was provisioned. |
| CreateExternalStreamStatement | default-parser-sql | not-live-tested | ; Requires a compatible streaming engine and actual stream/job resource prerequisites. The grammar conversion is statically tested; no running streaming engine was provisioned. |
| CreateExternalTableStatement | default-parser-sql | live-attempt-failed | external-table-metadata-create: execution-error-or-assertion-failure; SqlException: 'sqlserver://example.invalid:1433' contains an unsupported connector location prefix. Refer to product documentation for a list of supported connector location prefixes. |
| CreateFederationStatement | default-parser-sql | not-live-tested | ; Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| CreateFullTextCatalogStatement | default-parser-sql | live-pass | expanded-create-fulltext-catalog: pass |
| CreateFullTextIndexStatement | default-parser-sql | live-pass | expanded-create-fulltext-index: pass |
| CreateFullTextStopListStatement | default-parser-sql | live-pass | expanded-create-fulltext-stoplist: pass |
| CreateFunctionStatement | default-parser-sql | live-pass | create-function: pass |
| CreateIndexStatement | default-parser-sql | live-pass | create-index: pass |
| CreateJsonIndexStatement | default-parser-sql | live-pass | expanded-create-json-index: pass |
| CreateLoginStatement | default-parser-sql | live-pass | server-create-login: pass |
| CreateMasterKeyStatement | default-parser-sql | live-pass | expanded-create-master-key: pass |
| CreateMessageTypeStatement | default-parser-sql | live-pass | expanded-create-message-type: pass |
| CreateOrAlterExternalFunctionStatement | alternate-dialect-sql | not-live-tested | ; SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| CreateOrAlterFunctionStatement | default-parser-sql | live-pass | create-or-alter-function: pass |
| CreateOrAlterProcedureStatement | default-parser-sql | live-pass | create-or-alter-procedure: pass |
| CreateOrAlterTriggerStatement | default-parser-sql | live-pass | create-or-alter-trigger: pass |
| CreateOrAlterViewStatement | default-parser-sql | live-pass | create-or-alter-view: pass |
| CreatePartitionFunctionStatement | default-parser-sql | live-pass | create-partition-function: pass |
| CreatePartitionSchemeStatement | default-parser-sql | live-pass | create-partition-scheme: pass |
| CreateProcedureStatement | default-parser-sql | live-pass | create-temp-procedure: pass, create-procedure: pass |
| CreateQueueStatement | default-parser-sql | live-pass | expanded-create-queue: pass |
| CreateRemoteServiceBindingStatement | default-parser-sql | live-pass | expanded-create-remote-service-binding: pass |
| CreateResourcePoolStatement | default-parser-sql | live-pass | server-create-pool: pass |
| CreateRoleStatement | default-parser-sql | live-pass | create-role: pass |
| CreateRouteStatement | default-parser-sql | live-pass | expanded-create-local-route: pass |
| CreateRuleStatement | default-parser-sql | live-pass | create-rule: pass |
| CreateSchemaStatement | default-parser-sql | live-pass | create-schema: pass |
| CreateSearchPropertyListStatement | default-parser-sql | live-pass | expanded-create-search-property-list: pass |
| CreateSecurityPolicyStatement | default-parser-sql | live-pass | expanded-create-security-policy: pass |
| CreateSelectiveXmlIndexStatement | default-parser-sql | live-pass | expanded-create-selective-xml-index: pass |
| CreateSemanticIndexStatement | default-parser-sql | not-live-tested | ; Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| CreateSequenceStatement | default-parser-sql | live-pass | create-sequence: pass |
| CreateServerAuditSpecificationStatement | default-parser-sql | live-pass | server-create-audit-spec: pass |
| CreateServerAuditStatement | default-parser-sql | live-pass | server-create-audit: pass |
| CreateServerRoleStatement | default-parser-sql | live-pass | server-create-role: pass |
| CreateServiceStatement | default-parser-sql | live-pass | expanded-create-service: pass |
| CreateSpatialIndexStatement | default-parser-sql | live-pass | expanded-create-spatial-index: pass |
| CreateStatisticsStatement | default-parser-sql | live-pass | create-statistics: pass |
| CreateSymmetricKeyStatement | default-parser-sql | live-pass | expanded-create-symmetric-key: pass |
| CreateSynonymStatement | default-parser-sql | live-pass | create-synonym: pass |
| CreateTableStatement | default-parser-sql | live-pass | create-table: pass, expanded-inline-index-definition: pass |
| CreateTriggerStatement | default-parser-sql | live-pass | create-trigger: pass |
| CreateTypeTableStatement | default-parser-sql | live-pass | create-type-table: pass |
| CreateTypeUddtStatement | default-parser-sql | live-pass | create-type-alias: pass |
| CreateTypeUdtStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| CreateUserStatement | default-parser-sql | live-pass | create-user: pass |
| CreateVectorIndexStatement | default-parser-sql | live-pass | expanded-create-vector-index: pass |
| CreateViewStatement | default-parser-sql | live-pass | create-view: pass |
| CreateWorkloadClassifierStatement | default-parser-sql | not-live-tested | ; Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| CreateWorkloadGroupStatement | default-parser-sql | live-pass | workload-create: pass |
| CreateXmlIndexStatement | default-parser-sql | live-pass | expanded-create-primary-xml-index: pass, expanded-create-secondary-xml-index: pass |
| CreateXmlSchemaCollectionStatement | default-parser-sql | live-pass | create-xml-schema: pass |
| DbccStatement | default-parser-sql | live-pass | dbcc-checkident: pass, dbcc-reseed: pass, dbcc-checktable: pass, dbcc-useroptions: pass |
| DeallocateCursorStatement | default-parser-sql | live-pass | cursor-lifecycle: pass |
| DeclareCursorStatement | default-parser-sql | live-pass | cursor-lifecycle: pass |
| DeclareTableVariableStatement | default-parser-sql | live-pass | table-variable: pass |
| DeclareVariableStatement | default-parser-sql | live-pass | dynamic-runtime-write: pass, while-control-flow: pass, cursor-lifecycle: pass, expanded-begin-dialog: pass, expanded-begin-conversation-timer: pass, expanded-move-conversation: pass, expanded-end-conversation: pass, expanded-send-local-message: pass, expanded-get-empty-conversation-group: pass |
| DeleteStatement | default-parser-sql | live-pass | cte-delete: pass, delete-no-where: pass, delete-temp: pass |
| DenyStatement | default-parser-sql | live-pass | deny-select: pass |
| DenyStatement80 | alternate-dialect-sql | not-live-tested | ; Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| DiskStatement | alternate-dialect-sql | not-live-tested | ; Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| DropAggregateStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| DropApplicationRoleStatement | default-parser-sql | live-pass | drop-application-role: pass |
| DropAssemblyStatement | default-parser-sql | not-live-tested | ; No reviewed SQL CLR binary/UDT/aggregate and CLR execution environment was provisioned. Parser verdict tested; no assembly installation or execution claimed. |
| DropAsymmetricKeyStatement | default-parser-sql | live-pass | expanded-drop-asymmetric-key: pass |
| DropAvailabilityGroupStatement | default-parser-sql | not-live-tested | ; Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| DropBrokerPriorityStatement | default-parser-sql | live-pass | expanded-drop-broker-priority: pass |
| DropCertificateStatement | default-parser-sql | live-pass | expanded-drop-certificate: pass |
| DropColumnEncryptionKeyStatement | default-parser-sql | live-pass | cek-drop: pass |
| DropColumnMasterKeyStatement | default-parser-sql | live-pass | cmk-drop: pass |
| DropContractStatement | default-parser-sql | live-pass | expanded-drop-contract: pass |
| DropCredentialStatement | default-parser-sql | live-pass | server-drop-credential: pass |
| DropCryptographicProviderStatement | default-parser-sql | not-live-tested | ; Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| DropDatabaseAuditSpecificationStatement | default-parser-sql | live-pass | db-audit-drop: pass |
| DropDatabaseEncryptionKeyStatement | default-parser-sql | live-pass | tde-drop: pass |
| DropDatabaseStatement | default-parser-sql | live-pass | database-drop: pass |
| DropDefaultStatement | default-parser-sql | live-pass | drop-default: pass |
| DropEndpointStatement | default-parser-sql | live-pass | server-drop-endpoint: pass |
| DropEventNotificationStatement | default-parser-sql | live-pass | expanded-drop-event-notification: pass |
| DropEventSessionStatement | default-parser-sql | live-pass | server-drop-events: pass |
| DropExternalDataSourceStatement | default-parser-sql | live-pass | external-source-drop: pass |
| DropExternalFileFormatStatement | default-parser-sql | live-pass | expanded-drop-external-file-format: pass |
| DropExternalLanguageStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| DropExternalLibraryStatement | default-parser-sql | not-live-tested | ; Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| DropExternalModelStatement | default-parser-sql | not-live-tested | ; Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| DropExternalResourcePoolStatement | default-parser-sql | live-pass | server-drop-external-pool: pass |
| DropExternalStreamingJobStatement | constructed-ast-only | not-live-tested | ; Generator AST node; sp_drop_streaming_job parses as ExecuteStatement, not this node. Constructed AST fallback is tested; not executable SQL coverage. |
| DropExternalStreamStatement | constructed-ast-only | not-live-tested | ; Generator AST node; pinned parser has no DROP EXTERNAL STREAM production. Constructed AST fallback is tested; not executable SQL coverage. |
| DropExternalTableStatement | default-parser-sql | live-attempt-failed | external-table-metadata-drop: execution-error-or-assertion-failure; SqlException: 'sqlserver://example.invalid:1433' contains an unsupported connector location prefix. Refer to product documentation for a list of supported connector location prefixes. |
| DropFederationStatement | default-parser-sql | not-live-tested | ; Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| DropFullTextCatalogStatement | default-parser-sql | live-pass | expanded-drop-fulltext-catalog: pass |
| DropFullTextIndexStatement | default-parser-sql | live-pass | expanded-drop-fulltext-index: pass |
| DropFullTextStopListStatement | default-parser-sql | live-pass | expanded-drop-fulltext-stoplist: pass |
| DropFunctionStatement | default-parser-sql | live-pass | drop-function: pass |
| DropIndexStatement | default-parser-sql | live-pass | drop-index: pass |
| DropLoginStatement | default-parser-sql | live-pass | server-drop-login: pass |
| DropMasterKeyStatement | default-parser-sql | live-pass | expanded-drop-master-key: pass |
| DropMessageTypeStatement | default-parser-sql | live-pass | expanded-drop-message-type: pass |
| DropPartitionFunctionStatement | default-parser-sql | live-pass | drop-partition-function: pass |
| DropPartitionSchemeStatement | default-parser-sql | live-pass | drop-partition-scheme: pass |
| DropProcedureStatement | default-parser-sql | live-pass | drop-procedure: pass |
| DropQueueStatement | default-parser-sql | live-pass | expanded-drop-queue: pass |
| DropRemoteServiceBindingStatement | default-parser-sql | live-pass | expanded-drop-remote-service-binding: pass |
| DropResourcePoolStatement | default-parser-sql | live-pass | server-drop-pool: pass |
| DropRoleStatement | default-parser-sql | live-pass | drop-role: pass |
| DropRouteStatement | default-parser-sql | live-pass | expanded-drop-local-route: pass |
| DropRuleStatement | default-parser-sql | live-pass | drop-rule: pass |
| DropSchemaStatement | default-parser-sql | live-pass | drop-schema: pass |
| DropSearchPropertyListStatement | default-parser-sql | live-pass | expanded-drop-search-property-list: pass |
| DropSecurityPolicyStatement | default-parser-sql | live-pass | expanded-drop-security-policy: pass |
| DropSensitivityClassificationStatement | default-parser-sql | live-pass | expanded-drop-sensitivity-classification: pass |
| DropSequenceStatement | default-parser-sql | live-pass | drop-sequence: pass |
| DropServerAuditSpecificationStatement | default-parser-sql | live-pass | server-drop-audit-spec: pass |
| DropServerAuditStatement | default-parser-sql | live-pass | server-drop-audit: pass |
| DropServerRoleStatement | default-parser-sql | live-pass | server-drop-role: pass |
| DropServiceStatement | default-parser-sql | live-pass | expanded-drop-service: pass |
| DropSignatureStatement | default-parser-sql | live-pass | expanded-drop-signature: pass |
| DropStatisticsStatement | default-parser-sql | live-pass | drop-statistics: pass |
| DropSymmetricKeyStatement | default-parser-sql | live-pass | expanded-drop-symmetric-key: pass |
| DropSynonymStatement | default-parser-sql | live-pass | drop-synonym: pass |
| DropTableStatement | default-parser-sql | live-pass | drop-table: pass |
| DropTriggerStatement | default-parser-sql | live-pass | drop-trigger: pass |
| DropTypeStatement | default-parser-sql | live-pass | drop-type: pass |
| DropUserStatement | default-parser-sql | live-pass | drop-user: pass |
| DropViewStatement | default-parser-sql | live-pass | drop-view: pass |
| DropWorkloadClassifierStatement | default-parser-sql | not-live-tested | ; Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| DropWorkloadGroupStatement | default-parser-sql | live-pass | workload-drop: pass |
| DropXmlSchemaCollectionStatement | default-parser-sql | live-pass | drop-xml-schema: pass |
| EnableDisableTriggerStatement | default-parser-sql | live-pass | disable-trigger: pass |
| EndConversationStatement | default-parser-sql | live-pass | expanded-end-conversation: pass |
| EventSessionStatement | constructed-ast-only | not-live-tested | ; Concrete shared AST base; actual SQL produces create/alter event-session subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| ExecuteAsStatement | default-parser-sql | live-pass | execute-as-revert: pass |
| ExecuteStatement | default-parser-sql | live-pass | execute-procedure: pass, dynamic-literal-write: pass, dynamic-literal-read: pass, dynamic-runtime-write: pass, procedure-hidden-write: pass |
| ExternalResourcePoolStatement | constructed-ast-only | not-live-tested | ; Concrete shared AST base; actual SQL produces create/alter external-resource-pool subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| FetchCursorStatement | default-parser-sql | live-pass | cursor-lifecycle: pass |
| GetConversationGroupStatement | default-parser-sql | live-pass | expanded-get-empty-conversation-group: pass |
| GoToStatement | default-parser-sql | live-pass | goto-label: pass |
| GrantStatement | default-parser-sql | live-pass | grant-select: pass |
| GrantStatement80 | alternate-dialect-sql | not-live-tested | ; Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| IfStatement | default-parser-sql | live-pass | if-unreachable-write: pass, if-reachable-write: pass |
| IndexDefinition | default-parser-sql | live-pass | expanded-inline-index-definition: pass |
| InsertBulkStatement | default-parser-sql | not-live-tested | ; INSERT BULK uses the TDS bulk-copy protocol after its SQL header; an ordinary ExecuteReader batch is not an adequate bulk-stream test. No protocol-level execution claimed. |
| InsertStatement | default-parser-sql | live-pass | temp-insert: pass, composable-delete: pass, insert-real: pass, table-variable: pass |
| KillQueryNotificationSubscriptionStatement | default-parser-sql | not-live-tested | ; Requires an owned live query-notification subscription and verified subscription identifier. None provisioned; arbitrary IDs would test only failure paths. |
| KillStatement | default-parser-sql | live-pass | owned-session-kill: pass |
| KillStatsJobStatement | default-parser-sql | not-live-tested | ; Requires a compatible warehouse statistics job and genuine owned job identifier; no such job backend is provisioned. |
| LabelStatement | default-parser-sql | live-pass | goto-label: pass |
| LineNoStatement | default-parser-sql | live-pass | expanded-session-lineno-reset: pass |
| MergeStatement | default-parser-sql | live-pass | merge-real: pass, merge-temp: pass |
| MoveConversationStatement | default-parser-sql | live-pass | expanded-move-conversation: pass |
| OpenCursorStatement | default-parser-sql | live-pass | cursor-lifecycle: pass |
| OpenMasterKeyStatement | default-parser-sql | live-pass | expanded-alter-master-key: pass, expanded-open-master-key: pass |
| OpenSymmetricKeyStatement | default-parser-sql | live-pass | expanded-alter-symmetric-key: pass, expanded-open-symmetric-key: pass |
| PredicateSetStatement | default-parser-sql | live-pass | session-nocount: pass |
| PrintStatement | default-parser-sql | live-pass | print-read: pass |
| RaiseErrorLegacyStatement | alternate-dialect-sql | not-live-tested | ; Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| RaiseErrorStatement | default-parser-sql | live-pass | raiseerror-caught: pass |
| ReadTextStatement | default-parser-sql | live-pass | read-text: pass |
| ReceiveStatement | default-parser-sql | live-pass | expanded-receive-empty-queue: pass |
| ReconfigureStatement | default-parser-sql | live-pass | server-reconfigure: pass |
| RenameEntityStatement | default-parser-sql | not-live-tested | ; Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| ResourcePoolStatement | constructed-ast-only | not-live-tested | ; Concrete shared AST base; actual SQL produces create/alter resource-pool subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| RestoreMasterKeyStatement | default-parser-sql | live-pass | key-restore-master: pass |
| RestoreServiceMasterKeyStatement | default-parser-sql | live-pass | key-restore-service-master: pass |
| RestoreStatement | default-parser-sql | live-pass | database-restore: pass |
| ReturnStatement | default-parser-sql | live-pass | return-read: pass |
| RevertStatement | default-parser-sql | live-pass | execute-as-revert: pass |
| RevokeStatement | default-parser-sql | live-pass | revoke-select: pass |
| RevokeStatement80 | alternate-dialect-sql | not-live-tested | ; Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| RollbackTransactionStatement | default-parser-sql | live-pass | transaction-rollback: pass, write-rollback: pass, savepoint-rollback: pass, expanded-table-rebuild-rollback: pass |
| SaveTransactionStatement | default-parser-sql | live-pass | savepoint-rollback: pass |
| SelectStatement | default-parser-sql | live-pass | read-select: pass, read-join-window: pass, read-select-into-real: pass, read-select-into-temp: pass, sequence-select: pass, try-catch-read: pass, expanded-get-empty-conversation-group: pass |
| SelectStatementSnippet | constructed-ast-only | not-live-tested | ; Programmatically constructed SELECT snippet, not a standalone parser statement kind. Constructed AST fallback is tested; not executable SQL coverage. |
| SendStatement | default-parser-sql | live-pass | expanded-send-local-message: pass |
| SetCommandStatement | default-parser-sql | live-pass | expanded-set-command-lock-timeout: pass |
| SetErrorLevelStatement | default-parser-sql | not-live-tested | ; Legacy/internal SET ERRLVL grammar has no supported observable public session property in the SqlClient runner. No-op success would not establish its intended effect. |
| SetIdentityInsertStatement | default-parser-sql | live-pass | expanded-set-identity-insert: pass |
| SetOffsetsStatement | default-parser-sql | live-pass | expanded-session-offsets-reset: pass |
| SetRowCountStatement | default-parser-sql | live-pass | session-rowcount: pass |
| SetStatisticsStatement | default-parser-sql | live-pass | session-statistics: pass |
| SetTextSizeStatement | default-parser-sql | live-pass | session-textsize: pass |
| SetTransactionIsolationLevelStatement | default-parser-sql | live-pass | session-isolation: pass |
| SetUserStatement | default-parser-sql | live-pass | session-setuser: pass |
| SetVariableStatement | default-parser-sql | live-pass | while-control-flow: pass |
| ShutdownStatement | default-parser-sql | live-pass | owned-server-shutdown: pass |
| ThrowStatement | default-parser-sql | live-pass | throw-caught: pass |
| TruncateTableStatement | default-parser-sql | live-pass | truncate-table: pass |
| TryCatchStatement | default-parser-sql | live-pass | try-catch-read: pass, throw-caught: pass, raiseerror-caught: pass |
| TSqlStatementSnippet | constructed-ast-only | not-live-tested | ; Programmatically constructed raw statement snippet; not a parser production. Constructed AST fallback is tested; not executable SQL coverage. |
| UpdateStatement | default-parser-sql | live-pass | alias-update: pass, update-no-where: pass, update-temp: pass, alias-temp-looking-real: pass, output-into-real: pass, output-into-temp: pass, trigger-hidden-write: pass, view-target-update: pass, write-rollback: pass, write-uncommitted: pass, savepoint-rollback: pass, if-unreachable-write: pass, if-reachable-write: pass |
| UpdateStatisticsStatement | default-parser-sql | live-pass | expanded-update-statistics: pass |
| UpdateTextStatement | default-parser-sql | live-pass | update-text: pass |
| UseFederationStatement | default-parser-sql | not-live-tested | ; Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| UseStatement | default-parser-sql | live-pass | session-use: pass |
| WaitForStatement | default-parser-sql | live-pass | waitfor-zero: pass |
| WhileStatement | default-parser-sql | live-pass | while-control-flow: pass |
| WriteTextStatement | default-parser-sql | live-pass | write-text: pass |
