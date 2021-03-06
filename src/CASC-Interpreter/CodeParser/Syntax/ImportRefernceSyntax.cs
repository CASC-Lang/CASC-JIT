using Mono.Cecil;
namespace CASC.CodeParser.Syntax
{
    public sealed class ImportStatementSyntax : MemberSyntax
    {
        public ImportStatementSyntax(SyntaxTree syntaxTree,
                                     SyntaxToken importKeyword,
                                     SyntaxToken referencePathToken) 
            : base(syntaxTree)
        {
            ReferencePathToken = referencePathToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ImportReference;
        public SyntaxToken ReferencePathToken { get; }
    }
}