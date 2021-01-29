using System.Collections.Immutable;
using System.Collections.Generic;
using CASC.CodeParser.Text;

namespace CASC.CodeParser.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText source)
        {
            var parser = new Parser(source);
            var root = parser.ParseCompilationUnit();

            Source = source;
            Diagnostics = parser.Diagnostics.ToImmutableArray();
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

        public static ImmutableArray<SyntaxToken> ParseTokens(string text)
        {
            var source = SourceText.From(text);
            return ParseTokens(source);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(string text, out ImmutableArray<Diagnostic> diagnostics)
        {
            var source = SourceText.From(text);
            return ParseTokens(source, out diagnostics);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText source)
        {
            return ParseTokens(source, out _);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText source, out ImmutableArray<Diagnostic> diagnostics)
        {
            IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
            {
                while (true)
                {
                    var token = lexer.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;

                    yield return token;
                }
            }
            
            var l = new Lexer(source);
            var result =LexTokens(l);
            diagnostics = l.Diagnostics.ToImmutableArray();
            
            return result.ToImmutableArray();
        }
    }
}