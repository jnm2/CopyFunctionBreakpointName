using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CopyFunctionBreakpointName
{
    public static class FunctionBreakpointUtils
    {
        public static FunctionBreakpointNameFactory? GetFunctionBreakpointNameFactory(SyntaxNode syntaxRoot, TextSpan selectionRange)
        {
            if (selectionRange.IsEmpty)
            {
                return GetFunctionBreakpointNameFactory(syntaxRoot, new TextSpan(selectionRange.Start, 1))
                    ?? GetFunctionBreakpointNameFactory(syntaxRoot, new TextSpan(selectionRange.Start - 1, 1));
            }

            if (!(syntaxRoot is CSharpSyntaxNode csharpSyntaxRoot)) return null;

            switch (csharpSyntaxRoot.FindNode(selectionRange))
            {
                case MethodDeclarationSyntax method when method.Identifier.Span.Contains(selectionRange):
                    return new FunctionBreakpointNameFactory(method, method.Identifier, accessor: null);

                case PropertyDeclarationSyntax property when property.Identifier.Span.Contains(selectionRange):
                    return new FunctionBreakpointNameFactory(property, property.Identifier, accessor: null);

                case AccessorDeclarationSyntax accessor when accessor.Keyword.Span.Contains(selectionRange):
                    switch (accessor.Parent.Parent)
                    {
                        case PropertyDeclarationSyntax property:
                            return new FunctionBreakpointNameFactory(property, property.Identifier, accessor);

                        case EventDeclarationSyntax @event:
                            return new FunctionBreakpointNameFactory(@event, @event.Identifier, accessor);

                        default:
                            return null;
                    }

                // New Function Breakpoint window does not add breakpoints for event accessors by event name. 

                default:
                    return null;
            }
        }
    }
}
