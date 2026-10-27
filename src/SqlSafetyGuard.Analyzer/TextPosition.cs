namespace SqlSafetyGuard.Analyzer
{
    public static class TextPosition
    {
        /// <summary>
        /// Converts a 0-based character offset into a 1-based line and column. \r\n, \n and \r each count as one line break.
        /// </summary>
        public static void FromOffset(string text, int offset, out int line, out int column)
        {
            line = 1;
            column = 1;
            if (string.IsNullOrEmpty(text))
                return;

            if (offset > text.Length)
                offset = text.Length;

            for (int i = 0; i < offset; i++)
            {
                char c = text[i];
                if (c == '\r')
                {
                    if (i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        if (i + 1 == offset)
                        {
                            // Offset points at the \n of a \r\n pair: stay at the end of this line.
                            column++;
                            return;
                        }

                        i++;
                    }

                    line++;
                    column = 1;
                }
                else if (c == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }
        }

        /// <summary>Counts line breaks in text[0..offset).</summary>
        internal static int CountLineBreaks(string text, int offset)
        {
            FromOffset(text, offset, out var line, out _);
            return line - 1;
        }
    }
}
