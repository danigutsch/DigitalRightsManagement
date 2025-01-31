using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityInstantiationAnalyzer>;

namespace DigitalRightsManagement.UnitTests.Analyzers;

public sealed class EntityInstantiationAnalyzerTests
{
    [Fact]
    public async Task Reports_Entity_Parameterless_Instantiation()
    {
        // Arrange
        const string source = """
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace
                              {
                                  public class TestEntity : Entity
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
            .WithSpan(14, 26, 14, 42)
            .WithArguments("TestEntity");

        // Act & Assert
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
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
                                  public class TestEntity : Entity
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
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }
}