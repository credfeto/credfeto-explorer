using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Explorer.Ethereum.Startup.LoggingExtensions;
using Credfeto.Services.Startup.Interfaces;
using FunFair.Ethereum.Client.Interfaces;
using FunFair.Ethereum.Client.Interfaces.EventArguments;
using FunFair.Ethereum.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Explorer.Ethereum.Startup;

public sealed class EthereumProxyStartup : IRunOnStartup, IDisposable
{
    private readonly IEthereumClientStatus _ethereumClientStatus;
    private readonly ILogger<EthereumProxyStartup> _logger;
    private readonly INetworks _networks;
    private readonly IDisposable _subscription;

    public EthereumProxyStartup(INetworks networks, IEthereumClientStatus ethereumClientStatus, ILogger<EthereumProxyStartup> logger)
    {
        this._networks = networks;
        this._ethereumClientStatus = ethereumClientStatus;
        this._logger = logger;

        this._subscription = Observable.FromEventPattern<NewNetworkBlockEventArgs>(addHandler: h => this._ethereumClientStatus.OnNewLatestBlock += h,
                                                                                   removeHandler: h => this._ethereumClientStatus.OnNewLatestBlock -= h)
                                       .Select(n => Observable.FromAsync(ct => this.OnNewBlockAsync(blockHeader: n.EventArgs.LastBlock)
                                                                                   .AsTask()))
                                       .Concat()
                                       .Subscribe();
    }

    public void Dispose()
    {
        this._subscription.Dispose();
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                IReadOnlyList<EthereumNetwork> networks = await this._networks.GetActiveNetworksAsync(CancellationToken.None);
                this._logger.FoundNetworks(networks.Count);

                return;
            }
            catch (Exception exception)
            {
                this._logger.FailedToGetNetworks(message: exception.Message, exception: exception);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);
        }
    }

    private ValueTask OnNewBlockAsync(INetworkBlockHeader blockHeader)
    {
        this._logger.LogInformation($"{blockHeader.Network} New block {blockHeader.Number.Value} ({blockHeader.Hash})");

        return ValueTask.CompletedTask;
    }
}