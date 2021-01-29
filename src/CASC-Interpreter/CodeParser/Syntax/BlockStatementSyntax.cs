using System.Collections.Immutable;

namespace CASC.CodeParser.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceSyntax)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceSyntax = closeBraceSyntax;
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceSyntax { get; }
    }
}