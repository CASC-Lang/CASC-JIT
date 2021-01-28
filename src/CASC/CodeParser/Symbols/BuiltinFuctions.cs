using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace CASC.CodeParser.Symbols
{
    internal static class BuiltinFuctions
    {
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);

        internal static IEnumerable<FunctionSymbol> GetAll()
            => typeof(BuiltinFuctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                      .Where(f => f.FieldType == typeof(FunctionSymbol))
                                      .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}