using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;
using SqlSafetyGuard.UI;

namespace SqlSafetyGuard.Commands
{
    internal static class SettingsCommands
    {
        private const string Filter = "SQL Safety Guard settings (*.ssgsettings.json)|*.ssgsettings.json|JSON files (*.json)|*.json|All files (*.*)|*.*";

        public static void Initialize(SqlSafetyGuardPackage package, OleMenuCommandService service)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            service.AddCommand(new OleMenuCommand((s, e) => Export(package), new CommandID(PackageGuids.CommandSet, PackageIds.ExportSettingsCommand)));
            service.AddCommand(new OleMenuCommand((s, e) => Import(package), new CommandID(PackageGuids.CommandSet, PackageIds.ImportSettingsCommand)));
        }

        internal static SettingsSnapshot Capture(GeneralOptionsPage page) => new SettingsSnapshot((ExecutionMode)page.GuardMode,
            page.ShowServerAndDatabase, page.AllowList, page.TempdbIsTemp, page.TrustedProcedures, page.EnvironmentRules);

        private static void Export(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var dialog = new SaveFileDialog { Title = "Export SQL Safety Guard Settings", Filter = Filter,
                    FileName = "SqlSafetyGuard.ssgsettings.json", DefaultExt = ".ssgsettings.json", AddExtension = true, OverwritePrompt = true };
                if (dialog.ShowDialog() != true) return;
                ExportFile(package, dialog.FileName);
                Message(package, "Settings exported to " + dialog.FileName, false);
            }
            catch (Exception ex) { Message(package, "Could not export settings: " + ex.Message, true); }
        }

        internal static void ExportFile(SqlSafetyGuardPackage package, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var assembly = typeof(SqlSafetyGuardPackage).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? assembly.GetName().Version.ToString();
            File.WriteAllText(path, SettingsTransfer.Export(Capture(package.Options), version, DateTimeOffset.UtcNow), new UTF8Encoding(false));
        }

        private static void Import(SqlSafetyGuardPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var dialog = new OpenFileDialog { Title = "Import SQL Safety Guard Settings", Filter = Filter, CheckFileExists = true, Multiselect = false };
                if (dialog.ShowDialog() != true) return;
                ImportFile(package, dialog.FileName);
            }
            catch (Exception ex) { Message(package, "Settings were not imported. " + ex.Message, true); }
        }

        internal static bool ImportFile(SqlSafetyGuardPackage package, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // Bound the file before allocating/parsing it, even when it contains unusually large UTF-8 data.
            if (new FileInfo(path).Length > SettingsTransfer.MaximumFileCharacters * 4L)
                throw new FormatException("Settings file exceeds the size limit.");
            var import = SettingsTransfer.PrepareImport(File.ReadAllText(path, new UTF8Encoding(false, true)), Capture(package.Options));
            if (new SettingsImportDialog(import).ShowModal() != true) return false;
            Apply(package.Options, import);
            package.RefreshHealth();
            try { SafetyPaneMargin.RefreshAllSettings(); }
            catch (Exception ex) { Services.OutputLog.Error("Settings were imported, but refreshing the safety panes failed. Reopen the query windows.", ex); }
            return true;
        }

        internal static void Apply(GeneralOptionsPage page, SettingsImport import)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            SettingsTransfer.Apply(import, () => Capture(page), value => Assign(page, value), page.SaveSettingsToStorage);
        }

        private static void Assign(GeneralOptionsPage page, SettingsSnapshot value)
        {
            page.GuardMode = (GuardMode)value.GuardMode;
            page.ShowServerAndDatabase = value.ShowServerAndDatabase;
            page.AllowList = value.AllowList.Replace("\n", Environment.NewLine);
            page.TempdbIsTemp = value.TempdbIsTemp;
            page.TrustedProcedures = value.TrustedProcedures.Replace("\n", Environment.NewLine);
            page.EnvironmentRules = value.EnvironmentRules.Replace("\n", Environment.NewLine);
        }

        private static void Message(IServiceProvider package, string text, bool error)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            VsShellUtilities.ShowMessageBox(package, text, "SQL Safety Guard",
                error ? OLEMSGICON.OLEMSGICON_WARNING : OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
