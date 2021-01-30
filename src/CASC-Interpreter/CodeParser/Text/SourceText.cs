using System.Collections.Immutable;

namespace CASC.CodeParser.Text
{
    public sealed class SourceText
    {
        private readonly string _text;

        private SourceText(string text, string fileName)
        {
            _text = text;
            FileName = fileName;
            Lines = ParseLines(this, text);
        }

        public ImmutableArray<TextLine> Lines { get; }
        public char this[int index] => _text[index];
        public int Length => _text.Length;

        public string FileName { get; }

        public override string ToString() => _text;
        public string ToString(int start, int length) => _text.Substring(start, length);
        public string ToString(TextSpan span) => ToString(span.Start, span.Length);

        public int GetLineIndex(int pos)
        {
            var lower = 0;
            var upper = Lines.Length - 1;

            while (lower <= upper)
            {
                var index = lower + (upper - lower) / 2;
                var start = Lines[index].Start;

                if (pos == start)
                    return index;

                if (start > pos)
                {
                    upper = index - 1;
                }
                else
                {
                    lower = index + 1;
                }
            }

            return lower - 1;
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText source, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var pos = 0;
            var lineStart = 0;

            while (pos < text.Length)
            {
                var lineBreakWidth = GetLineBreakWith(text, pos);

                if (lineBreakWidth == 0)
                    pos++;
                else
                {
                    AddLine(result, source, pos, lineStart, lineBreakWidth);

                    pos += lineBreakWidth;
                    lineStart = pos;
                }
            }

            if (pos >= lineStart)
                AddLine(result, source, pos, lineStart, 0);

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder builder, SourceText source, int pos, int lineStart, int lineBreakWidth)
        {
            var lineLength = pos - lineStart;
            var lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            builder.Add(new TextLine(source, lineStart, lineLength, lineLengthIncludingLineBreak));
        }

        private static int GetLineBreakWith(string text, int i)
        {
            var c = text[i];
            var l = i + 1 >= text.Length ? '\0' : text[i + 1];

            if (c == '\r' && l == '\n')
                return 2;

            if (c == '\r' || c == '\n')
                return 1;

            return 0;
        }

        public static SourceText From(string text, string fileName = "")
        {
            return new SourceText(text, fileName);
        }
    }
}