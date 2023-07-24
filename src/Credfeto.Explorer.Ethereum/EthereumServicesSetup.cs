using Credfeto.Explorer.Ethereum.Services;
using Credfeto.Explorer.Ethereum.Startup;
using Credfeto.Services.Startup.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Ethereum;

public static class EthereumServicesSetup
{
    public static IServiceCollection AddEthereumLookupServices(this IServiceCollection services)
    {
        return services.AddRunOnStartupTask<EthereumProxyStartup>()
                       .AddSingleton<INetworks, Networks>();
    }
}