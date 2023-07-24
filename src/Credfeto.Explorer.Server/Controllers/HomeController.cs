using System.Collections.Generic;
using System.Diagnostics;
using Credfeto.Explorer.Server.Models;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Credfeto.Explorer.Server.Controllers;

public sealed class HomeController : Controller
{
    private readonly IEthereumNetworkConfigurationManager _ethereumNetworkConfigurationManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager, ILogger<HomeController> logger)
    {
        this._ethereumNetworkConfigurationManager = ethereumNetworkConfigurationManager;
        this._logger = logger;
    }

    public IActionResult Index()
    {
        BaseModel<int> model = new(Networks: this._ethereumNetworkConfigurationManager.EnabledNetworks, Model: 42);

        return this.View(model);
    }

    [Route(template: "{network}")]
    public IActionResult Network(EthereumNetwork network)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

        // lookup network status (endpoints etc)
        // lookup latest block
        // subscribe? to block updates

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: network);

        return this.View(model);
    }

    [Route(template: "{network}/block/{blockNumber}")]
    public IActionResult Block(EthereumNetwork network, long blockNumber)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

        // todo: lookup block by number return block with tx

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: network);

        return this.View(model);
    }

    [Route(template: "{network}/tx/{transactionHash}")]
    public IActionResult Transaction(EthereumNetwork network, TransactionHash transactionHash)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

        // todo: lookup transaction, get receipt, if mined

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: network);

        return this.View(model);
    }

    [Route(template: "{network}/account/{accountAddress}")]
    public IActionResult Account(EthereumNetwork network, AccountAddress accountAddress)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

        // todo: lookup account, get balance, get tokens?

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: network);

        return this.View(model);
    }

    [Route(template: "{network}/token/{contractAddress}")]
    public IActionResult Token(EthereumNetwork network, ContractAddress contractAddress)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

        // todo: lookup contract, get info

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: network);

        return this.View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        this._logger.LogError($"Error: {this.HttpContext.TraceIdentifier}");

        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}