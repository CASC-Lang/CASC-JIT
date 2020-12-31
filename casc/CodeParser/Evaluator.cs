using CASC.CodeParser.Syntax;
using System;

namespace CASC.CodeParser
{
    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is NumberExpressionSyntax N)
            {
                return (int)N.NumberToken.Value;
            }
            if (node is BinaryExpressionSyntax B)
            {
                var left = EvaluateExpression(B.Left);
                var right = EvaluateExpression(B.Right);

                if (B.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (B.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (B.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                if (B.OperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                    throw new Exception($"ERROR: Unexpected Binary Operator {B.OperatorToken.Kind}.");
            }

            if (node is ParenthesizedSyntax P)
                return EvaluateExpression(P.Expression);

            throw new Exception($"ERROR: Unexpected Node {node.Kind}.");
        }
    }
}