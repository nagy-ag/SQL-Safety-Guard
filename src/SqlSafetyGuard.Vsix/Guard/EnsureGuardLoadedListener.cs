using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SqlSafetyGuard.Guard
{
    /// <summary>
    /// Loads the package as soon as the first editor window is created.
    ///
    /// SSMS defers background auto-loaded packages until startup has finished (10-30 s in testing), while a
    /// query window opened at startup (e.g. double-clicking a .sql file) is usable within a second or two.
    /// Without this, F5 in that window would run unguarded until the package loaded. MEF text view listeners
    /// are created synchronously with the view, so the guard is attached before the user can type or execute.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class EnsureGuardLoadedListener : IWpfTextViewCreationListener
    {
        private static bool _loadRequested;

        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_loadRequested)
                return;
            _loadRequested = true;

            try
            {
                if (!(ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) is IVsShell shell))
                    return;

                var packageGuid = new Guid(PackageGuids.PackageString);
                if (shell.IsPackageLoaded(ref packageGuid, out _) != VSConstants.S_OK)
                    shell.LoadPackage(ref packageGuid, out _);
            }
            catch (Exception)
            {
                // The background auto-load registration still loads the package later.
            }
        }
    }
}
