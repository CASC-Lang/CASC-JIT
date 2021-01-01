using CASC.CodeParser.Binding;
using CASC.CodeParser.Syntax;
using System;

namespace CASC.CodeParser
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            this._root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression N)
            {
                return N.Value;
            }

            if (node is BoundUnaryExpression U)
            {
                var operand = EvaluateExpression(U.Operand);

                switch (U.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;

                    case BoundUnaryOperatorKind.Square:
                        return (int)operand * (int)operand;
                    case BoundUnaryOperatorKind.SquareRoot:
                        return Math.Sqrt((int)operand);
                    case BoundUnaryOperatorKind.NthRoot:
                        goto case BoundUnaryOperatorKind.SquareRoot;

                    default:
                        throw new Exception($"ERROR: Unexpected unary operator {U.Op}");
                }
            }

            if (node is BoundBinaryExpression B)
            {
                var left = EvaluateExpression(B.Left);
                var right = EvaluateExpression(B.Right);

                switch (B.Op.Kind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.Division:
                        return (int)left / (int)right;
                    case BoundBinaryOperatorKind.LogicalAND:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOR:
                        return (bool)left || (bool)right;
                    case BoundBinaryOperatorKind.Equals:
                        return Equals(left, right);
                    case BoundBinaryOperatorKind.NotEquals:
                        return !Equals(left, right);

                    case BoundBinaryOperatorKind.Power:
                        return (float) Math.Pow((int)left, (int)right);
                    case BoundBinaryOperatorKind.NthRoot:
                        return (float) Math.Pow((int)left, (float)1 / (int)right);

                    default:
                        throw new Exception($"ERROR: Unexpected Binary Operator {B.Op}.");
                }
            }

            throw new Exception($"ERROR: Unexpected Node {node.Kind}.");
        }
    }
}