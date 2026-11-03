#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Services
{
    /// <summary>
    /// Local installer handoff, independent of query analysis. Refuses an ambiguous host/profile or a
    /// non-per-user installation rather than removing the extension from a different SSMS profile.
    /// </summary>
    internal sealed class ExtensionUninstallPlan
    {
        public const string ExtensionId = "SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558";
        public string InstallerPath { get; }
        public string InstallationId { get; }
        public string RootSuffix { get; }

        private ExtensionUninstallPlan(string installerPath, string installationId, string rootSuffix)
        {
            InstallerPath = installerPath;
            InstallationId = installationId;
            RootSuffix = rootSuffix;
        }

        // All inputs come from the running process, its adjacent isolation INI and this loaded assembly.
        // No discovery of another installation, registry edits, deletion or shutdown belongs here.
        public static ExtensionUninstallPlan Create(string hostExecutable, string isolationIni,
            string assemblyPath, string localAppData, string[] commandLine)
        {
            if (!string.Equals(Path.GetFileName(hostExecutable), "SSMS.exe", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The running SSMS installation could not be identified.");

            string installationId = ReadInstallationId(isolationIni);
            string rootSuffix = ReadRootSuffix(commandLine);
            if (string.IsNullOrWhiteSpace(localAppData) || string.IsNullOrWhiteSpace(assemblyPath))
                throw new InvalidOperationException("The current extension profile could not be identified.");

            string extensionsRoot = Path.GetFullPath(Path.Combine(localAppData, "Microsoft", "SSMS",
                "22.0_" + installationId + rootSuffix, "Extensions")) + Path.DirectorySeparatorChar;
            string loadedAssembly = Path.GetFullPath(assemblyPath);
            if (!loadedAssembly.StartsWith(extensionsRoot, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetDirectoryName(loadedAssembly) + Path.DirectorySeparatorChar,
                    extensionsRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "This is not a recognized per-user VSIX installation in the current SSMS profile. " +
                    "For an administrator-managed or manually copied installation, ask your administrator " +
                    "to remove it. For a default-profile installation, close SSMS and use uninstall.ps1 " +
                    "from the project repository; isolated profiles need profile-specific removal.");

            string hostDirectory = Path.GetDirectoryName(Path.GetFullPath(hostExecutable))!;
            return new ExtensionUninstallPlan(Path.Combine(hostDirectory, "VSIXInstaller.exe"), installationId, rootSuffix);
        }

        /// <summary>Cancellation has no side effects. Starting the installer does not mean removal has completed.</summary>
        public bool Request(Func<bool> confirm, Action<ProcessStartInfo> startInstaller)
        {
            if (confirm == null) throw new ArgumentNullException(nameof(confirm));
            if (startInstaller == null) throw new ArgumentNullException(nameof(startInstaller));
            if (!confirm()) return false;

            string arguments = "/instanceIds:" + InstallationId;
            if (RootSuffix.Length != 0) arguments += " /rootSuffix:" + RootSuffix;
            arguments += " /uninstall:" + ExtensionId;
            startInstaller(new ProcessStartInfo
            {
                FileName = InstallerPath,
                WorkingDirectory = Path.GetDirectoryName(InstallerPath),
                Arguments = arguments,
                UseShellExecute = false
            });
            return true;
        }

        private static string ReadInstallationId(string ini)
        {
            string? id = null;
            bool inInfo = false;
            using (var reader = new StringReader(ini))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("[", StringComparison.Ordinal))
                        inInfo = string.Equals(line, "[Info]", StringComparison.OrdinalIgnoreCase);
                    if (!inInfo || line.StartsWith(";", StringComparison.Ordinal) || line.StartsWith("#", StringComparison.Ordinal))
                        continue;
                    int equals = line.IndexOf('=');
                    if (equals < 0 || !string.Equals(line.Substring(0, equals).Trim(), "InstallationID", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (id != null) throw new InvalidOperationException("The SSMS installation ID is ambiguous.");
                    id = line.Substring(equals + 1).Trim();
                }
            }
            if (!IsIdentifier(id)) throw new InvalidOperationException("The SSMS installation ID is missing or invalid.");
            return id!;
        }

        private static string ReadRootSuffix(string[] commandLine)
        {
            string? suffix = null;
            foreach (string argument in commandLine)
                if (argument == null) throw new InvalidOperationException("The SSMS command line could not be identified.");

            for (int i = 0; i < commandLine.Length; i++)
            {
                string argument = commandLine[i];
                if (!argument.StartsWith("/rootSuffix", StringComparison.OrdinalIgnoreCase) &&
                    !argument.StartsWith("-rootSuffix", StringComparison.OrdinalIgnoreCase)) continue;
                if (suffix != null) throw new InvalidOperationException("The SSMS profile suffix is ambiguous.");
                if (argument.Length == 11 && i + 1 < commandLine.Length)
                    suffix = commandLine[++i];
                else if (argument.StartsWith("/rootSuffix:", StringComparison.OrdinalIgnoreCase) ||
                         argument.StartsWith("-rootSuffix:", StringComparison.OrdinalIgnoreCase))
                    suffix = argument.Substring(12);
                else
                    throw new InvalidOperationException("The SSMS profile suffix is missing or invalid.");
                if (!IsIdentifier(suffix)) throw new InvalidOperationException("The SSMS profile suffix is missing or invalid.");
            }
            return suffix ?? string.Empty;
        }

        private static bool IsIdentifier(string? value) =>
            value != null && Regex.IsMatch(value, @"\A[A-Za-z0-9][A-Za-z0-9_-]{0,63}\z");
    }
}
