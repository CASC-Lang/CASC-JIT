using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using CASC.CodeParser.Text;

namespace CASC.CodeParser.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText source, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Source = source;
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public SourceText Source { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var source = SourceText.From(text);
            return Parse(source);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var source = SourceText.From(text);
            return ParseTokens(source);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText source)
        {
            var lexer = new Lexer(source);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                yield return token;
            }
        }
    }
}