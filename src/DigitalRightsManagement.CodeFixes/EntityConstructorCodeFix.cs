using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace DigitalRightsManagement.CodeFixes;

/// <summary>
/// Provides code fixes for diagnostics reported by <see cref="EntityConstructorAnalyzer"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public sealed class EntityConstructorCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [EntityConstructorAnalyzer.DiagnosticId];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Get the syntax root only once
        var root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        var constructor = root?.FindToken(diagnosticSpan.Start).Parent?
            .AncestorsAndSelf()
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        if (root is null)
        {
            return;
        }

        if (constructor?.Parent is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        var title = classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword)
            ? CodeFixResources.CodeFixTitleSealed
            : CodeFixResources.CodeFixTitleNonSealed;

        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                _ => RemoveConstructorAsync(context.Document, root, constructor),
                equivalenceKey: title),
            diagnostic);
    }

    private static Task<Document> RemoveConstructorAsync(
        Document document,
        SyntaxNode root,
        ConstructorDeclarationSyntax constructor)
    {
        var newRoot = root.RemoveNode(constructor, SyntaxRemoveOptions.KeepNoTrivia);
        return Task.FromResult(newRoot is null ? document : document.WithSyntaxRoot(newRoot));
    }
}
