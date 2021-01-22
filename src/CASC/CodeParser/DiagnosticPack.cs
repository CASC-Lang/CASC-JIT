using CASC.CodeParser.Syntax;
using System;
using System.Collections.Generic;
using System.Collections;
using CASC.CodeParser.Text;

namespace CASC.CodeParser
{
    internal sealed class DiagnosticPack : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticPack diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }
        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"ERROR: The Number {text} isn't valid {type}.";
            Report(span, message);
        }

        public void ReportBadCharacter(int position, char badChar)
        {
            var span = new TextSpan(position, 1);
            var message = $"ERROR: Bad Character input: '{badChar}'.";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"ERROR: Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type type)
        {
            var message = $"ERROR: Unary operator '{operatorText}' is not defined for type '{type}'.";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            var message = $"ERROR: Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"ERROR: Variable '{name}' does not exist.";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, Type fromType, Type toType)
        {
            var message = $"ERROR: Cannot convert type '{fromType}' to '{toType}'.";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"ERROR: Variable '{name}' is already declared.";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan span, string name)
        {
            var message = $"ERROR: Variable '{name}' is finalized and cannot be assigned to.";
            Report(span, message);
        }
    }
}