using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace SqlSafetyGuard.UI
{
    [Guid(PackageGuids.ResultsToolWindowString)]
    public sealed class ResultsToolWindow : ToolWindowPane
    {
        public ResultsToolWindow()
            : base(null)
        {
            Caption = "SQL Safety Guard";
            BitmapImageMoniker = KnownMonikers.SQLQueryChecked;
            Control = new ResultsControl();
            Content = Control;
        }

        internal ResultsControl Control { get; }

        protected override void OnCreate()
        {
            base.OnCreate();

            // When the window is shown again, catch up with whichever tab is active by then.
            Control.BecameVisible += () => (Package as SqlSafetyGuardPackage)?.RequestResultsRefresh();
        }
    }
}
