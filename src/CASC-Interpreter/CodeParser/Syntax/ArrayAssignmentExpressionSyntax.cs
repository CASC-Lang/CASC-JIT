namespace CASC.CodeParser.Syntax
{
    public sealed class ArrayAssignmentExpressionSyntax : AssignmentExpressionSyntax
    {
        public ArrayAssignmentExpressionSyntax(SyntaxTree syntaxTree,
                                               SyntaxToken identifierToken,
                                               SyntaxToken greaterToken,
                                               IndexExpressionSyntax indexSyntax,
                                               SyntaxToken equalsToken,
                                               ExpressionSyntax expression)
                                               : base(syntaxTree,
                                                      identifierToken,
                                                      equalsToken,
                                                      expression)
        {
            GreaterToken = greaterToken;
            IndexSyntax = indexSyntax;
        }

        public override SyntaxKind Kind => SyntaxKind.ArrayAssignmentExpression;
        public SyntaxToken GreaterToken { get; }
        public IndexExpressionSyntax IndexSyntax { get; }
    }
}