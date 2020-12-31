using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken token)
        {
            NumberToken = token;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberToken;

        public SyntaxToken NumberToken { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}