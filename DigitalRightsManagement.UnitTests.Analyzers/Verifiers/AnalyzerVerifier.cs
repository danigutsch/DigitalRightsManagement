using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Verifiers;

/// <summary>
/// Provides methods to verify diagnostic analyzers.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the diagnostic analyzer.</typeparam>
internal static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        => VerifyAnalyzerAsync([source], expected);

    /// <summary>
    /// Verifies the analyzer against the specified source codes and expected diagnostics.
    /// </summary>
    /// <param name="sources">The source codes to analyze.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task VerifyAnalyzerAsync(string[] sources, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestState = { AdditionalReferences = { typeof(IAggregateRoot).Assembly } },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            CompilerDiagnostics = CompilerDiagnostics.Errors,
            TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck
        };

        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);
        }

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
}
