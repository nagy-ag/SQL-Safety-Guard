using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>A contributing range in the original analyzed text, independent of the EXEC anchor.</summary>
    public readonly struct SourceRange
    {
        public SourceRange(int startOffset, int length) { StartOffset = startOffset; Length = length; }
        public int StartOffset { get; }
        public int Length { get; }
    }

    // Each decoded UTF-16 character retains its original spelling (including doubled quotes).
    // This optional display metadata never establishes or invalidates string-value proof.
    internal sealed class DynamicSqlSourceMap
    {
        private readonly SourceRange[] _characters;
        private DynamicSqlSourceMap(SourceRange[] characters) { _characters = characters; }

        public static DynamicSqlSourceMap FromLiteral(StringLiteral literal)
        {
            if (literal.ScriptTokenStream == null || literal.FirstTokenIndex < 0) return null;
            var raw = new StringBuilder();
            for (int i = literal.FirstTokenIndex; i <= literal.LastTokenIndex; i++)
                raw.Append(literal.ScriptTokenStream[i].Text);
            string text = raw.ToString();
            int cursor = text.Length > 0 && (text[0] == 'N' || text[0] == 'n') ? 1 : 0;
            if (cursor >= text.Length || text[cursor++] != '\'') return null;
            var characters = new SourceRange[literal.Value.Length];
            for (int i = 0; i < characters.Length; i++)
            {
                AnalysisWork.Check();
                if (cursor >= text.Length - 1 || text[cursor] != literal.Value[i]) return null;
                int length = text[cursor] == '\'' ? 2 : 1;
                if (length == 2 && (cursor + 1 >= text.Length - 1 || text[cursor + 1] != '\'')) return null;
                characters[i] = new SourceRange(literal.StartOffset + cursor, length);
                cursor += length;
            }
            return cursor == text.Length - 1 && text[cursor] == '\'' ? new DynamicSqlSourceMap(characters) : null;
        }

        public static DynamicSqlSourceMap Join(IEnumerable<DynamicSqlSourceMap> maps)
        {
            var parts = maps.ToArray();
            return parts.Any(p => p == null) ? null : new DynamicSqlSourceMap(parts.SelectMany(p => p._characters).ToArray());
        }

        public DynamicSqlSourceMap InOriginalScope(Scope parent)
        {
            var result = new SourceRange[_characters.Length];
            for (int i = 0; i < result.Length; i++)
            {
                AnalysisWork.Check();
                // A character spelling must be contiguous; never guess across unrelated source fragments.
                if (!parent.TryOriginalRange(_characters[i].StartOffset, _characters[i].Length, out result[i])) return null;
            }
            return new DynamicSqlSourceMap(result);
        }

        public bool TryRange(int start, int length, out SourceRange range)
        {
            range = default;
            if (start < 0 || length <= 0 || start > _characters.Length - length) return false;
            int first = _characters[start].StartOffset, end = first;
            for (int i = start; i < start + length; i++)
            {
                if (_characters[i].StartOffset != end) return false;
                end += _characters[i].Length;
            }
            range = new SourceRange(first, end - first);
            return true;
        }

        public IReadOnlyList<SourceRange> Ranges(int start, int length)
        {
            if (start < 0 || length <= 0 || start > _characters.Length - length) return Array.Empty<SourceRange>();
            var result = new List<SourceRange>();
            for (int i = start; i < start + length; i++)
            {
                AnalysisWork.Check();
                var next = _characters[i];
                if (result.Count > 0 && result[result.Count - 1].StartOffset + result[result.Count - 1].Length == next.StartOffset)
                {
                    var last = result[result.Count - 1];
                    result[result.Count - 1] = new SourceRange(last.StartOffset, last.Length + next.Length);
                }
                else result.Add(next);
            }
            return result.AsReadOnly();
        }
    }
}
