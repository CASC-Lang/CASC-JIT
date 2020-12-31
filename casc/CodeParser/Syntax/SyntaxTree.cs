using System.Collections.Generic;
using System.Linq;

namespace CASC.CodeParser.Syntax {
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

        public static SyntaxTree Parse(string text) {
            var parser =  new Parser(text);
            return parser.Parse();
        }
    }
}