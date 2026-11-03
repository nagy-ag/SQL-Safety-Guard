using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.Commands
{
    internal static class UninstallCommand
    {
        public static void Initialize(SqlSafetyGuardPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            commandService.AddCommand(new OleMenuCommand((s, e) => Execute(package),
                new CommandID(PackageGuids.CommandSet, PackageIds.UninstallCommand)));
        }

        private static void Execute(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                string hostExecutable;
                using (var host = Process.GetCurrentProcess())
                    hostExecutable = host.MainModule?.FileName ?? throw new InvalidOperationException("The running SSMS path is unavailable.");
                string isolationIni = File.ReadAllText(Path.Combine(Path.GetDirectoryName(hostExecutable), "SSMS.isolation.ini"));
                var plan = ExtensionUninstallPlan.Create(hostExecutable, isolationIni,
                    typeof(SqlSafetyGuardPackage).Assembly.Location,
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Environment.GetCommandLineArgs());
                if (!File.Exists(plan.InstallerPath))
                    throw new FileNotFoundException("SSMS's VSIXInstaller.exe was not found. Repair SSMS or ask your administrator for help.");

                plan.Request(() => Confirm(package, plan), startInfo =>
                {
                    using (var installer = Process.Start(startInfo))
                        if (installer == null) throw new InvalidOperationException("SSMS's uninstaller could not be started.");
                    OutputLog.Info("Opened the SSMS uninstaller. Removal is pending the installer's confirmation and completion.");
                });
            }
            catch (Exception ex)
            {
                // Maintenance failure must not change the query guard's mode or health.
                OutputLog.Error("Could not open the SSMS uninstaller.", ex);
                VsShellUtilities.ShowMessageBox(package, "SQL Safety Guard could not open its uninstaller.\n\n" + ex.Message,
                    "Uninstall SQL Safety Guard", OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private static bool Confirm(SqlSafetyGuardPackage package, ExtensionUninstallPlan plan)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string profile = plan.RootSuffix.Length == 0 ? "default profile" : "profile " + plan.RootSuffix;
            return VsShellUtilities.ShowMessageBox(package,
                "Open the SSMS uninstaller for SQL Safety Guard?\n\n" +
                "Target: SSMS installation " + plan.InstallationId + ", " + profile + ".\n\n" +
                "Save your work and close SSMS when the installer asks. Removal finishes in the installer. " +
                "This command does not disable the query guard.",
                "Uninstall SQL Safety Guard", OLEMSGICON.OLEMSGICON_QUERY,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND) == (int)VSConstants.MessageBoxResult.IDYES;
        }
    }
}
