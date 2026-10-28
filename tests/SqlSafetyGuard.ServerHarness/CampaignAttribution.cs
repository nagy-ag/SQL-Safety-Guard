using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record CampaignEffectEdge(string Source, string Target, string Reason);

internal static class CampaignAttribution
{
    internal static string Canonical(string name) => name.Replace("[", "").Replace("]", "").Replace("\"", "").ToLowerInvariant();

    internal static bool Matches(string target, string modified, string currentDatabase, string defaultSchema = "app", IReadOnlyCollection<string>? knownObjects = null)
    {
        target = Canonical(target);
        modified = Canonical(modified);
        string database = Canonical(currentDatabase);
        if (!target.Contains('.'))
        {
            string preferred = database + "." + defaultSchema + "." + target;
            if (knownObjects?.Any(name => Canonical(name) == preferred) == true) return modified == preferred;
            return modified == preferred || modified == database + ".dbo." + target || modified == database + "." + target;
        }
        return modified == target || modified == database + "." + target;
    }

    internal static string Classify(IReadOnlyList<string> targets, IReadOnlyList<string> modified, string database,
        IReadOnlyList<CampaignEffectEdge> edges, string defaultSchema = "app", IReadOnlyCollection<string>? knownObjects = null)
    {
        if (targets.Count == 0 || targets.All(target => target.StartsWith('@'))) return "UNRESOLVED TARGET";
        var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in modified.Concat(edges.Select(edge => edge.Source)))
            if (targets.Any(target => Matches(target, name, database, defaultSchema, knownObjects))) reachable.Add(Canonical(name));
        bool progress;
        do
        {
            progress = false;
            foreach (var edge in edges)
                if (reachable.Contains(Canonical(edge.Source))) progress |= reachable.Add(Canonical(edge.Target));
        } while (progress);
        if (modified.Any(name => !reachable.Contains(Canonical(name))))
            return targets.Any(target => target.TrimStart().StartsWith("OPENQUERY", StringComparison.OrdinalIgnoreCase)
                || target.TrimStart().StartsWith("OPENROWSET", StringComparison.OrdinalIgnoreCase)) ? "UNRESOLVED TARGET" : "WRONG TARGET";
        return modified.Any(name => !targets.Any(target => Matches(target, name, database, defaultSchema, knownObjects))) ? "INDIRECT EFFECT" : "PASS";
    }

    internal static List<CampaignEffectEdge> Capture(string connectionString, IReadOnlyDictionary<string, string> databases, int timeout)
    {
        var edges = new List<CampaignEffectEdge>();
        foreach (string database in databases.Values)
        {
            using var connection = CampaignSql.Open(connectionString, database);
            string Local(string name) => database + "." + name;
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(SCHEMA_NAME(schema_id))+'.'+QUOTENAME(name),base_object_name FROM sys.synonyms", timeout))
            {
                string target = row[1];
                if (target.Count(character => character == '.') == 1) target = Local(target);
                edges.Add(new(Local(row[0]), target, "synonym"));
            }
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(OBJECT_SCHEMA_NAME(object_id))+'.'+QUOTENAME(name),QUOTENAME(OBJECT_SCHEMA_NAME(history_table_id))+'.'+QUOTENAME(OBJECT_NAME(history_table_id)) FROM sys.tables WHERE temporal_type=2 UNION ALL SELECT QUOTENAME(OBJECT_SCHEMA_NAME(referenced_object_id))+'.'+QUOTENAME(OBJECT_NAME(referenced_object_id)),QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))+'.'+QUOTENAME(OBJECT_NAME(parent_object_id)) FROM sys.foreign_keys WHERE is_disabled=0 AND (delete_referential_action<>0 OR update_referential_action<>0)", timeout))
                edges.Add(new(Local(row[0]), Local(row[1]), "temporal/cascade"));
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(OBJECT_SCHEMA_NAME(defaults.parent_object_id))+'.'+QUOTENAME(OBJECT_NAME(defaults.parent_object_id)),COALESCE(QUOTENAME(deps.referenced_database_name)+'.','')+QUOTENAME(COALESCE(deps.referenced_schema_name,'dbo'))+'.'+QUOTENAME(deps.referenced_entity_name) FROM sys.default_constraints defaults JOIN sys.sql_expression_dependencies deps ON defaults.object_id=deps.referencing_id WHERE deps.referenced_class=1 AND deps.referenced_server_name IS NULL AND deps.referenced_entity_name IS NOT NULL", timeout))
                edges.Add(new(Local(row[0]), row[1].Count(character => character == '.') == 1 ? Local(row[1]) : row[1], "default dependency"));
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(functions.name),QUOTENAME(SCHEMA_NAME(tables.schema_id))+'.'+QUOTENAME(tables.name) FROM sys.partition_functions functions JOIN sys.partition_schemes schemes ON schemes.function_id=functions.function_id JOIN sys.indexes indexes ON indexes.data_space_id=schemes.data_space_id JOIN sys.tables tables ON tables.object_id=indexes.object_id WHERE tables.is_ms_shipped=0", timeout))
                edges.Add(new(Local(row[0]), Local(row[1]), "partition function"));
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(OBJECT_SCHEMA_NAME(views.object_id))+'.'+QUOTENAME(views.name),MIN(COALESCE(QUOTENAME(deps.referenced_database_name)+'.','')+QUOTENAME(COALESCE(deps.referenced_schema_name,'dbo'))+'.'+QUOTENAME(deps.referenced_entity_name)) FROM sys.views views JOIN sys.sql_expression_dependencies deps ON views.object_id=deps.referencing_id WHERE deps.referenced_class=1 AND deps.referenced_server_name IS NULL GROUP BY views.object_id,views.name HAVING COUNT(DISTINCT COALESCE(deps.referenced_database_name,'')+'.'+COALESCE(deps.referenced_schema_name,'dbo')+'.'+deps.referenced_entity_name)=1", timeout))
            {
                string target = row[1].Count(character => character == '.') == 1 ? Local(row[1]) : row[1];
                edges.Add(new(Local(row[0]), target, "single-base view"));
            }
            foreach (var row in CampaignSql.Rows(connection, "SELECT QUOTENAME(OBJECT_SCHEMA_NAME(parent_id))+'.'+QUOTENAME(OBJECT_NAME(parent_id)),OBJECT_DEFINITION(object_id) FROM sys.triggers WHERE parent_class=1 AND is_disabled=0", timeout))
            {
                var fragment = new TSql180Parser(true).Parse(new StringReader(row[1]), out var errors);
                if (errors.Count > 0) continue;
                var visitor = new WriteVisitor();
                fragment.Accept(visitor);
                foreach (string target in visitor.Targets)
                {
                    if (target.Count(character => character == '.') == 0) continue;
                    edges.Add(new(Local(row[0]), target.Count(character => character == '.') == 1 ? Local(target) : target, "trigger write"));
                }
            }
        }
        return edges;
    }

    internal static List<CampaignEffectEdge> Transfers(string sql, string database, string defaultSchema)
    {
        var fragment = new TSql180Parser(true).Parse(new StringReader(sql), out var errors);
        if (errors.Count != 0) return [];
        var visitor = new TransferVisitor(database, defaultSchema);
        fragment.Accept(visitor);
        return visitor.Edges;
    }

    internal static List<CampaignEffectEdge> VerifiedLoopback(IReadOnlyDictionary<string, string> databases, IEnumerable<string> objects)
    {
        return objects.Where(name => databases.Values.Any(database => Canonical(name).StartsWith(Canonical(database) + ".", StringComparison.Ordinal)))
            .Select(name => new CampaignEffectEdge("SgLoop." + name, name, "verified disposable loopback")).ToList();
    }

    private sealed class TransferVisitor(string database, string defaultSchema) : TSqlFragmentVisitor
    {
        internal List<CampaignEffectEdge> Edges { get; } = [];
        public override void ExplicitVisit(AlterSchemaStatement node)
        {
            var identifiers = node.ObjectName.Identifiers.Select(identifier => identifier.Value).ToArray();
            if (identifiers.Length is < 1 or > 2) return;
            string schema = identifiers.Length == 2 ? identifiers[0] : defaultSchema;
            string name = identifiers[^1];
            Edges.Add(new(database + "." + schema + "." + name, database + "." + node.Name.Value + "." + name, "schema transfer"));
        }
    }

    private sealed class WriteVisitor : TSqlFragmentVisitor
    {
        internal List<string> Targets { get; } = [];
        private void Add(TableReference target)
        {
            if (target is NamedTableReference named) Targets.Add(string.Join(".", named.SchemaObject.Identifiers.Select(identifier => identifier.Value)));
        }
        public override void ExplicitVisit(InsertSpecification node) { Add(node.Target); base.ExplicitVisit(node); }
        public override void ExplicitVisit(UpdateSpecification node) { Add(node.Target); base.ExplicitVisit(node); }
        public override void ExplicitVisit(DeleteSpecification node) { Add(node.Target); base.ExplicitVisit(node); }
        public override void ExplicitVisit(MergeSpecification node) { Add(node.Target); base.ExplicitVisit(node); }
    }
}
