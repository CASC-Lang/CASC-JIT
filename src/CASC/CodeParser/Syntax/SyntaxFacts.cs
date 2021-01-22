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
                    return 6;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
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

                case "讓":
                case "使":
                case "let":
                    return SyntaxKind.LetKeyword;

                case "變數":
                case "變值":
                case "var":
                    return SyntaxKind.VarKeyword;

                case "終值":
                case "val":
                    return SyntaxKind.ValKeyword;

                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
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
                case SyntaxKind.OpenParenthesesToken:
                    return "(";
                case SyntaxKind.CloseParenthesesToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.ValKeyword:
                    return "val";
                default:
                    return null;
            }
        }

        public static string GetZHText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "加";
                case SyntaxKind.MinusToken:
                    return "減";
                case SyntaxKind.StarToken:
                    return "乘";
                case SyntaxKind.SlashToken:
                    return "除";
                case SyntaxKind.BangToken:
                    return "反";
                case SyntaxKind.EqualsToken:
                    return "賦";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "且";
                case SyntaxKind.PipePipeToken:
                    return "或";
                case SyntaxKind.EqualsEqualsToken:
                    return "是";
                case SyntaxKind.BangEqualsToken:
                    return "不是";
                case SyntaxKind.OpenParenthesesToken:
                    return "(";
                case SyntaxKind.CloseParenthesesToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.FalseKeyword:
                    return "假";
                case SyntaxKind.TrueKeyword:
                    return "真";
                case SyntaxKind.LetKeyword:
                    return "讓";
                case SyntaxKind.VarKeyword:
                    return "變數";
                case SyntaxKind.ValKeyword:
                    return "終值";
                default:
                    return null;
            }
        }
    }
}