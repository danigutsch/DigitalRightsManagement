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
                if (baseType.Name == "Entity" &&
                    baseType.ContainingNamespace.ToDisplayString() == "DigitalRightsManagement.Common.DDD")
                {
                    return true;
                }
            }
            return false;
        });

    public static bool IsEntityBase(TypeSyntax type) =>
        type is SimpleNameSyntax { Identifier.Text: "Entity" } or
        GenericNameSyntax { Identifier.Text: "Entity" };
}
