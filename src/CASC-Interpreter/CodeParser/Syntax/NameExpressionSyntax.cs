namespace CASC.CodeParser.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxTree syntaxTree,
                                    SyntaxToken identifierToken,
                                    IndexExpressionSyntax indexClause = null)
                                    : base(syntaxTree)
        {
            IdentifierToken = identifierToken;
            IndexClause = indexClause;
        }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken IdentifierToken { get; }
        public IndexExpressionSyntax IndexClause { get; }
    }
}