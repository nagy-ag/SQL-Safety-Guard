using System.Text;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

/// <summary>
/// Two throwaway databases standing in for a production and a dev server (SqlGuard_Prod, SqlGuard_Dev), a
/// low-privilege login the cases run as, database snapshots to undo any change, and a fingerprint of everything a
/// write could change (data, schema, identity, sequences, security, database options).
///
/// Safety: cases run as the login sqlguard_harness via EXECUTE AS LOGIN ... WITH NO REVERT. It has no server
/// permissions (no SHUTDOWN, KILL, sp_configure, xp_cmdshell, CREATE DATABASE, bulk file access), is db_owner only
/// in the two sandbox databases and tempdb, and is denied BACKUP. Guest is enabled only in master, tempdb and msdb,
/// so other databases on the instance are unreachable; setup verifies this before anything runs, and the server-level
/// fingerprint is checked after every case.
/// </summary>
internal sealed class Sandbox : IDisposable
{
    public const string Prod = "SqlGuard_Prod";
    public const string Dev = "SqlGuard_Dev";
    public const string Login = "sqlguard_harness";
    private static readonly string[] Databases = { Prod, Dev };
    private const string TempdbObjectsQuery =
        "SELECT 'TO', s.name, o.name, o.type FROM tempdb.sys.objects o JOIN tempdb.sys.schemas s ON s.schema_id = o.schema_id " +
        "WHERE o.is_ms_shipped = 0 AND o.name NOT LIKE '#%' AND o.type IN ('U','V','P','FN','IF','TF','SO','SN','TR')";

    private readonly string _server;
    private readonly SqlConnection _admin;
    private readonly SchemaDiscovery _schema;
    private readonly List<string> _tempdbTables = new();
    private string _baseline = "";
    private string _serverBaseline = "";

    public Sandbox(string server, SchemaDiscovery schema)
    {
        _server = server;
        _schema = schema;
        _admin = new SqlConnection(ConnectionString("master"));
        _admin.Open();
    }

    public static bool IsSandboxDatabase(string db) =>
        db.Equals(Prod, StringComparison.OrdinalIgnoreCase) || db.Equals(Dev, StringComparison.OrdinalIgnoreCase);

    public string ConnectionString(string database) =>
        $"Server={_server};Database={database};Integrated Security=true;TrustServerCertificate=true;Pooling=false;Application Name=SqlGuardHarness;Connect Timeout=15";

    public int TableCount { get; private set; }

    /// <summary>How many times the sandbox had to be rebuilt from scratch (a snapshot restore didn't reproduce the baseline).</summary>
    public int Rebuilds { get; private set; }

    public void Exec(string sql, int timeout = 120)
    {
        using var cmd = new SqlCommand(sql, _admin) { CommandTimeout = timeout };
        cmd.ExecuteNonQuery();
    }

    public List<string> Lines(string sql, SqlConnection? connection = null)
    {
        var list = new List<string>();
        using var cmd = new SqlCommand(sql, connection ?? _admin) { CommandTimeout = 120 };
        using var reader = cmd.ExecuteReader();
        do
        {
            while (reader.Read())
            {
                var parts = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    parts[i] = reader.IsDBNull(i) ? "NULL" : Convert.ToString(reader.GetValue(i), System.Globalization.CultureInfo.InvariantCulture)!;
                list.Add(string.Join("|", parts));
            }
        } while (reader.NextResult());
        return list;
    }

    private static void Run(SqlConnection conn, string sql)
    {
        using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
        cmd.ExecuteNonQuery();
    }

    private static string Esc(string s) => s.Replace("]", "]]");

    // ---------------------------------------------------------------- setup

    public void Setup()
    {
        if (Lines("SELECT IS_SRVROLEMEMBER('sysadmin')")[0] != "1")
            throw new InvalidOperationException("Setup needs a sysadmin Windows login.");

        DropAll();
        Exec($"IF SUSER_ID(N'{Login}') IS NULL CREATE LOGIN [{Login}] WITH PASSWORD = N'{Guid.NewGuid():N}Aa1!', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;");

        foreach (var db in Databases)
        {
            Exec($"CREATE DATABASE [{db}];");
            // Statistics off: a SELECT must not change anything the fingerprint sees.
            Exec($"ALTER DATABASE [{db}] SET RECOVERY SIMPLE; ALTER DATABASE [{db}] SET AUTO_CREATE_STATISTICS OFF; ALTER DATABASE [{db}] SET AUTO_UPDATE_STATISTICS OFF;");
            using var conn = new SqlConnection(ConnectionString(db));
            conn.Open();
            foreach (var statement in SchemaScript(db))
                Run(conn, statement);
            Run(conn, $"CREATE USER [{Login}] FOR LOGIN [{Login}]; ALTER ROLE db_owner ADD MEMBER [{Login}]; DENY BACKUP DATABASE, BACKUP LOG TO [{Login}];");
        }

        using (var tempdb = new SqlConnection(ConnectionString("tempdb")))
        {
            tempdb.Open();
            Run(tempdb, $"IF USER_ID(N'{Login}') IS NULL CREATE USER [{Login}] FOR LOGIN [{Login}]; ALTER ROLE db_owner ADD MEMBER [{Login}];");
            RecreateTempdbTables(tempdb);
        }

        foreach (var db in Databases)
        {
            var file = Lines($"SELECT name, physical_name FROM sys.master_files WHERE database_id = DB_ID(N'{db}') AND type = 0")[0].Split('|');
            var path = Path.Combine(Path.GetDirectoryName(file[1])!, db + "_harness.ss");
            Exec($"CREATE DATABASE [{db}_snap] ON (NAME = N'{file[0]}', FILENAME = N'{path}') AS SNAPSHOT OF [{db}];");
        }

        VerifyIsolation();
        _baseline = Fingerprint();
        _serverBaseline = ServerFingerprint();
    }

    /// <summary>Under the harness login, only master/tempdb/msdb and the sandbox databases may be reachable.</summary>
    private void VerifyIsolation()
    {
        using var conn = new SqlConnection(ConnectionString(Prod));
        conn.Open();
        Run(conn, $"EXECUTE AS LOGIN = N'{Login}' WITH NO REVERT;");
        var row = Lines("SELECT IS_SRVROLEMEMBER('sysadmin'), ISNULL((SELECT STRING_AGG(name, ',') FROM sys.databases WHERE HAS_DBACCESS(name) = 1 AND name NOT IN ('master','tempdb','msdb') AND name NOT LIKE 'SqlGuard[_]%'), '')", conn)[0].Split('|');
        if (row[0] != "0" || row[1].Length > 0)
            throw new InvalidOperationException($"Harness login is not isolated: sysadmin={row[0]}, extra databases={row[1]}");
    }

    public void DropAll()
    {
        foreach (var db in Databases)
        {
            Exec($"IF DB_ID(N'{db}_snap') IS NOT NULL DROP DATABASE [{db}_snap];");
            Exec($"IF DB_ID(N'{db}') IS NOT NULL BEGIN ALTER DATABASE [{db}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{db}]; END");
        }
    }

    public void Teardown()
    {
        DropAll();
        using var tempdb = new SqlConnection(ConnectionString("tempdb"));
        tempdb.Open();
        foreach (var t in _tempdbTables)
            Run(tempdb, $"DROP TABLE IF EXISTS {t};");
        Run(tempdb, $"IF USER_ID(N'{Login}') IS NOT NULL DROP USER [{Login}];");
        Exec($"IF SUSER_ID(N'{Login}') IS NOT NULL DROP LOGIN [{Login}];");
    }

    /// <summary>Typed core tables (the samples' playground plus two for generated scripts), then every table the corpus names.</summary>
    private IEnumerable<string> SchemaScript(string targetDb)
    {
        var core = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Customers"] = "Id int PRIMARY KEY, Name nvarchar(50) NULL",
            ["Orders"] = "Id int PRIMARY KEY, CustomerId int NULL, Qty int NULL, Status varchar(20) NULL",
            ["AuditLog"] = "Id int IDENTITY PRIMARY KEY, OrderId int NULL, Note nvarchar(100) NULL",
            ["Staging_Import"] = "Id int NULL, X int NULL",
            ["OrderLines"] = "Id int PRIMARY KEY, OrderId int NULL, ProductId int NULL, Qty int NULL, Price decimal(10,2) NULL",
            ["Products"] = "Id int PRIMARY KEY, Name nvarchar(50) NULL, Price decimal(10,2) NULL, Category varchar(20) NULL",
        };

        var schemas = _schema.Tables.Values.Select(t => t.Schema).Concat(_schema.Sequences.Values.Select(s => s.Schema))
            .Where(s => !s.Equals("dbo", StringComparison.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase);
        foreach (var s in schemas)
            yield return $"EXEC(N'CREATE SCHEMA [{Esc(s).Replace("'", "''")}]');";

        var tables = _schema.Tables.Values
            .Where(t => t.Database == null || t.Database.Equals(targetDb, StringComparison.OrdinalIgnoreCase))
            .Select(t => (Schema: t.Schema, Name: t.Name))
            .Concat(core.Keys.Select(k => (Schema: "dbo", Name: k)))
            .DistinctBy(t => (t.Schema.ToLowerInvariant(), t.Name.ToLowerInvariant()))
            .ToList();
        TableCount = tables.Count;
        foreach (var (schema, name) in tables)
        {
            bool isCore = schema.Equals("dbo", StringComparison.OrdinalIgnoreCase) && core.ContainsKey(name);
            var typed = isCore ? core[name] : "Id nvarchar(100) NULL";
            var typedNames = typed.Split(',').Select(c => c.Trim().Split(' ')[0]).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var extra = _schema.Columns.Where(c => !typedNames.Contains(c)).Select(c => $"[{Esc(c)}] nvarchar(100) NULL");
            yield return $"CREATE TABLE [{Esc(schema)}].[{Esc(name)}] ({string.Join(", ", new[] { typed }.Concat(extra))});";
        }

        yield return "INSERT dbo.Customers (Id, Name) SELECT value, CONCAT(N'Customer ', value) FROM GENERATE_SERIES(1, 50);";
        yield return "INSERT dbo.Products (Id, Name, Price, Category) SELECT value, CONCAT(N'Product ', value), value * 1.5, CASE value % 3 WHEN 0 THEN 'a' WHEN 1 THEN 'b' ELSE 'c' END FROM GENERATE_SERIES(1, 40);";
        yield return "INSERT dbo.Orders (Id, CustomerId, Qty, Status) SELECT value, 1 + value % 50, value % 9, CASE value % 4 WHEN 0 THEN 'shipped' ELSE 'open' END FROM GENERATE_SERIES(1, 300);";
        yield return "INSERT dbo.OrderLines (Id, OrderId, ProductId, Qty, Price) SELECT value, 1 + value % 300, 1 + value % 40, 1 + value % 5, 2.5 FROM GENERATE_SERIES(1, 900);";
        yield return "INSERT dbo.AuditLog (OrderId, Note) SELECT value, N'seed' FROM GENERATE_SERIES(1, 20);";
        yield return "INSERT dbo.Staging_Import (Id, X) VALUES (1, 0), (2, 0);";
        foreach (var (schema, name) in tables.Where(t => !(t.Schema.Equals("dbo", StringComparison.OrdinalIgnoreCase) && core.ContainsKey(t.Name))))
            yield return $"INSERT [{Esc(schema)}].[{Esc(name)}] (Id) VALUES (N'1'), (N'2'), (N'3');";

        var sequences = _schema.Sequences.Values.Select(s => (Schema: s.Schema, Name: s.Name))
            .Concat(new[] { (Schema: "dbo", Name: "OrderNumbers"), (Schema: "dbo", Name: "Seq") })
            .DistinctBy(s => (s.Schema.ToLowerInvariant(), s.Name.ToLowerInvariant()));
        foreach (var (schema, name) in sequences)
            yield return $"CREATE SEQUENCE [{Esc(schema)}].[{Esc(name)}] AS int START WITH 1000;";

        // Procedures the samples call: one writes, one reads (EXEC of either must never be green).
        yield return "EXEC(N'CREATE PROCEDURE dbo.usp_Cleanup AS DELETE FROM dbo.Orders WHERE Qty = 0;');";
        yield return "EXEC(N'CREATE PROCEDURE dbo.usp_GetOrders AS SELECT Id, Qty FROM dbo.Orders;');";
    }

    private void RecreateTempdbTables(SqlConnection tempdb)
    {
        _tempdbTables.Clear();
        foreach (var t in _schema.Tables.Values.Where(t => "tempdb".Equals(t.Database, StringComparison.OrdinalIgnoreCase)))
        {
            var full = $"[{Esc(t.Schema)}].[{Esc(t.Name)}]";
            if (!t.Schema.Equals("dbo", StringComparison.OrdinalIgnoreCase))
                Run(tempdb, $"IF SCHEMA_ID(N'{t.Schema.Replace("'", "''")}') IS NULL EXEC(N'CREATE SCHEMA [{Esc(t.Schema).Replace("'", "''")}]');");
            Run(tempdb, $"DROP TABLE IF EXISTS {full}; CREATE TABLE {full} (Id nvarchar(100) NULL, Qty int NULL, X int NULL); INSERT {full} (Id, Qty, X) VALUES (N'1', 1, 1), (N'2', 2, 2);");
            _tempdbTables.Add(full);
        }
    }

    // ---------------------------------------------------------------- fingerprint

    public string Fingerprint()
    {
        var sb = new StringBuilder();
        foreach (var db in Databases)
        {
            if (Lines($"SELECT COUNT(*) FROM sys.databases WHERE name = N'{db}' AND state = 0 AND user_access = 0")[0] != "1")
            {
                sb.AppendLine($"{db}|MISSING, OFFLINE OR NOT MULTI_USER");
                continue;
            }

            var d = $"[{db}]";
            var id = $"DB_ID(N'{db}')";
            var parts = new List<string>();
            parts.AddRange(Lines($"SELECT 'DB', name, compatibility_level, recovery_model, user_access, is_read_only, snapshot_isolation_state, is_read_committed_snapshot_on, collation_name, is_trustworthy_on, page_verify_option, is_auto_close_on, is_auto_shrink_on FROM sys.databases WHERE name = N'{db}'"));
            parts.AddRange(Lines($"SELECT 'O', s.name, o.name, o.type, CONVERT(varchar(30), o.modify_date, 126) FROM {d}.sys.objects o JOIN {d}.sys.schemas s ON s.schema_id = o.schema_id WHERE o.is_ms_shipped = 0 AND o.type NOT IN ('IT','S','SQ')"));
            parts.AddRange(Lines($"SELECT 'C', OBJECT_SCHEMA_NAME(c.object_id, {id}), OBJECT_NAME(c.object_id, {id}), c.name, c.system_type_id, c.max_length, c.is_nullable, c.is_identity FROM {d}.sys.columns c JOIN {d}.sys.objects o ON o.object_id = c.object_id WHERE o.is_ms_shipped = 0"));
            parts.AddRange(Lines($"SELECT 'I', OBJECT_NAME(i.object_id, {id}), i.name, i.type, i.is_disabled FROM {d}.sys.indexes i JOIN {d}.sys.objects o ON o.object_id = i.object_id WHERE o.is_ms_shipped = 0 AND i.type > 0"));
            parts.AddRange(Lines($"SELECT 'Q', SCHEMA_NAME(schema_id), name, CONVERT(varchar(40), current_value), CONVERT(varchar(40), last_used_value), CONVERT(varchar(40), increment) FROM {d}.sys.sequences"));
            parts.AddRange(Lines($"SELECT 'S', name FROM {d}.sys.schemas"));
            parts.AddRange(Lines($"SELECT 'P', name, type, default_schema_name FROM {d}.sys.database_principals"));
            parts.AddRange(Lines($"SELECT 'R', USER_NAME(role_principal_id), member_principal_id FROM {d}.sys.database_role_members"));
            parts.AddRange(Lines($"SELECT 'G', class, major_id, minor_id, grantee_principal_id, permission_name, state FROM {d}.sys.database_permissions"));
            parts.AddRange(Lines($"SELECT 'X', class, major_id, minor_id, name, CONVERT(nvarchar(4000), value) FROM {d}.sys.extended_properties"));
            parts.AddRange(Lines($"SELECT 'Y', name, base_object_name FROM {d}.sys.synonyms"));
            parts.AddRange(Lines($"EXEC {d}.sys.sp_executesql N'SELECT ''ID'', OBJECT_SCHEMA_NAME(object_id), OBJECT_NAME(object_id), CONVERT(varchar(40), IDENT_CURRENT(QUOTENAME(OBJECT_SCHEMA_NAME(object_id)) + ''.'' + QUOTENAME(OBJECT_NAME(object_id)))) FROM sys.identity_columns'"));
            parts.AddRange(DataLines(db, permanentOnly: false));
            parts.Sort(StringComparer.Ordinal);
            foreach (var p in parts)
                sb.Append(db).Append('|').AppendLine(p);
        }

        var tempParts = Lines(TempdbObjectsQuery);
        tempParts.AddRange(DataLines("tempdb", permanentOnly: true));
        tempParts.Sort(StringComparer.Ordinal);
        foreach (var p in tempParts)
            sb.Append("tempdb|").AppendLine(p);
        return sb.ToString();
    }

    private List<string> DataLines(string db, bool permanentOnly)
    {
        var tables = Lines($"SELECT QUOTENAME(s.name) + '.' + QUOTENAME(t.name) FROM [{db}].sys.tables t JOIN [{db}].sys.schemas s ON s.schema_id = t.schema_id WHERE t.is_ms_shipped = 0" +
                           (permanentOnly ? " AND t.name NOT LIKE '#%'" : ""));
        var result = new List<string>();
        foreach (var t in tables)
        {
            try
            {
                result.AddRange(Lines($"SELECT 'D', N'{t.Replace("'", "''")}', COUNT_BIG(*), CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM [{db}].{t}"));
            }
            catch (SqlException ex)
            {
                result.Add($"D|{t}|unreadable {ex.Number}");
            }
        }

        return result;
    }

    public string ServerFingerprint()
    {
        var parts = new List<string>();
        parts.AddRange(Lines("SELECT 'L', name, type, is_disabled FROM sys.server_principals"));
        parts.AddRange(Lines("SELECT 'SR', role_principal_id, member_principal_id FROM sys.server_role_members"));
        parts.AddRange(Lines("SELECT 'SP', class, major_id, grantee_principal_id, permission_name, state FROM sys.server_permissions"));
        parts.AddRange(Lines("SELECT 'CFG', name, CONVERT(varchar(40), value_in_use) FROM sys.configurations"));
        parts.AddRange(Lines("SELECT 'DBS', name, state, user_access, is_read_only FROM sys.databases WHERE name NOT LIKE 'SqlGuard[_]%'"));
        parts.AddRange(Lines("SELECT 'LS', name, product, provider, data_source FROM sys.servers"));
        parts.AddRange(Lines("SELECT 'ST', name FROM sys.server_triggers"));
        parts.Sort(StringComparer.Ordinal);
        return string.Join("\n", parts);
    }

    public bool MatchesBaseline(string fingerprint) => fingerprint == _baseline;

    /// <summary>Lines that differ from the baseline (for the report).</summary>
    public List<string> Diff(string current)
    {
        var before = _baseline.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToHashSet();
        var after = current.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToHashSet();
        return before.Except(after).Select(l => "- " + l).Concat(after.Except(before).Select(l => "+ " + l)).Take(12).ToList();
    }

    public void AssertServerUnchanged(string caseId)
    {
        var now = ServerFingerprint();
        if (now == _serverBaseline)
            return;
        var before = _serverBaseline.Split('\n').ToHashSet();
        var diff = string.Join("; ", now.Split('\n').Where(l => !before.Contains(l)).Take(5));
        throw new InvalidOperationException($"SERVER-LEVEL CHANGE after case {caseId}: {diff}. Harness stopped.");
    }

    // ---------------------------------------------------------------- revert

    /// <summary>Restores the databases whose part of <paramref name="current"/> differs from the baseline, and cleans tempdb.</summary>
    public void Revert(string current)
    {
        SqlConnection.ClearAllPools();
        string Part(string fingerprint, string prefix) =>
            string.Join("\n", fingerprint.Split('\n').Where(l => l.StartsWith(prefix + "|", StringComparison.Ordinal)));
        foreach (var db in Databases.Where(d => Part(current, d) != Part(_baseline, d)))
        {
            try
            {
                Exec($"IF DB_ID(N'{db}') IS NOT NULL ALTER DATABASE [{db}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
                Exec($"RESTORE DATABASE [{db}] FROM DATABASE_SNAPSHOT = N'{db}_snap';", 300);
                Exec($"ALTER DATABASE [{db}] SET MULTI_USER;");
            }
            catch (SqlException)
            {
                // Dropped, offline or otherwise unrestorable: rebuilt below.
            }
        }

        using (var tempdb = new SqlConnection(ConnectionString("tempdb")))
        {
            tempdb.Open();
            // Drop permanent objects a case created in tempdb, then rebuild the harness's own tempdb tables.
            var baselineTemp = _baseline.Split('\n').Select(l => l.Trim()).Where(l => l.StartsWith("tempdb|TO|")).ToHashSet();
            foreach (var line in Lines(TempdbObjectsQuery))
            {
                if (baselineTemp.Contains("tempdb|" + line))
                    continue;
                var p = line.Split('|');
                var kind = p[3].Trim() switch { "U" => "TABLE", "V" => "VIEW", "P" => "PROCEDURE", "SO" => "SEQUENCE", "SN" => "SYNONYM", "TR" => "TRIGGER", _ => "FUNCTION" };
                try { Run(tempdb, $"DROP {kind} [{Esc(p[1])}].[{Esc(p[2])}];"); } catch (SqlException) { }
            }

            RecreateTempdbTables(tempdb);
            Run(tempdb, $"IF USER_ID(N'{Login}') IS NULL CREATE USER [{Login}] FOR LOGIN [{Login}]; ALTER ROLE db_owner ADD MEMBER [{Login}];");
        }

        if (!MatchesBaseline(Fingerprint()))
        {
            // Snapshot restore wasn't enough (e.g. a database was dropped or altered): rebuild everything.
            Setup();
            Rebuilds++;
        }
    }

    public void Dispose() => _admin.Dispose();
}
