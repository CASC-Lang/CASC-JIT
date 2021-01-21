using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using CASC.CodeParser.Text;

namespace CASC.CodeParser.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText source)
        {
            var parser = new Parser(source);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Source = source;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Source { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var source = SourceText.From(text);
            return Parse(source);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
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