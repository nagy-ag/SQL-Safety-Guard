# Architecture and analysis boundaries

The analyzer and VSIX never open a SQL connection, execute SQL, or retrieve server metadata. SSMS owns query execution.
The extension reads query text and existing editor/connection labels. Database execution exists only under tests/ tooling.

## Project dependencies

| Project | Target | Depends on | Boundary |
|---|---|---|---|
| Analyzer | netstandard2.0 | ScriptDom 180.140.0 | Text and caller-supplied options only |
| VSIX | net472 | Analyzer, Visual Studio SDK/build tools, ScriptDom, Newtonsoft.Json | SSMS adapter and presentation; no SQL client |
| Tests | net10.0 | Analyzer, ServerHarness, xUnit, test SDK/coverlet | Offline contracts, regression, generated references and file/tool checks |
| ServerHarness | net10.0 executable | SqlClient, ScriptDom, Roslyn, cryptography support | Loads chosen analyzer DLL by path; SQL execution only in explicit execution modes |

Direct versions and transitive closures are recorded in project files and packages.lock.json. Visual Studio MSBuild builds
the VSIX; dotnet builds the offline projects. See [development.md](development.md) for exact commands.

## Code navigation

- `SqlSafetyAnalyzer` owns preprocessing, limits and the analysis entry point.
- `StatementWalker` owns statement dispatch, control/transaction flow, DML findings and row-filter annotations.
- `Classifier` keeps audited tables; `TargetResolver` follows aliases/CTEs/derived targets before temporary-name classification.
- `Scope` maps positions from recovered batches and literal dynamic SQL to editor text; it does not query database scope.
- `SideEffectScanner` detects effects embedded in reads. `AnalysisResult` aggregates findings and parse errors.
- `EnvironmentRules` matches caller-supplied labels. `GuardSettings`, `ExecutionMode` and `GuardDecision` are pure policy types
  in the analyzer assembly so policy can be tested without SSMS. Legacy typed-confirmation inputs remain accepted and ignored.
- VSIX `UI` owns the pane/dialogs, `Guard` intercepts execution, `Services` adapts host capabilities, and `Options` persists settings.
- Debug-only pane partials contain capture/test hooks. They are excluded from Release behavior.

## Representative flows

1. Analyzer: text/options -> preprocessing and parser -> walker/resolver/classifier/scanner -> mapped findings -> aggregate verdict.
2. Pane: buffer/selection change -> debounce/version/cancellation -> snapshot/options -> background analysis -> version check -> tracking rows.
3. Guard: Query.Execute -> execution-target capture -> settings -> captured-text analysis -> GuardDecision -> Cancel or original SSMS dispatch.
4. Campaign: validate all rows -> owned world/principals -> analyze and execute -> observe -> durable receipts -> qualified reports.

The pane uses the active document for display. The guard independently captures SSMS's actual execution target and checks
other host identities for disagreement. Do not merge these authorities. Shared formatting and settings must never substitute
pane state for the captured execution snapshot.



### SSMS 22 target

- SSMS 22.11.2 is built on the **Visual Studio 18.11 shell** (VS 2026; `Microsoft.VisualStudio.Shell.15.0.dll`
  18.11), 64-bit, running on .NET Framework 4.7.2+. The extension targets **net472**, compiles against
  `Microsoft.VisualStudio.SDK` 17.14 (the shell's binding redirects cover it), and is packaged with
  `Microsoft.VSSDK.BuildTools` 17.14.
- Manifest target: `<InstallationTarget Id="Microsoft.VisualStudio.Ssms" Version="[22.0,)">` with
  `ProductArchitecture amd64`. This is the same Id/range Microsoft's own SSMS 22 extensions (SchemaCompare,
  SqlFormatter, PresenterMode…) use in `<SSMS>\Common7\IDE\Extensions\Microsoft\*\extension.vsixmanifest`.
- SQL Editor command IDs come from SSMS's own command table (`SQLEditors.dll` → `Menus.ctmenu`, decompiled with
  `vsct.exe`). `Query.Execute` is `{52692960-56BC-4989-B5D3-94C47A513E8D}:1`, the SQL Editor toolbar is
  `{33f13ac3-80bb-4ecb-85bc-225435603a5e}:0x0001`, the "SQL Query Editor" key-binding scope is
  `{b5a506eb-11be-4782-9a18-21265c2ca0b4}`, and the Available Databases combo is `{52692960-…}:0x0004`.
  `Query.Execute` is the only execute command in that table.

### Analyzer

`SqlSafetyAnalyzer.Analyze(sql, options, cancellationToken)` parses with the **newest `TSqlNNNParser`** in the loaded ScriptDom
assembly, found by reflection (`TSql180Parser` with ScriptDom 180.x, so SQL Server 2025 syntax parses). It walks
statement containers (BEGIN/END, IF/ELSE, WHILE, TRY/CATCH, all GO batches) and classifies their contents. ELSE-IF
chains are walked iteratively. Parsing and walking run on a dedicated background thread with a 64 MiB reserved
stack, not the SSMS UI stack. Cancellation is checked during tokenization, parsing and traversal.

- Targets are resolved through **aliases** (`UPDATE o … FROM dbo.Orders o`), **CTEs** (recursively, CTE → CTE,
  with a cycle/depth guard and per-CTE memoization), and **derived tables**. Base targets are deduplicated. Alias
  and CTE resolution comes before the temp-name rule: a `[#t]` alias for `dbo.Orders` is still a real write.
- **Composable DML** in derived table sources is classified independently of its outer target. Inserting into
  `#audit` from `(DELETE dbo.Customers OUTPUT deleted.Id)` is red. Nested `OUTPUT … INTO` targets are classified
  separately too. `INSERT … EXEC` adds an Unknown finding.
- **Literal dynamic SQL** (`EXEC('…')`, `EXEC('a' + 'b')`, `sp_executesql N'…'`) is parsed recursively, up to 3
  levels deep. Its findings are reported at the EXEC's line with the note "inside dynamic SQL". Anything built
  at runtime is Unknown.
- Procedure/function/trigger/view bodies are **not** walked: creating them doesn't run them.
- **Side effects hidden in reads:**
  - `OPENQUERY`, `OPENROWSET` (query form) and `OPENDATASOURCE` used as a *source* are Unknown. They send text
    to another server, and that text can change data there. This applies in FROM, subqueries, CTEs, `DECLARE`,
    `SET`, cursors and IF/WHILE conditions.
  - `NEXT VALUE FOR` is a Real write: it advances the sequence, and `ROLLBACK` doesn't undo it.
  - Not flagged: `OPENROWSET(BULK …)`, which only reads a file; four-part-name reads (`srv.db.dbo.t`), which read
    a remote table; and a `DEFAULT (NEXT VALUE FOR …)` in `CREATE TABLE`, which doesn't run anything.
- **Syntax-error recovery is bounded and conservative.** Each `GO` batch is parsed separately. Findings recovered
  from a broken batch are marked non-executable until that batch's syntax is fixed:
  - The statement with the error is blanked (line breaks kept, so every line and column number stays right) and
    the batch is parsed again, up to 25 times or 3 seconds. Everything around the error is analyzed normally.
  - Those statements are labelled "won't run now: this batch has a syntax error at line N" and shown dimmed. They
    still count toward the overall colour, because they will run once the error is fixed. The banner says so, e.g.
    "4 real writes, 1 unknown, 3 syntax errors · 13 more in a batch that will fail". The F5 dialog dims those entries and explains them separately.
  - A `CREATE PROCEDURE/FUNCTION/TRIGGER/VIEW` that isn't the first statement of its batch gets "add GO before it".
    Everything after it in that batch is treated as its body, which is what SQL Server would do.
  - What couldn't be parsed is listed as **NOT ANALYZED** (e.g. "line 86"). That text is scanned for write
    keywords (outside comments and strings), and each hit becomes a yellow "possible UPDATE" row.
  - Broken batches over 32 Ki characters skip repeated reparsing and use the whole-batch NOT ANALYZED/possible-write
    fallback. This avoids quadratic recovery on thousands of broken statements, at the cost of detailed target
    classification. Smaller batches that exhaust recovery use the same fallback.
- **UPDATE/DELETE without WHERE** gets "no WHERE: affects every row" in its pane details. If an inner join or `TOP (n)` limits the rows instead, the note says so and the bold line is not shown.
  A CTE or derived table gets a note saying it limits the rows. `TRUNCATE` gets "removes every row" and
  DROP gets "drops ...".
- **SQLCMD mode** (`:setvar`, `$(var)`, `:r`, `:connect`, `:on error`, `!!`). The guard reads whether
  **Query > SQLCMD Mode** is on for the window being executed or analyzed. It asks for the same command status SSMS
  uses to draw the menu check mark (`Query.SQLCMDMode`, `{52692960-…}:17`); nothing is sent to the server.
  - **Mode off:** SQLCMD lines are what the server would get: syntax errors, so their batches fail. They're
    reported that way, plus one **SQLCMD MODE OFF** row: "turn on Query > SQLCMD Mode, or the batches with SQLCMD
    lines fail". A `$(…)` inside an ordinary string is just text and gets no hint.
  - **Mode on, or the mode can't be read:**
    - The analyzer parses a temporary same-length copy of the script with the SQLCMD lines blanked and each
      `$(var)` replaced by a placeholder. Line and column numbers still match the editor, and the copy is
      discarded after the check.
    - **Every `$(…)` substitution adds Unknown**, even a value that looks like a safe name. The analyzer does not
      execute SQLCMD's expansion rules: values can inject statements, concatenate identifiers or escape strings.
      Ordered `:setvar` definitions supply display hints only; later definitions never rewrite earlier hints.
    - `:r` (another file, not read), `!!` (an operating-system command), and `:exit(query)` (which can execute a
      statement) are yellow. `:connect` adds a TARGET SWITCH warning, like `USE`.
  - Either way, nothing is executed and no file is opened. SSMS still sends your original text.

### F5 guard

The guard hooks `Query.Execute` via **`DTE.Events.CommandEvents[guid, id].BeforeExecute`** rather than
`IVsRegisterPriorityCommandTarget`:

- The event is filtered to this one command. A priority command target sees every command and every QueryStatus
  in the IDE, and a bug there can break unrelated commands.
- `CancelDefault` is a supported way to stop the command.
- It has worked for SSMS extensions since SSMS 17. It fires for every route into the command: F5, Ctrl+E, Alt+X,
  the Execute button and `DTE.ExecuteCommand`. The isolated SSMS 22 campaign verifies shared Query.Execute dispatch;
  physical shortcuts and the unavailable toolbar UIA surface are not independently covered.

**Loading early.** SSMS defers background auto-loaded packages until startup has finished, which took 10–30 s in
testing. A query window opened at startup, for example by double-clicking a `.sql` file, is usable within a second
or two, so F5 there would have run unguarded. `EnsureGuardLoadedListener`, a MEF text-view listener, loads the
package when the first editor window is created. Tested: Execute fired the moment a connected window appeared
after launch was caught on two consecutive starts. The package loaded about 0.9 s after the SQL editor package.

**Run anyway** does not re-issue the command. The modal dialog is shown while SSMS is still inside the original
dispatch, so the guard returns `CancelDefault = false` and that same execution continues. Nothing can be
intercepted twice, and a re-entrancy flag guards the handler anyway. Tested: the UPDATE ran exactly once.

Safety rules:

- If a guard failure happens before a verdict exists, it asks **Run anyway** or **Cancel**, with Cancel as the
  default and Escape cancelling. Only known Off settings with a verified execution target can skip this question.
  If the failure box cannot be shown, execution is cancelled. The Output pane logs the failure, the guard becomes
  degraded, and a once-per-session InfoBar is attempted. After a red/yellow result is computed, status, logging or
  wait-dialog cleanup failures cannot authorize execution. A failed confirmation dialog cancels that execution.
- The safety pane, finding rows and settings/dialog buttons use Visual Studio theme resources. Section headings
  pair themed text with verdict-colour markers; secondary row text uses the themed grey brush. Verdict markers
  have themed outlines. The pane's **Settings...** button opens the same integrated page as
  **Tools > SQL Safety Guard > Settings...**. Execution-target warnings use readable dialog text with an amber edge.
  Consult the verification status for the latest capture review across all 13 themes SSMS 22 lets you pick
  (`environment.visualExperience.colorTheme`): Light, Cool Breeze, Icy Mint, Sunny Day, Juicy Plum, Mango Paradise,
  Bubblegum, Silky Pink, Dark, Cool Slate, Moonlight Glow, Mystical Forest and Spicy Red. The Light/Dark (Extra
  Contrast) and High Contrast themes can't be selected through that setting; they follow the Windows contrast
  setting and weren't tested. The Cancel button has an accent border, so it reads as the default button in dark
  themes too. Settings list editors use the same explicit default Cancel border.
- **All execution analysis runs off the UI thread**, behind the Visual Studio threaded wait dialog (shown after
  300 ms). Cancel propagates to the analyzer and cancels execution. If the VS wait-dialog service is unavailable,
  analysis still uses a background thread, but no cancellable wait UI can be shown.
- Resource budgets return an Unknown **too complex to analyze** finding instead of attempting unbounded parsing:
  256 nested parentheses or BEGIN/CASE blocks, 512 consecutive control-flow prefixes or ELSE IF links,
  5,000 IF/WHILE/ELSE keywords per GO batch (flat expression/list length is not budgeted),
  256 CTE-resolution depth, and a shared 40-million-work-unit / 10-second budget. These are conservative limits,
  not SQL Server syntax limits. A valid complex script can therefore be yellow.
- Scripts over **5,242,880 characters** (about 10 MiB of UTF-16 text) return a yellow **too large to analyze (N MB)**
  finding. They prompt in the default YellowRed mode; explicit Red-only and Off modes still apply.

### Execution target and connection (SSMS-specific)

Execution capture does not use DTE.ActiveDocument. SSMS 22's ShellWindowPaneUserControl dispatches commands
through the thread-focused WinForms control's IOleCommandTarget ancestor chain. DTE activation can select B
while that chain still reaches SQL editor A. The guard reads GetFocus (it never sets focus), resolves the owning
SqlScriptEditorControl, and reads that owner's IVsCodeWindow last-active SQL view and selection. It cross-checks
the host's GetSelectedTextSpan, both monitor-selection window/document frames, DTE identity and the focused
IVsTextManager view. A Results text view must resolve to the same SQL owner, not be mistaken for SQL source.

The same owner supplies IsSqlCmdModeEnabled, its single ConnectionInfoList, and the live m_connection's
DataSource/Database. These execution fields never fall back to another window's global connection/combo.
Manual pane analysis still uses the active-document connection service and may describe a different candidate.

Missing services, unsupported host members, multiple connections or disagreeing sources produce a yellow
"The guard could not verify which window will run" confirmation, independent of verdict and mode (including Off).
The captured candidate is identified as such; Cancel is default. Target lookup failure is not a green result or a
permission to execute. Other guard failures ask for approval using the failure policy above.

These undocumented host members are version-sensitive. SSMS 22.11.2 was checked with isolated-profile Debug
instrumentation, SQL Server Extended Events and independent counters. See
[execution verification](verification/extension/extension-checks.md) for before/after evidence and coverage limits.

## Limitations

- **Not a security boundary.** Triggers fired by a write, the insides of procedures and functions, and dynamic
  SQL built at runtime can't be seen. Scalar functions called in a SELECT are treated as reads; a CLR or
  extended function could still have side effects. A green verdict means "nothing visible writes", not
  "nothing can write".
  Use a read-only login (`db_datareader` + `db_denydatawriter`) for real protection on production.
- **SQLCMD:**
  - If the SQLCMD Mode state can't be read (for example, a future SSMS changes the command), SQLCMD lines are
    interpreted as if the mode were on, which errs toward showing writes.
  - Variable substitution is not expanded for semantic classification. Every substitution is at least yellow,
    including inside strings and values supplied by SSMS options. Original editor positions are preserved.
  - Files included with `:r` are not read.
- **Characters the T-SQL tokenizer rejects** (such as a stray `\`) are skipped, and analysis resumes after them, so
  they can't hide a later `GO` or write.
- **Recovery around syntax errors is a heuristic.** It splits statements by keywords, so an unusual statement can
  make it blank more text than necessary. Anything blanked is listed as NOT ANALYZED and scanned for write
  keywords. Large broken batches deliberately use this coarse fallback instead of recovering every statement.
- Only text run through `Query.Execute` from a `.sql`/SQL editor is guarded. Not guarded: MDX/DMX/XMLA editors,
  Object Explorer actions (Edit Top 200 Rows, scripting wizards, dialogs), Copilot-run queries, and other
  extensions.
- Writes through a CTE or derived table that joins several tables list *every* base table (conservative).
- Views and synonyms are classified by name (a write to a view is red; the view's base tables aren't resolved).
- `KILL`, `SHUTDOWN`, `RECONFIGURE`, `RESTORE`, Service Broker statements (`SEND`, `RECEIVE`, …) and any other
  statement type not listed above are red (conservative). BACKUP, UPDATE STATISTICS, CHECKPOINT and recognized
  DBCC commands with unknown effects (for example, `DBCC MEMORYMAP`) are yellow. Unrecognized DBCC names
  (for example, `DBCC SSG_UNKNOWN`) parse as DLL unloads and are red; other DBCC commands are judged per command
  (read-only green, changing red).
  [docs/history/2026-10-27-statement-coverage.md](history/2026-10-27-statement-coverage.md) records the reviewed ScriptDom statement inventory and
  important semantic/resource limits; an inventory is not a proof of soundness. See also the
  [statement verdicts](statement-verdicts.md) for representative SQL examples and their colours. The
  [systematic verification report](history/2026-10-27-all-kinds/README.md) separates all 334 static
  checks from 267 live-passed kinds, three failed-only kinds, and 64 not live tested. The current verification state is always in
  [docs/verification/status.md](verification/status.md); [docs/README.md](README.md) lists every current document.
- **Transactions are tracked by reading the text, not the session.** A `COMMIT` with no `BEGIN TRAN` in the text is
  yellow, because it may commit work an earlier run left open. A transaction still open at the end gets an
  OPEN TRANSACTION row. The guard can't see whether your session actually has an open transaction right now.
  Parsed `@@TRANCOUNT`/`XACT_STATE()` predicates are distinguished from strings/comments. Unreachable branches,
  zero-iteration loops and statements after RETURN cannot close a possibly open transaction. GO repetition and
  uncertain control transfers are conservative. With implicit transactions on, explicit BEGIN can add two levels;
  a tableless `SELECT 1` does not open one. This is not a full control-flow or SQL Server session simulator.
- Box (column) selections are analyzed as the selected text; the reported line numbers assume a normal selection.
- Allow/trusted lists require known database/default-schema context when qualification is omitted. The VSIX does
  not discover the login's default schema. Target switches disable contextual trust for the entire analysis, but
  do not re-select environment mode mid-script. Trust entries are user assertions, not verified procedure bodies.
- Microsoft does not officially support third-party SSMS extensions. A future SSMS update could change command
  IDs or host internals. If the guard cannot attach to the execution command at all, execution is unguarded and
  can run without a question. An attached guard that fails during a check uses the failure confirmation above.

