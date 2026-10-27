using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Allow-list of "safe" tables. Patterns are case-insensitive, support * and ? wildcards and have their
    /// brackets stripped. A pattern with no dot matches the object name; with one dot it matches schema.object
    /// (a missing schema in the query counts as dbo); with more parts it matches the corresponding trailing
    /// parts (database.schema.object, server.database.schema.object). A query name that lacks a part the
    /// pattern requires (e.g. no database) does not match.
    /// </summary>
    public sealed class AllowList
    {
        private readonly List<Regex[]> _patterns = new List<Regex[]>();

        public AllowList(IEnumerable<string> patterns)
        {
            foreach (var raw in patterns)
            {
                var pattern = (raw ?? string.Empty).Trim();
                if (pattern.Length == 0 || pattern.StartsWith("--", StringComparison.Ordinal) || pattern.StartsWith("#", StringComparison.Ordinal))
                    continue;

                var parts = pattern.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty)
                    .Split('.')
                    .Select(p => p.Trim())
                    .ToArray();
                if (parts.Length > 4 || parts.Any(p => p.Length == 0))
                    continue;

                _patterns.Add(parts.Select(ToRegex).ToArray());
                Patterns.Add(pattern);
            }
        }

        public List<string> Patterns { get; } = new List<string>();

        public bool IsEmpty => _patterns.Count == 0;

        public static IEnumerable<string> SplitLines(string text) =>
            (text ?? string.Empty).Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>Matches a name given as its parts (any of server/database/schema may be null).</summary>
        public bool Matches(string server, string database, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // Rightmost first: object, schema (defaults to dbo), database, server.
            var actual = new[] { name, string.IsNullOrEmpty(schema) ? "dbo" : schema, database, server };

            foreach (var pattern in _patterns)
            {
                bool ok = true;
                for (int i = 0; i < pattern.Length && ok; i++)
                {
                    var part = actual[i];
                    ok = !string.IsNullOrEmpty(part) && pattern[pattern.Length - 1 - i].IsMatch(part);
                }

                if (ok)
                    return true;
            }

            return false;
        }

        private static Regex ToRegex(string part)
        {
            var body = Regex.Escape(part).Replace("\\*", ".*").Replace("\\?", ".");
            return new Regex("^" + body + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
