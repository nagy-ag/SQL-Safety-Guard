# Changelog

## 1.1.2 — 2026-11-17

- Group operations by their identified object within visible code bodies and decoded SQL blocks. Keep separate bodies and EXEC scopes separate, retain standalone statements without targets, and preserve existing colors, counts, filters, source navigation and persistent highlights.
- Start inner object groups collapsed; single-click navigates and highlights matching operations, and double-click reveals their statements. Retain expansion and selection through passive reanalysis.
- Add `samples/06-grouped-code-scopes.sql` to inspect repeated targets across procedures, functions, triggers, views, remote previews and stored job commands. SQL verdicts, F5 behavior, settings format and analysis limits are unchanged.

## 1.1.1 — 2026-11-17

- Fix scrolling through expanded objects taller than the safety pane, including procedure and function bodies and decoded dynamic SQL. The shared list scrolls by pixels so every inner operation remains reachable.
- Use the SSMS themed dropdown style for Procedure/Function rule types in the trusted-routine editor, including its open menu in dark mode.

## 1.1.0 — 2026-11-17

- Analyze supported variable-backed dynamic SQL built through straight-line DECLARE, SET, copies, concatenation and append operations. Runtime-dependent, truncated or otherwise unproved strings remain Unknown. Decoded rows have links to their string source and execution statement.
- Extend visible-body previews to triggers, scalar and table-valued functions, views, remote query strings and stored SQL Agent T-SQL commands. Preserve nested previews and incomplete-body warnings. Potential body effects remain separate from submitted operations; called implementations are not fetched.
- Add Unknown findings for unproved function calls, including calls inside predicates, RETURN expressions and dynamic SQL. Recognized safe built-ins retain their existing treatment.
- Require server.database.schema.object approval rules and separate procedure approvals from function approvals. Approved calls are AllowListed: authorization for their effects, not proof that their implementations are read-only. Definitions retain their actual write verdict.
- Present USE as blue Context while its analyzer verdict is ReadOnly. Keep prior possible databases after a switch, scope dynamic context changes, and apply stricter destination environment rules when confirmation is needed.
- Redesign the pane around compact object and individual-operation rows, six independent filters, automatic text/circle labels, a shared measured line-number gutter and semantic hover descriptions. The expanded header shows scope; the collapsed strip shows numbered circles for present categories, with spacing between risk and code/context colors.
- Keep highlights until another interaction clears them, join adjacent outlines, outline full definitions, and retain source positions and expansion state across reanalysis. Whole-target warnings appear in expanded operation details.
- Improve dark-theme settings controls and add the shared colors-and-meanings dictionary, code-body samples and qualified-trust examples.

**Upgrade note:** older short approval rules are retained but inactive until explicitly migrated to fully qualified identities. Missing server, database or schema parts are never guessed. Review the [migration instructions](docs/user/install.md#upgrading-to-110) before relying on previous approvals. Existing settings storage and import/export formats are retained, with stricter matching semantics.

Dynamic SQL depth remains three expansions and body-preview depth remains eight inspected levels. The extension remains text-only: no SQL execution, new connections or server metadata lookup is added.

## 1.0.5 — 2026-11-10

- Procedure definitions in the query editor now show **Body — potential effects when called**, with the usual safety sections and object rows nested inside the procedure. It covers `CREATE`, `ALTER` and `CREATE OR ALTER PROCEDURE`, including visible literal dynamic SQL, and keeps unknown or incomplete body analysis visible. The definition remains one row.
- Single-click an outer or nested object row to highlight all its statements; double-click to expand or collapse its details. Click an individual statement to highlight only it. Multiple rows can stay expanded across reanalysis, and highlighting preserves the SQL selection and what F5 executes.
- Body and safe-section chevrons point down when collapsed and up when expanded. Removed the body explanation paragraph to keep the pane compact.
- Added `samples/03-procedure-body.sql` for trying the pane without executing SQL, and updated the user and developer guides.
- Body findings are separate from the execution verdict and header counts: submitting a procedure definition does not run its body. F5 confirmation policy, settings formats and the text-only boundary are unchanged; bodies of called procedures are not fetched.

## 1.0.4 — 2026-11-09

- Fixed scrolling on Tools > Options > SQL Safety Guard. When the page was taller than the window, SSMS repeated the whole settings form as a sticky header over the page, which blocked the mouse wheel. The repeated copy is now hidden, so the page scrolls normally.
- Redesigned the list settings. The Options page now shows only how many safe tables, trusted procedures and environment rules you have, with a warning when one has a problem. **Edit list...** opens a list with a form for each entry: Server, Database, Schema and Table (or Procedure), a note, a plain-language "What this matches" and a **Try a name** check. **Edit rules...** opens the rules in order with a form for each rule. **Edit as text...** keeps copy and paste in the existing text format, and import, export and existing settings are unchanged.
- Dialog title bars follow the SSMS theme, so they are no longer white in dark themes.

## 1.0.3 — 2026-11-05

- Redesigned Tools > Options > SQL Safety Guard as a compact page: explanations on the left, aligned controls on the right, and Import, Export and Uninstall below the settings.
- Retained all six settings, their defaults, immediate persistence, list/rule editors and existing command dialogs. Analyzer verdicts and F5 confirmation policy are unchanged.
- Expanded the user guide with copyable table-pattern, trusted-procedure and environment-rule examples, field definitions and matching limits.

## 1.0.2 — 2026-11-05

- SQL statements are highlighted only when their finding group is chosen in the safety pane. Red and yellow statements no longer remain tinted while editing, and cursor movement no longer activates highlights.
- Returning to the editor, typing, changing the SQL selection, clicking empty pane space or collapsing the pane clears the highlight. Double-click and Enter still navigate to the statement without selecting SQL text.
- The arrow beside **No permanent changes detected** points **up when collapsed** and **down when expanded**. Its expand/collapse action is unchanged.
- Analyzer verdicts and F5 confirmation policy are unchanged.

## 1.0.1 — 2026-11-05

- Added **Uninstall SQL Safety Guard...** to the native settings page and corrected the Tools submenu registration.
- Added the full installation ZIP and company signing kit. GitHub release packages remain unsigned.

See [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases) for downloads and publication notes.
