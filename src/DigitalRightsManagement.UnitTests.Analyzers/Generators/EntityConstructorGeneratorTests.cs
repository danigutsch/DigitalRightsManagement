using DigitalRightsManagement.SourceGenerators;
using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification;
using Shouldly;

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
            using System;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity<Guid> { }
            }
            """
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
            using System;

            namespace TestNamespace;

            public sealed partial class TestEntity : Entity<Guid> { }
            """
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
            """
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
            using System;

            namespace Outer.Inner
            {
                public partial class TestEntity : Entity<Guid> { }
            }
            """
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
            using System;

            namespace TestNamespace;

            public partial class TestEntity : Entity<Guid> { }
            """
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

    [Fact]
    public void Generates_Constructor_For_Indirect_Entity_Generic_Inheritance()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;
            using System;

            namespace TestNamespace
            {
                public abstract partial class BaseEntity : Entity<Guid> { }
                public partial class TestEntity : BaseEntity { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        result.GeneratedFiles.Count.ShouldBe(2);
        result.GeneratedFiles.Select(gf => gf.Filename).ShouldContain("BaseEntity.g.cs");
        result.GeneratedFiles.Select(gf => gf.Filename).ShouldContain("TestEntity.g.cs");
         result.GeneratedFiles.Select(gf => gf.Content).ShouldContain(content => content.Contains("protected BaseEntity() { }"));
         result.GeneratedFiles.Select(gf => gf.Content).ShouldContain(content => content.Contains("protected TestEntity() { }"));
    }

    [Fact]
    public void Generates_Constructor_For_Multiple_Level_Entity_Generic_Inheritance()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;
            using System;

            namespace TestNamespace
            {
                public abstract partial class BaseEntity : Entity<Guid> { }
                public abstract partial class IntermediateEntity : BaseEntity { }
                public partial class TestEntity : IntermediateEntity { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        result.GeneratedFiles.Count.ShouldBe(3);
        result.GeneratedFiles.Select(gf => gf.Filename).ShouldContain("BaseEntity.g.cs");
        result.GeneratedFiles.Select(gf => gf.Filename).ShouldContain("IntermediateEntity.g.cs");
        result.GeneratedFiles.Select(gf => gf.Filename).ShouldContain("TestEntity.g.cs");
        result.GeneratedFiles.Select(gf => gf.Content).ShouldContain(content => content.Contains("protected BaseEntity() { }"));
        result.GeneratedFiles.Select(gf => gf.Content).ShouldContain(content => content.Contains("protected IntermediateEntity() { }"));
        result.GeneratedFiles.Select(gf => gf.Content).ShouldContain(content => content.Contains("protected TestEntity() { }"));
    }

    [Fact]
    public void No_Generation_For_Generic_Class_Not_Inheriting_Entity()
    {
        // Arrange
        var sources = new[]
        {
            """
            using System;

            namespace TestNamespace
            {
                public class BaseClass<T> { }
                public partial class TestClass : BaseClass<Guid> { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        result.GeneratedFiles.ShouldBeEmpty();
    }

    [Fact]
    public void Generates_Constructor_With_Generic_Constraints()
    {
        // Arrange
        var sources = new[]
        {
            """
            using DigitalRightsManagement.Common.DDD;
            using System;

            namespace TestNamespace
            {
                public partial class TestEntity<T> : Entity<Guid> where T : class, new() { }
            }
            """
        };

        // Act
        var result = SourceGeneratorVerifier.Verify<EntityConstructorGenerator>(sources);

        // Assert
        result.Diagnostics.ShouldBeEmpty();
        var (filename, content) = result.GeneratedFiles.ShouldHaveSingleItem();
        filename.ShouldBe("TestEntity.g.cs");
        content.ShouldContain("partial class TestEntity<T>");
        content.ShouldContain("protected TestEntity() { }");
    }
}
