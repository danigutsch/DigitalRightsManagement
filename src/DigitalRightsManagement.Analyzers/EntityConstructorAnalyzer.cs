using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace DigitalRightsManagement.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EntityConstructorAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM002";

    private static readonly LocalizableString Title =
        new LocalizableResourceString(
            nameof(Resources.EntityConstructorAnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(
            nameof(Resources.EntityConstructorAnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(
            nameof(Resources.EntityConstructorAnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

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

        context.RegisterSyntaxNodeAction(
            AnalyzeNode,
            SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Check if this is an Entity-derived class
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (symbol?.BaseType is null)
        {
            return;
        }

        var baseType = symbol.BaseType;
        var isEntity = false;

        while (baseType is not null)
        {
            if (baseType.Name == "Entity" &&
                baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
            {
                isEntity = true;
                break;
            }
            baseType = baseType.BaseType;
        }

        if (!isEntity)
        {
            return;
        }

        // Look for parameterless constructors
        foreach (var constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
        {
            if (constructor.ParameterList.Parameters.Count == 0)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    constructor.Identifier.GetLocation(),
                    classDeclaration.Identifier.Text);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
