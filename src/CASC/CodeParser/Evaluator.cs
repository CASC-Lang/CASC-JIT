using CASC.CodeParser.Binding;
using System.Collections.Generic;
using System;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser
{
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < _root.Statements.Length; i++)
            {
                if (_root.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i + 1);
            }

            var index = 0;

            while (index < _root.Statements.Length)
            {
                var s = _root.Statements[index];

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
            _variables[statement.Variable] = value;
            _lastValue = value;
        }

        private object EvaluateExpression(BoundExpression node)
        {
            return node.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)node),
                BoundNodeKind.VariableExpression => EvaluateVariableExpression((BoundVariableExpression)node),
                BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression)node),
                BoundNodeKind.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression)node),
                BoundNodeKind.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression)node),
                _ => throw new Exception($"ERROR: Unexpected Node {node.Kind}.")
            };
        }

        private object EvaluateLiteralExpression(BoundLiteralExpression l)
        {
            return l.Value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            return _variables[v.Variable];
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
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
                    return (decimal)left + (decimal)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (decimal)left - (decimal)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (decimal)left * (decimal)right;
                case BoundBinaryOperatorKind.Division:
                    return (decimal)left / (decimal)right;
                case BoundBinaryOperatorKind.BitwiseAND:
                    if (b.Type == (typeof(decimal)))
                        return Convert.ToDecimal(Convert.ToInt32(left) & Convert.ToInt32(right));
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorKind.BitwiseOR:
                    if (b.Type == (typeof(decimal)))
                        return Convert.ToDecimal(Convert.ToInt32(left) | Convert.ToInt32(right));
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorKind.BitwiseXOR:
                    if (b.Type == (typeof(decimal)))
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
    }
}