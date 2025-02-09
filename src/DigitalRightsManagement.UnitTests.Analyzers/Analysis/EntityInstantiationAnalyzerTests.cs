using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification.AnalyzerVerifier;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analysis;

public sealed class EntityInstantiationAnalyzerTests
{
    [Fact]
    public async Task Reports_Entity_Parameterless_Instantiation()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public class TestEntity : Entity<Guid>
                                  {
                                      public TestEntity() { }
                                  }

                                  public class Program
                                  {
                                      public void Main()
                                      {
                                          var entity = new TestEntity();
                                      }
                                  }
                              }
                              """;

        var expected = new DiagnosticResult(EntityInstantiationAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(15, 26, 15, 42)
            .WithArguments("TestEntity");

        // Act & Assert
        await VerifyAnalyzerAsync<EntityInstantiationAnalyzer>([source], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Parameterized_Instantiation()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public class TestEntity : Entity<Guid>
                                  {
                                      public TestEntity(Guid id) : base(id) { }
                                  }

                                  public class Program
                                  {
                                      public void Main()
                                      {
                                          var entity = new TestEntity(Guid.NewGuid());
                                      }
                                  }
                              }
                              """;

        // Act & Assert
        await VerifyAnalyzerAsync<EntityInstantiationAnalyzer>([source]);
    }
}
