namespace CASC.CodeParser.Syntax
{
    public sealed class ParenthesizedSyntax : ExpressionSyntax
    {
        public ParenthesizedSyntax(SyntaxTree syntaxTree,
                                   SyntaxToken openParentheizedToken,
                                   ExpressionSyntax expression,
                                   SyntaxToken closeParentheizedToken)
                                   : base(syntaxTree)
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