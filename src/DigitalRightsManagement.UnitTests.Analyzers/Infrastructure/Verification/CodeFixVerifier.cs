using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;

internal static class CodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <summary>
    /// Creates a <see cref="DiagnosticResult"/> with the specified diagnostic ID and error severity.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <returns>A <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => new(diagnosticId, DiagnosticSeverity.Error);

    /// <summary>
    /// Creates a <see cref="DiagnosticResult"/> with the specified diagnostic ID and severity.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <param name="severity">The severity of the diagnostic.</param>
    /// <returns>A <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
        => new(diagnosticId, severity);

    /// <summary>
    /// Verifies the analyzer with the provided source code and expected diagnostics.
    /// </summary>
    /// <param name="source">The source code to analyze.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        => VerifyAnalyzerAsync([source], expected);

    /// <summary>
    /// Verifies the analyzer with the provided source code and expected diagnostics.
    /// </summary>
    /// <param name="sources">The source code to analyze.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task VerifyAnalyzerAsync(string[] sources, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestState =
            {
                AdditionalReferences = { typeof(IAggregateRoot).Assembly }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90
        };

        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);
        }

        test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the code fix with the provided source code and fixed source code.
    /// </summary>
    /// <param name="source">The source code to fix.</param>
    /// <param name="fixedSource">The fixed source code.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task VerifyCodeFixAsync(string source, string fixedSource)
        => VerifyCodeFixAsync([source], fixedSource);

    /// <summary>
    /// Verifies the code fix with the provided source code and fixed source code.
    /// </summary>
    /// <param name="sources">The source code to fix.</param>
    /// <param name="fixedSource">The fixed source code.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task VerifyCodeFixAsync(string[] sources, string fixedSource)
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            TestState =
            {
                AdditionalReferences = { typeof(IAggregateRoot).Assembly }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            FixedCode = fixedSource
        };

        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);
        }

        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
