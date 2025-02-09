using DigitalRightsManagement.SourceGenerators;
using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.Analyzers.Generators;

public sealed class ValueObjectAttributeGeneratorTests
{
    [Fact]
    public void Generates_ValueObject_Attribute()
    {
        // Act
        var result = SourceGeneratorVerifier.Verify<ValueObjectAttributeGenerator>([]);

        // Assert
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("ValueObjectAttribute.g.cs");
        content.ShouldContain("public sealed class ValueObjectAttribute<T>");
        content.ShouldContain("namespace DigitalRightsManagement.Common.DDD");
        content.ShouldContain("[System.AttributeUsage(System.AttributeTargets.Struct)]");
        content.ShouldContain("public string? ErrorNamespace { get; init; }");
    }

    [Fact]
    public void Generated_Attribute_Can_Be_Used_On_Struct()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string>]
                public readonly partial struct TestStruct { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<ValueObjectAttributeGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Generated_Attribute_Cannot_Be_Used_On_Class()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string>]
                public class TestClass { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<ValueObjectAttributeGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldNotBeEmpty();
        result.Diagnostics.First().Id.ShouldBe("CS0592");
    }

    [Fact]
    public void Generated_Attribute_Requires_Non_Null_Type_Parameter()
    {
        // Arrange
        var sources = new[]
        {
            """
            #nullable enable
            
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [ValueObjectAttribute<string?>]
                public readonly partial struct TestStruct { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<ValueObjectAttributeGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldNotBeEmpty();
        result.Diagnostics.ShouldContain(d => d.Id == "CS8714");
    }
}
