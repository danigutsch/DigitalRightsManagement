using DigitalRightsManagement.Common;
using Microsoft.Extensions.Hosting;

namespace DigitalRightsManagement.Infrastructure.Caching;

internal static class CachingDependencyInjection
{
    public static TApplicationBuilder AddCaching<TApplicationBuilder>(this TApplicationBuilder builder) where TApplicationBuilder : IHostApplicationBuilder
    {
        builder.AddRedisDistributedCache(ResourceNames.Cache);

        return builder;
    }
}
