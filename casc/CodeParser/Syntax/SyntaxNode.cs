using System.Collections.Generic;

namespace CASC.CodeParser.Syntax
{
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}