using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class AllowList
    {
        private readonly List<Regex[]> _patterns = new List<Regex[]>();

        public AllowList(IEnumerable<string> patterns, bool procedures = false)
        {
            foreach (var raw in patterns ?? Enumerable.Empty<string>())
            {
                var pattern = (raw ?? string.Empty).Trim();
                if (pattern.Length == 0 || pattern.StartsWith("--", StringComparison.Ordinal) || pattern.StartsWith("#", StringComparison.Ordinal))
                    continue;
                var parts = SplitName(pattern);
                if (parts == null || parts.Length > 4 || parts.Any(part => part.Length == 0) ||
                    parts.All(part => part.All(character => character == '*' || character == '?')) ||
                    (procedures && (parts.Length < 2 || parts.Take(parts.Length - 1).Any(part => part.IndexOfAny(new[] { '*', '?' }) >= 0))))
                {
                    Warnings.Add("Ignored invalid " + (procedures ? "trusted procedure" : "allow-list") + " pattern: " + pattern);
                    continue;
                }
                _patterns.Add(parts.Select(ToRegex).ToArray());
                Patterns.Add(pattern);
            }
        }

        public List<string> Patterns { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public bool IsEmpty => _patterns.Count == 0;

        public static IEnumerable<string> SplitLines(string text) =>
            (text ?? string.Empty).Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

        public bool Matches(string server, string database, string schema, string name,
            string currentDatabase = null, string defaultSchema = null)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var actual = new[] { name, string.IsNullOrEmpty(schema) ? defaultSchema : schema, database ?? currentDatabase, server };
            foreach (var pattern in _patterns)
            {
                if (pattern.Length < 4 && !string.IsNullOrEmpty(server)) continue;
                if (pattern.Length < 3 && !string.IsNullOrEmpty(database) &&
                    !string.Equals(database, currentDatabase, StringComparison.OrdinalIgnoreCase)) continue;
                bool matches = true;
                for (int index = 0; index < pattern.Length && matches; index++)
                    matches = !string.IsNullOrEmpty(actual[index]) && pattern[pattern.Length - 1 - index].IsMatch(actual[index]);
                if (matches) return true;
            }
            return false;
        }

        internal static Regex ToRegex(string part) => new Regex("\\A" + Regex.Escape(part).Replace("\\*", ".*").Replace("\\?", ".") + "\\z",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

        internal static string[] SplitName(string pattern)
        {
            var parts = new List<string>();
            var value = new StringBuilder();
            char quoted = '\0';
            bool closed = false;
            for (int index = 0; index < pattern.Length; index++)
            {
                char character = pattern[index];
                if (quoted != '\0')
                {
                    if (character == quoted)
                    {
                        if (index + 1 < pattern.Length && pattern[index + 1] == quoted) { value.Append(character); index++; }
                        else { quoted = '\0'; closed = true; }
                    }
                    else value.Append(character);
                }
                else if (character == '.') { parts.Add(closed ? value.ToString() : value.ToString().Trim()); value.Clear(); closed = false; }
                else if ((character == '[' || character == '"') && value.ToString().Trim().Length == 0 && !closed)
                {
                    value.Clear();
                    quoted = character == '[' ? ']' : '"';
                }
                else if (closed) { if (!char.IsWhiteSpace(character)) return null; }
                else value.Append(character);
            }
            if (quoted != '\0') return null;
            parts.Add(closed ? value.ToString() : value.ToString().Trim());
            return parts.ToArray();
        }
    }
}
