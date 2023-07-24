#define EVER_THINK_ABOUT_TO_TURN_ON_RESPONSE_CACHING
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Server.ServiceStartup;

internal static class ResponseCaching
{
    [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Placeholder")]
    public static IServiceCollection AddHttpResponseCaching(this IServiceCollection services, ExecutionEnvironment configurationEnvironment)
    {
        if (!configurationEnvironment.IsLocalDevelopmentOrTest())
        {
            return services;
        }

        // Response caching deliberately disabled as on lowish memory boxes these responses can push the memory usage up insanely
        // This method deliberately here to document why.
#if EVER_THINK_ABOUT_TO_TURN_ON_RESPONSE_CACHING
        static void DefineCacheOptions(ResponseCachingOptions options)
        {
            const int maxBodySizeBytes = 16384;
            const int maxCacheSizeLimitMb = 64;

            // restrict on what max size of response
            // can be cached in bytes
            options.MaximumBodySize = maxBodySizeBytes;
            options.UseCaseSensitivePaths = false;
            options.SizeLimit = 1024 * 1024 * maxCacheSizeLimitMb;
        }

        return services.AddResponseCaching(DefineCacheOptions);
#else
        return services;
#endif
    }
}