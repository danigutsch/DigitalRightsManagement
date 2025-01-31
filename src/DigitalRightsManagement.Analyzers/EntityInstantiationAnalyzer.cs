using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace DigitalRightsManagement.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityInstantiationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM003";

    private static readonly LocalizableString Title =
        new LocalizableResourceString(nameof(Resources.EntityInstantiationAnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.EntityInstantiationAnalyzerMessageFormat),
            Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.EntityInstantiationAnalyzerDescription),
            Resources.ResourceManager, typeof(Resources));

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
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        if (objectCreation.ArgumentList?.Arguments.Count > 0)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(objectCreation.Type).Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "Entity" &&
                baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    objectCreation.GetLocation(),
                    typeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
                return;
            }
            baseType = baseType.BaseType;
        }
    }
}