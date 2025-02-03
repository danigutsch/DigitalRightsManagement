﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DigitalRightsManagement.SourceGenerators;

/// <summary>
/// A source generator that generates constructors for classes inheriting from the Entity base class.
/// </summary>
[Generator]
public sealed class EntityConstructorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax c && c.BaseList?.Types.Count > 0,
                static (ctx, _) => GetEntityClass(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, static (spc, source) => Execute(source!, spc));
    }

    private static ClassDeclarationSyntax? GetEntityClass(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        return InheritsFromEntity(context.SemanticModel, classDeclaration) ? classDeclaration : null;
    }

    private static bool InheritsFromEntity(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
    {
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        while (symbol?.BaseType is { } baseType)
        {
            if (baseType.Name == "Entity" && baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
                return true;
            symbol = baseType;
        }
        return false;
    }

    private static void Execute(ClassDeclarationSyntax classDeclaration, SourceProductionContext context)
    {
        var namespaceName = GetNamespace(classDeclaration);
        var className = classDeclaration.Identifier.Text;
        var isSealed = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword));
        var genericTypes = classDeclaration.TypeParameterList?.Parameters.Count > 0
            ? $"<{string.Join(", ", classDeclaration.TypeParameterList!.Parameters)}>"
            : string.Empty;

        var constructorAccessModifier = isSealed ? "private" : "protected";

        var source = GenerateConstructorSource(namespaceName, className, genericTypes, constructorAccessModifier);
        context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GetNamespace(ClassDeclarationSyntax classDeclaration)
    {
        var namespaceDeclaration = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        return namespaceDeclaration?.Name.ToString() ?? "UnknownNamespace";
    }

    private static string GenerateConstructorSource(string namespaceName, string className, string genericTypes, string constructorAccessModifier)
    {
        return $$"""
            {{SourceGeneratorUtilities.GetGeneratedHeader(className)}}
            
            namespace {{namespaceName}};
            
            partial class {{className}}{{genericTypes}}
            {
                {{constructorAccessModifier}} {{className}}() { }
            }
            """;
    }
}
