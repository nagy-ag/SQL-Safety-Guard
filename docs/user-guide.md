# SQL Safety Guard — user guide

SQL Safety Guard checks your query **before** SQL Server Management Studio (SSMS) 22 runs it and tells you whether it changes
data. It never runs anything itself and never sends your queries anywhere.

## Install (for yourself, no administrator needed)

1. Download `SqlSafetyGuard-<version>.vsix` from the project's GitHub Releases page.
2. If Windows marks the download as blocked: right-click the file > **Properties** > tick **Unblock** > **OK**.
3. **Close SSMS**, then double-click the `.vsix` file and follow the installer.
4. Start SSMS. Every query window now has the safety pane on its right.

If your IT department installed it for all users of the laptop, skip this; don't install a second copy for yourself.

**Update:** install the newer `.vsix` the same way; it replaces the old version.
**Uninstall:** in SSMS, **Extensions > Manage Extensions > Installed**, select SQL Safety Guard, **Uninstall**, then restart SSMS.

## The colours

| Colour | Meaning | What F5 does (default settings) |
|---|---|---|
| Green | Only reads, or only changes `#temp` tables / table variables, or only writes tables on your allow-list | Runs straight away |
| Yellow | The effect cannot be seen: a procedure call, dynamic SQL, a syntax error, something the analyzer cannot resolve | Asks first |
| Red | Changes a permanent table, object, permission or server setting | Asks first |

The pane also tells you *what* would change, for example "affects every row" for an `UPDATE` without a limiting `WHERE`.
When SSMS cannot be sure which window it is about to run, the guard always asks, even for green queries.

## Settings

**Tools > SQL Safety Guard > Settings...** (also **Tools > Options > SQL Safety Guard**).
Settings appear directly in the integrated Options tab and apply immediately.

- **F5 warnings**: warn on yellow and red (default), warn on red only, or off.
- **Safe tables allow-list**: tables whose writes you accept as green, one pattern per line (for example `dbo.Staging_*`).
- **Trusted procedures**: procedures you assert only read (for example `dbo.sp_WhoIsActive`).
- **Environment rules**: different labels, warning modes or extra allow-list entries per server and database, so a
  production server can always warn while a dev server warns less.

Use **Edit list...** or **Edit rules...** for a larger multiline editor. Save validates your entries; Cancel keeps the
previous values.

Be careful with the allow-list and trusted procedures: anything on them is shown green without a question.

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
