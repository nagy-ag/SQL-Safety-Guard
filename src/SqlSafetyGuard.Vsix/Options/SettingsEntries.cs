#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Options
{
    // Shared as source with the unit tests. The stored text stays the only settings format: these models are
    // editor views of it. Matching and validation always go through the analyzer's AllowList/EnvironmentRules.

    /// <summary>One safe-table or trusted-procedure pattern, edited as named parts.</summary>
    internal sealed class PatternEntry : INotifyPropertyChanged
    {
        private string _server = "", _database = "", _schema = "", _name = "", _note = "";

        internal PatternEntry(bool procedures) { Procedures = procedures; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Procedures { get; }

        /// <summary>The original text of an entry nobody has edited; kept byte-for-byte when saving.</summary>
        public string Raw { get; private set; }

        /// <summary>False when the original text cannot be split into 1–4 name parts.</summary>
        public bool Readable { get; private set; } = true;

        public string Server { get => _server; set => SetPart(ref _server, value); }
        public string Database { get => _database; set => SetPart(ref _database, value); }
        public string Schema { get => _schema; set => SetPart(ref _schema, value); }
        public string Name { get => _name; set => SetPart(ref _name, value); }

        public string Note
        {
            get => _note;
            set { _note = value ?? ""; Changed(); }
        }

        public string Pattern => Raw ?? PatternText.ComposePattern(Server, Database, Schema, Name);
        public string Display => Pattern.Length == 0 ? (Procedures ? "(new procedure)" : "(new pattern)") : Pattern;
        public string Problem => PatternText.Validate(this);
        public bool HasNote => Note.Length > 0;
        public bool HasProblem => Problem != null;

        internal static PatternEntry FromText(string text, string note, bool procedures)
        {
            var entry = new PatternEntry(procedures) { _note = note ?? "", Raw = text };
            if (PatternText.TryRead(text, out var server, out var database, out var schema, out var name))
            {
                entry._server = server; entry._database = database; entry._schema = schema; entry._name = name;
            }
            else entry.Readable = false;
            return entry;
        }

        private void SetPart(ref string field, string value)
        {
            value = (value ?? "").Trim();
            if (value == field) return;
            field = value;
            Raw = null; // An edited entry is composed from its parts.
            Readable = true;
            Changed();
        }

        private void Changed()
        {
            foreach (var name in new[] { nameof(Server), nameof(Database), nameof(Schema), nameof(Name), nameof(Note),
                nameof(Pattern), nameof(Display), nameof(Problem), nameof(HasNote), nameof(HasProblem) })
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal static class PatternText
    {
        /// <summary>Splits stored text exactly as the analyzer does; comment lines become the next entry's note.</summary>
        internal static List<PatternEntry> Parse(string text, bool procedures, out List<string> trailingComments)
        {
            var entries = new List<PatternEntry>();
            var notes = new List<string>();
            foreach (var piece in AllowList.SplitLines(text).Select(value => value.Trim()).Where(value => value.Length > 0))
            {
                if (IsComment(piece)) { notes.Add(piece); continue; }
                entries.Add(PatternEntry.FromText(piece, string.Join(" ", notes.Select(StripComment).Where(note => note.Length > 0)), procedures));
                notes.Clear();
            }
            trailingComments = notes;
            return entries;
        }

        internal static string Compose(IEnumerable<PatternEntry> entries, IEnumerable<string> trailingComments = null)
        {
            var lines = new List<string>();
            foreach (var entry in entries)
            {
                if (entry.Note.Trim().Length > 0) lines.Add("-- " + entry.Note.Trim());
                lines.Add(entry.Pattern);
            }
            lines.AddRange(trailingComments ?? Enumerable.Empty<string>());
            return string.Join("\n", lines);
        }

        internal static bool TryRead(string pattern, out string server, out string database, out string schema, out string name)
        {
            server = database = schema = name = "";
            var parts = AllowList.SplitName((pattern ?? "").Trim());
            if (parts == null || parts.Length == 0 || parts.Length > 4) return false;
            var padded = new string[4 - parts.Length].Select(_ => "").Concat(parts).ToArray();
            server = padded[0]; database = padded[1]; schema = padded[2]; name = padded[3];
            return true;
        }

        /// <summary>Joins the filled parts, bracketing any part the analyzer would otherwise read differently.</summary>
        internal static string ComposePattern(string server, string database, string schema, string name)
        {
            var parts = new[] { server, database, schema, name }.Select(part => (part ?? "").Trim()).ToArray();
            int first = Array.FindIndex(parts, part => part.Length > 0);
            if (first < 0) return "";
            return string.Join(".", parts.Skip(first).Select((part, index) => Quote(part, index == 0)));
        }

        private static string Quote(string part, bool first)
        {
            bool needs = part.IndexOf('.') >= 0 || part.StartsWith("[", StringComparison.Ordinal) || part.StartsWith("\"", StringComparison.Ordinal) ||
                (first && IsComment(part));
            return needs ? "[" + part.Replace("]", "]]") + "]" : part;
        }

        internal static string Validate(PatternEntry entry)
        {
            string kind = entry.Procedures ? "procedure" : "table";
            if (entry.Note.IndexOfAny(new[] { ';', '\r', '\n' }) >= 0) return "A note can't contain ; or line breaks.";
            if (!entry.Readable) return "This text can't be read as a name. Fix it in Edit as text, or remove it.";
            var parts = new[] { entry.Server, entry.Database, entry.Schema, entry.Name };
            if (parts.Any(part => part.IndexOfAny(new[] { ';', '\r', '\n' }) >= 0)) return "Names can't contain ; or line breaks.";
            if (entry.Name.Length == 0) return "Enter a " + kind + " name.";
            if (entry.Server.Length > 0 && entry.Database.Length == 0) return "A server also needs a database and a schema.";
            if (entry.Database.Length > 0 && entry.Schema.Length == 0) return "A database also needs a schema, for example dbo.";
            if (entry.Procedures && entry.Schema.Length == 0) return "Enter a schema, for example dbo. Procedures must be schema-qualified.";
            if (entry.Procedures && new[] { entry.Server, entry.Database, entry.Schema }.Any(part => part.IndexOfAny(new[] { '*', '?' }) >= 0))
                return "Wildcards are allowed only in the procedure name.";
            var list = new AllowList(new[] { entry.Pattern }, entry.Procedures);
            if (list.Warnings.Count == 0) return null;
            return parts.Where(part => part.Length > 0).All(part => part.All(character => character == '*' || character == '?'))
                ? "Only wildcards: this pattern would be ignored. Name at least one part."
                : "This pattern would be ignored: " + list.Warnings[0];
        }

        /// <summary>A plain-language description of what the entry matches.</summary>
        internal static string Describe(PatternEntry entry)
        {
            if (entry.Problem != null) return "";
            string objects = entry.Procedures ? "Procedures" : "Tables";
            string text = objects + " named " + entry.Name + (entry.Schema.Length == 0 ? " in any schema" : " in schema " + entry.Schema);
            if (entry.Database.Length == 0) text += ", in the window's current database";
            else text += ", in database " + entry.Database + " (also unqualified names when it is the current database)";
            text += entry.Server.Length == 0 ? ". Not on linked servers." : ", on linked server " + entry.Server + ".";
            if (entry.Schema.Length > 0)
                text += " Write the schema in the query too, for example " + (entry.Procedures ? "EXEC " : "UPDATE ") + entry.Schema + "." + Example(entry.Name) + ".";
            return text;
        }

        private static string Example(string name) => name.Replace("*", "Orders").Replace("?", "X");

        /// <summary>Tests a typed name against one entry with the analyzer's matcher.</summary>
        internal static string Try(PatternEntry entry, string candidate)
        {
            string text = (candidate ?? "").Trim().TrimEnd(';').Trim();
            if (text.Length == 0) return "";
            if (entry.Problem != null) return "Fix the pattern first.";
            foreach (var prefix in new[] { "EXECUTE ", "EXEC " })
                if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) text = text.Substring(prefix.Length).Trim();
            int space = text.IndexOfAny(new[] { ' ', '\t' });
            if (entry.Procedures && space > 0 && text[0] != '[' && text[0] != '"') text = text.Substring(0, space);
            if (!TryRead(text, out var server, out var database, out var schema, out var name) || name.Length == 0)
                return "Type a name such as dbo.Orders.";
            var list = new AllowList(new[] { entry.Pattern }, entry.Procedures);
            string green = entry.Procedures ? "calls to it would be green." : "writes to it would be green.";
            if (list.Matches(Null(server), Null(database), Null(schema), name)) return "✓ Matches: " + green;
            if (database.Length == 0 && entry.Database.Length > 0 && entry.Database.IndexOfAny(new[] { '*', '?' }) < 0 &&
                list.Matches(Null(server), null, Null(schema), name, entry.Database))
                return "✓ Matches only when the window's current database is " + entry.Database + ".";
            if (schema.Length == 0 && entry.Schema.Length > 0)
                return "✗ Doesn't match: write the schema too, for example " + entry.Schema + "." + name + ".";
            if (server.Length > 0 && entry.Server.Length == 0) return "✗ Doesn't match: names on a linked server need a server part in the pattern.";
            if (database.Length > 0 && entry.Database.Length == 0) return "✗ Doesn't match: a pattern without a database matches only the current database.";
            return "✗ Doesn't match.";
        }

        internal static (int Count, int Problems) Summarize(string text, bool procedures)
        {
            var list = new AllowList(AllowList.SplitLines(text), procedures);
            return (list.Patterns.Count + list.Warnings.Count, list.Warnings.Count);
        }

        internal static bool IsComment(string text) => text.StartsWith("--", StringComparison.Ordinal) || text.StartsWith("#", StringComparison.Ordinal);
        private static string StripComment(string text) => text.TrimStart('-', '#').Trim();
        private static string Null(string value) => value.Length == 0 ? null : value;
    }

    /// <summary>One environment rule, edited as named fields.</summary>
    internal sealed class RuleEntry : INotifyPropertyChanged
    {
        private string _server = "", _database = "", _label = "", _mode = "YellowRed", _safeTables = "", _trusted = "", _note = "";
        private int _number;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>The original line of a rule nobody has edited; kept byte-for-byte when saving.</summary>
        public string Raw { get; private set; }
        public bool Readable { get; private set; } = true;
        public string Legacy { get; private set; } = "false";

        public string Server { get => _server; set => Set(ref _server, value); }
        public string Database { get => _database; set => Set(ref _database, value); }
        public string Label { get => _label; set => Set(ref _label, value); }
        public string Mode { get => _mode; set => Set(ref _mode, value); }
        /// <summary>One pattern per line in the editor; stored semicolon-separated inside the rule.</summary>
        public string SafeTables { get => _safeTables; set => Set(ref _safeTables, value); }
        public string TrustedProcedures { get => _trusted; set => Set(ref _trusted, value); }

        public string Note
        {
            get => _note;
            set { _note = value ?? ""; Changed(); }
        }

        public int Number
        {
            get => _number;
            set { _number = value; Changed(); }
        }

        public string Line => Raw ?? RuleText.ComposeLine(this);
        public string Title => Number + " · " + (Readable ? (Label.Length == 0 ? "(new rule)" : Label) : "Unreadable rule");
        public string Subtitle => !Readable ? Raw : (Server.Length == 0 ? "(no server)" : Server) + " / " + (Database.Length == 0 ? "any" : Database) +
            (Mode == "Red" ? " · red only" : Mode == "Off" ? " · off" : "");
        public bool IsProduction => Label.IndexOf("PROD", StringComparison.OrdinalIgnoreCase) >= 0;
        public string Problem => RuleText.Validate(this);
        public bool HasProblem => Problem != null;

        internal static RuleEntry FromLine(string line, string note)
        {
            var entry = new RuleEntry { _note = note ?? "", Raw = line };
            var fields = line.Split('|').Select(field => field.Trim()).ToArray();
            if (fields.Length < 5 || fields.Length > 7) { entry.Readable = false; return entry; }
            entry._server = fields[0];
            entry._database = fields[1];
            entry._label = fields[2];
            entry._mode = Enum.TryParse(fields[3], true, out ExecutionMode mode) && Enum.IsDefined(typeof(ExecutionMode), mode) ? mode.ToString() : fields[3];
            entry.Legacy = fields[4];
            entry._safeTables = fields.Length > 5 ? string.Join("\n", AllowList.SplitLines(fields[5]).Select(item => item.Trim()).Where(item => item.Length > 0)) : "";
            entry._trusted = fields.Length > 6 ? string.Join("\n", AllowList.SplitLines(fields[6]).Select(item => item.Trim()).Where(item => item.Length > 0)) : "";
            return entry;
        }

        internal static RuleEntry New() => new RuleEntry();

        private void Set(ref string field, string value)
        {
            value = (value ?? "").Replace("\r\n", "\n").Replace('\r', '\n');
            if (!Readable || value == field) return;
            field = value;
            Raw = null; // An edited rule is composed from its fields.
            Changed();
        }

        private void Changed()
        {
            foreach (var name in new[] { nameof(Server), nameof(Database), nameof(Label), nameof(Mode), nameof(SafeTables), nameof(TrustedProcedures),
                nameof(Note), nameof(Line), nameof(Title), nameof(Subtitle), nameof(IsProduction), nameof(Problem), nameof(HasProblem) })
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal static class RuleText
    {
        internal static List<RuleEntry> Parse(string text, out List<string> trailingComments)
        {
            var entries = new List<RuleEntry>();
            var notes = new List<string>();
            foreach (var line in (text ?? "").Replace("\r", "").Split('\n').Select(value => value.Trim()).Where(value => value.Length > 0))
            {
                if (line.StartsWith("--", StringComparison.Ordinal)) { notes.Add(line); continue; }
                entries.Add(RuleEntry.FromLine(line, string.Join(" ", notes.Select(note => note.Substring(2).Trim()).Where(note => note.Length > 0))));
                notes.Clear();
            }
            Renumber(entries);
            trailingComments = notes;
            return entries;
        }

        internal static void Renumber(IList<RuleEntry> entries)
        {
            for (int index = 0; index < entries.Count; index++) entries[index].Number = index + 1;
        }

        internal static string Compose(IEnumerable<RuleEntry> entries, IEnumerable<string> trailingComments = null)
        {
            var lines = new List<string>();
            foreach (var entry in entries)
            {
                if (entry.Note.Trim().Length > 0) lines.Add("-- " + entry.Note.Trim());
                lines.Add(entry.Line);
            }
            lines.AddRange(trailingComments ?? Enumerable.Empty<string>());
            return string.Join("\n", lines);
        }

        internal static string ComposeLine(RuleEntry entry)
        {
            var fields = new List<string> { entry.Server.Trim(), entry.Database.Trim(), entry.Label.Trim(), entry.Mode, entry.Legacy };
            string tables = List(entry.SafeTables), procedures = List(entry.TrustedProcedures);
            if (tables.Length > 0 || procedures.Length > 0) fields.Add(tables);
            if (procedures.Length > 0) fields.Add(procedures);
            return string.Join(" | ", fields);
        }

        private static string List(string lines) =>
            string.Join(";", AllowList.SplitLines(lines).Select(item => item.Trim()).Where(item => item.Length > 0));

        internal static string Validate(RuleEntry entry)
        {
            if (entry.Note.IndexOfAny(new[] { '\r', '\n' }) >= 0) return "A note can't contain line breaks.";
            if (!entry.Readable) return "This line isn't a rule (it needs 5 to 7 fields separated by |). Fix it in Edit as text, or remove it.";
            if (new[] { entry.Server, entry.Database, entry.Label, entry.SafeTables, entry.TrustedProcedures }.Any(value => value.IndexOf('|') >= 0))
                return "Fields can't contain |.";
            if (new[] { entry.Server, entry.Database, entry.Label }.Any(value => value.IndexOfAny(new[] { '\r', '\n' }) >= 0))
                return "Server, database and label must be one line.";
            if (entry.Server.Trim().Length == 0) return "Enter a server name or pattern.";
            if (entry.Label.Trim().Length == 0) return "Enter a label.";
            var tables = new AllowList(AllowList.SplitLines(entry.SafeTables));
            if (tables.Warnings.Count > 0) return "Extra safe tables: " + tables.Warnings[0];
            var procedures = new AllowList(AllowList.SplitLines(entry.TrustedProcedures), procedures: true);
            if (procedures.Warnings.Count > 0) return "Extra trusted procedures: " + procedures.Warnings[0];
            var warnings = new EnvironmentRules(entry.Line).Warnings;
            return warnings.Count == 0 ? null : warnings[0].Replace("Environment rule line 1 ignored: ", "");
        }

        internal static (int Count, int Problems) Summarize(string text) =>
            (Parse(text, out _).Count, new EnvironmentRules(text).Warnings.Count);
    }
}
