using System.Linq;
using System;
using System.Collections.Generic;
using CASC.CodeParser.Syntax;
using NUnit.Framework;

namespace CASC_Test.Tests
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> _enumerator;
        private bool _hasErrors;

        public AssertingEnumerator(SyntaxNode node)
        {
            _enumerator = Flatten(node).GetEnumerator();
        }

        private bool MarkFailed()
        {
            _hasErrors = true;
            return false;
        }

        public void Dispose()
        {
            if (!_hasErrors)
                Assert.False(_enumerator.MoveNext());

            _enumerator.Dispose();
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                yield return n;

                foreach (var child in n.GetChildren().Reverse())
                    stack.Push(child);
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.AreEqual(kind, _enumerator.Current.Kind);
                Assert.IsNotInstanceOf<SyntaxToken>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertToken(SyntaxKind kind, string text)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.AreEqual(kind, _enumerator.Current.Kind);
                Assert.IsInstanceOf<SyntaxToken>(_enumerator.Current);
                var token = _enumerator.Current as SyntaxToken;
                Assert.AreEqual(text, token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }
    }
}