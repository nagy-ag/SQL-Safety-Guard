using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SqlSafetyGuard.Services
{
    internal sealed class ConnectionContext
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Description => $"Server: {Server ?? "server unknown"}   ·   Database: {Database ?? "database unknown"}";
    }

    /// <summary>
    /// Best-effort lookup of the active query window's server and database. Relies on undocumented SSMS
    /// internals, so unavailable fields remain explicitly unknown and use global settings.
    /// </summary>
    internal static class ConnectionInfoReader
    {
        private const string ServiceCacheTypeName = "Microsoft.SqlServer.Management.UI.VSIntegration.ServiceCache";

        public static ConnectionContext TryRead(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string server = null, database = null;
            try
            {
                ReadConnectionInfo(ref server, ref database);
            }
            catch (Exception ex)
            {
                OutputLog.Info($"Server/database lookup unavailable ({ex.GetType().Name}).");
            }
            try { database = ReadDatabaseCombo(serviceProvider) ?? database; }
            catch (Exception ex) { OutputLog.Info($"Database combo unavailable ({ex.GetType().Name})."); }
            return new ConnectionContext
            {
                Server = string.IsNullOrWhiteSpace(server) ? null : server,
                Database = string.IsNullOrWhiteSpace(database) ? null : database,
            };
        }

        /// <summary>ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo (SSMS internals).</summary>
        private static void ReadConnectionInfo(ref string server, ref string database)
        {
            var serviceCacheType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && IsSsmsAssembly(a.GetName().Name))
                .Select(a => SafeGetType(a, ServiceCacheTypeName))
                .FirstOrDefault(t => t != null);
            if (serviceCacheType == null)
                return;

            var scriptFactory = GetProperty(serviceCacheType, null, "ScriptFactory");
            var connectionInfo = GetProperty(scriptFactory?.GetType(), scriptFactory, "CurrentlyActiveWndConnectionInfo");
            var uiConnectionInfo = GetProperty(connectionInfo?.GetType(), connectionInfo, "UIConnectionInfo");
            if (uiConnectionInfo == null)
                return;

            server = GetProperty(uiConnectionInfo.GetType(), uiConnectionInfo, "ServerName") as string;
            if (GetProperty(uiConnectionInfo.GetType(), uiConnectionInfo, "AdvancedOptions") is NameValueCollection options)
                database = options["DATABASE"];
        }

        /// <summary>
        /// Reads the current value of the SQL editor's "Available Databases" combo using the standard
        /// VS combo protocol (Exec with no input and an output VARIANT returns the current value).
        /// This reflects USE statements that already ran.
        /// </summary>
        private static string ReadDatabaseCombo(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!(serviceProvider.GetService(typeof(SUIHostCommandDispatcher)) is Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget dispatcher))
                return null;

            var commandSet = SsmsCommands.QueryEditorCommandSet;
            IntPtr variant = Marshal.AllocCoTaskMem(64);
            try
            {
                for (int i = 0; i < 64; i++)
                    Marshal.WriteByte(variant, i, 0);

                int hr = dispatcher.Exec(ref commandSet, SsmsCommands.AvailableDatabasesCombo, 0, IntPtr.Zero, variant);
                if (ErrorHandler.Failed(hr))
                    return null;

                var value = Marshal.GetObjectForNativeVariant(variant) as string;
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            finally
            {
                NativeMethods.VariantClear(variant);
                Marshal.FreeCoTaskMem(variant);
            }
        }

        private static bool IsSsmsAssembly(string name) =>
            name != null &&
            (name.StartsWith("SqlPackageBase", StringComparison.OrdinalIgnoreCase) || // SSMS 22
             name.StartsWith("SqlWorkbench", StringComparison.OrdinalIgnoreCase) ||
             name.StartsWith("SQLEditors", StringComparison.OrdinalIgnoreCase) ||
             name.StartsWith("Microsoft.SqlServer.Management.SqlStudio", StringComparison.OrdinalIgnoreCase));

        private static Type SafeGetType(Assembly assembly, string name)
        {
            try
            {
                return assembly.GetType(name, false);
            }
            catch
            {
                return null;
            }
        }

        private static object GetProperty(Type type, object instance, string name)
        {
            if (type == null)
                return null;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | (instance == null ? BindingFlags.Static : BindingFlags.Instance);
            return type.GetProperty(name, flags)?.GetValue(instance);
        }

        private static class NativeMethods
        {
            [DllImport("oleaut32.dll")]
            public static extern int VariantClear(IntPtr variant);
        }
    }
}
