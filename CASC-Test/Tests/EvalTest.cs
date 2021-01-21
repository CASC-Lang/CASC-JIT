using System.Collections.Generic;
using CASC.CodeParser;
using CASC.CodeParser.Syntax;
using NUnit.Framework;

namespace CASC_Test
{
    public class EvalTest
    {
        private Dictionary<VariableSymbol, object> variables;

        [SetUp]
        public void Setup()
        {
            variables = new Dictionary<VariableSymbol, object>();
        }

        public EvaluationResult Evaluate(string input)
        {
            var syntaxTree = SyntaxTree.Parse(input);
            var compilation = new Compilation(syntaxTree);
            return compilation.Evaluate(variables);
        }

        [TestCase("2 + 1", "3")]
        [TestCase("2 - 1", "1")]
        [TestCase("2 * 1", "2")]
        [TestCase("2 / 1", "2")]
        [TestCase("2 + 1 * 3", "5")]
        [TestCase("(2 + 1) * 3", "9")]
        [TestCase("1 == 1", "True")]
        [TestCase("1 == 1.0", "True")]
        [TestCase("1 == 1 + 2", "False")]
        public void EvalTestI(string input, string expected)
        {
            var result = Evaluate(input);

            Assert.AreEqual(result.Value.ToString(), expected);
        }

        [TestCase("二加一", "3")]
        [TestCase("二減一", "1")]
        [TestCase("二乘一", "2")]
        [TestCase("二除一", "2")]
        [TestCase("二加一乘三", "5")]
        [TestCase("開二加一閉乘三", "9")]
        [TestCase("一是一", "True")]
        [TestCase("一是一點零", "True")]
        [TestCase("一是一加二", "False")]

        public void EvalTestZHI(string input, string expected)
        {
            EvalTestI(input, expected);
        }
    }
}