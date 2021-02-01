using System.Collections.Immutable;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal class BoundArrayExpression : BoundExpression
    {
        public BoundArrayExpression(ImmutableArray<BoundExpression> contents)
        {
            Contents = contents;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ArrayExpression;
        public override TypeSymbol Type => TypeSymbol.Array;
        public ImmutableArray<BoundExpression> Contents { get; }
    }
}