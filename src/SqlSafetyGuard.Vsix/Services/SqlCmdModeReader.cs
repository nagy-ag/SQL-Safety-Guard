using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SqlSafetyGuard.Services
{
    /// <summary>
    /// Reads whether Query &gt; SQLCMD Mode is on for the active query window: the same "checked" command status SSMS
    /// uses to draw the check mark in the menu. Returns null when it can't be read (the analyzer then assumes on).
    /// </summary>
    internal static class SqlCmdModeReader
    {
        public static bool? TryRead(System.IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (!(serviceProvider.GetService(typeof(SUIHostCommandDispatcher)) is IOleCommandTarget dispatcher))
                    return null;

                var commandSet = SsmsCommands.QueryEditorCommandSet;
                var commands = new[] { new OLECMD { cmdID = SsmsCommands.SqlCmdMode } };
                if (ErrorHandler.Failed(dispatcher.QueryStatus(ref commandSet, 1, commands, IntPtr.Zero)))
                    return null;

                var flags = (OLECMDF)commands[0].cmdf;
                if ((flags & OLECMDF.OLECMDF_SUPPORTED) == 0)
                    return null;
                return (flags & OLECMDF.OLECMDF_LATCHED) != 0;
            }
            catch (Exception ex)
            {
                OutputLog.Info($"SQLCMD mode unavailable ({ex.GetType().Name}); SQLCMD lines are interpreted.");
                return null;
            }
        }
    }
}
