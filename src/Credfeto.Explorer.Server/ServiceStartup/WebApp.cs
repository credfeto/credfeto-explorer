using System.IO.Compression;
using System.Linq;
using Credfeto.Explorer.Server.Controllers;
using FunFair.Common.Environment;
using FunFair.Common.Middleware;
using FunFair.Common.Middleware.Validation;
using FunFair.Common.Server.ServiceStartup;
using FunFair.Ethereum.Proxy.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Server.ServiceStartup;

internal static class WebApp
{
    public static IServiceCollection AddWebApp(this IServiceCollection services, ExecutionEnvironment executionEnvironment)
    {
        return services.AddCors()
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

    public static void Configure(IApplicationBuilder app)
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
}