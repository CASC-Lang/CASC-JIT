namespace CASC.CodeParser.Syntax
{
    public sealed class ArrayExpressionSyntax : IndexExpressionSyntax
    {
        public ArrayExpressionSyntax(SyntaxTree syntaxTree,
                                     SyntaxToken openBracket,
                                     SeparatedSyntaxList<ExpressionSyntax> contents,
                                     SyntaxToken closeBracket)
                                     : base(syntaxTree, openBracket, contents, closeBracket)
        {
        }

        public override SyntaxKind Kind => SyntaxKind.ArrayExpression;
    }
}