namespace CASC.CodeParser.Symbols
{
    public class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isFinalized, TypeSymbol type)
            : base(name)
        {
            IsFinalized = isFinalized;
            Type = type;
        }

        public override SymbolKind Kind => SymbolKind.Variable;
        public bool IsFinalized { get; }
        public TypeSymbol Type { get; }

        public override string ToString() => Name;
    }
}