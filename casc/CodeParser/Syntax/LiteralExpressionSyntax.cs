using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken token) : this(token, token.Value)
        {

        }
        public LiteralExpressionSyntax(SyntaxToken token, object value)
        {
            NumberToken = token;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken NumberToken { get; }
        public object Value { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}