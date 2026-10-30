#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Options
{
    // Shared as source with the unit tests. No shell, persistence or analyzer verdict changes here.
    internal sealed class SettingsSnapshot
    {
        public SettingsSnapshot(ExecutionMode guardMode = ExecutionMode.YellowRed, bool showServerAndDatabase = true,
            string allowList = "", bool tempdbIsTemp = false, string trustedProcedures = "", string environmentRules = "")
        {
            GuardMode = guardMode;
            ShowServerAndDatabase = showServerAndDatabase;
            AllowList = Normalize(allowList);
            TempdbIsTemp = tempdbIsTemp;
            TrustedProcedures = Normalize(trustedProcedures);
            EnvironmentRules = Normalize(environmentRules);
        }

        public ExecutionMode GuardMode { get; }
        public bool ShowServerAndDatabase { get; }
        public string AllowList { get; }
        public bool TempdbIsTemp { get; }
        public string TrustedProcedures { get; }
        public string EnvironmentRules { get; }
        private static string Normalize(string value) => (value ?? "").Replace("\r\n", "\n").Replace('\r', '\n');
    }

    internal sealed class SettingsChange
    {
        public SettingsChange(string name, string before, string after) { Name = name; Before = before; After = after; }
        public string Name { get; }
        public string Before { get; }
        public string After { get; }
    }

    internal sealed class SettingsImport
    {
        internal SettingsImport(SettingsSnapshot current, SettingsSnapshot proposed, string source,
            IEnumerable<string> ignored, IEnumerable<SettingsChange> changes, IEnumerable<string> warnings)
        {
            Current = current;
            Proposed = proposed;
            Source = source;
            IgnoredKeys = Array.AsReadOnly(ignored.ToArray());
            Changes = Array.AsReadOnly(changes.ToArray());
            Warnings = Array.AsReadOnly(warnings.Distinct().ToArray());
        }
        public SettingsSnapshot Current { get; }
        public SettingsSnapshot Proposed { get; }
        public string Source { get; }
        public IReadOnlyList<string> IgnoredKeys { get; }
        public IReadOnlyList<SettingsChange> Changes { get; }
        public IReadOnlyList<string> Warnings { get; }
    }

    internal static class SettingsTransfer
    {
        public const int FormatVersion = 1;
        public const int MaximumFileCharacters = 1024 * 1024;
        private static readonly string[] SettingKeys = { "guardMode", "showServerAndDatabase", "allowList", "tempdbIsTemp", "trustedProcedures", "environmentRules" };

        public static string Export(SettingsSnapshot settings, string extensionVersion, DateTimeOffset exportedAt)
        {
            return new JObject(
                new JProperty("formatVersion", FormatVersion),
                new JProperty("extensionVersion", extensionVersion),
                new JProperty("exportedAt", exportedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
                new JProperty("settings", new JObject(
                    new JProperty("guardMode", settings.GuardMode.ToString()),
                    new JProperty("showServerAndDatabase", settings.ShowServerAndDatabase),
                    new JProperty("allowList", Lines(settings.AllowList)),
                    new JProperty("tempdbIsTemp", settings.TempdbIsTemp),
                    new JProperty("trustedProcedures", Lines(settings.TrustedProcedures)),
                    new JProperty("environmentRules", Lines(settings.EnvironmentRules)))))
                .ToString(Formatting.Indented) + Environment.NewLine;
        }

        private static JArray Lines(string value) => new JArray(value.Length == 0 ? new string[0] : value.Split('\n'));

        public static SettingsImport PrepareImport(string json, SettingsSnapshot current)
        {
            if (json == null || json.Length > MaximumFileCharacters)
                throw new FormatException("Settings file is empty or exceeds the 1 MB character limit.");
            JObject root;
            try
            {
                using (var reader = new JsonTextReader(new StringReader(json)) { DateParseHandling = DateParseHandling.None, MaxDepth = 32 })
                {
                    root = JObject.Load(reader, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
                    if (reader.Read()) throw new JsonReaderException("Unexpected content after the settings object.");
                }
            }
            catch (JsonException ex) { throw new FormatException("Unreadable settings JSON: " + ex.Message, ex); }
            var version = Require(root, "formatVersion", JTokenType.Integer);
            if (version.ToString() != FormatVersion.ToString(CultureInfo.InvariantCulture))
                throw new FormatException("Unsupported formatVersion " + version + ". This extension supports formatVersion 1.");
            string extensionVersion = Require(root, "extensionVersion", JTokenType.String).Value<string>();
            string timestamp = Require(root, "exportedAt", JTokenType.String).Value<string>();
            if (string.IsNullOrWhiteSpace(extensionVersion) || !DateTimeOffset.TryParse(timestamp, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _)) throw new FormatException("extensionVersion must be nonempty and exportedAt must be a valid timestamp.");
            var data = (JObject)Require(root, "settings", JTokenType.Object);
            string modeText = Require(data, "guardMode", JTokenType.String).Value<string>();
            if (!new[] { "Off", "Red", "YellowRed" }.Contains(modeText, StringComparer.OrdinalIgnoreCase))
                throw new FormatException("guardMode must be Off, Red or YellowRed.");
            var proposed = new SettingsSnapshot((ExecutionMode)Enum.Parse(typeof(ExecutionMode), modeText, true),
                Require(data, "showServerAndDatabase", JTokenType.Boolean).Value<bool>(), ReadLines(data, "allowList"),
                Require(data, "tempdbIsTemp", JTokenType.Boolean).Value<bool>(), ReadLines(data, "trustedProcedures"), ReadLines(data, "environmentRules"));
            var errors = new AllowList(AllowList.SplitLines(proposed.AllowList)).Warnings
                .Concat(new AllowList(AllowList.SplitLines(proposed.TrustedProcedures), procedures: true).Warnings)
                .Concat(new EnvironmentRules(proposed.EnvironmentRules).Warnings).ToArray();
            if (errors.Length > 0) throw new FormatException("Invalid settings:\n" + string.Join("\n", errors));
            var ignored = root.Properties().Where(p => !new[] { "formatVersion", "extensionVersion", "exportedAt", "settings" }.Contains(p.Name))
                .Select(p => p.Name).Concat(data.Properties().Where(p => !SettingKeys.Contains(p.Name)).Select(p => "settings." + p.Name));
            var changes = Diff(current, proposed);
            var warnings = Widening(current, proposed);
            return new SettingsImport(current, proposed, "Exported by SQL Safety Guard " + extensionVersion + " at " + timestamp,
                ignored, changes, warnings);
        }

        private static JToken Require(JObject data, string name, JTokenType type)
        {
            var value = data[name];
            if (value == null || value.Type != type) throw new FormatException(name + " must be " + type + ".");
            return value;
        }

        private static string ReadLines(JObject data, string name)
        {
            var lines = (JArray)Require(data, name, JTokenType.Array);
            if (lines.Any(line => line.Type != JTokenType.String || line.Value<string>().IndexOfAny(new[] { '\r', '\n' }) >= 0))
                throw new FormatException(name + " must contain only strings, one line per array entry.");
            return string.Join("\n", lines.Select(line => line.Value<string>()));
        }

        public static IReadOnlyList<SettingsChange> Diff(SettingsSnapshot before, SettingsSnapshot after)
        {
            var changes = new List<SettingsChange>();
            AddChange(changes, "F5 guard", before.GuardMode.ToString(), after.GuardMode.ToString());
            AddChange(changes, "Show server and database", before.ShowServerAndDatabase.ToString(), after.ShowServerAndDatabase.ToString());
            AddChange(changes, "Safe tables allow-list", before.AllowList, after.AllowList);
            AddChange(changes, "Treat permanent tempdb objects as temporary", before.TempdbIsTemp.ToString(), after.TempdbIsTemp.ToString());
            AddChange(changes, "Trusted read-only procedures", before.TrustedProcedures, after.TrustedProcedures);
            AddChange(changes, "Server / environment rules (first match wins)", before.EnvironmentRules, after.EnvironmentRules);
            return changes.AsReadOnly();
        }

        public static void Apply(SettingsImport import, Func<SettingsSnapshot> capture, Action<SettingsSnapshot> assign, Action save)
        {
            if (Diff(import.Current, capture()).Count != 0)
                throw new InvalidOperationException("Settings changed while the preview was open. Import again to review the new differences.");
            try { assign(import.Proposed); save(); }
            catch (Exception saveError)
            {
                assign(import.Current);
                try { save(); }
                catch (Exception rollbackError)
                {
                    throw new InvalidOperationException("Saving failed and restoring stored settings also failed. The previous settings remain active in this session. " +
                        "Review Tools > Options before restarting SSMS. " + rollbackError.Message, saveError);
                }
                throw;
            }
        }

        private static void AddChange(List<SettingsChange> changes, string name, string before, string after)
        {
            if (before != after) changes.Add(new SettingsChange(name, before.Length == 0 ? "(empty)" : before, after.Length == 0 ? "(empty)" : after));
        }

        private static IEnumerable<string> Widening(SettingsSnapshot before, SettingsSnapshot after)
        {
            var warnings = new List<string>();
            CompareSettings(Global(before), Global(after), "Global", warnings);
            if (!before.TempdbIsTemp && after.TempdbIsTemp)
                warnings.Add("Permanent tempdb objects will count as temporary: writes to them will be green.");
            if (before.EnvironmentRules != after.EnvironmentRules || before.GuardMode != after.GuardMode ||
                before.AllowList != after.AllowList || before.TrustedProcedures != after.TrustedProcedures)
            {
                // Enumerate every distinct matching region of both sets of glob selectors. Comparing only
                // added lines misses removals, reordered first matches and global fallback changes.
                var selectors = RuleSelectors(before.EnvironmentRules).Concat(RuleSelectors(after.EnvironmentRules)).ToArray();
                var servers = GlobRegions.Representatives(selectors.Select(p => p[0]));
                var databases = GlobRegions.Representatives(selectors.Select(p => p[1].Length == 0 ? "*" : p[1])
                    .Concat(DatabaseQualifiers(before)).Concat(DatabaseQualifiers(after)));
                if ((long)servers.Count * databases.Count > 16384)
                    throw new FormatException("Environment rules are too complex to safely preview. Reduce the number of overlapping selectors.");
                var oldRules = new EnvironmentRules(before.EnvironmentRules);
                var newRules = new EnvironmentRules(after.EnvironmentRules);
                var comparedPairs = new HashSet<string>();
                foreach (string server in servers)
                    foreach (string database in databases)
                    {
                        var oldSettings = oldRules.Resolve(server, database, Global(before));
                        var newSettings = newRules.Resolve(server, database, Global(after));
                        // Globals have their own warning; include fallback only if it replaces an override.
                        if (oldSettings.MatchedRule == null && newSettings.MatchedRule == null) continue;
                        if (!comparedPairs.Add(oldSettings.MatchedRule + "\n" + newSettings.MatchedRule + "\n" + database)) continue;
                        CompareSettings(oldSettings, newSettings, "Environment " + newSettings.Environment +
                            " (example server '" + server + "', database '" + database + "')", warnings, database);
                    }
            }
            return warnings;
        }

        private static GuardSettings Global(SettingsSnapshot value) => new GuardSettings(value.GuardMode,
            allowList: AllowList.SplitLines(value.AllowList), trustedProcedures: AllowList.SplitLines(value.TrustedProcedures));

        private static IEnumerable<string> DatabaseQualifiers(SettingsSnapshot value)
        {
            var patterns = AllowList.SplitLines(value.AllowList).Concat(AllowList.SplitLines(value.TrustedProcedures));
            foreach (var fields in value.EnvironmentRules.Split('\n').Select(line => line.Trim()).Where(line => !line.StartsWith("--", StringComparison.Ordinal))
                .Select(line => line.Split('|')).Where(fields => fields.Length >= 5 && fields.Length <= 7))
                patterns = patterns.Concat(fields.Skip(5).SelectMany(AllowList.SplitLines));
            return patterns.Select(pattern => AllowList.SplitName(pattern.Trim())).Where(parts => parts != null && parts.Length == 3)
                .Select(parts => parts[0]);
        }

        private static IEnumerable<string[]> RuleSelectors(string text) => text.Split('\n').Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith("--", StringComparison.Ordinal))
            .Select(line => line.Split('|').Select(field => field.Trim()).ToArray())
            .Where(fields => fields.Length >= 5 && fields.Length <= 7).Select(fields => fields.Take(2).ToArray());

        private static void CompareSettings(GuardSettings before, GuardSettings after, string scope, List<string> warnings, string currentDatabase = null)
        {
            if (after.Mode < before.Mode)
                warnings.Add(scope + ": F5 guard is lowered from " + before.Mode + " to " + after.Mode +
                    ". " + (after.Mode == ExecutionMode.Off ? "Queries will run without guard prompts." : "Yellow queries will run without guard prompts."));
            ComparePatterns(before.AllowList, after.AllowList, false, scope, warnings, currentDatabase);
            ComparePatterns(before.TrustedProcedures, after.TrustedProcedures, true, scope, warnings, currentDatabase);
        }

        private static void ComparePatterns(IEnumerable<string> before, IEnumerable<string> after, bool procedures, string scope, List<string> warnings, string currentDatabase)
        {
            var oldPatterns = new AllowList(before, procedures).Patterns.Select(AllowList.SplitName).ToArray();
            var additions = new AllowList(after, procedures).Patterns.Where(pattern =>
            {
                var parts = AllowList.SplitName(pattern);
                return !PatternsCover(oldPatterns, parts, currentDatabase);
            }).Distinct(StringComparer.Ordinal).ToArray();
            if (additions.Length == 0) return;
            warnings.Add(scope + (procedures ? ": This file trusts " : ": This file allow-lists ") + additions.Length +
                (procedures ? " procedure pattern(s): calls matching them will be green. Trust is user asserted, not verified.\n" :
                " table pattern(s): writes matching them will be green.\n") + string.Join("\n", additions));
        }

        private static bool PatternsCover(string[][] oldPatterns, string[] candidate, string currentDatabase)
        {
            // One/two-part names match only the current local database. Three-part names also
            // match cross-database targets; four-part names require a server. Preserve these
            // boundaries when testing inclusion, rather than treating dots as ordinary glob text.
            if (candidate.Length == 4 || (candidate.Length == 3 && currentDatabase == null))
                return GlobRegions.TupleCovered(oldPatterns.Where(p => p.Length == candidate.Length).ToArray(), candidate);
            if (candidate.Length == 1 && !GlobRegions.TupleCovered(oldPatterns.Where(p => p.Length == 1).ToArray(), candidate))
                return false; // A schema-less target can have an unresolved schema.
            string[] Pad(string[] parts)
            {
                var padded = Enumerable.Repeat("*", 3 - parts.Length).Concat(parts).ToArray();
                if (parts.Length < 3 && currentDatabase != null) padded[0] = currentDatabase;
                return padded;
            }
            return GlobRegions.TupleCovered(oldPatterns.Where(p => p.Length <= 3).Select(Pad).ToArray(), Pad(candidate));
        }
    }

    // Finite-state exploration of glob languages (* and ?). Literal characters plus one 'other'
    // character cover the alphabet. A witness per accepting mask covers every first-match region.
    internal static class GlobRegions
    {
        private const int StateLimit = 4096;
        public static bool TupleCovered(string[][] broader, string[] narrower)
        {
            if (broader.Length == 0) return false;
            // The analyzer uses Regex IgnoreCase + CultureInvariant, whose case folding is
            // invariant lowercase. OrdinalIgnoreCase/uppercase would conflate long-s with S
            // even though the runtime regex distinguishes them, hiding a real expansion.
            if (broader.Any(parts => parts.Select(p => p.ToLowerInvariant()).SequenceEqual(narrower.Select(p => p.ToLowerInvariant())))) return true;
            var viable = new List<int[]> { Enumerable.Range(0, broader.Length).ToArray() };
            for (int part = 0; part < narrower.Length; part++)
            {
                var regions = Explore(new[] { narrower[part] }.Concat(broader.Select(parts => parts[part]))
                    .Select(pattern => pattern.ToLowerInvariant()).ToArray()).Where(region => region.Matches[0]).ToArray();
                var next = new Dictionary<string, int[]>();
                foreach (var remaining in viable)
                    foreach (var region in regions)
                    {
                        var matches = remaining.Where(index => region.Matches[index + 1]).ToArray();
                        if (matches.Length == 0) return false;
                        next[string.Join(",", matches)] = matches;
                        if (next.Count > StateLimit) throw new FormatException("Settings patterns are too complex to safely preview.");
                    }
                viable = next.Values.ToList();
            }
            return true;
        }

        public static IReadOnlyList<string> Representatives(IEnumerable<string> selectors) =>
            Explore(selectors.Select(p => p.ToLowerInvariant()).Distinct().ToArray(), excludeWhitespace: true).Select(region => region.Witness).ToArray();

        private sealed class Region
        {
            public string Witness;
            public bool[] Matches;
        }

        private static IReadOnlyList<Region> Explore(string[] patterns, bool excludeWhitespace = false)
        {
            if (patterns.Length > 128 || patterns.Sum(p => p.Length) > 4096)
                throw new FormatException("Settings patterns are too complex to safely preview. Simplify the patterns.");
            var literals = new HashSet<char>(patterns.SelectMany(p => p).Where(c => c != '*' && c != '?'));
            if (literals.Count > 128)
                throw new FormatException("Settings patterns use too many distinct characters to safely preview.");
            char other = 'x';
            while (literals.Contains(char.ToLowerInvariant(other)) || other == '*' || other == '?') other++;
            var alphabet = literals.Concat(new[] { char.ToLowerInvariant(other) }).ToArray();
            var initial = patterns.Select(p => Closure(p, new[] { 0 })).ToArray();
            var queue = new Queue<Tuple<int[][], string>>();
            var seen = new HashSet<string>();
            void Enqueue(int[][] state, string witness)
            {
                string key = (witness.Length == 0 ? "empty:" : excludeWhitespace && string.IsNullOrWhiteSpace(witness) ? "whitespace:" : "value:") +
                    string.Join("|", state.Select(s => string.Join(",", s)));
                if (!seen.Add(key)) return;
                if (seen.Count > StateLimit) throw new FormatException("Settings patterns are too complex to safely preview. Simplify the patterns.");
                queue.Enqueue(Tuple.Create(state, witness));
            }
            Enqueue(initial, "");
            var regions = new Dictionary<string, Region>();
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                var state = item.Item1;
                string witness = item.Item2;
                if (witness.Length > 0 && (!excludeWhitespace || !string.IsNullOrWhiteSpace(witness)))
                {
                    var matches = state.Select((s, i) => s.Contains(patterns[i].Length)).ToArray();
                    string mask = string.Join("", matches.Select(m => m ? "1" : "0"));
                    if (!regions.ContainsKey(mask)) regions.Add(mask, new Region { Witness = witness, Matches = matches });
                }
                foreach (char character in alphabet)
                {
                    var next = patterns.Select((pattern, i) => Closure(pattern, state[i].Where(pos => pos < pattern.Length &&
                        (pattern[pos] == '*' || pattern[pos] == '?' || pattern[pos] == character)).Select(pos => pattern[pos] == '*' ? pos : pos + 1))).ToArray();
                    Enqueue(next, witness + character);
                }
            }
            return regions.Values.ToArray();
        }

        private static int[] Closure(string pattern, IEnumerable<int> positions)
        {
            var result = new HashSet<int>();
            foreach (int position in positions)
            {
                int cursor = position;
                result.Add(cursor);
                while (cursor < pattern.Length && pattern[cursor] == '*') result.Add(++cursor);
            }
            return result.OrderBy(p => p).ToArray();
        }
    }
}
