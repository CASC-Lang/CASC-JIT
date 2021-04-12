using System.Collections.Generic;

namespace CASC.CodeParser.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?");
        public static readonly TypeSymbol Any = new TypeSymbol("any");
        public static readonly TypeSymbol Void = new TypeSymbol("void");
        public static readonly TypeSymbol Array = new TypeSymbol("array", 1);
        public static readonly TypeSymbol Number = new TypeSymbol("number");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol String = new TypeSymbol("string");

        internal TypeSymbol(string name, int genericCount = 0)
            : base(name)
        {
            GenericCount = genericCount;
        }

        public override SymbolKind Kind => SymbolKind.Type;
        public List<TypeSymbol> Generics { get; } = new();
        public int GenericCount { get; }

        public bool AddGeneric(TypeSymbol generic) {
            if (GenericCount > GenericCount + 1) return false;

            Generics.Add(generic);

            return true;
        }
    }
}