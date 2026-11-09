# User guide

SQL Safety Guard checks your query **before** SQL Server Management Studio (SSMS) 22 runs it and tells you whether it
changes data. It never runs anything itself and never sends your queries anywhere.

Not installed yet? See the [install guide](install.md). To change what is checked, see [settings](settings.md).

## The colours

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | 🟢 | SELECT, variable/session/control statements, balanced transactions, read-only or trusted procedures |
| Temp only | 🟢 | Writes only to temporary tables/procedures or table variables; permanent tempdb objects require opt-in |
| Allow-listed | 🟢 | Writes only to tables matching your allow-list |
| Unknown | 🟡 | Can't see what runs: EXEC of procedures, runtime dynamic SQL, `INSERT ... EXEC`, DBCC/BACKUP/…, parse errors |
| Real write | 🔴 | Any change to a permanent object (DML, DDL, security, remote writes) |

A query takes the colour of its most serious statement. [Statement verdicts](statement-verdicts.md) lists the colour
of every T-SQL statement kind, with examples.

## The safety pane

The pane sits on the right of every SQL query window. It analyzes exactly what F5 would run: the selection if there is
one, otherwise the whole window. It re-analyzes in the background about a quarter of a second after you stop typing or
change the selection; nothing is executed and SSMS does not freeze.

### Header

The header shows red and yellow circles with the number of affected objects, or a green check when there are none
(grey before the first result). Next to it, **Whole window** or **Selection, lines N–N** says what was analyzed. Hover
for the analysis summary and what F5 will do. The header never shows verdict words or the server name.

When an [environment rule](settings.md#environment-rules) matches the connection, a small label tag and a thin line
appear at the top: red for labels containing **PROD**, slate grey for all others. Unknown connections and unmatched
rules have no tag.

### ☰ Analysis

Objects are grouped into three sections, whose counts match the header circles:

- **Changes to permanent objects** (red)
- **Can't see inside** (yellow)
- **Allowed by your settings** (safe-table patterns and trusted procedures)

Each row is one object and shows its schema/name, statement type and first line number; the database is in the tooltip.
Repeated statements show **×N**, additional statement types **+N**, and transaction statements without a target show
**Transaction**. Empty sections are hidden.

**No permanent changes detected · N** at the bottom lists the green objects. It opens automatically when nothing is red
or yellow; its arrow points up when collapsed and down when expanded.

### Choosing a row

- **Click** a row to show a short explanation; groups with several findings also list their statement types and lines.
  Only one row is open at a time, and it stays open across reanalysis while it is selected.
- Choosing a row highlights only that group's statements in the editor and scrolls to the first one. Nothing is tinted
  automatically, and moving the editor cursor never highlights anything.
- **Double-click** or **Enter** jumps to the statement. **Space** toggles the details, **Right/Left** expand and
  collapse, and the arrow keys move across both lists.
- Clicking in the editor, typing, changing the selection, clicking blank pane space or collapsing the pane clears the
  highlight. Highlighting never selects SQL text or changes what F5 runs.
- Only permanent whole-target destructive operations show red warning text in the details; temporary-object operations
  stay neutral.

### ⚙ Settings

A read-only view of this window's server, database, matched environment rule, F5 warning mode, safe tables, trusted
procedures and tempdb handling. Each pattern shows whether it comes from the global settings or a rule.
**Settings...** opens the options page.

### Resizing and collapsing

Drag the pane's left edge to resize it. **»** collapses it to a thin strip that still shows the red/yellow counts or a
green check; click the strip to expand it. Size and collapsed state apply to all query windows for the session.

## Analyze Query

**Tools > SQL Safety Guard > Analyze Query**, the toolbar button (a SQL icon with a check mark, right after the Execute
group on the SQL Editor toolbar), or **Ctrl+Shift+F5** in a query window expands the pane if it is collapsed and
re-analyzes immediately.

To change the shortcut, open **Tools > Options > Environment > Keyboard** and search for the command
`SqlSafetyGuard.AnalyzeQuerySafety`.

## The F5 confirmation

F5, Ctrl+E, Alt+X and the Execute button all go through the same check. With the default settings, a yellow or red query
pauses before it runs:

- **CHANGES TO PERMANENT OBJECTS** and **CAN'T SEE INSIDE** each show a count and a collapsed list. Expanding a list
  shows each object (schema/name), statement and editor line. Long lists scroll; the buttons stay visible.
- Batches with syntax errors are dimmed and explained separately. Green findings and empty categories are hidden.
- **Cancel** is the default (Enter or Esc). **Run anyway** continues this execution only.
- No environment or operation ever requires typing a database name.

If SQL Safety Guard cannot confirm which query window SSMS is about to execute, it always asks, even in Off or Red-only
mode.

## When the check itself fails

If something goes wrong before a verdict is reached, a small box asks **Run anyway** or **Cancel**. Cancel is the default
and Escape cancels; if the box itself fails, execution is cancelled. Only the Off mode with a verified execution target
skips this question. Environment rules change the warning mode, not this failure handling.

The guard's state is shown in the toolbar caption. A failure marks it degraded, is logged, and shows a warning bar once
per session; the next successful check clears it. Messages and errors go to the **SQL Safety Guard** pane of the Output
window. Query text is never logged or stored.

## Try it

The `samples` folder of the source repository is a playground:

1. Open `samples\00-setup-sandbox.sql` in SSMS, connected to a local or development server, and press F5. The guard stops
   it (it creates a database), which is the first demo. Click **Run anyway**. This creates the throwaway database
   `SafetyGuardSandbox`; run the script again any time to reset it.
2. Open `samples\01-try-these.sql`. It has 29 numbered examples (green, yellow, red, settings, multi-batch, temp tables
   and reads with hidden side effects), each with its expected verdict. Select one and the pane shows its verdict;
   press **F5** to see the guard.
3. Open `samples\02-broken-batches.sql` and look at the pane with nothing selected, to see how syntax errors, a misplaced
   `CREATE PROCEDURE` and SQLCMD-mode lines are reported. Analyze it only; don't run it.
4. Drop the sandbox when you're done: `DROP DATABASE SafetyGuardSandbox;`

## Settings

Open **Tools > Options > SQL Safety Guard**. You can choose when F5 asks, list tables you routinely change, trust
specific procedures, set different rules per server, and import or export settings. Everything applies immediately.
See [settings](settings.md) for every option, with copyable examples.

## If something is wrong

- **The pane does not appear:** see [if the safety pane does not appear](install.md#if-the-safety-pane-does-not-appear).
- **Too many yellow warnings for your normal work:** add the tables you routinely change to the
  [allow-list](settings.md#safe-tables-allow-list), or use an [environment rule](settings.md#environment-rules) to warn
  less on development servers. Do not turn the guard off on production servers.
- **A query was green but changed data:** that is the most serious kind of bug. Please report it with the query text.

## Limitations

### It is a safety net, not a security boundary

- Triggers fired by a write, the insides of procedures and functions, and dynamic SQL built at runtime can't be seen.
  Scalar functions called in a SELECT are treated as reads; a CLR or extended function could still have side effects.
  A green verdict means "nothing visible writes", not "nothing can write".
- Use a read-only login (`db_datareader` + `db_denydatawriter`) for real protection on production.
- Trusted procedures and allow-list entries are your assertions; their bodies and effects are not verified.

### What is guarded

- Only text run through SSMS's Execute command from a `.sql`/SQL query editor is guarded. Not guarded: MDX/DMX/XMLA
  editors, Object Explorer actions (Edit Top 200 Rows, scripting wizards, dialogs), Copilot-run queries and other
  extensions.
- Microsoft does not officially support third-party SSMS extensions. A future SSMS update could change command IDs or
  internals. If the guard cannot attach to the Execute command at all, execution is unguarded and runs without a
  question. An attached guard that fails during a check asks Run anyway or Cancel, with Cancel as the default.

### Connections and names

- The server and database come from the window's initial connection. `USE` and `:connect` switch the target mid-script;
  they add a warning and disable the contextual allow/trusted lists, but do not re-select the environment rule.
- Allow-lists and trusted procedures need a known database and schema when a query omits them. The extension does not
  discover the login's default schema, so qualify names (`dbo.Table`) when you rely on schema-qualified patterns.
- Views and synonyms are judged by name: a write to a view is red; the view's base tables aren't resolved.
- Writes through a CTE or derived table that joins several tables list *every* base table (conservative).

### Transactions

Transactions are tracked by reading the text, not your session. A `COMMIT` with no `BEGIN TRAN` in the text is yellow,
because it may commit work an earlier run left open. A transaction still open at the end gets an OPEN TRANSACTION row.
The guard can't see whether your session has an open transaction right now.

Parsed `@@TRANCOUNT`/`XACT_STATE()` checks are told apart from strings and comments. Unreachable branches, zero-iteration
loops and statements after RETURN cannot close a possibly open transaction. GO repetition and uncertain control
transfers are handled conservatively. With implicit transactions on, an explicit BEGIN can add two levels; a tableless
`SELECT 1` does not open one. This is not a full control-flow or SQL Server session simulator.

### SQLCMD mode

- If the SQLCMD Mode state can't be read (for example, a future SSMS changes the command), SQLCMD lines are interpreted
  as if the mode were on, which errs toward showing writes.
- Variables (`$(name)`) are not expanded. Every substitution is at least yellow, including inside strings and values
  supplied by SSMS options. Original editor positions are preserved.
- Files included with `:r` are not read.

### Syntax errors and unusual text

- Characters the T-SQL tokenizer rejects (such as a stray `\`) are skipped and analysis resumes after them, so they can't
  hide a later `GO` or write.
- Recovery around syntax errors is a heuristic. It splits statements by keywords, so an unusual statement can blank more
  text than necessary. Anything blanked is listed as NOT ANALYZED and scanned for write keywords. Large broken batches
  deliberately use this coarse fallback instead of recovering every statement.
- Box (column) selections are analyzed as the selected text; the reported line numbers assume a normal selection.

### Conservative verdicts

`KILL`, `SHUTDOWN`, `RECONFIGURE`, `RESTORE`, Service Broker statements (`SEND`, `RECEIVE`, …) and any statement type
the analyzer doesn't recognize are red. BACKUP, UPDATE STATISTICS, CHECKPOINT and recognized DBCC commands with unknown
effects (for example `DBCC MEMORYMAP`) are yellow. Unrecognized DBCC names (for example `DBCC SSG_UNKNOWN`) parse as DLL
unloads and are red; other DBCC commands are judged per command (read-only green, changing red).

The [statement verdicts](statement-verdicts.md) list every statement kind. The
[verification status](verification/status.md) records what has been tested against a real server and the open gaps.
