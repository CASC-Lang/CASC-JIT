using CASC.CodeParser.Syntax;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CASC_Test.CodeParser.Syntax
{
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //                                                                 \\
    // DEPRECATED TEST RUN / DEPRECATED TEST RUN / DEPRECATED TEST RUN  >>
    //                                                                 //
    ////////////////////////////////////////////////////////////////////
    public class LexerTests
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_TokenPairs(SyntaxKind t1Kind, string t1Text,
                                           SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(tokens[0].Kind, t1Kind);
            Assert.Equal(tokens[0].Text, t1Text);
            Assert.Equal(tokens[1].Kind, t2Kind);
            Assert.Equal(tokens[1].Text, t2Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorData))]
        public void Lexer_Lexes_TokenPairs_WithSeparators(SyntaxKind t1Kind, string t1Text,
                                                          SyntaxKind separatorKind, string separatorText,
                                                          SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + separatorText + t2Text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);
            Assert.Equal(tokens[0].Kind, t1Kind);
            Assert.Equal(tokens[0].Text, t1Text);
            Assert.Equal(tokens[1].Kind, separatorKind);
            Assert.Equal(tokens[1].Text, separatorText);
            Assert.Equal(tokens[2].Kind, t2Kind);
            Assert.Equal(tokens[2].Text, t2Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens().Concat(GetSeparators()))
                yield return new object[] { t.kind, t.text };
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
                yield return new object[] { t.t1Kind, t.t1Text, t.t2Kind, t.t2Text };
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
                yield return new object[] { t.t1Kind, t.t1Text, t.separatorKind, t.separatorText, t.t2Kind, t.t2Text };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
            {
                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.PlusToken, "加"),
                (SyntaxKind.PlusToken, "正"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.MinusToken, "減"),
                (SyntaxKind.MinusToken, "負"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.StarToken, "乘"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.SlashToken, "除"),
                (SyntaxKind.BangToken, "!"),
                (SyntaxKind.BangToken, "反"),
                (SyntaxKind.EqualsToken, "="),
                //(SyntaxKind.EqualsToken, "賦"),
                (SyntaxKind.AmpersandAmpersandToken, "&&"),
                (SyntaxKind.AmpersandAmpersandToken, "且"),
                (SyntaxKind.PipePipeToken, "||"),
                (SyntaxKind.PipePipeToken, "或"),
                (SyntaxKind.EqualsEqualsToken, "=="),
                (SyntaxKind.EqualsEqualsToken, "是"),
                (SyntaxKind.BangEqualsToken, "!="),
                (SyntaxKind.BangEqualsToken, "不是"),
                (SyntaxKind.OpenParenthesisToken, "("),
                (SyntaxKind.OpenParenthesisToken, "開"),
                (SyntaxKind.CloseParenthesisToken, ")"),
                (SyntaxKind.CloseParenthesisToken, "閉"),
                (SyntaxKind.FalseKeyword, "false"),
                (SyntaxKind.FalseKeyword, "假"),
                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.TrueKeyword, "真"),
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
                (SyntaxKind.SquareToken, "平方"),
                (SyntaxKind.SquareRootToken, "平方根"),
                (SyntaxKind.PowerToken, "次方"),
                (SyntaxKind.NthRootToken, "開方")
            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxKind.WhiteSpaceToken, " "),
                (SyntaxKind.WhiteSpaceToken, "  "),
                (SyntaxKind.WhiteSpaceToken, "\r"),
                (SyntaxKind.WhiteSpaceToken, "\n"),
                (SyntaxKind.WhiteSpaceToken, "\r\n")
            };
        }

        private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
        {
            var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
            var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

            if (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t1IsKeyword && t2IsKeyword)
                return true;

            if (t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken)
                return true;

            if (t1Kind == SyntaxKind.IdentifierToken && t2IsKeyword)
                return true;

            if (t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken)
                return true;

            if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;

            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsToken)
                return true;

            if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsEqualsToken)
                return true;

            return false;
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
            }
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text,
                                    SyntaxKind separatorKind, string separatorText,
                                    SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach (var s in GetSeparators())
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                    }
                }
            }
        }
    }
}