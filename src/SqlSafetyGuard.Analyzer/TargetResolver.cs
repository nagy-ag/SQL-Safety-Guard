using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    internal enum BaseTargetKind
    {
        Named,
        TableVariable,
        Remote,
        Unresolved,
    }

    /// <summary>A base object a DML statement writes to, after following aliases, CTEs and derived tables.</summary>
    internal sealed class BaseTarget
    {
        public BaseTarget(BaseTargetKind kind, TSqlFragment fragment, SchemaObjectName name, IEnumerable<string> via, string note = null)
        {
            Kind = kind;
            Fragment = fragment;
            Name = name;
            Via = via.ToList();
            Note = note;
        }

        public BaseTargetKind Kind { get; }

        /// <summary>The fragment naming the base object (a SchemaObjectName, a table variable, OPENQUERY...).</summary>
        public TSqlFragment Fragment { get; }

        /// <summary>Set when Kind is Named.</summary>
        public SchemaObjectName Name { get; }

        /// <summary>How the target was reached, e.g. "via alias o", "via CTE c".</summary>
        public List<string> Via { get; }

        public string Note { get; }
    }

    /// <summary>
    /// Resolves the target of INSERT/UPDATE/DELETE/MERGE to the base tables that are actually written:
    /// aliases declared in the FROM clause, CTEs (recursively, with a cycle guard) and derived tables.
    /// </summary>
    internal sealed class TargetResolver
    {
        private readonly Dictionary<string, CommonTableExpression> _ctes =
            new Dictionary<string, CommonTableExpression>(StringComparer.OrdinalIgnoreCase);

        public TargetResolver(WithCtesAndXmlNamespaces with)
        {
            if (with?.CommonTableExpressions == null)
                return;
            foreach (var cte in with.CommonTableExpressions)
            {
                var name = cte.ExpressionName?.Value;
                if (!string.IsNullOrEmpty(name))
                    _ctes[name] = cte;
            }
        }

        public List<BaseTarget> ResolveWriteTarget(TableReference target, FromClause from)
        {
            var empty = new string[0];

            switch (target)
            {
                case NamedTableReference named:
                {
                    var name = named.SchemaObject;
                    var id = SinglePartName(name);
                    if (id != null && !Classifier.IsTempName(id))
                    {
                        var exposed = FindExposedTable(from, id);
                        if (exposed != null && !ReferenceEquals(exposed, named))
                        {
                            bool isAlias = exposed.Alias != null && string.Equals(exposed.Alias.Value, id, StringComparison.OrdinalIgnoreCase);
                            var via = isAlias ? new[] { "via alias " + id } : empty;
                            return OrUnresolved(ResolveSource(exposed, via, new HashSet<string>(StringComparer.OrdinalIgnoreCase)), named, via);
                        }

                        if (_ctes.ContainsKey(id))
                            return OrUnresolved(ResolveSource(named, empty, new HashSet<string>(StringComparer.OrdinalIgnoreCase)), named, new[] { "via CTE " + id });
                    }

                    return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Named, name, name, empty) };
                }

                case VariableTableReference variable:
                    return new List<BaseTarget> { new BaseTarget(BaseTargetKind.TableVariable, variable.Variable, null, empty) };

                default:
                    if (IsRemote(target))
                        return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Remote, target, null, empty, "remote write") };
                    return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Unresolved, target, null, empty, "could not resolve the target") };
            }
        }

        private static List<BaseTarget> OrUnresolved(List<BaseTarget> resolved, TSqlFragment at, IEnumerable<string> via)
        {
            if (resolved.Count > 0)
                return resolved;
            return new List<BaseTarget> { new BaseTarget(BaseTargetKind.Unresolved, at, null, via, "could not resolve the base table") };
        }

        /// <summary>Base tables behind a table source (alias target, CTE, derived table, join).</summary>
        private List<BaseTarget> ResolveSource(TableReference source, IEnumerable<string> via, HashSet<string> visitingCtes)
        {
            var result = new List<BaseTarget>();
            var viaList = via.ToList();

            switch (source)
            {
                case NamedTableReference named:
                {
                    var id = SinglePartName(named.SchemaObject);
                    if (id != null && _ctes.TryGetValue(id, out var cte))
                    {
                        if (!visitingCtes.Add(id))
                        {
                            // Recursive CTE reference: its rows come from the anchor part, already collected.
                            break;
                        }

                        result.AddRange(ResolveQuery(cte.QueryExpression, viaList.Concat(new[] { "via CTE " + id }), visitingCtes));
                        visitingCtes.Remove(id);
                        break;
                    }

                    result.Add(new BaseTarget(BaseTargetKind.Named, named.SchemaObject, named.SchemaObject, viaList));
                    break;
                }

                case VariableTableReference variable:
                    result.Add(new BaseTarget(BaseTargetKind.TableVariable, variable.Variable, null, viaList));
                    break;

                case QueryDerivedTable derived:
                {
                    var alias = derived.Alias?.Value;
                    var derivedVia = alias != null && !viaList.Any(v => v.EndsWith(" " + alias, StringComparison.OrdinalIgnoreCase))
                        ? viaList.Concat(new[] { "via derived table " + alias })
                        : viaList;
                    result.AddRange(ResolveQuery(derived.QueryExpression, derivedVia, visitingCtes));
                    break;
                }

                case JoinTableReference join:
                    result.AddRange(ResolveSource(join.FirstTableReference, viaList, visitingCtes));
                    result.AddRange(ResolveSource(join.SecondTableReference, viaList, visitingCtes));
                    break;

                case JoinParenthesisTableReference parenthesis:
                    result.AddRange(ResolveSource(parenthesis.Join, viaList, visitingCtes));
                    break;

                default:
                    if (IsRemote(source))
                        result.Add(new BaseTarget(BaseTargetKind.Remote, source, null, viaList, "remote write"));
                    // Table-valued functions, VALUES lists etc. are not base tables.
                    break;
            }

            return result;
        }

        private List<BaseTarget> ResolveQuery(QueryExpression query, IEnumerable<string> via, HashSet<string> visitingCtes)
        {
            var result = new List<BaseTarget>();
            switch (query)
            {
                case QuerySpecification spec when spec.FromClause != null:
                    foreach (var tableReference in spec.FromClause.TableReferences)
                        result.AddRange(ResolveSource(tableReference, via, visitingCtes));
                    break;

                case BinaryQueryExpression binary:
                    result.AddRange(ResolveQuery(binary.FirstQueryExpression, via, visitingCtes));
                    result.AddRange(ResolveQuery(binary.SecondQueryExpression, via, visitingCtes));
                    break;

                case QueryParenthesisExpression parenthesis:
                    result.AddRange(ResolveQuery(parenthesis.QueryExpression, via, visitingCtes));
                    break;
            }

            return result;
        }

        /// <summary>Finds the FROM-clause table source exposed under <paramref name="id"/> (its alias, or its name when it has no alias).</summary>
        private static TableReferenceWithAlias FindExposedTable(FromClause from, string id)
        {
            if (from?.TableReferences == null)
                return null;

            TableReferenceWithAlias byName = null;
            foreach (var tableReference in Flatten(from.TableReferences))
            {
                if (tableReference.Alias != null)
                {
                    if (string.Equals(tableReference.Alias.Value, id, StringComparison.OrdinalIgnoreCase))
                        return tableReference;
                }
                else if (byName == null && tableReference is NamedTableReference named &&
                         string.Equals(named.SchemaObject?.BaseIdentifier?.Value, id, StringComparison.OrdinalIgnoreCase))
                {
                    byName = named;
                }
            }

            return byName;
        }

        private static IEnumerable<TableReferenceWithAlias> Flatten(IEnumerable<TableReference> references)
        {
            foreach (var reference in references)
            {
                switch (reference)
                {
                    case JoinTableReference join:
                        foreach (var inner in Flatten(new[] { join.FirstTableReference, join.SecondTableReference }))
                            yield return inner;
                        break;
                    case JoinParenthesisTableReference parenthesis:
                        foreach (var inner in Flatten(new[] { parenthesis.Join }))
                            yield return inner;
                        break;
                    case TableReferenceWithAlias withAlias:
                        yield return withAlias;
                        break;
                }
            }
        }

        internal static string SinglePartName(SchemaObjectName name)
        {
            if (name == null || name.BaseIdentifier == null)
                return null;
            if (name.SchemaIdentifier != null || name.DatabaseIdentifier != null || name.ServerIdentifier != null)
                return null;
            return name.BaseIdentifier.Value;
        }

        internal static bool IsRemote(TableReference reference) =>
            reference is OpenQueryTableReference ||
            reference is OpenRowsetTableReference ||
            reference is AdHocTableReference ||
            reference is BulkOpenRowset ||
            reference is InternalOpenRowset;
    }
}
