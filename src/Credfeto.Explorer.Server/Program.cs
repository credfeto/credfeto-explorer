using System;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Explorer.Server.Controllers;
using Credfeto.Explorer.Server.ServiceStartup;
using FunFair.Common.Environment;
using FunFair.Common.Middleware;
using FunFair.Common.Middleware.Validation;
using FunFair.Common.Server;
using FunFair.Common.Server.ServiceStartup;
using FunFair.Common.Services;
using FunFair.Ethereum.Proxy.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                                                        serviceBuilder: ConfigureServices,
                                                        appBuilder: ConfigureApp))
        {
            ILogger? logger = null;

            try
            {
                //await ApplicationSetup.StartupAsync(serviceProvider: host.Services, cancellationToken: CancellationToken.None);
                logger = host.Services.GetRequiredService<ILogger<IHost>>();

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

    private static void ConfigureServices(IServiceCollection services)
    {
        const ExecutionEnvironment executionEnvironment = ExecutionEnvironment.LOCAL;

        Logging.ConfigureLogging(services: services, environment: executionEnvironment);

        services.AddDotNetMemoryCaching()
                .AddCors()
                .AddControllersWithViews()
                .AddApplicationPart(typeof(HomeController).Assembly)
                .Services.AddSingleton<ValidationActionFilter>()
                .Configure<GzipCompressionProviderOptions>(configureOptions: options => options.Level = CompressionLevel.Fastest)
                .Configure<BrotliCompressionProviderOptions>(configureOptions: options => options.Level = CompressionLevel.Fastest)
                .AddResponseCompression(configureOptions: InitialiseResponseCompression)
                .AddHttpResponseCaching(executionEnvironment)
                .AddMvc(setupAction: options => options.Filters.AddService(typeof(ValidationActionFilter)))
                .AddMvcOptions(setupAction: _ =>
                                            {
                                                // Note Additional ModelMetadata providers that require DI are enabled elsewhere
                                            })
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(configure: options => JsonSerialiser.Configure(options.JsonSerializerOptions))
                .Services.AddHttpContextAccessor()
                // .AddFluentValidationAutoValidation()
                //.AddValidatorsFromAssemblyContaining<EthereumAddressBulkDtoValidator>(ServiceLifetime.Singleton)
                // .ConfigureSwaggerServices(version: this._configuration.Version)
                .AddRouting();
    }

    private static void ConfigureApp(IApplicationBuilder app)
    {
        app = app.UseDefaultToNoResponseCachingMiddleware()
                 .UseResponseCompression()
                 .UseErrorHandling()
                 .UseHttpStrictTransportSecurity()
                 .UseServerNameHeader()
                 .UseRequestIpAddressMiddleware()
                 .UseForwardedHeaders(new() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost })
                 .UseRouting()
                 .UseEndpoints(configure: endpoints => { endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}"); });

        ApplicationInsights.DisableTelemetry(app);
    }

    private static void InitialiseResponseCompression(ResponseCompressionOptions options)
    {
        string[] compressibleMimeTypes =
        {
            "image/svg+xml"
        };

        options.EnableForHttps = true;

        // Explicitly enable Gzip
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();

        // Add Custom mime types
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(compressibleMimeTypes);
    }
}