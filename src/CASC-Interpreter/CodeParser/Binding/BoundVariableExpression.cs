using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable, BoundIndexExpression indexClause = null) : base(variable.Type)
        {
            Type = variable.Type;
            Variable = variable;
            IndexClause = indexClause;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public override TypeSymbol Type { get; }
        public VariableSymbol Variable { get; }
        public BoundIndexExpression IndexClause { get; }
    }
}