namespace CASC.CodeParser.Syntax
{
    public sealed class ArrayExpressionSyntax : ExpressionSyntax
    {
        public ArrayExpressionSyntax(SyntaxTree syntaxTree,
                                     SyntaxToken openBracket,
                                     SeparatedSyntaxList<ExpressionSyntax> contents,
                                     SyntaxToken closeBracket)
                                     : base(syntaxTree)
        {
            OpenBracket = openBracket;
            Contents = contents;
            CloseBracket = closeBracket;
        }

        public override SyntaxKind Kind => SyntaxKind.ArrayExpression;
        public SyntaxToken OpenBracket { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Contents { get; }
        public SyntaxToken CloseBracket { get; }
    }
}