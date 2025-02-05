using DigitalRightsManagement.Analyzers;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework.AnalyzerConstants;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification.AnalyzerVerifier;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analysis;

public class EntityConstructorAnalyzerTests
{
    [Fact]
    public async Task Reports_Manual_Parameterless_Constructor()
    {
        const string source = """
                              using System;
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace;
                              public partial class TestEntity : Entity<Guid>
                              {
                                  protected TestEntity() { }
                              }
                              """;

        var expected = ExpectedDiagnostic("DRM002", 7, 15, "TestEntity");
        await VerifyAnalyzerAsync<EntityConstructorAnalyzer>([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Parameterized_Constructor()
    {
        const string source = """
                              using System;
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace;
                              public partial class TestEntity : Entity<Guid>
                              {
                                  public TestEntity(Guid id) : base(id) { }
                              }
                              """;

        await VerifyAnalyzerAsync<EntityConstructorAnalyzer>([source, EntityBaseClass]);
    }

    [Fact]
    public async Task No_Diagnostic_For_Non_Entity()
    {
        const string source = """
                              namespace TestNamespace;
                              public class TestClass
                              {
                                  public TestClass() { }
                              }
                              """;

        await VerifyAnalyzerAsync<EntityConstructorAnalyzer>([source, EntityBaseClass]);
    }

    [Fact]
    public async Task No_Diagnostic_For_Entity_Without_Constructor()
    {
        const string source = """
                              using System;
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace;
                              public partial class TestEntity : Entity<Guid> { }
                              """;

        await VerifyAnalyzerAsync<EntityConstructorAnalyzer>([source, EntityBaseClass]);
    }
}
