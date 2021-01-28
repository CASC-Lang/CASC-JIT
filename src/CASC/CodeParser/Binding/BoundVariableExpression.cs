using System;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public VariableSymbol Variable { get; }
        public string Name => Variable.Name;
        public override Type Type => Variable.Type;
    }
}