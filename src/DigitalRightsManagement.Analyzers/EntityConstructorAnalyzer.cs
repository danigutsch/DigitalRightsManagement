using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using static DigitalRightsManagement.Analyzers.AnalyzerUtilities;

namespace DigitalRightsManagement.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityConstructorAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DRM002";

    private static readonly LocalizableString Title = CreateLocalizableString(nameof(Resources.EntityConstructorAnalyzerTitle));
    private static readonly LocalizableString MessageFormat = CreateLocalizableString(nameof(Resources.EntityConstructorAnalyzerMessageFormat));
    private static readonly LocalizableString Description = CreateLocalizableString(nameof(Resources.EntityConstructorAnalyzerDescription));

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

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var entityCache = new ConcurrentDictionary<INamedTypeSymbol, bool>(SymbolEqualityComparer.Default);

            compilationContext.RegisterSyntaxNodeAction(
                analysisContext => AnalyzeConstructor(analysisContext, entityCache),
                SyntaxKind.ConstructorDeclaration);
        });
    }

    private static void AnalyzeConstructor(SyntaxNodeAnalysisContext context, ConcurrentDictionary<INamedTypeSymbol, bool> entityCache)
    {
        if (context.Node is not ConstructorDeclarationSyntax constructor ||
            constructor.ParameterList.Parameters.Count > 0 ||
            constructor.Parent is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (typeSymbol is null)
        {
            return;
        }

        if (!IsEntity(typeSymbol, entityCache))
        {
            return;
        }

        ReportDiagnostic(context, Rule, constructor.Identifier.GetLocation(), classDeclaration.Identifier.Text);
    }

    private static bool IsEntity(INamedTypeSymbol typeSymbol, ConcurrentDictionary<INamedTypeSymbol, bool> cache)
    {
        return cache.GetOrAdd(typeSymbol, symbol =>
        {
            for (var baseType = symbol.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                if (baseType.Name == "Entity" &&
                    baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
                {
                    return true;
                }
            }
            return false;
        });
    }
}
