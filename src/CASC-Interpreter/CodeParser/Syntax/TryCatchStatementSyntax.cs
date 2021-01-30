namespace CASC.CodeParser.Syntax
{
    internal sealed class TryCatchStatementSyntax : StatementSyntax
    {
        public TryCatchStatementSyntax(SyntaxTree syntaxTree,
                                       SyntaxToken tryKeyword,
                                       StatementSyntax tryBody,
                                       SyntaxToken catchKeyword,
                                       StatementSyntax catchBody)
                                       : base(syntaxTree)
        {
            TryKeyword = tryKeyword;
            TryBody = tryBody;
            CatchKeyword = catchKeyword;
            CatchBody = catchBody;
        }

        public override SyntaxKind Kind => SyntaxKind.TryCatchStatement;
        public SyntaxToken TryKeyword { get; }
        public StatementSyntax TryBody { get; }
        public SyntaxToken CatchKeyword { get; }
        public StatementSyntax CatchBody { get; }
    }
}