using System.Collections.Generic;
using System;

namespace CASC.CodeParser.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.BangToken:
                    return 9;

                case SyntaxKind.SquareToken:
                case SyntaxKind.SquareRootToken:
                case SyntaxKind.NthRootToken:
                    return 8;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PointToken:
                    return 7;

                case SyntaxKind.PowerToken:
                case SyntaxKind.NthRootToken:
                    return 6;

                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 5;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                    return 3;

                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;

                case SyntaxKind.PipePipeToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "真":
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "假":
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.PipePipeToken:
                    return "||";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.BangEqualsToken:
                    return "!=";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.TrueKeyword:
                    return "true";
                default:
                    return null;
            }
        }
    }
}