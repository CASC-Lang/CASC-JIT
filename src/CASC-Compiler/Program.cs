using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CASC.CodeParser;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;

namespace CASC
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: casc <source-path>");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("ERROR: Only one path supported.");
                return;
            }

            var path = args.Single();

            var text = File.ReadAllText(path);
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            Console.WriteLine("Hi");
        }
    }
}