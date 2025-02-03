using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DigitalRightsManagement.SourceGenerators;

/// <summary>
/// A source generator that generates the ValueObject attribute.
/// </summary>
[Generator]
public sealed class ValueObjectAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateValueObjectAttribute);
    }

    private static void GenerateValueObjectAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        var source = GenerateAttributeSource();
        context.AddSource("ValueObjectAttribute.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GenerateAttributeSource() =>
        $$"""
          {{SourceGeneratorUtilities.GetGeneratedHeader("ValueObjectAttribute<T>")}}

          namespace DigitalRightsManagement.Common.DDD;

          [System.AttributeUsage(System.AttributeTargets.Struct)]
          public sealed class ValueObjectAttribute<T> : System.Attribute where T : notnull
          {
              public string? ErrorNamespace { get; init; }
          }
          """;
}
