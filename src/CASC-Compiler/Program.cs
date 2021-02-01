using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CASC.CodeParser;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;
using CASC.IO;

namespace CASC
{
    internal static class Program
    {
        private static readonly Regex CASCFileRegex = new Regex("\\.casc|\\.cas");

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(
                "Usage: casc <source-directory-path>\n   or  casc repl\nIf you are running CASC by local executable file,\nconsider adds arguments behind the excutable file."
                );
                return 1;
            }

            if (string.Equals(args[0], "repl"))
            {
                var repl = new CASCRepl();
                repl.Run();
                return 0;
            }


            var paths = GetFilePaths(args);
            var syntaxTrees = new List<SyntaxTree>();
            var hasError = false;

            foreach (var path in paths)
            {
                var extensionName = Path.GetExtension(path);

                if (!File.Exists(path))
                {
                    Console.Error.WriteLine($"ERROR: File '{path}' doesn't exist.");
                    hasError = true;
                    continue;
                }

                var syntaxTree = SyntaxTree.Load(path);
                syntaxTrees.Add(syntaxTree);
            }

            if (hasError)
                return 1;

            var compilation = new Compilation(syntaxTrees.ToArray());
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                {
                    if (result.Value.GetType().IsArray)
                        Console.WriteLine(IOUtil.ArrayToString((object[])result.Value));
                    else
                        Console.WriteLine(result.Value.ToString());
                }
            }
            else
            {
                Console.Error.WriteDiagnostics(result.Diagnostics);
                return 1;
            }

            return 0;
        }

        private static IEnumerable<string> GetFilePaths(IEnumerable<string> paths)
        {

            var result = new SortedSet<string>();

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                    result.UnionWith(Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                                              .Where(f => CASCFileRegex.IsMatch(Path.GetExtension(f))));
                else
                    result.Add(path);
            }

            return result;
        }
    }
}