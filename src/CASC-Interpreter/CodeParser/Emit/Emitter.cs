using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CASC.CodeParser.Binding;
using CASC.CodeParser.Symbols;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CASC.CodeParser.Emit
{
    internal class Emitter
    {
        private readonly DiagnosticPack _diagnostics = new();
        private readonly MethodReference _consoleWriteLineReference;
        private readonly MethodReference _decimalCtorReference;
        private readonly AssemblyDefinition _asmDefinition;
        private TypeDefinition _typeDefinition;
        private readonly Dictionary<TypeSymbol, TypeReference> _knownTypes;
        private readonly Dictionary<FunctionSymbol, MethodDefinition> _methods = new();

        private Emitter(string moduleName,
                        string[] references)
        {
            var asms = new List<AssemblyDefinition>();

            foreach (var reference in references)
            {
                try
                {
                    var asm = AssemblyDefinition.ReadAssembly(reference);
                    asms.Add(asm);
                }
                catch (BadImageFormatException)
                {
                    _diagnostics.ReportInvalidReference(reference);
                }
            }

            /***
             * Type Conversion Table
             * CASC         | C#
             * Any          | System.Object
             * Array        | System.Collections.Generic.List<object>
             * Bool         | System.Boolean
             * Number       | System.Decimal
             * String       | System.String
             * Void         | System.Void
             */

            var builtInTypes = new List<(TypeSymbol type, string metadataName)>
            {
                (TypeSymbol.Any, "System.Object"),
                // (TypeSymbol.Array, "System.Collections.Generic.List`1[System.String]"),
                (TypeSymbol.Bool, "System.Boolean"),
                (TypeSymbol.Number, "System.Decimal"),
                (TypeSymbol.String, "System.String"),
                (TypeSymbol.Void, "System.Void")
            };

            var asmName = new AssemblyNameDefinition(moduleName, new Version(1, 0));
            _asmDefinition = AssemblyDefinition.CreateAssembly(asmName, moduleName, ModuleKind.Console);
            _knownTypes = new Dictionary<TypeSymbol, TypeReference>();

            foreach (var (type, metadataName) in builtInTypes)
            {
                var typeReference = ResolveType(type.Name, metadataName);
                _knownTypes.Add(type, typeReference);
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
                        var typeReference = _asmDefinition.MainModule.ImportReference(foundTypes[0]);
                        return typeReference;
                    }
                    case 0:
                        _diagnostics.ReportRequiredTypeNotFound(minskName, metadataName);
                        break;
                    default:
                        _diagnostics.ReportRequiredTypeAmbiguous(minskName, metadataName, foundTypes);
                        break;
                }

                return null;
            }

            MethodReference ResolveMethod(string typeName, string methodName, string[] parameterTypeNames)
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

                            return _asmDefinition.MainModule.ImportReference(method);
                        }

                        _diagnostics.ReportRequiredMethodNotFound(typeName, methodName, parameterTypeNames);
                        return null;
                    }
                    case 0:
                        _diagnostics.ReportRequiredTypeNotFound(null, typeName);
                        break;
                    default:
                        _diagnostics.ReportRequiredTypeAmbiguous(null, typeName, foundTypes);
                        break;
                }

                return null;
            }

            _consoleWriteLineReference = ResolveMethod("System.Console", "WriteLine", new[] {"System.String"});
            _decimalCtorReference = ResolveMethod("System.Decimal", ".ctor", new[]
                {
                    "System.Int32", "System.Int32",
                    "System.Int32", "System.Boolean", "System.Byte"
                }
            );
        }

        public static ImmutableArray<Diagnostic> Emit(BoundProgram program,
                                                      string moduleName,
                                                      string[] references,
                                                      string outputPath)
        {
            if (program.Diagnostics.Any())
                return program.Diagnostics;

            var emitter = new Emitter(moduleName, references);
            return emitter.Emit(program, outputPath);
        }

        public ImmutableArray<Diagnostic> Emit(BoundProgram program, string outputPath)
        {
            if (_diagnostics.Any())
                return _diagnostics.ToImmutableArray();

            var objectType = _knownTypes[TypeSymbol.Any];

            _typeDefinition = new TypeDefinition("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed, objectType);
            _asmDefinition.MainModule.Types.Add(_typeDefinition);

            foreach (var (function, _) in program.Functions)
                EmitFunctionDeclaration(function);

            foreach (var (function, body) in program.Functions)
                EmitFunctionBody(function, body);

            if (program.MainFunction != null)
                _asmDefinition.EntryPoint = _methods[program.MainFunction];

            _asmDefinition.Write(outputPath);

            return _diagnostics.ToImmutableArray();
        }

        private void EmitFunctionBody(FunctionSymbol function, BoundBlockStatement body)
        {

            var method = _methods[function];
            var ilProcessor = method.Body.GetILProcessor();

            foreach (var statement in body.Statements)
                EmitStatement(ilProcessor, statement);

            if (function.ReturnType == TypeSymbol.Void)
                ilProcessor.Emit(OpCodes.Ret);
        }

        private void EmitFunctionDeclaration(FunctionSymbol function)
        {
            var method = new MethodDefinition(function.Name, MethodAttributes.Static | MethodAttributes.Private, _knownTypes[TypeSymbol.Void]);
            _typeDefinition.Methods.Add(method);
            _methods.Add(function, method);
        }

        private void EmitStatement(ILProcessor ilProcessor, BoundStatement statement)
        {
            switch (statement.Kind)
            {

                case BoundNodeKind.VariableDeclaration:
                    EmitVariableDeclaration(ilProcessor, (BoundVariableDeclaration)statement);
                    break;
                case BoundNodeKind.LabelStatement:
                    EmitLabelStatement(ilProcessor, (BoundLabelStatement)statement);
                    break;
                case BoundNodeKind.GotoStatement:
                    EmitGotoStatement(ilProcessor, (BoundGotoStatement)statement);
                    break;
                case BoundNodeKind.ConditionalGotoStatement:
                    EmitConditionalGotoStatement(ilProcessor, (BoundConditionalGotoStatement)statement);
                    break;
                case BoundNodeKind.ReturnStatement:
                    EmitReturnStatement(ilProcessor, (BoundReturnStatement)statement);
                    break;
                case BoundNodeKind.ExpressionStatement:
                    EmitExpressionStatement(ilProcessor, (BoundExpressionStatement)statement);
                    break;
                default:
                    throw new Exception($"Unexpected node kind {statement.Kind}");
            }

            ilProcessor.Emit(OpCodes.Ldstr, "Greeting from CASC!");
            ilProcessor.Emit(OpCodes.Call, _consoleWriteLineReference);
            ilProcessor.Emit(OpCodes.Ret);
        }

        private void EmitVariableDeclaration(ILProcessor ilProcessor, BoundVariableDeclaration statement)
        {
            throw new NotImplementedException();
        }

        private void EmitLabelStatement(ILProcessor ilProcessor, BoundLabelStatement statement)
        {
            throw new NotImplementedException();
        }
        private void EmitGotoStatement(ILProcessor ilProcessor, BoundGotoStatement statement)
        {
            throw new NotImplementedException();
        }
        private void EmitConditionalGotoStatement(ILProcessor ilProcessor, BoundConditionalGotoStatement statement)
        {
            throw new NotImplementedException();
        }
        private void EmitReturnStatement(ILProcessor ilProcessor, BoundReturnStatement statement)
        {
            throw new NotImplementedException();
        }
        private void EmitExpressionStatement(ILProcessor ilProcessor, BoundExpressionStatement statement)
        {
            EmitExpression(ilProcessor, statement.Expression);

            if (statement.Expression.Type != TypeSymbol.Void)
                ilProcessor.Emit(OpCodes.Pop);
        }
        private void EmitExpression(ILProcessor ilProcessor, BoundExpression expression)
        {
            switch (expression.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    EmitLiteralExpression(ilProcessor, (BoundLiteralExpression)expression);
                    break;
                case BoundNodeKind.VariableExpression:
                    EmitVariableExpression(ilProcessor, (BoundVariableExpression)expression);
                    break;
                case BoundNodeKind.AssignmentExpression:
                    EmitAssignmentExpression(ilProcessor, (BoundAssignmentExpression)expression);
                    break;
                case BoundNodeKind.UnaryExpression:
                    EmitUnaryExpression(ilProcessor, (BoundUnaryExpression)expression);
                    break;
                case BoundNodeKind.BinaryExpression:
                    EmitBinaryExpression(ilProcessor, (BoundBinaryExpression)expression);
                    break;
                case BoundNodeKind.CallExpression:
                    EmitCallExpression(ilProcessor, (BoundCallExpression)expression);
                    break;
                case BoundNodeKind.ConversionExpression:
                    EmitConversionExpression(ilProcessor, (BoundConversionExpression)expression);
                    break;
                case BoundNodeKind.ArrayExpression:
                    EmitArrayExpression(ilProcessor, (BoundArrayExpression)expression);
                    break;
                default:
                    throw new Exception($"Unexpected node kind {expression.Kind}");
            }
        }
        private void EmitLiteralExpression(ILProcessor ilProcessor, BoundLiteralExpression expression)
        {
            if (expression.Type == TypeSymbol.Bool)
            {
                var value = (bool)expression.Value;
                ilProcessor.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else if (expression.Type == TypeSymbol.Number)
            {
                var value = (decimal)expression.Value;
                var bits = decimal.GetBits(value);
                var sign = (bits[3] & 0x80000000) != 0;
                var scale = (byte)((bits[3] >> 16) & 0x7f);

                ilProcessor.Emit(OpCodes.Ldc_I4_4, bits[0]);
                ilProcessor.Emit(OpCodes.Ldc_I4, bits[1]);
                ilProcessor.Emit(OpCodes.Ldc_I4, bits[2]);
                ilProcessor.Emit(sign ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                ilProcessor.Emit(OpCodes.Ldc_I4, scale);
                ilProcessor.Emit(OpCodes.Newobj, _decimalCtorReference);
            }
            else if (expression.Type == TypeSymbol.String)
            {
                var value = (string)expression.Value;
                ilProcessor.Emit(OpCodes.Ldstr, value);
            }
            else if (expression.Type == TypeSymbol.Array)
            {
                // TODO: construct a list with a array
            }
            else
                throw new Exception($"Unexpected literal type: {expression.Type}.");
        }
        private void EmitVariableExpression(ILProcessor ilProcessor, BoundVariableExpression expression)
        {
            throw new NotImplementedException();
        }
        private void EmitAssignmentExpression(ILProcessor ilProcessor, BoundAssignmentExpression expression)
        {
            throw new NotImplementedException();
        }
        private void EmitUnaryExpression(ILProcessor ilProcessor, BoundUnaryExpression expression)
        {
            throw new NotImplementedException();
        }
        private void EmitBinaryExpression(ILProcessor ilProcessor, BoundBinaryExpression expression)
        {
            throw new NotImplementedException();
        }
        private void EmitCallExpression(ILProcessor ilProcessor, BoundCallExpression expression)
        {
            foreach (var argument in expression.Arguments)
                EmitExpression(ilProcessor, argument);

            if (expression.Function == BuiltinFunctions.Print)
            {
                ilProcessor.Emit(OpCodes.Call, _consoleWriteLineReference);
            }
            else if (expression.Function == BuiltinFunctions.Input)
            {

            }
            else if (expression.Function == BuiltinFunctions.Random)
            {

            }
            else
            {
                var methodDefinition = _methods[expression.Function];
                ilProcessor.Emit(OpCodes.Call, methodDefinition);
            }
        }
        private void EmitConversionExpression(ILProcessor ilProcessor, BoundConversionExpression expression)
        {
            throw new NotImplementedException();
        }
        private void EmitArrayExpression(ILProcessor ilProcessor, BoundArrayExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}