namespace CASC.CodeParser.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText source, int start, int length, int lengthIncludingLineBreak)
        {
            Source = source;
            Start = start;
            Length = length;
            LengthIncludingLineBreak = lengthIncludingLineBreak;
        }

        public SourceText Source { get; }
        public int Start { get; }
        public int End => Start + Length;
        public int Length { get; }
        public int LengthIncludingLineBreak { get; }
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanIncludingLineBreak => new TextSpan(Start, LengthIncludingLineBreak);
        public override string ToString() => Source.ToString(Span);
    }
}