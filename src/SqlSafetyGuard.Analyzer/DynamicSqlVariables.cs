using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Analyzer
{
    /// <summary>
    /// Proves string inputs to EXEC from straight-line, batch-local DECLARE/SET assignments.
    /// This is deliberately not an interpreter: control flow, conversions, truncation and runtime
    /// inputs discard proof. Every other statement is a barrier, including procedure OUTPUT values,
    /// SELECT/UPDATE assignments and cursor FETCH. Dynamic batches get independent maps.
    /// </summary>
    internal sealed class DynamicSqlVariables
    {
        private const int MaxTextLength = 1000000;
        private readonly Dictionary<ExecutableEntity, string> _inputs = new Dictionary<ExecutableEntity, string>();
        private readonly Dictionary<ExecutableEntity, DynamicSqlSourceMap> _sources = new Dictionary<ExecutableEntity, DynamicSqlSourceMap>();
        private readonly Dictionary<ExecuteParameter, StringValue> _parameters = new Dictionary<ExecuteParameter, StringValue>();
        private readonly Dictionary<string, StringType> _types = new Dictionary<string, StringType>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _spellings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, StringValue> _values = new Dictionary<string, StringValue>(StringComparer.OrdinalIgnoreCase);

        private sealed class StringType
        {
            public bool Unicode;
            public int Limit; // int.MaxValue means (max).
        }

        private sealed class StringValue
        {
            public string Text;
            public bool Unicode;
            public bool Max;
            public DynamicSqlSourceMap Source;
        }

        public bool TryGet(ExecutableEntity executable, out string sql) => _inputs.TryGetValue(executable, out sql);
        public DynamicSqlSourceMap SourceOf(ExecutableEntity executable) => _sources.TryGetValue(executable, out var source) ? source : null;
        public bool TryGetParameter(ExecuteParameter parameter, out string text, out DynamicSqlSourceMap source)
        {
            text = null; source = null;
            if (parameter == null || !_parameters.TryGetValue(parameter, out var value)) return false;
            text = value.Text; source = value.Source; return true;
        }

        public static DynamicSqlVariables Collect(IEnumerable<TSqlStatement> statements)
        {
            var result = new DynamicSqlVariables();
            var flat = new List<TSqlStatement>();
            var pending = new Stack<TSqlStatement>((statements ?? Enumerable.Empty<TSqlStatement>()).Reverse());
            while (pending.Count > 0)
            {
                AnalysisWork.Check();
                var statement = pending.Pop();
                if (statement is BeginEndBlockStatement block)
                {
                    if (block.StatementList != null)
                        foreach (var child in block.StatementList.Statements.Reverse()) pending.Push(child);
                }
                else if (statement is IfStatement || statement is WhileStatement || statement is TryCatchStatement ||
                         statement is GoToStatement || statement is LabelStatement || statement is ReturnStatement ||
                         statement is BreakStatement || statement is ContinueStatement || statement is WaitForStatement)
                {
                    // Even an EXEC textually before a backwards jump can run again with a different value.
                    // Do not infer execution order from source order in any batch with control flow.
                    return result;
                }
                else flat.Add(statement);
            }

            foreach (var statement in flat)
            {
                AnalysisWork.Check();
                if (statement is DeclareVariableStatement declaration)
                {
                    foreach (var variable in declaration.Declarations)
                    {
                        var name = variable.VariableName.Value;
                        result.Forget(name);
                        // A differently cased spelling is ambiguous across database collations.
                        if (result._types.ContainsKey(name)) result._types[name] = null;
                        else
                        {
                            result._types[name] = GetType(variable.DataType);
                            result._spellings[name] = name;
                        }
                        result.Assign(name, result.Evaluate(variable.Value));
                    }
                }
                else if (statement is SetVariableStatement assignment)
                {
                    var name = assignment.Variable?.Name;
                    var value = result.Evaluate(assignment.Expression);
                    if (assignment.AssignmentKind == AssignmentKind.AddEquals)
                        value = result.Join(result.Evaluate(assignment.Variable), value);
                    else if (assignment.AssignmentKind != AssignmentKind.Equals)
                        value = null;
                    // SET @v.Property / @v.Method(...) is not a scalar assignment.
                    if (assignment.Identifier != null || assignment.FunctionCallExists) value = null;
                    result.Assign(name, value);
                }
                else
                {
                    if (statement is ExecuteStatement execute)
                        result.Capture(execute.ExecuteSpecification?.ExecutableEntity);
                    // No assumption survives a statement we do not model. Keep declarations so a
                    // later supported SET can establish a fresh value without inheriting old text.
                    result._values.Clear();
                }
            }
            return result;
        }

        private void Capture(ExecutableEntity executable)
        {
            if (executable is ExecutableStringList strings && strings.Strings.Count > 0)
            {
                var parts = strings.Strings.Select(Evaluate).ToList();
                if (parts.Any(part => part == null)) return;
                // EXEC's string list is distinct from an ordinary scalar + expression. Only join
                // small lists; a single (max) variable can carry a larger exact value unchanged.
                int length = 0;
                foreach (var part in parts)
                {
                    AnalysisWork.Check(part.Text.Length + 1);
                    if (part.Text.Length > MaxTextLength - length) return;
                    length += part.Text.Length;
                }
                if (parts.Count > 1 && length > 4000) return;
                var sql = new StringBuilder(length);
                foreach (var part in parts) sql.Append(part.Text);
                _inputs[executable] = sql.ToString();
                _sources[executable] = DynamicSqlSourceMap.Join(parts.Select(p => p.Source));
            }
            else if (executable is ExecutableProcedureReference procedure)
            {
                foreach (var argument in procedure.Parameters)
                {
                    var argumentValue = Evaluate(argument.ParameterValue);
                    if (argumentValue != null) _parameters[argument] = argumentValue;
                }
                var parameter = procedure.Parameters.FirstOrDefault(p => p.Variable == null ||
                    string.Equals(p.Variable.Name, "@stmt", StringComparison.OrdinalIgnoreCase));
                var value = Evaluate(parameter?.ParameterValue);
                if (value != null)
                {
                    AnalysisWork.Check(value.Text.Length + 1);
                    _inputs[executable] = value.Text;
                    _sources[executable] = value.Source;
                }
            }
        }

        private static StringType GetType(DataTypeReference dataType)
        {
            if (!(dataType is SqlDataTypeReference sql) ||
                (sql.SqlDataTypeOption != SqlDataTypeOption.NVarChar && sql.SqlDataTypeOption != SqlDataTypeOption.VarChar)) return null;
            bool unicode = sql.SqlDataTypeOption == SqlDataTypeOption.NVarChar;
            int limit = 1; // DECLARE's omitted length defaults to 1, unlike CAST.
            if (sql.Parameters.Count == 1)
            {
                if (sql.Parameters[0] is MaxLiteral) limit = int.MaxValue;
                else if (!int.TryParse(sql.Parameters[0].Value, out limit) || limit < 1 || limit > (unicode ? 4000 : 8000)) return null;
            }
            else if (sql.Parameters.Count != 0) return null;
            return new StringType { Unicode = unicode, Limit = limit };
        }

        private void Forget(string name)
        {
            _values.Remove(name);
        }

        private void Assign(string name, StringValue value)
        {
            if (name == null) { _values.Clear(); return; }
            // Exact spelling must agree with the declaration. A CI lookup alone could preserve a
            // stale value on one collation and update a different variable on another.
            Forget(name);
            if (!_spellings.TryGetValue(name, out var declared) || declared != name ||
                !_types.TryGetValue(name, out var type) || type == null || value == null) return;
            if (value.Text.Length > type.Limit || (!type.Unicode && !IsAscii(value.Text))) return;
            _values[name] = new StringValue { Text = value.Text, Unicode = type.Unicode, Max = type.Limit == int.MaxValue, Source = value.Source };
        }

        private StringValue Evaluate(ScalarExpression expression, int depth = 0)
        {
            AnalysisWork.Check();
            if (depth > 128) return null;
            if (expression is StringLiteral literal)
            {
                AnalysisWork.Check(literal.Value.Length + 1);
                if (literal.Value.Length > MaxTextLength || (!literal.IsNational && !IsAscii(literal.Value))) return null;
                return new StringValue { Text = literal.Value, Unicode = literal.IsNational,
                    Max = literal.Value.Length > (literal.IsNational ? 4000 : 8000), Source = DynamicSqlSourceMap.FromLiteral(literal) };
            }
            if (expression is VariableReference variable)
                return _spellings.TryGetValue(variable.Name, out var declared) && declared == variable.Name &&
                    _values.TryGetValue(variable.Name, out var value) ? value : null;
            if (expression is ParenthesisExpression parenthesis)
                return Evaluate(parenthesis.Expression, depth + 1);
            if (expression is BinaryExpression binary && binary.BinaryExpressionType == BinaryExpressionType.Add)
                return Join(Evaluate(binary.FirstExpression, depth + 1), Evaluate(binary.SecondExpression, depth + 1));
            return null;
        }

        private StringValue Join(StringValue left, StringValue right)
        {
            if (left == null || right == null) return null;
            bool unicode = left.Unicode || right.Unicode, max = left.Max || right.Max;
            // Do not guess at code-page conversion or mixed-type intermediate truncation.
            if (left.Unicode != right.Unicode &&
                ((!left.Unicode && left.Text.Length > 4000) || (!right.Unicode && right.Text.Length > 4000))) return null;
            int limit = max ? MaxTextLength : unicode ? 4000 : 8000;
            if (left.Text.Length > limit - right.Text.Length) return null;
            AnalysisWork.Check(left.Text.Length + right.Text.Length + 1);
            return new StringValue { Text = left.Text + right.Text, Unicode = unicode, Max = max,
                Source = DynamicSqlSourceMap.Join(new[] { left.Source, right.Source }) };
        }

        private static bool IsAscii(string text)
        {
            foreach (char character in text)
            {
                AnalysisWork.Check();
                if (character > 127) return false;
            }
            return true;
        }
    }
}
