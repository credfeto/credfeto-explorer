using FunFair.Common.Environment;
using FunFair.Common.Server.ServiceStartup;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Server.ServiceStartup;

internal static class App
{
    public static void ConfigureServices(IServiceCollection services)
    {
        const ExecutionEnvironment executionEnvironment = ExecutionEnvironment.LOCAL;

        Logging.ConfigureLogging(services: services, environment: executionEnvironment);

        services.AddSingleton(typeof(ExecutionEnvironment), implementationInstance: executionEnvironment)
                .AddDotNetMemoryCaching()
                .AddWebApp(executionEnvironment);
    }
}