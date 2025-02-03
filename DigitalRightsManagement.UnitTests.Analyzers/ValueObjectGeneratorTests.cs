using DigitalRightsManagement.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System.Collections.Immutable;

namespace DigitalRightsManagement.UnitTests.Analyzers;

public class ValueObjectGeneratorTests
{
    [Fact]
    public void Generates_ValueObject_Attribute()
    {
        // Arrange
        var compilation = CreateCompilation();
        var generator = new ValueObjectAttributeGenerator();

        // Act
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        // Assert
        var generatedFiles = result.Results.SelectMany(r => r.GeneratedSources)
            .Select(f => (f.HintName, f.SourceText.ToString()))
            .ToList();

        generatedFiles.ShouldHaveSingleItem();
        var (filename, content) = generatedFiles[0];

        filename.ShouldBe("ValueObjectAttribute.g.cs");
        content.ShouldContain("public sealed class ValueObjectAttribute<T>");
        content.ShouldContain("namespace DigitalRightsManagement.Common.DDD");
        content.ShouldContain("[AttributeUsage(AttributeTargets.Struct)]");
        content.ShouldContain("public string? ErrorNamespace { get; init; }");
    }

    [Fact]
    public void Generated_Attribute_Can_Be_Used_On_Struct()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string>]
                public readonly partial struct TestStruct { }
            }
            """;

        // Act
        var (diagnostics, _) = CompileWithGenerator(source);

        // Assert
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generated_Attribute_Cannot_Be_Used_On_Class()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string>]
                public class TestClass { }
            }
            """;

        // Act
        var (diagnostics, _) = CompileWithGenerator(source);

        // Assert
        diagnostics.ShouldNotBeEmpty();
        diagnostics.First().Id.ShouldBe("CS0592"); // Attribute not valid on this declaration type
    }

    [Fact]
    public void Generated_Attribute_Requires_Non_Null_Type_Parameter()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string?>]
                public readonly partial struct TestStruct { }
            }
            """;

        // Act
        var (diagnostics, _) = CompileWithGenerator(source);

        // Assert
        diagnostics.ShouldNotBeEmpty();
        // CS8983: A type parameter cannot be made nullable in this context
        diagnostics.ShouldContain(d => d.Id == "CS8983");
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, List<(string filename, string content)> Output) CompileWithGenerator(string source)
    {
        var compilation = CreateCompilation(source);
        var generator = new ValueObjectAttributeGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        var runResult = driver.GetRunResult();
        var generatedFiles = runResult.Results.SelectMany(r => r.GeneratedSources)
            .Select(f => (f.HintName, f.SourceText.ToString()))
            .ToList();

        // Extract generated syntax trees and combine with original compilation
        var generatedSyntaxTrees = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .Select(s => s.SyntaxTree)
            .ToArray();

        var newCompilation = compilation.AddSyntaxTrees(generatedSyntaxTrees);
        var diagnostics = newCompilation.GetDiagnostics();

        return (diagnostics, generatedFiles);
    }

    private static CSharpCompilation CreateCompilation(string? source = null)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            // Remove DigitalRightsManagement.Common assembly to avoid reference conflicts
            .Where(assembly => !assembly.FullName!.Contains("DigitalRightsManagement.Common", StringComparison.Ordinal))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        var syntaxTrees = source is not null
            ? new[] { CSharpSyntaxTree.ParseText(source) }
            : [];

        return CSharpCompilation.Create(
            "TestAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
