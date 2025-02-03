using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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
        => semanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol typeSymbol &&
           InheritsFromEntity(typeSymbol);

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
    public static LocalizableString CreateLocalizableString(string resourceName)
        => new LocalizableResourceString(resourceName, Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// Reports a diagnostic in a consistent manner.
    /// </summary>
    public static void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, Location location, params object[] args)
        => context.ReportDiagnostic(Diagnostic.Create(rule, location, args));
}
