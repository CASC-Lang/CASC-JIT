using CASC.CodeParser.Utils;
using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    internal sealed class Lexer
    {
        private static readonly List<char> _exceptionChineseChar = new List<char> {
            '且',
            '或'
        };

        private readonly string _text;
        private List<string> _diagnostics = new List<string>();
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (_position >= _text.Length)
                return '\0';

            return _text[_position];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if (ChineseParser.isDigit(Current))
            {
                var start = _position;

                while (ChineseParser.isDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!ChineseParser.tryParseDigits(text, out var value))
                {
                    _diagnostics.Add($"ERROR: The Number {_text} isn't valid Int32.");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (char.IsLetter(Current) && !_exceptionChineseChar.Contains(Current))
            {
                var start = _position;

                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '加':
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '減':
                case '負':
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '乘':
                case '*':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '除':
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '開':
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case '閉':
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '不':
                    if (LookAhead == '是')
                        return new SyntaxToken(SyntaxKind.BangToken, _position += 2, "!", null);
                    break;
                case '!':
                    return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
                case '&':
                    if (LookAhead == '&')
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position += 2, "&&", null);
                    break;
                case '且':
                    return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position++, "&&", null);
                case '|':
                    if (LookAhead == '|')
                        return new SyntaxToken(SyntaxKind.PipePipeToken, _position += 2, "||", null);
                    break;
                case '或':
                    return new SyntaxToken(SyntaxKind.PipePipeToken, _position++, "||", null);
            }

            _diagnostics.Add($"ERROR: Bad Character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

}