using System;
using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value) : base(TypeSymbol.Error)
        {
            Value = value;

            if (value is decimal)
                Type = TypeSymbol.Number;
            else if (value is bool)
                Type = TypeSymbol.Bool;
            else if (value is string)
                Type = TypeSymbol.String;
            else
                throw new Exception($"ERROR: Unexpected literal '{value}' of type {value.GetType()}.");

            ActualType = Type;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override TypeSymbol Type { get; }
        public object Value { get; }
    }
}