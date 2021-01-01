using System.Diagnostics;
using System;
using CASC.CodeParser.Utils;
using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    internal sealed class Lexer
    {
        private static readonly List<char> _exceptionChineseChar = new List<char> {
            // Common Operators
            '加', // Add                加
            '減', // Minus              減
            '乘', // Multiply           乘
            '除', // Divide             除
            '開', // OpenParenthesis    開
            '閉', // CloseParenthesis   閉
            '正', // Positive           正
            '負', // Negation           負
            '且', // Logical AND        且
            '或', // Logical OR         或
            '反', // Logical Negation   反
            '是', // Eqauls             是
            '不', // Not Equals         不是

            // Special Operators Only Exists In Chinse
            '平', // Square     Square Root 平方    平方根
            '方', // nth Root               開方
            '次'  // Power                  次方
        };

        private readonly string _text;
        private DiagnosticPack _diagnostics = new DiagnosticPack();
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticPack Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (_position >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            var start = _position;

            if (ChineseParser.isDigit(Current))
            {
                while (ChineseParser.isDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!ChineseParser.tryParseDigits(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (char.IsLetter(Current) && !_exceptionChineseChar.Contains(Current))
            {
                while (char.IsLetter(Current) && !_exceptionChineseChar.Contains(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '加':
                case '正':
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '減':
                case '負':
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '乘':
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '除':
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '開':
                    if (LookAhead == '方')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.NthRootToken, start, "√", null);
                    }
                    goto case '(';
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case '閉':
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '且':
                    return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position++, "&&", null);
                case '&':
                    if (LookAhead == '&')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                    }
                    break;
                case '或':
                    return new SyntaxToken(SyntaxKind.PipePipeToken, _position++, "||", null);
                case '|':
                    if (LookAhead == '|')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                    }
                    break;
                case '反':
                    return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
                case '!':
                    if (LookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                    }
                    else
                        return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
                case '不':
                    if (LookAhead == '是')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                    }
                    break;
                case '是':
                    return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position++, "==", null);
                case '=':
                    if (LookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                    else
                        return new SyntaxToken(SyntaxKind.EqualsToken, _position++, "=", null);


                // Special Operators Only Exists In Chinese
                case '平':
                    if (LookAhead == '方')
                    {
                        if (Peek(2) == '根')
                        {
                            _position += 3;
                            return new SyntaxToken(SyntaxKind.SquareRootToken, start, "2√", null);
                        }

                        _position += 2;
                        return new SyntaxToken(SyntaxKind.SquareToken, start, "^2", null);
                    }
                    break;
                case '次':
                    if (LookAhead == '方')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.PowerToken, start, "^^", null);
                    }
                    break;
            }

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

}