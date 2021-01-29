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
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        CommaToken,
        ColonToken,
        BangToken,
        TildeToken,
        HatToken,
        AmpersandAmpersandToken,
        AmpersandToken,
        PipePipeToken,
        PipeToken,
        EqualsEqualsToken,
        BangEqualsToken,
        EqualsToken,
        GreaterEqualsToken,
        GreaterToken,
        LessEqualsToken,
        LessToken,
        IdentifierToken,
        StringToken,
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
        DoKeyword,
        ForKeyword,
        ToKeyword,
        FunctionKeyword,
        TryKeyword,
        CatchKeyword,

        // Nodes
        CompilationUnit,
        ElseClause,
        TypeClause,
        Parameter,
        GlobalStatement,
        FunctionDeclaration,

        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        TryCatchStatement,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression,
        CallExpression
    }
}