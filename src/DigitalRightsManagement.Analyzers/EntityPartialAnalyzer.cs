using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static DigitalRightsManagement.Analyzers.AnalyzerUtilities;

namespace DigitalRightsManagement.Analyzers;

/// <summary>
/// Analyzes entity classes to ensure they are declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityPartialAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM001";

    private static readonly LocalizableString Title = CreateLocalizableString(nameof(Resources.EntityPartialAnalyzerTitle));
    private static readonly LocalizableString MessageFormat = CreateLocalizableString(nameof(Resources.EntityPartialAnalyzerMessageFormat));
    private static readonly LocalizableString Description = CreateLocalizableString(nameof(Resources.EntityPartialAnalyzerDescription));

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        if (classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        if (!InheritsFromEntity(context.SemanticModel, classDeclaration))
        {
            return;
        }

        ReportDiagnostic(context, Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
    }
}
