using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using GeneratorResult = (System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics, System.Collections.Generic.List<(string Filename, string Content)> GeneratedFiles);

namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;

public static class SourceGeneratorVerifier
{
    /// <summary>
    /// Verifies the source generator by running it on the provided sources.
    /// </summary>
    /// <typeparam name="TGenerator">The type of the source generator to verify.</typeparam>
    /// <param name="sources">The source code to run the generator on.</param>
    /// <returns>A tuple containing the diagnostics and the generated files.</returns>
    public static GeneratorResult Verify<TGenerator>(string[] sources) where TGenerator : IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(sources);
        var generator = new TGenerator();

#pragma warning disable S3220
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
#pragma warning restore S3220
        driver = driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        var generatedFiles = result.Results
            .SelectMany(r => r.GeneratedSources)
            .Select(f => (f.HintName, f.SourceText.ToString()))
            .ToList();

        var newCompilation = compilation.AddSyntaxTrees(
            result.Results
                .SelectMany(r => r.GeneratedSources)
                .Select(s => s.SyntaxTree));

        return new GeneratorResult(newCompilation.GetDiagnostics(), generatedFiles);
    }

    /// <summary>
    /// Creates a C# compilation for the provided sources and references.
    /// </summary>
    /// <param name="sources">The source code to compile.</param>
    /// <returns>A C# compilation.</returns>
    private static CSharpCompilation CreateCompilation(string[] sources)
    {
        var references = TestConfiguration.GetCommonReferences();

        return CSharpCompilation.Create(
            "TestAssembly",
            sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
