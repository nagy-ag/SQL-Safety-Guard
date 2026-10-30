# Changelog

Every release lists what changed for users. **Colour changes** (a statement that is now judged differently) are always listed
in their own section, because they change what users see before F5. Versions follow `MAJOR.MINOR.PATCH`: patch = fixes with no
colour change, minor = colour changes or new features, major = changes users must relearn.

## [1.0.0] - unreleased

First official release for SQL Server Management Studio 22.

### What it does

- **Safety pane** beside every query editor: re-analyzes the selection (or the whole window) as you type and shows what would
  change, grouped by risk. It never executes anything.
- **F5 guard** for F5, Ctrl+E, Alt+X and the Execute button: analyzes the exact text SSMS is about to run. Green runs without
  interruption only when the window being executed is verified; yellow and red ask for confirmation with buttons (never typing).
- **Verdicts**: green = read-only, temp-only or allow-listed; yellow = the effect cannot be seen (procedures, dynamic SQL, parse
  errors); red = any change to a permanent object.
- **Settings** (Tools > SQL Safety Guard > Settings): warning mode, safe-table allow-list, trusted procedures,
  per-server/database environment rules.
- **Settings import/export** to share a configuration with colleagues; the import preview highlights anything that would make
  more statements green. Import and Export live on the integrated settings page, alongside multiline list/rule editors.
- **Compact commands**: Tools > SQL Safety Guard contains Analyze Query and Settings; the toolbar shows a short guard state.

- Dark-theme finding rows now use readable themed text; settings buttons and dialog defaults follow the SSMS theme.
- A guard failure before classification now asks Run anyway or Cancel instead of running; a failed box cancels.
- First installation accepts VSIXInstaller's not-installed exit code during the upgrade removal step.
- Deterministic Release builds use neutral source paths; the release script validates unsigned packages and writes checksums.
- Distributed under the MIT License, Copyright (c) 2026 NagyAG; bundled ScriptDom license notices are included.

### Verification

- 1,571 automated tests.
- Verdicts checked against a real SQL Server: 37,229 executed cases plus 890 offline checks with no real false green
  (see `docs/verification/status.md`).

### Known limits

- The analyzer reads only the query text; it cannot see inside procedures, triggers or cascades, so those are yellow or reported
  as such. It is a safety net, not a security boundary: use read-only logins for real protection.
- Some behaviours await a company policy decision (`docs/verification/policy-questions.md`); they may change colour in 1.1.0.
- GitHub release packages are always unsigned. The owner may sign a downloaded copy on their company computer.
