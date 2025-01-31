using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityConstructorAnalyzer>;

namespace DigitalRightsManagement.UnitTests.Analyzers;

public class EntityConstructorAnalyzerTests
{
    [Fact]
    public async Task Reports_Manual_Parameterless_Constructor()
    {
        // Arrange
        const string source = """
            using System;
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    protected TestEntity() { }
                }
            }
            """;

        var expected = new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(8, 19, 8, 29)
            .WithArguments("TestEntity");

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Parameterized_Constructor()
    {
        // Arrange
        const string source = """
            using System;
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    public TestEntity(Guid id) : base(id) { }
                }
            }
            """;

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }

    [Fact]
    public async Task No_Diagnostic_For_Non_Entity()
    {
        // Arrange
        const string source = """
            namespace TestNamespace
            {
                public class TestClass
                {
                    public TestClass() { }
                }
            }
            """;

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task No_Diagnostic_For_Generated_Code()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                [System.CodeDom.Compiler.GeneratedCode("SomeGenerator", "1.0")]
                public partial class TestEntity : Entity
                {
                    protected TestEntity() { }
                }
            }
            """;

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }

    [Fact]
    public async Task No_Diagnostic_For_Entity_Without_Constructor()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity { }
            }
            """;

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }
}
