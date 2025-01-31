using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace DigitalRightsManagement.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public sealed class EntityConstructorCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [EntityConstructorAnalyzer.DiagnosticId];

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var constructor = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

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
                ct => RemoveConstructorAsync(context.Document, constructor, ct),
                equivalenceKey: title),
            diagnostic);
    }

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private static async Task<Document> RemoveConstructorAsync(Document document, ConstructorDeclarationSyntax constructor, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var newRoot = root.RemoveNode(constructor, SyntaxRemoveOptions.KeepNoTrivia);
        return newRoot is null
            ? document
            : document.WithSyntaxRoot(newRoot);
    }
}
