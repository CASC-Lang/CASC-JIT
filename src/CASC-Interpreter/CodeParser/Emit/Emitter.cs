using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CASC.CodeParser.Binding;
using CASC.CodeParser.Symbols;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CASC.CodeParser.Emit
{
    internal class Emitter
    {
        private readonly DiagnosticPack _diagnostics = new();
        private readonly List<AssemblyDefinition> _asms = new();
        private readonly Dictionary<TypeSymbol, TypeReference> _knownTypes = new();
        private readonly TypeReference consoleType;

        public static ImmutableArray<Diagnostic> Emit(BoundProgram program,
                                                      string moduleName,
                                                      string[] references,
                                                      string outputPath)
        {
            if (program.Diagnostics.Any())
                return program.Diagnostics;

            var asms = new List<AssemblyDefinition>();
            var result = new DiagnosticPack();

            foreach (var reference in references)
            {
                try
                {
                    var asm = AssemblyDefinition.ReadAssembly(reference);
                    asms.Add(asm);
                }
                catch (BadImageFormatException)
                {
                    result.ReportInvalidReference(reference);
                }
            }

            /***
             * Type Conversion Table
             * CASC         | C#
             * Any          | System.Object
             * Bool         | System.Boolean
             * Number       | System.Decimal
             * String       | System.String
             * Void         | System.Void
             */

            var builtInTypes = new List<(TypeSymbol type, string metadataName)>
            {
                (TypeSymbol.Any, "System.Object"),
                (TypeSymbol.Bool, "System.Boolean"),
                (TypeSymbol.Number, "System.Decimal"),
                (TypeSymbol.String, "System.String"),
                (TypeSymbol.Void, "System.Void")
            };

            var asmName = new AssemblyNameDefinition(moduleName, new Version(1, 0));
            var asmDefinition = AssemblyDefinition.CreateAssembly(asmName, moduleName, ModuleKind.Console);
            var knownTypes = new Dictionary<TypeSymbol, TypeReference>();

            foreach (var (type, metadataName) in builtInTypes)
            {
                var typeReference = ResolveType(type.Name, metadataName);
                knownTypes.Add(type, typeReference);
            }

            TypeReference ResolveType(string minskName, string metadataName)
            {
                var foundTypes = asms.SelectMany(a => a.Modules)
                                     .SelectMany(m => m.Types)
                                     .Where(t => t.FullName == metadataName)
                                     .ToArray();
                switch (foundTypes.Length)
                {
                    case 1:
                    {
                        var typeReference = asmDefinition.MainModule.ImportReference(foundTypes[0]);
                        return typeReference;
                    }
                    case 0:
                        result.ReportRequiredTypeNotFound(minskName, metadataName);
                        break;
                    default:
                        result.ReportRequiredTypeAmbiguous(minskName, metadataName, foundTypes);
                        break;
                }

                return null;
            }

            MethodReference ReseolveMethod(string typeName, string methodName, string[] parameterTypeNames)
            {
                var foundTypes = asms.SelectMany(a => a.Modules)
                                     .SelectMany(m => m.Types)
                                     .Where(t => t.FullName == typeName)
                                     .ToArray();
                switch (foundTypes.Length)
                {
                    case 1:
                    {
                        var foundType = foundTypes[0];
                        var methods = foundType.Methods.Where(m => m.Name == methodName);

                        foreach (var method in methods)
                        {
                            if (method.Parameters.Count != parameterTypeNames.Length)
                                continue;

                            var allParameterMatch = true;

                            for (var i = 0; i < parameterTypeNames.Length; i++)
                            {
                                if (method.Parameters[i].ParameterType.FullName != parameterTypeNames[i])
                                {
                                    allParameterMatch = false;
                                    break;
                                }
                            }

                            if (!allParameterMatch)
                                continue;

                            return asmDefinition.MainModule.ImportReference(method);
                        }

                        result.ReportRequiredMethodNotFound(typeName, methodName, parameterTypeNames);
                        return null;
                    }
                    case 0:
                        result.ReportRequiredTypeNotFound(null, typeName);
                        break;
                    default:
                        result.ReportRequiredTypeAmbiguous(null, typeName, foundTypes);
                        break;
                }

                return null;
            }

            var consoleTypeReference = ReseolveMethod("System.Console", "WriteLine", new[] {"System.String"});

            if (result.Any())
                return result.ToImmutableArray();

            var objectType = knownTypes[TypeSymbol.Any];
            var voidType = knownTypes[TypeSymbol.Void];

            var typeDefinition = new TypeDefinition("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed, objectType);
            asmDefinition.MainModule.Types.Add(typeDefinition);
            var main = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private, voidType);
            typeDefinition.Methods.Add(main);

            var ilProcessor = main.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldstr, "Greeting from CASC!");
            ilProcessor.Emit(OpCodes.Call, consoleTypeReference);
            ilProcessor.Emit(OpCodes.Ret);

            asmDefinition.EntryPoint = main;

            asmDefinition.Write(outputPath);

            return result.ToImmutableArray();
        }
    }
}