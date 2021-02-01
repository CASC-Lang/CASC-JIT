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
                case SyntaxKind.TildeToken:
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
                case SyntaxKind.GreaterEqualsToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.LessEqualsToken:
                case SyntaxKind.LessToken:
                    return 3;

                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.AmpersandToken:
                    return 2;

                case SyntaxKind.PipePipeToken:
                case SyntaxKind.PipeToken:
                case SyntaxKind.HatToken:
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

                case "返回":
                case "return":
                    return SyntaxKind.ReturnKeyword;

                case "如果":
                case "若":
                case "if":
                    return SyntaxKind.IfKeyword;

                case "否則":
                case "else":
                    return SyntaxKind.ElseKeyword;

                case "當":
                case "while":
                    return SyntaxKind.WhileKeyword;

                case "持續":
                case "do":
                    return SyntaxKind.DoKeyword;

                case "從":
                case "for":
                    return SyntaxKind.ForKeyword;

                case "跳離":
                case "break":
                    return SyntaxKind.BreakKeyword;

                case "繼續":
                case "continue":
                    return SyntaxKind.ContinueKeyword;

                case "到":
                case "to":
                    return SyntaxKind.ToKeyword;

                case "函式":
                case "func":
                    return SyntaxKind.FunctionKeyword;

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
                case SyntaxKind.TildeToken:
                    return "~";
                case SyntaxKind.HatToken:
                    return "^";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.GreaterEqualsToken:
                    return ">=";
                case SyntaxKind.GreaterToken:
                    return ">";
                case SyntaxKind.LessEqualsToken:
                    return "<=";
                case SyntaxKind.LessToken:
                    return "<";
                case SyntaxKind.AmpersandToken:
                    return "&";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.PipeToken:
                    return "|";
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
                case SyntaxKind.OpenBracketToken:
                    return "[";
                case SyntaxKind.CloseBracketToken:
                    return "]";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.CommaToken:
                    return ",";
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
                case SyntaxKind.ReturnKeyword:
                    return "return";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.DoKeyword:
                    return "do";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.BreakKeyword:
                    return "break";
                case SyntaxKind.ContinueKeyword:
                    return "continue";
                case SyntaxKind.ToKeyword:
                    return "to";
                case SyntaxKind.FunctionKeyword:
                    return "func";
                default:
                    return null;
            }
        }

        public static IEnumerable<string> GetZHText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    yield return "加";
                    yield return "正";
                    break;
                case SyntaxKind.MinusToken:
                    yield return "減";
                    yield return "負";
                    break;
                case SyntaxKind.StarToken:
                    yield return "乘";
                    break;
                case SyntaxKind.SlashToken:
                    yield return "除";
                    break;
                case SyntaxKind.TildeToken:
                    yield return "~";
                    break;
                case SyntaxKind.HatToken:
                    yield return "^";
                    break;
                case SyntaxKind.BangToken:
                    yield return "反";
                    break;
                case SyntaxKind.EqualsToken:
                    yield return "賦";
                    break;
                case SyntaxKind.GreaterEqualsToken:
                    yield return "大等於";
                    break;
                case SyntaxKind.GreaterToken:
                    yield return "大於";
                    break;
                case SyntaxKind.LessEqualsToken:
                    yield return "小等於";
                    break;
                case SyntaxKind.LessToken:
                    yield return "小於";
                    break;
                case SyntaxKind.AmpersandToken:
                    yield return "&";
                    break;
                case SyntaxKind.AmpersandAmpersandToken:
                    yield return "且";
                    break;
                case SyntaxKind.PipeToken:
                    yield return "|";
                    break;
                case SyntaxKind.PipePipeToken:
                    yield return "或";
                    break;
                case SyntaxKind.EqualsEqualsToken:
                    yield return "是";
                    break;
                case SyntaxKind.BangEqualsToken:
                    yield return "不是";
                    break;
                case SyntaxKind.OpenParenthesisToken:
                    yield return "(";
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    yield return ")";
                    break;
                case SyntaxKind.OpenBracketToken:
                    yield return "[";
                    break;
                case SyntaxKind.CloseBracketToken:
                    yield return "]";
                    break;
                case SyntaxKind.OpenBraceToken:
                    yield return "{";
                    break;
                case SyntaxKind.CloseBraceToken:
                    yield return "}";
                    break;
                case SyntaxKind.ColonToken:
                    yield return ":";
                    break;
                case SyntaxKind.CommaToken:
                    yield return ",";
                    break;
                case SyntaxKind.FalseKeyword:
                    yield return "假";
                    break;
                case SyntaxKind.TrueKeyword:
                    yield return "真";
                    break;
                case SyntaxKind.LetKeyword:
                    yield return "讓";
                    yield return "使";
                    break;
                case SyntaxKind.VarKeyword:
                    yield return "變數";
                    yield return "變值";
                    break;
                case SyntaxKind.ValKeyword:
                    yield return "終值";
                    break;
                case SyntaxKind.ReturnKeyword:
                    yield return "返回";
                    break;
                case SyntaxKind.IfKeyword:
                    yield return "如果";
                    yield return "若";
                    break;
                case SyntaxKind.ElseKeyword:
                    yield return "否則";
                    break;
                case SyntaxKind.WhileKeyword:
                    yield return "當";
                    break;
                case SyntaxKind.DoKeyword:
                    yield return "持續";
                    break;
                case SyntaxKind.ForKeyword:
                    yield return "從";
                    break;
                case SyntaxKind.BreakKeyword:
                    yield return "跳離";
                    break;
                case SyntaxKind.ContinueKeyword:
                    yield return "繼續";
                    break;
                case SyntaxKind.ToKeyword:
                    yield return "到";
                    break;
                case SyntaxKind.FunctionKeyword:
                    yield return "函式";
                    break;
                default:
                    yield break;
            }
        }
    }
}