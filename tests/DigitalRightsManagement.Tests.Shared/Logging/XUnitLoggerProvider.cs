using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DigitalRightsManagement.Tests.Shared.Logging;

internal sealed class XUnitLoggerProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{
    private readonly LoggerExternalScopeProvider _scopeProvider = new();

    public ILogger CreateLogger(string categoryName) => new XUnitLogger(testOutputHelper, _scopeProvider, categoryName);

    public void Dispose() { }
}