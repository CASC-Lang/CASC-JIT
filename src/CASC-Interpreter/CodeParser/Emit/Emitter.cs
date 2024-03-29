using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CASC.CodeParser.Binding;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CASC.CodeParser.Emit
{
    internal class Emitter
    {
        private readonly DiagnosticPack _diagnostics = new();
        private readonly AssemblyDefinition _asmDefinition;
        private readonly Dictionary<TypeSymbol, TypeReference> _knownTypes;
        private readonly Dictionary<FunctionSymbol, MethodDefinition> _methods = new();
        private readonly Dictionary<VariableSymbol, VariableDefinition> _locals = new();
        private readonly Dictionary<BoundLabel, int> _labels = new();
        private readonly List<(int InstructionIndex, BoundLabel Target)> _fixups = new();

        private readonly MethodReference _consoleWriteLineReference;
        private readonly MethodReference _consoleReadLineReference;
        private readonly MethodReference _stringConcatReference;

        private readonly MethodReference _decimalCtorReference;
        private readonly MethodReference _decimalAdditionReference;
        private readonly MethodReference _decimalLessReference;
        private readonly MethodReference _decimalLessEqualReference;

        private readonly MethodReference _convert2BoolReference;
        private readonly MethodReference _convert2DecimalReference;
        private readonly MethodReference _convert2StringReference;
        private readonly MethodReference _objectEqualsReference;

        private TypeDefinition _typeDefinition;

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
            _consoleReadLineReference = ResolveMethod("System.Console", "ReadLine", Array.Empty<string>());
            _stringConcatReference = ResolveMethod("System.String", "Concat", new[] {"System.String", "System.String"});
            _decimalCtorReference = ResolveMethod("System.Decimal", ".ctor", new[]
                {
                    "System.Int32", "System.Int32",
                    "System.Int32", "System.Boolean", "System.Byte"
                }
            );
            _decimalAdditionReference = ResolveMethod("System.Decimal", "op_Addition", new[] {"System.Decimal", "System.Decimal"});
            _decimalLessReference = ResolveMethod("System.Decimal", "op_LessThan", new[] {"System.Decimal", "System.Decimal"});
            _decimalLessEqualReference = ResolveMethod("System.Decimal", "op_LessThanOrEqual", new[] {"System.Decimal", "System.Decimal"});

            _convert2BoolReference = ResolveMethod("System.Convert", "ToBoolean", new[] {"System.Object"});
            _convert2DecimalReference = ResolveMethod("System.Convert", "ToDecimal", new[] {"System.Object"});
            _convert2StringReference = ResolveMethod("System.Convert", "ToString", new[] {"System.Object"});

            _objectEqualsReference = ResolveMethod("System.Object", "Equals", new[] {"System.Object", "System.Object"});
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

        private void EmitFunctionDeclaration(FunctionSymbol function)
        {
            var method = new MethodDefinition(function.Name, MethodAttributes.Static | MethodAttributes.Private, _knownTypes[function.ReturnType]);

            foreach (var parameter in function.Parameters)
            {
                var parameterType = _knownTypes[parameter.Type];
                var parameterAttribute = ParameterAttributes.None;
                var parameterDefinition = new ParameterDefinition(parameter.Name, parameterAttribute, parameterType);
                method.Parameters.Add(parameterDefinition);
            }

            _typeDefinition.Methods.Add(method);
            _methods.Add(function, method);
        }

        private void EmitFunctionBody(FunctionSymbol function, BoundBlockStatement body)
        {
            var method = _methods[function];
            _locals.Clear();
            _labels.Clear();
            _fixups.Clear();

            var ilProcessor = method.Body.GetILProcessor();

            foreach (var statement in body.Statements)
                EmitStatement(ilProcessor, statement);

            foreach (var fixup in _fixups)
            {
                var targetLabel = fixup.Target;
                var targetInstructionIndex = _labels[targetLabel];
                var targetInstruction = ilProcessor.Body.Instructions[targetInstructionIndex];

                var instructionToFixup = ilProcessor.Body.Instructions[fixup.InstructionIndex];
                instructionToFixup.Operand = targetInstruction;
            }

            method.Body.OptimizeMacros();
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
        }

        private void EmitVariableDeclaration(ILProcessor ilProcessor, BoundVariableDeclaration statement)
        {
            var typeReference = _knownTypes[statement.Variable.Type];
            var variableDefinition = new VariableDefinition(typeReference);
            _locals.Add(statement.Variable, variableDefinition);
            ilProcessor.Body.Variables.Add(variableDefinition);

            EmitExpression(ilProcessor, statement.Initializer);
            ilProcessor.Emit(OpCodes.Stloc, variableDefinition);
        }

        private void EmitLabelStatement(ILProcessor ilProcessor, BoundLabelStatement statement)
        {
            _labels.Add(statement.Label, ilProcessor.Body.Instructions.Count);
        }

        private void EmitGotoStatement(ILProcessor ilProcessor, BoundGotoStatement statement)
        {
            _fixups.Add((ilProcessor.Body.Instructions.Count, statement.Label));
            ilProcessor.Emit(OpCodes.Br, Instruction.Create(OpCodes.Nop));
        }

        private void EmitConditionalGotoStatement(ILProcessor ilProcessor, BoundConditionalGotoStatement statement)
        {
            EmitExpression(ilProcessor, statement.Condition);

            var opCode = statement.JumpIfTrue ? OpCodes.Brtrue : OpCodes.Brfalse;
            _fixups.Add((ilProcessor.Body.Instructions.Count, statement.Label));
            ilProcessor.Emit(opCode, Instruction.Create(OpCodes.Nop));
        }

        private void EmitReturnStatement(ILProcessor ilProcessor, BoundReturnStatement statement)
        {
            if (statement.Expression != null)
                EmitExpression(ilProcessor, statement.Expression);

            ilProcessor.Emit(OpCodes.Ret);
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
                ilProcessor.Emit(OpCodes.Ldc_I4, bits[0]);
                ilProcessor.Emit(OpCodes.Ldc_I4, bits[1]);
                ilProcessor.Emit(OpCodes.Ldc_I4, bits[2]);
                ilProcessor.Emit(sign ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                ilProcessor.Emit(OpCodes.Ldc_I4, (int)scale);
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
            if (expression.Variable is ParameterSymbol parameter)
            {
                ilProcessor.Emit(OpCodes.Ldarg, parameter.Ordinal);
            }
            else
            {
                var variableDefinition = _locals[expression.Variable];
                ilProcessor.Emit(OpCodes.Ldloc, variableDefinition.Index);
            }
        }

        private void EmitAssignmentExpression(ILProcessor ilProcessor, BoundAssignmentExpression expression)
        {
            var variableDefinition = _locals[expression.Variable];
            EmitExpression(ilProcessor, expression.Expression);
            ilProcessor.Emit(OpCodes.Dup);
            ilProcessor.Emit(OpCodes.Stloc, variableDefinition);

            EmitExpression(ilProcessor, expression.Expression);
        }

        private void EmitUnaryExpression(ILProcessor ilProcessor, BoundUnaryExpression expression)
        {
            EmitExpression(ilProcessor, expression.Operand);

            switch (expression.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    break;
                case BoundUnaryOperatorKind.Negation:
                    ilProcessor.Emit(OpCodes.Neg);
                    break;
                case BoundUnaryOperatorKind.LogicalNegation:
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundUnaryOperatorKind.OnesComplement:
                    ilProcessor.Emit(OpCodes.Not);
                    break;
                default:
                    throw new Exception($"Unexpected unary operator {SyntaxFacts.GetText(expression.Op.SyntaxKind)}({expression.Operand.Type})");
            }
        }

        private void EmitBinaryExpression(ILProcessor ilProcessor, BoundBinaryExpression expression)
        {
            EmitExpression(ilProcessor, expression.Left);
            EmitExpression(ilProcessor, expression.Right);

            if (expression.Op.Kind == BoundBinaryOperatorKind.Addition)
            {
                if (expression.Left.Type == TypeSymbol.String &&
                    expression.Right.Type == TypeSymbol.String)
                {
                    ilProcessor.Emit(OpCodes.Call, _stringConcatReference);
                }
            }

            if (expression.Op.Kind == BoundBinaryOperatorKind.Equals)
            {
                if (expression.Left.Type == TypeSymbol.Any && expression.Right.Type == TypeSymbol.Any ||
                    expression.Left.Type == TypeSymbol.String && expression.Right.Type == TypeSymbol.String)
                {
                    ilProcessor.Emit(OpCodes.Call, _objectEqualsReference);
                    return;
                }
            }

            if (expression.Op.Kind == BoundBinaryOperatorKind.NotEquals)
            {
                if (expression.Left.Type == TypeSymbol.Any && expression.Right.Type == TypeSymbol.Any ||
                    expression.Left.Type == TypeSymbol.String && expression.Right.Type == TypeSymbol.String)
                {
                    ilProcessor.Emit(OpCodes.Call, _objectEqualsReference);
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    return;
                }
            }

            if (expression.Left.Type == TypeSymbol.String &&
                expression.Right.Type == TypeSymbol.String)
            {

            }

            switch (expression.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    ilProcessor.Emit(OpCodes.Call, _decimalAdditionReference);
                    break;
                case BoundBinaryOperatorKind.Subtraction:
                    ilProcessor.Emit(OpCodes.Sub);
                    break;
                case BoundBinaryOperatorKind.Multiplication:
                    ilProcessor.Emit(OpCodes.Mul);
                    break;
                case BoundBinaryOperatorKind.Division:
                    ilProcessor.Emit(OpCodes.Div);
                    break;
                case BoundBinaryOperatorKind.BitwiseXOR:
                    ilProcessor.Emit(OpCodes.Xor);
                    break;
                case BoundBinaryOperatorKind.LogicalAND:
                case BoundBinaryOperatorKind.BitwiseAND:
                    ilProcessor.Emit(OpCodes.And);
                    break;
                case BoundBinaryOperatorKind.LogicalOR:
                case BoundBinaryOperatorKind.BitwiseOR:
                    ilProcessor.Emit(OpCodes.Or);
                    break;
                case BoundBinaryOperatorKind.Equals:
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundBinaryOperatorKind.NotEquals:
                    ilProcessor.Emit(OpCodes.Ceq);
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundBinaryOperatorKind.Greater:
                    ilProcessor.Emit(OpCodes.Cgt);
                    break;
                case BoundBinaryOperatorKind.GreaterEquals:
                    ilProcessor.Emit(OpCodes.Clt);
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundBinaryOperatorKind.Less:
                    ilProcessor.Emit(OpCodes.Call, _decimalLessReference);
                    break;
                case BoundBinaryOperatorKind.LessEquals:
                    ilProcessor.Emit(OpCodes.Call, _decimalLessEqualReference);
                    break;
                default:
                    throw new Exception($"Unexpected binary operator {SyntaxFacts.GetText(expression.Op.SyntaxKind)} ({expression.Left.Type}, {expression.Right.Type})");
            }
        }

        private void EmitCallExpression(ILProcessor ilProcessor, BoundCallExpression expression)
        {
            foreach (var argument in expression.Arguments)
                EmitExpression(ilProcessor, argument);

            if (expression.Function == BuiltinFunctions.Println)
            {
                ilProcessor.Emit(OpCodes.Call, _consoleWriteLineReference);
            }
            else if (expression.Function == BuiltinFunctions.Input)
            {
                ilProcessor.Emit(OpCodes.Call, _consoleReadLineReference);
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
            EmitExpression(ilProcessor, expression.Expression);
            var needsBoxing = expression.Expression.Type == TypeSymbol.Bool ||
                              expression.Expression.Type == TypeSymbol.Number;

            if (needsBoxing)
                ilProcessor.Emit(OpCodes.Box, _knownTypes[expression.Expression.Type]);

            if (expression.Type == TypeSymbol.Any)
            {

            }
            else if (expression.Type == TypeSymbol.Bool)
            {
                ilProcessor.Emit(OpCodes.Call, _convert2BoolReference);
            }
            else if (expression.Type == TypeSymbol.Number)
            {
                ilProcessor.Emit(OpCodes.Call, _convert2DecimalReference);
            }
            else if (expression.Type == TypeSymbol.String)
            {
                ilProcessor.Emit(OpCodes.Call, _convert2StringReference);
            }
            else
                throw new Exception($"Unexpected conversion from {expression.Expression.Type} to {expression.Type}");
        }

        private void EmitArrayExpression(ILProcessor ilProcessor, BoundArrayExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}