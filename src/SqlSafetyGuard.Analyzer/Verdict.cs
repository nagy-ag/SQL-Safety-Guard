namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Classification of a statement or of a whole script, ordered from least to most severe.
    /// </summary>
    public enum Verdict
    {
        /// <summary>SELECT, variable/session/control statements.</summary>
        ReadOnly = 0,

        /// <summary>Writes only to #temp, ##temp, @table variables (and optionally tempdb objects).</summary>
        TempOnly = 1,

        /// <summary>Writes only to tables matching the user's allow-list.</summary>
        AllowListed = 2,

        /// <summary>The analyzer cannot see what runs (procedures, runtime dynamic SQL, parse errors...).</summary>
        Unknown = 3,

        /// <summary>A change to a permanent object.</summary>
        RealWrite = 4,
    }

    public static class VerdictExtensions
    {
        /// <summary>True for ReadOnly, TempOnly and AllowListed: the query may run without a prompt.</summary>
        public static bool IsGreen(this Verdict verdict) => verdict <= Verdict.AllowListed;

        public static bool IsYellow(this Verdict verdict) => verdict == Verdict.Unknown;

        public static bool IsRed(this Verdict verdict) => verdict == Verdict.RealWrite;

        public static string DisplayName(this Verdict verdict)
        {
            switch (verdict)
            {
                case Verdict.ReadOnly: return "Read-only";
                case Verdict.TempOnly: return "Temp only";
                case Verdict.AllowListed: return "Allow-listed";
                case Verdict.Unknown: return "Unknown";
                case Verdict.RealWrite: return "Real write";
                default: return verdict.ToString();
            }
        }

        public static Verdict Max(Verdict a, Verdict b) => a >= b ? a : b;
    }
}
