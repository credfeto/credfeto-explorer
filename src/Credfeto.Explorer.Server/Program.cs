using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Explorer.Server.ServiceStartup;
using FunFair.Common.Environment;
using FunFair.Common.Server;
using FunFair.Common.Server.ServiceStartup;
using FunFair.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Explorer.Server;

internal static class Program
{
    private const int MIN_THREADS = 32;

    public static async Task Main(string[] args)
    {
        ServerStartup.SetThreads(MIN_THREADS);

        string configurationFilesPath = ApplicationConfigLocator.ConfigurationFilesPath;

        using (IHost host = ServerStartup.CreateWebHost(args: args,
                                                        httpPort: 7000,
                                                        httpsPort: 7001,
                                                        h2Port: 7002,
                                                        configurationFilesPath: configurationFilesPath,
                                                        serviceBuilder: App.ConfigureServices,
                                                        appBuilder: WebApp.Configure))
        {
            ILogger? logger = null;

            try
            {
                IServiceProvider serviceProvider = host.Services;
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ExecutionEnvironment executionEnvironment = serviceProvider.GetRequiredService<ExecutionEnvironment>();
                IOptions<LoggingConfiguration> logging = serviceProvider.GetRequiredService<IOptions<LoggingConfiguration>>();

                Logging.InitializeLogging(environment: executionEnvironment, loggerFactory: loggerFactory, configuration: logging.Value, typeof(Program).Namespace!, version: "1.0", tenant: "");

                //await ApplicationSetup.StartupAsync(serviceProvider: host.Services, cancellationToken: CancellationToken.None);
                logger = serviceProvider.GetRequiredService<ILogger<IHost>>();

                await host.RunAsync(CancellationToken.None);
            }
            catch (Exception exception)
            {
                if (logger is not null)
                {
                    logger.LogCritical(new(exception.HResult), exception: exception, message: "Service going down due to fatal exception!");
                }
                else
                {
                    await Console.Error.WriteLineAsync($"Service going down due to fatal exception! : {exception.Message}");
                }
            }
        }
    }
}