namespace CASC.CodeParser.Symbols
{
    public class LocalVariableSymbol : VariableSymbol
    {
        internal LocalVariableSymbol(string name, bool isFinalized, TypeSymbol type)
            : base(name, isFinalized, type)
        {
        }

        public override SymbolKind Kind => SymbolKind.LocalVariable;
    }
}