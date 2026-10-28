using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record CampaignSnapshot(Dictionary<string, string[]> Databases, Dictionary<string, long> Operations,
    Dictionary<string, string> Objects, string[] Server, string[] Tempdb, string[] Files);
internal sealed record CampaignSession(int TransactionCount, int Options, string OriginalLogin, string EffectiveLogin, string DatabaseUser, string Database);
internal sealed record CampaignObservation(bool Changed, string[] ModifiedObjects, string[] LogicalChanges, string[] PhysicalWrites,
    string[] SessionLeftovers, string[] BeforeHashes, string[] AfterHashes, bool ServerChanged, bool TempdbChanged, bool FilesChanged);

internal static class CampaignOracle
{
    public static CampaignSession Session(SqlConnection connection)
    {
        var row = CampaignSql.Rows(connection, "SELECT @@TRANCOUNT,@@OPTIONS,ORIGINAL_LOGIN(),SUSER_SNAME(),USER_NAME(),DB_NAME()")[0];
        return new CampaignSession(int.Parse(row[0]), int.Parse(row[1]), row[2], row[3], row[4], row[5]);
    }
    public static CampaignSnapshot Capture(string connectionString, IReadOnlyDictionary<string, string> databases, int timeout, string? container)
    {
        var snapshots = new Dictionary<string, string[]>();
        var operations = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        var objects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var database in databases.Values)
        {
            using var connection = CampaignSql.Open(connectionString, database);
            snapshots[database] = CoverageDatabase.Capture(connection, timeout);
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(SCHEMA_NAME(objects.schema_id))+'.'+QUOTENAME(objects.name),objects.type,CONVERT(nvarchar(max),(SELECT objects.name,objects.type,objects.schema_id,objects.modify_date,OBJECT_DEFINITION(objects.object_id) definition FOR JSON PATH,INCLUDE_NULL_VALUES)) FROM sys.objects objects WHERE is_ms_shipped=0", timeout))
                objects[database + "." + row[0]] = row[2];
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(SCHEMA_NAME(schema_id))+'.'+QUOTENAME(name),CONVERT(nvarchar(max),(SELECT sequences.current_value,sequences.last_used_value FOR JSON PATH,INCLUDE_NULL_VALUES)) FROM sys.sequences sequences", timeout))
                objects[database + "." + row[0]] += row[1];
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(OBJECT_SCHEMA_NAME(object_id))+'.'+QUOTENAME(OBJECT_NAME(object_id)),CONVERT(nvarchar(max),(SELECT identities.name,identities.seed_value,identities.increment_value,identities.last_value FOR JSON PATH,INCLUDE_NULL_VALUES)) FROM sys.identity_columns identities WHERE OBJECTPROPERTY(object_id,'IsMSShipped')=0", timeout))
                objects[database + "." + row[0]] += row[1];
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(SCHEMA_NAME(tables.schema_id))+'.'+QUOTENAME(tables.name),COALESCE(SUM(stats.leaf_insert_count+stats.leaf_update_count+stats.leaf_delete_count),0) FROM sys.tables tables LEFT JOIN sys.dm_db_index_operational_stats(DB_ID(),NULL,NULL,NULL) stats ON tables.object_id=stats.object_id AND stats.index_id IN(0,1) WHERE tables.is_ms_shipped=0 GROUP BY tables.schema_id,tables.name", timeout))
                operations[database + "." + row[0]] = long.Parse(row[1]);
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(SCHEMA_NAME(schema_id))+'.'+QUOTENAME(name) FROM sys.service_queues WHERE is_ms_shipped=0", timeout))
            {
                var queue = row[0];
                var messages = CampaignSql.Rows(connection, $"SELECT conversation_handle,message_sequence_number,message_type_name,CONVERT(varchar(max),message_body,2) FROM {queue}", timeout);
                snapshots[database] = snapshots[database].Concat(messages.Select(message => "queue:" + queue + ":" + JsonSerializer.Serialize(message))).Order(StringComparer.Ordinal).ToArray();
            }
        }
        using var admin = CampaignSql.Open(connectionString);
        string[] serverQueries = [
            "SELECT name,type,is_disabled,default_database_name FROM sys.server_principals WHERE name NOT LIKE 'SgRun[_]%' ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT name,value,value_in_use FROM sys.configurations ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT name,product,provider,data_source,is_linked,is_data_access_enabled,is_rpc_out_enabled FROM sys.servers ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT role_principal_id,member_principal_id FROM sys.server_role_members ORDER BY role_principal_id,member_principal_id FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT class,major_id,grantee_principal_id,type,state FROM sys.server_permissions WHERE grantee_principal_id NOT IN(SELECT principal_id FROM sys.server_principals WHERE name LIKE 'SgRun[_]%') ORDER BY class,major_id,grantee_principal_id,type FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT name,state,is_read_only,is_trustworthy_on,is_db_chaining_on FROM sys.databases WHERE name NOT LIKE 'SgRun[_]%' ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT name,type_desc,state_desc FROM sys.endpoints ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES",
            "SELECT name FROM sys.server_event_sessions ORDER BY name FOR JSON PATH,INCLUDE_NULL_VALUES"
        ];
        var server = serverQueries.Select(query => string.Join("", CampaignSql.Rows(admin, query, timeout).Select(row => row[0]))).ToArray();
        using var tempdb = CampaignSql.Open(connectionString, "tempdb");
        var temp = CampaignSql.Rows(tempdb, "SELECT QUOTENAME(SCHEMA_NAME(schema_id))+'.'+QUOTENAME(name),type,CONVERT(varchar(30),modify_date,126) FROM sys.objects WHERE is_ms_shipped=0 AND name NOT LIKE '#%' ORDER BY object_id", timeout).Select(row => string.Join("|", row)).ToList();
        foreach (var row in CampaignSql.Rows(tempdb, "SELECT QUOTENAME(SCHEMA_NAME(schema_id))+'.'+QUOTENAME(name) FROM sys.tables WHERE is_ms_shipped=0 AND name NOT LIKE '#%'", timeout))
            temp.AddRange(CoverageDatabase.CaptureTableRows(tempdb, row[0], timeout));
        var files = container == null ? [] : CampaignSql.Docker(container, "exec", container, "sh", "-c", "set -e; for directory in /var/opt/mssql/data /var/opt/mssql/backup; do if [ -d \"$directory\" ]; then find \"$directory\" -maxdepth 1 -type f ! -name '*.mdf' ! -name '*.ldf' ! -name '*.ndf' ! -name 'Entropy.bin' -exec sha256sum {} +; fi; done").Split('\n', StringSplitOptions.RemoveEmptyEntries).Order(StringComparer.Ordinal).ToArray();
        return new CampaignSnapshot(snapshots, operations, objects, server, temp.Order(StringComparer.Ordinal).ToArray(), files);
    }
    public static CampaignObservation Compare(CampaignSnapshot before, CampaignSnapshot after, CampaignSession baseline, CampaignSession? session)
    {
        var logical = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var database in before.Databases.Keys)
        {
            var beforeCounts = before.Databases[database].GroupBy(line => line).ToDictionary(group => group.Key, group => group.Count());
            var afterCounts = after.Databases[database].GroupBy(line => line).ToDictionary(group => group.Key, group => group.Count());
            foreach (var line in beforeCounts.Keys.Union(afterCounts.Keys).Where(line => beforeCounts.GetValueOrDefault(line) != afterCounts.GetValueOrDefault(line)))
            {
                if (line.StartsWith('[')) logical.Add(database + "." + line[..line.IndexOf(':')]);
                else if (line.StartsWith("queue:")) logical.Add(database + "." + line.Split(':')[1]);
                else logical.Add(database + ".<catalog:" + line.Split(':')[1] + ">");
            }
        }
        foreach (var name in before.Objects.Keys.Union(after.Objects.Keys))
            if (before.Objects.GetValueOrDefault(name) != after.Objects.GetValueOrDefault(name)) logical.Add(name);
        var physical = after.Operations.Where(pair => pair.Value > before.Operations.GetValueOrDefault(pair.Key)).Select(pair => pair.Key).ToArray();
        var leftovers = new List<string>();
        if (session == null) leftovers.Add("Session unavailable (closed or terminated); leftovers unobserved");
        else
        {
            if (session.TransactionCount != baseline.TransactionCount) leftovers.Add("@@TRANCOUNT=" + session.TransactionCount);
            if (session.Options != baseline.Options) leftovers.Add($"@@OPTIONS={session.Options}; baseline={baseline.Options}; changed mask={session.Options ^ baseline.Options}");
            if (session.EffectiveLogin != baseline.EffectiveLogin || session.DatabaseUser != baseline.DatabaseUser)
                leftovers.Add($"EXECUTE AS: ORIGINAL_LOGIN={session.OriginalLogin}; SUSER_SNAME={session.EffectiveLogin}; USER_NAME={session.DatabaseUser}");
            if (session.Database != baseline.Database) leftovers.Add("Database context=" + session.Database);
        }
        bool serverChanged = !before.Server.SequenceEqual(after.Server);
        bool tempChanged = !before.Tempdb.SequenceEqual(after.Tempdb);
        bool filesChanged = !before.Files.SequenceEqual(after.Files);
        if (serverChanged) logical.Add("<server-state>");
        if (tempChanged) logical.Add("tempdb.<permanent-objects>");
        if (filesChanged) logical.Add("<container-files>");
        return new CampaignObservation(logical.Count > 0 || physical.Length > 0, logical.Concat(physical).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToArray(), logical.Order().ToArray(), physical, leftovers.ToArray(), before.Databases.Select(pair => pair.Key + ":" + CoverageDatabase.Hash(pair.Value)).ToArray(), after.Databases.Select(pair => pair.Key + ":" + CoverageDatabase.Hash(pair.Value)).ToArray(), serverChanged, tempChanged, filesChanged);
    }
}
