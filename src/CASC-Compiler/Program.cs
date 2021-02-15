using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CASC.CodeParser;
using CASC.CodeParser.Syntax;
using CASC.IO;
using Mono.Options;

namespace CASC
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var outputPath = (string)null;
            var moduleName = (string)null;
            var references = new List<string>();
            var sourcePaths = new List<string>();
            var helpRequested = false;

            var options = new OptionSet
            {
                "usage: mcs <source-paths> (or repl) [options]",
                {"r=", "The {path} of  an assembly to reference", v => references.Add(v)},
                {"o=", "The output {path} of an assembly to create", v => outputPath = v},
                {"m=", "The output {path} of the module", v => moduleName = v},
                {"<>", v => sourcePaths.Add(v)},
                {"?|h|help", _ => helpRequested = true}
            };

            options.Parse(args);

            if (helpRequested)
            {
                options.WriteOptionDescriptions(Console.Out);

                return 0;
            }

            if (string.Equals(args[0], "repl"))
            {
                var repl = new CASCRepl();
                repl.Run();
                return 0;
            }

            if (sourcePaths.Count == 0)
            {
                Console.Error.WriteLine("ERROR: Require at least one source file.");

                return 1;
            }

            outputPath ??= Path.ChangeExtension(sourcePaths[0], ".exe");

            moduleName ??= Path.GetFileNameWithoutExtension(outputPath);

            var syntaxTrees = new List<SyntaxTree>();
            var hasError = false;

            foreach (var path in sourcePaths)
            {
                if (!File.Exists(path))
                {
                    Console.Error.WriteLine($"ERROR: File '{path}' doesn't exist.");
                    hasError = true;
                    continue;
                }

                var syntaxTree = SyntaxTree.Load(path);
                syntaxTrees.Add(syntaxTree);
            }

            foreach (var path in references)
            {
                if (!File.Exists(path))
                {
                    Console.Error.WriteLine($"ERROR: File '{path}' doesn't exist.");
                    hasError = true;
                }
            }

            if (hasError)
                return 1;

            var compilation = Compilation.Create(syntaxTrees.ToArray());
            var diagnostics = compilation.EmitTree(moduleName, references.ToArray(), outputPath);

            if (!diagnostics.Any())
            {
            }
            else
            {
                Console.Error.WriteDiagnostics(diagnostics);
                return 1;
            }

            return 0;
        }
    }
}