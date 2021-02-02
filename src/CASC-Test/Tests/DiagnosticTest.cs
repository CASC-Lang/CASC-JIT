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

        [Test]
        public void Evaluator_InvokeFunctionArguments_Missing()
        {
            var text = @"
                print([)]
            ";

            var diagnostics = @"
                Function 'print' requires 1 arguments but was given 0.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_InvokeFunctionArguments_Exceeding()
        {
            var text = @"
                print(""Hello""[, "" "", "" world!""])
            ";

            var diagnostics = @"
                Function 'print' requires 1 arguments but was given 3.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Void_Function_Should_Not_Return_Value()
        {
            var text = @"
                func test()
                {
                    return [1]
                }
            ";

            var diagnostics = @"
                Since the function 'test' doesn't return a value the 'return' keyword cannot be followed by an expression.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Function_With_ReturnValue_Should_Not_Return_Void()
        {
            var text = @"
                func test(): number
                {
                    [return]
                }
            ";

            var diagnostics = @"
                An expression of type 'number' expected.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Not_All_Code_Paths_Return_Value()
        {
            var text = @"
                func [test](n: number): bool
                {
                    if (n > 10)
                       return true
                }
            ";

            var diagnostics = @"
                Not all code paths return a value.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Expression_Must_Have_Value()
        {
            var text = @"
                func test(n: number)
                {
                    return
                }
                let value = [test(100)]
            ";

            var diagnostics = @"
                Expression must have a value.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Theory]
        [TestCase("[break]", "break")]
        [TestCase("[continue]", "continue")]
        public void Evaluator_Invalid_Break_Or_Continue(string text, string keyword)
        {
            var diagnostics = $@"
                The keyword '{keyword}' can only be used inside of loops.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Invalid_Return()
        {
            var text = @"
                return
            ";

            AssertValue(text, "");
        }

        [Test]
        public void Evaluator_Parameter_Already_Declared()
        {
            var text = @"
                func sum(a: number, b: number, [a: number]): number
                {
                    return a + b + c
                }
            ";

            var diagnostics = @"
                A parameter with the name 'a' is already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Function_Must_Have_Name()
        {
            var text = @"
                func [(]a: number, b: number): number
                {
                    return a + b
                }
            ";

            var diagnostics = @"
                Unexpected token <OpenParenthesisToken>, expected <IdentifierToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Wrong_Argument_Type()
        {
            var text = @"
                func test(n: number): bool
                {
                    return n > 10
                }
                let testValue = ""string""
                test([testValue])
            ";

            var diagnostics = @"
                Cannot implicitly convert type 'string' to 'number'. An explicit conversion exist (missing a cast?)
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Bad_Type()
        {
            var text = @"
                func test(n: [invalidtype])
                {
                }
            ";

            var diagnostics = @"
                Type 'invalidtype' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_AssignmentExpression_Reports_NotAVariable()
        {
            var text = @"[print] = 42";

            var diagnostics = @"
                'print' is not a variable.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_CallExpression_Reports_Undefined()
        {
            var text = @"[foo](42)";

            var diagnostics = @"
                Function 'foo' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_CallExpression_Reports_NotAFunction()
        {
            var text = @"
                {
                    let foo = 42
                    [foo](42)
                }
            ";

            var diagnostics = @"
                'foo' is not a function.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = Compilation.CreateScript(null, syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.IsEmpty(result.Diagnostics);
            Assert.AreEqual(expectedValue, result.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = Compilation.CreateScript(null, syntaxTree);
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
                var actualSpan = result.Diagnostics[i].Location.Span;
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
                Unary operator '正' is not defined for type 'bool'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Binary_Reports_Undefined()
        {
            var text = @"十 [乘] 假";

            var diagnostics = @"
                Binary operator '乘' is not defined for types 'number' and 'bool'.
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

        [Test]
        public void Evaluator_InvokeFunctionArguments_Missing()
        {
            var text = @"
                print([)]
            ";

            var diagnostics = @"
                Function 'print' requires 1 arguments but was given 0.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_InvokeFunctionArguments_Exceeding()
        {
            var text = @"
                print(""你好""[, "" "", "" 世界!""])
            ";

            var diagnostics = @"
                Function 'print' requires 1 arguments but was given 3.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Void_Function_Should_Not_Return_Value()
        {
            var text = @"
                函式 測試()
                {
                    返回 [一]
                }
            ";

            var diagnostics = @"
                Since the function '測試' doesn't return a value the 'return' keyword cannot be followed by an expression.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Function_With_ReturnValue_Should_Not_Return_Void()
        {
            var text = @"
                函式 測試(): number
                {
                    [return]
                }
            ";

            var diagnostics = @"
                An expression of type 'number' expected.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Not_All_Code_Paths_Return_Value()
        {
            var text = @"
                函式 [測試](甲: number): bool
                {
                    如果 (甲 大於 十)
                       返回 真
                }
            ";

            var diagnostics = @"
                Not all code paths return a value.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Expression_Must_Have_Value()
        {
            var text = @"
                函式 測試(甲: number)
                {
                    返回
                }
                讓 值 賦 [測試(一百)]
            ";

            var diagnostics = @"
                Expression must have a value.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Invalid_Return()
        {
            var text = @"
                返回
            ";

            AssertValue(text, "");
        }

        [Test]
        public void Evaluator_Wrong_Argument_Type()
        {
            var text = @"
                函式 測試(甲: number): bool
                {
                    返回 甲 大於 十
                }
                讓 測試值 = ""字串""
                測試([測試值])
            ";

            var diagnostics = @"
                Cannot implicitly convert type 'string' to 'number'. An explicit conversion exist (missing a cast?)
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Evaluator_Bad_Type()
        {
            var text = @"
                函式 測試(甲: [無效種類])
                {
                }
            ";

            var diagnostics = @"
                Type '無效種類' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = Compilation.CreateScript(null, syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.IsEmpty(result.Diagnostics);
            Assert.AreEqual(expectedValue, result.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = Compilation.CreateScript(null, syntaxTree);
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
                var actualSpan = result.Diagnostics[i].Location.Span;
                Assert.AreEqual(expectedSpan, actualSpan);
            }
        }
    }
}