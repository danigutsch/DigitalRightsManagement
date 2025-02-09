using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Concurrent;
using System.Text;
using static DigitalRightsManagement.SourceGenerators.EntityTypeChecker;

namespace DigitalRightsManagement.SourceGenerators;

[Generator]
public sealed class EntityConstructorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                transform: static (context, cancellationToken) =>
                {
                    var classDeclaration = (ClassDeclarationSyntax)context.Node;

                    if (classDeclaration.BaseList?.Types.Count == 0)
                    {
                        return null;
                    }

                    if (classDeclaration.BaseList?.Types.Any(baseType => IsEntityBase(baseType.Type, context.SemanticModel)) is true)
                    {
                        return classDeclaration;
                    }

                    var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);
                    if (symbol is null)
                    {
                        return null;
                    }

                    var cache = new ConcurrentDictionary<INamedTypeSymbol, bool>(SymbolEqualityComparer.Default);
                    return IsEntity(symbol, cache) ? classDeclaration : null;
                })
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, static (spc, source) => Execute(source!, spc));
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
        return namespaceDeclaration?.Name.ToString() ?? string.Empty;
    }

    private static string GenerateConstructorSource(string namespaceName, string className, string genericTypes, string constructorAccessModifier)
    {
        var namespaceDeclaration = string.IsNullOrEmpty(namespaceName)
            ? string.Empty
#pragma warning disable RS1035
            : $"namespace {namespaceName};{Environment.NewLine}{Environment.NewLine}";
#pragma warning restore RS1035

        return $$"""
                 {{SourceGeneratorUtilities.GetGeneratedHeader(className)}}
                 {{namespaceDeclaration}}
                 partial class {{className}}{{genericTypes}}
                 {
                     {{constructorAccessModifier}} {{className}}() { }
                 }
                 """;
    }
}
