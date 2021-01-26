namespace CASC.CodeParser.Binding
{
    internal enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,

        // Expressions
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression
    }
}