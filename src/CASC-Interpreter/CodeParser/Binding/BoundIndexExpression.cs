using System.Collections.Immutable;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal class BoundIndexExpression : BoundExpression
    {
        public BoundIndexExpression(ImmutableArray<BoundExpression> contents)
        {
            Contents = contents;
        }

        public override TypeSymbol Type => TypeSymbol.Array;
        public override BoundNodeKind Kind => BoundNodeKind.IndexExpression;
        public ImmutableArray<BoundExpression> Contents { get; }
    }
}