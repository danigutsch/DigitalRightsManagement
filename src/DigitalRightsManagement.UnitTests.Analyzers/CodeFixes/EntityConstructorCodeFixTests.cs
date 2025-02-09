using DigitalRightsManagement.Analyzers;
using DigitalRightsManagement.CodeFixes;
using DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.CodeFixes;

#pragma warning disable S2699

public sealed class EntityConstructorCodeFixTests
{
    [Fact]
    public async Task Removes_Manual_Constructor_In_Non_Sealed_Class()
    {
        const string source = """
                              using DigitalRightsManagement.Common.DDD;
                              using System;

                              namespace TestNamespace
                              {
                                  public partial class TestEntity : Entity<Guid>
                                  {
                                      protected TestEntity() { }
                                  }
                              }
                              """;

        var test = new CodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources =
                {
                    """
                    using DigitalRightsManagement.Common.DDD;
                    using System;

                    namespace TestNamespace
                    {
                        public partial class TestEntity : Entity<Guid>
                        {
                        }
                    }
                    """
                }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                DiagnosticResult.CompilerError("DRM002")
                    .WithSpan(8, 19, 8, 29)
                    .WithArguments("TestEntity")
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task Removes_Manual_Constructor_In_Sealed_Class()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;
            using System;
            
            namespace TestNamespace
            {
                public sealed partial class TestEntity : Entity<Guid>
                {
                    private TestEntity() { }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;
            using System;
            
            namespace TestNamespace
            {
                public sealed partial class TestEntity : Entity<Guid>
                {
                }
            }
            """;

        var test = new CodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(8, 17, 8, 27)
                    .WithArguments("TestEntity")
            }
        };

        // Act & Assert
        await test.RunAsync();
    }

    [Fact]
    public async Task Removes_Manual_Constructor_With_Nested_Body()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;
            using System;
            
            namespace TestNamespace
            {
                public partial class TestEntity : Entity<Guid>
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
            using System;
            
            namespace TestNamespace
            {
                public partial class TestEntity : Entity<Guid>
                {
                }
            }
            """;

        var test = new CodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(8, 19, 8, 29)
                    .WithArguments("TestEntity")
            }
        };

        // Act & Assert
        await test.RunAsync();
    }

    [Fact]
    public async Task Updates_Class_With_Multiple_Constructors()
    {
        // Arrange
        const string source = """
            using DigitalRightsManagement.Common.DDD;
            using System;
            
            namespace TestNamespace
            {
                public partial class TestEntity : Entity<Guid>
                {
                    protected TestEntity() { }
                    public TestEntity(string name) { }
                }
            }
            """;

        const string fixedSource = """
            using DigitalRightsManagement.Common.DDD;
            using System;
            
            namespace TestNamespace
            {
                public partial class TestEntity : Entity<Guid>
                {
                    public TestEntity(string name) { }
                }
            }
            """;

        var test = new CodeFixTest<EntityConstructorAnalyzer, EntityConstructorCodeFix>
        {
            TestCode = source,
            FixedState =
            {
                Sources = { fixedSource }
            },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            ExpectedDiagnostics =
            {
                new DiagnosticResult(EntityConstructorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(8, 19, 8, 29)
                    .WithArguments("TestEntity")
            }
        };

        // Act & Assert
        await test.RunAsync();
    }
}
