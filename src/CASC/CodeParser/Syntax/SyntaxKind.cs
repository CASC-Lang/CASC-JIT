namespace CASC.CodeParser.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesesToken,
        CloseParenthesesToken,
        OpenBraceToken,
        CloseBraceToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,
        EqualsToken,
        GreaterEqualsToken,
        GreaterToken,
        LessEqualsToken,
        LessToken,
        IdentifierToken,
        EndOfFileToken,
        BadToken,

        // Keywords
        TrueKeyword,
        FalseKeyword,
        LetKeyword,
        VarKeyword,
        ValKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,

        // Nodes
        CompilationUnit,
        ElseClause,

        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression
    }
}