# Development

Build and debug instructions for SQL Safety Guard. For installation and daily use, see the
[user guide](user-guide.md). Architecture: [architecture.md](architecture.md). Current evidence: [verification/status.md](verification/status.md).

## Solution layout

| Path | What |
|---|---|
| `src/SqlSafetyGuard.Analyzer` | netstandard2.0 class library, no SSMS dependency. All parsing/classification (ScriptDom). |
| `tests/SqlSafetyGuard.Tests` | xUnit analyzer, policy, resource-budget and installer regression tests. |
| `tests/SqlSafetyGuard.ServerHarness` | Explicit offline/SQL verification modes; dynamic analyzer loading and durable campaign receipts. |
| `src/SqlSafetyGuard.Vsix` | The SSMS extension: package, commands, F5 guard, dialog, safety pane (editor margin), options page (net472). |
| `install.ps1` / `uninstall.ps1` | Find SSMS 22, install/remove the VSIX. Shared helpers in `scripts/SsmsCommon.ps1`. |
| `SqlSafetyGuard.sln` | Opens in Visual Studio 2026. |

## Build

Requirements: Visual Studio 2026 with the **Visual Studio extension development** and **.NET desktop development**
workloads; the .NET 10.0.4xx SDK feature band selected by global.json. Offline/release verification also needs
Python 3.10+ and Windows PowerShell 5.1. Package closures are locked; restore uses the configured NuGet sources.

In Visual Studio: open `SqlSafetyGuard.sln`, choose **Release**, **Build > Build Solution**.
Output: `src\SqlSafetyGuard.Vsix\bin\Release\SqlSafetyGuard.vsix`.
Release builds use deterministic compilation and map repository source paths, including the SourceLink document map,
to `/_/SqlSafetyGuard/`.
Debug builds keep normal local paths. [release-process.md](release-process.md) covers asset validation and publication.

From a terminal:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" SqlSafetyGuard.sln /restore /p:RestoreLockedMode=true /p:Configuration=Release
dotnet test tests\SqlSafetyGuard.Tests
```

(`dotnet build` can build the analyzer and tests but not the VSIX project, which needs Visual Studio's MSBuild.)

After changing the extension version, use **Rebuild Solution** or add `/t:Rebuild` to MSBuild. An incremental VSSDK build
can retain an old generated `extension.vsixmanifest` even when the assembly version changes. Check the manifest inside
the VSIX; the release script already rebuilds and rejects a mismatched version.

To intentionally change dependencies, review project changes, run restore without locked mode to refresh the affected
packages.lock.json files, review that diff and run the required checks. Do not delete lock files to bypass a failed gate.

## Debug

1. Build **Debug**. Install the VSIX into an isolated profile, for example `Exp`, with SSMS's
   `VSIXInstaller.exe /quiet /instanceIds:<id> /rootSuffix:Exp <Debug VSIX>`. The VSIX includes PDBs.
   Close that profile before reinstalling: uninstall the extension there, install, then run
   `SSMS.exe /rootSuffix Exp /updateconfiguration`. `install.ps1` targets the normal profile, so do not use it for this check.
2. Either press **F5** in Visual Studio (the VSIX project's start action launches
   `SSMS.exe /rootSuffix Exp -nosplash` under the debugger), or
   start `SSMS.exe /rootSuffix Exp -nosplash` and use **Debug > Attach to Process > `Ssms.exe`** (Managed (.NET Framework) code).
3. Breakpoints in `ExecuteGuard.OnBeforeExecute` and `AnalyzeQueryCommand.Execute` are good starting points.
   `SSMS.exe -log <file>.xml` writes an activity log that shows whether `SqlSafetyGuardPackage` loaded.
4. To test the failure path, start an isolated SSMS (Debug build installed) with
   `SQLSAFETYGUARD_FAULT_INJECTION=1`. A checked execution throws before classification and shows the failure box:
   Cancel blocks execution; Run anyway allows that dispatch to continue. Known Off settings skip a verified-target
   check. Output logs the failure, the toolbar shows degraded health, and an InfoBar is attempted once per session.
   The switch is compiled out of Release builds.
5. Debug builds also write a pane timing trace (no query text) to `%TEMP%\SqlSafetyGuard-pane-trace.log`, and the
   pane accepts test commands from `%TEMP%\SqlSafetyGuard-pane-cmd.txt` (lines `documentName|verb|argument`, e.g.
   `SQLQuery1.sql|caret|12`, `|select|8 10`, `|append|DELETE FROM t;`, `|pick|dbo.Orders`, `|collapse`, `|dump`;
   see `SafetyPaneMargin.TestChannel.cs`). `page|settings` / `page|analysis` switch pages; `safe` toggles the footer.
   `dump` includes header counts/scope/environment, sections and all object rows, plus effective settings and pattern
   sources. That lets the pane be tested without driving the mouse or keyboard.
   `guardfailurecheck|Cancel` and `guardfailurecheck|Run anyway` invoke the production guard handler with fault
   injection and exercise the themed failure box. They journal CancelDefault instead of dispatching SQL; this checks
   the handler decision, not an actual server submission. Set `SQLSAFETYGUARD_TEST_DIRECTORY` to a private folder
   for the channel and `events.tsv`. Neither tracing nor the channel exists in Release builds.

Visual Studio's own "deploy to experimental instance" is turned off (`DeployExtension=false`): that would target
Visual Studio, not SSMS.

## Validation boundaries

Run `scripts/Verify-Offline.ps1` for locked restore, the offline test suite, generator freshness and Python tool tests.
The GitHub workflow runs the same offline checks on Windows. It does not install SSMS or connect to SQL Server.

For VSIX changes, build Debug and Release with Visual Studio MSBuild. Run the in-process WPF interaction scripts for
changed surfaces; these load controls without starting SSMS or executing SQL. See [verification/extension/theme-capture.md](verification/extension/theme-capture.md)
for optional isolated visual capture. The complete theme pass requires the owner's explicit finished-release instruction.

Unit guard tests establish policy decisions; XAML tests establish structure/defaults; in-process interaction checks establish
control behavior. Actual host target binding and server submission require separate integration evidence. A successful
build or any one of these layers does not prove the others. See [verification/how-to.md](verification/how-to.md) for the evidence map.
