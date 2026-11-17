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
- `DynamicSqlVariables` proves supported batch-local string inputs to EXEC without executing SQL or retrieving session values.
- `Classifier` keeps audited tables; `TargetResolver` follows aliases/CTEs/derived targets before temporary-name classification.
- `Scope` maps positions from recovered batches and literal dynamic SQL to editor text; it does not query database scope.
- `SideEffectScanner` detects effects embedded in reads. `AnalysisResult` aggregates findings and parse errors.
- `EnvironmentRules` matches caller-supplied labels. `GuardSettings`, `ExecutionMode` and `GuardDecision` are pure policy types
  in the analyzer assembly so policy can be tested without SSMS. Legacy typed-confirmation inputs remain accepted and ignored.
- VSIX `UI` owns the pane/dialogs, `Guard` intercepts execution, `Services` adapts host capabilities, and `Options` persists settings.
  `CompactSettingsControl` presents the six existing options in aligned rows through the native GEL content factory.
  The registered settings keys, types, defaults and full legacy/native migration remain unchanged; generated property
  controls are hidden to avoid duplicates. The custom form persists through `GeneralOptionsPage.PersistChanges`,
  observes external option changes without writing them back, and invokes the existing menu command handlers.
  SSMS labels the GEL content area Preview; edits in this form are real and apply immediately. Its scoped focus handler
  permits keyboard navigation inside the editable form. Classic-mode property-grid fallback is retained.
- `Commands/UninstallCommand` confirms and hands off to the local SSMS installer. `Services/ExtensionUninstallPlan` binds
  the running host's isolation ID, command-line suffix and loaded per-user extension path before constructing fixed uninstall
  arguments. It has no query/guard/settings dependency; mismatched or administrator-managed installs are refused.
- Debug-only pane partials contain capture/test hooks. They are excluded from Release behavior.

## Representative flows

1. Analyzer: text/options -> preprocessing and parser -> walker/resolver/classifier/scanner -> mapped findings -> aggregate verdict.
2. Pane: buffer/selection change -> debounce/version/cancellation -> snapshot/options -> background analysis -> version check -> tracking rows.
3. Guard: Query.Execute -> execution-target capture -> settings -> captured-text analysis -> GuardDecision -> Cancel or original SSMS dispatch.
4. Campaign: validate all rows -> owned world/principals -> analyze and execute -> observe -> durable receipts -> qualified reports.

The pane uses the active document for display. The guard independently captures SSMS's actual execution target and checks
other host identities for disagreement. Do not merge these authorities. Shared formatting and settings must never substitute
pane state for the captured execution snapshot.

`PanePresentation` owns display-only verdict filtering and operation counts. `PaneGroup.Findings` always retains the
complete findings; `VisibleFindings` is a projection used only for rendering and explicit pane navigation/highlighting.
Worst object verdicts, unfiltered display totals and guard decisions never use the filtered projection. Operations are not merged
by editor line or source span: dynamic SQL can map several distinct actions to one EXEC span. `PaneOperationPresentation`
uses the analyzer's dynamic-SQL provenance note and source span to place decoded operations under one outer EXEC;
separate EXECs retain separate containers. An ordinal keeps otherwise identical operations independently expandable.
Highlight rendering combines identical editor spans at the worst visible verdict, so a green decoded operation cannot
paint over a permanent write at the same EXEC anchor. This combines adornments only, never operation rows or counts.
Optional `Finding.SourceRanges` track decoded operation characters back through proven literals, variable copies,
concatenation and nested dynamic text. Existing Finding offsets remain execution anchors for guard compatibility.
Selecting a dynamic block uses its EXEC as the primary location; selecting a decoded operation uses its string
fragments, with the other location outlined neutrally. Missing provenance falls back to EXEC without changing proof.
Rendered text rectangles are unioned before stroking, removing shared borders across adjacent visual rows, including
wrapped lines. Verdict fills remain separate, ordered from least to most severe; disjoint ranges stay unfilled.
Typed code bodies attach to their exact containing finding. Object and containing-operation markers retain the actual verdict; their preview badges show purple Inside or gray Remote totals. An expandable preview row uses the matching purple/gray left circle and the real risk breakdown on the right. Child operations retain their risk markers. Top-level `Counts` and `DisplayCounts` stay submission-only; badges count preview descendants separately.

`PaneScopeRows` projects each body and decoded EXEC into local `PaneGroup` object rows plus standalone findings
without targets. It reuses the original `PaneFinding` instances; no verdict, guard collection or count is rewritten.
Groups follow first occurrence and their findings retain source order. Decoded EXEC containers are kept separate
before grouping their target objects; nested scopes never merge by name. Preview keys normalize identified targets
without assuming that the editor's database resolves a short name. Embedded remote/job bodies already supply a decoded
scope, so they do not add a redundant Dynamic SQL container. Remote-query expressions stay direct operation rows,
rather than being parsed as object names. Inner object groups suppress dynamic-container
labels, reuse the existing header template and navigate/highlight proven string sources where available. Implicit
row templates render the mixed collection recursively. Expansion restoration follows each scope's group keys and
existing operation identities; persistent group selection additionally checks the containing operation's scoped identity.

The six display-only flags combine effect colors by union and gate Inside and Remote previews. Context independently matches `IsContextChange`, without requiring Safe. `PaneDisplayCounts` separates those context operations from green display totals while `PaneVerdictCounts` and analyzer verdicts remain unchanged. Recursive matching retains ancestors without changing their verdicts. `MatchingOperations` supplies matched operations for object navigation/highlighting. Full model collections and counts remain unfiltered. Automatic label measurement switches the whole row between text and circles without wrapping; explicit Text raises the minimum width. Modes are available from the filter/Analysis context menu and retain filter selections. Tooltips pair each name/count with its semantic `SafetyDictionary` description.

The expanded header contains the analyzed scope without color badges. The collapsed strip derives passive numbered circles from the same six unfiltered `FilterCount` totals as the display controls; absent categories are omitted. It keeps color meanings and accessible names, shrinks large numbers within circular badges, and clears stale counts on pending/placeholder transitions. All circles share the strip's existing expand target; they are not additional filter controls.

`LineGutterWidth` measures the widest operation line across full model descendants when results change. All header templates bind the same width, including an empty gutter on previews. Tabular, right-aligned digits and spacing separate line numbers from counts; filtering and expansion do not recalculate the width.
Definition risk fill and navigation use a tracking span for the header line. A separate, unfilled purple rectangle
outlines the full parsed module span, excluding GO and separate calls. Parent-operation links preserve the nearest
enclosing module context when selecting nested preview operations. Context adornments use the body-context tag;
they clear and restore with the existing persistent selection. The pane moves the caret without taking
editor focus for mouse navigation and preserves the execution selection. Highlights persist through passive analysis
and clear on another click, editor interaction, edits, selection changes or filter changes. The process-local WPF input
observer is removed when the margin is disposed. Minimum expanded width is measured from the actual filter buttons and
counts; whole-target warnings appear only in expanded operation reasons, never row headers.



### SSMS 22 target

- SSMS 22.11.2 is built on the **Visual Studio 18.11 shell** (VS 2026; `Microsoft.VisualStudio.Shell.15.0.dll`
  18.11), 64-bit, running on .NET Framework 4.7.2+. The extension targets **net472**, compiles against
  `Microsoft.VisualStudio.SDK` 17.14 (the shell's binding redirects cover it), and is packaged with
  `Microsoft.VSSDK.BuildTools` 17.14.
- Manifest target: `<InstallationTarget Id="Microsoft.VisualStudio.Ssms" Version="[22.0,23.0)">` with
  `ProductArchitecture amd64`. It supports SSMS 22.0 and later 22.x releases, with an upper bound before the next
  major version. Representative-build evidence and coverage limits are in [status.md](../status.md).
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
- **Dynamic SQL** (`EXEC('…')`, `EXEC('a' + 'b')`, `sp_executesql N'…'`) is parsed recursively, up to 3
  levels deep. Its findings are reported at the EXEC's line with the note "inside dynamic SQL".
  `DynamicSqlVariables` additionally proves batch-local `varchar`/`nvarchar` strings from straight-line `DECLARE`,
  `SET`, copies, `+` and `+=`, including ordinary `BEGIN/END` blocks. It records inputs at each EXEC and gives nested
  dynamic batches and procedure bodies independent maps. No values cross `GO` or come from text outside the selection.
  Batches containing branches, loops, jumps or TRY/CATCH do not use variable proof. Other unmodeled statements clear
  known values, including SELECT/UPDATE assignments, cursor FETCH and procedure outputs. Truncating assignments or
  expressions, unsupported conversions/types, non-ASCII varchar text and ambiguous variable casing stay Unknown.
  Expansion is bounded to one million characters and shares the analyzer's work/time budget. Decoded findings use the normal pane and confirmation paths; qualified approval and destination-context policies still apply.
- Creating a module does not run its body. `AnalysisResult.CodeBodies` contains typed `CodeBodyAnalysis` previews
  for procedure, DML/DDL/logon trigger, scalar/inline-table/multi-statement-table function and view definitions.
  (`ProcedureBodies` is a compatibility alias.) Each fresh walker has independent transaction/target/variable context,
  original editor positions, and the shared cancellation/work budget. RETURN expressions are scanned too.
  Preview findings never enter execution verdicts, counts or F5 warnings. Recovery in a broken batch, unavailable
  T-SQL/CLR bodies, dynamic parse failures and limits mark the preview incomplete. A module owns the rest of its batch
  during recovery, including errors reported inside RETURN or SELECT. Calls never resolve definitions from the editor
  or server.
- Embedded remote query strings and SQL Agent T-SQL commands use the same model with separate context labels and
  source maps. OPENQUERY/query OPENROWSET (sources and write targets) and EXEC AT keep their original outer verdicts;
  remote T-SQL interpretation never proves a remote runtime safe. Job command previews apply to named proven strings
  in sp_add_jobstep/update_jobstep. Only add defaults to TSQL; update needs a proven subsystem. Agent tokens, positional
  arguments and unproven/non-T-SQL commands stay opaque. Remote/job walkers clear local allow/trust/tempdb opt-ins.
  Previews can nest to eight inspected levels, with an explicit Unknown diagnostic at the limit. Pane models attach
  nested bodies by exact finding identity and restore expansion/selection recursively. Children use SQL source ranges
  as primary highlights; Sent at/Stored at links retain the containing execution anchor.
- **Side effects hidden in reads:**
  - `OPENQUERY`, `OPENROWSET` (query form) and `OPENDATASOURCE` used as a *source* are Unknown. They send text
    to another server, and that text can change data there. This applies in FROM, subqueries, CTEs, `DECLARE`,
    `SET`, cursors and IF/WHILE conditions.
  - Qualified scalar/object-method and unknown table-valued calls produce Unknown FUNCTION findings; unqualified scalar
    built-ins and recognized unqualified STRING_SPLIT/GENERATE_SERIES retain their existing behavior. No deployed
    implementation is inferred, including from a visible definition.
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

The same owner supplies SQLCMD state through `IsSqlCmdModeEnabled` or inherited `IsSqlCmdMode`, its single ConnectionInfoList, and the live m_connection's
DataSource/Database. These execution fields never fall back to another window's global connection/combo.
`TargetSqlCmdModeReader` checks both boolean aliases across the captured editor's type hierarchy. Missing, malformed,
throwing or conflicting properties stay unknown; there is no global active-window fallback or cached cross-window state.
Manual pane analysis still uses the active-document connection service and may describe a different candidate.

Missing services, unsupported host members, multiple connections or disagreeing sources produce a yellow
"The guard could not verify which window will run" confirmation, independent of verdict and mode (including Off).
The captured candidate is identified as such; Cancel is default. Target lookup failure is not a green result or a
permission to execute. Other guard failures ask for approval using the failure policy above.

These undocumented host members are version-sensitive. SSMS 22.11.2 was checked with isolated-profile Debug
instrumentation, SQL Server Extended Events and independent counters. See
[execution verification](ssms-testing.md) for before/after evidence and coverage limits.

## Limitations

User-visible limitations are maintained in one place: [user guide, Limitations](../user/user-guide.md#limitations).
That section covers what the guard cannot see, what is not guarded, SQLCMD, syntax-error recovery, transactions,
connection and name context, and conservative verdicts.

The evidence behind them:

- The [statement coverage inventory](../history/2026-10-27-statement-coverage.md) records the reviewed ScriptDom statement
  inventory and important semantic and resource limits; an inventory is not a proof of soundness.
- The [systematic verification report](../history/2026-10-27-all-kinds/README.md) separates all 334 static checks from 267
  live-passed kinds, three failed-only kinds and 64 kinds not tested live.
- [Statement verdicts](../user/statement-verdicts.md) give representative SQL and colours; [status.md](../status.md) has
  the current verification state.

## Qualified authorization and context

AnalyzerOptions uses strict four-part rule matching for production classification. The existing AllowList parser retains a compatibility mode for text import and legacy editor round-trips; that mode is not used to grant analyzer authorization. Object rules require literal server/database/schema and exact case, with globs only in the final name. Procedure and FUNCTION-prefixed function entries are separate. Written linked-server aliases cannot establish a destination. SQL must include a schema; default-schema fallback is not proof of object identity.

StatementWalker retains a conservative union of possible databases after USE, including across GO. Repeated control flow pre-collects visible USE destinations; decoded dynamic execution saves/restores context. Body previews use a fresh walker without verified execution identity. Context-specific allow-list regexes are cached per options instance. Finding.RequiresConfirmation carries destination policy escalation to GuardSettings.ShouldWarn independently of the overall verdict. Context rules prevent an initial Off fast path from bypassing that analysis; errors and unverified switches fail closed.

SafetyDictionary supplies shared semantic descriptions to the toggle tooltips and six structured settings rows. IsContextChange drives the primary blue pane marker, independent Context count/filter, and selection outline. The analyzer still classifies USE as ReadOnly; no execution policy is inferred from its display category. Compact settings use supported shell styles for checkboxes and the warning ComboBox, with theme-resource foregrounds for checkbox labels and the legend.
