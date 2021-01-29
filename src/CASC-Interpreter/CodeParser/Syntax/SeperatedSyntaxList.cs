using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CASC.CodeParser.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeperators();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _nodesAndSeperators;

        public SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodesAndSeperators)
        {
            _nodesAndSeperators = nodesAndSeperators;
        }

        public int Count => (_nodesAndSeperators.Length + 1) / 2;

        public T this[int index] => (T)_nodesAndSeperators[index * 2];

        public SyntaxToken GetSeparator(int index)
        {
            if (index == Count - 1)
                return null;

            return (SyntaxToken)_nodesAndSeperators[index * 2 + 1];
        }

        public override ImmutableArray<SyntaxNode> GetWithSeperators() => _nodesAndSeperators;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}