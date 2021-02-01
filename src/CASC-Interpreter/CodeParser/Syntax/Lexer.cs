using CASC.CodeParser.Utilities;
using System.Collections.Generic;
using CASC.CodeParser.Text;
using System.Text;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Syntax
{
    internal sealed class Lexer
    {
        private readonly DiagnosticPack _diagnostics = new DiagnosticPack();
        private readonly SyntaxTree _syntaxTree;
        private readonly SourceText _source;
        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;

        public Lexer(SyntaxTree syntaxTree)
        {
            _syntaxTree = syntaxTree;
            _source = syntaxTree.Source;
        }

        public DiagnosticPack Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _source.Length)
                return '\0';

            return _source[index];
        }

        public SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;

                case '加':
                case '正':
                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                case '減':
                case '負':
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                case '乘':
                case '*':
                    _kind = SyntaxKind.StarToken;
                    _position++;
                    break;
                case '除':
                case '/':
                    _kind = SyntaxKind.SlashToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenthesisToken;
                    _position++;
                    break;
                case '[':
                    _kind = SyntaxKind.OpenBracketToken;
                    _position++;
                    break;
                case ']':
                    _kind = SyntaxKind.CloseBracketToken;
                    _position++;
                    break;
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position++;
                    break;
                case ':':
                    _kind = SyntaxKind.ColonToken;
                    _position++;
                    break;
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.TildeToken;
                    _position++;
                    break;
                case '^':
                    _kind = SyntaxKind.HatToken;
                    _position++;
                    break;
                case '且':
                    _kind = SyntaxKind.AmpersandAmpersandToken;
                    _position++;
                    break;
                case '&':
                    _position++;
                    if (Current != '&')
                        _kind = SyntaxKind.AmpersandToken;
                    else
                    {
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position++;
                    }
                    break;
                case '或':
                    _kind = SyntaxKind.PipePipeToken;
                    _position++;
                    break;
                case '|':
                    _position++;
                    if (Current != '|')
                        _kind = SyntaxKind.PipeToken;
                    else
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position++;
                    }
                    break;
                case '反':
                    _kind = SyntaxKind.BangToken;
                    _position++;
                    break;
                case '!':
                    if (LookAhead == '=')
                    {
                        _kind = SyntaxKind.BangEqualsToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.BangToken;
                    _position++;
                    break;
                case '不':
                    if (LookAhead == '是')
                    {
                        _kind = SyntaxKind.BangEqualsToken;
                        _position += 2;
                        break;
                    }
                    break;
                case '是':
                    _kind = SyntaxKind.EqualsEqualsToken;
                    _position++;
                    break;
                case '=':
                    if (LookAhead != '=')
                        goto case '賦';
                    _kind = SyntaxKind.EqualsEqualsToken;
                    _position += 2;
                    break;
                case '賦':
                    _kind = SyntaxKind.EqualsToken;
                    _position++;
                    break;
                case '大':
                    _position++;
                    if (Current == '等' && LookAhead == '於')
                    {
                        _kind = SyntaxKind.GreaterEqualsToken;
                        _position += 2;
                    }
                    else if (Current == '於')
                    {
                        _kind = SyntaxKind.GreaterToken;
                        _position++;
                    }
                    break;
                case '小':
                    _position++;
                    if (Current == '等' && LookAhead == '於')
                    {
                        _kind = SyntaxKind.LessEqualsToken;
                        _position += 2;
                    }
                    else if (Current == '於')
                    {
                        _kind = SyntaxKind.LessToken;
                        _position++;
                    }
                    break;
                case '>':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.GreaterToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.GreaterEqualsToken;
                        _position++;
                    }
                    break;
                case '<':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.LessToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.LessEqualsToken;
                        _position++;
                    }
                    break;

                case '"':
                    ReadString();
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '零':
                case '一':
                case '二':
                case '三':
                case '四':
                case '五':
                case '六':
                case '七':
                case '八':
                case '九':
                case '壹':
                case '貳':
                case '參':
                case '肆':
                case '伍':
                case '陆':
                case '柒':
                case '捌':
                case '玖':
                case '拾':
                case '十':
                case '百':
                case '千':
                case '萬':
                case '億':
                    ReadNumber();
                    break;

                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    ReadWhiteSpace();
                    break;

                default:
                    if (char.IsLetter(Current))
                        ReadIdentifierOrKeyword();
                    else if (char.IsWhiteSpace(Current))
                        ReadWhiteSpace();
                    else
                    {
                        var span = new TextSpan(_position, 1);
                        var location = new TextLocation(_source, span);
                        _diagnostics.ReportBadCharacter(location, Current);
                        _position++;
                    }
                    break;
            }

            var length = _position - _start;
            var text = _source.ToString(_start, length);

            // var text = SyntaxFacts.GetText(_kind);
            // if (text == null)
            //     text = _text.ToString(_start, length);

            return new SyntaxToken(_syntaxTree, _kind, _start, text, _value);
        }

        private void ReadString()
        {
            _position++;

            var builder = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '"':
                        _position++;
                        done = true;
                        break;

                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(_start, 1);
                        var location = new TextLocation(_source, span);
                        _diagnostics.ReportUnterminatedString(location);
                        done = true;
                        break;

                    case '\\':
                        _position++;

                        switch (Current)
                        {
                            case '"':
                                builder.Append(Current);
                                _position++;
                                break;

                            default:
                                builder.Append('\\');
                                break;
                        }
                        break;

                    default:
                        builder.Append(Current);
                        _position++;
                        break;
                }
            }

            _kind = SyntaxKind.StringToken;
            _value = builder.ToString();
        }

        private void ReadNumber()
        {
            while (ChineseParser.isDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _source.ToString(_start, length);
            if (!ChineseParser.tryParseDigits(text, out var value))
            {
                var span = new TextSpan(_start, length);
                var location = new TextLocation(_source, span);
                _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Number);
            }

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.WhiteSpaceToken;
        }

        private string ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                _position++;

            var length = _position - _start;
            var text = _source.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
            return text;
        }
    }

}