# User guide

SQL Safety Guard checks your query **before** SQL Server Management Studio (SSMS) 22 runs it and tells you whether it
changes data. It never runs anything itself and never sends your queries anywhere.

Not installed yet? See the [install guide](install.md). To change what is checked, see [settings](settings.md).

## The colours

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | 🟢 | SELECT, variable/session/control statements, balanced transactions, recognized read-only procedures |
| Temp only | 🟢 | Writes only to temporary tables/procedures or table variables; permanent tempdb objects require opt-in |
| Allow-listed | 🟢 | Explicitly authorized target writes or typed routine calls and their reachable effects |
| Unknown | 🟡 | Can't see what runs: EXEC of procedures, runtime dynamic SQL, `INSERT ... EXEC`, DBCC/BACKUP/…, parse errors |
| Real write | 🔴 | Any change to a permanent object (DML, DDL, security, remote writes) |

A query takes the colour of its most serious statement. [Statement verdicts](statement-verdicts.md) lists the colour
of every T-SQL statement kind, with examples.

## The safety pane

The pane sits on the right of every SQL query window. It analyzes exactly what F5 would run: the selection if there is
one, otherwise the whole window. It re-analyzes in the background about a quarter of a second after you stop typing or
change the selection; nothing is executed and SSMS does not freeze.

### Header

The header shows **Whole window** or **Selection, lines N through N** to identify the analyzed text. Color counts live in the
six display controls below it; the header has no verdict badges. Hover a control for its name, operation count and color meaning.

When an [environment rule](settings.md#environment-rules) matches the connection, a small label tag and a thin line
appear at the top: red for labels containing **PROD**, slate grey for all others. Unknown connections and unmatched
rules have no tag.

### ☰ Analysis

Each object appears once, ordered by its most serious verdict and then source position. Its single-line row shows the
schema/name, worst overall verdict, counts of submitted red/yellow/green operations, and the first visible source line.
The left marker keeps the actual safety verdict; database-context operations use blue. On the right, red/yellow/green counts describe submitted operations; a purple circle and total describe operations inside deferred code, and a gray circle and total describe remote previews. For example, a trigger can show red 1 plus purple 2: one definition write, two operations inside. Expanding its preview shows a purple left circle and the actual red/yellow/green breakdown on the right. Individual operations retain their own risk markers. Hover for the execution context. Purple editor outlines describe the full code container, not another safety verdict.
Dynamic SQL executed now keeps its risk colours. Hover for
the full name and database. Whole-target warnings appear only in the expanded operation explanation, such as
**No WHERE clause: can affect every row**. Object and operation headers carry no repeated risk badge.

Six independent toggle buttons replace All: **Writes**, **Unknown**, **Safe**, **Inside** (purple), **Context** (blue) and **Remote** (gray). Click to enable or disable each; all enabled shows everything. Risk colors combine as a union; Inside and Remote enable their previews using the selected effect colors. Context independently shows `USE`, even with Safe off. Ancestors remain visible to reach matching descendants, keeping their actual risk markers. If nothing matches, the pane explains the empty state.

**Automatic** labels use text and counts when space allows, then circles and counts in a narrow pane. Right-click the filter row or Analysis button for Automatic / Text / Circles. Text mode requires a wider minimum pane; all modes keep one row, including large counts. Label changes preserve enabled filters. Hover and accessible names identify circles; inactive circles are outlined.

Risk-control counts and ordinary object counts exclude previews. Context counts submitted context changes separately from Safe. Inside and Remote count their preview operations, so the six totals should not be added. Preview rows show their own effect/context breakdown. All counts and overall warnings stay stable when filtering. Hover a toggle for its name, number and color meaning. Display filters never change execution analysis or the confirmation policy.

Line numbers have a shared, right-aligned gutter sized to the widest operation line in the current results, including collapsed children. The gutter uses spacing only and stays fixed while filtering or expanding, so right-side counts remain aligned. A new analysis can release unused width.

Objects start collapsed. Double-click one, or use its arrow, to see one compact row per analyzed operation, including
repeated UPDATE, DELETE and INSERT operations. Each operation retains its own verdict and source position. A SQL
statement can produce several operations, including several at the same line or inside dynamic SQL; none are discarded
to make the counts smaller. Double-click an operation for a short reason; the SQL stays in the editor.

The pane has one shared scrollbar. Scrolling over an expanded object or body moves through its inner operations,
even when that object is taller than the pane.

Operations without a known target remain separate from named objects. Ordinary reads are not currently attributed to
every referenced table, and an unknown dynamic SQL target is never guessed. Transaction operations show **Transaction**.

Open [the compact-pane sample](../../samples/04-compact-pane.sql) to inspect repeated writes and an object with both
red and yellow operations. No SQL connection or execution is needed.

### Procedure definitions in the editor

When the window contains a `CREATE`, `ALTER` or `CREATE OR ALTER PROCEDURE`, its definition keeps its normal verdict:
red for a permanent procedure, green for a temporary procedure. Submitting a definition does not run its body.

Double-click the procedure object, then its **CREATE/ALTER PROCEDURE** row to inspect the preview. Its compact **Inside** row has a purple circle on the left and the preview's actual risk counts on the right. It starts open when the definition is expanded. Identified targets appear as object groups; expand an object for its individual operations. Groups follow their first occurrence, and operations within each group follow source order. Statements without an identified target, such as PRINT and RETURN, remain direct rows. Single-click the preview to navigate and highlight its matching operations; double-click or use its arrow to collapse it. Remote previews use the same structure with a gray circle. There is only one global filter row, and it applies at every nested preview depth.
Single-click an inner object to navigate to its first matching operation and highlight that object's matching operations. Its actual risk circle and complete counts remain visible while filters hide individual operations. Groups stay within their own body or decoded EXEC; matching names in other scopes never merge. Grouping uses only targets already identified by the analyzer, not every table referenced by a SELECT. Remote-query expressions such as OPENQUERY stay as operation rows; their Remote preview groups its identified objects directly.
Single-click a body row to move the cursor and highlight that operation; double-click for one short reason.
A definition click tints its header with the actual verdict and outlines the complete parsed definition in purple,
from CREATE/ALTER through its ending, excluding GO and separate calls. Selecting an inner operation keeps that context
outline and tints the selected operation. A separate **EXEC** row keeps its own verdict: a visible definition
does not prove which stored body the server will execute. Matching expansion state survives passive reanalysis.
New objects, definitions and reasons start collapsed. Body findings do not enter submitted-operation risk counts or the F5 confirmation: those describe only what submitting the current selection/window would do. Purple Inside and gray Remote counts describe preview operations separately.

Only text present in this window is inspected. Calls to other unknown procedures and SQL strings built at runtime
remain yellow. **Body analysis incomplete** means parsing, a resource limit or an unavailable T-SQL body prevented
complete inspection; visible findings must not be treated as a complete list of effects.
Body allow-list and trust matches use written names; the connection's database and default schema are not assumed
to prove how unqualified names will resolve when the procedure is called.

Open `samples/03-procedure-body.sql` with nothing selected to try this without a SQL connection or executing anything.

### Other visible code bodies

Triggers, scalar functions, inline and multi-statement table-valued functions, and views use the same compact
CREATE/ALTER operation and locally grouped preview rows. Hover describes its context: firing for triggers,
calling for functions, and reading for views. Defining these objects remains a permanent write;
their preview effects do not enter execution totals. RETURN expressions are inspected, and writes to a function's
return-table variable remain temporary-only operations.

Unknown scalar, table-valued and object-method calls have separate yellow **FUNCTION** findings, including calls in
predicates and temporary-object statements. Recognized built-ins retain their existing behavior. A visible definition
does not prove the deployed implementation of a separate call; no server lookup or transitive dependency graph is made.
Some qualified built-ins and XML/spatial methods therefore receive conservative yellow warnings.

Visible query strings in `OPENQUERY`, query-form `OPENROWSET` and `EXEC (...) AT` open a gray **Remote** preview.
The outer remote operation stays yellow (or red for a remote write), regardless of its preview. This is a T-SQL
interpretation of visible text, not proof of the remote dialect, provider or runtime. Query sources and write targets
both support previews. A child click goes to its SQL string; **Sent at** returns to the containing operation.

`sp_add_jobstep` and `sp_update_jobstep` expose a purple **Inside** preview for the stored command for a proven named `@command` with
T-SQL subsystem. Adding a job step defaults to T-SQL; updating an existing step requires a proven `@subsystem` because
the old subsystem is unavailable. The outer operation remains a job-metadata write. **Stored at** returns to that call;
the command is not executed when stored. Positional parameters, runtime strings, SQL Agent tokens and other subsystems
show an unavailable body instead of an invented SQL interpretation. Local allow-list, procedure-trust and permanent-tempdb
opt-ins are not carried into remote or stored-command previews.

External/CLR implementations and unproven bodies show **Body unavailable** with a yellow diagnostic; broken or limited
previews are marked incomplete. Nested visible bodies reuse the same rows and navigation, with bounded nesting.
Open [the code-body sample](../../samples/05-other-code-bodies.sql) to inspect all 13 examples without executing SQL.
Open [the grouped-scope sample](../../samples/06-grouped-code-scopes.sql) for repeated permanent/temporary targets,
table variables and separate nested dynamic, remote and stored-command scopes. Inspection requires no connection.

### Dynamic SQL held in variables

The same pane also checks SQL stored in a variable when its full value can be proven from the executed text:

```sql
DECLARE @table nvarchar(128) = N'dbo.Orders';
DECLARE @sql nvarchar(max);
SET @sql = N'DELETE FROM ' + @table + N' WHERE Id = 1';
EXEC sys.sp_executesql @sql;
```

This produces a **Dynamic SQL** row for that EXEC, with a `dbo.Orders` object group containing the red DELETE operation.
The **Decoded SQL** label distinguishes these operations from deferred procedure-body previews.
A proven `SELECT` stays green; proven temporary-only work keeps its normal green verdict. Each EXEC has its own block,
even when several blocks affect the same table. Clicking the block moves to EXEC and highlights its visible operations
there, with neutral outlines around their contributing SQL strings. Clicking a decoded operation moves to its actual
string fragments and highlights them in its verdict colour, with a neutral outline at EXEC. Clicking an object inside
the block highlights that object's matching string fragments using the same source navigation. Expanded details include
**SQL source** and **Executed at** links. The declaration's own operation stays green: storing text does not execute it.
Copies, concatenation, reassignment and escaped quotes retain their proven source fragments. If a source cannot be
mapped, navigation falls back to EXEC. **Runtime SQL** stays yellow when its text is unavailable, with no invented body.

Supported strings use `varchar` or `nvarchar` declarations, `SET`, variable copies, simple `+` concatenation and `+=`.
Ordinary `BEGIN/END` blocks are supported. Branches, loops and TRY/CATCH, table/procedure-supplied values, unsupported
types or conversions, truncation and analysis limits remain yellow. Other statements between assignments and EXEC
also discard known values conservatively. Only the current batch and executed selection count: selecting just the
EXEC does not let the extension assume a value from an earlier execution or unselected declarations.

Open example 9 in `samples/01-try-these.sql` to inspect a variable-backed write without executing it.

### Choosing a row

- **Single-click** an object to move the cursor to its first visible operation and highlight its visible
  operations. In Writes view, this means only its red operations. Clicking leaves expansion unchanged.
- **Single-click** an operation to move the cursor to that operation and highlight its SQL. Adjacent highlighted rows
  share one outer outline; separate ranges stay separate. Supporting dynamic-SQL locations use neutral outlines.
- **Double-click** an object to expand/collapse its operations, or an operation to expand/collapse one short reason.
  A Definition operation opens its scoped body preview instead.
  Arrow buttons toggle the same details directly. Other open rows stay open.
- **Space/Enter** navigate using the focused row. **Right/Left** expand and collapse its details.
  Mouse navigation keeps pane focus so a second click can expand the same row.
- Highlights stay until a click elsewhere, editor interaction, an edit, selection change, filter change or pane collapse.
  Passive reanalysis preserves a matching highlight; a cleared highlight is not restored. Navigation preserves the
  existing SQL selection exactly, so choosing a row does not change the text F5 executes.
- Only permanent whole-target destructive operations show red warning text in the details; temporary-object operations
  stay neutral.

### ⚙ Settings

A read-only view of this window's server, database, matched environment rule, F5 warning mode, safe tables, trusted
procedures and tempdb handling. Each pattern shows whether it comes from the global settings or a rule.
**Settings...** opens the options page.

### Resizing and collapsing

Drag the pane's left edge to resize it. **»** collapses it to a thin strip with numbered circles for every color present:
red, yellow, green, purple, blue and gray, in the same order as the display controls. A larger gap separates the visible risk colors from the code/context colors when both groups are present. Zero-count colors are omitted.
These are the same unfiltered operation totals as the controls, including separate Inside and Remote preview totals;
clicking filters never lowers them. Green is a numbered circle, not a checkmark. Hover a circle for its category, count and meaning.
Click anywhere on the strip to expand it. During pending analysis or a placeholder, stale counts are removed and a neutral gray marker appears.
The expanded pane cannot be narrowed past the width needed to keep all six controls on one row; Automatic labels use circles when space is limited.
Size and collapsed state apply to all query windows for the session.

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

- Triggers fired by a write, the insides of called procedures and functions, and dynamic SQL whose complete text
  cannot be proven from supported local assignments can't be seen.
  Uninspected user-function and object-method calls receive yellow findings; visible definitions do not prove deployed code.
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

- Connection labels identify the initial server/database. `USE` appears blue under Context while retaining its ReadOnly analyzer verdict. Trust retains all possible databases if a switch fails; destination rules can escalate warnings. Literal dynamic SQL scopes its context. SQLCMD server switches remain unverified. See [settings](settings.md#context-changes-inside-a-script).

- Custom approvals require a full server.database.schema.object rule and an explicit schema in SQL. Exact server/database labels and identifier case must match; legacy short rules are retained but inactive.
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
[verification status](../status.md) records what has been tested against a real server and the open gaps.

The [dictionary](dictionary.md) explains the safety dots, blue context marker, purple code outline, and separate preview counts.
