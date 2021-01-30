namespace CASC.CodeParser.Syntax
{
    internal class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxTree syntaxTree,
                                    SyntaxToken keyword,
                                    ExpressionSyntax condition,
                                    StatementSyntax body)
                                    : base(syntaxTree)
        {
            Keyword = keyword;
            Condition = condition;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
        public SyntaxToken Keyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }
    }
}