# Settings

Open **Tools > Options > SQL Safety Guard**, or **Tools > SQL Safety Guard > Settings...**, or **Settings...** in the
safety pane's ⚙ view.

Settings appear in SSMS's integrated Options tab and **apply immediately**; there is no Save or Apply button. SSMS labels
the custom area of this page **Preview**, but the controls in it are live. **Import Settings...**, **Export Settings...**
and **Uninstall SQL Safety Guard...** sit below the settings.

The safe tables, trusted procedures and environment rules show only how many entries they have, plus a warning when an
entry has a problem (such entries are ignored). Their **Edit list...** / **Edit rules...** buttons open an editor:

- The entries are listed on the left; the selected entry's fields are on the right. **+ Add**, **↑**, **↓** and
  **Remove** manage the list. Each entry can have a **Note**, stored as a `--` comment line above it.
- Entries with a problem are marked in the list, and **Save** refuses to store them until they are fixed or removed.
  Nothing is stored until you choose **Save**; **Cancel** or Escape discards the changes.
- **Edit as text...** shows the same entries in the plain-text format described below, for pasting or bulk edits.
  Its Save returns to the list.

Settings files from export, and settings saved by earlier versions, use the same text format, so they open in these
editors unchanged.

## Overview

| Setting | Default | What it does |
|---|---|---|
| [F5 warnings](#f5-warnings) | Warn on yellow and red | When executing asks for confirmation: Off / Warn on red only / Warn on yellow and red |
| Show connection in confirmation | On | Shows the server and database in the confirmation. Unknown values always stay visible. |
| [Safe tables allow-list](#safe-tables-allow-list) | empty | Table patterns whose data writes count as green |
| [Trusted procedures](#trusted-procedures) | empty | Procedures you assert are safe; calls to them count as green |
| [Environment rules](#environment-rules) | empty | Per server/database warning mode and extra lists; first matching rule wins |
| Permanent tempdb objects are temporary | Off | Explicit opt-in to treat permanent `tempdb` tables as temporary. `#temp` and `##temp` are green regardless. |

The pane's **⚙ Settings** view shows the effective settings for the current window, including which global setting or
environment rule each pattern comes from.

## F5 warnings

- **Off**: execution is not checked (when the execution target is verified).
- **Warn on red only**: asks before real writes.
- **Warn on yellow and red** (default): also asks when the effect cannot be seen (procedures, dynamic SQL, parse errors).

Read-only, temp-only and allow-listed queries run without a question. In every mode, SQL Safety Guard asks when it
cannot confirm which window SSMS will execute. If the check itself fails, it asks **Run anyway** or **Cancel**; only
Off with a verified execution target skips that question. Confirmation never requires typing.

## Safe tables allow-list

The editor has four fields per pattern: **Server**, **Database**, **Schema** and **Table**. Leave Server and Database
empty for the usual case, and Schema empty to match any schema. **What this matches** describes the pattern in words,
and **Try a name** tests a table name, such as `dbo.Staging_Orders`, with the same matching the guard uses.

In **Edit as text...** (plain text, not SQL or JSON), use one pattern per line, for example:

```text
-- One exact table
dbo.ImportBuffer
-- Tables starting with Staging_
dbo.Staging_*
-- Tables in a specific database
Sandbox.dbo.Test_*
```

The same entries can be written `dbo.ImportBuffer;dbo.Staging_*;Sandbox.dbo.Test_*`.
These are example names, not targets the extension has verified as harmless. Matching writes and their side effects
are explicitly authorized by you. For `dbo.Staging_*`, qualify the query too: `UPDATE dbo.Staging_Orders SET ...`.

The allow-list applies to INSERT/UPDATE/DELETE/MERGE/TRUNCATE/SELECT INTO/OUTPUT INTO/BULK INSERT targets, not to
DDL (CREATE/ALTER/DROP TABLE stays red).

### How patterns match

Patterns are case-insensitive, support `*` and `?`, and match **entire identifier parts**, not substrings.
Brackets/double quotes delimit parts; escaped delimiters and quoted whitespace are preserved. One entry per
line (semicolon-separated entries also work); `--` and `#` lines are comments. Wildcard-only entries such as
`*` or `*.*` are ignored with a visible settings warning.

- `Staging_*` matches the object name in any schema, including a schema-less name.
- `dbo.Staging_*` requires a resolved `dbo` schema. **No implicit dbo assumption**: a schema-less query name
  matches only a one-part pattern unless the caller supplies a known default schema. The extension does not discover
  the login's default schema, so qualify your query names when relying on schema-qualified entries.
- A pattern with no database part matches unqualified names or names explicitly qualified with the **known
  current database**. It does not match another database, or an explicit database when the current one is unknown.
- `Sandbox.dbo.*` matches explicit Sandbox names, or unqualified database names when the known current database
  is Sandbox. It still requires a resolved schema.
- A pattern with no server part **never** matches a linked-server-qualified target. To trust that explicitly,
  use all four parts, e.g. `Reporting.Sandbox.dbo.Staging_*`.

## Trusted procedures

The editor has the same four fields, with **Procedure** instead of Table; Schema is required. **Try a name** also accepts
a call such as `EXEC dbo.usp_GetSalesReport @id = 1`. In **Edit as text...**, use one schema-qualified procedure pattern
per line (or semicolon-separated entries):

```text
dbo.usp_GetSalesReport
reporting.usp_Read_*
Sandbox.dbo.usp_GetTestResults
```

Use only procedures you explicitly trust: **their bodies are not checked**. Entries such as `dbo.sp_WhoIsActive` or
`master.dbo.sp_Blitz*` assert that a procedure is safe; successful matches get green **trusted procedure** findings.
The example `reporting.usp_Read_*` asserts trust; a name beginning with Read is not proof of read-only behavior.

- Entries need 2–4 parts, and wildcards are allowed **only in the final procedure-name part**. `usp_GetSalesReport`
  is invalid because it lacks a schema; `*.usp_GetSalesReport` is invalid because its schema contains a wildcard.
- Database/server qualification follows the same rules as the table allow-list.
- Call the procedure with its schema too, such as `EXEC dbo.usp_GetSalesReport;`. `dbo.sp_WhoIsActive` matches
  `EXEC dbo.sp_WhoIsActive`, but matches `EXEC sp_WhoIsActive` only when a known `dbo` default schema is supplied,
  which the extension does not do. This avoids silently trusting a same-named procedure in another schema.

Built-in system procedure recognition is separate: only unqualified, sys/dbo, or master.sys/master.dbo names (plus
explicitly recognized msdb system procedures) use the built-in lists. An unrelated `OtherDb.custom.sp_executesql` is an
ordinary unknown procedure, not dynamic SQL. `sp_spaceused` with true or unknown `@updateusage` is red; its normal
read-only form remains green.

## Environment rules

Environment rules choose a different warning mode, and add trusted tables and procedures, per server and database.
One rule per line, with five required fields and two optional lists:

```text
server pattern | database pattern | environment label | Off/Red/YellowRed | legacy true/false (ignored) | allow-list | trusted procedures
```

| Field | Required structure | Example |
|---|---|---|
| Server | Nonempty exact name or wildcard pattern | `PROD-SQL*` |
| Database | Exact name, wildcard pattern, or blank for any known database | `AppDb`, `Sandbox*`, or blank |
| Label | Nonempty display label | `Production` |
| Mode | `Off`, `Red`, or `YellowRed` | `YellowRed` |
| Legacy flag | `true` or `false`; accepted but ignored | Use `false` |
| Table allow-list | Optional patterns separated by `;` | `dbo.Test_*;dbo.Import_*` |
| Trusted procedures | Optional qualified patterns separated by `;` | `dbo.usp_Read_*` |

**Edit rules...** shows the rules in order with a form for the selected rule: Server, Database, Label, **When F5 asks**
(Off / Warn on red only / Warn on yellow and red), extra safe tables and extra trusted procedures (one per line) and a
Note. Use **↑** and **↓** to change which rule wins.

In **Edit as text...**, the first five fields must be present, even when the database field is blank. Use one complete
rule per line; semicolons separate list entries inside a rule, not rules. Example rules:

```text
-- Production: warn on yellow and red
PROD-SQL* | AppDb | Production | YellowRed | false
-- Test: red-only warnings and additional trusted targets
TEST-SQL* | Sandbox | Test | Red | false | dbo.Test_*;dbo.Import_* | dbo.usp_Read_*
-- Development: any known database, normal yellow/red warnings
DEV-SQL* | | Development | YellowRed | false
```

To supply only trusted procedures, keep the empty allow-list field:

```text
TEST-SQL* | Sandbox | Test | YellowRed | false | | dbo.usp_GetSalesReport
```

### How rules are applied

- `*` and `?` in server/database patterns are case-insensitive; a blank database means any **known** database.
- The **first matching rule wins**. Blank lines and `--` comments are ignored. Bad lines are ignored with a settings
  warning in the analysis and the guard state indicator, rather than crashing.
- No match, or an unknown server **or database**, uses the global settings. Unknown is never inferred to mean Off or
  Development.
- `Off` skips execution checks for a verified target, `Red` asks only on real writes, `YellowRed` asks on yellow and
  red. An ambiguous execution target overrides all three: confirmation is required and Cancel remains the default.
- Rule allow-lists and trusted procedures **add to** the global lists; they do not replace them. Keep the global lists
  narrow if production rules must be restrictive. The text format has no escaping for `|` and `;`.
- A matched rule shows its label in the pane header: labels containing **PROD** are red, others slate grey.

The environment is chosen from the **initial connection only**. `USE`, `USE FEDERATION` and `:connect` add a TARGET
SWITCH warning and dialog notice and disable the contextual allow/trusted lists for that analysis, but the rule is not
re-selected mid-script. A Development/Off rule can therefore leave a script that switches to production unchecked.
Do not mix environments in one script; this is not server-aware enforcement.

Confirmation never requires typing, for any operation or environment. The fifth field is kept for compatibility with
older settings: existing true/false values are accepted but ignored.

## Share settings with colleagues

- **Export Settings...** saves all your settings to one `.ssgsettings.json` file.
- **Import Settings...** opens such a file and shows exactly what would change before anything is applied; **Cancel**
  leaves your settings unchanged. Changes that make **more statements green** (new allow-listed tables, trusted
  procedures, a lower warning mode) are listed first and highlighted. Only import files from people you trust, and
  read those highlighted lines.
- A file that is damaged, or was written by a newer version, is rejected and nothing changes.
