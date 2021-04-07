using System.Collections.Immutable;
using System.Collections.Generic;
using CASC.CodeParser.Text;
using System.IO;

namespace CASC.CodeParser.Syntax
{
    public sealed class SyntaxTree
    {
        private delegate void ParseHandler(SyntaxTree syntaxTree,
                                           out CompilationUnitSyntax root,
                                           out ImmutableArray<Diagnostic> diagnostics);

        private SyntaxTree(SourceText source, ParseHandler handler)
        {
            Source = source;

            handler(this, out var root, out var diagnostics);
            
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Source { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Load(string fileName)
        {
            var text = File.ReadAllText(fileName);
            var sourceText = SourceText.From(text, fileName);

            return Parse(sourceText);
        }

        public static SyntaxTree Load(string source, string builtInModule)
        {
            return Parse(SourceText.From(source, builtInModule));
        }

        private static void Parse(SyntaxTree syntaxTree, out CompilationUnitSyntax root, out ImmutableArray<Diagnostic> diagnostics)
        {
            var parser = new Parser(syntaxTree);
            root = parser.ParseCompilationUnit();
            diagnostics = parser.Diagnostics.ToImmutableArray();
        }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);

            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text) => new SyntaxTree(text, Parse);

        public static ImmutableArray<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);

            return ParseTokens(sourceText);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(string text, out ImmutableArray<Diagnostic> diagnostics)
        {
            var sourceText = SourceText.From(text);

            return ParseTokens(sourceText, out diagnostics);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text) => ParseTokens(text, out _);

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, out ImmutableArray<Diagnostic> diagnostics)
        {
            var tokens = new List<SyntaxToken>();

            void ParseTokens(SyntaxTree syntaxTree, out CompilationUnitSyntax root, out ImmutableArray<Diagnostic> diagnostics)
            {
                root = null;
                var lexer = new Lexer(syntaxTree);

                while (true)
                {
                    var token = lexer.Lex();

                    if (token.Kind == SyntaxKind.EndOfFileToken)
                    {
                        root = new CompilationUnitSyntax(syntaxTree, ImmutableArray<MemberSyntax>.Empty, token);
                        break;
                    }

                    tokens.Add(token);
                }

                diagnostics = lexer.Diagnostics.ToImmutableArray();
            }

            var tempSyntaxTree = new SyntaxTree(text, ParseTokens);
            diagnostics = tempSyntaxTree.Diagnostics.ToImmutableArray();

            return tokens.ToImmutableArray();
        }

        public ImmutableArray<string> ParseReferencedSources(string path)
        {
            var tokens = ParseTokens(Source);
            var referencedSources = new List<string>();

            for (var i = 0; i < tokens.Length; i++)
                if (tokens[i].Kind == SyntaxKind.ImportKeyword)
                    referencedSources.Add(path + tokens[i + 2].Text.Replace("\"", ""));

            return referencedSources.ToImmutableArray();
        }
    }
}