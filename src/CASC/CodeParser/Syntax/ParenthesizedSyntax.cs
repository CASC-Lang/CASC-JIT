namespace CASC.CodeParser.Syntax
{
    public sealed class ParenthesizedSyntax : ExpressionSyntax
    {
        public ParenthesizedSyntax(SyntaxToken openParentheizedToken, ExpressionSyntax expression, SyntaxToken closeParentheizedToken)
        {
            OpenParentheizedToken = openParentheizedToken;
            Expression = expression;
            CloseParentheizedToken = closeParentheizedToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public SyntaxToken OpenParentheizedToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParentheizedToken { get; }
    }
}