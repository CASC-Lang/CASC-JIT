namespace CASC.CodeParser.Binding
{
    internal enum BoundUnaryOperatorKind
    {
        Identity,
        Negation,
        LogicalNegation,

        // Special Chinese Operator
        Square,
        SquareRoot,
        NthRoot
    }
}