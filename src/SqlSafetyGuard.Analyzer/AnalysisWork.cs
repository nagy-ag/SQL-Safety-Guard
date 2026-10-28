using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    internal sealed class AnalysisLimitException : Exception
    {
        public AnalysisLimitException(string message) : base(message) { }
    }

    internal sealed class AnalysisWork
    {
        [ThreadStatic] private static AnalysisWork _current;
        private readonly CancellationToken _cancellation;
        private readonly Stopwatch _elapsed = Stopwatch.StartNew();
        private int _remaining = 40000000;

        public AnalysisWork(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            _current = this;
        }

        public static void Check(int cost = 1)
        {
            var current = _current;
            if (current == null) return;
            current._cancellation.ThrowIfCancellationRequested();
            current._remaining -= cost;
            if (current._remaining < 0 || current._elapsed.Elapsed > TimeSpan.FromSeconds(10))
                throw new AnalysisLimitException("too complex to analyze: work/time budget exceeded");
        }

        public static IList<TSqlParserToken> Tokenize(TSqlParser parser, string text, out IList<ParseError> errors)
        {
            Check();
            using (var reader = new CheckedReader(text))
            {
                var tokens = parser.GetTokenStream(reader, out errors);
                // Flat lists (IN lists, VALUES rows, columns) and long operator chains parse and walk without deep recursion,
                // so statement length is not budgeted; nesting that recurses is, here and by the walker's depth limit.
                int parentheses = 0, blocks = 0, conditions = 0, elseConditions = 0, controlFlowKeywords = 0;
                TSqlTokenType previous = TSqlTokenType.EndOfFile;
                foreach (var token in tokens)
                {
                    Check();
                    if (token.TokenType == TSqlTokenType.WhiteSpace || token.TokenType == TSqlTokenType.SingleLineComment ||
                        token.TokenType == TSqlTokenType.MultilineComment) continue;
                    if (parentheses == 0 && StartsAction(token.TokenType))
                        conditions = 0;
                    if (previous == TSqlTokenType.Begin && (token.TokenType == TSqlTokenType.Transaction || token.TokenType == TSqlTokenType.Tran ||
                        token.TokenType == TSqlTokenType.Distributed)) blocks = Math.Max(0, blocks - 1);
                    if (token.TokenType == TSqlTokenType.LeftParenthesis) parentheses++;
                    if (token.TokenType == TSqlTokenType.RightParenthesis) parentheses = Math.Max(0, parentheses - 1);
                    if (token.TokenType == TSqlTokenType.Begin || token.TokenType == TSqlTokenType.Case) blocks++;
                    if (token.TokenType == TSqlTokenType.End) blocks = Math.Max(0, blocks - 1);
                    if (token.TokenType == TSqlTokenType.If)
                    {
                        conditions++;
                        elseConditions = previous == TSqlTokenType.Else ? elseConditions + 1 : 0;
                    }
                    if (token.TokenType == TSqlTokenType.While) conditions++;
                    if (token.TokenType == TSqlTokenType.If || token.TokenType == TSqlTokenType.While || token.TokenType == TSqlTokenType.Else)
                        controlFlowKeywords++;
                    if (token.TokenType == TSqlTokenType.Go) { conditions = 0; elseConditions = 0; blocks = 0; controlFlowKeywords = 0; }
                    if (controlFlowKeywords > 5000)
                        throw new AnalysisLimitException("too complex to analyze: control-flow keyword budget exceeded");
                    if (parentheses > 256 || blocks > 256 || conditions > 512 || elseConditions > 512)
                        throw new AnalysisLimitException("too complex to analyze: nesting/control-flow budget exceeded");
                    previous = token.TokenType;
                }
                return new CheckedTokens(tokens);
            }
        }

        private static bool StartsAction(TSqlTokenType token)
        {
            switch (token)
            {
                case TSqlTokenType.Select:
                case TSqlTokenType.Insert:
                case TSqlTokenType.Update:
                case TSqlTokenType.Delete:
                case TSqlTokenType.Merge:
                case TSqlTokenType.Create:
                case TSqlTokenType.Alter:
                case TSqlTokenType.Drop:
                case TSqlTokenType.Declare:
                case TSqlTokenType.Set:
                case TSqlTokenType.Print:
                case TSqlTokenType.Execute:
                case TSqlTokenType.Return:
                case TSqlTokenType.Truncate:
                    return true;
                default:
                    return false;
            }
        }

        public static TSqlFragment Parse(TSqlParser parser, string text, out IList<ParseError> errors)
        {
            var tokens = Tokenize(parser, text, out var lexicalErrors);
            var fragment = parser.Parse(tokens, out errors);
            if (lexicalErrors != null && lexicalErrors.Count > 0)
            {
                var combined = new List<ParseError>(lexicalErrors);
                if (errors != null) combined.AddRange(errors);
                errors = combined;
            }
            errors = errors ?? new List<ParseError>();
            Check();
            return fragment;
        }

        private sealed class CheckedReader : StringReader
        {
            public CheckedReader(string text) : base(text) { }
            public override int Read() { Check(); return base.Read(); }
            public override int Read(char[] buffer, int index, int count) { Check(); return base.Read(buffer, index, count); }
            public override string ReadToEnd() { Check(); return base.ReadToEnd(); }
        }

        private sealed class CheckedTokens : IList<TSqlParserToken>
        {
            private readonly IList<TSqlParserToken> _tokens;
            public CheckedTokens(IList<TSqlParserToken> tokens) { _tokens = tokens; }
            public TSqlParserToken this[int index] { get { Check(); return _tokens[index]; } set => throw new NotSupportedException(); }
            public int Count { get { Check(); return _tokens.Count; } }
            public bool IsReadOnly => true;
            public int IndexOf(TSqlParserToken item) => _tokens.IndexOf(item);
            public bool Contains(TSqlParserToken item) => _tokens.Contains(item);
            public void CopyTo(TSqlParserToken[] array, int arrayIndex) => _tokens.CopyTo(array, arrayIndex);
            public IEnumerator<TSqlParserToken> GetEnumerator() { foreach (var token in _tokens) { Check(); yield return token; } }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public void Add(TSqlParserToken item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public void Insert(int index, TSqlParserToken item) => throw new NotSupportedException();
            public bool Remove(TSqlParserToken item) => throw new NotSupportedException();
            public void RemoveAt(int index) => throw new NotSupportedException();
        }
    }
}
