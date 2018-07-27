using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CopyFunctionBreakpointName
{
    public static class FunctionBreakpointUtils
    {
        public static async Task<FunctionBreakpointNameFactory?> GetFunctionBreakpointNameFactoryAsync(
            SyntaxNode syntaxRoot,
            TextSpan selectionRange,
            Func<CancellationToken, Task<SemanticModel>> semanticModelAccessor,
            CancellationToken cancellationToken)
        {
            if (syntaxRoot == null) throw new ArgumentNullException(nameof(syntaxRoot));
            if (semanticModelAccessor == null) throw new ArgumentNullException(nameof(semanticModelAccessor));

            if (selectionRange.IsEmpty)
            {
                return await GetFunctionBreakpointNameFactoryAsync(syntaxRoot, new TextSpan(selectionRange.Start, 1), semanticModelAccessor, cancellationToken).ConfigureAwait(false)
                    ?? await GetFunctionBreakpointNameFactoryAsync(syntaxRoot, new TextSpan(selectionRange.Start - 1, 1), semanticModelAccessor, cancellationToken).ConfigureAwait(false);
            }

            if (!(syntaxRoot is CSharpSyntaxNode csharpSyntaxRoot)) return null;

            switch (csharpSyntaxRoot.FindNode(selectionRange))
            {
                case MethodDeclarationSyntax method when method.Identifier.Span.Contains(selectionRange):
                    return new FunctionBreakpointNameFactory(method, method.Identifier, accessor: null);

                case ConstructorDeclarationSyntax constructor when constructor.Identifier.Span.Contains(selectionRange):
                {
                    var isStatic = constructor.Modifiers.Any(SyntaxKind.StaticKeyword);

                    if (isStatic && constructor.Parent is StructDeclarationSyntax) return null;

                    return new FunctionBreakpointNameFactory(
                        constructor,
                        isStatic ? SyntaxFactory.Identifier("cctor") : constructor.Identifier,
                        accessor: null);
                }

                case DestructorDeclarationSyntax destructor when destructor.Identifier.Span.Contains(selectionRange):
                    return new FunctionBreakpointNameFactory(destructor, SyntaxFactory.Identifier("Finalize"), accessor: null);

                case PropertyDeclarationSyntax property when property.Identifier.Span.Contains(selectionRange):
                    return new FunctionBreakpointNameFactory(property, property.Identifier, accessor: null);

                case IndexerDeclarationSyntax indexer when indexer.ThisKeyword.Span.Contains(selectionRange):
                {
                    var semanticModel = await semanticModelAccessor.Invoke(cancellationToken).ConfigureAwait(false);
                    var metadataName = GetMetadataName(indexer, semanticModel);
                    return new FunctionBreakpointNameFactory(indexer, metadataName, accessor: null);
                }

                case AccessorDeclarationSyntax accessor when accessor.Keyword.Span.Contains(selectionRange):
                    switch (accessor.Parent.Parent)
                    {
                        case PropertyDeclarationSyntax property:
                            return new FunctionBreakpointNameFactory(property, property.Identifier, accessor);

                        case IndexerDeclarationSyntax indexer:
                        {
                            var semanticModel = await semanticModelAccessor.Invoke(cancellationToken).ConfigureAwait(false);
                            var metadataName = GetMetadataName(indexer, semanticModel);
                            return new FunctionBreakpointNameFactory(indexer, metadataName, accessor);
                        }

                        case EventDeclarationSyntax @event:
                            return new FunctionBreakpointNameFactory(@event, @event.Identifier, accessor);

                        default:
                            return null;
                    }

                case OperatorDeclarationSyntax op when IsFunctionNameSpan(op, selectionRange):
                {
                    var semanticModel = await semanticModelAccessor.Invoke(cancellationToken).ConfigureAwait(false);
                    var metadataName = GetMetadataName(op, semanticModel);
                    return new FunctionBreakpointNameFactory(op, metadataName, accessor: null);
                }

                case ConversionOperatorDeclarationSyntax op when op.OperatorKeyword.Span.Contains(selectionRange):
                {
                    return new FunctionBreakpointNameFactory(
                        op,
                        SyntaxFactory.Identifier(op.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ExplicitKeyword)
                            ? WellKnownMemberNames.ExplicitConversionName
                            : WellKnownMemberNames.ImplicitConversionName),
                        accessor: null);
                }

                // New Function Breakpoint window does not add breakpoints for event accessors by event name.

                default:
                    return null;
            }
        }

        private static SyntaxToken GetMetadataName(MemberDeclarationSyntax syntax, SemanticModel semanticModel)
        {
            var metadataName = semanticModel.GetDeclaredSymbol(syntax).MetadataName;

            return SyntaxFactory.Identifier(metadataName);
        }

        private static bool IsFunctionNameSpan(OperatorDeclarationSyntax syntax, TextSpan span)
        {
            if (syntax.OperatorKeyword.Span.Contains(span) || syntax.OperatorToken.Span.Contains(span))
            {
                return true;
            }

            if (span.Start < syntax.OperatorKeyword.Span.Start || syntax.OperatorToken.Span.End < span.End)
            {
                return false;
            }

            return span.Start < syntax.OperatorKeyword.Span.End || syntax.OperatorToken.Span.Start < span.End;
        }
    }
}
