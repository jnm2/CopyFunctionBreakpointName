using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CopyFunctionBreakpointName
{
    public readonly struct FunctionBreakpointNameFactory
    {
        private readonly CSharpSyntaxNode member;
        private readonly SyntaxToken memberIdentifier;
        private readonly AccessorDeclarationSyntax accessor;

        public FunctionBreakpointNameFactory(CSharpSyntaxNode member, SyntaxToken memberIdentifier, AccessorDeclarationSyntax accessor)
        {
            this.member = member;
            this.memberIdentifier = memberIdentifier;
            this.accessor = accessor;
        }

        public override string ToString()
        {
            var reverseSegments = new List<SyntaxToken>();

            var current = member.Parent;

            while (current is TypeDeclarationSyntax type)
            {
                reverseSegments.Add(type.Identifier);
                current = current.Parent;
            }

            while (current is NamespaceDeclarationSyntax @namespace)
            {
                var currentName = @namespace.Name;

                while (currentName is QualifiedNameSyntax qualified)
                {
                    reverseSegments.Add(qualified.Right.Identifier);
                    currentName = qualified.Left;
                }

                reverseSegments.Add(((IdentifierNameSyntax)currentName).Identifier);

                current = current.Parent;
            }

            var sb = new StringBuilder();

            for (var i = reverseSegments.Count - 1; i >= 0; i--)
            {
                sb.Append(reverseSegments[i].ValueText).Append('.');
            }

            switch (accessor?.Parent.Parent)
            {
                case PropertyDeclarationSyntax _:
                case IndexerDeclarationSyntax _:
                    sb.Append(memberIdentifier.ValueText).Append('.').Append(accessor.Keyword.ValueText);
                    break;
                case EventDeclarationSyntax _:
                    // New Function Breakpoint window does not recognize `EventName.add` syntax.
                    sb.Append(accessor.Keyword.ValueText).Append('_').Append(memberIdentifier.ValueText);
                    break;
                default:
                    sb.Append(memberIdentifier.ValueText);
                    break;
            }

            return sb.ToString();
        }
    }
}
