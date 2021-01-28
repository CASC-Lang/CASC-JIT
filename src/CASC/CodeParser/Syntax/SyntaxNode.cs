using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CASC.CodeParser.Text;

namespace CASC.CodeParser.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        public IEnumerable<SyntaxNode> GetChildren()
        {
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(prop.PropertyType))
                {
                    var child = (SyntaxNode)prop.GetValue(this);

                    if (child != null)
                        yield return child;
                }
                else if (typeof(SyntaxNode).IsAssignableFrom(prop.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)prop.GetValue(this);

                    foreach (var child in children)
                        if (child != null)
                            yield return child;
                }
            }
        }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            return GetChildren().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "└── " : "├── ";

            writer.Write(indent);

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                writer.Write(marker);
                Console.ResetColor();
            }

            if (isToConsole)
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.DarkBlue : ConsoleColor.DarkCyan;

            writer.Write(node.Kind);

            if (node is SyntaxToken T && T != null)
            {
                writer.Write(" ");
                writer.Write(T.Value);
            }

            if (isToConsole)
                Console.ResetColor();

            writer.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}