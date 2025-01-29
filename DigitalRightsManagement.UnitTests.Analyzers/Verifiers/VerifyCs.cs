using DigitalRightsManagement.Analyzers;
using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Verifiers;

public static class VerifyCs
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => new(diagnosticId, DiagnosticSeverity.Error);

    public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
        => new(diagnosticId, severity);

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        => VerifyAnalyzerAsync([source], expected);

    public static async Task VerifyAnalyzerAsync(string[] sources, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<EntityPartialAnalyzer, DefaultVerifier>
        {
            TestState = { AdditionalReferences = { typeof(Entity).Assembly } },
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
