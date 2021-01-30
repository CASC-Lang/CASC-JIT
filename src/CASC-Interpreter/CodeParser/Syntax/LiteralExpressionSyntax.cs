namespace CASC.CodeParser.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxTree syntaxTree,
                                       SyntaxToken token)
                                       : this(syntaxTree,
                                              token,
                                              token.Value)
        {

        }
        public LiteralExpressionSyntax(SyntaxTree syntaxTree,
                                       SyntaxToken token,
                                       object value)
                                       : base(syntaxTree)
        {
            NumberToken = token;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken NumberToken { get; }
        public object Value { get; }
    }
}