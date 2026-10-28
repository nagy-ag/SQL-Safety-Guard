using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed class CampaignWorld
{
    public static readonly string[] Roles = ["Core", "Sales", "Audit", "Archive"];
    private readonly string _connectionString;
    private readonly string _scriptDirectory;
    private readonly Dictionary<string, string> _backups = new();
    public CampaignWorld(string connectionString, string scriptDirectory)
    {
        _connectionString = connectionString;
        _scriptDirectory = scriptDirectory;
    }
    public static string Expand(string sql, IReadOnlyDictionary<string, string> databases, int seed)
    {
        foreach (var role in Roles) sql = sql.Replace("{{" + role + "}}", databases[role], StringComparison.Ordinal);
        return sql.Replace("{{seed}}", seed.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
    public static string ScriptHash(string directory)
    {
        var scripts = File.ReadAllText(Path.Combine(directory, "world.sql")).ReplaceLineEndings("\n") + File.ReadAllText(Path.Combine(directory, "rebind.sql")).ReplaceLineEndings("\n");
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(scripts)));
    }
    public void Provision()
    {
        using var admin = CampaignSql.Open(_connectionString);
        var backupDirectory = Convert.ToString(CampaignSql.Scalar(admin, "SELECT SERVERPROPERTY('InstanceDefaultBackupPath')"))!;
        var separator = backupDirectory.Contains('/') ? "/" : "\\";
        var databases = Roles.ToDictionary(role => role, role => "SgPlay_" + role);
        var scriptHash = ScriptHash(_scriptDirectory);
        var created = new List<string>();
        foreach (var role in Roles)
        {
            var database = databases[role];
            if (CampaignSql.Scalar(admin, $"SELECT DB_ID({CampaignSql.Literal(database)})") is DBNull)
            {
                CampaignSql.Execute(admin, $"CREATE DATABASE {CampaignSql.Quote(database)}; ALTER DATABASE {CampaignSql.Quote(database)} SET RECOVERY SIMPLE; ALTER DATABASE {CampaignSql.Quote(database)} SET AUTO_CREATE_STATISTICS OFF; ALTER DATABASE {CampaignSql.Quote(database)} SET AUTO_UPDATE_STATISTICS OFF;");
                using var setup = CampaignSql.Open(_connectionString, database);
                CampaignSql.Execute(setup, "EXEC sys.sp_addextendedproperty @name=N'SgCampaignTemplate',@value=N'building';");
                CampaignSql.Execute(setup, File.ReadAllText(Path.Combine(_scriptDirectory, "world.sql")));
                created.Add(database);
            }
            else
            {
                using var existing = CampaignSql.Open(_connectionString, database);
                if (Convert.ToString(CampaignSql.Scalar(existing, "SELECT value FROM sys.extended_properties WHERE class=0 AND name=N'SgCampaignTemplate'")) != "1")
                    throw new InvalidOperationException("Refusing an unowned template: " + database);
            }
            _backups[role] = backupDirectory.TrimEnd('/', '\\') + separator + database + "_campaign.bak";
        }
        foreach (var database in created)
        {
            using var setup = CampaignSql.Open(_connectionString, database);
            CampaignSql.Execute(setup, Expand(File.ReadAllText(Path.Combine(_scriptDirectory, "rebind.sql")), databases, 0));
            CampaignSql.Execute(setup, "EXEC sys.sp_updateextendedproperty @name=N'SgCampaignTemplate',@value=N'1';");
        }
        foreach (var role in Roles)
        {
            using var template = CampaignSql.Open(_connectionString, databases[role]);
            var previousHash = Convert.ToString(CampaignSql.Scalar(template, "SELECT value FROM sys.extended_properties WHERE class=0 AND name=N'SgCampaignWorldHash'"));
            if (!string.IsNullOrEmpty(previousHash) && previousHash != scriptHash) throw new InvalidOperationException("Template scripts changed; explicitly rebuild owned templates before proceeding.");
            if (string.IsNullOrEmpty(previousHash)) CampaignSql.Execute(template, $"EXEC sys.sp_addextendedproperty @name=N'SgCampaignWorldHash',@value={CampaignSql.Literal(scriptHash)};");
            CampaignSql.Execute(admin, $"BACKUP DATABASE {CampaignSql.Quote(databases[role])} TO DISK={CampaignSql.Literal(_backups[role])} WITH INIT, COPY_ONLY, CHECKSUM;");
        }
    }
    public CampaignDatabaseSet Clone()
    {
        var set = new CampaignDatabaseSet(_connectionString);
        set.Reserve();
        try
        {
            using var admin = CampaignSql.Open(_connectionString);
            var directory = Convert.ToString(CampaignSql.Scalar(admin, "SELECT SERVERPROPERTY('InstanceDefaultDataPath')"))!;
            var separator = directory.Contains('/') ? "/" : "\\";
            foreach (var role in Roles)
            {
                var database = set.Databases[role];
                var files = CampaignSql.Rows(admin, $"RESTORE FILELISTONLY FROM DISK={CampaignSql.Literal(_backups[role])}");
                var moves = files.Select((file, index) => $"MOVE {CampaignSql.Literal(file[0])} TO {CampaignSql.Literal(directory.TrimEnd('/', '\\') + separator + database + index + (file[2] == "L" ? ".ldf" : ".mdf"))}");
                CampaignSql.Execute(admin, $"RESTORE DATABASE {CampaignSql.Quote(database)} FROM DISK={CampaignSql.Literal(_backups[role])} WITH {string.Join(",", moves)}, RECOVERY; ALTER DATABASE {CampaignSql.Quote(database)} SET NEW_BROKER WITH ROLLBACK IMMEDIATE;");
            }
            foreach (var database in set.Databases.Values)
            {
                using var setup = CampaignSql.Open(_connectionString, database);
                CampaignSql.Execute(setup, Expand(File.ReadAllText(Path.Combine(_scriptDirectory, "rebind.sql")), set.Databases, 0));
            }
            var password = Guid.NewGuid().ToString("N") + "aA!9";
            CampaignSql.Execute(admin, $"CREATE LOGIN {CampaignSql.Quote(set.Login)} WITH PASSWORD={CampaignSql.Literal(password)}, CHECK_POLICY=OFF;");
            foreach (var database in set.Databases.Values)
            {
                using var setup = CampaignSql.Open(_connectionString, database);
                CampaignSql.Execute(setup, $"CREATE USER runner FOR LOGIN {CampaignSql.Quote(set.Login)} WITH DEFAULT_SCHEMA=app; ALTER ROLE db_owner ADD MEMBER runner; DENY BACKUP DATABASE, BACKUP LOG TO runner;");
            }
            using var restricted = set.OpenCase("Core", "runner", false);
            if (Convert.ToInt32(CampaignSql.Scalar(restricted, "SELECT ISNULL(HAS_PERMS_BY_NAME(NULL,NULL,'CONTROL SERVER'),0)")) != 0 ||
                CampaignSql.Rows(restricted, "SELECT name FROM sys.databases WHERE HAS_DBACCESS(name)=1 AND name NOT IN ('master','tempdb','msdb')").Any(row => !set.Databases.Values.Contains(row[0], StringComparer.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Runner can access a database outside its disposable set.");
            return set;
        }
        catch { set.Dispose(); throw; }
    }
}

internal sealed class CampaignDatabaseSet : IDisposable
{
    private readonly string _connectionString;
    private readonly string _identity;
    public Dictionary<string, string> Databases { get; }
    public string Login => "SgRun_" + _identity;
    public CampaignDatabaseSet(string connectionString, string? identity = null)
    {
        _identity = identity ?? Guid.NewGuid().ToString("N");
        if (!System.Text.RegularExpressions.Regex.IsMatch(_identity, "^[a-f0-9]{32}$")) throw new InvalidDataException("Invalid owned-set identity.");
        _connectionString = connectionString;
        Databases = CampaignWorld.Roles.ToDictionary(role => role, role => "SgRun_" + _identity + "_" + role);
    }
    public void Reserve()
    {
        using var admin = CampaignSql.Open(_connectionString, "SgRun_Results");
        foreach (var database in Databases.Values)
            if (CampaignSql.Scalar(admin, $"SELECT DB_ID({CampaignSql.Literal(database)})") is not DBNull) throw new InvalidOperationException("Disposable name collision.");
        if (CampaignSql.Scalar(admin, $"SELECT SUSER_ID({CampaignSql.Literal(Login)})") is not DBNull) throw new InvalidOperationException("Login name collision.");
        CampaignSql.Execute(admin, $"INSERT dbo.CampaignOwnedSets(IdentityKey) VALUES({CampaignSql.Literal(_identity)});");
    }
    public static void Recover(string connectionString)
    {
        using var admin = CampaignSql.Open(connectionString, "SgRun_Results");
        foreach (var row in CampaignSql.Rows(admin, "SELECT IdentityKey FROM dbo.CampaignOwnedSets ORDER BY CreatedUtc"))
        {
            using var set = new CampaignDatabaseSet(connectionString, row[0]);
            Console.WriteLine("Recovering registered disposable set: " + row[0]);
        }
    }
    public SqlConnection OpenCase(string role, string principal, bool privileged)
    {
        var connection = CampaignSql.Open(_connectionString, Databases[role]);
        try
        {
            if (!privileged) CampaignSql.Execute(connection, $"EXECUTE AS LOGIN={CampaignSql.Literal(Login)} WITH NO REVERT;");
            if (principal != "runner") CampaignSql.Execute(connection, $"EXECUTE AS USER={CampaignSql.Literal(principal)};");
            return connection;
        }
        catch { connection.Dispose(); throw; }
    }
    public void Dispose()
    {
        using var admin = CampaignSql.Open(_connectionString);
        var errors = new List<string>();
        foreach (var database in Databases.Values)
        {
            try { CampaignSql.Execute(admin, $"IF DB_ID({CampaignSql.Literal(database)}) IS NOT NULL BEGIN ALTER DATABASE {CampaignSql.Quote(database)} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {CampaignSql.Quote(database)}; END"); }
            catch (Exception exception) { errors.Add(database + ": " + exception.Message); }
        }
        {
            try { CampaignSql.Execute(admin, $"IF SUSER_ID({CampaignSql.Literal(Login)}) IS NOT NULL DROP LOGIN {CampaignSql.Quote(Login)};"); }
            catch (Exception exception) { errors.Add(Login + ": " + exception.Message); }
        }
        if (errors.Count > 0) throw new InvalidOperationException("Cleanup failed: " + string.Join("; ", errors));
        admin.ChangeDatabase("SgRun_Results");
        CampaignSql.Execute(admin, $"DELETE dbo.CampaignOwnedSets WHERE IdentityKey={CampaignSql.Literal(_identity)};");
    }
}
