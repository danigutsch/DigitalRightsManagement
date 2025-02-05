using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework.AnalyzerConstants;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification.AnalyzerVerifier;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analysis;

public sealed class EntityPartialAnalyzerTests
{
    [Fact]
    public async Task Reports_Non_Partial_Entity()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;
                              
                              namespace TestNamespace
                              {
                                  public class TestEntity : Entity<Guid> { }
                              }
                              """;

        var expected = new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(6, 18, 6, 28)
            .WithArguments("TestEntity");

        // Act & Assert
        await VerifyAnalyzerAsync<EntityPartialAnalyzer>([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Partial_Entity()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public partial class TestEntity : Entity<Guid> { }
                              }
                              """;

        // Act & Assert
        await VerifyAnalyzerAsync<EntityPartialAnalyzer>([source, EntityBaseClass]);
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
        await VerifyAnalyzerAsync<EntityPartialAnalyzer>(source);
    }

    [Fact]
    public async Task Reports_Multiple_Non_Partial_Entities()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public class FirstEntity : Entity<Guid> { }
                                  public class SecondEntity : Entity<Guid> { }
                              }
                              """;

        var expected = new[]
        {
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(6, 18, 6, 29)
                .WithArguments("FirstEntity"),
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(7, 18, 7, 30)
                .WithArguments("SecondEntity")
        };

        // Act & Assert
        await VerifyAnalyzerAsync<EntityPartialAnalyzer>([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Indirect_Entity()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public abstract class BaseEntity : Entity<Guid> { }
                                  public class DerivedClass : BaseEntity { }
                              }
                              """;

        var expected = new[]
        {
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(6, 27, 6, 37)
                .WithArguments("BaseEntity"),
            new DiagnosticResult(EntityPartialAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(7, 18, 7, 30)
                .WithArguments("DerivedClass")
        };

        // Act & Assert
        await VerifyAnalyzerAsync<EntityPartialAnalyzer>([source, EntityBaseClass], expected);
    }
}
