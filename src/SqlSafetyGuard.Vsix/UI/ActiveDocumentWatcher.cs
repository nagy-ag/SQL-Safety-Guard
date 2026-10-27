using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SqlSafetyGuard.UI
{
    /// <summary>Refreshes the guard status (status bar, environment) when the active document (editor tab) changes.</summary>
    internal sealed class ActiveDocumentWatcher : IVsSelectionEvents
    {
        private readonly SqlSafetyGuardPackage _package;

        public ActiveDocumentWatcher(SqlSafetyGuardPackage package)
        {
            _package = package;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // Selection events are raised on the UI thread.
            if (elementid == (uint)VSConstants.VSSELELEMID.SEID_DocumentFrame && !ReferenceEquals(varValueOld, varValueNew))
                _package.RefreshHealth();
            return VSConstants.S_OK;
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
            IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew) => VSConstants.S_OK;

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive) => VSConstants.S_OK;
    }
}
