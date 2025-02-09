using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;

public static class TestConfiguration
{
    public static IEnumerable<MetadataReference> GetCommonReferences()
    {
        var baseReferences = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Where(assembly => !assembly.FullName!.Contains("DigitalRightsManagement.Common", StringComparison.Ordinal))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        IEnumerable<MetadataReference> specificReferences =
        [
            MetadataReference.CreateFromFile(typeof(Entity<>).Assembly.Location)
        ];

        return baseReferences.Concat(specificReferences);
    }

    public static void ConfigureTest<TAnalyzer, TVerifier>(CSharpAnalyzerTest<TAnalyzer, TVerifier> test)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
    {
        test.TestState.AdditionalReferences.AddRange(GetCommonReferences());
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
        test.TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;
    }
}
