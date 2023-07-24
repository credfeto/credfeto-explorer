using System.Diagnostics;
using Credfeto.Explorer.Server.Models;
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        this._logger.LogError($"Error: {this.HttpContext.TraceIdentifier}");

        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}