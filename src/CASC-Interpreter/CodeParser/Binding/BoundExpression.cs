using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}