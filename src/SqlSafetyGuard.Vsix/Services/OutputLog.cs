using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SqlSafetyGuard.Services
{
    /// <summary>
    /// Writes to the "SQL Safety Guard" pane of the Output window. Never logs query text.
    /// </summary>
    internal static class OutputLog
    {
        private static IVsOutputWindowPane _pane;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (!(serviceProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow))
                    return;

                var paneGuid = PackageGuids.OutputPane;
                outputWindow.CreatePane(ref paneGuid, "SQL Safety Guard", 1, 0);
                if (ErrorHandler.Succeeded(outputWindow.GetPane(ref paneGuid, out var pane)))
                    _pane = pane;
            }
            catch
            {
                // Logging must never break the extension.
            }
        }

        public static void Info(string message) => Write("INFO", message);

        public static void Warning(string message) => Write("WARN", message);

        public static void Error(string message, Exception exception = null) =>
            Write("ERROR", exception == null ? message : $"{message} {exception.GetType().Name}: {exception.Message}{Environment.NewLine}{exception.StackTrace}");

        private static void Write(string level, string message)
        {
            try
            {
#pragma warning disable VSTHRD010 // OutputStringThreadSafe may be called from any thread.
                _pane?.OutputStringThreadSafe($"[{DateTime.Now:HH:mm:ss}] {level} {message}{Environment.NewLine}");
#pragma warning restore VSTHRD010
            }
            catch
            {
                // Ignore.
            }
        }
    }
}
