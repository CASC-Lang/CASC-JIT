namespace CASC.CodeParser.Syntax
{
    enum SyntaxKind
    {
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
        BadToken
    }
}