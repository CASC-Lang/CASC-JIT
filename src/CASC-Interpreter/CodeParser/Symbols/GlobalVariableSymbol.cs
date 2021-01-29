namespace CASC.CodeParser.Symbols
{
    public sealed class GlobalVariableSymbol : VariableSymbol
    {
        internal GlobalVariableSymbol(string name, bool isFinalized, TypeSymbol type)
            : base(name, isFinalized, type)
        {
        }

        public override SymbolKind Kind => SymbolKind.GlobalVariable;
    }
}