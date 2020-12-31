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

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression N)
            {
                return (int)N.Value;
            }

            if (node is BoundUnaryExpression U)
            {
                var operand = EvaluateExpression(U.Operand);

                if (U.OperatorKind == BoundUnaryOperatorKind.Identity)
                    return operand;
                else if (U.OperatorKind == BoundUnaryOperatorKind.Negation)
                    return -operand;
                else
                    throw new Exception($"ERROR: Unexpected unary operator {U.OperatorKind}");
            }

            if (node is BoundBinaryExpression B)
            {
                var left = EvaluateExpression(B.Left);
                var right = EvaluateExpression(B.Right);

                switch (B.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return left * right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    default:
                        throw new Exception($"ERROR: Unexpected Binary Operator {B.OperatorKind}.");
                }
            }

            throw new Exception($"ERROR: Unexpected Node {node.Kind}.");
        }
    }
}