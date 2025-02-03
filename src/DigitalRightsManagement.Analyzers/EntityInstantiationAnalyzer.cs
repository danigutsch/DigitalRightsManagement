using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static DigitalRightsManagement.Analyzers.AnalyzerUtilities;

namespace DigitalRightsManagement.Analyzers;

/// <summary>
/// Analyzes entity instantiations to ensure they do not use parameterless constructors.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityInstantiationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM003";

    private static readonly LocalizableString Title = CreateLocalizableString(nameof(Resources.EntityInstantiationAnalyzerTitle));
    private static readonly LocalizableString MessageFormat = CreateLocalizableString(nameof(Resources.EntityInstantiationAnalyzerMessageFormat));
    private static readonly LocalizableString Description = CreateLocalizableString(nameof(Resources.EntityInstantiationAnalyzerDescription));

    private const string Category = "Usage";

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
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ObjectCreationExpressionSyntax objectCreation)
        {
            return;
        }

        if (objectCreation.ArgumentList?.Arguments.Count > 0)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(objectCreation.Type).Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        if (!InheritsFromEntity(typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreation.GetLocation(), typeSymbol.Name));
    }
}
