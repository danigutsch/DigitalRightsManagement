using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analyzers;

/// <summary>
/// Provides helper methods for creating expected diagnostic results in tests.
/// </summary>
public static class DiagnosticTestHelper
{
    /// <summary>
    /// Creates an expected diagnostic result with the specified parameters.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <param name="line">The line number where the diagnostic is expected.</param>
    /// <param name="column">The column number where the diagnostic is expected.</param>
    /// <param name="entityName">The name of the entity that caused the diagnostic.</param>
    /// <returns>A <see cref="DiagnosticResult"/> representing the expected diagnostic.</returns>
    public static DiagnosticResult ExpectedDiagnostic(string diagnosticId, int line, int column, string entityName)
        => new DiagnosticResult(diagnosticId, DiagnosticSeverity.Error)
            .WithSpan(line, column, line, column + entityName.Length)
            .WithArguments(entityName);
}
