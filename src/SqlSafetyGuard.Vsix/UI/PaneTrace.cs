using System;
using System.Diagnostics;
using System.IO;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// Debug builds only: timing trace in %TEMP%\SqlSafetyGuard-pane-trace.log, to measure how the pane feels.
    /// Never writes query text (object names and counts only). Calls are compiled out of Release builds.
    /// </summary>
    internal static class PaneTrace
    {
        private static readonly string FilePath = Path.Combine(Path.GetTempPath(), "SqlSafetyGuard-pane-trace.log");
        private static readonly object Gate = new object();

        [Conditional("DEBUG")]
        public static void Write(string message)
        {
            try
            {
                lock (Gate)
                    File.AppendAllText(FilePath, $"{DateTime.Now:HH:mm:ss.fff} {message}{Environment.NewLine}");
            }
            catch
            {
                // Tracing must never break the pane.
            }
        }
    }
}
