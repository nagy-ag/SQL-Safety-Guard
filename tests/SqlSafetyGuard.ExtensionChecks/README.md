# Isolated SSMS extension checks

Windows PowerShell 5.1; SSMS 22, VS extension-development MSBuild, .NET SDK 10, and Windows-authenticated SQL Express are required. The SQL login must be able to create/drop its own database and server Extended Events session. Read the repository README first.

Run from the repository root:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\SqlSafetyGuard.ExtensionChecks\Invoke-Checks.ps1
~~~

Optional parameters: -Server (default .\SQLEXPRESS), -MSBuild (default VS 18 Community). The driver builds Debug, installs its VSIX with VSIXInstaller /rootSuffix:SgExt<random>, creates one SgExt_<random> database and identically named XE session, and starts exactly one SSMS process with that suffix. Never use install.ps1 here: that script targets the normal profile. No existing database is adopted, altered, or dropped. The independent Counter table has red/yellow/outside rows; dbo.Tick changes yellow. Nothing uses another agent's SgPlay_* or SgRun_* databases.

Evidence is retained under %TEMP%\SgExt<random>: ownership.json, events.tsv, commands.tsv, results-*.json, installer/activity logs. The experimental profile/installed VSIX is retained for inspection; the SSMS process, XE session and database are removed in finally. If interrupted, recover with:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\SqlSafetyGuard.ExtensionChecks\Stop-Checks.ps1 -OwnershipFile <exact-path-to-ownership.json>
~~~

Cleanup validates suffix, PID and process start time before sending the Debug close command; it drops only resources recorded as successfully created. Do not edit an ownership manifest to adopt someone else's resources. Cleanup deliberately refuses changed process identity or a process that cannot quit; inspect and retry rather than killing other SSMS processes. Test data and query documents are disposable; the close command marks this process's documents saved.

## No desktop input

Client.ps1 enumerates only the selected PID's top-level windows and filters all UIA elements by that PID. Confirmation buttons use InvokePattern. Startup connection confirmation and Save Results cancellation use BM_CLICK on PID-validated native button HWNDs. The shell can be shown with SW_SHOWNOACTIVATE; no foreground/focus API, SendKeys, SendInput, pointer input, or clipboard is used. NativeWindows.cs has no such input APIs. Document.Activate changes the in-process DTE active document, not the OS foreground window.

The Debug-only channel reads documentName|verb|argument lines from SQLSAFETYGUARD_TEST_DIRECTORY\SqlSafetyGuard-pane-cmd.txt. Client writes a stage file and atomically renames it; channel acknowledges completed verbs in events.tsv. replacefile edits that editor's text buffer; select accepts inclusive 1-based line numbers. document/open select/create editors; execute invokes DTE.ExecuteCommand with the real SSMS command name. mode/rulesfile change only this process's in-memory options. commands journals the installed command IDs/bindings. state captures active-document hash, SQLCMD state and the pane's cached summary. dismisswelcome closes only this process's exact What's new DTE surface after startup. close quits this process. Debug guard tracing hashes exactly the analyzed snapshot before showing a confirmation. No tracing/channel code is compiled into Release.

Run-Checks.ps1 can also attach to an already provisioned owned instance with -ChannelDirectory, -SsmsProcessId, -Database and -Server. -FunctionsOnly defines helpers without running the campaign. Only one driver may use a directory at a time. Avoid editing this process manually while it runs. Commands can require SSMS startup to finish; unavailable commands are reported, not simulated. A new profile's What's New surface can leave Query.Execute unavailable even after DTE document activation: setup waits for it and closes that owned surface through the Debug channel. Any remaining unavailable setup command aborts rather than fabricating coverage.

## Evidence and interpretation

Each scenario samples server XE events and all three counters before/after dispatch. XE is scoped to the owned database, then filtered to this PID and exactly the SSMS Query application (not IntelliSense). Truncated/dropped ring-buffer evidence aborts. Unique SQL comments distinguish submissions. Guard UTF-8 SHA-256 must match the intended document/selection and guard document/count must match. Target tracing also records verification warnings, SQLCMD mode and the same owner's server/database; assertions verify those fields and the yellow warning text through UIA. Server SQL comparison removes CR characters and trims outer whitespace; internal text must match. Thus the server comparison is not byte-for-byte whitespace verification. SQLCMD-on expects expanded SQL without :setvar. Assertions also check dialog presence and counter delta. Cancel should have no target submission. Normal Run should have one; replaying the command to implement Run anyway would fail.

ObservedCount counts target completions with XE result=0 for ordinary commands, not a guarantee of SQL success. BatchSubmissionCount counts all observed target completions regardless of that result; inspect it as well when checking duplication. SQLCMD-off can submit once and fail syntax with zero counter changes. Parse/estimated-plan ObservedCount uses counter changes (zero); BatchSubmissionCount still records the submitted batch. SET PARSEONLY, SHOWPLAN_XML and STATISTICS XML wrapper submissions are excluded from target counts. Batches contains the actual server SQL and XE result. Do not infer success from XE result=0 alone.

PASS means assertions passed for the stated command-dispatch path, not complete shortcut/toolbar coverage. FAIL is a mismatch; BLOCKED is an unavailable automation surface or dispatch failure. The multi-editor regressions keep the original no-focus routing intact: B is DTE-active while A remains the focused SQL command target. They now require analysis of A plus a visible target-ambiguity warning. Cancel must submit nothing; approved Run must execute exactly the captured A text once. Additional cases require the warning even in Off mode and for green text in Red-only mode. The 36-scenario campaign currently has 35 PASS and one toolbar BLOCKED; all 30 baseline passing scenarios remain PASS. Review the results JSON even when the PowerShell process completes normally. Setup/runtime errors throw; individual scenario failures accumulate for diagnosis.

Physical F5/Ctrl+E/Alt+X are never synthesized. Their bindings are inventoried and their shared Query.Execute command is dispatched. Toolbar Invoke is attempted but blocked when the shell exposes no UIA descendants. Results-to-file cancellation and guard coverage are tested; successful Save is not claimed because the file-name control lacks ValuePattern. Custom stored-procedure shortcuts are not configured and have no coverage guarantee. See docs/verification/extension-checks.md for measured coverage and reproduction.
