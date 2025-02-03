using DigitalRightsManagement.Analyzers;
using DigitalRightsManagement.CodeFixes;
using DigitalRightsManagement.UnitTests.Analyzers.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using static DigitalRightsManagement.UnitTests.Analyzers.AnalyzerConstants;

namespace DigitalRightsManagement.UnitTests.Analyzers.CodeFixes;

#pragma warning disable S2699

public sealed class EntityConstructorCodeFixTests
{
    [Fact]
    public async Task Removes_Manual_Constructor_In_Non_Sealed_Class()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    protected TestEntity() { }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                }
            }
            """;

        var test = new CSharpCodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource, EntityBaseClass }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(7, 19, 7, 29)
                    .WithArguments("TestEntity")
            }
        };

        test.TestState.Sources.Add(EntityBaseClass);

        // Act & Assert
        await test.RunAsync();
    }

    [Fact]
    public async Task Removes_Manual_Constructor_In_Sealed_Class()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public sealed partial class TestEntity : Entity
                {
                    private TestEntity() { }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public sealed partial class TestEntity : Entity
                {
                }
            }
            """;

        var test = new CSharpCodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource, EntityBaseClass }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(7, 17, 7, 27)
                    .WithArguments("TestEntity")
            }
        };

        test.TestState.Sources.Add(EntityBaseClass);

        // Act & Assert
        await test.RunAsync();
    }

    [Fact]
    public async Task Removes_Manual_Constructor_With_Nested_Body()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    protected TestEntity()
                    {
                        var x = 1;
                        if (x > 0)
                        {
                            x++;
                        }
                    }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                }
            }
            """;

        var test = new CSharpCodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource, EntityBaseClass }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(7, 19, 7, 29)
                    .WithArguments("TestEntity")
            }
        };

        test.TestState.Sources.Add(EntityBaseClass);

        // Act & Assert
        await test.RunAsync();
    }

    [Fact]
    public async Task Updates_Class_With_Multiple_Constructors()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    protected TestEntity() { }
                    public TestEntity(string name) { }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;

            namespace TestNamespace
            {
                public partial class TestEntity : Entity
                {
                    public TestEntity(string name) { }
                }
            }
            """;

        var test = new CSharpCodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource, EntityBaseClass }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(7, 19, 7, 29)
                    .WithArguments("TestEntity")
            }
        };

        test.TestState.Sources.Add(EntityBaseClass);

        // Act & Assert
        await test.RunAsync();
    }
}
