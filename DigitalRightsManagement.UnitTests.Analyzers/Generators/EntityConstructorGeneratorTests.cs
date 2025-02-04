using DigitalRightsManagement.SourceGenerators;
using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;
using Shouldly;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework.AnalyzerConstants;

namespace DigitalRightsManagement.UnitTests.Analyzers.Generators;

public sealed class EntityConstructorGeneratorTests
{
    [Fact]
    public void Generates_Protected_Constructor_For_Non_Sealed_Entity()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity { }
            }
            """,
            EntityBaseClass
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("TestEntity.g.cs");
        content.ShouldContain("protected TestEntity() { }");
    }

    [Fact]
    public void Generates_Private_Constructor_For_Sealed_Entity()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace;

            public sealed partial class TestEntity : Entity { }
            """,
            EntityBaseClass
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("TestEntity.g.cs");
        content.ShouldContain("private TestEntity() { }");
    }

    [Fact]
    public void No_Generation_For_Non_Entity()
    {
        // Arrange
        var sources = new[]
        {
            """
            namespace TestNamespace;

            public partial class TestClass { }
            """,
            EntityBaseClass
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        result.GeneratedFiles.ShouldBeEmpty();
    }

    [Fact]
    public void Handles_Nested_Namespaces()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace Outer.Inner
            {
                public partial class TestEntity : Entity { }
            }
            """,
            EntityBaseClass
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("TestEntity.g.cs");
        content.ShouldContain("namespace Outer.Inner");
        content.ShouldContain("protected TestEntity() { }");
    }

    [Fact]
    public void Handles_File_Scoped_Namespaces()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace;

            public partial class TestEntity : Entity { }
            """,
            EntityBaseClass
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("TestEntity.g.cs");
        content.ShouldContain("namespace TestNamespace");
        content.ShouldContain("protected TestEntity() { }");
    }
}
