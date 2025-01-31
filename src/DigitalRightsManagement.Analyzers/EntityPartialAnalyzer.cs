using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace DigitalRightsManagement.Analyzers;

/// <summary>
/// Analyzes entity classes to ensure they are declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityPartialAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM001";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EntityPartialAnalyzerTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EntityPartialAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EntityPartialAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze |
            GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Analyzes the syntax node to find entity classes that are not declared as partial.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        if (classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (symbol?.BaseType is null)
        {
            return;
        }

        var baseType = symbol.BaseType;
        while (baseType is not null)
        {
            if (baseType.Name == "Entity")
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    classDeclaration.Identifier.GetLocation(),
                    classDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                return;
            }
            baseType = baseType.BaseType;
        }
    }
}
