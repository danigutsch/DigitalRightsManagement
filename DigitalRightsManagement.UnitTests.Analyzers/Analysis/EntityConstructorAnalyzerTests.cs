using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework.AnalyzerConstants;
using static DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework.DiagnosticTestHelper;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.Verification.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityConstructorAnalyzer>;

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
                              public partial class TestEntity : Entity
                              {
                                  protected TestEntity() { }
                              }
                              """;

        var expected = ExpectedDiagnostic("DRM002", 7, 15, "TestEntity");
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }

    [Fact]
    public async Task No_Diagnostic_For_Parameterized_Constructor()
    {
        const string source = """
                              using System;
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace;
                              public partial class TestEntity : Entity
                              {
                                  public TestEntity(Guid id) : base(id) { }
                              }
                              """;

        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
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

        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }

    [Fact]
    public async Task No_Diagnostic_For_Entity_Without_Constructor()
    {
        const string source = """
                              using System;
                              using DigitalRightsManagement.Common.DDD;

                              namespace TestNamespace;
                              public partial class TestEntity : Entity { }
                              """;

        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
    }
}
