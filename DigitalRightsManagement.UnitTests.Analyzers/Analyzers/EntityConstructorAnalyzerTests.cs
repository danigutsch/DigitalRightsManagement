using DigitalRightsManagement.Analyzers;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityConstructorAnalyzer>;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analyzers;

public sealed class EntityConstructorAnalyzerTests
{
    [Theory]
    [InlineData("""
        using System;
        using DigitalRightsManagement.Common.DDD;
        
        namespace TestNamespace;
        public partial class TestEntity : Entity
        {
            protected TestEntity() { }
        }
        """, 7, 15, "TestEntity")]
    public async Task Reports_Manual_Parameterless_Constructor(string source, int line, int column, string entityName)
    {
        var expected = DiagnosticTestHelper.ExpectedDiagnostic(EntityConstructorAnalyzer.DiagnosticId, line, column, entityName);
        await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass], expected);
    }

    [Theory]
    [InlineData("""
        using System;
        using DigitalRightsManagement.Common.DDD;
        
        namespace TestNamespace;
        public partial class TestEntity : Entity
        {
            public TestEntity(Guid id) : base(id) { }
        }
        """)]
    [InlineData("""
        namespace TestNamespace;
        public class TestClass
        {
            public TestClass() { }
        }
        """)]
    public async Task No_Diagnostic_For_Valid_Cases(string source) => await VerifyCs.VerifyAnalyzerAsync([source, EntityBaseClass]);
}
