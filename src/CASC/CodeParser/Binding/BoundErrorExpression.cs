using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
        public override TypeSymbol Type => TypeSymbol.Error;
    }
}