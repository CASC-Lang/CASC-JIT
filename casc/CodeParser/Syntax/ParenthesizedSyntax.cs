using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    sealed class ParenthesizedSyntax : ExpressionSyntax
    {
        public ParenthesizedSyntax(SyntaxToken openParentheizedToken, ExpressionSyntax expression, SyntaxToken closeParentheizedToken)
        {
            OpenParentheizedToken = openParentheizedToken;
            Expression = expression;
            CloseParentheizedToken = closeParentheizedToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedToken;
        public SyntaxToken OpenParentheizedToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParentheizedToken { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParentheizedToken;
            yield return Expression;
            yield return CloseParentheizedToken;
        }
    }
}