namespace CASC.CodeParser.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
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
    }
}