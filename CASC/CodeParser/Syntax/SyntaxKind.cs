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
        PointToken,
        OpenParenthesesToken,
        CloseParenthesesToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        ParenthesizedToken,
        EqualsEqualsToken,
        BangEqualsToken,
        EqualsToken,
        BinaryToken,
        IdentifierToken,
        EndOfFileToken,
        BadToken,

        // Special Chinese Token
        SquareToken,
        SquareRootToken,
        NthRootToken,
        PowerToken,

        // Keywords
        TrueKeyword,
        FalseKeyword,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression
    }
}