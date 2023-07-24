using System;
using FunFair.Common.Services;
using FunFair.Ethereum.BloomFilters;
using FunFair.Ethereum.Crypto;
using FunFair.Ethereum.Crypto.Managed;
using FunFair.Ethereum.Proxy.Client;
using FunFair.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Explorer.Server.ServiceStartup;

public static class EthereumServices
{
#if DEBUG
    private static readonly Uri ServerUri = new("https://dev.proxy.ethereum.funfair.io");
#else
    private static readonly Uri ServerUri = new("https://proxy.ethereum.funfair.io");
#endif

    public static IServiceCollection AddEthereumServices(this IServiceCollection services)
    {
        return services.AddRandomNumbers()
                       .AddCommonServices()
                       .AddEthereumBloomFilters()
                       .AddEthereumCrypto<Secp256K1ManagedDigitalSignatureAlgorithm>()
                       .AddEthereumProxyNetworksDynamicEnableAllAvailable()
                       .AddEthereumProxyClient(proxyServerBaseUrl: ServerUri);
    }
}