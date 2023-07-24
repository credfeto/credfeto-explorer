using System.Collections.Generic;
using FunFair.Ethereum.DataTypes;

namespace Credfeto.Explorer.Server.Models;

public interface IBaseModel
{
    IReadOnlyList<EthereumNetwork> Networks { get; }
}