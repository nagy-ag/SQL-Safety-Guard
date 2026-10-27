using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Creates the newest TSqlNNNParser available in the loaded ScriptDom assembly, so the newest syntax parses
    /// even when a newer ScriptDom is picked up at runtime than the one compiled against.
    /// </summary>
    internal static class ParserFactory
    {
        private static readonly Lazy<Tuple<ConstructorInfo, string>> Best = new Lazy<Tuple<ConstructorInfo, string>>(FindBest);

        public static string ParserName => Best.Value.Item2;

        public static TSqlParser Create()
        {
            var ctor = Best.Value.Item1;
            if (ctor != null)
                return (TSqlParser)ctor.Invoke(new object[] { true });
            return new TSql160Parser(true);
        }

        private static Tuple<ConstructorInfo, string> FindBest()
        {
            try
            {
                Type[] types;
                try
                {
                    types = typeof(TSqlParser).Assembly.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                var regex = new Regex(@"^TSql(\d+)Parser$");
                var best = types
                    .Where(t => !t.IsAbstract && typeof(TSqlParser).IsAssignableFrom(t))
                    .Select(t => new { Type = t, Match = regex.Match(t.Name) })
                    .Where(x => x.Match.Success)
                    .Select(x => new { x.Type, Version = int.Parse(x.Match.Groups[1].Value) })
                    .OrderByDescending(x => x.Version)
                    .Select(x => new { x.Type, Ctor = x.Type.GetConstructor(new[] { typeof(bool) }) })
                    .FirstOrDefault(x => x.Ctor != null);

                if (best != null)
                    return Tuple.Create(best.Ctor, best.Type.Name);
            }
            catch
            {
                // Fall back to a known parser below.
            }

            return Tuple.Create<ConstructorInfo, string>(null, nameof(TSql160Parser));
        }
    }
}
