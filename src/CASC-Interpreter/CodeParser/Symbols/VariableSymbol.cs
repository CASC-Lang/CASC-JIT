namespace CASC.CodeParser.Symbols
{
    public abstract class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isFinalized, TypeSymbol type)
            : base(name)
        {
            IsFinalized = isFinalized;
            Type = type;
        }

        public bool IsFinalized { get; }
        public TypeSymbol Type { get; }
    }
}