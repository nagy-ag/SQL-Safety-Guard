using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed class CoverageDatabase : IDisposable
{
    private readonly SqlConnection _admin;
    private readonly string _database = "SqlGuard_Coverage_" + Guid.NewGuid().ToString("N");
    private readonly int _timeout;
    private readonly string _adminConnectionString;
    private bool _created;
    public SqlConnection Connection { get; }
    public string DatabaseName => _database;
    public string ServerVersion => _admin.ServerVersion;

    public CoverageDatabase(string server, int timeout)
    {
        _timeout = timeout;
        var configuredConnection = Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONNECTION");
        var builder = new SqlConnectionStringBuilder(configuredConnection ?? string.Empty)
        {
            InitialCatalog = "master",
            TrustServerCertificate = true, Pooling = false, ConnectTimeout = 15,
            ApplicationName = "SqlGuardStrictCoverage"
        };
        if (string.IsNullOrWhiteSpace(configuredConnection))
        {
            builder.DataSource = server;
            builder.IntegratedSecurity = true;
        }
        _adminConnectionString = builder.ConnectionString;
        _admin = new SqlConnection(_adminConnectionString);
        Connection = new SqlConnection();
        try
        {
            _admin.Open();
            if (Convert.ToInt32(Scalar(_admin, "SELECT IS_SRVROLEMEMBER('sysadmin');")) != 1)
                throw new InvalidOperationException("Disposable database creation needs a sysadmin provisioning login.");
            Execute(_admin, $"CREATE DATABASE [{_database}];");
            _created = true;
            Execute(_admin, $"ALTER DATABASE [{_database}] SET AUTO_CREATE_STATISTICS OFF; ALTER DATABASE [{_database}] SET AUTO_UPDATE_STATISTICS OFF;");
            builder.InitialCatalog = _database;
            Connection.ConnectionString = builder.ConnectionString;
            Connection.Open();
            Execute(Connection, "CREATE USER coverage_runner WITHOUT LOGIN; ALTER ROLE db_owner ADD MEMBER coverage_runner; DENY BACKUP DATABASE, BACKUP LOG TO coverage_runner; EXECUTE AS USER = 'coverage_runner' WITH NO REVERT;");
            if (Convert.ToInt32(Scalar(Connection, "SELECT ISNULL(HAS_PERMS_BY_NAME(NULL, NULL, 'CONTROL SERVER'), 0);")) != 0 ||
                Convert.ToString(Scalar(Connection, "SELECT DB_NAME();")) != _database)
                throw new InvalidOperationException("Coverage connection isolation failed.");
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void Execute(string sql)
    {
        var script = SqlCmdLite.Split(sql, sqlCmdMode: false);
        if (script.NotRunnable != null || script.Batches.Any(batch => batch.Error != null || batch.Repeat != 1))
            throw new InvalidDataException("Strict fixtures only support plain SQL and nonrepeated GO batches.");
        foreach (var batch in script.Batches) Execute(Connection, batch.Sql);
    }

    private void Execute(SqlConnection connection, string sql)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = _timeout };
        using var reader = command.ExecuteReader();
        do { while (reader.Read()) { } } while (reader.NextResult());
    }

    public object? Scalar(string sql) => Scalar(Connection, sql);

    private object? Scalar(SqlConnection connection, string sql)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = _timeout };
        return command.ExecuteScalar();
    }

    public string[] Snapshot() => Snapshot(Connection);

    public string[] PersistedSnapshot()
    {
        Connection.Close();
        var builder = new SqlConnectionStringBuilder(_adminConnectionString) { InitialCatalog = _database };
        using var observer = new SqlConnection(builder.ConnectionString);
        observer.Open();
        return Snapshot(observer);
    }

    private string[] Snapshot(SqlConnection connection) => Capture(connection, _timeout);

    internal static string[] Capture(SqlConnection connection, int timeout)
    {
        using (var reset = new SqlCommand("SET ROWCOUNT 0; SET IMPLICIT_TRANSACTIONS OFF;", connection) { CommandTimeout = timeout })
            reset.ExecuteNonQuery();
        var lines = new List<string>();
        string[] catalogQueries =
        {
            "SELECT name, type, schema_id, parent_object_id, modify_date FROM sys.objects WHERE is_ms_shipped = 0 ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, definition, uses_ansi_nulls, uses_quoted_identifier FROM sys.sql_modules ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, column_id, system_type_id, user_type_id, max_length, precision, scale, is_nullable, is_identity, is_computed, default_object_id FROM sys.columns ORDER BY object_id, column_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, index_id, type, is_unique, is_disabled, fill_factor, filter_definition FROM sys.indexes WHERE object_id IN (SELECT object_id FROM sys.objects WHERE is_ms_shipped = 0) ORDER BY object_id, index_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, index_id, index_column_id, column_id, key_ordinal, is_descending_key, is_included_column FROM sys.index_columns WHERE object_id IN (SELECT object_id FROM sys.objects WHERE is_ms_shipped = 0) ORDER BY object_id, index_id, index_column_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, schema_id, current_value, last_used_value, increment, minimum_value, maximum_value, is_cycling FROM sys.sequences ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, last_value FROM sys.identity_columns ORDER BY object_id, column_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, schema_id, principal_id FROM sys.schemas ORDER BY schema_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, type, default_schema_name FROM sys.database_principals ORDER BY principal_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT * FROM sys.database_role_members ORDER BY role_principal_id, member_principal_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT class, major_id, minor_id, grantee_principal_id, grantor_principal_id, type, state FROM sys.database_permissions ORDER BY class, major_id, minor_id, grantee_principal_id, type FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, user_type_id, schema_id, system_type_id, max_length, precision, scale, is_nullable FROM sys.types WHERE is_user_defined = 1 ORDER BY user_type_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, base_object_name FROM sys.synonyms ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT parent_object_id, name, definition, is_disabled, is_not_trusted FROM sys.check_constraints ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT parent_object_id, name, definition FROM sys.default_constraints ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, stats_id, auto_created, user_created, no_recompute, filter_definition FROM sys.stats WHERE object_id IN (SELECT object_id FROM sys.objects WHERE is_ms_shipped = 0) ORDER BY object_id, stats_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT compatibility_level, collation_name, is_read_only, is_auto_create_stats_on, is_auto_update_stats_on, is_trustworthy_on, is_db_chaining_on FROM sys.databases WHERE database_id = DB_ID() FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, is_disabled, is_instead_of_trigger FROM sys.triggers ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT xml_collection_id, schema_id, name FROM sys.xml_schema_collections ORDER BY xml_collection_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT function_id, name, type, boundary_value_on_right, fanout FROM sys.partition_functions ORDER BY function_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT function_id, boundary_id, value FROM sys.partition_range_values ORDER BY function_id, boundary_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT data_space_id, name, function_id FROM sys.partition_schemes ORDER BY data_space_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, certificate_id, subject, expiry_date, start_date, pvt_key_encryption_type FROM sys.certificates WHERE name NOT LIKE '##%' ORDER BY certificate_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, asymmetric_key_id, algorithm_desc, key_length, pvt_key_encryption_type FROM sys.asymmetric_keys WHERE name NOT LIKE '##%' ORDER BY asymmetric_key_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, symmetric_key_id, key_guid, algorithm_desc, key_length FROM sys.symmetric_keys ORDER BY symmetric_key_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT class, major_id, thumbprint, crypt_type FROM sys.crypt_properties ORDER BY class, major_id, thumbprint, crypt_type FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, column_encryption_key_id FROM sys.column_encryption_keys ORDER BY column_encryption_key_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT column_encryption_key_id, column_master_key_id, encrypted_value, encryption_algorithm_name FROM sys.column_encryption_key_values ORDER BY column_encryption_key_id, column_master_key_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, key_store_provider_name, key_path FROM sys.column_master_keys ORDER BY column_master_key_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, validation, xml_collection_id FROM sys.service_message_types ORDER BY message_type_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id FROM sys.service_contracts ORDER BY service_contract_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT * FROM sys.service_contract_message_usages ORDER BY service_contract_id, message_type_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, service_queue_id FROM sys.services ORDER BY service_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, object_id, is_receive_enabled, is_enqueue_enabled, is_retention_enabled, is_activation_enabled, activation_procedure, max_readers FROM sys.service_queues WHERE is_ms_shipped = 0 ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, remote_service_name, broker_instance, lifetime, address, mirror_address FROM sys.routes WHERE name <> 'AutoCreatedLocal' ORDER BY route_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, remote_service_name, service_contract_id, local_service_id, priority FROM sys.conversation_priorities ORDER BY priority_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id, remote_service_name, remote_principal_id, is_anonymous_on FROM sys.remote_service_bindings ORDER BY remote_service_binding_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT conversation_handle, conversation_group_id, service_id, service_contract_id, state, far_service, is_initiator FROM sys.conversation_endpoints ORDER BY conversation_handle FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT conversation_handle, send_sequence, receive_sequence, end_dialog_sequence, system_sequence, dialog_timer FROM sys.conversation_endpoints ORDER BY conversation_handle FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT service_id, service_contract_id FROM sys.service_contract_usages ORDER BY service_id, service_contract_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT key_id, thumbprint, crypt_type, crypt_property FROM sys.key_encryptions ORDER BY key_id, thumbprint, crypt_type FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT partition_scheme_id, destination_id, data_space_id FROM sys.destination_data_spaces ORDER BY partition_scheme_id, destination_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, parent_class, parent_id, service_name, broker_instance FROM sys.event_notifications ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, format_type, field_terminator FROM sys.external_file_formats ORDER BY file_format_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, data_space_id, type, is_default, is_read_only FROM sys.filegroups ORDER BY data_space_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, value, value_for_secondary FROM sys.database_scoped_configurations ORDER BY configuration_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, is_enabled, is_schema_bound FROM sys.security_policies ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, security_predicate_id, target_object_id, predicate_definition, predicate_type, operation FROM sys.security_predicates ORDER BY object_id, security_predicate_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT class, major_id, minor_id, label, label_id, information_type, information_type_id, rank FROM sys.sensitivity_classifications ORDER BY class, major_id, minor_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, is_default, principal_id, is_accent_sensitivity_on FROM sys.fulltext_catalogs ORDER BY fulltext_catalog_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT name, principal_id FROM sys.fulltext_stoplists ORDER BY stoplist_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT stoplist_id, stopword, language_id FROM sys.fulltext_stopwords ORDER BY stoplist_id, language_id, stopword FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, unique_index_id, fulltext_catalog_id, is_enabled, change_tracking_state, stoplist_id FROM sys.fulltext_indexes ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT property_list_id, name, principal_id FROM sys.registered_search_property_lists ORDER BY property_list_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT property_list_id, property_name, property_set_guid, property_int_id, property_description FROM sys.registered_search_properties ORDER BY property_list_id, property_name FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, is_track_columns_updated_on FROM sys.change_tracking_tables ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT object_id, name, is_memory_optimized, durability, lock_escalation, is_filetable, filestream_data_space_id FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY object_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT partition_id, data_compression, xml_compression FROM sys.partitions WHERE object_id IN (SELECT object_id FROM sys.tables WHERE is_ms_shipped = 0) ORDER BY partition_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT xml_collection_id, xml_component_id, name, kind, derivation FROM sys.xml_schema_components WHERE xml_collection_id > 0 ORDER BY xml_component_id FOR JSON PATH, INCLUDE_NULL_VALUES",
            "SELECT stats.object_id, stats.stats_id, properties.last_updated, properties.rows, properties.rows_sampled, properties.steps, properties.unfiltered_rows, properties.modification_counter FROM sys.stats CROSS APPLY sys.dm_db_stats_properties(stats.object_id, stats.stats_id) properties WHERE stats.object_id IN (SELECT object_id FROM sys.tables WHERE is_ms_shipped = 0) ORDER BY stats.object_id, stats.stats_id FOR JSON PATH, INCLUDE_NULL_VALUES"
        };
        using (var catalogCommand = new SqlCommand(string.Join(";\n", catalogQueries), connection) { CommandTimeout = timeout })
        using (var catalogReader = catalogCommand.ExecuteReader())
        {
            int catalogIndex = 0;
            do
            {
                var json = new StringBuilder();
                while (catalogReader.Read()) json.Append(catalogReader.GetString(0));
                lines.Add($"catalog:{catalogIndex++}:" + json);
            } while (catalogReader.NextResult());
            if (catalogIndex != catalogQueries.Length) throw new InvalidDataException("Incomplete catalog snapshot.");
        }
        var tables = new List<string>();
        using (var command = new SqlCommand("SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY object_id", connection) { CommandTimeout = timeout })
        using (var reader = command.ExecuteReader())
            while (reader.Read()) tables.Add(reader.GetString(0));
        foreach (var table in tables) lines.AddRange(CaptureTableRows(connection, table, timeout));
        return lines.Order(StringComparer.Ordinal).ToArray();
    }

    internal static string[] CaptureTableRows(SqlConnection connection, string table, int timeout)
    {
        var lines = new List<string>();
        using var command = new SqlCommand($"SELECT * FROM {table}", connection) { CommandTimeout = timeout };
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var cells = Enumerable.Range(0, reader.FieldCount).Select(index =>
            {
                if (reader.IsDBNull(index)) return "null";
                var value = reader.GetValue(index);
                var text = value switch
                {
                    byte[] bytes => Convert.ToBase64String(bytes),
                    DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
                    DateTimeOffset date => date.ToString("O", CultureInfo.InvariantCulture),
                    _ => Convert.ToString(value, CultureInfo.InvariantCulture)!
                };
                return reader.GetDataTypeName(index) + ":" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            });
            lines.Add(table + ":" + string.Join("|", cells));
        }
        return lines.Order(StringComparer.Ordinal).ToArray();
    }

    private static string ReadJson(SqlConnection connection, string sql, int timeout)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = timeout };
        using var reader = command.ExecuteReader();
        var text = new StringBuilder();
        while (reader.Read()) text.Append(reader.GetString(0));
        return text.ToString();
    }

    public static string Hash(string[] snapshot) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", snapshot))));

    public void Dispose()
    {
        Connection.Dispose();
        try
        {
            if (_created)
            {
                Execute(_admin, $"ALTER DATABASE [{_database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_database}];");
                _created = false;
            }
        }
        finally { _admin.Dispose(); }
    }
}
