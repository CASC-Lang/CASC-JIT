namespace CASC.CodeParser.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Number = new TypeSymbol("number");
        public static readonly TypeSymbol Boolean = new TypeSymbol("bool");
        public static readonly TypeSymbol String = new TypeSymbol("string");

        internal TypeSymbol(string name)
            : base(name)
        {

        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}