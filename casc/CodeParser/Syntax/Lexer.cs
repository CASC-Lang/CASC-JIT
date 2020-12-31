using CASC.CodeParser.Utils;
using System.Collections.Generic;

namespace CASC.CodeParser.Syntax {
    internal sealed class Lexer
    {
        private readonly string _text;
        private List<string> _diagnotics = new List<string>();
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnotics => _diagnotics;

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';

                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
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
                    _diagnotics.Add($"ERROR: The Number {_text} isn't valid Int32.");
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

            if (Current == '+' || Current == '加')
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            else if (Current == '-' || Current == '減' || Current == '負')
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            else if (Current == '*' || Current == '乘')
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            else if (Current == '/' || Current == '除')
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            else if (Current == '(' || Current == '開')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            else if (Current == ')' || Current == '閉')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

            _diagnotics.Add($"ERROR: Bad Character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

}