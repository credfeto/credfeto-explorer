using System.Collections.Generic;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes;

namespace Credfeto.Explorer.Server.Models;

[DebuggerDisplay("Networks: {Networks.Count}")]
public readonly record struct BaseModel<TModel>(IReadOnlyList<EthereumNetwork> Networks, TModel Model) : IBaseModel;