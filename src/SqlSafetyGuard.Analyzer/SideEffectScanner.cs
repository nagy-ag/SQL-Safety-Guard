using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Finds side effects hidden in expressions and FROM clauses of statements that otherwise only read:
    /// remote pass-through sources (OPENQUERY / OPENROWSET / OPENDATASOURCE run text on another server, which
    /// may change data there) and NEXT VALUE FOR (advances a sequence; ROLLBACK does not undo it).
    /// </summary>
    internal static class SideEffectScanner
    {
        public sealed class RemoteSource
        {
            public RemoteSource(TableReference reference, string action, string note)
            {
                Reference = reference;
                Action = action;
                Note = note;
            }

            public TableReference Reference { get; }

            public string Action { get; }

            public string Note { get; }
        }

        public static void Scan(TSqlFragment fragment, out List<RemoteSource> remoteSources, out List<NextValueForExpression> sequences)
        {
            var collector = new Collector();
            fragment?.Accept(collector);
            remoteSources = collector.RemoteSources;
            sequences = collector.Sequences;
        }

        /// <summary>
        /// Remote references that the statement writes to (directly or through an alias/CTE). Those are already
        /// reported as remote writes, so they are not reported again as remote sources.
        /// </summary>
        public static HashSet<TSqlFragment> WrittenFragments(TSqlStatement statement)
        {
            var written = new HashSet<TSqlFragment>();

            void AddTarget(TableReference target, FromClause from, WithCtesAndXmlNamespaces with)
            {
                if (target == null)
                    return;
                written.Add(target);
                foreach (var baseTarget in new TargetResolver(with).ResolveWriteTarget(target, from))
                {
                    if (baseTarget.Fragment != null)
                        written.Add(baseTarget.Fragment);
                }
            }

            void AddOutput(OutputIntoClause output)
            {
                if (output?.IntoTable != null)
                    written.Add(output.IntoTable);
            }

            switch (statement)
            {
                case InsertStatement insert:
                    AddTarget(insert.InsertSpecification.Target, null, insert.WithCtesAndXmlNamespaces);
                    AddOutput(insert.InsertSpecification.OutputIntoClause);
                    break;
                case UpdateStatement update:
                    AddTarget(update.UpdateSpecification.Target, update.UpdateSpecification.FromClause, update.WithCtesAndXmlNamespaces);
                    AddOutput(update.UpdateSpecification.OutputIntoClause);
                    break;
                case DeleteStatement delete:
                    AddTarget(delete.DeleteSpecification.Target, delete.DeleteSpecification.FromClause, delete.WithCtesAndXmlNamespaces);
                    AddOutput(delete.DeleteSpecification.OutputIntoClause);
                    break;
                case MergeStatement merge:
                    AddTarget(merge.MergeSpecification.Target, null, merge.WithCtesAndXmlNamespaces);
                    AddOutput(merge.MergeSpecification.OutputIntoClause);
                    break;
            }

            return written;
        }

        private sealed class Collector : TSqlFragmentVisitor
        {
            public List<RemoteSource> RemoteSources { get; } = new List<RemoteSource>();

            public List<NextValueForExpression> Sequences { get; } = new List<NextValueForExpression>();

            public override void Visit(OpenQueryTableReference node) =>
                RemoteSources.Add(new RemoteSource(node, "OPENQUERY",
                    $"runs a pass-through query on linked server {node.LinkedServer?.Value}; it may change data there"));

            public override void Visit(OpenRowsetTableReference node) =>
                RemoteSources.Add(new RemoteSource(node, "OPENROWSET", "runs a query through a remote provider; it may change data there"));

            public override void Visit(InternalOpenRowset node) =>
                RemoteSources.Add(new RemoteSource(node, "OPENROWSET", "runs through a remote provider; the remote side is not analyzed"));

            public override void Visit(AdHocTableReference node) =>
                RemoteSources.Add(new RemoteSource(node, "OPENDATASOURCE", "opens an ad hoc remote connection; the remote side is not analyzed"));

            // OPENROWSET(BULK 'file', ...) only reads a file: not collected.

            public override void Visit(NextValueForExpression node) => Sequences.Add(node);
        }
    }
}
