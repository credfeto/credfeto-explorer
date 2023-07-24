using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Explorer.Ethereum.Services.LoggingExtensions;
using FunFair.Ethereum.Client.Interfaces;
using FunFair.Ethereum.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Explorer.Ethereum.Services;

public sealed class Networks : INetworks
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ILogger<Networks> _logger;
    private readonly SemaphoreSlim _semaphore;
    private readonly IWeb3Factory _web3Factory;
    private IReadOnlyList<EthereumNetwork> _ethereumNetworks;
    private DateTimeOffset _lastUpdated;

    public Networks(IWeb3Factory web3Factory, ICurrentTimeSource currentTimeSource, ILogger<Networks> logger)
    {
        this._web3Factory = web3Factory;
        this._currentTimeSource = currentTimeSource;
        this._logger = logger;

        this._semaphore = new(1);
        this._lastUpdated = DateTimeOffset.MinValue;
        this._ethereumNetworks = Array.Empty<EthereumNetwork>();
    }

    public async ValueTask<IReadOnlyList<EthereumNetwork>> GetActiveNetworksAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset now = this._currentTimeSource.UtcNow();
        TimeSpan elapsed = now - this._lastUpdated;

        if (elapsed.TotalMinutes < 5)
        {
            return this._ethereumNetworks;
        }

        await this._semaphore.WaitAsync(cancellationToken);

        try
        {
            elapsed = now - this._lastUpdated;

            if (elapsed.TotalMinutes < 5)
            {
                return this._ethereumNetworks;
            }

            this._logger.RefreshingNetworks(elapsed.TotalSeconds);
            this._ethereumNetworks = await this.GetNetworksAsync(cancellationToken);
        }
        finally
        {
            this._lastUpdated = now;
            this._semaphore.Release();
        }

        return this._ethereumNetworks;
    }

    private async ValueTask<IReadOnlyList<EthereumNetwork>> GetNetworksAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<EthereumNetwork> networks = await this._web3Factory.GetNetworksAsync(cancellationToken);

        return networks.Where(n => n.IsProductionNetwork)
                       .ToArray();
    }
}