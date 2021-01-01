namespace CASC.CodeParser.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAND,
        LogicalOR,
        Equals,
        NotEquals,

        // Special Chinese Operator
        Power,
        NthRoot
    }
}