using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Verifiers;

public sealed class CSharpCodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public CSharpCodeFixTest()
    {
        TestState.AdditionalReferences.Add(typeof(IAggregateRoot).Assembly);
        SolutionTransforms.Add((solution, projectId) =>
        {
            var project = solution.GetProject(projectId);
            if (project is null)
            {
                return solution;
            }

            var parseOptions = project.ParseOptions;
            if (parseOptions is null)
            {
                return solution;
            }

            return solution.WithProjectParseOptions(projectId, parseOptions);
        });
    }
}
