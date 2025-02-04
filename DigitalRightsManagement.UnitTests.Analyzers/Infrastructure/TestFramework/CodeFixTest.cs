using DigitalRightsManagement.Common.DDD;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;

/// <summary>
/// Represents a test for code fixes, inheriting from <see cref="CSharpCodeFixTest{TAnalyzer, TCodeFix, DefaultVerifier}"/>.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the diagnostic analyzer.</typeparam>
/// <typeparam name="TCodeFix">The type of the code fix provider.</typeparam>
public sealed class CodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeFixTest{TAnalyzer, TCodeFix}"/> class.
    /// </summary>
    public CodeFixTest()
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
            return parseOptions is null
                ? solution :
                solution.WithProjectParseOptions(projectId, parseOptions);
        });
    }
}
