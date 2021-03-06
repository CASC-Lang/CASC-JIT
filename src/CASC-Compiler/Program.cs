using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CASC.CodeParser;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;
using CASC.IO;
using Mono.Options;

namespace CASC
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var emitToIL = false;
            var outputPath = (string)null;
            var moduleName = (string)null;
            var references = new List<string>();
            var helpRequested = false;

            var options = new OptionSet
            {
                "usage: casc (<source-path> / repl) [options]",
                {"i=", "Compile CASC source code to assembly (Unstable)", v => emitToIL = true},
                {"r=", "The {path} of  an assembly to reference (Unstable)", v => references.Add(v)},
                {"o=", "The output {path} of an assembly to create (Unstable)", v => outputPath = v},
                {"m=", "The output {path} of the module (Unstable)", v => moduleName = v},
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
            
            var sourcePath = args[0];

            if (!File.Exists(sourcePath))
            {
                Console.Error.WriteLine($"ERROR: File '{sourcePath}' doesn't exist.");

                return 1;
            }

            var syntaxTree = SyntaxTree.Load(sourcePath);
            
            if (emitToIL)
            {
                outputPath ??= Path.ChangeExtension(sourcePath, ".exe");
                moduleName ??= Path.GetFileNameWithoutExtension(outputPath);

                var hasError = false;

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

                var compilation = Compilation.Create(syntaxTree);
                var diagnostics = compilation.EmitTree(moduleName, references.ToArray(), outputPath);

                if (diagnostics.Any())
                {
                    Console.Error.WriteDiagnostics(diagnostics);

                    return 1;
                }
            }
            else
            {
                var syntaxTrees = new List<SyntaxTree>
                {
                    syntaxTree
                };

                var referencedPaths = syntaxTree.ParseReferencedSources(Directory.GetParent(sourcePath).FullName);
                
                foreach (var referencePath in referencedPaths)
                    syntaxTrees.Add(SyntaxTree.Load(referencePath));

                var compilation = Compilation.Create(syntaxTrees.ToArray());
                var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

                if (result.Diagnostics.Any())
                {
                    Console.Error.WriteDiagnostics(result.Diagnostics);

                    return 1;
                }
            }

            return 0;
        }
    }
}