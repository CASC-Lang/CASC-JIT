namespace CASC.CodeParser.Syntax {
    public class IndexExpressionSyntax : ExpressionSyntax {
        public IndexExpressionSyntax(SyntaxTree syntaxTree,
                                     SyntaxToken openBracket,
                                     SeparatedSyntaxList<ExpressionSyntax> contents,
                                     SyntaxToken closeBracket)
                                     : base(syntaxTree)
        {
            OpenBracket = openBracket;
            Contents = contents;
            CloseBracket = closeBracket;
        }

        public override SyntaxKind Kind => SyntaxKind.IndexExpression;

        public SyntaxToken OpenBracket { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Contents { get; }
        public SyntaxToken CloseBracket { get; }
    }
}