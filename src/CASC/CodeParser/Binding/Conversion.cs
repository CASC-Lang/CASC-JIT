using CASC.CodeParser.Symbols;

namespace CASC.CodeParser.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(exists: false, isIdentity: false, isImplicit: false);
        public static readonly Conversion Identity = new Conversion(exists: true, isIdentity: true, isImplicit: true);
        public static readonly Conversion Implicit = new Conversion(exists: true, isIdentity: false, isImplicit: true);
        public static readonly Conversion Explicit = new Conversion(exists: true, isIdentity: false, isImplicit: false);

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from == TypeSymbol.Number ||
                from == TypeSymbol.Boolean)
                if (to == TypeSymbol.String)
                    return Explicit;

            if (from == TypeSymbol.String)
                if (to == TypeSymbol.Number ||
                    to == TypeSymbol.Boolean)
                    return Explicit;

            if (from == TypeSymbol.Boolean && to == TypeSymbol.Number)
                return Explicit;

            if (from == TypeSymbol.Number && to == TypeSymbol.Boolean)
                return Explicit;

            return None;
        }
    }
}