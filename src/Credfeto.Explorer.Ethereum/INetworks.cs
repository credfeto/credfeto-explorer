using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace Credfeto.Explorer.Ethereum;

public interface INetworks
{
    ValueTask<IReadOnlyList<EthereumNetwork>> GetActiveNetworksAsync(CancellationToken cancellationToken);
}