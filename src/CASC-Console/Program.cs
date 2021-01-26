using CASC.CodeParser.Syntax;
using CASC.CodeParser;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using CASC.CodeParser.Text;

namespace CASC
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var showParseTree = false;
            var showProgram = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var builder = new StringBuilder();
            Compilation previous = null;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;

                if (builder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write("| ");

                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (builder.Length == 0)
                    if (isBlank)
                        break;
                    else if (input == "/showTree")
                    {
                        showParseTree = !showParseTree;
                        continue;
                    }
                    else if (input == "/showProgram")
                    {
                        showProgram = !showProgram;
                        continue;
                    }
                    else if (input == "/clear")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "/reset")
                    {
                        previous = null;
                        variables.Clear();
                        continue;
                    }

                builder.AppendLine(input);
                var text = builder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = previous == null
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);

                if (showParseTree)
                    syntaxTree.Root.WriteTo(Console.Out);
                if (showProgram)
                    compilation.EmitTree(Console.Out);

                var result = compilation.Evaluate(variables);

                if (!result.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    previous = compilation;
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.Source.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.Source.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - line.Start + 1;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {lineIndex}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.Source.ToString(prefixSpan);
                        var error = syntaxTree.Source.ToString(diagnostic.Span);
                        var suffix = syntaxTree.Source.ToString(suffixSpan);

                        Console.Write("    ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.Write(suffix);

                        Console.WriteLine();
                    }

                    Console.WriteLine();
                }

                builder.Clear();
            }
        }
    }
}
