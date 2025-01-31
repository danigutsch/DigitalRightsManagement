using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityPartialAnalyzer>;

namespace DigitalRightsManagement.UnitTests.Analyzers;

public class EntityPartialAnalyzerTests
{
    [Fact]
    public async Task Reports_Non_Partial_Entity()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace
                              {
                                  public class TestEntity : Entity { }
                              }
                              """;

        var expected = new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(5, 18, 5, 28)
            .WithArguments("TestEntity");

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Partial_Entity()
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

    [Fact]
    public async Task No_Diagnostic_For_Non_Entity()
    {
        // Arrange
        const string source = """
                              namespace TestNamespace
                              {
                                  public class TestClass { }
                              }
                              """;

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task Reports_Multiple_Non_Partial_Entities()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace
                              {
                                  public class FirstEntity : Entity { }
                                  public class SecondEntity : Entity { }
                              }
                              """;

        var expected = new[]
        {
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(5, 18, 5, 29)
                .WithArguments("FirstEntity"),
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(6, 18, 6, 30)
                .WithArguments("SecondEntity")
        };

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Indirect_Entity()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace
                              {
                                  public abstract class BaseEntity : Entity { }
                                  public class DerivedClass : BaseEntity { }
                              }
                              """;

        var expected = new[]
        {
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(5, 27, 5, 37)
                .WithArguments("BaseEntity"),
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(6, 18, 6, 30)
                .WithArguments("DerivedClass")
        };

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }
}
