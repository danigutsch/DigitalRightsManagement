using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace DigitalRightsManagement.Infrastructure.Endpoints;

/// <summary>
/// Extension method to register all endpoints from IEndpointModule implementations.
/// </summary>
public static class EndpointDependencyInjection
{
    /// <summary>
    /// Maps all endpoint modules found in the specified assemblies
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="assembly">The first assembly to scan for endpoint modules</param>
    /// <param name="additionalAssemblies">Additional assemblies to scan for endpoint modules</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapEndpointModules(
        this WebApplication app,
        Assembly assembly,
        params Assembly[] additionalAssemblies)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblies = new[] { assembly }.Concat(additionalAssemblies ?? []);
        var endpointModules = DiscoverEndpointModules(assemblies);

        foreach (var module in endpointModules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }

    private static IEnumerable<IEndpointModule> DiscoverEndpointModules(IEnumerable<Assembly> assemblies)
    {
        var moduleType = typeof(IEndpointModule);

        return assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => moduleType.IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpointModule>();
    }
}
