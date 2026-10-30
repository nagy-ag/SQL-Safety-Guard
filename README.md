# SQL Safety Guard for SSMS 22

SQL Safety Guard checks T-SQL **before it runs** in SQL Server Management Studio 22.11.
It shows whether a query reads data, touches temporary objects, or changes permanent data.
The extension never executes SQL itself or sends your queries elsewhere.

It is for people working on databases where a mistaken `UPDATE` without `WHERE`,
a write in the wrong window, or a hidden procedure write would be expensive.

## What you see

A safety pane beside every query editor updates as you type or change the selection.
It groups the objects a query may change and explains the warning.
**Analyze Query** (Tools > SQL Safety Guard, toolbar, or **Ctrl+Shift+F5**) checks immediately.

The F5 guard checks the exact selection or whole window SSMS is about to execute.
It covers F5, Ctrl+E, Alt+X and the Execute button through the same command.
With the default settings, yellow and red pause for a buttons-only confirmation.
**Cancel** is the default; **Run anyway** continues that execution. No typing is required.

| Verdict | Colour | Meaning |
|---|---|---|
| Read-only | Green | Reads, variables, session/control statements, balanced transactions, known read-only procedures |
| Temp only | Green | Writes only temporary tables/procedures or table variables |
| Allow-listed | Green | Writes only tables matching your safe-table settings |
| Unknown | Yellow | Cannot see what runs: procedures, runtime dynamic SQL, parse errors or unresolved targets |
| Real write | Red | Changes a permanent object, permission or server state |

Green runs directly only when the execution target is verified.
An ambiguous target asks for confirmation even in Off or Red-only mode.
Environment rules can choose warning modes and add trusted tables/procedures per connection.

## Safety rules

Unknown effects stay yellow. Parse errors and analysis limits do not become green.
A guard failure before a verdict asks **Run anyway** or **Cancel**; a failed dialog cancels.
Known Off settings with a verified target respect the user's choice to skip checking.
Failures are logged in Output, mark the guard degraded, and attempt a session warning InfoBar.
A later status or logging failure cannot authorize a computed red/yellow warning.

This is a safety net, not a security boundary. It cannot inspect procedure bodies,
triggers or runtime SQL. Use a read-only login for production protection.
If an SSMS change prevents the guard attaching to the execution command at all,
that command is unguarded. See the [limitations](docs/user-guide.md#limitations).

## Install

1. Download the VSIX from [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases).
2. Close SSMS, unblock the download in Properties if needed, and double-click the VSIX.
3. Start SSMS; the safety pane appears beside each SQL query editor.

GitHub packages are unsigned. The [user guide](docs/user-guide.md) covers scripts, upgrades and uninstalling.

## Documentation

- [User guide](docs/user-guide.md): use the pane, try examples, shortcuts and settings.
- [IT deployment](docs/it-deployment.md): installation for a team.
- [Development](docs/development.md): build, debug, implementation and limitations.
- [Verification status](docs/verification/status.md): evidence, coverage and open questions.
- [Changelog](CHANGELOG.md): changes in each release.
- [All documentation](docs/README.md): current documents and historical reports.

## License

[MIT License](LICENSE), Copyright (c) 2026 NagyAG.
Bundled dependency information is in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
