# SQL Safety Guard for SSMS 22

A Visual Studio–style extension (VSIX) for **SQL Server Management Studio 22** that analyzes T-SQL *before* it runs
and tells you whether it changes real data or only reads / touches temp objects.

- **Analyze Query Safety**: Tools menu, SQL Editor toolbar button, or **Ctrl+Shift+F5**. Analyzes the selection,
  or the whole window if nothing is selected, and shows a results tool window. **Never executes anything.**
- **F5 guard**: when you execute (F5, Ctrl+E, Alt+X, the Execute button), the exact text SSMS is about to run is
  analyzed. **Green runs with no interruption**; yellow/red shows a confirmation dialog first by default.
- **Environment rules**: first-match server/database rules choose an environment label, warning mode, additive
  allow/trusted lists and optional typed database confirmation for whole-target red changes.
- **Visible guard health**: the SQL Editor toolbar caption and status bar show on/off, environment, unknown
  server or degraded state. A failed check also attempts a once-per-session, non-modal warning InfoBar.

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | 🟢 | SELECT, variable/session/control statements, balanced transactions, read-only/trusted procs |
| Temp only | 🟢 | Writes only to temporary tables/procedures or table variables; permanent tempdb objects require opt-in |
| Allow-listed | 🟢 | Writes only to tables matching your allow-list |
| Unknown | 🟡 | Can't see what runs: EXEC of procedures, runtime dynamic SQL, `INSERT ... EXEC`, DBCC/BACKUP/…, parse errors |
| Real write | 🔴 | Any change to a permanent object (DML, DDL, security, remote writes) |

> **This is not a security boundary.** See [Limitations](#limitations). For production, the real protection is a
> read-only login (`db_datareader` + `db_denydatawriter`).

## Solution layout

| Path | What |
|---|---|
| `src/SqlSafetyGuard.Analyzer` | netstandard2.0 class library, no SSMS dependency. All parsing/classification (ScriptDom). |
| `tests/SqlSafetyGuard.Tests` | xUnit analyzer, policy, resource-budget and installer regression tests. |
| `src/SqlSafetyGuard.Vsix` | The SSMS extension: package, commands, F5 guard, dialog, tool window, options page (net472). |
| `install.ps1` / `uninstall.ps1` | Find SSMS 22, install/remove the VSIX. Shared helpers in `scripts/SsmsCommon.ps1`. |
| `SqlSafetyGuard.sln` | Opens in Visual Studio 2026. |

## Build

Requirements: Visual Studio 2026 with the **Visual Studio extension development** and **.NET desktop development**
workloads; .NET SDK 10 (for the tests). NuGet restore is automatic.

In Visual Studio: open `SqlSafetyGuard.sln`, choose **Release**, **Build > Build Solution**.
Output: `src\SqlSafetyGuard.Vsix\bin\Release\SqlSafetyGuard.vsix`.

From a terminal:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" SqlSafetyGuard.sln /restore /p:Configuration=Release
dotnet test tests\SqlSafetyGuard.Tests
```

(`dotnet build` can build the analyzer and tests but not the VSIX project, which needs Visual Studio's MSBuild.)

## Install

Close SSMS, then from the repository folder:

```powershell
.\install.ps1
```

The script finds the newest SSMS 22 with `vswhere` (product `Microsoft.VisualStudio.Product.Ssms`, versions compared
numerically), validates the package, removes any previous version, and runs **SSMS's own `VSIXInstaller.exe`**
against the SSMS setup instance:

```
"<SSMS>\Common7\IDE\VSIXInstaller.exe" /quiet /instanceIds:<instanceId> SqlSafetyGuard.vsix
```

This is a per-user install. The files land in `%LocalAppData%\Microsoft\SSMS\22.0_<instanceId>\Extensions\<random>\`,
and the extension shows up in SSMS under **Extensions > Manage Extensions**, where it can also be disabled or
uninstalled. Double-clicking the `.vsix` (with SSMS closed) does the same thing. Explorer opens it with VSLauncher →
VSIXInstaller, which installs straight into SSMS 22 when that's the only matching product.

Options: `-VsixPath <path>` to install a specific build (e.g. the Debug one); `-Copy` (elevated PowerShell only) to
extract the VSIX by hand into `<SSMS>\Common7\IDE\Extensions\SqlSafetyGuard` and touch
`extensions.configurationchanged`.

The copy path rejects unsafe archive paths (including traversal, rooted/alternate-stream paths and duplicate
entries), validates the manifest and required assemblies, and extracts into a sibling staging directory **before**
moving the current install. A failed replacement restores the backup. The VSIXInstaller path aborts a failed
uninstall and reports failed upgrades with recovery instructions. **Keep the previous working VSIX**: if the new
install fails after uninstall, reinstall it with `.\install.ps1 -VsixPath <previous.vsix>`. Installer log paths are
quoted even when TEMP contains spaces. Uninstall failures propagate a nonzero exit instead of printing success.

Historical smoke checks on SSMS 22.11.2 (VS shell 18.11), before the current soundness/rules changes:

- ✅ VSIXInstaller per-user install (`install.ps1`): loads, all features work.
- ✅ Double-clicking the `.vsix`: same per-user install.
- ✅ Manual copy into `<SSMS>\Common7\IDE\Extensions` (`install.ps1 -Copy`, elevated): on the next start SSMS
  regenerates its pkgdef cache, imports `SqlSafetyGuard.pkgdef`, and the package, command, shortcut and F5 guard all
  work. `uninstall.ps1` (elevated) removes it. Don't combine it with a per-user install of the same extension.
- ❌ Manual copy into `%LocalAppData%\Microsoft\SSMS\22.0_<id>\Extensions\<folder>` plus touching
  `extensions.configurationchanged`: **not picked up**. Per-user extensions must be registered by VSIXInstaller, so
  `install.ps1` doesn't offer this.

## Use

- **Tools > Analyze Query Safety**, the toolbar button ("SQL with a check mark" icon right after the Execute group
  on the SQL Editor toolbar), or **Ctrl+Shift+F5** in a query window. The **SQL Safety Guard** tool window opens (also available
  from **View > Other Windows**): a coloured verdict banner, then a grid with Line, Action, Target, Verdict and Note.
  Double-click a row, or select it and press Enter, to move the caret there. *Only yellow/red* filters the grid.
  **It follows the active tab:** while the window is visible, switching query tabs re-analyzes the new tab in the
  background (nothing is executed, SSMS doesn't freeze). A non-SQL tab shows a neutral message instead of stale
  results. If the window is hidden, it catches up when you show it again.
- **F5 guard**: a yellow/red query shows a dialog listing only the yellow/red findings (line, action, target, e.g.
  `o → dbo.Orders (via alias o)`), plus the server and database. **Cancel** is the default (Enter/Esc). **Run anyway**
  lets that execution continue; it has no access key. The environment has a prominent coloured banner
  (Production red, Test amber, Development green, other labels blue). With typed confirmation enabled, a
  whole-target red finding (or any red DROP action) requires the **exact initial database name**, including case,
  before Run anyway enables.
  If that database is unknown, cancel and verify the connection first. Double-clicking a finding cancels and jumps to it.
- **Guard state** stays in the toolbar caption even when the status bar is temporarily used by SSMS. A check
  failure marks it degraded and attempts a once-per-session InfoBar as well as logging. A successful guard check
  clears the degraded indicator. Environment rules override the global warning mode, not this failure reporting.
- Messages and errors go to the **SQL Safety Guard** pane of the Output window. Query text is never logged or stored.

Historical interaction checks on SSMS 22.11.2, before the current changes (not a runtime verification of the new UI):
- Commands: the Tools menu item, the toolbar button, Ctrl+Shift+F5, and View > Other Windows (reopens the tool
  window after it has been closed).
- Jumping to code: mouse double-click and Enter on a grid row; double-click on a finding in the F5 dialog.
- Settings: changed in the Options dialog, kept after an SSMS restart, and applied (red-only mode, allow-list,
  hidden server/database line).

The current analyzer/policy/installer changes are regression-tested and the Release VSIX is compiled; the new
health indicator, InfoBar, cancellation and confirmation UI still need an SSMS smoke test.

### Try it

The `samples` folder is a playground:

1. Open `samples\00-setup-sandbox.sql` in SSMS, connected to a local or dev server, and press F5. The guard stops
   it (it creates a database), which is the first demo. Click **Run anyway**. This creates the throwaway
   database `SafetyGuardSandbox`; run the script again any time to reset it.
2. Open `samples\01-try-these.sql`. It has 29 numbered examples (green, yellow, red, settings, multi-batch, temp
   tables, and reads with hidden side effects), each with its expected verdict. Select one and press
   **Ctrl+Shift+F5** to analyze it, or **F5** to see the guard. `SamplesTests.cs` checks every `expect:` line,
   and `WholeFileSamplesTests.cs` checks that analyzing the whole file lists every example.
3. Open `samples\02-broken-batches.sql` and press **Ctrl+Shift+F5** with nothing selected, to see how syntax
   errors, a misplaced `CREATE PROCEDURE` and SQLCMD-mode lines are reported. Analyze it only; don't run it.
4. Drop the sandbox when you're done: `DROP DATABASE SafetyGuardSandbox;`

### Shortcut

The handoff asked for Ctrl+Shift+A, but in SSMS 22 that is already bound globally to `Project.AddNewItem`. The
extension uses **Ctrl+Shift+F5** (free in SSMS 22, scoped to the SQL Query Editor, and "check before F5" is easy to
remember). Ctrl+Alt combinations were avoided because on many European layouts Ctrl+Alt is AltGr and types
characters such as `\`. To change it: **Tools > Options > Environment > Keyboard**, command
`SqlSafetyGuard.AnalyzeQuerySafety`.

## Settings

**Tools > Options > SQL Safety Guard > General**

| Setting | Default | |
|---|---|---|
| F5 guard | Warn on yellow and red | Off / Warn on red only / Warn on yellow and red |
| Show server and database in the dialog | On | Unknown fields always remain visible and use global settings |
| Safe tables allow-list | empty | One pattern per line; `--` lines are comments |
| Treat permanent tempdb objects as temporary | Off | Explicit opt-in; `#temp` / `##temp` remain green regardless |
| Trusted read-only procedures | empty | Schema-qualified patterns; wildcards only in the final part |
| Require typed confirmation globally | Off | Exact database name for whole-target red changes and red DROP actions |
| Server / environment rules | empty | First matching rule overrides mode, environment and typed-confirmation setting |

### Safe tables allow-list

Patterns are case-insensitive, support `*` and `?`, and match **entire identifier parts**, not substrings.
Brackets/double quotes delimit parts; escaped delimiters and quoted whitespace are preserved. One entry per
line (semicolon-separated entries also work); `--` and `#` lines are comments. Wildcard-only entries such as
`*` or `*.*` are ignored with a visible settings warning.

- `Staging_*` matches the object name in any schema, including a schema-less name.
- `dbo.Staging_*` requires a resolved `dbo` schema. **No implicit dbo assumption**: a schema-less query name
  matches only a one-part pattern unless the caller supplies a known default schema. The VSIX does not discover
  the login's default schema, so qualify your query names when relying on schema-qualified entries.
- A pattern with no database part matches unqualified names or names explicitly qualified with the **known
  current database**. It does not match another database, or an explicit database when the current one is unknown.
- `Sandbox.dbo.*` matches explicit Sandbox names, or unqualified database names when the known current database
  is Sandbox. It still requires a resolved schema.
- A pattern with no server part **never** matches a linked-server-qualified target. To trust that explicitly,
  use all four parts, e.g. `Reporting.Sandbox.dbo.Staging_*`.

The allow-list applies to INSERT/UPDATE/DELETE/MERGE/TRUNCATE/SELECT INTO/OUTPUT INTO/BULK INSERT targets, not to
DDL (CREATE/ALTER/DROP TABLE stays red).

### Environment rules

One rule per line, with five required fields and two optional lists:

```text
server pattern | database pattern | environment label | Off/Red/YellowRed | typed true/false | allow-list | trusted procedures
```

`*` and `?` in server/database patterns are case-insensitive; blank database means any **known** database.
The **first matching rule wins**. Blank lines and `--` comments are ignored; bad lines are ignored with a settings
warning in analysis and the health indicator, rather than crashing. No match, or unknown server **or database**,
uses global settings. Unknown is never inferred to mean Off or Development.

```text
PROD-SQL* | | Production | YellowRed | true | dbo.Staging_* | dbo.sp_WhoIsActive;master.dbo.sp_Blitz*
TEST-* | Sandbox* | Test | Red | false | dbo.Test_* |
DEV-* | | Development | Off | false
```

`Off` skips execution checks, `Red` prompts only on real writes, `YellowRed` prompts on yellow and red.
Rule allow-lists and trusted procedures **add to** the global lists; they do not replace them. Use narrow global
lists if production rules must be restrictive. List entries within a rule are semicolon-separated. The text
format has no escaping for field/list delimiters (`|` and `;`).

Typed confirmation applies to red UPDATE/DELETE without WHERE, TRUNCATE and DROP TABLE/PROCEDURE. An UPDATE/DELETE
whose rows a `TOP (n)` or an INNER JOIN / CROSS APPLY on the target visibly limits (the usual
`UPDATE t ... FROM dbo.T t JOIN #ids ...`) is still red but needs no typing. Unproven limits keep typing: a CTE or
derived table, `TOP ... PERCENT`, a FROM with only outer/cross joins, or a target missing from the FROM. It does not require typing for temp-only or allow-listed changes.
The environment policy is selected from the **initial connection only**: `USE`, `USE FEDERATION` and `:connect`
add a TARGET SWITCH warning and dialog notice, and invalidate contextual allow/trusted lists for the analysis;
rules are not re-selected mid-script. A Development/Off rule can therefore leave a script switching to
production unchecked. Do not mix environments in one script; this is not server-aware enforcement.

### Trusted procedures

Entries such as `dbo.sp_WhoIsActive` or `master.dbo.sp_Blitz*` assert that a procedure is read-only; the guard
does not inspect its body. Successful matches get green **trusted procedure** findings. Entries require 2–4
parts and wildcards are allowed **only in the final procedure-name part**. Database/server qualification follows
the same rules as the table allow-list.

`dbo.sp_WhoIsActive` matches `EXEC dbo.sp_WhoIsActive`, but matches `EXEC sp_WhoIsActive` only when the analyzer's
caller supplies a known `dbo` default schema. In the VSIX, use the schema-qualified call. This avoids silently
trusting a same-named procedure in another schema. Built-in system procedure recognition is separate: only
unqualified, sys/dbo, or master.sys/master.dbo names (plus explicitly recognized msdb system procedures) use the
built-in lists. An unrelated `OtherDb.custom.sp_executesql` is an ordinary unknown procedure, not dynamic SQL.
`sp_spaceused` with true or unknown `@updateusage` is red; its normal read-only form remains green.

## Debug

1. Build **Debug** and install it: `.\install.ps1 -VsixPath .\src\SqlSafetyGuard.Vsix\bin\Debug\SqlSafetyGuard.vsix`.
   The VSIX includes the PDBs.
2. Either press **F5** in Visual Studio (the VSIX project's start action launches
   `C:\Program Files\Microsoft SQL Server Management Studio 22\Release\Common7\IDE\SSMS.exe` under the debugger), or
   start SSMS yourself and use **Debug > Attach to Process > `Ssms.exe`** (Managed (.NET Framework) code).
3. Breakpoints in `ExecuteGuard.OnBeforeExecute` and `AnalyzeQueryCommand.Execute` are good starting points.
   `SSMS.exe -log <file>.xml` writes an activity log that shows whether `SqlSafetyGuardPackage` loaded.
4. To test the fail-open path, start SSMS (Debug build installed) with the environment variable
   `SQLSAFETYGUARD_FAULT_INJECTION=1`. The guard then throws on every execute; the query must still run, and the
   Output pane logs `ERROR SQL Safety Guard failed…` and the toolbar shows degraded health. A non-modal InfoBar is
   attempted once per session. The switch is compiled out of Release builds.

Visual Studio's own "deploy to experimental instance" is turned off (`DeployExtension=false`): that would target
Visual Studio, not SSMS.

## Uninstall

Close SSMS, then:

```powershell
.\uninstall.ps1
```

This runs `VSIXInstaller /uninstall:SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558` for the SSMS instance and
removes any `-Copy` install folder. Alternatively, use **Extensions > Manage Extensions** in SSMS.

## How it works

### SSMS 22 target (checked on this machine)

- SSMS 22.11.2 is built on the **Visual Studio 18.11 shell** (VS 2026; `Microsoft.VisualStudio.Shell.15.0.dll`
  18.11), 64-bit, running on .NET Framework 4.7.2+. The extension targets **net472**, compiles against
  `Microsoft.VisualStudio.SDK` 17.14 (the shell's binding redirects cover it), and is packaged with
  `Microsoft.VSSDK.BuildTools` 17.14.
- Manifest target: `<InstallationTarget Id="Microsoft.VisualStudio.Ssms" Version="[22.0,)">` with
  `ProductArchitecture amd64`. This is the same Id/range Microsoft's own SSMS 22 extensions (SchemaCompare,
  SqlFormatter, PresenterMode…) use in `<SSMS>\Common7\IDE\Extensions\Microsoft\*\extension.vsixmanifest`.
- SQL Editor command IDs come from SSMS's own command table (`SQLEditors.dll` → `Menus.ctmenu`, decompiled with
  `vsct.exe`). `Query.Execute` is `{52692960-56BC-4989-B5D3-94C47A513E8D}:1`, the SQL Editor toolbar is
  `{33f13ac3-80bb-4ecb-85bc-225435603a5e}:0x0001`, the "SQL Query Editor" key-binding scope is
  `{b5a506eb-11be-4782-9a18-21265c2ca0b4}`, and the Available Databases combo is `{52692960-…}:0x0004`.
  `Query.Execute` is the only execute command in that table.

### Analyzer

`SqlSafetyAnalyzer.Analyze(sql, options, cancellationToken)` parses with the **newest `TSqlNNNParser`** in the loaded ScriptDom
assembly, found by reflection (`TSql180Parser` with ScriptDom 180.x, so SQL Server 2025 syntax parses). It walks
statement containers (BEGIN/END, IF/ELSE, WHILE, TRY/CATCH, all GO batches) and classifies their contents. ELSE-IF
chains are walked iteratively. Parsing and walking run on a dedicated background thread with a 64 MiB reserved
stack, not the SSMS UI stack. Cancellation is checked during tokenization, parsing and traversal.

- Targets are resolved through **aliases** (`UPDATE o … FROM dbo.Orders o`), **CTEs** (recursively, CTE → CTE,
  with a cycle/depth guard and per-CTE memoization), and **derived tables**. Base targets are deduplicated. Alias
  and CTE resolution comes before the temp-name rule: a `[#t]` alias for `dbo.Orders` is still a real write.
- **Composable DML** in derived table sources is classified independently of its outer target. Inserting into
  `#audit` from `(DELETE dbo.Customers OUTPUT deleted.Id)` is red. Nested `OUTPUT … INTO` targets are classified
  separately too. `INSERT … EXEC` adds an Unknown finding.
- **Literal dynamic SQL** (`EXEC('…')`, `EXEC('a' + 'b')`, `sp_executesql N'…'`) is parsed recursively, up to 3
  levels deep. Its findings are reported at the EXEC's line with the note "inside dynamic SQL". Anything built
  at runtime is Unknown.
- Procedure/function/trigger/view bodies are **not** walked: creating them doesn't run them.
- **Side effects hidden in reads:**
  - `OPENQUERY`, `OPENROWSET` (query form) and `OPENDATASOURCE` used as a *source* are Unknown. They send text
    to another server, and that text can change data there. This applies in FROM, subqueries, CTEs, `DECLARE`,
    `SET`, cursors and IF/WHILE conditions.
  - `NEXT VALUE FOR` is a Real write: it advances the sequence, and `ROLLBACK` doesn't undo it.
  - Not flagged: `OPENROWSET(BULK …)`, which only reads a file; four-part-name reads (`srv.db.dbo.t`), which read
    a remote table; and a `DEFAULT (NEXT VALUE FOR …)` in `CREATE TABLE`, which doesn't run anything.
- **Syntax-error recovery is bounded and conservative.** Each `GO` batch is parsed separately. Findings recovered
  from a broken batch are marked non-executable until that batch's syntax is fixed:
  - The statement with the error is blanked (line breaks kept, so every line and column number stays right) and
    the batch is parsed again, up to 25 times or 3 seconds. Everything around the error is analyzed normally.
  - Those statements are labelled "won't run now: this batch has a syntax error at line N" and shown dimmed. They
    still count toward the overall colour, because they will run once the error is fixed. The banner says so, e.g.
    "4 real writes, 1 unknown, 3 syntax errors · 13 more in a batch that will fail". The F5 dialog lists them in a
    separate group.
  - A `CREATE PROCEDURE/FUNCTION/TRIGGER/VIEW` that isn't the first statement of its batch gets "add GO before it".
    Everything after it in that batch is treated as its body, which is what SQL Server would do.
  - What couldn't be parsed is listed as **NOT ANALYZED** (e.g. "line 86"). That text is scanned for write
    keywords (outside comments and strings), and each hit becomes a yellow "possible UPDATE" row.
  - Broken batches over 32 Ki characters skip repeated reparsing and use the whole-batch NOT ANALYZED/possible-write
    fallback. This avoids quadratic recovery on thousands of broken statements, at the cost of detailed target
    classification. Smaller batches that exhaust recovery use the same fallback.
- **UPDATE/DELETE without WHERE** gets "no WHERE: affects every row" and a bold "can affect every row" line in the
  F5 dialog. If an inner join or `TOP (n)` limits the rows instead, the note says so and the bold line is not shown.
  A CTE or derived table gets a bold line saying only it limits the rows. `TRUNCATE` gets "removes every row" and
  DROP gets "drops ...".
- **SQLCMD mode** (`:setvar`, `$(var)`, `:r`, `:connect`, `:on error`, `!!`). The guard reads whether
  **Query > SQLCMD Mode** is on for the window being executed or analyzed. It asks for the same command status SSMS
  uses to draw the menu check mark (`Query.SQLCMDMode`, `{52692960-…}:17`); nothing is sent to the server.
  - **Mode off:** SQLCMD lines are what the server would get: syntax errors, so their batches fail. They're
    reported that way, plus one **SQLCMD MODE OFF** row: "turn on Query > SQLCMD Mode, or the batches with SQLCMD
    lines fail". A `$(…)` inside an ordinary string is just text and gets no hint.
  - **Mode on, or the mode can't be read:**
    - The analyzer parses a temporary same-length copy of the script with the SQLCMD lines blanked and each
      `$(var)` replaced by a placeholder. Line and column numbers still match the editor, and the copy is
      discarded after the check.
    - **Every `$(…)` substitution adds Unknown**, even a value that looks like a safe name. The analyzer does not
      execute SQLCMD's expansion rules: values can inject statements, concatenate identifiers or escape strings.
      Ordered `:setvar` definitions supply display hints only; later definitions never rewrite earlier hints.
    - `:r` (another file, not read), `!!` (an operating-system command), and `:exit(query)` (which can execute a
      statement) are yellow. `:connect` adds a TARGET SWITCH warning, like `USE`.
  - Either way, nothing is executed and no file is opened. SSMS still sends your original text.

### F5 guard

The guard hooks `Query.Execute` via **`DTE.Events.CommandEvents[guid, id].BeforeExecute`** rather than
`IVsRegisterPriorityCommandTarget`:

- The event is filtered to this one command. A priority command target sees every command and every QueryStatus
  in the IDE, and a bug there can break unrelated commands.
- `CancelDefault` is a supported way to stop the command.
- It has worked for SSMS extensions since SSMS 17. It fires for every route into the command: F5, Ctrl+E, Alt+X,
  the Execute button and `DTE.ExecuteCommand`. All five were tested on SSMS 22.

**Loading early.** SSMS defers background auto-loaded packages until startup has finished, which took 10–30 s in
testing. A query window opened at startup, for example by double-clicking a `.sql` file, is usable within a second
or two, so F5 there would have run unguarded. `EnsureGuardLoadedListener`, a MEF text-view listener, loads the
package when the first editor window is created. Tested: Execute fired the moment a connected window appeared
after launch was caught on two consecutive starts. The package loaded about 0.9 s after the SQL editor package.

**Run anyway** does not re-issue the command. The modal dialog is shown while SSMS is still inside the original
dispatch, so the guard returns `CancelDefault = false` and that same execution continues. Nothing can be
intercepted twice, and a re-entrancy flag guards the handler anyway. Tested: the UPDATE ran exactly once.

Safety rules:

- A failure **before classification fails open visibly**: the Output pane logs it, the guard becomes degraded,
  and a once-per-session InfoBar is attempted. After a red/yellow result is computed, tool-window, logging or
  wait-dialog cleanup failures cannot authorize execution. A failed confirmation dialog cancels that execution.
- The tool window and dialog use Visual Studio theme colours. The previous UI was checked in all 13 themes SSMS 22 lets you pick
  (`environment.visualExperience.colorTheme`): Light, Cool Breeze, Icy Mint, Sunny Day, Juicy Plum, Mango Paradise,
  Bubblegum, Silky Pink, Dark, Cool Slate, Moonlight Glow, Mystical Forest and Spicy Red. The Light/Dark (Extra
  Contrast) and High Contrast themes can't be selected through that setting; they follow the Windows contrast
  setting and weren't tested. The Cancel button has an accent border, so it reads as the default button in dark
  themes too.
- **All execution analysis runs off the UI thread**, behind the Visual Studio threaded wait dialog (shown after
  300 ms). Cancel propagates to the analyzer and cancels execution. If the VS wait-dialog service is unavailable,
  analysis still uses a background thread, but no cancellable wait UI can be shown.
- Resource budgets return an Unknown **too complex to analyze** finding instead of attempting unbounded parsing:
  256 nested parentheses or BEGIN/CASE blocks, 512 consecutive control-flow prefixes or ELSE IF links,
  20,000 significant tokens between top-level action keywords (a lexical approximation, not statement parsing),
  256 CTE-resolution depth, and a shared 40-million-work-unit / 10-second budget. These are conservative limits,
  not SQL Server syntax limits. A valid complex script can therefore be yellow.
- Scripts over **5,242,880 characters** (about 10 MiB of UTF-16 text) return a yellow **too large to analyze (N MB)**
  finding. They prompt in the default YellowRed mode; explicit Red-only and Off modes still apply. The old
  100 Ki-character threshold only controls a tool-window loading placeholder, not UI-thread analysis.

### Server/database line (undocumented)

- Server comes from `Microsoft.SqlServer.Management.UI.VSIntegration.ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo.ServerName`,
  read by reflection. In SSMS 22 that type lives in `SqlPackageBase.dll`.
- Database is the current value of the SQL Editor's **Available Databases** combo, read through the standard VS
  combo protocol (`Exec` with no input and an output VARIANT), so it reflects `USE` statements that already ran.
  If the combo can't be read, the connection's initial database is used.
- Unknown fields are shown explicitly, and unknown connection context uses global settings, never an inferred
  Development/Off rule. The connection-reading mechanism was previously verified on SSMS 22.11.2 with `.\SQLEXPRESS`;
  the new policy and health UI still need runtime validation there.

## Limitations

- **Not a security boundary.** Triggers fired by a write, the insides of procedures and functions, and dynamic
  SQL built at runtime can't be seen. Scalar functions called in a SELECT are treated as reads; a CLR or
  extended function could still have side effects. A green verdict means "nothing visible writes", not
  "nothing can write".
  Use a read-only login (`db_datareader` + `db_denydatawriter`) for real protection on production.
- **SQLCMD:**
  - If the SQLCMD Mode state can't be read (for example, a future SSMS changes the command), SQLCMD lines are
    interpreted as if the mode were on, which errs toward showing writes.
  - Variable substitution is not expanded for semantic classification. Every substitution is at least yellow,
    including inside strings and values supplied by SSMS options. Original editor positions are preserved.
  - Files included with `:r` are not read.
- **Characters the T-SQL tokenizer rejects** (such as a stray `\`) are skipped, and analysis resumes after them, so
  they can't hide a later `GO` or write.
- **Recovery around syntax errors is a heuristic.** It splits statements by keywords, so an unusual statement can
  make it blank more text than necessary. Anything blanked is listed as NOT ANALYZED and scanned for write
  keywords. Large broken batches deliberately use this coarse fallback instead of recovering every statement.
- Only text run through `Query.Execute` from a `.sql`/SQL editor is guarded. Not guarded: MDX/DMX/XMLA editors,
  Object Explorer actions (Edit Top 200 Rows, scripting wizards, dialogs), Copilot-run queries, and other
  extensions.
- Writes through a CTE or derived table that joins several tables list *every* base table (conservative).
- Views and synonyms are classified by name (a write to a view is red; the view's base tables aren't resolved).
- `KILL`, `SHUTDOWN`, `RECONFIGURE`, `RESTORE`, Service Broker statements (`SEND`, `RECEIVE`, …) and any other
  statement type not listed above are red (conservative). BACKUP, UPDATE STATISTICS, CHECKPOINT and unrecognized
  DBCC commands are yellow; DBCC is otherwise judged per command (read-only green, changing red).
  [docs/statement-coverage.md](docs/statement-coverage.md) records the reviewed ScriptDom statement inventory and
  important semantic/resource limits; an inventory is not a proof of soundness. The
  [systematic verification report](docs/verification/2026-10-27-all-kinds/README.md) separates all 334 static
  checks from 267 live-passed kinds, three failed-only kinds, and 64 not live tested.
- **Transactions are tracked by reading the text, not the session.** A `COMMIT` with no `BEGIN TRAN` in the text is
  yellow, because it may commit work an earlier run left open. A transaction still open at the end gets an
  OPEN TRANSACTION row. The guard can't see whether your session actually has an open transaction right now.
  Parsed `@@TRANCOUNT`/`XACT_STATE()` predicates are distinguished from strings/comments. Unreachable branches,
  zero-iteration loops and statements after RETURN cannot close a possibly open transaction. GO repetition and
  uncertain control transfers are conservative. With implicit transactions on, explicit BEGIN can add two levels;
  a tableless `SELECT 1` does not open one. This is not a full control-flow or SQL Server session simulator.
- Box (column) selections are analyzed as the selected text; the reported line numbers assume a normal selection.
- Allow/trusted lists require known database/default-schema context when qualification is omitted. The VSIX does
  not discover the login's default schema. Target switches disable contextual trust for the entire analysis, but
  do not re-select environment mode mid-script. Trust entries are user assertions, not verified procedure bodies.
- Microsoft does not officially support third-party SSMS extensions. A future SSMS update could change the command
  IDs or internals used here: the toolbar group, the database combo, and `ServiceCache`. The guard fails open if
  that happens.
