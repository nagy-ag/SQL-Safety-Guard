using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Analyzer
{
    public sealed class EnvironmentRules
    {
        private sealed class Rule
        {
            public Regex Server { get; set; }
            public Regex Database { get; set; }
            public GuardSettings Settings { get; set; }
        }

        private readonly List<Rule> _rules = new List<Rule>();
        public IReadOnlyList<string> Warnings { get; }

        public EnvironmentRules(string text)
        {
            var warnings = new List<string>();
            var lines = (text ?? string.Empty).Replace("\r", string.Empty).Split('\n');
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (line.Length == 0 || line.StartsWith("--", StringComparison.Ordinal)) continue;
                var fields = line.Split('|').Select(field => field.Trim()).ToArray();
                if (fields.Length < 5 || fields.Length > 7 || fields[0].Length == 0 || fields[2].Length == 0 ||
                    !Enum.TryParse(fields[3], true, out ExecutionMode mode) || !Enum.IsDefined(typeof(ExecutionMode), mode) ||
                    !bool.TryParse(fields[4], out bool typed))
                {
                    warnings.Add($"Environment rule line {index + 1} ignored: expected server | database | label | Off/Red/YellowRed | true/false | allow-list | trusted procedures");
                    continue;
                }
                var allowList = new AllowList(fields.Length > 5 ? AllowList.SplitLines(fields[5]) : null);
                var trusted = new AllowList(fields.Length > 6 ? AllowList.SplitLines(fields[6]) : null, procedures: true);
                if (allowList.Warnings.Count > 0 || trusted.Warnings.Count > 0)
                {
                    warnings.Add($"Environment rule line {index + 1} ignored: " + string.Join("; ", allowList.Warnings.Concat(trusted.Warnings)));
                    continue;
                }
                _rules.Add(new Rule
                {
                    Server = AllowList.ToRegex(fields[0]),
                    Database = fields[1].Length == 0 ? null : AllowList.ToRegex(fields[1]),
                    Settings = new GuardSettings(mode, fields[2], typed, allowList.Patterns, trusted.Patterns,
                        $"line {index + 1}: {fields[0]} | {(fields[1].Length == 0 ? "(any database)" : fields[1])}"),
                });
            }
            Warnings = warnings;
        }

        public GuardSettings Resolve(string server, string database, GuardSettings global)
        {
            if (global == null) throw new ArgumentNullException(nameof(global));
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database)) return global;
            foreach (var rule in _rules)
                if (rule.Server.IsMatch(server) && (rule.Database == null || rule.Database.IsMatch(database)))
                    return rule.Settings.Add(global);
            return global;
        }
    }
}
