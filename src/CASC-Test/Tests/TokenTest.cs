using System;
using System.Collections.Generic;
using System.Linq;
using CASC.CodeParser.Syntax;
using NUnit.Framework;

namespace CASC_Test.Tests
{
    public class TokenTest
    {
        [Theory]
        [TestCaseSource(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText_RoundTrips(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);
            if (text == null)
                return;

            var tokens = SyntaxTree.ParseTokens(text);
            Assert.That(tokens, Has.Exactly(1).Items);
            var token = tokens.First();
            Assert.AreEqual(kind, token.Kind);
            Assert.AreEqual(text, token.Text);
        }

        [Theory]
        [TestCaseSource(nameof(GetSyntaxKindData))]
        public void TokenTestZHI(SyntaxKind kind)
        {
            var texts = SyntaxFacts.GetZHText(kind);
            if (!texts.Any())
                return;

            foreach (var t in texts)
            {
                var tokens = SyntaxTree.ParseTokens(t);
                Assert.That(tokens, Has.Exactly(1).Items);
                Assert.AreEqual(kind, tokens.First().Kind);
                Assert.That(texts.Any(s => s == t));
            }
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
                yield return new object[] { kind };
        }
    }
}