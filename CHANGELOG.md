# Changelog

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
