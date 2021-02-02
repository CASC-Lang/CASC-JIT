using System;
using System.Collections.Generic;
using CASC.CodeParser;
using CASC.CodeParser.Symbols;
using CASC.CodeParser.Syntax;
using NUnit.Framework;

namespace CASC_Test
{
    public class EvalTest
    {
        private Dictionary<VariableSymbol, object> variables;
        Compilation previous = null;

        [SetUp]
        public void Setup()
        {
            variables = new Dictionary<VariableSymbol, object>();
        }

        public EvaluationResult Evaluate(string input)
        {
            var syntaxTree = SyntaxTree.Parse(input);
            var compilation = Compilation.CreateScript(null, syntaxTree);

            return compilation.Evaluate(variables);
        }

        [TestCase("2 + 1", 3)]
        [TestCase("2 - 1", 1)]
        [TestCase("2 * 1", 2)]
        [TestCase("2 / 1", 2)]
        [TestCase("-1", -1)]
        [TestCase("~1", -2)]
        [TestCase("2 + 1 * 3", 5)]
        [TestCase("(2 + 1) * 3", 9)]
        [TestCase("1 | 2", 3)]
        [TestCase("1 | 0", 1)]
        [TestCase("1 & 3", 1)]
        [TestCase("1 & 0", 0)]
        [TestCase("1 ^ 0", 1)]
        [TestCase("0 ^ 1", 1)]
        [TestCase("1 ^ 3", 2)]
        [TestCase("1 == 1", true)]
        [TestCase("1 == 1.0", true)]
        [TestCase("1 == 1 + 2", false)]
        [TestCase("true == false", false)]
        [TestCase("true == true", true)]
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("!true", false)]
        [TestCase("!false", true)]
        [TestCase("true && true", true)]
        [TestCase("true && false", false)]
        [TestCase("true || true", true)]
        [TestCase("true || false", true)]
        [TestCase("false || false", false)]
        [TestCase("false | false", false)]
        [TestCase("false | true", true)]
        [TestCase("true | false", true)]
        [TestCase("true | true", true)]
        [TestCase("false & false", false)]
        [TestCase("false & true", false)]
        [TestCase("true & false", false)]
        [TestCase("true & true", true)]
        [TestCase("false ^ false", false)]
        [TestCase("true ^ false", true)]
        [TestCase("false ^ true", true)]
        [TestCase("true ^ true", false)]
        [TestCase("5 > 4", true)]
        [TestCase("5 > 9", false)]
        [TestCase("5 >= 4", true)]
        [TestCase("5 >= 5", true)]
        [TestCase("5 >= 6", false)]
        [TestCase("5 < 4", false)]
        [TestCase("5 < 9", true)]
        [TestCase("5 <= 4", false)]
        [TestCase("5 <= 5", true)]
        [TestCase("5 <= 6", true)]
        [TestCase("var a = 10 return a", 10)]
        [TestCase("\"test\"", "test")]
        [TestCase("\"test\" == \"test\"", true)]
        [TestCase("\"test\" != \"test\"", false)]
        [TestCase("if 10 > 100 return 1 + 10 else return 10 * 10", 100)]
        [TestCase("{ var a = 1 while a < 3 { a = a + 1 } return a }", 3)]
        [TestCase("{ var a = 0 for i = 0 to 100 { a = i + a } return a }", 5050)]
        [TestCase("{ var a = 10 for i = 1 to (a = a - 1) { } return a }", 9)]
        [TestCase("{ var a = 0 do a = a + 1 while a < 10 return a}", 10)]
        [TestCase("{ var i = 0 while i < 5 { i = i + 1 if i == 5 continue } return i }", 5)]
        [TestCase("{ var i = 0 do { i = i + 1 if i == 5 continue } while i < 5 return i }", 5)]
        public void EvalTestI(string input, object expected)
        {
            var result = Evaluate(input);

            Assert.That(result.Diagnostics, Has.Exactly(0).Count);
            Assert.AreEqual(result.Value, expected);
        }

        [TestCase("二 加 一", 3)]
        [TestCase("二 減 一", 1)]
        [TestCase("二 乘 一", 2)]
        [TestCase("二 除 一", 2)]
        [TestCase("二 加 一 乘 三", 5)]
        [TestCase("(二 加 一) 乘 三", 9)]
        [TestCase("一 是 一", true)]
        [TestCase("一 是 一點零", true)]
        [TestCase("一 是 一 加 二", false)]
        [TestCase("真 是 假", false)]
        [TestCase("真 是 真", true)]
        [TestCase("真", true)]
        [TestCase("假", false)]
        [TestCase("反 真", false)]
        [TestCase("反 假", true)]
        [TestCase("真 且 真", true)]
        [TestCase("真 且 假", false)]
        [TestCase("真 或 真", true)]
        [TestCase("真 或 假", true)]
        [TestCase("假 或 假", false)]
        [TestCase("五 大於 四", true)]
        [TestCase("五 大於 九", false)]
        [TestCase("五 大等於 四", true)]
        [TestCase("五 大等於 五", true)]
        [TestCase("五 大等於 六", false)]
        [TestCase("五 小於 四", false)]
        [TestCase("五 小於 九", true)]
        [TestCase("五 小等於 四", false)]
        [TestCase("五 小等於 五", true)]
        [TestCase("五 小等於 六", true)]
        [TestCase("變數 甲 賦 十 返回 甲", 10)]
        [TestCase("\"測試\"", "測試")]
        [TestCase("\"測試\" 是 \"測試\"", true)]
        [TestCase("\"測試\" 不是 \"測試\"", false)]
        [TestCase("若 十 大於 一百 返回 一 加 十 否則 返回 十 乘 十", 100)]
        [TestCase("{ 變數 甲 賦 一 當 甲 小於 三 { 甲 賦 甲 加 一 甲 } 返回 甲 }", 3)]
        [TestCase("{ 變數 甲 賦 零 從 索引值 賦 零 到 一百 { 甲 = 索引值 加 甲 } 返回 甲 }", 5050)]
        [TestCase("{ 變數 甲 賦 十 從 索引值 賦 一 到 (甲 賦 甲 減 一) { } 返回 甲 }", 9)]
        [TestCase("{ 變數 甲 賦 零 持續 甲 賦 甲 加 一 當 甲 小於 十 返回 甲 }", 10)]
        [TestCase("{ 變數 甲 賦 零 當 甲 小於 五 { 甲 賦 甲 加 一 如果 甲 是 五 繼續 } 返回 甲 }", 5)]
        [TestCase("{ 變數 甲 賦 零 持續 { 甲 賦 甲 加 一 如果 甲 是 五 繼續 } 當 甲 小於 五 返回 甲 }", 5)]
        public void EvalTestZHI(string input, object expected)
        {
            var result = Evaluate(input);

            Assert.That(result.Diagnostics, Has.Exactly(0).Count);
            Assert.AreEqual(result.Value, expected);
        }
    }
}