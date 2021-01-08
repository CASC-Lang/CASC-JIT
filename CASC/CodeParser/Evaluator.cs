using CASC.CodeParser.Binding;
using System.Collections.Generic;
using System;

namespace CASC.CodeParser
{
    internal sealed class Evaluator
    {
        private readonly Dictionary<VariableSymbol, object> _variables;
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
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
                    return ToDouble(operand);
                case BoundUnaryOperatorKind.Negation:
                    return -ToDouble(operand);
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;

                case BoundUnaryOperatorKind.Square:
                    return ToDouble(operand) * ToDouble(operand);
                case BoundUnaryOperatorKind.SquareRoot:
                    return Math.Sqrt(ToDouble(operand));
                case BoundUnaryOperatorKind.NthRoot:
                    goto case BoundUnaryOperatorKind.SquareRoot;

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
                    return ToDouble(left) + ToDouble(right);
                case BoundBinaryOperatorKind.Subtraction:
                    return ToDouble(left) - ToDouble(right);
                case BoundBinaryOperatorKind.Multiplication:
                    return ToDouble(left) * ToDouble(right);
                case BoundBinaryOperatorKind.Division:
                    return ToDouble(left) / ToDouble(right);
                case BoundBinaryOperatorKind.Point:
                    return float.Parse($"{Convert.ToInt64(left)}.{Convert.ToInt64(right)}");
                case BoundBinaryOperatorKind.LogicalAND:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOR:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);

                case BoundBinaryOperatorKind.Power:
                    return (double)Math.Pow(ToDouble(left), ToDouble(right));
                case BoundBinaryOperatorKind.NthRoot:
                    return (double)Math.Pow(ToDouble(left), (double)1 / Convert.ToInt64(right));

                default:
                    throw new Exception($"ERROR: Unexpected Binary Operator {b.Op}.");
            }
        }

        private double ToDouble(object value) => Convert.ToDouble(value);
    }
}