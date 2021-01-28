using System;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public override Type Type => Expression.Type;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}