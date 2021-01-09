// using System.Collections.Generic;
// using CASC.CodeParser.Syntax;
// using NUnit.Framework;

// namespace CASC_Test.Tests
// {
//     public class ParserTest
//     {
//         [Theory]
//         [TestCaseSource(nameof(GetBinaryOperatorPairsData))]
//         [Ignore("")]
//         public void Parser_BinaryExpression_HonorsPrecedences(SyntaxKind op1, SyntaxKind op2)
//         {
//             var op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
//             var op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
//             var op1Text = SyntaxFacts.GetText(op1);
//             var op2Text = SyntaxFacts.GetText(op2);
//             var text = $"a {op1Text} b {op2Text} c";
//             var expression = SyntaxTree.Parse(text).Root;

//             if (op1Precedence >= op2Precedence)
//             {
//                 //     op2
//                 //    /   \
//                 //   op1   c
//                 //  /   \
//                 // a     b

//                 using (var e = new AssertingEnumerator(expression))
//                 {
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "a");
//                     e.AssertToken(op1, op1Text);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "b");
//                     e.AssertToken(op2, op2Text);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "c");
//                 }
//             }
//             else
//             {
//                 //   op1
//                 //  /   \
//                 // a    op2
//                 //     /   \
//                 //    b     c

//                 using (var e = new AssertingEnumerator(expression))
//                 {
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "a");
//                     e.AssertToken(op1, op1Text);
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "b");
//                     e.AssertToken(op2, op2Text);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "c");
//                 }
//             }
//         }

//         [Theory]
//         [TestCaseSource(nameof(GetUnaryOperatorPairsData))]
//         [Ignore("")]
//         public void Parser_UnaryExpression_HonorsPrecedences(SyntaxKind unaryKind, SyntaxKind binaryKind)
//         {
//             var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unaryKind);
//             var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binaryKind);
//             var unaryText = SyntaxFacts.GetText(unaryKind);
//             var binaryText = SyntaxFacts.GetText(binaryKind);
//             var text = $"{unaryText} a {binaryText} b";
//             var expression = SyntaxTree.Parse(text).Root;

//             if (unaryPrecedence >= binaryPrecedence)
//             {
//                 //   binary
//                 //   /    \
//                 // unary   b
//                 //   |
//                 //   a

//                 using (var e = new AssertingEnumerator(expression))
//                 {
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.UnaryExpression);
//                     e.AssertToken(unaryKind, unaryText);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "a");
//                     e.AssertToken(binaryKind, binaryText);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "b");
//                 }
//             }
//             else
//             {
//                 //  unary
//                 //    |
//                 //  binary
//                 //  /   \
//                 // a     b

//                 using (var e = new AssertingEnumerator(expression))
//                 {
//                     e.AssertNode(SyntaxKind.UnaryExpression);
//                     e.AssertToken(unaryKind, unaryText);
//                     e.AssertNode(SyntaxKind.BinaryExpression);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "a");
//                     e.AssertToken(binaryKind, binaryText);
//                     e.AssertNode(SyntaxKind.NameExpression);
//                     e.AssertToken(SyntaxKind.IdentifierToken, "b");
//                 }
//             }
//         }

//         public static IEnumerable<object[]> GetBinaryOperatorPairsData()
//         {
//             foreach (var op1 in SyntaxFacts.GetBinaryOperatorKinds())
//             {
//                 foreach (var op2 in SyntaxFacts.GetBinaryOperatorKinds())
//                 {
//                     yield return new object[] { op1, op2 };
//                 }
//             }
//         }

//         public static IEnumerable<object[]> GetUnaryOperatorPairsData()
//         {
//             foreach (var unary in SyntaxFacts.GetUnaryOperatorKinds())
//             {
//                 foreach (var binary in SyntaxFacts.GetBinaryOperatorKinds())
//                 {
//                     yield return new object[] { unary, binary };
//                 }
//             }
//         }
//     }
// }