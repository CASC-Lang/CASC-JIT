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
        ParenthesizedToken,
        BinaryToken,
        EndOfFileToken,
        BadToken,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}