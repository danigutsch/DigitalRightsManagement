using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace DigitalRightsManagement.Analyzers;

/// <summary>
/// Provides utility methods for analyzers.
/// </summary>
public static class AnalyzerUtilities
{
    /// <summary>
    /// Determines if a given class inherits from the Entity base class.
    /// </summary>
    public static bool InheritsFromEntity(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
    {
        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol typeSymbol)
        {
            return false;
        }

        return InheritsFromEntity(typeSymbol);
    }

    /// <summary>
    /// Determines if a given type symbol inherits from the Entity base class.
    /// </summary>
    public static bool InheritsFromEntity(INamedTypeSymbol typeSymbol)
    {
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

    /// <summary>
    /// Creates a localized string using the resource manager.
    /// </summary>
    public static LocalizableResourceString CreateLocalizableString(string resourceName) => new LocalizableResourceString(resourceName, Resources.ResourceManager, typeof(Resources));
}
