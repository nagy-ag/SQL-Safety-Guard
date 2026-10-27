using System.Reflection;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.ServerHarness;

/// <summary>An object a case refers to, so the sandbox can contain it (a statement on a missing table proves nothing).</summary>
internal sealed record ObjectName(string? Database, string Schema, string Name)
{
    public string Key => $"{Database?.ToLowerInvariant()}|{Schema.ToLowerInvariant()}|{Name.ToLowerInvariant()}";
}

/// <summary>Finds the tables, columns, sequences and schemas the corpus refers to (including inside literal dynamic SQL).</summary>
internal sealed class SchemaDiscovery
{
    public Dictionary<string, ObjectName> Tables { get; } = new();
    public Dictionary<string, ObjectName> Sequences { get; } = new();
    public HashSet<string> Columns { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static TSqlParser NewParser()
    {
        var best = typeof(TSqlParser).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && typeof(TSqlParser).IsAssignableFrom(t) && t.Name.StartsWith("TSql") && t.Name.EndsWith("Parser"))
            .Select(t => (Type: t, V: int.TryParse(t.Name[4..^6], out var v) ? v : 0))
            .OrderByDescending(x => x.V).First().Type;
        return (TSqlParser)Activator.CreateInstance(best, new object[] { true })!;
    }

    public void Add(string sql, int depth = 0)
    {
        if (depth > 3)
            return;
        var parser = NewParser();
        var fragment = parser.Parse(new StringReader(SqlCmdLite.StripForDiscovery(sql)), out _);
        if (fragment == null)
            return;
        var visitor = new Visitor(this, depth);
        fragment.Accept(visitor);
    }

    /// <summary>Temp tables a case uses but never creates (unit-test snippets), so the harness can create them first.</summary>
    public static List<string> TempTablesToProvide(string sql)
    {
        var fragment = NewParser().Parse(new StringReader(SqlCmdLite.StripForDiscovery(sql)), out _);
        if (fragment == null)
            return new List<string>();
        var finder = new TempFinder();
        fragment.Accept(finder);
        return finder.Used.Where(u => !finder.Created.Contains(u)).ToList();
    }

    private sealed class TempFinder : TSqlFragmentVisitor
    {
        public HashSet<string> Used { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Created { get; } = new(StringComparer.OrdinalIgnoreCase);

        private static string? Temp(SchemaObjectName? n) =>
            n?.BaseIdentifier?.Value is { } v && v.StartsWith('#') && n.ServerIdentifier == null ? v : null;

        public override void Visit(NamedTableReference node) { if (Temp(node.SchemaObject) is { } t) Used.Add(t); }
        public override void Visit(TruncateTableStatement node) { if (Temp(node.TableName) is { } t) Used.Add(t); }
        public override void Visit(AlterTableStatement node) { if (Temp(node.SchemaObjectName) is { } t) Used.Add(t); }
        public override void Visit(CreateIndexStatement node) { if (Temp(node.OnName) is { } t) Used.Add(t); }
        public override void Visit(SelectStatement node) { if (Temp(node.Into) is { } t) Created.Add(t); }
        public override void Visit(CreateTableStatement node) { if (Temp(node.SchemaObjectName) is { } t) Created.Add(t); }
    }

    private void AddTable(SchemaObjectName? n)
    {
        if (n?.BaseIdentifier == null || n.ServerIdentifier != null)
            return;
        var name = n.BaseIdentifier.Value;
        var schema = n.SchemaIdentifier?.Value ?? "dbo";
        var db = n.DatabaseIdentifier?.Value;
        if (string.IsNullOrEmpty(name) || name.StartsWith('#') || name.StartsWith('@') || name.Length > 100)
            return;
        if (schema.Equals("sys", StringComparison.OrdinalIgnoreCase) || schema.Equals("INFORMATION_SCHEMA", StringComparison.OrdinalIgnoreCase))
            return;
        if (db != null && !Sandbox.IsSandboxDatabase(db) && !db.Equals("tempdb", StringComparison.OrdinalIgnoreCase))
            return;
        var o = new ObjectName(db, schema, name);
        Tables.TryAdd(o.Key, o);
    }

    private sealed class Visitor : TSqlFragmentVisitor
    {
        private readonly SchemaDiscovery _d;
        private readonly int _depth;
        public Visitor(SchemaDiscovery d, int depth) { _d = d; _depth = depth; }

        public override void Visit(NamedTableReference node) => _d.AddTable(node.SchemaObject);
        public override void Visit(SelectStatement node) => _d.AddTable(node.Into);
        public override void Visit(TruncateTableStatement node) => _d.AddTable(node.TableName);
        public override void Visit(DropTableStatement node) { foreach (var o in node.Objects) _d.AddTable(o); }
        public override void Visit(AlterTableStatement node) => _d.AddTable(node.SchemaObjectName);
        public override void Visit(CreateIndexStatement node) => _d.AddTable(node.OnName);
        public override void Visit(AlterIndexStatement node) => _d.AddTable(node.OnName);
        public override void Visit(UpdateStatisticsStatement node) => _d.AddTable(node.SchemaObjectName);
        public override void Visit(BulkInsertBase node) => _d.AddTable(node.To);

        public override void Visit(NextValueForExpression node)
        {
            var n = node.SequenceName;
            if (n?.BaseIdentifier == null || n.ServerIdentifier != null) return;
            var o = new ObjectName(n.DatabaseIdentifier?.Value, n.SchemaIdentifier?.Value ?? "dbo", n.BaseIdentifier.Value);
            if (o.Database == null || Sandbox.IsSandboxDatabase(o.Database)) _d.Sequences.TryAdd(o.Key, o);
        }

        public override void Visit(ColumnReferenceExpression node)
        {
            var ids = node.MultiPartIdentifier?.Identifiers;
            if (ids == null || ids.Count == 0) return;
            AddColumn(ids[^1].Value);
        }

        public override void Visit(InsertSpecification node)
        {
            foreach (var c in node.Columns) Visit(c);
        }

        public override void Visit(DbccStatement node)
        {
            // DBCC CHECKIDENT ('dbo.T', ...): the table is a string.
            if (node.Literals.FirstOrDefault()?.Value is StringLiteral s)
                _d.AddTable(ParseName(s.Value));
        }

        public override void Visit(ExecutableStringList node)
        {
            if (node.Strings.All(s => s is StringLiteral))
                _d.Add(string.Concat(node.Strings.Cast<StringLiteral>().Select(s => s.Value)), _depth + 1);
        }

        public override void Visit(ExecuteParameter node)
        {
            if (node.ParameterValue is StringLiteral s && s.Value.Length > 6)
                _d.Add(s.Value, _depth + 1);
        }

        private void AddColumn(string name)
        {
            if (string.IsNullOrEmpty(name) || name.StartsWith('@') || name.StartsWith('$') || name.StartsWith('#') || name == "*" || name.Length > 100)
                return;
            if (_d.Columns.Count < 700) _d.Columns.Add(name);
        }

        private static SchemaObjectName? ParseName(string text)
        {
            var parser = NewParser();
            var f = parser.Parse(new StringReader("SELECT * FROM " + text), out var errors);
            if (errors.Count > 0) return null;
            var finder = new FirstNamed();
            f.Accept(finder);
            return finder.Name;
        }

        private sealed class FirstNamed : TSqlFragmentVisitor
        {
            public SchemaObjectName? Name;
            public override void Visit(NamedTableReference node) => Name ??= node.SchemaObject;
        }
    }
}
