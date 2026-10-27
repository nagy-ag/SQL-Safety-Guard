using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Parses text and walks every statement (recursing into blocks, IF/WHILE, TRY/CATCH, GO batches and
    /// literal dynamic SQL), adding one finding per classified action.
    /// </summary>
    internal sealed class StatementWalker
    {
        private const string DynamicSqlNote = "inside dynamic SQL";

        /// <summary>Statement keywords that can change something; used to scan text that could not be parsed.</summary>
        private static readonly HashSet<TSqlTokenType> WriteKeywords = new HashSet<TSqlTokenType>
        {
            TSqlTokenType.Insert, TSqlTokenType.Update, TSqlTokenType.Delete, TSqlTokenType.Merge, TSqlTokenType.Truncate,
            TSqlTokenType.Drop, TSqlTokenType.Alter, TSqlTokenType.Create, TSqlTokenType.Execute, TSqlTokenType.Grant,
            TSqlTokenType.Revoke, TSqlTokenType.Deny, TSqlTokenType.Bulk, TSqlTokenType.Restore, TSqlTokenType.Dbcc,
            TSqlTokenType.Kill, TSqlTokenType.Shutdown, TSqlTokenType.Reconfigure,
        };

        private AnalyzerOptions _options;
        private readonly List<Finding> _findings;
        private readonly List<SqlParseError> _errors;
        private readonly List<SkippedRegion> _skipped;

        /// <summary>Transaction nesting across the whole text (a transaction outlives GO batches).</summary>
        private TransactionState _transactions = new TransactionState();

        /// <summary>True inside an IF whose condition checks @@TRANCOUNT or XACT_STATE().</summary>
        private bool _transactionGuard;

        /// <summary>
        /// Tracks @@TRANCOUNT as the text would change it, to find a COMMIT of a transaction started outside the text
        /// and a transaction left open at the end. IF/ELSE and TRY/CATCH branches are walked from the same state.
        /// </summary>
        private sealed class TransactionState
        {
            public int Depth { get; set; }

            public int Peak { get; set; }

            public Position? OpenedAt { get; set; }

            public Position? PeakOpenedAt { get; set; }

            public bool OpenedImplicitly { get; set; }

            public bool Implicit { get; set; }

            public bool Terminated { get; set; }

            public bool Uncertain { get; set; }

            private HashSet<string> SavePoints { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            public TransactionState Copy() => new TransactionState
            {
                Depth = Depth, Peak = Peak, OpenedAt = OpenedAt, PeakOpenedAt = PeakOpenedAt, OpenedImplicitly = OpenedImplicitly,
                Implicit = Implicit, Terminated = Terminated, Uncertain = Uncertain, SavePoints = new HashSet<string>(SavePoints, StringComparer.OrdinalIgnoreCase),
            };

            public void Begin(Position at, bool implicitly = false)
            {
                if (Depth == 0)
                {
                    OpenedAt = at;
                    OpenedImplicitly = implicitly;
                }

                Depth++;
                if (Depth > Peak)
                {
                    Peak = Depth;
                    PeakOpenedAt = OpenedAt;
                }
            }

            public void Commit()
            {
                if (Depth > 0)
                    Depth--;
                if (Depth == 0)
                    Reset();
            }

            /// <summary>ROLLBACK ends the whole transaction, unless it rolls back to a savepoint.</summary>
            public void Rollback(string name)
            {
                if (name != null && SavePoints.Contains(name))
                    return;
                Depth = 0;
                Reset();
            }

            public void Save(string name)
            {
                if (name != null)
                    SavePoints.Add(name);
            }

            private void Reset()
            {
                OpenedAt = null;
                OpenedImplicitly = false;
                SavePoints.Clear();
            }

            /// <summary>The state after one of two alternative branches: the more open one, to be safe.</summary>
            public static TransactionState Merge(TransactionState a, TransactionState b)
            {
                if (a.Terminated != b.Terminated)
                {
                    var exited = a.Terminated ? a : b;
                    if (exited.Depth == 0 && !exited.Uncertain)
                    {
                        var continuing = (a.Terminated ? b : a).Copy();
                        continuing.Peak = Math.Max(a.Peak, b.Peak);
                        continuing.PeakOpenedAt = a.Peak >= b.Peak ? a.PeakOpenedAt : b.PeakOpenedAt;
                        return continuing;
                    }
                }
                var merged = (a.Depth >= b.Depth ? a : b).Copy();
                merged.Peak = Math.Max(a.Peak, b.Peak);
                merged.Implicit = a.Implicit || b.Implicit;
                merged.Terminated = a.Terminated && b.Terminated;
                merged.Uncertain = a.Uncertain || b.Uncertain || a.Depth != b.Depth || a.Implicit != b.Implicit || a.Terminated != b.Terminated;
                return merged;
            }
        }

        private static bool StartsImplicitTransaction(TSqlStatement statement)
        {
            if (statement is SelectStatement select)
            {
                var tables = new TableCollector();
                select.Accept(tables);
                return select.Into != null || tables.HasTable;
            }
            if (statement is DataModificationStatement || statement is TruncateTableStatement ||
                statement is FetchCursorStatement || statement is OpenCursorStatement || statement is ExecuteStatement)
                return true;
            if (statement is BeginEndBlockStatement || statement is IfStatement || statement is WhileStatement ||
                statement is TryCatchStatement || statement is WaitForStatement || statement is DbccStatement)
                return false;
            return !Classifier.IsReadOnlyStatement(statement) && Classifier.UnknownStatementNote(statement) == null;
        }

        /// <summary>Called once after the whole text: a transaction still open at the end is reported.</summary>
        public void FinishTransactions(Scope scope)
        {
            if (_transactions.Uncertain)
                AddAt(scope, scope.PositionOfOffset(0, 0), "TRANSACTION FLOW", string.Empty, null, Verdict.Unknown,
                    "transaction state differs between reachable paths or loop iterations; balance cannot be verified");
            if (_transactions.Depth <= 0 || !_transactions.OpenedAt.HasValue)
                return;

            var note = _transactions.OpenedImplicitly
                ? "IMPLICIT_TRANSACTIONS is ON: this opens a transaction that stays open after the script, holding its locks; nothing is permanent until COMMIT"
                : "BEGIN TRAN without a matching COMMIT or ROLLBACK: the transaction stays open after this runs, holding its locks; the changes are permanent only after COMMIT";
            AddAt(scope, _transactions.OpenedAt.Value, "OPEN TRANSACTION", string.Empty, null, Verdict.Unknown, note);
        }

        public StatementWalker(AnalyzerOptions options, List<Finding> findings, List<SqlParseError> errors, List<SkippedRegion> skipped)
        {
            _options = options;
            _findings = findings;
            _errors = errors;
            _skipped = skipped;
        }

        public void AnalyzeText(Scope scope)
        {
            var parser = ParserFactory.Create();
            var fragment = Parse(parser, scope.Text, out var errors);
            var tokens = fragment?.ScriptTokenStream ?? BatchRecovery.Tokenize(parser, scope.Text);
            CheckBatchRepetition(tokens, scope);
            var use = tokens.FirstOrDefault(token => token.TokenType == TSqlTokenType.Use);
            if (use != null) InvalidateTargetContext(scope, scope.PositionOfOffset(use.Offset, use.Text.Length));
            if (errors.Count == 0)
            {
                WalkScript(fragment, scope);
                return;
            }

            if (scope.IsDynamic)
            {
                ReportParseErrors(scope, errors);
                return;
            }

            // Parse each GO batch on its own so one broken batch does not hide the others, and recover the
            // statements around the errors inside a broken batch.
            foreach (var batch in SplitBatches(parser, scope.Text))
            {
                if (batch.Length == 0 || string.IsNullOrWhiteSpace(scope.Text.Substring(batch.Start, batch.Length)))
                    continue;

                var batchScope = scope.ForBatch(batch.Start, batch.Length, batch.LineBreaksBefore);
                var batchFragment = Parse(parser, batchScope.Text, out var batchErrors);
                if (batchErrors.Count == 0)
                    WalkScript(batchFragment, batchScope);
                else
                    RecoverBatch(parser, batchScope);
            }
        }

        /// <summary>
        /// A batch with syntax errors: SQL Server would run none of it. Its statements are still analyzed (around the
        /// broken parts) and labelled "won't run now"; the parts that could not be parsed are listed and scanned for
        /// write keywords.
        /// </summary>
        private void RecoverBatch(TSqlParser parser, Scope scope)
        {
            var recovery = BatchRecovery.Run(parser, scope.Text);
            ReportParseErrors(scope, recovery.Errors);
            var firstError = _errors
                .Where(e => e.Offset >= scope.OffsetDelta && e.Offset <= scope.OffsetDelta + scope.Text.Length)
                .OrderBy(e => e.Offset)
                .FirstOrDefault();
            var blocked = scope.Blocked(firstError?.Line ?? scope.LineDelta + 1);

            if (recovery.Fragment == null)
            {
                // Recovery gave up: the whole batch is unanalyzed.
                AddSkippedRegion(scope, blocked, 0, scope.Text.Length, "the batch could not be parsed");
                return;
            }

            // Positions in the recovered tree match the original text (blanking keeps every character's place).
            int before = _findings.Count;
            WalkScript(recovery.Fragment, blocked);

            // Drop anything found inside the blanked parts (e.g. the PRINT 0 placeholders).
            for (int i = _findings.Count - 1; i >= before; i--)
            {
                int relative = _findings[i].StartOffset - scope.OffsetDelta;
                if (recovery.Regions.Any(r => r.Contains(relative)))
                    _findings.RemoveAt(i);
            }

            foreach (var region in MergeAdjacent(scope.Text, recovery.Regions))
            {
                var module = region.Module;
                if (module == null)
                {
                    AddSkippedRegion(scope, blocked, region.Start, region.End - region.Start, "could not be parsed");
                    continue;
                }

                // CREATE PROCEDURE/FUNCTION/TRIGGER/VIEW with an error: it is a definition change, and everything after
                // it in the batch is its body (nothing there runs on its own), so the body is not scanned.
                var note = module.Misplaced
                    ? $"must be the first statement in its batch: add GO before it. Everything after it in this batch becomes its body"
                    : "has a syntax error; its body is not analyzed";
                var position = scope.PositionOfOffset(region.Start, region.End - region.Start);
                AddAt(blocked, position, module.Action, module.Name, null, Verdict.RealWrite, note);
                RecordSkipped(scope, region.Start, region.End - region.Start,
                    module.Misplaced ? module.Action + " is not the first statement in its batch" : module.Action + " could not be parsed");
            }
        }

        private void AddSkippedRegion(Scope scope, Scope blocked, int start, int length, string reason)
        {
            var region = RecordSkipped(scope, start, length, reason);
            var position = scope.PositionOfOffset(region.StartOffset - scope.OffsetDelta, region.Length);
            AddAt(scope, position, "NOT ANALYZED", region.Lines, null, Verdict.Unknown, reason);

            // Safety net: write keywords (outside comments and strings) in the text that could not be parsed.
            var parser = ParserFactory.Create();
            foreach (var token in BatchRecovery.Tokenize(parser, scope.Text.Substring(start, length)))
            {
                if (!WriteKeywords.Contains(token.TokenType))
                    continue;
                var keyword = token.Text.ToUpperInvariant() == "EXEC" ? "EXECUTE" : token.Text.ToUpperInvariant();
                AddAt(blocked, scope.PositionOfOffset(start + token.Offset, token.Text.Length), "possible " + keyword, string.Empty, null,
                    Verdict.Unknown, "in text that could not be parsed");
            }
        }

        private SkippedRegion RecordSkipped(Scope scope, int start, int length, string reason)
        {
            // Trim whitespace and comments at both ends (e.g. the header comment of the next example).
            int end = Math.Min(start + length, scope.Text.Length);
            var code = BatchRecovery.Tokenize(ParserFactory.Create(), scope.Text.Substring(start, end - start))
                .Where(t => t.TokenType != TSqlTokenType.WhiteSpace && t.TokenType != TSqlTokenType.SingleLineComment &&
                            t.TokenType != TSqlTokenType.MultilineComment && t.TokenType != TSqlTokenType.EndOfFile)
                .ToList();
            if (code.Count > 0)
            {
                end = start + code[code.Count - 1].Offset + code[code.Count - 1].Text.Length;
                start += code[0].Offset;
            }
            var first = scope.PositionOfOffset(start);
            var last = scope.PositionOfOffset(Math.Max(start, end - 1));
            var region = new SkippedRegion(first.Line, last.Line, first.Offset, end - start, reason);
            _skipped.Add(region);
            return region;
        }

        private static IEnumerable<BatchRecovery.Region> MergeAdjacent(string text, List<BatchRecovery.Region> regions)
        {
            BatchRecovery.Region current = null;
            foreach (var region in regions.OrderBy(r => r.Start))
            {
                if (current != null && current.Module == null && region.Module == null &&
                    string.IsNullOrWhiteSpace(text.Substring(current.End, Math.Max(0, region.Start - current.End))))
                {
                    current.End = Math.Max(current.End, region.End);
                    continue;
                }

                if (current != null)
                    yield return current;
                current = new BatchRecovery.Region(region.Start, region.End, region.Module);
            }

            if (current != null)
                yield return current;
        }

        public const string SqlCmdModeOffAction = "SQLCMD MODE OFF";

        /// <summary>
        /// SQLCMD mode is off but the script has SQLCMD lines (or $(variables) the parser rejected): one row that says
        /// why those batches fail and how to fix it.
        /// </summary>
        public void AddSqlCmdModeOffHint(Scope scope)
        {
            SqlCmdPreprocessor.FindSqlCmdSyntax(scope.Text, out var commands, out var variables);
            var rejectedVariable = variables.FirstOrDefault(v =>
                _errors.Any(e => e.Offset >= v.Offset && e.Offset <= v.Offset + v.Length));
            var first = commands.Concat(rejectedVariable != null ? new[] { rejectedVariable } : new SqlCmdPreprocessor.Directive[0])
                .OrderBy(d => d.Offset)
                .FirstOrDefault();
            if (first == null)
                return;

            AddAt(scope, scope.PositionOfOffset(first.Offset, first.Length), SqlCmdModeOffAction, string.Empty, null, Verdict.Unknown,
                "this looks like a SQLCMD script: turn on Query > SQLCMD Mode, or the batches with SQLCMD lines fail");
        }

        /// <summary>Adds rows for SQLCMD commands (they are handled by SSMS, not sent to the server).</summary>
        public void AddSqlCmdDirectives(Scope scope, IEnumerable<SqlCmdPreprocessor.Directive> directives, IReadOnlyDictionary<string, string> variables)
        {
            foreach (var directive in directives)
            {
                var position = scope.PositionOfOffset(directive.Offset, directive.Length);
                var args = directive.Arguments;
                switch (directive.Command)
                {
                    case ":connect":
                        InvalidateTargetContext(scope, position);
                        break;
                    case "$()":
                        AddAt(scope, position, "SQLCMD VARIABLE", args, null, Verdict.Unknown,
                            "SQLCMD substitution is not expanded: its executable meaning cannot be verified; values are considered only in definition order");
                        break;
                    case ":r":
                        AddAt(scope, position, ":r", args, null, Verdict.Unknown, "includes another script; it is not analyzed");
                        break;
                    case "!!":
                        AddAt(scope, position, "!!", args, null, Verdict.Unknown, "runs an operating-system command");
                        break;
                    case ":setvar":
                        AddAt(scope, position, ":setvar", args, null, Verdict.ReadOnly, "SQLCMD variable");
                        break;
                    case ":exit":
                        AddAt(scope, position, directive.Command, args, null, string.IsNullOrWhiteSpace(args) ? Verdict.ReadOnly : Verdict.Unknown,
                            string.IsNullOrWhiteSpace(args) ? "ends the script here" : "SQLCMD executes this exit query; it is not analyzed");
                        break;
                    case ":quit":
                        AddAt(scope, position, directive.Command, args, null, Verdict.ReadOnly, "ends the script here");
                        break;
                    default:
                        AddAt(scope, position, directive.Command, args, null, Verdict.ReadOnly, "SQLCMD command");
                        break;
                }
            }
        }

        private static TSqlFragment Parse(TSqlParser parser, string text, out IList<ParseError> errors)
            => AnalysisWork.Parse(parser, text, out errors);

        private void ReportParseErrors(Scope scope, IList<ParseError> errors)
        {
            if (errors.Count == 0)
                return;

            if (scope.IsDynamic)
            {
                var first = errors[0];
                Add(scope, null, "EXEC", "dynamic SQL", null, Verdict.Unknown,
                    "could not parse dynamic SQL: " + first.Message);
                return;
            }

            // "Incorrect syntax near end of file" points past trailing whitespace; move it to the last character.
            int lastCode = scope.Text.Length - 1;
            while (lastCode > 0 && char.IsWhiteSpace(scope.Text[lastCode]))
                lastCode--;

            foreach (var error in errors)
            {
                int line = error.Line, column = error.Column, offset = error.Offset;
                if (offset > lastCode && lastCode >= 0)
                {
                    offset = lastCode;
                    TextPosition.FromOffset(scope.Text, offset, out line, out column);
                }

                var position = scope.PositionOf(line, column, offset);
                _errors.Add(new SqlParseError(position.Line, position.Column, position.Offset, error.Number, error.Message));
                _findings.Add(new Finding(position.Line, position.Column, position.Offset, 0, "PARSE ERROR", string.Empty, null,
                    Verdict.Unknown, error.Message));
            }
        }

        private void WalkScript(TSqlFragment fragment, Scope scope)
        {
            if (!(fragment is TSqlScript script))
                return;
            foreach (var batch in script.Batches)
            {
                _transactions.Terminated = false;
                WalkStatements(batch.Statements, scope);
            }
        }

        private void InvalidateTargetContext(Scope scope, Position position)
        {
            _options = new AnalyzerOptions(tempdbIsTemp: _options.TempdbIsTemp, maxDynamicSqlDepth: _options.MaxDynamicSqlDepth,
                sqlCmdMode: _options.SqlCmdMode);
            AddAt(scope, position, "TARGET SWITCH", string.Empty, null, Verdict.Unknown,
                "USE or :connect changes the target within this script; connection rules describe only the initial connection; custom allow-lists and procedure trust are disabled");
        }

        private void CheckBatchRepetition(IList<TSqlParserToken> tokens, Scope scope)
        {
            if (tokens == null) return;
            for (int index = 0; index < tokens.Count; index++)
            {
                if (tokens[index].TokenType != TSqlTokenType.Go) continue;
                var repeat = tokens[index].Text.Substring(2).Trim();
                int next = index + 1;
                while (next < tokens.Count && tokens[next].TokenType == TSqlTokenType.WhiteSpace) next++;
                if (next < tokens.Count && tokens[next].Line == tokens[index].Line &&
                    tokens[next].TokenType == TSqlTokenType.Integer) repeat = tokens[next].Text;
                if (int.TryParse(repeat, out int count) && count != 1)
                    AddAt(scope, scope.PositionOfOffset(tokens[index].Offset, tokens[index].Text.Length), "BATCH REPETITION", repeat,
                        null, Verdict.Unknown, "GO repeats this batch: transaction nesting and effects across repetitions cannot be verified");
            }
        }

        private void WalkStatements(IEnumerable<TSqlStatement> statements, Scope scope)
        {
            if (statements == null)
                return;
            foreach (var statement in statements)
                WalkStatement(statement, scope);
        }

        private void WalkStatement(TSqlStatement statement, Scope scope)
        {
            var terminated = _transactions.Terminated ? _transactions.Copy() : null;
            WalkStatementCore(statement, scope);
            if (terminated != null) _transactions = terminated;
        }

        private void WalkStatementCore(TSqlStatement statement, Scope scope)
        {
            AnalysisWork.Check();
            if (statement == null)
                return;

            // With IMPLICIT_TRANSACTIONS ON, the first statement that touches data opens a transaction.
            if (_transactions.Implicit && _transactions.Depth == 0 && !scope.BlockingErrorLine.HasValue && StartsImplicitTransaction(statement))
                _transactions.Begin(scope.PositionOf(statement), implicitly: true);

            switch (statement)
            {
                // Control flow: recurse.
                case BeginEndBlockStatement block: // includes BEGIN ATOMIC blocks
                    WalkStatements(block.StatementList?.Statements, scope);
                    return;
                case IfStatement ifStatement:
                {
                    WalkIf(ifStatement, scope);
                    return;
                }
                case WhileStatement whileStatement:
                {
                    AddHiddenSideEffects(scope, whileStatement.Predicate, null);
                    var before = _transactions.Copy();
                    WalkStatement(whileStatement.Statement, scope);
                    _transactions = TransactionState.Merge(before, _transactions);
                    return;
                }
                case ReturnStatement returned:
                    Add(scope, returned, "RETURN", string.Empty, null, Verdict.ReadOnly, null);
                    _transactions.Terminated = true;
                    return;
                case ThrowStatement thrown:
                    Add(scope, thrown, "THROW", string.Empty, null, Verdict.ReadOnly, null);
                    _transactions.Terminated = true;
                    return;
                case BreakStatement _:
                case ContinueStatement _:
                case GoToStatement _:
                    Add(scope, statement, Classifier.ActionName(statement), string.Empty, null,
                        _transactions.Depth > 0 || _transactions.Implicit ? Verdict.Unknown : Verdict.ReadOnly,
                        _transactions.Depth > 0 || _transactions.Implicit ? "control transfer may skip transaction cleanup; balance cannot be verified" : null);
                    _transactions.Uncertain |= _transactions.Depth > 0 || _transactions.Implicit;
                    return;
                case TryCatchStatement tryCatch:
                {
                    // CATCH runs after TRY failed somewhere, possibly inside a transaction TRY opened.
                    var before = _transactions.Copy();
                    _transactions.Peak = _transactions.Depth;
                    WalkStatements(tryCatch.TryStatements?.Statements, scope);
                    var afterTry = _transactions.Copy();
                    _transactions = before.Copy();
                    if (afterTry.Peak > _transactions.Depth)
                    {
                        _transactions.Depth = afterTry.Peak;
                        _transactions.OpenedAt = afterTry.OpenedAt ?? afterTry.PeakOpenedAt;
                    }

                    WalkStatements(tryCatch.CatchStatements?.Statements, scope);

                    bool catchEnds = _transactions.Terminated;
                    if (catchEnds && (_transactions.Depth > 0 || _transactions.Uncertain))
                        Add(scope, tryCatch, "TRANSACTION FLOW", string.Empty, null, Verdict.Unknown,
                            "CATCH can exit before transaction cleanup; a transaction may remain open on that path");
                    _transactions = catchEnds ? afterTry : TransactionState.Merge(afterTry, _transactions);
                    return;
                }
                case BeginTransactionStatement begin:
                    Add(scope, begin, Classifier.ActionName(begin), string.Empty, null, Verdict.ReadOnly, null);
                    if (!scope.BlockingErrorLine.HasValue)
                    {
                        if (_transactions.Implicit && _transactions.Depth == 0) _transactions.Begin(scope.PositionOf(begin), implicitly: true);
                        _transactions.Begin(scope.PositionOf(begin));
                    }
                    return;
                case CommitTransactionStatement commit:
                    if (!scope.BlockingErrorLine.HasValue && _transactions.Depth == 0 && !_transactionGuard)
                    {
                        Add(scope, commit, Classifier.ActionName(commit), string.Empty, null, Verdict.Unknown,
                            "commits a transaction started outside this text: uncommitted changes from an earlier run become permanent");
                        return;
                    }

                    Add(scope, commit, Classifier.ActionName(commit), string.Empty, null, Verdict.ReadOnly, null);
                    if (!scope.BlockingErrorLine.HasValue)
                        _transactions.Commit();
                    return;
                case RollbackTransactionStatement rollback:
                    Add(scope, rollback, Classifier.ActionName(rollback), string.Empty, null, Verdict.ReadOnly, null);
                    if (!scope.BlockingErrorLine.HasValue)
                        _transactions.Rollback(rollback.Name?.Identifier?.Value ?? (rollback.Name?.ValueExpression as Literal)?.Value);
                    return;
                case SaveTransactionStatement save:
                    Add(scope, save, Classifier.ActionName(save), string.Empty, null, Verdict.ReadOnly, null);
                    if (!scope.BlockingErrorLine.HasValue)
                        _transactions.Save(save.Name?.Identifier?.Value ?? (save.Name?.ValueExpression as Literal)?.Value);
                    return;
                case PredicateSetStatement set when (set.Options & SetOptions.ImplicitTransactions) == SetOptions.ImplicitTransactions:
                    Add(scope, set, "SET", string.Empty, null, Verdict.ReadOnly,
                        set.IsOn ? "IMPLICIT_TRANSACTIONS ON: later changes stay uncommitted until COMMIT" : null);
                    if (!scope.BlockingErrorLine.HasValue)
                        _transactions.Implicit = set.IsOn;
                    return;
                case WaitForStatement waitFor when waitFor.Statement != null:
                    WalkStatement(waitFor.Statement, scope);
                    return;

                // Data manipulation.
                case SelectStatement select:
                    if (select.Into != null)
                        AddNamedWrite(scope, select, "SELECT INTO", select.Into, allowListApplies: true);
                    else
                        Add(scope, select, "SELECT", string.Empty, null, Verdict.ReadOnly, null);
                    AddHiddenSideEffects(scope, select, select);
                    return;
                case InsertStatement insert:
                    WalkInsert(insert, scope);
                    AddHiddenSideEffects(scope, insert, insert);
                    return;
                case UpdateStatement update:
                {
                    var spec = update.UpdateSpecification;
                    var resolver = new TargetResolver(update.WithCtesAndXmlNamespaces);
                    AddDmlWrite(scope, update, "UPDATE", spec.Target, resolver.ResolveWriteTarget(spec.Target, spec.FromClause),
                        new RowFilter(spec.WhereClause, spec.FromClause, spec.TopRowFilter));
                    AddOutputInto(scope, spec.OutputIntoClause);
                    AddHiddenSideEffects(scope, update, update);
                    return;
                }
                case DeleteStatement delete:
                {
                    var spec = delete.DeleteSpecification;
                    var resolver = new TargetResolver(delete.WithCtesAndXmlNamespaces);
                    AddDmlWrite(scope, delete, "DELETE", spec.Target, resolver.ResolveWriteTarget(spec.Target, spec.FromClause),
                        new RowFilter(spec.WhereClause, spec.FromClause, spec.TopRowFilter));
                    AddOutputInto(scope, spec.OutputIntoClause);
                    AddHiddenSideEffects(scope, delete, delete);
                    return;
                }
                case MergeStatement merge:
                {
                    var spec = merge.MergeSpecification;
                    var resolver = new TargetResolver(merge.WithCtesAndXmlNamespaces);
                    AddDmlWrite(scope, merge, "MERGE", spec.Target, resolver.ResolveWriteTarget(spec.Target, null));
                    AddOutputInto(scope, spec.OutputIntoClause);
                    AddHiddenSideEffects(scope, merge, merge);
                    return;
                }
                case TruncateTableStatement truncate:
                    AddNamedWrite(scope, truncate, "TRUNCATE TABLE", truncate.TableName, allowListApplies: true, allRows: true);
                    return;
                case BulkInsertBase bulkInsert:
                    AddNamedWrite(scope, bulkInsert, Classifier.ActionName(bulkInsert), bulkInsert.To, allowListApplies: true);
                    return;

                // Table / index DDL: temp objects are fine, everything else is a real change.
                case CreateTableStatement createTable:
                    AddNamedWrite(scope, createTable, "CREATE TABLE", createTable.SchemaObjectName, allowListApplies: false);
                    return;
                case AlterTableStatement alterTable:
                    AddNamedWrite(scope, alterTable, Classifier.ActionName(alterTable), alterTable.SchemaObjectName, allowListApplies: false);
                    return;
                case DropTableStatement dropTable:
                    foreach (var name in dropTable.Objects)
                        AddNamedWrite(scope, dropTable, "DROP TABLE", name, allowListApplies: false, allRows: true);
                    return;
                case CreateIndexStatement createIndex:
                    AddNamedWrite(scope, createIndex, "CREATE INDEX", createIndex.OnName, allowListApplies: false);
                    return;
                case CreateColumnStoreIndexStatement createColumnStore:
                    AddNamedWrite(scope, createColumnStore, "CREATE COLUMNSTORE INDEX", createColumnStore.OnName, allowListApplies: false);
                    return;
                case AlterIndexStatement alterIndex:
                    AddNamedWrite(scope, alterIndex, "ALTER INDEX", alterIndex.OnName, allowListApplies: false);
                    return;
                case DropIndexStatement dropIndex:
                    WalkDropIndex(dropIndex, scope);
                    return;
                case CreateStatisticsStatement createStatistics:
                    AddNamedWrite(scope, createStatistics, "CREATE STATISTICS", createStatistics.OnName, allowListApplies: false);
                    return;
                case DropStatisticsStatement dropStatistics:
                    foreach (var name in dropStatistics.Objects)
                    {
                        // table.statistics: everything but the last part names the table.
                        var ids = name.Identifiers;
                        bool temp = ids != null && ids.Count >= 2 && Classifier.IsTempName(ids[ids.Count - 2].Value);
                        Add(scope, dropStatistics, "DROP STATISTICS", scope.TextOf(name), null, temp ? Verdict.TempOnly : Verdict.RealWrite, null);
                    }

                    return;
                case DbccStatement dbcc:
                {
                    var dbccText = scope.TextOf(dbcc);
                    var dbccVerdict = Classifier.ClassifyDbcc(dbcc, out var dbccNote);
                    var written = System.Text.RegularExpressions.Regex.Match(dbccText, @"^DBCC\s+([A-Za-z_]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var dbccAction = "DBCC " + (written.Success ? written.Groups[1].Value.ToUpperInvariant() : dbcc.Command.ToString().ToUpperInvariant());
                    Add(scope, dbcc, dbccAction, string.Empty, null, dbccVerdict, dbccNote);
                    return;
                }

                case ExecuteStatement execute:
                    WalkExecute(execute.ExecuteSpecification, execute, scope, insertExec: false);
                    return;
            }

            if (Classifier.IsReadOnlyStatement(statement))
            {
                Add(scope, statement, Classifier.ActionName(statement), string.Empty, null, Verdict.ReadOnly, null);
                // DECLARE @x = ..., SET @x = (SELECT ...), cursors over remote queries, etc.
                AddHiddenSideEffects(scope, statement, null);
                return;
            }

            var unknownNote = Classifier.UnknownStatementNote(statement);
            if (unknownNote != null)
            {
                Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.Unknown, unknownNote);
                return;
            }

            // CREATE/ALTER/DROP of modules, security, databases, GRANT/REVOKE/DENY and anything unrecognized.
            // Module bodies are not walked: they do not run when the module is created.
            if (statement is ProcedureStatementBody procedureDefinition && Classifier.IsTemp(procedureDefinition.ProcedureReference?.Name, _options))
            {
                Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.TempOnly, "temporary procedure definition; the body is not run now");
                return;
            }
            if (statement is DropProcedureStatement dropProcedures)
            {
                foreach (var name in dropProcedures.Objects)
                    AddNamedWrite(scope, statement, "DROP PROCEDURE", name, allowListApplies: false, allRows: true);
                return;
            }
            var note = IsModuleDefinition(statement) ? "changes a definition; the body is not run now" : null;
            Add(scope, statement, Classifier.ActionName(statement), GuessTarget(statement, scope), null, Verdict.RealWrite, note);
        }

        /// <summary>
        /// Reports remote pass-through sources (Unknown) and NEXT VALUE FOR (RealWrite) inside a statement or
        /// condition. Not used for DDL: a DEFAULT (NEXT VALUE FOR ...) in CREATE TABLE does not run anything.
        /// </summary>
        private void AddHiddenSideEffects(Scope scope, TSqlFragment fragment, TSqlStatement writer)
        {
            SideEffectScanner.Scan(fragment, out var remoteSources, out var sequences);
            var nested = new ModificationCollector();
            fragment?.Accept(nested);
            var with = (writer as StatementWithCtesAndXmlNamespaces)?.WithCtesAndXmlNamespaces;
            foreach (var modification in nested.Specifications)
            {
                var resolver = new TargetResolver(with);
                FromClause from = (modification as UpdateSpecification)?.FromClause ?? (modification as DeleteSpecification)?.FromClause;
                var filter = modification is UpdateSpecification update ? new RowFilter(update.WhereClause, update.FromClause, update.TopRowFilter) :
                    modification is DeleteSpecification delete ? new RowFilter(delete.WhereClause, delete.FromClause, delete.TopRowFilter) : null;
                AddDmlWrite(scope, modification, modification is DeleteSpecification ? "DELETE" : modification is UpdateSpecification ? "UPDATE" :
                    modification is MergeSpecification ? "MERGE" : "INSERT", modification.Target, resolver.ResolveWriteTarget(modification.Target, from), filter);
                AddOutputInto(scope, modification.OutputIntoClause);
            }

            var written = writer != null ? SideEffectScanner.WrittenFragments(writer) : new HashSet<TSqlFragment>();
            foreach (var source in remoteSources)
            {
                if (!written.Contains(source.Reference))
                    Add(scope, source.Reference, source.Action, scope.TextOf(source.Reference), null, Verdict.Unknown, source.Note);
            }

            foreach (var sequence in sequences)
            {
                Add(scope, sequence, "NEXT VALUE FOR", scope.TextOf(sequence.SequenceName), null, Verdict.RealWrite,
                    "advances the sequence; ROLLBACK does not undo it");
            }
        }

        private void WalkInsert(InsertStatement insert, Scope scope)
        {
            var spec = insert.InsertSpecification;
            var resolver = new TargetResolver(insert.WithCtesAndXmlNamespaces);
            AddDmlWrite(scope, insert, "INSERT", spec.Target, resolver.ResolveWriteTarget(spec.Target, null));
            AddOutputInto(scope, spec.OutputIntoClause);

            if (spec.InsertSource is ExecuteInsertSource executeSource)
                WalkExecute(executeSource.Execute, insert, scope, insertExec: true);
        }

        private sealed class ModificationCollector : TSqlFragmentVisitor
        {
            public List<DataModificationSpecification> Specifications { get; } = new List<DataModificationSpecification>();
            public override void Visit(TSqlFragment node) => AnalysisWork.Check();
            public override void Visit(DataModificationTableReference node) => Specifications.Add(node.DataModificationSpecification);
        }

        private void WalkDropIndex(DropIndexStatement dropIndex, Scope scope)
        {
            foreach (var clause in dropIndex.DropIndexClauses)
            {
                switch (clause)
                {
                    case DropIndexClause modern:
                        AddNamedWrite(scope, dropIndex, "DROP INDEX", modern.Object, allowListApplies: false);
                        break;
                    case BackwardsCompatibleDropIndexClause legacy:
                    {
                        // DROP INDEX table.index: everything but the last part names the table.
                        var ids = legacy.Index?.Identifiers;
                        bool temp = ids != null && ids.Count >= 2 && Classifier.IsTempName(ids[ids.Count - 2].Value);
                        Add(scope, dropIndex, "DROP INDEX", scope.TextOf(legacy.Index), null, temp ? Verdict.TempOnly : Verdict.RealWrite, null);
                        break;
                    }
                    default:
                        Add(scope, dropIndex, "DROP INDEX", scope.TextOf(clause), null, Verdict.RealWrite, null);
                        break;
                }
            }
        }

        private void WalkExecute(ExecuteSpecification spec, TSqlStatement statement, Scope scope, bool insertExec)
        {
            var action = insertExec ? "INSERT ... EXEC" : "EXEC";
            if (spec == null)
            {
                Add(scope, statement, action, string.Empty, null, Verdict.Unknown, "could not analyze EXEC");
                return;
            }

            if (spec.LinkedServer != null)
            {
                Add(scope, statement, action, "AT " + spec.LinkedServer.Value, null, Verdict.Unknown, "runs on a linked server");
                return;
            }

            switch (spec.ExecutableEntity)
            {
                case ExecutableProcedureReference procedure:
                {
                    var reference = procedure.ProcedureReference;
                    if (reference?.ProcedureVariable != null || reference?.ProcedureReference?.Name == null)
                    {
                        Add(scope, statement, action, scope.TextOf(reference), null, Verdict.Unknown, "procedure name is in a variable");
                        return;
                    }

                    var name = reference.ProcedureReference.Name;
                    var baseName = name.BaseIdentifier?.Value ?? string.Empty;
                    var target = scope.TextOf(name);

                    bool system = IsSystemProcedure(name);
                    if (system && string.Equals(baseName, "sp_executesql", StringComparison.OrdinalIgnoreCase))
                    {
                        var statementParameter = procedure.Parameters.FirstOrDefault(p =>
                            p.Variable == null || string.Equals(p.Variable.Name, "@stmt", StringComparison.OrdinalIgnoreCase));
                        if (statementParameter?.ParameterValue is StringLiteral literal)
                            AnalyzeDynamicSql(literal.Value, statement, scope, action, target);
                        else
                            Add(scope, statement, action, target, null, Verdict.Unknown, "dynamic SQL is built at runtime");
                        return;
                    }

                    if (system && string.Equals(baseName, "sp_spaceused", StringComparison.OrdinalIgnoreCase) && UpdatesUsage(procedure))
                    {
                        Add(scope, statement, action, target, null, Verdict.RealWrite, "updates space usage metadata");
                        return;
                    }
                    if (system && Classifier.ReadOnlyProcedures.Contains(baseName))
                    {
                        Add(scope, statement, action, target, null, Verdict.ReadOnly, "read-only system procedure");
                        return;
                    }

                    if (system && Classifier.WritingProcedures.TryGetValue(baseName, out var writes))
                    {
                        Add(scope, statement, action, target, null, Verdict.RealWrite, "system procedure that " + writes);
                        return;
                    }

                    if (_options.TrustedProcedures.Matches(name.ServerIdentifier?.Value, name.DatabaseIdentifier?.Value,
                        name.SchemaIdentifier?.Value, baseName, _options.CurrentDatabase, _options.DefaultSchema))
                    {
                        Add(scope, statement, action, target, null, Verdict.ReadOnly, "trusted procedure");
                        return;
                    }
                    Add(scope, statement, action, target, null, Verdict.Unknown,
                        insertExec ? "the procedure may write" : "procedure body is not analyzed");
                    return;
                }

                case ExecutableStringList strings:
                {
                    if (strings.Strings.Count > 0 && strings.Strings.All(s => s is StringLiteral))
                    {
                        var sql = new StringBuilder();
                        foreach (StringLiteral literal in strings.Strings)
                            sql.Append(literal.Value);
                        AnalyzeDynamicSql(sql.ToString(), statement, scope, action, "dynamic SQL");
                    }
                    else
                    {
                        Add(scope, statement, action, "dynamic SQL", null, Verdict.Unknown, "dynamic SQL is built at runtime");
                    }

                    return;
                }

                default:
                    Add(scope, statement, action, scope.TextOf(spec.ExecutableEntity), null, Verdict.Unknown, "could not analyze EXEC");
                    return;
            }
        }

        private sealed class TableCollector : TSqlFragmentVisitor
        {
            public bool HasTable { get; private set; }
            public override void Visit(NamedTableReference node) { AnalysisWork.Check(); HasTable = true; }
        }

        private void WalkIf(IfStatement conditional, Scope scope)
        {
            var before = _transactions.Copy();
            var alternatives = new List<TransactionState>();
            bool reachable = true, outerGuard = _transactionGuard;
            while (conditional != null)
            {
                AnalysisWork.Check();
                AddHiddenSideEffects(scope, conditional.Predicate, null);
                bool? condition = EvaluateTransactionPredicate(conditional.Predicate, before.Depth, out bool guard);
                if (before.Uncertain) condition = null;
                _transactionGuard = outerGuard || guard;
                _transactions = before.Copy();
                WalkStatement(conditional.ThenStatement, scope);
                if (reachable && condition != false) alternatives.Add(_transactions);
                reachable &= condition != true;
                if (conditional.ElseStatement is IfStatement next) conditional = next;
                else
                {
                    _transactions = before.Copy();
                    WalkStatement(conditional.ElseStatement, scope);
                    if (reachable) alternatives.Add(_transactions);
                    conditional = null;
                }
            }
            _transactions = alternatives.Count == 0 ? before : alternatives.Aggregate(TransactionState.Merge);
            _transactionGuard = outerGuard;
        }

        private static bool? EvaluateTransactionPredicate(BooleanExpression expression, int depth, out bool guard)
        {
            guard = false;
            if (expression is BooleanParenthesisExpression parenthesis)
                return EvaluateTransactionPredicate(parenthesis.Expression, depth, out guard);
            if (expression is BooleanNotExpression negated)
            {
                var result = EvaluateTransactionPredicate(negated.Expression, depth, out guard);
                return result.HasValue ? !result.Value : (bool?)null;
            }
            if (!(expression is BooleanComparisonExpression comparison)) return null;
            var left = TransactionValue(comparison.FirstExpression, depth, out bool leftGuard);
            var right = TransactionValue(comparison.SecondExpression, depth, out bool rightGuard);
            guard = leftGuard || rightGuard;
            if (!guard || left == null || right == null) return null;
            bool? answer = null;
            foreach (int first in left)
                foreach (int second in right)
                {
                    bool value;
                    switch (comparison.ComparisonType)
                    {
                        case BooleanComparisonType.Equals: value = first == second; break;
                        case BooleanComparisonType.NotEqualToBrackets:
                        case BooleanComparisonType.NotEqualToExclamation: value = first != second; break;
                        case BooleanComparisonType.GreaterThan: value = first > second; break;
                        case BooleanComparisonType.GreaterThanOrEqualTo: value = first >= second; break;
                        case BooleanComparisonType.LessThan: value = first < second; break;
                        case BooleanComparisonType.LessThanOrEqualTo: value = first <= second; break;
                        default: return null;
                    }
                    if (answer.HasValue && answer.Value != value) return null;
                    answer = value;
                }
            return answer;
        }

        private static int[] TransactionValue(ScalarExpression expression, int depth, out bool guard)
        {
            guard = false;
            if (expression is GlobalVariableExpression variable && string.Equals(variable.Name, "@@TRANCOUNT", StringComparison.OrdinalIgnoreCase))
            { guard = true; return new[] { depth }; }
            if (expression is FunctionCall function && function.CallTarget == null && function.Parameters.Count == 0 &&
                string.Equals(function.FunctionName?.Value, "XACT_STATE", StringComparison.OrdinalIgnoreCase))
            { guard = true; return depth == 0 ? new[] { 0 } : new[] { -1, 1 }; }
            if (expression is IntegerLiteral integer && int.TryParse(integer.Value, out int value)) return new[] { value };
            return null;
        }

        private static bool IsSystemProcedure(SchemaObjectName name)
        {
            if (name.ServerIdentifier != null) return false;
            var schema = name.SchemaIdentifier?.Value;
            var database = name.DatabaseIdentifier?.Value;
            if (schema != null && !string.Equals(schema, "sys", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase)) return false;
            if (database == null) return true;
            if (string.Equals(database, "master", StringComparison.OrdinalIgnoreCase)) return schema != null;
            var procedure = name.BaseIdentifier?.Value ?? string.Empty;
            return string.Equals(database, "msdb", StringComparison.OrdinalIgnoreCase) && schema != null &&
                new[] { "sp_send_dbmail", "sp_delete_backuphistory", "sp_purge_jobhistory", "sp_help_job", "sp_help_jobstep", "sp_help_jobhistory", "sp_help_schedule",
                     "sp_add_job", "sp_update_job", "sp_delete_job", "sp_add_jobstep", "sp_update_jobstep", "sp_delete_jobstep",
                     "sp_add_jobschedule", "sp_add_schedule", "sp_attach_schedule", "sp_add_jobserver", "sp_start_job", "sp_stop_job" }
                     .Contains(procedure, StringComparer.OrdinalIgnoreCase);
        }

        private static bool UpdatesUsage(ExecutableProcedureReference procedure)
        {
            var parameter = procedure.Parameters.FirstOrDefault(value =>
                string.Equals(value.Variable?.Name, "@updateusage", StringComparison.OrdinalIgnoreCase));
            if (parameter == null && procedure.Parameters.Count > 1 && procedure.Parameters[1].Variable == null)
                parameter = procedure.Parameters[1];
            if (parameter == null) return false;
            var literal = parameter.ParameterValue as Literal;
            return literal == null || !(string.Equals(literal.Value, "false", StringComparison.OrdinalIgnoreCase) || literal.Value == "0");
        }

        private void AnalyzeDynamicSql(string sql, TSqlStatement executeStatement, Scope scope, string action, string target)
        {
            if (scope.DynamicDepth >= _options.MaxDynamicSqlDepth)
            {
                Add(scope, executeStatement, action, target, null, Verdict.Unknown, "dynamic SQL nested too deeply to analyze");
                return;
            }

            int before = _findings.Count;
            if (!string.IsNullOrWhiteSpace(sql))
                AnalyzeText(scope.ForDynamicSql(sql, executeStatement));

            if (_findings.Count == before)
                Add(scope, executeStatement, action, target, null, Verdict.ReadOnly, "empty dynamic SQL");
        }

        /// <summary>How an UPDATE/DELETE limits its rows; null for statements where that doesn't apply.</summary>
        private sealed class RowFilter
        {
            public RowFilter(WhereClause where, FromClause from, TopRowFilter top)
            {
                HasWhere = where != null;
                HasFrom = from != null;
                HasTop = top != null;
            }

            public bool HasWhere { get; }

            public bool HasFrom { get; }

            public bool HasTop { get; }
        }

        private void AddDmlWrite(Scope scope, TSqlFragment at, string action, TableReference target, List<BaseTarget> bases, RowFilter filter = null)
        {
            var written = TargetText(target, scope);
            foreach (var baseTarget in bases)
            {
                Verdict verdict;
                var notes = new List<string>();
                bool allRows = filter != null && !filter.HasWhere;
                if (filter != null && !filter.HasWhere)
                {
                    if (filter.HasTop)
                        notes.Add("no WHERE: limited only by TOP");
                    else if (filter.HasFrom)
                        notes.Add("no WHERE: rows limited only by the FROM/JOIN");
                    else if (baseTarget.Via.Count > 0)
                        notes.Add("no WHERE: rows limited only by the CTE or derived table");
                    else
                    {
                        notes.Add("no WHERE: affects every row");
                        allRows = true;
                    }
                }

                notes.AddRange(baseTarget.Via);
                string resolved = null;

                switch (baseTarget.Kind)
                {
                    case BaseTargetKind.Named:
                        verdict = Classifier.ClassifyWrite(baseTarget.Name, _options, allowListApplies: true);
                        resolved = scope.TextOf(baseTarget.Name);
                        if (baseTarget.Name.ServerIdentifier != null)
                            notes.Add("remote server " + baseTarget.Name.ServerIdentifier.Value);
                        break;
                    case BaseTargetKind.TableVariable:
                        verdict = Verdict.TempOnly;
                        resolved = scope.TextOf(baseTarget.Fragment);
                        break;
                    case BaseTargetKind.Remote:
                        verdict = Verdict.RealWrite;
                        break;
                    default:
                        verdict = Verdict.Unknown;
                        break;
                }

                if (!string.IsNullOrEmpty(baseTarget.Note))
                    notes.Add(baseTarget.Note);
                if (verdict == Verdict.AllowListed)
                    notes.Add("matches allow-list");

                if (string.Equals(resolved, written, StringComparison.OrdinalIgnoreCase))
                    resolved = null;

                AddAt(scope, scope.PositionOf(at), action, written, resolved, verdict, string.Join(", ", notes), allRows);
            }
        }

        private void AddOutputInto(Scope scope, OutputIntoClause outputInto)
        {
            if (outputInto?.IntoTable == null)
                return;
            var resolver = new TargetResolver(null);
            AddDmlWrite(scope, outputInto, "OUTPUT INTO", outputInto.IntoTable, resolver.ResolveWriteTarget(outputInto.IntoTable, null));
        }

        private void AddNamedWrite(Scope scope, TSqlFragment at, string action, SchemaObjectName name, bool allowListApplies, bool allRows = false)
        {
            var verdict = Classifier.ClassifyWrite(name, _options, allowListApplies);
            var notes = new List<string>();
            if (allRows)
                notes.Add("removes every row");
            if (verdict == Verdict.AllowListed)
                notes.Add("matches allow-list");
            AddAt(scope, scope.PositionOf(at), action, scope.TextOf(name), null, verdict, string.Join(", ", notes), allRows);
        }

        private void Add(Scope scope, TSqlFragment at, string action, string target, string resolved, Verdict verdict, string note) =>
            AddAt(scope, scope.PositionOf(at), action, target, resolved, verdict, note);

        private void AddAt(Scope scope, Position position, string action, string target, string resolved, Verdict verdict, string note,
            bool affectsAllRows = false)
        {
            var notes = new List<string>();
            if (scope.IsDynamic)
                notes.Add(DynamicSqlNote);
            if (!string.IsNullOrEmpty(note))
                notes.Add(note);
            if (scope.BlockingErrorLine.HasValue)
                notes.Add($"won't run now: this batch has a syntax error at line {scope.BlockingErrorLine.Value}");
            if (resolved == null && !action.StartsWith(":", StringComparison.Ordinal))
                resolved = scope.SubstituteVariables(target, position.Offset);

            _findings.Add(new Finding(position.Line, position.Column, position.Offset, position.Length, action, target, resolved, verdict,
                string.Join(", ", notes), willRun: !scope.BlockingErrorLine.HasValue, blockingErrorLine: scope.BlockingErrorLine,
                affectsAllRows: affectsAllRows));
        }

        private static string TargetText(TableReference target, Scope scope)
        {
            switch (target)
            {
                case NamedTableReference named:
                    return scope.TextOf(named.SchemaObject);
                case VariableTableReference variable:
                    return scope.TextOf(variable.Variable);
                default:
                    return scope.TextOf(target);
            }
        }

        private static bool IsModuleDefinition(TSqlStatement statement) =>
            statement is ProcedureStatementBodyBase || statement is TriggerStatementBody || statement is ViewStatementBody;

        private static string GuessTarget(TSqlStatement statement, Scope scope)
        {
            switch (statement)
            {
                case ProcedureStatementBody procedure when procedure.ProcedureReference?.Name != null:
                    return scope.TextOf(procedure.ProcedureReference.Name);
                case DropObjectsStatement drop when drop.Objects != null:
                    return string.Join(", ", drop.Objects.Select(scope.TextOf));
            }

            foreach (var propertyName in new[] { "Name", "SchemaObjectName", "OnName", "DatabaseName", "To" })
            {
                var property = statement.GetType().GetProperty(propertyName);
                if (property == null)
                    continue;

                object value;
                try
                {
                    value = property.GetValue(statement);
                }
                catch (Exception)
                {
                    continue;
                }

                switch (value)
                {
                    case SchemaObjectName schemaObjectName:
                        return scope.TextOf(schemaObjectName);
                    case Identifier identifier:
                        return identifier.Value;
                    case IdentifierOrValueExpression identifierOrValue:
                        return identifierOrValue.Value;
                }
            }

            return string.Empty;
        }

        private struct Batch
        {
            public Batch(int start, int length, int lineBreaksBefore)
            {
                Start = start;
                Length = length;
                LineBreaksBefore = lineBreaksBefore;
            }

            public int Start { get; }

            public int Length { get; }

            public int LineBreaksBefore { get; }
        }

        /// <summary>Splits text at GO separators using the T-SQL tokenizer (so GO inside comments or strings is ignored).</summary>
        private static List<Batch> SplitBatches(TSqlParser parser, string text)
        {
            var tokens = BatchRecovery.Tokenize(parser, text);

            var batches = new List<Batch>();
            int start = 0;
            int lineBreaksBefore = 0;
            foreach (var token in tokens)
            {
                if (token.TokenType != TSqlTokenType.Go)
                    continue;

                batches.Add(new Batch(start, Math.Max(0, token.Offset - start), lineBreaksBefore));

                // The next batch starts on the line after GO (GO may be followed by a count or a comment).
                int next = token.Offset;
                while (next < text.Length && text[next] != '\r' && text[next] != '\n')
                    next++;
                if (next < text.Length && text[next] == '\r')
                    next++;
                if (next < text.Length && text[next] == '\n')
                    next++;

                start = next;
                lineBreaksBefore = token.Line;
            }

            batches.Add(new Batch(start, text.Length - start, lineBreaksBefore));
            return batches;
        }
    }
}
