using DigitalRightsManagement.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;
using VerifyCs = DigitalRightsManagement.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<DigitalRightsManagement.Analyzers.EntityConstructorAnalyzer>;

namespace DigitalRightsManagement.UnitTests.Analyzers.Analyzers;

public class EntityConstructorAnalyzerTests
{
    private static DiagnosticResult ExpectedDiagnostic(int line, int column, string entityName)
        => new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(line, column, line, column + entityName.Length)
            .WithArguments(entityName);

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
        var expected = ExpectedDiagnostic(line, column, entityName);
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
