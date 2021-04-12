using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public BoundExpression(TypeSymbol actualType)
        {
            ActualType = actualType;
        }

        public abstract TypeSymbol Type { get; }
        public TypeSymbol ActualType { get; set; }
    }
}