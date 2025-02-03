﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace DigitalRightsManagement.Analyzers;

/// <summary>
/// Analyzes entity classes to ensure they do not have parameterless constructors.
/// </summary>
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
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        if (!InheritsFromEntity(context.SemanticModel, classDeclaration))
        {
            return;
        }

        foreach (var constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
        {
            if (constructor.ParameterList.Parameters.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, constructor.Identifier.GetLocation(), classDeclaration.Identifier.Text));
            }
        }
    }

    private static bool InheritsFromEntity(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
    {
        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not { } typeSymbol)
        {
            return false;
        }

        for (var baseType = typeSymbol.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            if (baseType.Name == "Entity" &&
                baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
            {
                return true;
            }
        }

        return false;
    }

    private static LocalizableResourceString CreateLocalizableString(string resourceName)
        => new(resourceName, Resources.ResourceManager, typeof(Resources));
}
