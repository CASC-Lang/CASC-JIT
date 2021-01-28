namespace CASC.CodeParser.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name)
        {
            IsFinalized = isReadOnly;
            Type = type;
        }

        public override SymbolKind Kind => SymbolKind.Variable;
        public bool IsFinalized { get; }
        public TypeSymbol Type { get; }

        public override string ToString() => Name;
    }
}