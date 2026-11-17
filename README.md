# SQL Safety Guard for SSMS 22

SQL Safety Guard is an extension for SQL Server Management Studio (SSMS) **22.x** that checks your T-SQL **before it runs**
and tells you whether it changes real data. A forgotten `WHERE`, a write in the wrong query window, or a write hidden
inside a procedure or dynamic SQL is caught before you press F5, not after.

It reads only the query text. It never executes SQL itself, opens no connections of its own, and never sends your
queries anywhere.

## Who it is for

People who work on databases where a mistake is expensive: production support, data fixes, reporting servers shared
with applications, or any environment where an accidental `UPDATE` without `WHERE` would hurt.

## How it works

- **Safety pane.** A narrow pane beside every query editor shows, as you type, what the query would change. It checks
  exactly what F5 would run: the selection if there is one, otherwise the whole window. Each object appears once with
  its worst verdict and operation counts, with blue context operations counted separately. Six compact toggles combine Writes/Unknown/Safe with Inside/Remote previews and an independent Context category; labels automatically fit the pane.
  Expand a definition's operation row for its purple **Inside** preview, separate from the effects of
  submitting the definition. Procedures, triggers, functions and views have typed visible-body previews;
  remote query strings use gray previews. Decoded dynamic SQL has one block per EXEC, with its operations directly below it.
  Single-click to navigate and highlight matching SQL until another interaction; double-click an
  object for its operations, or an operation for a short reason. Filters never change the overall warning or F5 scope.
- **F5 guard.** When you execute (F5, Ctrl+E, Alt+X or the Execute button), SQL Safety Guard analyzes the exact text
  SSMS is about to run. With the default settings, a yellow or red query pauses for a buttons-only confirmation:
  **Cancel** is the default, **Run anyway** continues. Nothing ever needs to be typed.
- **Analyze Query** (Tools > SQL Safety Guard, the toolbar, or **Ctrl+Shift+F5**) re-checks the current window on demand.

## The colours

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | 🟢 Green | Reads, variables, session settings, balanced transactions, known read-only procedures |
| Temp only | 🟢 Green | Writes only `#temp` tables, table variables or temporary procedures |
| Allow-listed | 🟢 Green | Explicitly authorized target writes or routine calls, including their side effects |
| Unknown | 🟡 Yellow | Cannot see what runs: procedures, dynamic SQL built at runtime, parse errors |
| Real write | 🔴 Red | Changes a permanent table, schema, permission or server setting |

Green runs without a question once the guard has confirmed which window SSMS will execute. By default, yellow and red
ask first; the warning mode can be changed globally or per server. See [which colour each statement gets](docs/user/statement-verdicts.md).

## Safety principles

- **Green needs proof.** If the analyzer cannot see what a query does, the answer is yellow, never green.
- **Fail closed.** A parse error, an analysis limit, an internal error or doubt about which window SSMS will execute
  leads to a question, not a silent run.
- **Spelling does not matter.** Brackets, missing schemas, `BEGIN/END`, `TRY/CATCH`, `IF`, loops, transactions,
  `EXEC(...)`, `sp_executesql`, CTEs, `OUTPUT INTO` and multiple `GO` batches do not hide a write.
- **A safety net, not a security boundary.** It previews visible procedure, trigger, function and view definitions, remote query strings
  and stored T-SQL job commands. It cannot retrieve deployed definitions, discover fired triggers or resolve SQL built at runtime. Use
  database permissions (for example a read-only login) for real protection. See the [limitations](docs/user/user-guide.md#limitations).

## Release and upgrade

Version **1.1.0 is the stable Latest release**; download it from [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.1.0). See [status](docs/status.md) for verification and the [upgrade instructions](docs/user/install.md#upgrading-to-110) for approval-rule migration.
Older short approval rules are retained but inactive in 1.1.0. Follow the [upgrade instructions](docs/user/install.md#upgrading-to-110) to review and fully qualify them. Color and context terminology is defined in the [dictionary](docs/user/dictionary.md).

## Get started

1. Download the latest `.vsix` from [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases).
2. Save your work, close SSMS, and sign in to the Windows account you use SSMS with.
3. Double-click the `.vsix` and follow the installer (if Windows blocks it, unblock it in its Properties first).
4. Start SSMS and open a query window: the safety pane appears on the right.

Requires 64-bit SSMS 22.x. No administrator rights are needed. The [install guide](docs/user/install.md) covers scripted
installs, updates, uninstalling and checking downloads.

## Documentation

**Using it**

- [Install guide](docs/user/install.md): download, install, update and uninstall.
- [User guide](docs/user/user-guide.md): the pane, the F5 confirmation, samples to try, troubleshooting and limitations.
- [Settings](docs/user/settings.md): warning modes, qualified safe targets, trusted routines and per-server environment rules.
- [Statement verdicts](docs/user/statement-verdicts.md): the colour of every T-SQL statement kind.

**Deploying it in a company**

- [IT deployment](docs/it/deployment.md): all-users installation, security facts and post-install checks.
- [Company signing](docs/it/signing.md): sign an internal copy of the unsigned GitHub package.

**Trusting it**

- [Status](docs/status.md): how well it is verified right now, and what is still open.
- [What the colours promise](docs/safety/contract.md), and the [policy questions](docs/safety/policy-questions.md) awaiting a decision.

**Developing it**

- [Contributing](CONTRIBUTING.md): the safety rule, code layout, build and test, and where to read before a change.
- [All documentation](docs/README.md) and the [changelog](CHANGELOG.md).

## License

[MIT License](LICENSE), Copyright (c) 2026 NagyAG.
Bundled dependency information is in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
