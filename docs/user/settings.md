# Settings

**Colors and meanings** groups six individual descriptions into Effects and Code and context, with a separate expandable
SQL glossary. The legend, Enabled checkbox labels and F5 warning dropdown follow the SSMS theme, including the dropdown popup.

Open **Tools > Options > SQL Safety Guard**, or **Tools > SQL Safety Guard > Settings...**, or **Settings...** in the
safety pane's ⚙ view.

Settings appear in SSMS's integrated Options tab and **apply immediately**; there is no Save or Apply button. SSMS labels
the custom area of this page **Preview**, but the controls in it are live. **Import Settings...**, **Export Settings...**
and **Uninstall SQL Safety Guard...** sit below the settings.

The safe tables, trusted routines and environment rules show only how many entries they have, plus a warning when an
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
| [Trusted routines](#trusted-routines) | empty | Procedures you assert are safe; calls to them count as green |
| [Environment rules](#environment-rules) | empty | Per server/database warning mode and extra lists; first matching rule wins |
| Permanent tempdb objects are temporary | Off | Explicit opt-in to treat permanent `tempdb` tables as temporary. `#temp` and `##temp` are green regardless. |

The pane's **⚙ Settings** view shows the effective settings for the current window, including which global setting or
environment rule each pattern comes from.

## F5 warnings

- **Off**: no warning in the initial context when the execution target is verified. A stricter destination rule can still require confirmation.
- **Warn on red only**: asks before real writes.
- **Warn on yellow and red** (default): also asks when the effect cannot be seen (procedures, dynamic SQL, parse errors).

Read-only, temp-only and allow-listed queries run without a question. In every mode, SQL Safety Guard asks when it
cannot confirm which window SSMS will execute. If the check itself fails, it asks **Run anyway** or **Cancel**; only
Off with a verified execution target and no context rules skips that question. Confirmation never requires typing.

## Safe tables allow-list

The existing editor uses **Server**, **Database**, **Schema** and **Table**. All four parts are required for an active rule. Use the exact server label shown by the verified SSMS connection, and the exact identifier case. Wildcards `*` and `?` are allowed only in the object name. The guard does not query metadata or assume a default schema.

```text
-- Explicit authorization, including side effects
[DEV-SQL].Sandbox.dbo.ImportBuffer
[DEV-SQL].Sandbox.dbo.Staging_*
```

SQL can still use `UPDATE dbo.ImportBuffer ...` when the current server/database are known and every possible database context is approved. An explicit `UPDATE Sandbox.dbo.ImportBuffer ...` does not depend on the active database. SQL must include the schema. A known default schema alone does not prove that an object exists there rather than in a fallback schema.

A server part in **settings** binds the rule to the verified current connection label. A four-part name in **SQL** uses a linked-server alias, whose actual destination cannot be established from text alone. Such remote references do not receive custom trust. No equivalence between `.`, `(local)`, host names, instance names or aliases is guessed. Matching is case-sensitive to avoid silently broadening authorization on case-sensitive servers.

The allow-list applies to INSERT/UPDATE/DELETE/MERGE/TRUNCATE/SELECT INTO/OUTPUT INTO/BULK INSERT targets. CREATE/ALTER/DROP definitions remain red. Green here means **authorized**, not inherently harmless or read-only.

### Existing rules and migration

Older short rules remain visible and retain their original text, comments and order. They are **inactive**, with a qualification warning; upgrading never adds missing parts automatically. In the editor, **Use current connection for missing parts** explicitly fills known server/database labels. Review them, supply the schema, and save. Editing other entries can retain an untouched legacy rule in its inactive state. New or edited entries require full qualification.

**Try a name** uses the same qualified matcher and explains that a match depends on the indicated connection/database. Import/export retains the existing format and previews added authorization before applying it.

## Trusted routines

The same editor adds a **Procedure / Function** rule type. Text entries default to procedure; prefix a function rule with `FUNCTION `:

```text
[DEV-SQL].Sandbox.reporting.usp_GetReport
FUNCTION [DEV-SQL].Sandbox.reporting.GetTotal
```

Procedure approval never approves a function with the same name, or vice versa. A match produces an **AllowListed** finding: the call and its side effects are explicitly authorized. Server implementations are not retrieved. Approval of a call does not authorize CREATE/ALTER, and body previews do not inherit the connection's trust. Visible preview effects keep their own red/yellow/green verdicts.

Built-in system procedure recognition is separate: only unqualified, sys/dbo, or master.sys/master.dbo names (plus
explicitly recognized msdb system procedures) use the built-in lists. An unrelated `OtherDb.custom.sp_executesql` is an
ordinary unknown procedure, not dynamic SQL. `sp_spaceused` with true or unknown `@updateusage` is red; its normal
read-only form remains green.

## Environment rules

Environment rules choose a different warning mode, and add approved targets and routines, per server and database.
One rule per line, with five required fields and two optional lists:

```text
server pattern | database pattern | environment label | Off/Red/YellowRed | legacy true/false (ignored) | allow-list | trusted routines
```

| Field | Required structure | Example |
|---|---|---|
| Server | Nonempty exact name or wildcard pattern | `PROD-SQL*` |
| Database | Exact name, wildcard pattern, or blank for any known database | `AppDb`, `Sandbox*`, or blank |
| Label | Nonempty display label | `Production` |
| Mode | `Off`, `Red`, or `YellowRed` | `YellowRed` |
| Legacy flag | `true` or `false`; accepted but ignored | Use `false` |
| Table allow-list | Optional patterns separated by `;` | `DEV-SQL.Sandbox.dbo.Test_*` |
| Trusted routines | Optional qualified patterns separated by `;` | `DEV-SQL.Sandbox.dbo.usp_Read_*` |

**Edit rules...** shows the rules in order with a form for the selected rule: Server, Database, Label, **When F5 asks**
(Off / Warn on red only / Warn on yellow and red), extra safe tables and extra trusted routines (one per line) and a
Note. Use **↑** and **↓** to change which rule wins.

In **Edit as text...**, the first five fields must be present, even when the database field is blank. Use one complete
rule per line; semicolons separate list entries inside a rule, not rules. Example rules:

```text
-- Production: warn on yellow and red
PROD-SQL* | AppDb | Production | YellowRed | false
-- Test: red-only warnings and additional trusted targets
TEST-SQL | Sandbox | Test | Red | false | TEST-SQL.Sandbox.dbo.Test_* | TEST-SQL.Sandbox.dbo.usp_Read_*
-- Development: any known database, normal yellow/red warnings
DEV-SQL* | | Development | YellowRed | false
```

To supply only trusted routines, keep the empty allow-list field:

```text
TEST-SQL | Sandbox | Test | YellowRed | false | | TEST-SQL.Sandbox.dbo.usp_GetSalesReport
```

### How rules are applied

- `*` and `?` in server/database patterns are case-insensitive; a blank database means any **known** database.
- The **first matching rule wins**. Blank lines and `--` comments are ignored. Bad lines are ignored with a settings
  warning in the analysis and the guard state indicator, rather than crashing.
- No match, or an unknown server **or database**, uses the global settings. Unknown is never inferred to mean Off or
  Development.
- `Off` suppresses initial-context warnings for a verified target, `Red` asks only on real writes, `YellowRed` asks on yellow and
  red. An ambiguous execution target overrides all three: confirmation is required and Cancel remains the default.
- Rule allow-lists and trusted routines **add to** the global lists; they do not replace them. Keep the global lists
  narrow if production rules must be restrictive. The text format has no escaping for `|` and `;`.
- A matched rule shows its label in the pane header: labels containing **PROD** are red, others slate grey.

### Context changes inside a script

The pane presents `USE` as blue Context, independently of Safe. Its analyzer verdict remains ReadOnly; this display category does not suppress destination-policy warnings. A mixed decoded block can show blue context and green read counts together.

`USE` does not itself change permanent data. It also does not prove that the requested database exists or that switching will succeed. The analyzer conservatively retains both the previous and requested databases across `GO`. A short object name needs approval in **every possible context**. Loops and backward jumps consider visible switches before granting trust to earlier statements that may run again. An explicit database-qualified target can match independently.

Literal dynamic SQL has its own context scope: a `USE` inside it affects its inner analysis but does not leak into following outer statements. Module/remote previews remain separate from submitted execution.

Destination environment rules are evaluated for the possible database contexts and explicit database targets. A stricter destination can require confirmation even if the initial rule is Off. The header still identifies the initial connection; it is not a live server-state inspector. A SQLCMD `:connect` or federation switch keeps an unknown target warning and disables object trust; with environment rules configured, an unverified switch cannot inherit an Off bypass. Unknown runtime SQL and metadata/alias relationships remain unproven.

Confirmation never requires typing, for any operation or environment. The fifth field is kept for compatibility with
older settings: existing true/false values are accepted but ignored.

## Share settings with colleagues

- **Export Settings...** saves all your settings to one `.ssgsettings.json` file.
- **Import Settings...** opens such a file and shows exactly what would change before anything is applied; **Cancel**
  leaves your settings unchanged. Changes that make **more statements green** (new allow-listed tables, trusted
  procedures, a lower warning mode) are listed first and highlighted. Only import files from people you trust, and
  read those highlighted lines.
- A file that is damaged, or was written by a newer version, is rejected and nothing changes.
