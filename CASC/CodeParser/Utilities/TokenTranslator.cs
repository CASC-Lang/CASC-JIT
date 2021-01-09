using CASC.CodeParser.Syntax;

namespace CASC.CodeParser.Utilities
{
    public sealed class TokenTranslator
    {
        public TokenTranslator(SyntaxKind kind)
        {
            Kind = kind;
        }

        public TokenTranslator(SyntaxToken token) : this(token.Kind) { }

        public SyntaxKind Kind { get; }

        public string GetTranslated() => SyntaxFacts.GetZHText(Kind);
    }
}