using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CASC.CodeParser.Syntax
{
    public abstract class SeperatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeperators();
    }

    public sealed class SeperatedSyntaxList<T> : SeperatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _nodesAndSeperators;

        public SeperatedSyntaxList(ImmutableArray<SyntaxNode> nodesAndSeperators)
        {
            _nodesAndSeperators = nodesAndSeperators;
        }

        public int Count => (_nodesAndSeperators.Length + 1) / 2;

        public T this[int index] => (T)_nodesAndSeperators[index * 2];

        public SyntaxToken GetSeperator(int index)
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