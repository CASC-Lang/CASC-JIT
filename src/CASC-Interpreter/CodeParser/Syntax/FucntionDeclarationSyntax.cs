namespace CASC.CodeParser.Syntax
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public FunctionDeclarationSyntax(SyntaxTree syntaxTree,
                                         SyntaxToken functionKeyword,
                                         SyntaxToken identifier,
                                         SyntaxToken openParentheizedToken,
                                         SeparatedSyntaxList<ParameterSyntax> parameters,
                                         SyntaxToken closeParentheizedToken,
                                         TypeClauseSyntax type,
                                         BlockStatementSyntax body)
                                         : base(syntaxTree)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            OpenParentheizedToken = openParentheizedToken;
            Parameters = parameters;
            CloseParentheizedToken = closeParentheizedToken;
            Type = type;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParentheizedToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParentheizedToken { get; }
        public TypeClauseSyntax Type { get; }
        public BlockStatementSyntax Body { get; }
    }
}