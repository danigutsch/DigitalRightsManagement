using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analyzers;

public static class DiagnosticTestHelper
{
    public static DiagnosticResult ExpectedDiagnostic(string diagnosticId, int line, int column, string entityName)
        => new DiagnosticResult(diagnosticId, DiagnosticSeverity.Error)
            .WithSpan(line, column, line, column + entityName.Length)
            .WithArguments(entityName);
}
