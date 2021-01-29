using System;
using System.Collections.Generic;
using CASC.CodeParser;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;
using CASC_Test.Tests.Objects;
using NUnit.Framework;

namespace CASC_Test.Tests
{
    class EnglishDiagnosticTest
    {
        [Test]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 10
                    }
                    var [x] = 5
                }
            ";

            var diagnostics = @"
                'x' is already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Name_Reports_Undefined()
        {
            var text = @"[x] * 10";

            var diagnostics = @"
                Variable 'x' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Assigned_Reports_Undefined()
        {
            var text = @"[x] = 10";

            var diagnostics = @"
                Variable 'x' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Assigned_Reports_CannotAssign()
        {
            var text = @"
                {
                    val x = 10
                    x [=] 0
                }
            ";

            var diagnostics = @"
                Variable 'x' is finalized and cannot be assigned to.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Unary_Reports_Undefined()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'bool'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Binary_Reports_Undefined()
        {
            var text = @"10 [*] false";

            var diagnostics = @"
                Binary operator '*' is not defined for types 'number' and 'bool'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Variables_Can_Shadow_Functions()
        {
            var text = @"
                {
                    let print = 42
                    [print](""test"")
                }
            ";

            var diagnostics = @"
                Function 'print' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_InvokeFunctionArguments_NoInfiniteLoop()
        {
            var text = @"
                print(""Hi""[[=]][)]
            ";

            var diagnostics = @"
                Unexpected token <EqualsToken>, expected <CloseParenthesisToken>.
                Unexpected token <EqualsToken>, expected <IdentifierToken>.
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_FunctionParameters_NoInfiniteLoop()
        {
            var text = @"
                func hi(name: string[[[=]]][)]
                {
                    print(""Hi "" + name + ""!"" )
                }[]
            ";

            var diagnostics = @"
                Unexpected token <EqualsToken>, expected <CloseParenthesisToken>.
                Unexpected token <EqualsToken>, expected <OpenBraceToken>.
                Unexpected token <EqualsToken>, expected <IdentifierToken>.
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("Must mark as many spans as there are expected diagnostics");

            Assert.AreEqual(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.AreEqual(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.AreEqual(expectedSpan, actualSpan);
            }
        }
    }

    class MandarinDiagnosticTest
    {
        [Test]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
                {
                    變數 甲 賦 十
                    變數 乙 賦 一百
                    {
                        變數 甲 賦 十
                    }
                    變數 [甲] = 五
                }
            ";

            var diagnostics = @"
                '甲' is already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Name_Reports_Undefined()
        {
            var text = @"[甲] 乘 十";

            var diagnostics = @"
                Variable '甲' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Assigned_Reports_Undefined()
        {
            var text = @"[甲] 賦 十";

            var diagnostics = @"
                Variable '甲' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Assigned_Reports_CannotAssign()
        {
            var text = @"
                {
                    終值 甲 賦 十
                    甲 [賦] 零
                }
            ";

            var diagnostics = @"
                Variable '甲' is finalized and cannot be assigned to.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Unary_Reports_Undefined()
        {
            var text = @"[正]真";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'bool'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Binary_Reports_Undefined()
        {
            var text = @"十 [乘] 假";

            var diagnostics = @"
                Binary operator '*' is not defined for types 'number' and 'bool'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Variables_Can_Shadow_Functions()
        {
            var text = @"
                {
                    讓 print 賦 四十二
                    [print](""test"")
                }
            ";

            var diagnostics = @"
                Function 'print' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_InvokeFunctionArguments_NoInfiniteLoop()
        {
            var text = @"
                print(""你好""[[=]][)]
            ";

            var diagnostics = @"
                Unexpected token <EqualsToken>, expected <CloseParenthesisToken>.
                Unexpected token <EqualsToken>, expected <IdentifierToken>.
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_FunctionParameters_NoInfiniteLoop()
        {
            var text = @"
                函式 你好(名字: string[[[=]]][)]
                {
                    print(""你好 "" + 名字 + ""!"" )
                }[]
            ";

            var diagnostics = @"
                Unexpected token <EqualsToken>, expected <CloseParenthesisToken>.
                Unexpected token <EqualsToken>, expected <OpenBraceToken>.
                Unexpected token <EqualsToken>, expected <IdentifierToken>.
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("Must mark as many spans as there are expected diagnostics");

            Assert.AreEqual(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.AreEqual(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.AreEqual(expectedSpan, actualSpan);
            }
        }
    }
}