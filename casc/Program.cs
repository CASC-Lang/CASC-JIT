using System;
using System.Linq;
using System.Collections.Generic;

namespace casc
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;

                if (!syntaxTree.Diagnotics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnotic in syntaxTree.Diagnotics)
                        Console.WriteLine(diagnotic);

                    Console.ForegroundColor = color;
                }
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken T && T != null)
            {
                Console.Write(" ");
                Console.Write(T.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }

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
        BinaryToken,
        EndOfFileToken,
        BadToken
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    class Lexer
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
            var chineseParser = new TraditionalChineseParser(Current);

            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if (chineseParser.isDigit())
            {
                var start = _position;

                while (new TraditionalChineseParser(Current).isDigit())
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!chineseParser.tryParseChineseDigits(out var value))
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
            else if (Current == '-' || Current == '減')
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

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {
    }

    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken token)
        {
            NumberToken = token;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberToken;

        public SyntaxToken NumberToken { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryToken;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnotics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnotics = diagnotics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnotics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
    }

    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private List<string> _diagnotics = new List<string>();
        private int _position;

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }

            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnotics.AddRange(lexer.Diagnotics);
        }

        public IEnumerable<string> Diagnotics => _diagnotics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnotics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnotics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.PlusToken ||
                   Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }

    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is NumberExpressionSyntax N)
            {
                return (int)N.NumberToken.Value;
            }
            if (node is BinaryExpressionSyntax B)
            {
                var left = EvaluateExpression(B.Left);
                var right = EvaluateExpression(B.Right);

                if (B.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (B.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (B.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                if (B.OperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                    throw new Exception($"ERROR: Unexpected Binary Operator {B.OperatorToken.Kind}.");
            }

            throw new Exception($"ERROR: Unexpected Node {node.Kind}.");
        }
    }

    abstract class ChineseParser
    {
        public abstract char Character { get; }
        public abstract bool isDigit();
        public abstract bool tryParseChineseDigits(out int value);
    }

    sealed class TraditionalChineseParser : ChineseParser
    {

        public TraditionalChineseParser(char character)
        {
            Character = character;
        }

        public override char Character { get; }

        public override bool isDigit()
        {
            if (char.IsDigit(Character))
                return true;
            else
                switch (Character)
                {
                    case '零':
                        return true;
                    case '一':
                        return true;
                    case '二':
                        return true;
                    case '三':
                        return true;
                    case '四':
                        return true;
                    case '五':
                        return true;
                    case '六':
                        return true;
                    case '七':
                        return true;
                    case '八':
                        return true;
                    case '九':
                        return true;
                    default:
                        return false;
                }
        }

        public override bool tryParseChineseDigits(out int value)
        {
            if (int.TryParse(Character.ToString(), out value))
                return true;
            else
                switch (Character)
                {
                    case '零':
                        value = 0;
                        return true;
                    case '一':
                        value = 1;
                        return true;
                    case '二':
                        value = 2;
                        return true;
                    case '三':
                        value = 3;
                        return true;
                    case '四':
                        value = 4;
                        return true;
                    case '五':
                        value = 5;
                        return true;
                    case '六':
                        value = 6;
                        return true;
                    case '七':
                        value = 7;
                        return true;
                    case '八':
                        value = 8;
                        return true;
                    case '九':
                        value = 9;
                        return true;
                    default:
                        value = -1;
                        return false;
                }
        }
    }
}
