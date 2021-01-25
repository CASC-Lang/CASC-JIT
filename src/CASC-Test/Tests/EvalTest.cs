using System.Collections.Generic;
using CASC.CodeParser;
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
            var compilation = previous == null
                                ? new Compilation(syntaxTree)
                                : previous.ContinueWith(syntaxTree);
            return compilation.Evaluate(variables);
        }

        [TestCase("2 + 1", 3)]
        [TestCase("2 - 1", 1)]
        [TestCase("2 * 1", 2)]
        [TestCase("2 / 1", 2)]
        [TestCase("2 + 1 * 3", 5)]
        [TestCase("(2 + 1) * 3", 9)]
        [TestCase("1 == 1", true)]
        [TestCase("1 == 1.0", true)]
        [TestCase("1 == 1 + 2", false)]
        [TestCase("true == false", false)]
        [TestCase("true == true", true)]
        [TestCase("!true", false)]
        [TestCase("!false", true)]
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
        public void EvalTestI(string input, object expected)
        {
            var result = Evaluate(input);

            Assert.That(result.Diagnostics, Has.Exactly(0).Count);
            Assert.AreEqual(result.Value, expected);
        }

        [TestCase("二加一", 3)]
        [TestCase("二減一", 1)]
        [TestCase("二乘一", 2)]
        [TestCase("二除一", 2)]
        [TestCase("二加一乘三", 5)]
        [TestCase("(二加一)乘三", 9)]
        [TestCase("一是一", true)]
        [TestCase("一是一點零", true)]
        [TestCase("一是一加二", false)]
        [TestCase("真 是 假", false)]
        [TestCase("真 是 真", true)]
        [TestCase("反真", false)]
        [TestCase("反假", true)]
        [TestCase("五大於四", true)]
        [TestCase("五大於九", false)]
        [TestCase("五大等於四", true)]
        [TestCase("五大等於五", true)]
        [TestCase("五大等於六", false)]
        [TestCase("五小於四", false)]
        [TestCase("五小於九", true)]
        [TestCase("五小等於四", false)]
        [TestCase("五小等於五", true)]
        [TestCase("五小等於六", true)]
        public void EvalTestZHI(string input, object expected)
        {
            var result = Evaluate(input);

            Assert.That(result.Diagnostics, Has.Exactly(0).Count);
            Assert.AreEqual(result.Value, expected);
        }
    }
}