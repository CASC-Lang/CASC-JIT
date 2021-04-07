using System.Collections.Immutable;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal class BoundArrayExpression : BoundIndexExpression
    {
        public BoundArrayExpression(ImmutableArray<BoundExpression> contents) : base(contents)
        {
        }

        public override BoundNodeKind Kind => BoundNodeKind.ArrayExpression;
        public override TypeSymbol Type => TypeSymbol.Array;
    }
}