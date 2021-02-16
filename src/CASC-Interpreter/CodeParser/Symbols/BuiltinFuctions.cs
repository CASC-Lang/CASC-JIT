using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace CASC.CodeParser.Symbols
{
    internal static class BuiltinFunctions
    {
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("string", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Random = new FunctionSymbol("random", ImmutableArray.Create(new ParameterSymbol("min", TypeSymbol.Number), new ParameterSymbol("max", TypeSymbol.Number)), TypeSymbol.Number);

        internal static IEnumerable<FunctionSymbol> GetAll()
            => typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                      .Where(f => f.FieldType == typeof(FunctionSymbol))
                                      .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}