# SQL Safety Guard — user guide

SQL Safety Guard checks your query **before** SQL Server Management Studio (SSMS) 22 runs it and tells you whether it changes
data. It never runs anything itself and never sends your queries anywhere.

## Install

Download `SqlSafetyGuard-<version>.vsix` from [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases).
If Windows blocks the download, right-click it > Properties > Unblock > OK. Close SSMS, then double-click the VSIX.
The install is per user and needs no administrator. If IT already installed an all-users copy, do not add a second copy.
To update, install a newer VSIX the same way.

For script installation, download and extract the scripts ZIP from the same release, then provide the package path:

```powershell
.\install.ps1 -VsixPath .\SqlSafetyGuard-1.0.0.vsix
```

From a source checkout, the script can also find the newest built package:

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

After installing or uninstalling, both scripts run `SSMS.exe /updateconfiguration` (hidden, about 10 seconds) so SSMS
rebuilds its editor component cache. The safety pane is an editor (MEF) component; without this step SSMS can keep a
stale cache after an upgrade and the pane silently doesn't appear (or SSMS reports a missing file). If the step fails,
the script prints the command to run by hand with SSMS closed.

Previous installer checks on SSMS 22.11.2 (VS shell 18.11):

- ✅ VSIXInstaller per-user install (`install.ps1`): loads, all features work.
- ✅ Double-clicking the `.vsix`: same per-user install.
- ✅ Manual copy into `<SSMS>\Common7\IDE\Extensions` (`install.ps1 -Copy`, elevated): on the next start SSMS
  regenerates its pkgdef cache, imports `SqlSafetyGuard.pkgdef`, and the package, command, shortcut and F5 guard all
  work. `uninstall.ps1` (elevated) removes it. Don't combine it with a per-user install of the same extension.
- ❌ Manual copy into `%LocalAppData%\Microsoft\SSMS\22.0_<id>\Extensions\<folder>` plus touching
  `extensions.configurationchanged`: **not picked up**. Per-user extensions must be registered by VSIXInstaller, so
  `install.ps1` doesn't offer this.

## The colours

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | 🟢 | SELECT, variable/session/control statements, balanced transactions, read-only/trusted procs |
| Temp only | 🟢 | Writes only to temporary tables/procedures or table variables; permanent tempdb objects require opt-in |
| Allow-listed | 🟢 | Writes only to tables matching your allow-list |
| Unknown | 🟡 | Can't see what runs: EXEC of procedures, runtime dynamic SQL, `INSERT ... EXEC`, DBCC/BACKUP/…, parse errors |
| Real write | 🔴 | Any change to a permanent object (DML, DDL, security, remote writes) |

## Use

- **Safety pane** (right of the editor in every SQL query window). It analyzes exactly what F5 would run: the
  selection if there is one, otherwise the whole window. It re-analyzes in the background about a quarter of a
  second after you stop typing or change the selection; nothing is executed and SSMS doesn't freeze.
  - The shared **34 px header** shows red/yellow object-count circles, or a green check when neither is present
    (grey before a result), followed by **Whole window** or **Selection, lines N–N**. The tooltip contains the
    analysis summary and what F5 will do. It never puts verdict words or the server name in the header.
  - A matched environment rule adds a small label tag and a 3 px top line: labels containing **PROD** are red,
    all others slate grey. Unknown connections and unmatched rules have no tag.
  - **☰ Analysis** groups one row per resolved object into **Changes to permanent objects**, **Can't see inside**, and
    **Allowed by your settings** (safe-table patterns and trusted procedures). Section counts match the red/yellow
    circles. Each collapsed row shows only the schema/object, statement type, and first line number. The database
    stays in the tooltip. Repeated statements use **×N**; additional statement types use **+N**. Collapsed actions
    have neutral text; section headings carry the safety category. Targetless transaction statements show **Transaction**.
  - **No permanent changes detected · N** is an expandable footer, opened automatically when nothing is red
    or yellow. Empty sections are omitted. **Click** a row to expand or collapse a short explanation; groups with
    multiple findings also show their statement types and lines. Only permanent whole-target destructive operations
    have red warning text in the details; temporary-object operations remain neutral. Only one row can be expanded;
    selecting another row closes the previous details. Clicking blank space in the pane or whitespace in the SQL
    editor clears the selected row and closes its details. Expansion survives reanalysis
    for the same object. **Space** toggles details, **Right/Left** expands/collapses, and **double-click** or **Enter**
    jumps to the statements. Selection highlights them; the editor caret follows the corresponding row without
    automatically expanding it. Arrow-key navigation works across both lists. Yellow/red statements retain light tinting.
  - **⚙ Settings** is a read-only view of this window's server, database, matched rule, F5 warning mode, button-only
    confirmation, safe tables, trusted procedures, and tempdb handling. Each effective pattern identifies its
    global or rule source. **Settings...** opens SQL Safety Guard's options page.
  - Drag the pane's left edge to resize it; **»** collapses it to the unchanged thin strip with red/yellow count
    circles or a green check (click the strip to expand). Size and collapsed state apply to all query windows for
    the session.
- **Tools > SQL Safety Guard > Analyze Query**, the toolbar button ("SQL with a check mark" icon right after the Execute group
  on the SQL Editor toolbar), or **Ctrl+Shift+F5** in a query window: expands the pane if it is collapsed and
  re-analyzes immediately.
- **F5 guard**: a yellow/red query pauses for confirmation. **CHANGES TO PERMANENT OBJECTS** and
  **CAN'T SEE INSIDE** each show a grouped-entry count and a dropdown, collapsed initially even for one entry.
  Expanding shows only object (schema/name), statement and editor line. Lists scroll independently above
  190 pixels; the buttons stay outside the scrolling content. Empty categories and green findings are hidden.
  Entries are passive: no statement navigation or extra detail rows. Syntax-error batches are dimmed and
  explained separately. **Cancel** is the default (Enter/Esc); **Run anyway** continues this execution.
  No environment or operation requires typing a database name. Existing explicit Off/Red policies still apply.
- **Guard state** stays in the toolbar caption even when the status bar is temporarily used by SSMS. A check
  failure marks it degraded and attempts a once-per-session InfoBar as well as logging. A successful guard check
  clears the degraded indicator. If no verdict was produced, a small failure box asks **Run anyway** or **Cancel**.
  Cancel is the default and Escape cancels; if the box fails, execution is cancelled. Only known Off settings with a
  verified execution target skip that question. Environment rules override the global warning mode, not this failure reporting.
- Messages and errors go to the **SQL Safety Guard** pane of the Output window. Query text is never logged or stored.

### Try it

The `samples` folder is a playground:

1. Open `samples\00-setup-sandbox.sql` in SSMS, connected to a local or dev server, and press F5. The guard stops
   it (it creates a database), which is the first demo. Click **Run anyway**. This creates the throwaway
   database `SafetyGuardSandbox`; run the script again any time to reset it.
2. Open `samples\01-try-these.sql`. It has 29 numbered examples (green, yellow, red, settings, multi-batch, temp
   tables, and reads with hidden side effects), each with its expected verdict. Select one and the pane shows its
   verdict; press **F5** to see the guard. `SamplesTests.cs` checks every `expect:` line,
   and `WholeFileSamplesTests.cs` checks that analyzing the whole file lists every example.
3. Open `samples\02-broken-batches.sql` and look at the pane with nothing selected, to see how syntax errors, a
   misplaced `CREATE PROCEDURE` and SQLCMD-mode lines are reported. Analyze it only; don't run it.
4. Drop the sandbox when you're done: `DROP DATABASE SafetyGuardSandbox;`

### Shortcut

The handoff asked for Ctrl+Shift+A, but in SSMS 22 that is already bound globally to `Project.AddNewItem`. The
extension uses **Ctrl+Shift+F5** (free in SSMS 22, scoped to the SQL Query Editor, and "check before F5" is easy to
remember). Ctrl+Alt combinations were avoided because on many European layouts Ctrl+Alt is AltGr and types
characters such as `\`. To change it: **Tools > Options > Environment > Keyboard**, command
`SqlSafetyGuard.AnalyzeQuerySafety`.

## Settings

**Tools > SQL Safety Guard > Settings...** or **Tools > Options > SQL Safety Guard**

Settings appear directly in SSMS's integrated Options tab and apply immediately. Existing settings are preserved.
**Import Settings...** and **Export Settings...** belong to this page. Import still validates the file and shows
a preview before applying anything; Cancel leaves the settings unchanged. The allow-list, trusted procedures and
environment rules have **Edit list...** / **Edit rules...** buttons for a larger multiline editor with Save and Cancel.

| Setting | Default | |
|---|---|---|
| F5 warnings | Warn on yellow and red | Off / Warn on red only / Warn on yellow and red |
| Show connection in confirmation | On | Unknown fields always remain visible and use global settings |
| Safe tables allow-list | empty | One pattern per line; `--` lines are comments |
| Permanent tempdb objects are temporary | Off | Explicit opt-in; `#temp` / `##temp` remain green regardless |
| Trusted procedures | empty | Schema-qualified patterns; wildcards only in the final part |
| Environment rules | empty | First matching rule overrides mode and environment |

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
server pattern | database pattern | environment label | Off/Red/YellowRed | legacy true/false (ignored) | allow-list | trusted procedures
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

`Off` skips verified-target execution checks, `Red` prompts only on real writes, `YellowRed` prompts on yellow and red.
An ambiguous execution target overrides all three modes: confirmation is required and Cancel remains the default.
Rule allow-lists and trusted procedures **add to** the global lists; they do not replace them. Use narrow global
lists if production rules must be restrictive. List entries within a rule are semicolon-separated. The text
format has no escaping for field/list delimiters (`|` and `;`).

Confirmation never requires typing, including whole-target writes, DROP, unknown databases, and all environments.
The fifth rule field is retained for compatibility: existing true/false values are accepted but ignored.
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

## Uninstall

Close SSMS, then:

```powershell
.\uninstall.ps1
```

This runs `VSIXInstaller /uninstall:SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558` for the SSMS instance and
removes any `-Copy` install folder. Alternatively, use **Extensions > Manage Extensions** in SSMS.

## Share settings with colleagues

- **Export:** **Export Settings...** on the SQL Safety Guard settings page saves all your settings to one `.ssgsettings.json` file.
- **Import:** **Import Settings...** on that page opens such a file and shows exactly what would change before anything is applied.
  Changes that make **more statements green** (new allow-listed tables, trusted procedures, a lower warning mode) are listed
  first and highlighted. Only import files from people you trust, and read those highlighted lines.
- A file that is damaged, or was written by a newer version, is rejected and nothing changes.

## If something is wrong

- **The pane does not appear:** close SSMS, reinstall the `.vsix`, start SSMS again. If it still does not appear, ask IT to run
  `SSMS.exe /updateconfiguration` with SSMS closed (it rebuilds SSMS's extension cache).
- **Too many yellow warnings for your normal work:** add the tables you routinely change to the allow-list, or use an environment
  rule to warn less on development servers. Do not turn the guard off on production servers.
- **A query was green but changed data:** that is the most serious kind of bug. Please report it with the query text.

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
  statement type not listed above are red (conservative). BACKUP, UPDATE STATISTICS, CHECKPOINT and recognized
  DBCC commands with unknown effects (for example, `DBCC MEMORYMAP`) are yellow. Unrecognized DBCC names
  (for example, `DBCC SSG_UNKNOWN`) parse as DLL unloads and are red; other DBCC commands are judged per command
  (read-only green, changing red).
  [docs/history/2026-10-27-statement-coverage.md](history/2026-10-27-statement-coverage.md) records the reviewed ScriptDom statement inventory and
  important semantic/resource limits; an inventory is not a proof of soundness. See also the
  [statement verdicts](statement-verdicts.md) for representative SQL examples and their colours. The
  [systematic verification report](history/2026-10-27-all-kinds/README.md) separates all 334 static
  checks from 267 live-passed kinds, three failed-only kinds, and 64 not live tested. The current verification state is always in
  [docs/verification/status.md](verification/status.md); [docs/README.md](README.md) lists every current document.
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
- Microsoft does not officially support third-party SSMS extensions. A future SSMS update could change command
  IDs or host internals. If the guard cannot attach to the execution command at all, execution is unguarded and
  can run without a question. An attached guard that fails during a check asks Run anyway or Cancel, with Cancel as the default.
