using CASC.CodeParser.Syntax;
using CASC.CodeParser;
using System.Collections.Generic;
using System.Linq;
using System;
using CASC.CodeParser.Symbols;
using CASC.IO;

namespace CASC
{
    public sealed class CASCRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree;
        private bool _showProgram;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);

            foreach (var token in tokens)
            {
                var isKeyword = token.Kind.ToString().EndsWith("Keyword");
                var isNumber = token.Kind == SyntaxKind.NumberToken;
                var isIdentifier = token.Kind == SyntaxKind.IdentifierToken;
                var isString = token.Kind == SyntaxKind.StringToken;

                if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isIdentifier)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (isNumber)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (isString)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }

        protected override void EvalCommand(string input)
        {
            switch (input)
            {
                case "/showTree":
                    _showTree = !_showTree;
                    break;
                case "/showProgram":
                    _showProgram = !_showProgram;
                    break;
                case "/clear":
                    Console.Clear();
                    break;
                case "/reset":
                    _previous = null;
                    _variables.Clear();
                    break;
                default:
                    base.EvalCommand(input);
                    break;
            }
        }

        protected override bool IsComplete(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            var lastTwoLinesAreBlank = text.Split(Environment.NewLine)
                                           .Reverse()
                                           .TakeWhile(s => string.IsNullOrEmpty(s))
                                           .Take(2)
                                           .Count() == 2;

            if (lastTwoLinesAreBlank)
                return true;

            var syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Root.Members.Last().GetLastToken().IsMissing)
                return false;

            return true;
        }

        protected override void Eval(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                                ? new Compilation(syntaxTree)
                                : _previous.ContinueWith(syntaxTree);

            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (_showProgram)
                compilation.EmitTree(Console.Out);

            var result = compilation.Evaluate(_variables);

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                {
                    Console.ForegroundColor = result.Value is EvaluatorException
                                            ? ConsoleColor.Red
                                            : ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                }

                _previous = compilation;
            }
            else
                Console.Out.WriteDiagnostics(result.Diagnostics);
        }
    }
}
