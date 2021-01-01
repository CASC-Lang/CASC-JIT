namespace CASC.CodeParser.Syntax
{
    enum SyntaxKind
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
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        ParenthesizedToken,
        EqualsEqualsToken,
        BangEqualsToken,
        BinaryToken,
        IdentifierToken,
        EndOfFileToken,
        BadToken,

        // Keywords
        TrueKeyword,
        FalseKeyword,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}