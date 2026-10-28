using System.Text;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record Batch(string Sql, int Repeat, string? Error);

/// <summary>The batches a script sends to the server, the way SSMS splits them (GO lines) and expands SQLCMD mode.</summary>
internal sealed class Script
{
    public List<Batch> Batches { get; } = new();

    /// <summary>Set when the script uses a client-side command the harness refuses to run (e.g. !! or :r).</summary>
    public string? NotRunnable { get; set; }
}

/// <summary>
/// A small, faithful subset of SQLCMD mode: :setvar (applied in order), $(var) substitution everywhere on a line
/// (including inside strings, as sqlcmd does), :on error, :exit / :exit() / :exit(query), :quit and GO [n].
/// Client-side commands that touch the machine (!!, :r, :out, :error, :connect, :perftrace, ...) are refused.
/// </summary>
internal static class SqlCmdLite
{
    private static readonly Regex Go = new(@"^\s*GO(?:\s+(\d+))?\s*(?:--.*)?$", RegexOptions.IgnoreCase);
    private static readonly Regex Command = new(@"^\s*(:[A-Za-z]+|!!)(.*)$");
    private static readonly Regex Variable = new(@"\$\(([A-Za-z_][A-Za-z0-9_]*)\)");
    private static readonly Regex SetVar = new(@"^\s*([A-Za-z_][A-Za-z0-9_]*)(?:\s+(?:""((?:[^""]|"""")*)""|(\S+)))?\s*$");

    public static bool LooksLikeSqlCmd(string sql) =>
        sql.Split('\n').Any(l => Command.IsMatch(l.TrimEnd('\r'))) || Variable.IsMatch(sql);

    public static Script Split(string sql, bool sqlCmdMode)
    {
        var script = new Script();
        var vars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var current = new StringBuilder();
        string? batchError = null;
        int commentDepth = 0;
        char quoted = '\0';

        void Flush(int repeat)
        {
            var text = current.ToString();
            if (text.Trim().Length > 0 || batchError != null)
                script.Batches.Add(new Batch(text, repeat, batchError));
            current.Clear();
            batchError = null;
        }

        foreach (var rawLine in sql.Replace("\r\n", "\n").Split('\n'))
        {
            var line = rawLine;
            if (sqlCmdMode)
            {
                // Variables are substituted before the line is interpreted (sqlcmd semantics).
                line = Variable.Replace(line, m =>
                {
                    if (vars.TryGetValue(m.Groups[1].Value, out var value))
                        return value;
                    batchError ??= $"'{m.Groups[1].Value}' scripting variable not defined.";
                    return m.Value;
                });

                var cmd = commentDepth == 0 && quoted == '\0' ? Command.Match(line) : Match.Empty;
                if (cmd.Success)
                {
                    var name = cmd.Groups[1].Value.ToLowerInvariant();
                    var args = cmd.Groups[2].Value.Trim();
                    switch (name)
                    {
                        case ":setvar":
                            var sv = SetVar.Match(args);
                            if (sv.Success)
                            {
                                if (sv.Groups[2].Success || sv.Groups[3].Success)
                                    vars[sv.Groups[1].Value] = sv.Groups[2].Success ? sv.Groups[2].Value.Replace("\"\"", "\"") : sv.Groups[3].Value;
                                else
                                    vars.Remove(sv.Groups[1].Value);
                            }
                            continue;
                        case ":on":
                            continue; // ":on error exit|ignore": batches are run one by one either way; errors are recorded.
                        case ":exit":
                        case ":quit":
                            if (name == ":exit" && args.StartsWith("("))
                            {
                                // :exit(query) runs the buffered batch plus the query, then stops. :exit() runs the batch.
                                var inner = args.Trim().TrimStart('(');
                                if (inner.EndsWith(")"))
                                    inner = inner[..^1];
                                current.Append(inner).Append('\n');
                                Flush(1);
                            }

                            return script; // Plain :exit / :quit stop without running the buffered batch.
                        default:
                            script.NotRunnable = $"client-side SQLCMD command {name}";
                            return script;
                    }
                }
            }

            if (commentDepth == 0 && quoted == '\0' && Go.Match(line) is { Success: true } go)
            {
                Flush(go.Groups[1].Success ? int.Parse(go.Groups[1].Value) : 1);
                continue;
            }

            UpdateLexicalState(line, ref commentDepth, ref quoted);
            current.Append(line).Append('\n');
        }

        Flush(1);
        return script;
    }

    /// <summary>Tracks /* */ across lines (SSMS doesn't split on a GO inside a block comment). Strings and -- are respected.</summary>
    private static void UpdateLexicalState(string line, ref int depth, ref char quoted)
    {
        for (int index = 0; index < line.Length; index++)
        {
            char current = line[index];
            char next = index + 1 < line.Length ? line[index + 1] : '\0';
            if (depth > 0)
            {
                if (current == '*' && next == '/') { depth--; index++; }
                else if (current == '/' && next == '*') { depth++; index++; }
            }
            else if (quoted != '\0')
            {
                if (current == quoted && next == quoted) index++;
                else if (current == quoted) quoted = '\0';
            }
            else if (current is '\'' or '"') quoted = current;
            else if (current == '[') quoted = ']';
            else if (current == '-' && next == '-') break;
            else if (current == '/' && next == '*') { depth++; index++; }
        }
    }

    /// <summary>The script's T-SQL with SQLCMD lines expanded, for schema discovery only.</summary>
    public static string StripForDiscovery(string sql)
    {
        if (!LooksLikeSqlCmd(sql))
            return sql;
        var script = Split(sql, sqlCmdMode: true);
        return string.Join("\nGO\n", script.Batches.Select(b => b.Sql));
    }
}
