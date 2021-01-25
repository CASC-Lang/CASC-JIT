using CASC.CodeParser.Binding;
using System.Collections.Generic;
using System;

namespace CASC.CodeParser
{
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)statement);
                    break;
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    break;
                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                    break;
                default:
                    throw new Exception($"ERROR: Unexpected Node {statement.Kind}.");
            };
        }

        private void EvaluateBlockStatement(BoundBlockStatement statements)
        {
            foreach (var statement in statements.Statements)
                EvaluateStatement(statement);
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