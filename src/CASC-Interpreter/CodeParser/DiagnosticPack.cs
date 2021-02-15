using CASC.CodeParser.Syntax;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using CASC.CodeParser.Text;
using CASC.CodeParser.Symbols;
using Mono.Cecil;

namespace CASC.CodeParser
{
    internal sealed class DiagnosticPack : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticPack diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }
        private void Report(TextLocation location, string message)
        {
            var diagnostic = new Diagnostic(location, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type)
        {
            var message = $"The Number {text} isn't valid {type}.";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char badChar)
        {
            var message = $"Bad Character input: '{badChar}'.";
            Report(location, message);
        }

        public void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol type)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{type}'.";
            Report(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
            Report(location, message);
        }

        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            var message = $"Variable '{name}' doesn't exist.";
            Report(location, message);
        }

        public void ReportNotAVariable(TextLocation location, string name)
        {
            var message = $"'{name}' is not a variable.";
            Report(location, message);
        }

        public void ReportNotAFunction(TextLocation location, string name)
        {
            var message = $"'{name}' is not a function.";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            var message = $"Function '{name}' doesn't exist.";
            Report(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string parameterName)
        {
            var message = $"A parameter with the name '{parameterName}' is already declared.";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            var message = $"Type '{name}' doesn't exist.";
            Report(location, message);
        }


        public void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert type '{fromType}' to '{toType}'.";
            Report(location, message);
        }

        public void ReportFunctionsAreUnsupported(TextLocation location)
        {
            var message = "Functions with return values are unsupported.";
            Report(location, message);
        }

        public void ReportCannotConvertImplicity(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot implicitly convert type '{fromType}' to '{toType}'. An explicit conversion exist (missing a cast?)";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            var message = $"'{name}' is already declared.";
            Report(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            var message = $"Variable '{name}' is finalized and cannot be assigned to.";
            Report(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            var message = $"The keyword '{text}' can only be used inside of loops.";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            var message = "Unterminated string literal.";
            Report(location, message);
        }

        public void ReportArgumentCountMismatch(TextLocation location, string name, int expectedCount, int actualCount)
        {
            var message = $"Function '{name}' requires {expectedCount} arguments but was given {actualCount}.";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            Report(location, "Expression must have a value.");
        }

        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            var message = $"Since the function '{functionName}' doesn't return a value the 'return' keyword cannot be followed by an expression.";
            Report(location, message);
        }

        public void ReportInvalidReturnWithValueInGlobalStatements(TextLocation location)
        {
            var message = "The 'return' keyword cannot be followed by an expression in global statements.";
            Report(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            var message = $"An expression of type '{returnType}' expected.";
            Report(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            var message = "Not all code paths return a value.";
            Report(location, message);
        }

        public void ReportOnlyOneFileCanHaveGlobalStatements(TextLocation location)
        {
            var message = "At most one file can have global statements.";
            Report(location, message);
        }

        public void ReportMainMustHaveCorrectSignature(TextLocation location)
        {
            var message = "main must not take arguments and not return anything.";
            Report(location, message);
        }

        public void ReportCannotMixMainAndGlobalStatements(TextLocation location)
        {
            var message = "Cannot declare main function when global statements are used.";
            Report(location, message);
        }

        public void ReportInvalidExpressionStatement(TextLocation location)
        {
            var message = "Only assignment and call expressions can be used as a statement.";
            Report(location, message);
        }
        
        public void ReportInvalidReference(string path)
        {
            var message = $"The reference is not a valid .NET assembly: {path}.";
            Report(default, message);
        }
        
        public void ReportRequiredTypeNotFound(string cascName, string metadataName)
        {
            var message = cascName == null
                ? $"The required type '{cascName}' ('{metadataName}') cannot be resolve among the given references."
                : $"The required type '{metadataName}' cannot be resolve among the given references.";
            Report(default, message);
        }
        
        public void ReportRequiredTypeAmbiguous(string cascName, string metadataName, TypeDefinition[] foundTypes)
        {
            var assemblyNames = foundTypes.Select(t => t.Module.Assembly.Name.Name);
            var assemblyNameList = string.Join(", ", assemblyNames);
            var message = cascName == null
                ? $"The required type '{metadataName}' was found in multiple references: {assemblyNameList}."
                : $"The required type '{cascName}' ('{metadataName}') was found in multiple references: {assemblyNameList}.";
            Report(default, message);
        }
        
        public void ReportRequiredMethodNotFound(string typeName, string methodName, string[] parameterTypeNames)
        {
            var assemblyNameList = string.Join(", ", parameterTypeNames);
            var message = $"The required method '{typeName}.{methodName}({parameterTypeNames})' cannot be resolve among the given references: {assemblyNameList}.";
            Report(default, message);
        }
    }
}