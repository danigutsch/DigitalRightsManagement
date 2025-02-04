using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using GeneratorResult = (System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics, System.Collections.Generic.List<(string Filename, string Content)> GeneratedFiles);

namespace DigitalRightsManagement.UnitTests.Analyzers.Verifiers;

public static class SourceGeneratorVerifier
{
    public static GeneratorResult Verify<TGenerator>(string[] sources) where TGenerator : IIncrementalGenerator, new()
    {
        var generator = new TGenerator();
        var references = GetReferences();
        var compilation = CreateCompilation(sources, references);

#pragma warning disable S3220
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
#pragma warning restore S3220
        driver = driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        var generatedFiles = result.Results
            .SelectMany(r => r.GeneratedSources)
            .Select(f => (f.HintName, f.SourceText.ToString()))
            .ToList();

        // For testing generated code usage
        var newCompilation = compilation.AddSyntaxTrees(
            result.Results
                .SelectMany(r => r.GeneratedSources)
                .Select(s => s.SyntaxTree));

        return new GeneratorResult(newCompilation.GetDiagnostics(), generatedFiles);
    }

    private static IEnumerable<MetadataReference> GetReferences() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Where(assembly => !assembly.FullName!.Contains("DigitalRightsManagement.Common", StringComparison.Ordinal))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

    private static CSharpCompilation CreateCompilation(
        string[] sources,
        IEnumerable<MetadataReference> references) =>
        CSharpCompilation.Create(
            "TestAssembly",
            sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}
