using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// SQLCMD mode support. Produces a same-length copy of the script for the parser: SQLCMD command lines
    /// (:setvar, :r, :connect, :on error, !!, ...) are blanked and $(variables) are replaced by placeholders, so
    /// every line and column still matches the editor. Nothing is executed and no file is read.
    /// </summary>
    internal static class SqlCmdPreprocessor
    {
        private static readonly Regex CommandLine = new Regex(
            @"^[ \t]*(?:(?<cmd>:(?:setvar|r|on|connect|out|error|exit|quit|reset|ed|list|listvar|help|perftrace|serverlist|xml))(?=\s|$)(?<args>.*)|(?<cmd>!!)(?<args>.*))$",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);

        private static readonly Regex Variable = new Regex(@"\$\((?<name>[A-Za-z_][A-Za-z0-9_]*)\)", RegexOptions.CultureInvariant);

        private static readonly Regex SetVar = new Regex(@"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s+(?:""(?<value>[^""]*)""|(?<value>\S+)))?",
            RegexOptions.CultureInvariant);

        public sealed class Directive
        {
            public Directive(string command, string arguments, int offset, int length)
            {
                Command = command;
                Arguments = arguments;
                Offset = offset;
                Length = length;
            }

            /// <summary>":setvar", ":r", ":connect", ":on", "!!", ...</summary>
            public string Command { get; }

            public string Arguments { get; }

            public int Offset { get; }

            public int Length { get; }
        }

        public static bool LooksLikeSqlCmd(string text) =>
            !string.IsNullOrEmpty(text) && (CommandLine.IsMatch(text) || Variable.IsMatch(text));

        /// <summary>Offsets of SQLCMD command lines and $(variable) references, for the "SQLCMD mode is off" hint.</summary>
        public static void FindSqlCmdSyntax(string text, out List<Directive> commands, out List<Directive> variableReferences)
        {
            commands = new List<Directive>();
            variableReferences = new List<Directive>();
            if (string.IsNullOrEmpty(text))
                return;

            foreach (Match match in CommandLine.Matches(text))
                commands.Add(new Directive(match.Groups["cmd"].Value.ToLowerInvariant(), match.Groups["args"].Value.Trim(),
                    match.Groups["cmd"].Index, match.Length - (match.Groups["cmd"].Index - match.Index)));
            foreach (Match match in Variable.Matches(text))
                variableReferences.Add(new Directive("$()", match.Groups["name"].Value, match.Index, match.Length));
        }

        public static string Process(string text, out List<Directive> directives, out Dictionary<string, string> variables)
        {
            directives = new List<Directive>();
            variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var buffer = new StringBuilder(text);

            foreach (Match match in CommandLine.Matches(text))
            {
                var command = match.Groups["cmd"].Value.ToLowerInvariant();
                var arguments = match.Groups["args"].Value.Trim().TrimEnd('\r');
                int start = match.Groups["cmd"].Index;
                int end = match.Index + match.Length;
                while (end > start && (text[end - 1] == '\r' || text[end - 1] == '\n'))
                    end--;

                directives.Add(new Directive(command, arguments, start, end - start));
                if (command == ":setvar")
                {
                    var setVar = SetVar.Match(arguments);
                    if (setVar.Success && setVar.Groups["value"].Success)
                        variables[setVar.Groups["name"].Value] = setVar.Groups["value"].Value;
                }

                for (int i = match.Index; i < end; i++)
                {
                    if (buffer[i] != '\r' && buffer[i] != '\n')
                        buffer[i] = ' ';
                }
            }

            // $(Name) -> placeholder identifier of the same length. It starts with '#' when the variable holds a
            // temp table name, so temp detection still works; anything else is treated as a permanent name.
            foreach (Match match in Variable.Matches(buffer.ToString()))
            {
                variables.TryGetValue(match.Groups["name"].Value, out var value);
                char first = value != null && value.StartsWith("#", StringComparison.Ordinal) ? '#' : '_';
                buffer[match.Index] = first;
                for (int i = match.Index + 1; i < match.Index + match.Length; i++)
                    buffer[i] = 'v';
            }

            return buffer.ToString();
        }
    }
}
