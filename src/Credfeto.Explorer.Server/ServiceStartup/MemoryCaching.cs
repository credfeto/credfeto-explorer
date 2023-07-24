using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Server.ServiceStartup;

internal static class MemoryCaching
{
    private const int CACHE_SIZE_MB = 128;

    public static IServiceCollection AddDotNetMemoryCaching(this IServiceCollection services)
    {
        return services.AddMemoryCache(x => x.SizeLimit = 1024 * 1024 * CACHE_SIZE_MB);
    }
}