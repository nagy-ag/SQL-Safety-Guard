using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Recovers the analyzable statements of a GO batch that has syntax errors. The statement containing an error
    /// is blanked (newlines kept, so positions stay correct) and the batch is parsed again, until it parses or a
    /// limit is reached. A CREATE PROCEDURE/FUNCTION/TRIGGER/VIEW with an error swallows the rest of the batch,
    /// because that is where its body ends.
    /// </summary>
    internal sealed class BatchRecovery
    {
        private const int MaxIterations = 25;
        private const string Filler = " PRINT 0 ";
        private static readonly TimeSpan TimeBudget = TimeSpan.FromSeconds(3);

        private BatchRecovery()
        {
        }

        /// <summary>The parse tree of the batch with the broken parts blanked, or null when recovery failed.</summary>
        public TSqlFragment Fragment { get; private set; }

        /// <summary>The syntax errors to report (offsets relative to the batch text).</summary>
        public List<ParseError> Errors { get; } = new List<ParseError>();

        /// <summary>Blanked regions, in text order (offsets relative to the batch text).</summary>
        public List<Region> Regions { get; } = new List<Region>();

        public sealed class Region
        {
            public Region(int start, int end, ModuleDefinition module)
            {
                Start = start;
                End = end;
                Module = module;
            }

            public int Start { get; set; }

            public int End { get; set; }

            /// <summary>Set when the region is a CREATE/ALTER of a procedure, function, trigger or view.</summary>
            public ModuleDefinition Module { get; }

            public bool Contains(int offset) => offset >= Start && offset < End;
        }

        public sealed class ModuleDefinition
        {
            public ModuleDefinition(string action, string name, bool misplaced)
            {
                Action = action;
                Name = name;
                Misplaced = misplaced;
            }

            /// <summary>For example "CREATE OR ALTER PROCEDURE".</summary>
            public string Action { get; }

            public string Name { get; }

            /// <summary>True when the definition is not the first statement of its batch.</summary>
            public bool Misplaced { get; }
        }

        public static BatchRecovery Run(TSqlParser parser, string text)
        {
            var recovery = new BatchRecovery();
            var buffer = text.ToCharArray();
            var fragment = Parse(parser, new string(buffer), out var errors);
            var stopwatch = Stopwatch.StartNew();

            for (int iteration = 0; errors.Count > 0; iteration++)
            {
                if (iteration >= MaxIterations || stopwatch.Elapsed > TimeBudget)
                    return recovery.Failed(errors);

                var current = new string(buffer);
                var first = errors.OrderBy(e => e.Offset).First();
                int errorOffset = Math.Min(Math.Max(first.Offset, 0), Math.Max(current.Length - 1, 0));

                var tokens = Tokenize(parser, current);
                var starts = StatementStarts(tokens);
                var module = FindModuleRegion(tokens, starts, current, errorOffset);

                Region chosen;
                char[] chosenBuffer;
                TSqlFragment chosenFragment;
                IList<ParseError> chosenErrors;

                if (module != null)
                {
                    chosen = module;
                    chosenBuffer = Blank(buffer, chosen.Start, chosen.End);
                    chosenFragment = Parse(parser, new string(chosenBuffer), out chosenErrors);
                }
                else
                {
                    // Candidate A: the statement containing the error.
                    var a = StatementAround(starts, tokens, current.Length, errorOffset);
                    var bufferA = Blank(buffer, a.Start, a.End);
                    var fragmentA = Parse(parser, new string(bufferA), out var errorsA);
                    chosen = a;
                    chosenBuffer = bufferA;
                    chosenFragment = fragmentA;
                    chosenErrors = errorsA;

                    // Candidate B: an error reported at the first word of a statement usually means the previous
                    // statement is incomplete ("Incorrect syntax near 'SELECT'"). Try blanking that one instead.
                    var b = PreviousStatement(starts, tokens, errorOffset);
                    if (b != null && errorsA.Count > 0)
                    {
                        var bufferB = Blank(buffer, b.Start, b.End);
                        var fragmentB = Parse(parser, new string(bufferB), out var errorsB);
                        if (Score(errorsB) > Score(errorsA))
                        {
                            chosen = b;
                            chosenBuffer = bufferB;
                            chosenFragment = fragmentB;
                            chosenErrors = errorsB;
                        }
                    }
                }

                if (!HasCode(buffer, chosen.Start, chosen.End))
                    return recovery.Failed(errors);

                // Report the errors of this iteration that lie in the blanked part (or the triggering error).
                var inRegion = errors.Where(e => chosen.Contains(e.Offset) || (e.Offset >= current.Length && chosen.End >= current.Length)).ToList();
                if (inRegion.Count == 0)
                    inRegion.Add(first);
                foreach (var error in inRegion)
                {
                    if (!recovery.Errors.Any(e => e.Offset == error.Offset && e.Message == error.Message))
                        recovery.Errors.Add(error);
                }

                recovery.AddRegion(chosen);
                buffer = chosenBuffer;
                fragment = chosenFragment;
                errors = chosenErrors;
            }

            recovery.Fragment = fragment;
            return recovery;
        }

        private BatchRecovery Failed(IList<ParseError> remaining)
        {
            foreach (var error in remaining)
            {
                if (!Errors.Any(e => e.Offset == error.Offset && e.Message == error.Message))
                    Errors.Add(error);
            }

            Fragment = null;
            return this;
        }

        private void AddRegion(Region region)
        {
            Regions.Add(region);
            Regions.Sort((x, y) => x.Start.CompareTo(y.Start));
        }

        private static int Score(IList<ParseError> errors) =>
            errors.Count == 0 ? int.MaxValue : errors.Min(e => e.Offset);

        private static TSqlFragment Parse(TSqlParser parser, string text, out IList<ParseError> errors)
        {
            using (var reader = new StringReader(text))
            {
                var fragment = parser.Parse(reader, out errors);
                errors = errors ?? new List<ParseError>();
                return fragment;
            }
        }

        /// <summary>
        /// Tokenizes the whole text. The ScriptDom tokenizer stops at a character it doesn't accept (e.g. the "\" in
        /// ":r .\file.sql"); this resumes right after it, so GO separators and keywords further on are still seen.
        /// </summary>
        internal static IList<TSqlParserToken> Tokenize(TSqlParser parser, string text)
        {
            var result = new List<TSqlParserToken>();
            int position = 0;
            for (int guard = 0; position < text.Length && guard < 10000; guard++)
            {
                IList<TSqlParserToken> tokens;
                IList<ParseError> errors;
                using (var reader = new StringReader(position == 0 ? text : text.Substring(position)))
                    tokens = parser.GetTokenStream(reader, out errors) ?? new List<TSqlParserToken>();

                TextPosition.FromOffset(text, position, out var startLine, out var startColumn);
                foreach (var token in tokens)
                {
                    if (position > 0)
                    {
                        if (token.Line == 1)
                            token.Column += startColumn - 1;
                        token.Line += startLine - 1;
                        token.Offset += position;
                    }

                    if (token.TokenType != TSqlTokenType.EndOfFile || errors == null || errors.Count == 0)
                        result.Add(token);
                }

                if (errors == null || errors.Count == 0)
                    break;
                position += Math.Max(errors.Min(e => e.Offset), 0) + 1;
            }

            return result;
        }

        /// <summary>Blanks [start, end) keeping line breaks, and drops in a "PRINT 0" where it fits so that a
        /// blanked IF/ELSE/WHILE/BEGIN body still has a statement.</summary>
        private static char[] Blank(char[] source, int start, int end)
        {
            var result = (char[])source.Clone();
            end = Math.Min(end, result.Length);
            for (int i = Math.Max(start, 0); i < end; i++)
            {
                if (result[i] != '\r' && result[i] != '\n')
                    result[i] = ' ';
            }

            int run = 0;
            for (int i = Math.Max(start, 0); i < end; i++)
            {
                run = result[i] == ' ' ? run + 1 : 0;
                if (run == Filler.Length)
                {
                    Filler.CopyTo(0, result, i - Filler.Length + 1, Filler.Length);
                    break;
                }
            }

            return result;
        }

        private static bool HasCode(char[] buffer, int start, int end)
        {
            for (int i = Math.Max(start, 0); i < Math.Min(end, buffer.Length); i++)
            {
                if (!char.IsWhiteSpace(buffer[i]))
                    return true;
            }

            return false;
        }

        private static bool IsTrivia(TSqlParserToken token) =>
            token.TokenType == TSqlTokenType.WhiteSpace ||
            token.TokenType == TSqlTokenType.SingleLineComment ||
            token.TokenType == TSqlTokenType.MultilineComment ||
            token.TokenType == TSqlTokenType.EndOfFile;

        private static readonly HashSet<TSqlTokenType> AlwaysStatementStart = new HashSet<TSqlTokenType>
        {
            TSqlTokenType.Declare, TSqlTokenType.Print, TSqlTokenType.If, TSqlTokenType.While, TSqlTokenType.Begin,
            TSqlTokenType.Else, TSqlTokenType.Use, TSqlTokenType.Return, TSqlTokenType.Grant, TSqlTokenType.Revoke,
            TSqlTokenType.Deny, TSqlTokenType.Truncate, TSqlTokenType.Create, TSqlTokenType.WaitFor, TSqlTokenType.Commit,
            TSqlTokenType.Rollback, TSqlTokenType.Save, TSqlTokenType.Open, TSqlTokenType.Close, TSqlTokenType.Fetch,
            TSqlTokenType.Deallocate, TSqlTokenType.Break, TSqlTokenType.Continue, TSqlTokenType.GoTo, TSqlTokenType.Kill,
            TSqlTokenType.Backup, TSqlTokenType.Restore, TSqlTokenType.Dbcc, TSqlTokenType.Checkpoint,
            TSqlTokenType.Reconfigure, TSqlTokenType.Shutdown, TSqlTokenType.Bulk,
        };

        /// <summary>
        /// Token indexes where a statement probably starts (outer level only). Heuristic: a wrong split only makes
        /// a blanked region smaller, which costs another iteration but never hides valid code.
        /// </summary>
        internal static List<int> StatementStarts(IList<TSqlParserToken> tokens)
        {
            var starts = new List<int>();
            int depth = 0, caseDepth = 0;
            bool expectStart = true, pendingInsert = false, pendingUpdate = false, inMerge = false, pendingCte = false, inDdl = false;
            TSqlParserToken previous = null;

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (IsTrivia(token))
                    continue;

                var type = token.TokenType;
                bool start = false;

                if (depth == 0)
                {
                    if (type == TSqlTokenType.Semicolon)
                    {
                        expectStart = true;
                        pendingInsert = pendingUpdate = inMerge = pendingCte = inDdl = false;
                        previous = token;
                        continue;
                    }

                    if (type == TSqlTokenType.Case)
                        caseDepth++;

                    if (expectStart)
                    {
                        start = true;
                    }
                    else if (type == TSqlTokenType.End)
                    {
                        if (caseDepth > 0)
                            caseDepth--;
                        else
                            start = true;
                    }
                    else if (AlwaysStatementStart.Contains(type))
                    {
                        start = true;
                    }
                    else
                    {
                        switch (type)
                        {
                            case TSqlTokenType.Select:
                                start = !pendingInsert && !pendingCte && !IsSetOperator(previous) && !(previous?.TokenType == TSqlTokenType.For);
                                pendingInsert = false;
                                break;
                            case TSqlTokenType.Insert:
                            case TSqlTokenType.Update:
                            case TSqlTokenType.Delete:
                                start = !inMerge && !pendingCte;
                                break;
                            case TSqlTokenType.Merge:
                                start = !pendingCte;
                                break;
                            case TSqlTokenType.Execute:
                                start = !pendingInsert;
                                pendingInsert = false;
                                break;
                            case TSqlTokenType.Values:
                                pendingInsert = false;
                                break;
                            case TSqlTokenType.Set:
                                start = !pendingUpdate && !inDdl && !inMerge;
                                break;
                            case TSqlTokenType.Drop:
                            case TSqlTokenType.Alter:
                                start = !inDdl;
                                break;
                            case TSqlTokenType.With:
                                start = IsCteStart(tokens, i);
                                break;
                        }
                    }

                    if (start)
                    {
                        // A CTE's main statement and the parts of INSERT/UPDATE/MERGE/DDL are not new statements.
                        bool consumedByCte = pendingCte && (type == TSqlTokenType.Select || type == TSqlTokenType.Insert ||
                            type == TSqlTokenType.Update || type == TSqlTokenType.Delete || type == TSqlTokenType.Merge);
                        if (!consumedByCte)
                            starts.Add(i);

                        pendingCte = type == TSqlTokenType.With;
                        pendingInsert = type == TSqlTokenType.Insert;
                        pendingUpdate = type == TSqlTokenType.Update;
                        inMerge = type == TSqlTokenType.Merge;
                        // ALTER TABLE ... ADD / DROP / ALTER COLUMN / SET: those words don't start new statements.
                        inDdl = type == TSqlTokenType.Alter;
                        if (consumedByCte)
                        {
                            pendingCte = false;
                            pendingInsert = type == TSqlTokenType.Insert;
                            pendingUpdate = type == TSqlTokenType.Update;
                            inMerge = type == TSqlTokenType.Merge;
                        }
                    }
                    else if (pendingCte && (type == TSqlTokenType.Select || type == TSqlTokenType.Insert || type == TSqlTokenType.Update ||
                                            type == TSqlTokenType.Delete || type == TSqlTokenType.Merge))
                    {
                        pendingCte = false;
                        pendingInsert = type == TSqlTokenType.Insert;
                        pendingUpdate = type == TSqlTokenType.Update;
                        inMerge = type == TSqlTokenType.Merge;
                    }

                    expectStart = false;
                }

                if (type == TSqlTokenType.LeftParenthesis)
                    depth++;
                else if (type == TSqlTokenType.RightParenthesis && depth > 0)
                    depth--;

                previous = token;
            }

            return starts;
        }

        private static bool IsSetOperator(TSqlParserToken previous) =>
            previous != null && (previous.TokenType == TSqlTokenType.Union || previous.TokenType == TSqlTokenType.All ||
                                 previous.TokenType == TSqlTokenType.Except || previous.TokenType == TSqlTokenType.Intersect);

        private static bool IsCteStart(IList<TSqlParserToken> tokens, int index)
        {
            var next = NextSignificant(tokens, index);
            return next >= 0 && (tokens[next].TokenType == TSqlTokenType.Identifier || tokens[next].TokenType == TSqlTokenType.QuotedIdentifier);
        }

        private static int NextSignificant(IList<TSqlParserToken> tokens, int index)
        {
            for (int i = index + 1; i < tokens.Count; i++)
            {
                if (!IsTrivia(tokens[i]))
                    return i;
            }

            return -1;
        }

        private static Region StatementAround(List<int> starts, IList<TSqlParserToken> tokens, int textLength, int offset)
        {
            int start = 0, end = textLength;
            foreach (var index in starts)
            {
                var tokenOffset = tokens[index].Offset;
                if (tokenOffset <= offset)
                    start = tokenOffset;
                else
                {
                    end = tokenOffset;
                    break;
                }
            }

            return new Region(start, end, null);
        }

        private static Region PreviousStatement(List<int> starts, IList<TSqlParserToken> tokens, int offset)
        {
            int at = starts.FindIndex(i => tokens[i].Offset == offset);
            if (at <= 0)
                return null;
            return new Region(tokens[starts[at - 1]].Offset, offset, null);
        }

        /// <summary>
        /// When the error is in (or right at) a CREATE/ALTER PROCEDURE/FUNCTION/TRIGGER/VIEW, returns the region from
        /// it to the end of the batch: everything after AS is its body, so nothing after it runs on its own.
        /// </summary>
        private static Region FindModuleRegion(IList<TSqlParserToken> tokens, List<int> starts, string text, int errorOffset)
        {
            // The statement containing the error; a misplaced CREATE is reported at the CREATE itself.
            int startIndex = -1;
            foreach (var index in starts)
            {
                if (tokens[index].Offset <= errorOffset)
                    startIndex = index;
                else
                    break;
            }

            if (startIndex < 0)
                return null;

            var first = tokens[startIndex].TokenType;
            if (first != TSqlTokenType.Create && first != TSqlTokenType.Alter)
                return null;

            var words = new List<string> { tokens[startIndex].Text.ToUpperInvariant() };
            int i = NextSignificant(tokens, startIndex);
            if (first == TSqlTokenType.Create && i >= 0 && tokens[i].TokenType == TSqlTokenType.Or)
            {
                words.Add("OR");
                i = NextSignificant(tokens, i);
                if (i < 0 || tokens[i].TokenType != TSqlTokenType.Alter)
                    return null;
                words.Add("ALTER");
                i = NextSignificant(tokens, i);
            }

            if (i < 0)
                return null;
            string kind;
            switch (tokens[i].TokenType)
            {
                case TSqlTokenType.Proc:
                case TSqlTokenType.Procedure:
                    kind = "PROCEDURE";
                    break;
                case TSqlTokenType.Function:
                    kind = "FUNCTION";
                    break;
                case TSqlTokenType.Trigger:
                    kind = "TRIGGER";
                    break;
                case TSqlTokenType.View:
                    kind = "VIEW";
                    break;
                default:
                    return null;
            }

            words.Add(kind);

            // Name: identifiers and dots right after the kind keyword.
            int nameStart = -1, nameEnd = -1;
            for (int j = NextSignificant(tokens, i); j >= 0 && j < tokens.Count; j++)
            {
                var type = tokens[j].TokenType;
                if (type == TSqlTokenType.Identifier || type == TSqlTokenType.QuotedIdentifier || type == TSqlTokenType.Dot)
                {
                    if (nameStart < 0)
                        nameStart = tokens[j].Offset;
                    nameEnd = tokens[j].Offset + tokens[j].Text.Length;
                }
                else
                {
                    break;
                }
            }

            string name = nameStart >= 0 ? text.Substring(nameStart, nameEnd - nameStart) : string.Empty;
            bool misplaced = tokens.Take(startIndex).Any(t => !IsTrivia(t));
            int regionStart = tokens[startIndex].Offset;
            return new Region(regionStart, text.Length, new ModuleDefinition(string.Join(" ", words), name, misplaced));
        }
    }
}
