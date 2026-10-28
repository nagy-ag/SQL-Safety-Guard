# Wave-2 disposition of all 67 original live gaps

Original matrix is immutable historical evidence. The isolated Developer feature run `wave2-features-final/isolated-results.json` has 15/15 passing fixtures, closing 12 original gaps; 55 remain open. A missing prerequisite is **not** proof a statement cannot run. No fabricated binary, endpoint, key-count equality or CREATE-only atomic body is accepted as execution proof.

| Kind | Wave-2 status | Evidence / exact remaining prerequisite |
| --- | --- | --- |
| AlterAssemblyStatement | live-pass | `clr-alter`; final isolated run, exact scalar/change assertion. |
| AlterAvailabilityGroupStatement | open; not reattempted | Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterCryptographicProviderStatement | open; not reattempted | Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| AlterDatabasePerformCutoverStatement | open; not reattempted | Requires a supported Azure database copy/cutover topology, absent from the disposable standalone SQL Server. |
| AlterDatabaseRebuildLogStatement | open; not reattempted | Requires deliberately damaged/offline database/log files and a recovery-specific runner; normal disposable database observations do not prove log reconstruction. |
| AlterExternalFunctionStatement | open; not reattempted | SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| AlterExternalLanguageStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| AlterExternalLibraryStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| AlterExternalModelStatement | open; not reattempted | Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| AlterFederationStatement | open; not reattempted | Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| AlterServerConfigurationSetBufferPoolExtensionStatement | open; not reattempted | Requires supported buffer-pool-extension storage/platform plus file-size and memory prerequisites, not provisioned in the Linux test container. |
| AlterServerConfigurationSetDiagnosticsLogStatement | open; not reattempted | Requires supported diagnostic-log platform/configuration; no Windows SQL Server diagnostic-file environment was provisioned. |
| AlterServerConfigurationSetExternalAuthenticationStatement | open; not reattempted | Requires configured external identity provider/authentication environment; intentionally not pointed at user identities or shared server authentication. |
| AlterServerConfigurationSetFailoverClusterPropertyStatement | open; not reattempted | Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterServerConfigurationSetHadrClusterStatement | open; not reattempted | Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| AlterServerConfigurationSetSoftNumaStatement | open; not reattempted | Original failed OFF -> OFF assertion reads effective state, while configuration may require restart. A restart-aware configured/in-use observer was not implemented; no new pass claimed. |
| AlterServiceMasterKeyStatement | live-pass | `service-master-regenerate`; final isolated run, exact scalar/change assertion. |
| AlterTableAddClusterByStatement | open; not reattempted | SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| AlterTableAlterIndexStatement | live-pass | `memory-index`; final isolated run, exact scalar/change assertion. |
| AlterTableAlterPartitionStatement | open; not reattempted | Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| AlterTableFileTableNamespaceStatement | open; not reattempted | Requires a real FileTable and FILESTREAM-enabled supported server/storage; not available in the disposable Linux fixture environment. |
| BeginEndAtomicBlockStatement | live-pass | `memory-atomic-execute`; final isolated run, exact scalar/change assertion. |
| CopyStatement | open; not reattempted | Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| CreateAggregateStatement | live-pass | `clr-aggregate-create`; final isolated run, exact scalar/change assertion. |
| CreateAssemblyStatement | live-pass | `clr-create`; final isolated run, exact scalar/change assertion. |
| CreateAvailabilityGroupStatement | open; not reattempted | Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| CreateCryptographicProviderStatement | open; not reattempted | Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| CreateExternalFunctionStatement | open; not reattempted | SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| CreateExternalLanguageStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| CreateExternalLibraryStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| CreateExternalModelStatement | open; not reattempted | Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| CreateExternalStreamingJobStatement | open; not reattempted | Requires a compatible streaming engine and actual stream/job resource prerequisites. The grammar conversion is statically tested; no running streaming engine was provisioned. |
| CreateExternalStreamStatement | open; not reattempted | Requires a compatible streaming engine and actual stream/job resource prerequisites. The grammar conversion is statically tested; no running streaming engine was provisioned. |
| CreateExternalTableStatement | live-pass | `external-table-create`; final isolated run, exact scalar/change assertion. |
| CreateFederationStatement | open; not reattempted | Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| CreateOrAlterExternalFunctionStatement | open; not reattempted | SQL uses the Fabric DW grammar; the production TSql180 parser behavior is separately asserted. No Fabric warehouse is connected for live execution. |
| CreateSemanticIndexStatement | open; not reattempted | Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| CreateTypeUdtStatement | live-pass | `clr-type-create`; final isolated run, exact scalar/change assertion. |
| CreateWorkloadClassifierStatement | open; not reattempted | Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| DenyStatement80 | open; not reattempted | Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| DiskStatement | open; not reattempted | Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| DropAggregateStatement | live-pass | `clr-aggregate-drop`; final isolated run, exact scalar/change assertion. |
| DropAssemblyStatement | live-pass | `clr-drop`; final isolated run, exact scalar/change assertion. |
| DropAvailabilityGroupStatement | open; not reattempted | Requires a configured availability/cluster environment; the disposable single Linux SQL Server is not such an environment. Never run against the shared developer server. |
| DropCryptographicProviderStatement | open; not reattempted | Requires a genuine installed EKM cryptographic provider and provider configuration; fabricated file paths are not executable evidence. |
| DropExternalLanguageStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| DropExternalLibraryStatement | open; not reattempted | Requires genuine reviewed external-language runtime/library packages and enabled extensibility services; none provisioned. No dummy executable package is treated as a pass. |
| DropExternalModelStatement | open; not reattempted | Requires compatible preview engine/features and a genuine external model endpoint/credentials. No external resource is contacted or invented. |
| DropExternalStreamingJobStatement | open; not reattempted | Generator AST node; sp_drop_streaming_job parses as ExecuteStatement, not this node. Constructed AST fallback is tested; not executable SQL coverage. |
| DropExternalStreamStatement | open; not reattempted | Generator AST node; pinned parser has no DROP EXTERNAL STREAM production. Constructed AST fallback is tested; not executable SQL coverage. |
| DropExternalTableStatement | live-pass | `external-table-drop`; final isolated run, exact scalar/change assertion. |
| DropFederationStatement | open; not reattempted | Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |
| DropWorkloadClassifierStatement | open; not reattempted | Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| EventSessionStatement | open; not reattempted | Concrete shared AST base; actual SQL produces create/alter event-session subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| ExternalResourcePoolStatement | open; not reattempted | Concrete shared AST base; actual SQL produces create/alter external-resource-pool subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| GrantStatement80 | open; not reattempted | Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| InsertBulkStatement | live-pass | `protocol-insert-bulk`; final isolated run, exact scalar/change assertion. |
| KillQueryNotificationSubscriptionStatement | open; not reattempted | Requires an owned live query-notification subscription and verified subscription identifier. None provisioned; arbitrary IDs would test only failure paths. An owned query-notification subscription fixture remains feasible but was not provisioned in wave 2. |
| KillStatsJobStatement | open; not reattempted | Requires a compatible warehouse statistics job and genuine owned job identifier; no such job backend is provisioned. |
| RaiseErrorLegacyStatement | open; not reattempted | Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| RenameEntityStatement | open; not reattempted | Warehouse-specific table/partition/copy/rename/workload grammar requires a compatible Synapse/Fabric engine, not this standalone SQL Server. |
| ResourcePoolStatement | open; not reattempted | Concrete shared AST base; actual SQL produces create/alter resource-pool subclasses. Constructed AST fallback is tested; not executable SQL coverage. |
| RevokeStatement80 | open; not reattempted | Exact AST requires TSql80Parser; default production parser errors or modern AST conversion are separately tested. No SQL Server 2000 environment is provisioned. |
| SelectStatementSnippet | open; not reattempted | Programmatically constructed SELECT snippet, not a standalone parser statement kind. Constructed AST fallback is tested; not executable SQL coverage. |
| SetErrorLevelStatement | open; not reattempted | Legacy/internal SET ERRLVL grammar has no supported observable public session property in the SqlClient runner. No-op success would not establish its intended effect. |
| TSqlStatementSnippet | open; not reattempted | Programmatically constructed raw statement snippet; not a parser production. Constructed AST fallback is tested; not executable SQL coverage. |
| UseFederationStatement | open; not reattempted | Deprecated Azure SQL federation dialect is not implemented by the SQL Server 2025 container; parser handling only. |

CLR uses a self-built reviewed trivial .NET Framework assembly (WaveClr.cs), SHA-512 trusted only inside the disposable container; aggregate invocation actually returns 3. Native MemoryWrite is invoked after a GO boundary and inserts one row. INSERT BULK uses the TDS SqlBulkCopy protocol and verifies a real row. Service-master regeneration observes non-null changed modify_date, not an unchanged key count. S3 loopback external tables exercise metadata creation/deletion without claiming external reads. Agent job creation/deletion and full-text catalog creation are additional feature assertions, not original missing kinds. Container destruction was verified by the wrapper; no server configuration was changed on the shared instance.
