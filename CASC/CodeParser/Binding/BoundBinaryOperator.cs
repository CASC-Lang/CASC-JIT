using CASC.CodeParser.Syntax;
using CASC.CodeParser.Types;
using System;
using System.Collections.Generic;

namespace CASC.CodeParser.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
         : this(syntaxKind, kind, new HashSet<Type>() { type }, new HashSet<Type>() { type }, type)
        {
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType)
         : this(syntaxKind, kind, new HashSet<Type>() { operandType }, new HashSet<Type>() { operandType }, resultType)
        {
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, HashSet<Type> operandTypes, Type resultType)
         : this(syntaxKind, kind, operandTypes, operandTypes, resultType)
        {
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftTypes, Type rightTyes, Type resultType)
         : this(syntaxKind, kind, new HashSet<Type>() { leftTypes }, new HashSet<Type>() { rightTyes }, resultType)
        {
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, HashSet<Type> leftTypes, HashSet<Type> rightTyes, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftTypes = leftTypes;
            RightTyes = rightTyes;
            ResultType = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public HashSet<Type> LeftTypes { get; }
        public HashSet<Type> RightTyes { get; }
        public Type ResultType { get; }



        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeDictionary.Instance[TypeKind.Number], typeof(decimal)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeDictionary.Instance[TypeKind.Number], typeof(decimal)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeDictionary.Instance[TypeKind.Number], typeof(decimal)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeDictionary.Instance[TypeKind.Number], typeof(decimal)),
            new BoundBinaryOperator(SyntaxKind.PointToken, BoundBinaryOperatorKind.Point, TypeDictionary.Instance[TypeKind.Number], new HashSet<Type>() {typeof(int)}, typeof(decimal)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeDictionary.Instance[TypeKind.Number], typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeDictionary.Instance[TypeKind.Number], typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAND, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOR, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
        {
            foreach (var op in _operators)
                if (op.SyntaxKind == syntaxKind && op.LeftTypes.Contains(leftType) && op.RightTyes.Contains(rightType))
                    return op;

            return null;
        }
    }
}