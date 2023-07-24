using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    [Route(template: "{networkName}")]
    public IActionResult Network(string networkName)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;
        EthereumNetwork? match = networks.FirstOrDefault(predicate: n => StringComparer.InvariantCultureIgnoreCase.Equals(x: n.Name, y: networkName));

        if (match is null)
        {
            return this.NotFound();
        }

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: match);

        return this.View(model);
    }

    [Route(template: "{networkName}/block/{blockNumber}")]
    public IActionResult Block(string networkName, BlockNumber blockNumber)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;
        EthereumNetwork? match = networks.FirstOrDefault(predicate: n => StringComparer.InvariantCultureIgnoreCase.Equals(x: n.Name, y: networkName));

        if (match is null)
        {
            return this.NotFound();
        }

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: match);

        return this.View(model);
    }

    [Route(template: "{networkName}/tx/{transactionHash}")]
    public IActionResult Transaction(string networkName, TransactionHash transactionHash)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;
        EthereumNetwork? match = networks.FirstOrDefault(predicate: n => StringComparer.InvariantCultureIgnoreCase.Equals(x: n.Name, y: networkName));

        if (match is null)
        {
            return this.NotFound();
        }

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: match);

        return this.View(model);
    }

    [Route(template: "{networkName}/account/{accountAddress}")]
    public IActionResult Account(string networkName, AccountAddress accountAddress)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;
        EthereumNetwork? match = networks.FirstOrDefault(predicate: n => StringComparer.InvariantCultureIgnoreCase.Equals(x: n.Name, y: networkName));

        if (match is null)
        {
            return this.NotFound();
        }

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: match);

        return this.View(model);
    }

    [Route(template: "{networkName}/token/{contractAddress}")]
    public IActionResult Token(string networkName, ContractAddress contractAddress)
    {
        IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;
        EthereumNetwork? match = networks.FirstOrDefault(predicate: n => StringComparer.InvariantCultureIgnoreCase.Equals(x: n.Name, y: networkName));

        if (match is null)
        {
            return this.NotFound();
        }

        BaseModel<EthereumNetwork> model = new(Networks: networks, Model: match);

        return this.View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        this._logger.LogError($"Error: {this.HttpContext.TraceIdentifier}");

        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}