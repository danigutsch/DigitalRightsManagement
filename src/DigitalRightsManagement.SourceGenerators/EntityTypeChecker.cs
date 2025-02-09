using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;

namespace DigitalRightsManagement.SourceGenerators;

public static class EntityTypeChecker
{
    public static bool IsEntity(INamedTypeSymbol typeSymbol, ConcurrentDictionary<INamedTypeSymbol, bool> cache) =>
        cache.GetOrAdd(typeSymbol, symbol =>
        {
            for (var baseType = symbol.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                // Check if it's Entity<T>
                if (baseType is { Name: "Entity", TypeArguments.Length: 1 } &&
                    baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
                {
                    return true;
                }
            }
            return false;
        });

    public static bool IsEntityBase(TypeSyntax typeSyntax, SemanticModel semanticModel)
    {
        // Get the symbol for the type
        var typeSymbol = semanticModel.GetTypeInfo(typeSyntax).Type as INamedTypeSymbol;
        if (typeSymbol is null)
        {
            return false;
        }

        // Check if it's Entity<T>
        return typeSymbol.Name == "Entity" &&
               typeSymbol.TypeArguments.Length == 1 &&
               typeSymbol.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD";
    }
}
