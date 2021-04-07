using System.Collections.Immutable;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundArrayAssignmentExpression : BoundAssignmentExpression {
        public BoundArrayAssignmentExpression(VariableSymbol variable,
                                              BoundIndexExpression indexes,
                                              BoundExpression expression)
                                              : base(variable, expression)
        {
            Indexes = indexes;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ArrayAssignmentExpression;
        public BoundIndexExpression Indexes { get; }
    }
}