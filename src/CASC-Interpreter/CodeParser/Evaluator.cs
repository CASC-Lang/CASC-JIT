using System.Runtime.Serialization;
using System.Linq;
using CASC.CodeParser.Binding;
using System.Collections.Generic;
using System;
using CASC.CodeParser.Symbols;
using System.Collections.Immutable;

namespace CASC.CodeParser
{
    internal sealed class Evaluator
    {
        private readonly BoundProgram _program;
        private readonly Dictionary<VariableSymbol, object> _globals;
        private readonly Dictionary<FunctionSymbol, BoundBlockStatement> _functions = new Dictionary<FunctionSymbol, BoundBlockStatement>();
        private readonly Stack<Dictionary<VariableSymbol, object>> _locals = new Stack<Dictionary<VariableSymbol, object>>();
        private Random _random;

        private object _lastValue;

        public Evaluator(BoundProgram program, Dictionary<VariableSymbol, object> variables)
        {
            _program = program;
            _globals = variables;
            _locals.Push(new Dictionary<VariableSymbol, object>());
            var current = program;

            while (current != null)
            {
                foreach (var kv in current.Functions)
                {
                    var function = kv.Key;
                    var body = kv.Value;
                    _functions.Add(function, body);
                }

                current = current.Previous;
            }
        }

        public object Evaluate()
        {
            var function = _program.MainFunction ?? _program.ScriptFunction;

            if (function == null)
                return null;

            var body = _functions[function];

            return EvaluateStatement(body);
        }

        private object EvaluateStatement(BoundBlockStatement body)
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i + 1);
            }

            var index = 0;

            while (index < body.Statements.Length)
            {
                var s = body.Statements[index];

                switch (s.Kind)
                {
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;

                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;

                    case BoundNodeKind.GotoStatement:
                        var gs = (BoundGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;

                    case BoundNodeKind.ConditionalGotoStatement:
                        var cgs = (BoundConditionalGotoStatement)s;
                        var condition = (bool)EvaluateExpression(cgs.Condition);

                        if (condition == cgs.JumpIfTrue)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;

                        break;

                    case BoundNodeKind.LabelStatement:
                        index++;
                        break;

                    case BoundNodeKind.ReturnStatement:
                        var rs = (BoundReturnStatement)s;
                        var returnValue = rs.Expression == null ? null : EvaluateExpression(rs.Expression);
                        return returnValue;

                    default:
                        throw new Exception($"Unexpected node {s.Kind}");
                }
            }

            return _lastValue;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            _lastValue = EvaluateExpression(statement.Expression);
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
        {
            var value = EvaluateExpression(statement.Initializer);
            _lastValue = value;
            Assign(statement.Variable, value);
        }

        private object EvaluateExpression(BoundExpression node) => node.Kind switch
        {
            BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)node),
            BoundNodeKind.ArrayExpression => EvaluateArrayExpression((BoundArrayExpression)node),
            BoundNodeKind.VariableExpression => EvaluateVariableExpression((BoundVariableExpression)node),
            BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression)node),
            BoundNodeKind.ArrayAssignmentExpression => EvaluateArrayAssignmentExpression((BoundArrayAssignmentExpression)node),
            BoundNodeKind.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression)node),
            BoundNodeKind.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression)node),
            BoundNodeKind.CallExpression => EvaluateCallExpression((BoundCallExpression)node),
            BoundNodeKind.ConversionExpression => EvaluateConversionExpression((BoundConversionExpression)node),
            _ => throw new Exception($"ERROR: Unexpected Node {node.Kind}.")
        };

        private object EvaluateLiteralExpression(BoundLiteralExpression l)
        {
            return l.Value;
        }

        private object EvaluateArrayExpression(BoundArrayExpression node)
        {
            var arrayBuilder = new List<object>();
            var contents = node.Contents;

            foreach (var content in contents)
            {
                var evaluatedContent = EvaluateExpression(content);

                arrayBuilder.Add(evaluatedContent);
            }

            return arrayBuilder;
        }

        private object EvaluateIndexExpression(BoundIndexExpression i)
            => i.Contents.Select(expression => Convert.ToInt32(EvaluateExpression(expression))).ToImmutableArray();

        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            object value;

            if (v.Variable.Kind == SymbolKind.GlobalVariable)
                value = _globals[v.Variable];
            else
            {
                var locals = _locals.Peek();

                value = locals[v.Variable];
            }

            if (v.IndexClause != null)
            {
                var indexes = EvaluateIndexExpression(v.IndexClause);

                if (indexes is ImmutableArray<int> index)
                {
                    for (int i = 0; i < index.Length; i++)
                    {
                        if (value is List<object> array) value = array[index[i]];
                    }

                    return value;
                }
                else throw new Exception($"Error: Variable '{v.Variable.Name}' with type '{v.Variable.Type}' cannot be index.");
            }
            else return value;
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            Assign(a.Variable, value);

            return value;
        }

        private object EvaluateArrayAssignmentExpression(BoundArrayAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            var indexes = EvaluateIndexExpression(a.Indexes);
            AssignIndex(a.Variable, indexes, value);

            return value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (decimal)operand;

                case BoundUnaryOperatorKind.Negation:
                    return -(decimal)operand;

                case BoundUnaryOperatorKind.OnesComplement:
                    return Convert.ToDecimal(~Convert.ToInt32(operand));

                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;

                default:
                    throw new Exception($"ERROR: Unexpected unary operator {u.Op}");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if (b.Type == TypeSymbol.Number)
                        return (decimal)left + (decimal)right;
                    else
                        return (string)left + (string)right;

                case BoundBinaryOperatorKind.Subtraction:
                    return (decimal)left - (decimal)right;

                case BoundBinaryOperatorKind.Multiplication:
                    return (decimal)left * (decimal)right;

                case BoundBinaryOperatorKind.Division:
                    return (decimal)left / (decimal)right;

                case BoundBinaryOperatorKind.BitwiseAND:
                    if (b.Type == TypeSymbol.Number)
                        return Convert.ToDecimal(Convert.ToInt32(left) & Convert.ToInt32(right));
                    else
                        return (bool)left & (bool)right;

                case BoundBinaryOperatorKind.BitwiseOR:
                    if (b.Type == TypeSymbol.Number)
                        return Convert.ToDecimal(Convert.ToInt32(left) | Convert.ToInt32(right));
                    else
                        return (bool)left | (bool)right;

                case BoundBinaryOperatorKind.BitwiseXOR:
                    if (b.Type == TypeSymbol.Number)
                        return Convert.ToDecimal(Convert.ToInt32(left) ^ Convert.ToInt32(right));
                    else
                        return (bool)left ^ (bool)right;

                case BoundBinaryOperatorKind.LogicalAND:
                    return (bool)left && (bool)right;

                case BoundBinaryOperatorKind.LogicalOR:
                    return (bool)left || (bool)right;

                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);

                case BoundBinaryOperatorKind.GreaterEquals:
                    return (decimal)left >= (decimal)right;

                case BoundBinaryOperatorKind.Greater:
                    return (decimal)left > (decimal)right;

                case BoundBinaryOperatorKind.LessEquals:
                    return (decimal)left <= (decimal)right;

                case BoundBinaryOperatorKind.Less:
                    return (decimal)left < (decimal)right;

                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);

                default:
                    throw new Exception($"ERROR: Unexpected Binary Operator {b.Op}.");
            }
        }

        private object EvaluateCallExpression(BoundCallExpression node)
        {
            if (node.Function == BuiltinFunctions.Input)
                return Console.ReadLine();

            if (node.Function == BuiltinFunctions.Print)
            {
                var value = IO.Formatter.Format(EvaluateExpression(node.Arguments[0]));
                Console.Write(value);

                return null;
            }

            if (node.Function == BuiltinFunctions.Println)
            {
                var value = IO.Formatter.Format(EvaluateExpression(node.Arguments[0]));
                Console.WriteLine(value);

                return null;
            }

            if (node.Function == BuiltinFunctions.Type)
            {
                var value = EvaluateExpression(node.Arguments[0]);

                return value switch
                {
                    decimal d => "number",
                    string s => "string",
                    List<object> l => "array",
                    bool b => "bool",
                    _ => "any"
                };
            }

            if (node.Function == BuiltinFunctions.Random)
            {
                var min = (decimal)EvaluateExpression(node.Arguments[0]);
                var max = (decimal)EvaluateExpression(node.Arguments[1]);

                if (_random == null)
                    _random = new Random();

                return _random.Next(Convert.ToInt32(min), Convert.ToInt32(max));
            }

            var locals = new Dictionary<VariableSymbol, object>();

            for (int i = 0; i < node.Arguments.Length; i++)
            {
                var parameter = node.Function.Parameters[i];
                var value = EvaluateExpression(node.Arguments[i]);

                locals.Add(parameter, value);
            }

            _locals.Push(locals);

            var statement = _functions[node.Function];
            object result;

            try
            {
                result = EvaluateStatement(statement);
            }
            finally
            {
                _locals.Pop();
            }

            return result;
        }

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);

            if (node.Type == TypeSymbol.Any)
                return value;
            else if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            else if (node.Type == TypeSymbol.Number)
                return Convert.ToDecimal(value);
            else if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);
            else
                throw new Exception($"ERROR: Unexpected type {node.Type}");
        }

        private void Assign(VariableSymbol variable, object value)
        {
            if (variable.Kind == SymbolKind.GlobalVariable)
                _globals[variable] = value;
            else
            {
                var locals = _locals.Peek();
                locals[variable] = value;
            }
        }

        private void AssignIndex(VariableSymbol variable, object indexes, object value)
        {
            object array = null;

            if (variable.Kind == SymbolKind.GlobalVariable)
            {
                array = _globals[variable];
            }
            else
            {
                var locals = _locals.Peek();
                array = locals[variable];
            }

            if (array is List<object> list && indexes is ImmutableArray<int> index)
            {
                if (index.Length == 1) list[index[0]] = value;
                else
                {
                    object temp = null;

                    for (var i = 0; i < index.Length; i++)
                    {
                        if (temp == null)
                        {
                            temp = list[index[i]];
                        }
                        else if (temp is List<object> t)
                        {
                            if (!(t[index[i]] is List<object>)) t[index[i]] = value;
                            else temp = t[index[i]];
                        }
                        else throw new Exception($"Error: Variable '{variable.Name}' with type '{variable.Type}' cannot be index.");
                    }
                }

                if (variable.Kind == SymbolKind.GlobalVariable)
                {
                    _globals[variable] = array;
                }
                else
                {
                    var locals = _locals.Peek();
                    locals[variable] = array;
                }
            }
        }
    }
}