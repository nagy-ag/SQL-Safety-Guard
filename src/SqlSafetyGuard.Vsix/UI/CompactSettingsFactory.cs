using System;
using System.Runtime.InteropServices;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SqlSafetyGuard.UI
{
    internal sealed class CompactSettingsFactory : WpfUIFactory
    {
        internal const string GuidString = "58c1ef91-2879-445f-b373-7cb8972d9d1f";
        private static readonly Guid FactoryId = new Guid(GuidString);

#pragma warning disable VSTHRD010 // Constructed only by Register after its UI-thread check.
        private CompactSettingsFactory(SqlSafetyGuardPackage package)
            : base(package, (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)package) { }
#pragma warning restore VSTHRD010

        internal static void Register(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var service = Package.GetGlobalService(typeof(SVsUIFactory)) as IVsRegisterUIFactories;
            if (service == null) throw new InvalidOperationException("SSMS settings UI factory service is unavailable.");
            var id = FactoryId;
            Marshal.ThrowExceptionForHR(service.RegisterUIFactory(ref id, new CompactSettingsFactory(package)));
        }

        protected override WpfUIFactoryElement[] GetFactoryElements() => new[]
        {
            new WpfUIFactoryElement(FactoryId, 1, typeof(CompactSettingsControl))
        };
    }
}
