using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;

/// <summary>
/// Provides methods to verify diagnostic analyzers.
/// </summary>
internal static class AnalyzerVerifier
{
    public static DiagnosticResult ExpectedDiagnostic(string diagnosticId, int line, int column, string entityName)
        => new DiagnosticResult(diagnosticId, DiagnosticSeverity.Error)
            .WithSpan(line, column, line, column + entityName.Length)
            .WithArguments(entityName);

    /// <summary>
    /// Creates a <see cref="DiagnosticResult"/> with the specified diagnostic ID and error severity.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <returns>A <see cref="DiagnosticResult"/> with the specified diagnostic ID and error severity.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => new(diagnosticId, DiagnosticSeverity.Error);

    /// <summary>
    /// Creates a <see cref="DiagnosticResult"/> with the specified diagnostic ID and severity.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <param name="severity">The severity of the diagnostic.</param>
    /// <returns>A <see cref="DiagnosticResult"/> with the specified diagnostic ID and severity.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
        => new(diagnosticId, severity);

    /// <summary>
    /// Verifies the analyzer against the specified source code and expected diagnostics.
    /// </summary>
    /// <param name="source">The source code to analyze.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <typeparam name="TAnalyzer">The type of the diagnostic analyzer.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task VerifyAnalyzerAsync<TAnalyzer>(string source, params DiagnosticResult[] expected)
        where TAnalyzer : DiagnosticAnalyzer, new()
        => VerifyAnalyzerAsync<TAnalyzer>([source], expected);


    /// <summary>
    /// Verifies the analyzer against the specified source codes and expected diagnostics.
    /// </summary>
    /// <param name="sources">The source codes to analyze.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <typeparam name="TAnalyzer">The type of the diagnostic analyzer.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task VerifyAnalyzerAsync<TAnalyzer>(string[] sources, params DiagnosticResult[] expected)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>();
        TestConfiguration.ConfigureTest<TAnalyzer, DefaultVerifier>(test);

        // Convert string sources to the required type
        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);  // This overload handles the conversion
        }

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }
}
